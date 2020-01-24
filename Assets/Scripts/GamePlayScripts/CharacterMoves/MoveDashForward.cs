using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashForward : CharacterMove
    {
        private CharacterProperties _properties;
        private readonly int _inputLimit;
        private readonly int[] _movePattern = {1, 0, 1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastInput;
        private int _moveDetectCounter;

        //Tracks invincibility States per frame.
        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
        public int[] InvincibilyFrames =
        {
            0
        };

        //Tracks attack properties of the move per frame.
        //0:StartUp, 1:Active, 2:Recovery 3:End
        public int[] AttackStateFrames =
        {
            0, 0, 0, 1, 2, 2, 2, 3
        };

        //Tracks Cancellable states of the move per frame.
        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
        public int[] Cancellability =
        {
            1
        };

        //Tracks Airborne State of move per frame
        //0: No, 1:Yes
        public int[] AirborneState =
        {
            0
        };

        public MoveDashForward()
        {
            _lastInput = -1;
            _moveDetectCounter = 0;
            _inputLimit = 20;
        }

        public override void InitializeMove(ref CharacterProperties properties)
        {
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
                AttackStateFrames[_properties.DashFrameCounter] == 1)
            {
                _properties.MoveDirection = new Vector3(_properties.DashForwardXSpeed[AttackStateFrames[_properties.DashFrameCounter]],
                    0, 0);
                return;
            }


            //Play out dash animation
            if (_properties.CurrentState == CharacterProperties.CharacterState.Dash &&
                AttackStateFrames[_properties.DashFrameCounter] < 3)
            {
                _properties.DashFrameCounter++;
                _properties.MoveDirection = new Vector3(_properties.DashForwardXSpeed[AttackStateFrames[_properties.DashFrameCounter]],
                    0, 0);
                if (AttackStateFrames[_properties.DashFrameCounter] >= 2)
                {
                    _properties.LastState = _properties.CurrentState;
                    _properties.CurrentState = CharacterProperties.CharacterState.CancellableAnimation;
                }

            }

            if (_properties.CurrentState == CharacterProperties.CharacterState.CancellableAnimation &&
                AttackStateFrames[_properties.DashFrameCounter] < 3)
            {
                _properties.DashFrameCounter++;
            }

            //Exit dash animation
            if (_properties.CurrentState == CharacterProperties.CharacterState.CancellableAnimation &&
                AttackStateFrames[_properties.DashFrameCounter] >= 3)
            {
                _properties.LastState = _properties.CurrentState;
                _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                _properties.DashFrameCounter = 0;
            }

            //Begin Dash Detection
            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState != CharacterProperties.CharacterState.Stand &&
                _properties.CurrentState != CharacterProperties.CharacterState.Crouch) return;
            _properties.DashFrameCounter = 0;
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.Dash;
            _properties.MoveDirection = new Vector3(_properties.DashForwardXSpeed[0], 0, 0);
        }
    }
}
