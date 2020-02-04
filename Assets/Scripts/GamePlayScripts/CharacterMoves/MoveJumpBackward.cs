using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveJumpBackward : CharacterMove
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
            return inputClass.DPadNumPad == 7;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
             //Advance Jump Frame Counter and assess Startup/Recovery
            if (Properties.CurrentState == CharacterProperties.CharacterState.JumpBackward && 
                FrameData.AttackFrameState[Properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Startup)
            {
                if (FrameData.AttackFrameState[Properties.JumpFrameCounter] ==
                    FrameDataHandler.AttackFrameStates.Startup)
                {
                    Properties.JumpFrameCounter++;
                    FrameData.Update(Properties.JumpFrameCounter);
                    return;
                }
                //Perform Jump motion
                if (FrameData.AttackFrameState[Properties.JumpFrameCounter] ==
                    FrameDataHandler.AttackFrameStates.Active)
                {
                    Properties.JumpFrameCounter++;
                    Properties.LastState = Properties.CurrentState;
                    Properties.IsAirborne = true;
                    Properties.MoveDirection = new Vector3(-Properties.WalkBackwardXSpeed, Properties.JumpYSpeed, 0);
                    return;
                }
            }
            
            //Use Character controller to determine if animation has reached the ground
            if (Properties.CurrentState == CharacterProperties.CharacterState.JumpBackward &&
                Properties.CharacterController.isGrounded &
                FrameData.AttackFrameState[Properties.JumpFrameCounter] != FrameDataHandler.AttackFrameStates.Startup)
            {
                Properties.JumpFrameCounter++;
                Properties.LastState = Properties.CurrentState;
                Properties.CurrentState = CharacterProperties.CharacterState.LandingJumpBackward;
                FrameData.Update(Properties.JumpFrameCounter);
            }

            if (Properties.CurrentState == CharacterProperties.CharacterState.LandingJumpBackward)
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
            
            //Continue to apply backward motion until landing
            if (Properties.CurrentState == CharacterProperties.CharacterState.JumpBackward)
            {
                Debug.Log(Properties.MoveDirection.x);
                Properties.MoveDirection.x = -Properties.WalkForwardXSpeed;
            }

            //Detect proper state and detect input
            if (!DetectMoveInput(inputClass)) return;
            if (Properties.CurrentState != CharacterProperties.CharacterState.Crouch &&
                Properties.CurrentState != CharacterProperties.CharacterState.Stand && Properties.CurrentState !=
                CharacterProperties.CharacterState.CancellableAnimation) return;

            Properties.JumpFrameCounter = 0;
            Properties.LastState = Properties.CurrentState;
            Properties.CurrentState = CharacterProperties.CharacterState.JumpBackward;
        }
    }
}