using System;
using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveAirDashForward : CharacterMove
    {
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Properties = properties;
            Animator = animator;
            FrameData = new FrameDataHandler(18);
            FrameData.SetFieldsZero();
            FrameData.SetActionFrames(3, 6);
            ActionCounter = 0;
            LastInput = -1;
            InputLimit = 15;
            InputCounter = 0;
            MovePattern = new int[] {1, 0, 1};
            PatternMatch = new bool[] {false, false, false};
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            //Check the limit of how many frames are available to input the move
            if (InputCounter >= InputLimit)
                ResetInputDetect();
        
            //Unity repeats Axis inputs for several frames, this allows us to not check those
            if (LastInput == inputClass.DPadNumPad)
                return false;
            
            //Check for first forward input
            if (inputClass.DPadX == MovePattern[0] && !PatternMatch[0])
            {
                InputCounter = 0;
                PatternMatch[0] = true;            
            }
            //Checks for a neutral input
            if (inputClass.DPadX == MovePattern[1] && PatternMatch[0])
                PatternMatch[1] = true;
            
            //Checks for the second forward input
            if (inputClass.DPadX == MovePattern[2] && PatternMatch[1])
                PatternMatch[2] = true;
            //Return a positive match
            if (PatternMatch[0] && PatternMatch[1] && PatternMatch[2])
            {
                ResetInputDetect();
                return true;
            }

            LastInput = inputClass.DPadNumPad;
            return false;
        }
        
        //No holdable effect for AirDash
        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            if (DetectMoveInput(inputClass))
            {
                if (Properties.CurrentState == CharacterProperties.CharacterState.Jump ||
                    Properties.CurrentState == CharacterProperties.CharacterState.JumpForward ||
                    Properties.CurrentState == CharacterProperties.CharacterState.JumpBackward)
                {
                    Animator.Play("AirDashForward");
                    ActionCounter = 0;
                    FrameData.Update(ActionCounter);
                    Properties.LastState = Properties.CurrentState;
                    Properties.CurrentState = CharacterProperties.CharacterState.AirDash;
                    Properties.FrameDataHandler = FrameData;
                    Properties.IsIgnoringGravity = true;
                    Properties.MoveDirection = new Vector3(Properties.AirDashForwardSpeed[0],0,0);
                    return;
                }
            }

            if (Properties.CurrentState == CharacterProperties.CharacterState.AirDash)
            {
                //Startup
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Startup)
                {
                    FrameData.Update(ActionCounter++);
                    Properties.MoveDirection = new Vector3(Properties.AirDashForwardSpeed[0],0,0);
                    return;    
                }
                //Active
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Active)
                {
                    FrameData.Update(ActionCounter++);
                    Properties.MoveDirection = new Vector3(Properties.AirDashForwardSpeed[1],0,0);
                    return;
                }
                //Recovery
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Recovery)
                {
                        FrameData.Update(ActionCounter++);
                        Properties.MoveDirection = new Vector3(Properties.AirDashForwardSpeed[1],0,0);
                        return;
                }
                //Exit
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.None)
                {
                    Properties.IsIgnoringGravity = false;
                    Properties.LastState = Properties.CurrentState;
                    Properties.CurrentState = CharacterProperties.CharacterState.JumpForward;
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