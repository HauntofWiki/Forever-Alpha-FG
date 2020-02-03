using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveJumpBackward : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        private FrameDataHandler _frameData;

//        //Tracks invincibility States per frame.
//        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
//        public int[] InvincibilyFrames =
//        {
//            4,4,4,0,0,0
//        };
//        
//        //Tracks attack properties of the move per frame.
//        //0:StartUp, 1:Active, 2:Recovery 3:End
//        public int[] AttackStateFrames =
//        {
//            0,0,0,1,2,2,2,3
//        };
//        
//        //Tracks Cancellable states of the move per frame.
//        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
//        public int[] Cancellability =
//        {
//            0
//        };
//        
//        //Tracks Airborne State of move per frame
//        //0: No, 1:Yes
//        public int[] AirborneState =
//        {
//            0,0,0,1,0,0,0
//        };

        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            _animator = animator;
            _properties = properties;
            _frameData = new FrameDataHandler(7);
            _frameData.SetFieldsZero();
            //For jumps length isnt important to how they function
            _frameData.SetAttackFrames(3, 1);
            _frameData.SetAirborneFrames(0, 6);
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 7;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            Debug.Log(_properties.CurrentState + ", " + _frameData.AttackFrameState[_properties.JumpFrameCounter]);
            //Use Character controller to determine if animation has reached the ground
            if (_properties.CurrentState == CharacterProperties.CharacterState.JumpBackward &&
                _properties.CharacterController.isGrounded &
                _frameData.AttackFrameState[_properties.JumpFrameCounter] != FrameDataHandler.AttackFrameStates.Startup)
            {
                _properties.JumpFrameCounter++;
                _properties.LastState = _properties.CurrentState;
                _properties.CurrentState = CharacterProperties.CharacterState.LandingJumpBackward;
                _frameData.Update(_properties.JumpFrameCounter);
            }

            //Advance Jump Frame Counter and assess Startup/Recovery
            if (_properties.CurrentState == CharacterProperties.CharacterState.JumpBackward)
            {
                if (_frameData.AttackFrameState[_properties.JumpFrameCounter] ==
                    FrameDataHandler.AttackFrameStates.Startup)
                {
                    _properties.JumpFrameCounter++;
                    return;
                }

                if (_frameData.AttackFrameState[_properties.JumpFrameCounter] ==
                    FrameDataHandler.AttackFrameStates.Active)
                {
                    _properties.IsAirborne = true;
                    _properties.MoveDirection = new Vector3(-_properties.WalkBackwardXSpeed, _properties.JumpYSpeed, 0);
                }

                _frameData.Update(_properties.JumpFrameCounter);
            }

            if (_properties.CurrentState == CharacterProperties.CharacterState.LandingJumpBackward)
            {
                _properties.JumpFrameCounter++;
                if (_frameData.AttackFrameState[_properties.JumpFrameCounter] !=
                    FrameDataHandler.AttackFrameStates.Recovery)
                {
                    _properties.JumpFrameCounter = 0;
                    _properties.LastState = _properties.CurrentState;
                    _properties.IsAirborne = false;
                    _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                }
            }

            //Detect proper state and detect input
            if (_properties.CurrentState == CharacterProperties.CharacterState.JumpBackward &&
                _frameData.AttackFrameState[_properties.JumpFrameCounter] != FrameDataHandler.AttackFrameStates.Active)
            {
                _properties.MoveDirection.x = -_properties.WalkBackwardXSpeed;
                return;
            }

            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState != CharacterProperties.CharacterState.Crouch &&
                _properties.CurrentState != CharacterProperties.CharacterState.Stand && _properties.CurrentState !=
                CharacterProperties.CharacterState.CancellableAnimation) return;

            _properties.JumpFrameCounter = 0;
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.JumpBackward;
        }
    }
}