using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterMove
{
    bool DetectMoveInput(InputClass inputClass);
    Vector3 PerformAction(ref Character.CharacterState characterState);
}
