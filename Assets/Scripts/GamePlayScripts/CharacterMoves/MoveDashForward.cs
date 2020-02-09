using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashForward : CharacterMove
    {
        public override void InitializeMove(ref CharacterManager manager, Animator animator)
        {
            Manager = manager;
            Animator = animator;
            //Forward dash is special because length doesnt matter because you can hold the input
            //Basically you just have startup and recovery
            FrameData = new FrameDataManager(7);
            FrameData.SetFieldsZero();
            FrameData.SetActionFrames(3,1);
            ActionCounter = 0;
            LastInput = -1;
            InputLimit = 15;
            InputCounter = 0;
            MovePattern = new int[] {1, 0, 1};
            PatternMatch = new bool[] {false, false, false};
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            //Input limit reached
            if (InputCounter >= InputLimit)
                ResetInputDetect();

            //Repeated input return till next new input
            if (LastInput == inputClass.DPadX)
                return false;
            //First input
            if (inputClass.DPadX == MovePattern[0] && !PatternMatch[0])
            {
                InputCounter = 0;
                PatternMatch[0] = true;
            }
            //Second input
            if (inputClass.DPadX == MovePattern[1] && PatternMatch[0])
                PatternMatch[1] = true;
            //Third input
            if (inputClass.DPadX == MovePattern[2] && PatternMatch[1])
                PatternMatch[2] = true;
            //Pattern detected
            if (PatternMatch[0] && PatternMatch[1] && PatternMatch[2])
            {
                ResetInputDetect();
                return true;
            }

            //Prepare for next rep
            InputCounter++;
            LastInput = inputClass.DPadNumPad;
            return false;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return inputClass.DPadX > 0;
        }

        public override void PerformAction(InputClass inputClass)
        {
            //Detect input and proper state
            if (DetectMoveInput(inputClass))
            {
                if (Manager.CurrentState == CharacterManager.CharacterState.Stand ||
                    Manager.CurrentState == CharacterManager.CharacterState.Crouch)
                {
                    //Start animation
                    ActionCounter = 0;
                    Animator.Play("DashForward");
                    FrameData.Update(ActionCounter);
                    Manager.SetFrameDataManager(FrameData);
                    Manager.LastState = Manager.CurrentState;
                    Manager.CurrentState = CharacterManager.CharacterState.Dash;
                    return;
                }
            }
            
            //Play out dash animation
            if (Manager.CurrentState == CharacterManager.CharacterState.Dash)
            {
                //Move at startup speed
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Startup)
                {
                    FrameData.Update(ActionCounter++);
                    Manager.MoveDirection = new Vector3(Manager.DashForwardXSpeed[0], 0, 0);
                    return;
                }

                //Check for a held input for a long dash, do not advance ActionCounter
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Active)
                {
                    if (DetectHoldInput(inputClass))
                    {
                        Manager.LastState = Manager.CurrentState;
                        Manager.MoveDirection = new Vector3(Manager.DashForwardXSpeed[1], 0, 0);
                        return;
                    }

                    FrameData.Update(ActionCounter++);
                }

                //Recovery, this is also the minimum dash distance
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Recovery)
                {
                    FrameData.Update(ActionCounter++);
                    Animator.Play("DashForwardBrake");
                    Manager.MoveDirection = new Vector3(Manager.DashForwardXSpeed[2], 0, 0);
                    Manager.LastState = Manager.CurrentState;
                    return;
                }

                //Exit Dash action
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.None)
                {
                    Manager.LastState = Manager.CurrentState;
                    Manager.CurrentState = CharacterManager.CharacterState.Stand;
                }
            }
        }
        
        private void ResetInputDetect()
        {
            LastInput = -1;
            InputCounter = 0;
            PatternMatch[0] = false;
            PatternMatch[1] = false;
            PatternMatch[2] = false;
        }
    }
    
    
}
