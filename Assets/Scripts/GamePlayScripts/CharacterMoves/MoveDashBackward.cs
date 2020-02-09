using System;
using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashBackward : CharacterMove
    {
        public override void InitializeMove(ref CharacterManager manager, Animator animator)
        {
            Manager = manager;
            Animator = animator;
            FrameData = new FrameDataManager(12);
            FrameData.SetFieldsZero();
            FrameData.SetActionFrames(3, 6);
            FrameData.SetAirborneFrames(0, 20);
            ActionCounter = 0;
            LastInput = -1;
            InputLimit = 15;
            InputCounter = 0;
            MovePattern = new int[] {-1, 0, -1};
            PatternMatch = new bool[] {false, false, false,};
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            InputCounter++;
            //Determine if it has been too long to input the command
            if (InputCounter >= InputLimit)
                ResetInputDetect();

            if (LastInput == inputClass.DPadNumPad)
                return false;
            //First Input, also resets input counter
            if (inputClass.DPadX == MovePattern[0] && !PatternMatch[0])
            {
                InputCounter = 0;
                PatternMatch[0] = true;
            }

            //Second Input
            if (inputClass.DPadX == MovePattern[1] && PatternMatch[0])
                PatternMatch[1] = true;
            //Third input
            if (inputClass.DPadX == MovePattern[2] && PatternMatch[1])
                PatternMatch[2] = true;
            //Match found
            if (PatternMatch[0] && PatternMatch[1] && PatternMatch[2])
            {
                ResetInputDetect();
                return true;
            }

            LastInput = inputClass.DPadNumPad;
            return false;
        }

        private void ResetInputDetect()
        {
            LastInput = -1;
            InputCounter = 0;
            PatternMatch[0] = false;
            PatternMatch[1] = false;
            PatternMatch[2] = false;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            //Detect Input
            if (DetectMoveInput(inputClass))
            {
                if (Manager.CurrentState == CharacterManager.CharacterState.Stand ||
                    Manager.CurrentState == CharacterManager.CharacterState.Crouch)
                {
                    Animator.Play("DashBackward");
                    ActionCounter = 0;
                    FrameData.Update(ActionCounter);
                    Manager.SetFrameDataManager(FrameData);
                    Manager.LastState = Manager.CurrentState;
                    Manager.CurrentState = CharacterManager.CharacterState.BackDash;
                    Manager.MoveDirection = new Vector3(-Manager.DashBackwardXSpeed[0], 0, 0);
                    return;
                }
            }

            //Play out Back Dash Animation
            if (Manager.CurrentState == CharacterManager.CharacterState.BackDash)
            {
                //Startup
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Startup)
                {
                    FrameData.Update(ActionCounter++);
                    Manager.MoveDirection = new Vector3(-Manager.DashBackwardXSpeed[0], 0, 0);
                    return;
                }

                //Active
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Active)
                {
                    FrameData.Update(ActionCounter++);
                    Manager.MoveDirection = new Vector3(-Manager.DashBackwardXSpeed[1], 0, 0);
                    return;
                }

                //Recovery
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Recovery)
                {
                    FrameData.Update(ActionCounter++);
                    Manager.MoveDirection = new Vector3(-Manager.DashBackwardXSpeed[2], 0, 0);
                    return;
                }

                //Exit
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.None)
                {
                    Manager.CurrentState = CharacterManager.CharacterState.Stand;
                }

            }
        }
    }
}