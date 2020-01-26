﻿using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public abstract class CharacterMove
    {
        private CharacterProperties _properties;
        public abstract void InitializeMove(ref CharacterProperties properties, Animator animator);
        public abstract bool DetectMoveInput(InputClass inputClass);
        public abstract bool DetectHoldInput(InputClass inputClass);
        public abstract void PerformAction(InputClass inputClass);
    }
}
