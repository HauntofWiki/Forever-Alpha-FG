namespace CharacterMoves
{
    public class MoveDashForward : ICharacterMove
    {
   
        private readonly int _inputLimit;
        private readonly int[] _movePattern = {1,0,1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastInput;
        private int _moveDetectCounter;
    
        public MoveDashForward()
        {
            _lastInput = -1;
            _moveDetectCounter = 0;
            _inputLimit = 20;
        }

        public bool DetectMoveInput(InputClass inputClass)
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

        public bool DetectHoldInput(InputClass inputClass)
        {
            return inputClass.DPadX > 0;
        }

        private void ResetInputDetect()
        {
            _lastInput = -1;
            _moveDetectCounter = 0;
            _patternMatch[0] = false;
            _patternMatch[1] = false;
            _patternMatch[2] = false;
        }
    }
}
