using UnityEngine;

namespace CharacterMoves
{
    public class MoveJumpBackward : ICharacterMove
    {
        public bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 7;
        }

        public bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public Vector3 PerformAction(ref Character.CharacterState characterState)
        {
            return new Vector3(0,0,0);
        }
    }
}
