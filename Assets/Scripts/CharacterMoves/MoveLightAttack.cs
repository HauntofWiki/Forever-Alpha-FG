﻿namespace CharacterMoves
{
    public class MoveLightAttack : CharacterMove
    {
        private CharacterProperties _properties;
        
        //Tracks invincibility States per frame.
        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
        public int[] InvincibilyFrames =
        {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };
        
        //Tracks attack properties of the move per frame.
        //0:StartUp, 1:Active, 2:Recovery 3:End
        public int[] AttackStateFrames =
        {
            0,0,0,1,1,1,1,1,2,2,2,2,2,2,2,2,2,3
        };
        
        //Tracks Cancellable states of the move per frame.
        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
        //5:JumpCancel, 6:AirJumpCancel, 7:DashCancel, 8:AirDashCancel
        public int[] Cancellability =
        {
            0,0,0,2,2,2,2,2,2,2,2,2,2,2,2
        };
        
        //Tracks Airborne State of move per frame
        //0: No, 1:Yes
        public int[] AirborneState =
        {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };

        public override void InitializeMove(ref CharacterProperties properties)
        {
            _properties = properties;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.LightAttackButtonDown == 1;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            if (_properties.CurrentState == CharacterProperties.CharacterState.Stand && DetectMoveInput(inputClass) &&
                _properties.AttackState == CharacterProperties.AttackStates.None)
            {
                _properties.AttackFrameCounter = 0;
                //_animator.Play("LightAttack");
                _properties.AttackState = CharacterProperties.AttackStates.LightAttack;
            }

            if (_properties.AttackState == CharacterProperties.AttackStates.LightAttack)
            {
                _properties.AttackFrameCounter++;
                switch (Cancellability[_properties.AttackFrameCounter])
                {
                    case 0:
                        _properties.CancellableState = CharacterProperties.CancellableStates.None;
                        break;
                    case 1:
                        _properties.CancellableState = CharacterProperties.CancellableStates.EmptyCancellable;
                        break;
                    case 2:
                        _properties.CancellableState = CharacterProperties.CancellableStates.NormalCancellable;
                        break;
                    case 3:
                        _properties.CancellableState = CharacterProperties.CancellableStates.SpecialCancellable;
                        break;
                    case 4:
                        _properties.CancellableState = CharacterProperties.CancellableStates.SuperCancellable;
                        break;
                }
            }
        }
    }
}