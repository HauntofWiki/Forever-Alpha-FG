using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveAirDashForward : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        private FrameDataHandler _frameData;
        private int _inputLimit;
        private readonly int[] _movePattern = {1,0,1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastInput;
        private int _moveDetectCounter;

//        //Tracks invincibility States per frame.
//        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
//        public int[] InvincibilyFrames =
//        {
//            0
//        };
//        
//        //Tracks attack properties of the move per frame.
//        //0:StartUp, 1:Active, 2:Recovery 3:End
//        public int[] AttackStateFrames =
//        {
//            0,0,0,1,1,1,1,1,1,1,1,1,1,1,2,2,2,3
//        };
//        
//        //Tracks Cancellable states of the move per frame.
//        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
//        public int[] Cancellability =
//        {
//            1
//        };
//        
//        //Tracks Airborne State of move per frame
//        //0: No, 1:Yes
//        public int[] AirborneState =
//        {
//            1
//        };

        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            _animator = animator;
            _properties = properties;
            _lastInput = -1;
            _moveDetectCounter = 0;
            _inputLimit = 15;
            _frameData = new FrameDataHandler(18);
            _frameData.SetFieldsZero();
            _frameData.SetAttackFrames(3, 11);
            
            
           // _properties.AirDashDuration = AttackStateFrames.Length;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            if (!_properties.IsAirborne) return false;
            _moveDetectCounter++;
        
            //Check the limit of how many frames are available to input the move
            if (_moveDetectCounter >= _inputLimit)
                ResetInputDetect();
        
            //Unity repeats Axis inputs for several frames, this allows us to not check those
            if (_lastInput == inputClass.DPadNumPad)
                return false;
            
            //Check for first forward input
            if (inputClass.DPadX == _movePattern[0] && !_patternMatch[0])
            {
                _moveDetectCounter = 0;
                _patternMatch[0] = true;            
            }
            //Checks for a neutral input
            if (inputClass.DPadX == _movePattern[1] && _patternMatch[0])
                _patternMatch[1] = true;
            
            //Checks for the second forward input
            if (inputClass.DPadX == _movePattern[2] && _patternMatch[1])
                _patternMatch[2] = true;
            //Return a positive match
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

        //No holdable effect for AirDash
        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            //Play out AirDash Duration
		    if (_properties.CurrentState == CharacterProperties.CharacterState.AirDash)
            {
                _properties.LastState = _properties.CurrentState;
                _properties.DashFrameCounter++;
                if (_frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Startup)
                    _properties.MoveDirection = new Vector3(_properties.AirDashForwardSpeed[0],0,0);
			    if (_frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Active)
                    _properties.MoveDirection = new Vector3(_properties.AirDashForwardSpeed[1],0,0);
                if (_frameData.AttackFrameState[_properties.DashFrameCounter] == FrameDataHandler.AttackFrameStates.Recovery)
                {
                    //Exit the dash state
                    _properties.LastState = _properties.CurrentState;
                    _properties.CurrentState = CharacterProperties.CharacterState.JumpForward;
                    _properties.DashFrameCounter = 0;
                    _properties.IsIgnoringGravity = false;
                }
                
                _frameData.Update(_properties.AttackFrameCounter);
            }
            
            
            
            //Begin AirDash Detection
            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState == CharacterProperties.CharacterState.Stand) return;
            _animator.Play("AirDashForward");
            _properties.DashFrameCounter = 0;
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.AirDash;
            _properties.IsIgnoringGravity = true;
            _properties.MoveDirection = new Vector3(_properties.AirDashForwardSpeed[0],0,0);
        }
    }
}