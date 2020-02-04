using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashForward : CharacterMove
    {
        private readonly int _inputLimit;
        private readonly int[] _movePattern = {1, 0, 1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastInput;
        private int _moveDetectCounter;

        public MoveDashForward()
        {
            _lastInput = -1;
            _moveDetectCounter = 0;
            _inputLimit = 15;
            //Forward dash is special because length doesnt matter because you can hold the input
            //Basically you just have input and recovery
            FrameData = new FrameDataHandler(7);
            FrameData.SetFieldsZero();
            FrameData.SetAttackFrames(2,1);
        }

        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Animator = animator;
            Properties = properties;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            _moveDetectCounter++;

            if (_moveDetectCounter >= _inputLimit)
                ResetInputDetect();

            if (_lastInput == inputClass.DPadNumPad)
                return false;
            if (inputClass.DPadX == _movePattern[0] && !_patternMatch[0])
            {
                _moveDetectCounter = 0;
                _patternMatch[0] = true;
            }

            if (inputClass.DPadX == _movePattern[1] && _patternMatch[0])
                _patternMatch[1] = true;
            if (inputClass.DPadX == _movePattern[2] && _patternMatch[1])
                _patternMatch[2] = true;
            if (_patternMatch[0] && _patternMatch[1] && _patternMatch[2])
            {
                ResetInputDetect();
                return true;
            }

            _lastInput = inputClass.DPadNumPad;
            return false;
        }

        private void ResetInputDetect()
        {
            _lastInput = -1;
            _moveDetectCounter = 0;
            _patternMatch[0] = false;
            _patternMatch[1] = false;
            _patternMatch[2] = false;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return inputClass.DPadX > 0;
        }

        public override void PerformAction(InputClass inputClass)
        {

            //Detect whether the Forward is held for a long dash
            if (Properties.CurrentState == CharacterProperties.CharacterState.Dash &&
                DetectHoldInput(inputClass) &&
                FrameData.AttackFrameState[Properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Active)
            {
                Properties.MoveDirection = new Vector3(Properties.DashForwardXSpeed[1],0, 0);
                return;
            }

            //Play out dash animation
            if (Properties.CurrentState == CharacterProperties.CharacterState.Dash)
            {
                Properties.DashFrameCounter++;
                Properties.MoveDirection = new Vector3(Properties.DashForwardXSpeed[2],0, 0);
                if (FrameData.AttackFrameState[Properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Recovery)
                {
                    Animator.Play("DashForwardBrake");
                    Properties.LastState = Properties.CurrentState;
                    Properties.CurrentState = CharacterProperties.CharacterState.Stand;
                }
                FrameData.Update(Properties.DashFrameCounter);
            }
            
            //Begin Dash Detection
            if (!DetectMoveInput(inputClass)) return;
            if (Properties.CurrentState != CharacterProperties.CharacterState.Stand &&
                Properties.CurrentState != CharacterProperties.CharacterState.Crouch) return;
            Animator.Play("DashForward");
            Properties.DashFrameCounter = 0;
            Properties.LastState = Properties.CurrentState;
            Properties.CurrentState = CharacterProperties.CharacterState.Dash;
            Properties.MoveDirection = new Vector3(Properties.DashForwardXSpeed[0], 0, 0);
        }
    }
}
