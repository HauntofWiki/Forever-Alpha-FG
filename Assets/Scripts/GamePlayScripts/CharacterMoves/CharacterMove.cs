using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public abstract class CharacterMove
    {
        protected CharacterManager Manager;
        protected Animator Animator;
        protected FrameDataManager FrameData;
        //Counter tracks the animation frames to sync with frame data;
        protected int ActionCounter = 0;
        //LastInput is used to protect against repeated inputs from holding for a fraction of a second
        protected int LastInput;
        //Input Limit tracks the amount of frames available to input a pattern
        protected int InputLimit;
        //InputCounter tracks how many frames have occured while looking for a pattern
        protected int InputCounter;
        //MovePattern sets the desired input pattern for a move
        protected int[] MovePattern;
        //PatternMatch tracks each successful input
        protected bool[] PatternMatch;
        
        
        public abstract void InitializeMove(ref CharacterManager manager, Animator animator);
        public abstract bool DetectMoveInput(InputClass inputClass);
        public abstract bool DetectHoldInput(InputClass inputClass);
        public abstract void PerformAction(InputClass inputClass);
    }
}
