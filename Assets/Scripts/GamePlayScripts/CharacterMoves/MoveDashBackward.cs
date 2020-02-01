using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashBackward : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        private int _inputLimit;
        private readonly int[] _movePattern = {-1,0,-1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastMove;
        private int _moveDetectCounter;
    
        //Tracks invincibility States per frame.
        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
        public int[] InvincibilyFrames =
        {
            1,1,1,0,0,0,0,0,0,0,0,0,0,0,0
        };
        
        //Tracks attack properties of the move per frame.
        //0:StartUp, 1:Active, 2:Recovery 3:End
        public int[] AttackStateFrames =
        {
            0,0,0,1,1,1,1,1,1,1,1,1,1,2,2,2,3
        };
        
        //Tracks Cancellable states of the move per frame.
        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
        public int[] Cancellability =
        {
            0
        };
        
        //Tracks Airborne State of move per frame
        //0: No, 1:Yes
        public int[] AirborneState =
        {
            1,1,1,1,1,1,1,1,1,1,1,1,0,0,0
        };
        
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            _animator = animator;
            _properties = properties;
            _lastMove = -1;
            _moveDetectCounter = 0;
            _inputLimit = 15;
            _properties.BackDashDuration = AttackStateFrames.Length;
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
                //Debug.Log("d");
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
                if (_properties.CurrentState == CharacterProperties.CharacterState.BackDash && _properties.DashFrameCounter < _properties.BackDashDuration - 1)
                {
                    _properties.DashFrameCounter++;
                    if (AttackStateFrames[_properties.DashFrameCounter] == 1)
                        _properties.MoveDirection.x = -_properties.DashBackwardXSpeed[1];
                    else if (AttackStateFrames[_properties.DashFrameCounter] == 2)
                        _properties.MoveDirection.x = -_properties.DashBackwardXSpeed[2];
                }
                
                //Exit dash animation
                if (_properties.CurrentState == CharacterProperties.CharacterState.BackDash && _properties.DashFrameCounter >= _properties.BackDashDuration - 1)
                    _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                
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