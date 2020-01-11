using UnityEngine;

namespace CharacterMoves
{
    
    public class MoveJumpBackward : ICharacterMove
    {
        //Tracks invincibility States per frame.
        //0:None, 1:Full, 2:UpperBody, 3:LowerBody 4:throw
        public int[] InvincibilyFrames =
        {
            4,4,4,0,0,0
        };
        
        //Tracks attack properties of the move per frame.
        //0:StartUp, 1:Active, 2:Recovery 3:End
        public int[] AttackStateFrames =
        {
            0,0,0,1,2,2,2,3
        };
        
        //Tracks Cancellable states of the move per frame.
        //0:None, 1:EmptyCancel, 2:NormalCancel, 3:SpecialCancel, 4:SuperCancel
        public int[] Cancellability =
        {
            0
        };
        
        //Tracks Airborne State of move per frame
        //0: No, 1:Yes
        public int[] AirborneState =
        {
            0,0,0,1,0,0,0
        };
        
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
