using System.Transactions;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

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
            FrameData.SetActionFrames(3, 1);
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
            
            //Detect initial input, set state and reset counter.
            if (DetectMoveInput(inputClass))
            {
                if (Properties.CurrentState == CharacterProperties.CharacterState.Stand ||
                    Properties.CurrentState == CharacterProperties.CharacterState.Crouch)
                {
                    if (Properties.IsGrounded)
                    {
                        ActionCounter = 0;
                        FrameData.Update(ActionCounter);
                        Properties.LastState = Properties.CurrentState;
                        Properties.CurrentState = CharacterProperties.CharacterState.JumpForward;
                        Properties.IsAirborne = FrameData.Airborne;
                        Properties.FrameDataHandler = FrameData;
                        return;
                    }
                }
            }

            //Play out jump according to frame data.
            if (Properties.CurrentState == CharacterProperties.CharacterState.JumpForward)
            {
                //In the start up of the jump there is no movement, only frame advancement
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Startup)
                {
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Perform Jump animation
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Active)
                {
                    FrameData.Update(ActionCounter++);
                    Properties.MoveDirection = new Vector3(Properties.WalkForwardXSpeed, Properties.JumpYSpeed, 0);
                    Animator.Play("JumpForward");
                    return;
                }
                
                //Continue forward momentum after jump starts
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Recovery)
                {
                    //Continue forward momentum after jump starts
                    if (!Properties.IsGrounded)
                    {
                        FrameData.Update(ActionCounter);
                        Properties.MoveDirection.x = Properties.WalkForwardXSpeed;
                        return;
                    }
                    
                    //Once grounded, begin recovery portion of the jump
                    if (Properties.IsGrounded)
                    {
                        FrameData.Update(ActionCounter++);
                        Properties.CurrentState = CharacterProperties.CharacterState.LandingJumpForward;
                        return;
                    }
                }
                //In some cases we recover out of an action into a JumpForward state where ActionState is none;
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.None)
                {
                    ActionCounter = 4;
                    FrameData.Update(ActionCounter);
                    Properties.MoveDirection.x = Properties.WalkForwardXSpeed;
                }
            }
            //Play out recovery
            if (Properties.CurrentState == CharacterProperties.CharacterState.LandingJumpForward)
            {
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Recovery)
                {
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Exit Jump state
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.None)
                {
                    ActionCounter = 0;
                    Properties.CurrentState = CharacterProperties.CharacterState.Stand;
                }
            }
        }
    }
}

