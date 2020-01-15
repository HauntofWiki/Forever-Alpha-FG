using UnityEngine;

namespace CharacterMoves
{
    public class MoveWalkForward : CharacterMove
    {
        private CharacterProperties _properties;
        public override void InitializeMove(ref CharacterProperties properties)
        {
            _properties = properties;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 6;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState != CharacterProperties.CharacterState.Crouch && _properties.CurrentState != CharacterProperties.CharacterState.Stand) return;
        
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.Stand;
            _properties.MoveDirection = new Vector3(_properties.WalkForwardXSpeed, 0, 0);
        }
    }
}
