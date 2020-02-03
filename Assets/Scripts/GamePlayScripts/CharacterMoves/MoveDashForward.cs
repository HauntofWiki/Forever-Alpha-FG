using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashForward : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        private FrameDataHandler _frameData;
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
            _frameData = new FrameDataHandler(7);
            _frameData.SetFieldsZero();
            _frameData.SetAttackFrames(2,1);
        }

        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            _animator = animator;
            _properties = properties;
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
            if (_properties.CurrentState == CharacterProperties.CharacterState.Dash &&
                DetectHoldInput(inputClass) &&
                _frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Active)
            {
                _properties.MoveDirection = new Vector3(_properties.DashForwardXSpeed[1],0, 0);
                return;
            }

            //Play out dash animation
            if (_properties.CurrentState == CharacterProperties.CharacterState.Dash)
            {
                _properties.DashFrameCounter++;
                _properties.MoveDirection = new Vector3(_properties.DashForwardXSpeed[2],0, 0);
                if (_frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Recovery)
                {
                    _animator.Play("DashForwardBrake");
                    _properties.LastState = _properties.CurrentState;
                    _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                }
                _frameData.Update(_properties.DashFrameCounter);
            }
            
            //Begin Dash Detection
            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState != CharacterProperties.CharacterState.Stand &&
                _properties.CurrentState != CharacterProperties.CharacterState.Crouch) return;
            _animator.Play("DashForward");
            _properties.DashFrameCounter = 0;
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.Dash;
            _properties.MoveDirection = new Vector3(_properties.DashForwardXSpeed[0], 0, 0);
        }
    }
}
