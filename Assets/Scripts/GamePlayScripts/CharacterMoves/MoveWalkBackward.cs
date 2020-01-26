using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveWalkBackward : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            _animator = animator;
            _properties = properties;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 4;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            if (!DetectMoveInput(inputClass)) return;
            if (_properties.CurrentState != CharacterProperties.CharacterState.Crouch && _properties.CurrentState != CharacterProperties.CharacterState.Stand) return;

            _animator.Play("WalkBackward");
            _properties.LastState = _properties.CurrentState;
            _properties.CurrentState = CharacterProperties.CharacterState.Stand;
            _properties.MoveDirection = new Vector3(-_properties.WalkBackwardXSpeed,0,0);
        }
    }
}
