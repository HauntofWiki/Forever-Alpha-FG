using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveJumpForward : ICharacterMove 
{
    public bool DetectMoveInput(InputClass inputClass)
    {
        return inputClass.DPadNumPad == 9;
    }

    public Vector3 PerformAction(ref Character.CharacterState characterState)
    {
        return new Vector3(0,0,0);
    }
}
