using UnityEngine;

namespace CharacterMoves
{
    public interface ICharacterMove
    {
        bool DetectMoveInput(InputClass inputClass);
        bool DetectHoldInput(InputClass inputClass);
    }
}
