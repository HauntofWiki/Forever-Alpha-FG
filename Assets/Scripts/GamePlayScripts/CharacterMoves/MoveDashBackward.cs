using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashBackward : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        private FrameDataHandler _frameData;
        private int _inputLimit;
        private readonly int[] _movePattern = {-1,0,-1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastMove;
        private int _moveDetectCounter;
        
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            _animator = animator;
            _properties = properties;
            _lastMove = -1;
            _moveDetectCounter = 0;
            _inputLimit = 15;
            _frameData = new FrameDataHandler(30);
            _frameData.SetFieldsZero();
            _frameData.SetAttackFrames(3,24);
            _frameData.SetAirborneFrames(0,20);
            //_properties.BackDashDuration = AttackStateFrames.Length;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            _moveDetectCounter++;
        
            if (_moveDetectCounter >= _inputLimit)
                ResetInputDetect();
        
            if (_lastMove == inputClass.DPadNumPad)
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
            _lastMove = inputClass.DPadNumPad;
            return false;
        }

        private void ResetInputDetect()
        {
            _lastMove = -1;
            _moveDetectCounter = 0;
            _patternMatch[0] = false;
            _patternMatch[1] = false;
            _patternMatch[2] = false;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
                //Play out BackDash animation
                if (_properties.CurrentState == CharacterProperties.CharacterState.BackDash)
                {
                    _properties.DashFrameCounter++;
                    if (_frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Startup)
                        _properties.MoveDirection.x = -_properties.DashBackwardXSpeed[1];
                    else if (_frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Active)
                        _properties.MoveDirection.x = -_properties.DashBackwardXSpeed[2];
                    else if (_frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Recovery)
                    {
                        _properties.MoveDirection.x = 0;
                        _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                    }

                    _frameData.Update(_properties.DashFrameCounter);
                }
                
                //Begin Dash Detection
                if (!DetectMoveInput(inputClass)) return;
                if (_properties.CurrentState != CharacterProperties.CharacterState.Stand) return;
                _animator.Play("DashBackward");
                _properties.DashFrameCounter = 0;
                _properties.LastState = _properties.CurrentState;
                _properties.CurrentState = CharacterProperties.CharacterState.BackDash;
                _properties.MoveDirection = new Vector3(-_properties.DashBackwardXSpeed[0], 0, 0);
            }
        
    }
}