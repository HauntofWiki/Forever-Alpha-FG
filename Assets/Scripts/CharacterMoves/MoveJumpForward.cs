using UnityEngine;

namespace CharacterMoves
{
    public class MoveJumpForward : CharacterMove
    {
        private CharacterProperties _properties;

        //Tracks invincibility States per frame.
        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
        public int[] InvincibilyFrames =
        {
            4, 4, 4, 0, 0, 0
        };

        //Tracks attack properties of the move per frame.
        //0:StartUp, 1:Active, 2:Recovery 3:End
        public int[] AttackStateFrames =
        {
            0, 0, 0, 1, 2, 2, 2, 3
        };

        //Tracks Cancellable states of the move per frame.
        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
        public int[] Cancellability =
        {
            0
        };

        //Tracks Airborne State of move per frame
        //0: No, 1:Yes
        public int[] AirborneState =
        {
            0, 0, 0, 1, 0, 0, 0
        };

        public override void InitializeMove(ref CharacterProperties properties)
        {
            _properties = properties;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 9;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            //Use Character controller to determine if animation have reached the ground
            if (_properties.CurrentState == CharacterProperties.CharacterState.JumpForward &&
                _properties.CharacterController.isGrounded && AttackStateFrames[_properties.JumpFrameCounter] != 0)
            {
                _properties.JumpFrameCounter++;
                _properties.LastState = _properties.CurrentState;
                _properties.CurrentState = CharacterProperties.CharacterState.LandingJumpForward;
            }

            //Advance Jump Frame Counter and assess Startup/Recovery
            if (_properties.CurrentState == CharacterProperties.CharacterState.JumpForward &&
                AttackStateFrames[_properties.JumpFrameCounter] == 0)
            {
                _properties.JumpFrameCounter++;
                if (AttackStateFrames[_properties.JumpFrameCounter] == 0)
                {
                    _properties.LastState = _properties.CurrentState;
                    return;
                }

                if (AttackStateFrames[_properties.JumpFrameCounter] == 1)
                {
                    _properties.LastState = _properties.CurrentState;
                    _properties.IsAirborne = true;
                    _properties.MoveDirection = new Vector3(_properties.WalkForwardXSpeed, _properties.JumpYSpeed, 0);
                }
            }

            if (_properties.CurrentState == CharacterProperties.CharacterState.LandingJumpForward)
            {
                _properties.JumpFrameCounter++;
                if (AttackStateFrames[_properties.JumpFrameCounter] == 3)
                {
                    _properties.JumpFrameCounter = 0;
                    _properties.LastState = _properties.CurrentState;
                    _properties.IsAirborne = false;
                    _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                }
            }
            
            //Detect proper state and detect input
            if (_properties.CurrentState == CharacterProperties.CharacterState.JumpForward && AttackStateFrames[_properties.JumpFrameCounter] == 1)
            {
                _properties.MoveDirection.x = _properties.WalkForwardXSpeed;
                return;
            }

            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState != CharacterProperties.CharacterState.Crouch && _properties.CurrentState != CharacterProperties.CharacterState.Stand && _properties.CurrentState != CharacterProperties.CharacterState.CancellableAnimation) return;
        
            _properties.JumpFrameCounter = 0;
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.JumpForward;
        }
    }
}

