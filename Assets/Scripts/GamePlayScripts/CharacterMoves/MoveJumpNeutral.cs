using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveJumpNeutral : CharacterMove
    {
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Animator = animator;
            Properties = properties;
            FrameData = new FrameDataHandler(7);
            FrameData.SetFieldsZero();
            //For jumps length isn't important to how they function
            FrameData.SetActionFrames(3, 1);
            FrameData.SetAirborneFrames(0, 6);
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 8;
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
                        FrameData.Update(ActionCounter++);
                        Properties.LastState = Properties.CurrentState;
                        Properties.CurrentState = CharacterProperties.CharacterState.Jump;
                        Properties.IsAirborne = FrameData.Airborne;
                        Properties.FrameDataHandler = FrameData;
                        return;
                    }
                }
            }

            //Play out jump according to frame data.
            if (Properties.CurrentState == CharacterProperties.CharacterState.Jump)
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
                    Properties.MoveDirection = new Vector3(0, Properties.JumpYSpeed, 0);
                    return;
                }
                //Once grounded, begin recovery portion of the jump
                if (Properties.IsGrounded && FrameData.ActionState == FrameDataHandler.ActionFrameStates.Recovery)
                {
                    FrameData.Update(ActionCounter++);
                    Properties.CurrentState = CharacterProperties.CharacterState.Landing;
                    return;
                }
            }
            //Play out recovery
            if (Properties.CurrentState == CharacterProperties.CharacterState.Landing)
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
