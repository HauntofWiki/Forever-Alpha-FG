using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveDashBackward : CharacterMove
    {
        private int _inputLimit;
        private readonly int[] _movePattern = {-1,0,-1}; //Dash uses X-axis only
        private readonly bool[] _patternMatch = {false, false, false};
        private int _lastMove;
        private int _moveDetectCounter;
        
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Animator = animator;
            Properties = properties;
            _lastMove = -1;
            _moveDetectCounter = 0;
            _inputLimit = 15;
            FrameData = new FrameDataHandler(30);
            FrameData.SetFieldsZero();
            FrameData.SetActionFrames(3,24);
            FrameData.SetAirborneFrames(0,20);
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
                if (Properties.CurrentState == CharacterProperties.CharacterState.BackDash)
                {
                    Properties.DashFrameCounter++;
                    if (FrameData.ActionFrameState[Properties.DashFrameCounter] == FrameDataHandler.ActionFrameStates.Startup)
                        Properties.MoveDirection.x = -Properties.DashBackwardXSpeed[1];
                    else if (FrameData.ActionFrameState[Properties.DashFrameCounter] == FrameDataHandler.ActionFrameStates.Active)
                        Properties.MoveDirection.x = -Properties.DashBackwardXSpeed[2];
                    else if (FrameData.ActionFrameState[Properties.DashFrameCounter] == FrameDataHandler.ActionFrameStates.Recovery)
                    {
                        Properties.MoveDirection.x = 0;
                        Properties.CurrentState = CharacterProperties.CharacterState.Stand;
                    }

                    FrameData.Update(Properties.DashFrameCounter);
                }
                
                
                //Begin Dash Detection
                if (!DetectMoveInput(inputClass)) return;
                if (Properties.CurrentState != CharacterProperties.CharacterState.Stand) return;
                Animator.Play("DashBackward");
                Properties.DashFrameCounter = 0;
                Properties.LastState = Properties.CurrentState;
                Properties.CurrentState = CharacterProperties.CharacterState.BackDash;
                Properties.MoveDirection = new Vector3(-Properties.DashBackwardXSpeed[0], 0, 0);
            }
        
    }
}