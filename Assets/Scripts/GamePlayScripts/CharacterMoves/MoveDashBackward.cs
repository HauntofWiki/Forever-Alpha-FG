using System;
using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashBackward : CharacterMove
    {
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Properties = properties;
            Animator = animator;
            FrameData = new FrameDataHandler(12);
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
                if (Properties.CurrentState == CharacterProperties.CharacterState.Stand ||
                    Properties.CurrentState == CharacterProperties.CharacterState.Crouch)
                {
                    Animator.Play("DashBackward");
                    ActionCounter = 0;
                    FrameData.Update(ActionCounter);
                    Properties.FrameDataHandler = FrameData;
                    Properties.LastState = Properties.CurrentState;
                    Properties.CurrentState = CharacterProperties.CharacterState.BackDash;
                    Properties.MoveDirection = new Vector3(-Properties.DashBackwardXSpeed[0], 0, 0);
                    return;
                }
            }

            //Play out Back Dash Animation
            if (Properties.CurrentState == CharacterProperties.CharacterState.BackDash)
            {
                //Startup
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Startup)
                {
                    FrameData.Update(ActionCounter++);
                    Properties.MoveDirection = new Vector3(-Properties.DashBackwardXSpeed[0], 0, 0);
                    return;
                }

                //Active
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Active)
                {
                    FrameData.Update(ActionCounter++);
                    Properties.MoveDirection = new Vector3(-Properties.DashBackwardXSpeed[1], 0, 0);
                    return;
                }

                //Recovery
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Recovery)
                {
                    FrameData.Update(ActionCounter++);
                    Properties.MoveDirection = new Vector3(-Properties.DashBackwardXSpeed[2], 0, 0);
                    return;
                }

                //Exit
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.None)
                {
                    Properties.CurrentState = CharacterProperties.CharacterState.Stand;
                }

            }
        }
    }
}