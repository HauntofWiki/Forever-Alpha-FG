using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveJumpNeutral : CharacterMove
    {
        public override void InitializeMove(ref CharacterManager manager, Animator animator)
        {
            Animator = animator;
            Manager = manager;
            FrameData = new FrameDataManager(7);
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
                if (Manager.CurrentState == CharacterManager.CharacterState.Stand ||
                    Manager.CurrentState == CharacterManager.CharacterState.Crouch)
                {
                    if (Manager.Grounded)
                    {
                        FrameData.Update(ActionCounter++);
                        Manager.LastState = Manager.CurrentState;
                        Manager.CurrentState = CharacterManager.CharacterState.Jump;
                        Manager.IsAirborne = FrameData.Airborne;
                        Manager.SetFrameDataManager(FrameData);
                        return;
                    }
                }
            }

            //Play out jump according to frame data.
            if (Manager.CurrentState == CharacterManager.CharacterState.Jump)
            {
                //In the start up of the jump there is no movement, only frame advancement
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Startup)
                {
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Perform Jump animation
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Active)
                {
                    FrameData.Update(ActionCounter++);
                    Manager.MoveDirection = new Vector3(0, Manager.JumpYSpeed, 0);
                    return;
                }
                //Once grounded, begin recovery portion of the jump
                if (Manager.Grounded && FrameData.ActionState == FrameDataManager.ActionFrameStates.Recovery)
                {
                    FrameData.Update(ActionCounter++);
                    Manager.CurrentState = CharacterManager.CharacterState.Landing;
                    return;
                }
            }
            //Play out recovery
            if (Manager.CurrentState == CharacterManager.CharacterState.Landing)
            {
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Recovery)
                {
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Exit Jump state
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.None)
                {
                    ActionCounter = 0;
                    Manager.CurrentState = CharacterManager.CharacterState.Stand;
                }
            }
        }
    }
}
