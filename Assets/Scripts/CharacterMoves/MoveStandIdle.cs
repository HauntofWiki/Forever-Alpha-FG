using UnityEngine;

namespace CharacterMoves
{
    public class MoveStandIdle : ICharacterMove
    {

        public bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.DPadNumPad == 5;
        }

        public bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public Vector3 PerformAction(ref Character.CharacterState characterState)
        {
            characterState = Character.CharacterState.Stand;
            var moveDirection = new Vector3(0, 0, 0);

            return moveDirection;
        }
    }
}
