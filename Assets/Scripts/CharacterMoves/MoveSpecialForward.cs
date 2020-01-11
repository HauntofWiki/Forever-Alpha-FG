using UnityEngine;

namespace CharacterMoves
{
    public class MoveSpecialForward : ICharacterMove
    {
        private readonly int _inputLimit;
        private readonly int[] _movePattern = {2,3,6};
        private bool _attackButton = false;
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastMove;
        private int _moveDetectCounter;
    
        public MoveSpecialForward()
        {
            _lastMove = -1;
            _moveDetectCounter = 0;
            _inputLimit = 15;
        }
    
        public bool DetectMoveInput(InputClass inputClass)
        {
            _moveDetectCounter++;
            if (_moveDetectCounter >= _inputLimit)
                ResetInputDetect();
            if (_lastMove == inputClass.DPadNumPad)
                return false;
        
            if (inputClass.DPadNumPad == _movePattern[0] && !_patternMatch[0])
            {
                _moveDetectCounter = 0;
                _patternMatch[0] = true;            
            }
            if (inputClass.DPadNumPad == _movePattern[1] && _patternMatch[0])
                _patternMatch[1] = true;
            if (inputClass.DPadNumPad == _movePattern[2] && _patternMatch[1])
                _patternMatch[2] = true;
            if ((inputClass.LightAttackButtonDown == 1 || inputClass.MediumAttackButtonDown == 1 ||
                 inputClass.HeavyAttackButtonDown == 1 || inputClass.SpecialAttackButtonDown == 1) && _patternMatch[2])
                _attackButton = true;
            if (_patternMatch[0] && _patternMatch[1] && _patternMatch[2] && _attackButton)
            {
                ResetInputDetect();
                return true;
            }

        
            _lastMove = inputClass.DPadNumPad;
            return false;
        }

        public bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public Vector3 PerformAction(ref Character.CharacterState characterState)
        {
            return new Vector3(0,0,0);
        }

        public void ResetInputDetect()
        {
            _lastMove = -1;
            _moveDetectCounter = 0;
            _patternMatch[0] = false;
            _patternMatch[1] = false;
            _patternMatch[2] = false;
        }
    }
}
