using System.Transactions;
using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveJumpForward : CharacterMove
    {
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Animator = animator;
            Properties = properties;
            FrameData = new FrameDataHandler(7);
            FrameData.SetFieldsZero();
            //For jumps length isnt important to how they function
            FrameData.SetAttackFrames(3, 1);
            FrameData.SetAirborneFrames(0, 6);
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
            //Advance Jump Frame Counter and assess Startup/Recovery
            if (Properties.CurrentState == CharacterProperties.CharacterState.JumpForward && 
                FrameData.AttackFrameState[Properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Startup)
            {
                if (FrameData.AttackFrameState[Properties.JumpFrameCounter] ==
                    FrameDataHandler.AttackFrameStates.Startup)
                {
                    Properties.JumpFrameCounter++;
                    FrameData.Update(Properties.JumpFrameCounter);
                    return;
                }

                if (FrameData.AttackFrameState[Properties.JumpFrameCounter] ==
                    FrameDataHandler.AttackFrameStates.Active)
                {
                    Properties.JumpFrameCounter++;
                    Properties.LastState = Properties.CurrentState;
                    Properties.IsAirborne = true;
                    Properties.MoveDirection = new Vector3(Properties.WalkForwardXSpeed, Properties.JumpYSpeed, 0);
                    return;
                }
            }
            
            //Continue to apply forward motion until landing
            if (Properties.CurrentState == CharacterProperties.CharacterState.JumpForward)
            {
                Debug.Log(Properties.MoveDirection.x);
                Properties.MoveDirection.x = Properties.WalkForwardXSpeed;
            }
            
            //Use Character controller to determine if animation has reached the ground
            if (Properties.CurrentState == CharacterProperties.CharacterState.JumpForward &&
                Properties.CharacterController.isGrounded &
                FrameData.AttackFrameState[Properties.JumpFrameCounter] != FrameDataHandler.AttackFrameStates.Startup)
            {
                Properties.JumpFrameCounter++;
                Properties.LastState = Properties.CurrentState;
                Properties.CurrentState = CharacterProperties.CharacterState.LandingJumpForward;
                FrameData.Update(Properties.JumpFrameCounter);
            }
            
            if (Properties.CurrentState == CharacterProperties.CharacterState.LandingJumpForward)
            {
                Properties.JumpFrameCounter++;
                if (FrameData.AttackFrameState[Properties.JumpFrameCounter] !=
                    FrameDataHandler.AttackFrameStates.Recovery)
                {
                    Properties.JumpFrameCounter = 0;
                    Properties.LastState = Properties.CurrentState;
                    Properties.IsAirborne = false;
                    Properties.CurrentState = CharacterProperties.CharacterState.Stand;
                }
            }
            
            //Detect proper state and detect input
            if (!DetectMoveInput(inputClass)) return;
            if (Properties.CurrentState != CharacterProperties.CharacterState.Crouch &&
                Properties.CurrentState != CharacterProperties.CharacterState.Stand && Properties.CurrentState !=
                CharacterProperties.CharacterState.CancellableAnimation) return;
            Animator.Play("JumpForward");
            Properties.JumpFrameCounter = 0;
            Properties.LastState = Properties.CurrentState;
            Properties.CurrentState = CharacterProperties.CharacterState.JumpForward;
        }
    }
}

