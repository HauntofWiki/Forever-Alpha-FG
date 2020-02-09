using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveStandIdle : CharacterMove
    {
        private CharacterManager _manager;
        private Animator _animator;
        public override void InitializeMove(ref CharacterManager manager, Animator animator)
        {
            _animator = animator;
            _manager = manager;
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
            if (_manager.CurrentState != CharacterManager.CharacterState.Crouch && _manager.CurrentState != CharacterManager.CharacterState.Stand) return;

            _manager.LastState = _manager.CurrentState;
            _manager.CurrentState = CharacterManager.CharacterState.Stand;
            _manager.MoveDirection = new Vector3(0, 0, 0);
        }
        
    }
}
