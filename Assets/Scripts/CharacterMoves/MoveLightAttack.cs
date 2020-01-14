namespace CharacterMoves
{
    public class MoveLightAttack : ICharacterMove
    {
        private readonly int _inputLimit;
        private readonly int[] _movePattern = {1,0,1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastInput;
        private int _moveDetectCounter;

        //Tracks invincibility States per frame.
        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
        public int[] InvincibilyFrames =
        {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };
        
        //Tracks attack properties of the move per frame.
        //0:StartUp, 1:Active, 2:Recovery 3:End
        public int[] AttackStateFrames =
        {
            0,0,0,1,1,1,1,1,2,2,2,2,2,2,2,2,2,3
        };
        
        //Tracks Cancellable states of the move per frame.
        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
        //5:JumpCancel, 6:AirJumpCancel, 7:DashCancel, 8:AirDashCancel
        public int[] Cancellability =
        {
            0,0,0,2,2,2,2,2,2,2,2,2,2,2,2
        };
        
        //Tracks Airborne State of move per frame
        //0: No, 1:Yes
        public int[] AirborneState =
        {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };
        public MoveLightAttack()
        {
            _lastInput = -1;
            _moveDetectCounter = 0;
            _inputLimit = 20;
        }
        
        public bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.LightAttackButtonDown == 1;
        }

        public bool DetectHoldInput(InputClass inputClass)
        {
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
    }
}