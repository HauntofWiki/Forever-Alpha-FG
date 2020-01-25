using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveStandIdle : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        public override void InitializeMove(ref CharacterProperties properties)
        {
            _animator = GameObject.Find("Player1").GetComponent<Animator>();
            _properties = properties;
        }
        
        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 5;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState != CharacterProperties.CharacterState.Crouch && _properties.CurrentState != CharacterProperties.CharacterState.Stand) return;
            _properties.DashFrameCounter = 0;
            
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.Stand;
            _properties.MoveDirection = new Vector3(0, 0, 0);
        }
        
    }
}
