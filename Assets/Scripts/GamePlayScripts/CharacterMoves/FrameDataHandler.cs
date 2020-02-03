using System;
using System.Collections.Generic;

namespace GamePlayScripts.CharacterMoves
{
    public class FrameDataHandler
    {
        public enum AttackFrameStates
        {
            None,
            Startup,
            Active,
            Recovery
        }

        public enum CancellabilityStates
        {
            None,
            Empty,
            Normal,
            Special,
            Super
        }
        
        public enum InvincibilityStates
        {
            None,
            UpperBody,
            LowerBody,
            Full,
            Throw,
        }

        public List<AttackFrameStates> AttackFrameState;
        public List<CancellabilityStates> Cancellability;
        public List<InvincibilityStates> Invincibility;
        public List<bool> AirborneFrames;
        public AttackFrameStates AttackState;
        public CancellabilityStates Cancellable;
        public InvincibilityStates Invinsible;
        public bool Airborne;
        public int Length;
        
        public float Damage = 0;
        public float Dizzy = 0;
        public float MeterGain = 0;
        public float PushBack = 0;
        public float HitStun = 0;
        public float BlockStun = 0;

        public FrameDataHandler(int length)
        {
            Length = length;
            AttackFrameState = new List<AttackFrameStates>();
            Cancellability = new List<CancellabilityStates>();
            Invincibility = new List<InvincibilityStates>();
            AirborneFrames = new List<bool>();
            
            //initialize lists to be composed of 'None'
            for (var i = 0; i <= Length; i++)
            {
                AttackFrameState.Add(AttackFrameStates.None);
                Cancellability.Add(CancellabilityStates.None);
                Invincibility.Add(InvincibilityStates.None);
                AirborneFrames.Add(false);
            }
        }

        public void Update(int frameCounter)
        {
            if (frameCounter > Length) return;
            
            switch (AttackFrameState[frameCounter])
            {
                case AttackFrameStates.Startup:
                    AttackState = AttackFrameStates.Startup;
                    break;
                case AttackFrameStates.Active:
                    AttackState = AttackFrameStates.Active;
                    break;
                case AttackFrameStates.Recovery:
                    AttackState = AttackFrameStates.Recovery;
                    break;
                default:
                    AttackState = AttackFrameStates.None;
                    break;
            }

            switch (Cancellability[frameCounter])
            {
                case CancellabilityStates.Empty:
                    Cancellable = CancellabilityStates.Empty;
                    break;
                case CancellabilityStates.Normal:
                    Cancellable = CancellabilityStates.Normal;
                    break;
                case CancellabilityStates.Special:
                    Cancellable = CancellabilityStates.Special;
                    break;
                case CancellabilityStates.Super:
                    Cancellable = CancellabilityStates.Super;
                    break;
                default:
                    Cancellable = CancellabilityStates.None;
                    break;
            }

            switch (Invincibility[frameCounter])
            {
                case InvincibilityStates.UpperBody:
                    Invinsible = InvincibilityStates.UpperBody;
                    break;
                case InvincibilityStates.LowerBody:
                    Invinsible = InvincibilityStates.LowerBody;
                    break;
                case InvincibilityStates.Full:
                    Invinsible = InvincibilityStates.Full;
                    break;
                case InvincibilityStates.Throw:
                    Invinsible = InvincibilityStates.Throw;
                    break;
            }

            if (AirborneFrames[frameCounter])
                Airborne = true;

        }

        //Everything that isnt startup or Active is considered recovery
        public void SetAttackFrames(int startUpFrames, int activeFrames)
        {
            for (var i = 0; i < Length; i++)
            {
                if (i < startUpFrames)
                    AttackFrameState[i] = AttackFrameStates.Startup;
                else if (i < (startUpFrames + activeFrames))
                    AttackFrameState[i] = AttackFrameStates.Active;
                else if (i >= (startUpFrames + activeFrames))
                    AttackFrameState[i] = AttackFrameStates.Recovery;
                else AttackFrameState[i] = AttackFrameStates.None;
            }
        }

        public void SetCancellableFrames(int start, int end, CancellabilityStates state)
        {
            for (int i = start; i < end; i++)
            {
                Cancellability[i] = state;
            }
        }
        
        public void SetInvincibleFrames(int start, int end, CancellabilityStates state)
        {
            for (int i = start; i < end; i++)
            {
                Cancellability[i] = state;
            }
        }
        
        
    }
}