using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class FrameDataManager
    {
        public enum ActionFrameStates
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

        public List<ActionFrameStates> ActionFrameState;
        public List<CancellabilityStates> Cancellability;
        public List<InvincibilityStates> Invincibility;
        public List<bool> AirborneFrames;
        public ActionFrameStates ActionState;
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

        public FrameDataManager(int length)
        {
            Length = length;
            ActionFrameState = new List<ActionFrameStates>();
            Cancellability = new List<CancellabilityStates>();
            Invincibility = new List<InvincibilityStates>();
            AirborneFrames = new List<bool>();
            
            //initialize lists to be composed of 'None'
            for (var i = 0; i <= Length; i++)
            {
                ActionFrameState.Add(ActionFrameStates.None);
                Cancellability.Add(CancellabilityStates.None);
                Invincibility.Add(InvincibilityStates.None);
                AirborneFrames.Add(false);
            }
        }

        public void Update(int frameCounter)
        {
            if (frameCounter > Length) return;
            
            switch (ActionFrameState[frameCounter])
            {
                case ActionFrameStates.Startup:
                    ActionState = ActionFrameStates.Startup;
                    break;
                case ActionFrameStates.Active:
                    ActionState = ActionFrameStates.Active;
                    break;
                case ActionFrameStates.Recovery:
                    ActionState = ActionFrameStates.Recovery;
                    break;
                default:
                    ActionState = ActionFrameStates.None;
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

        //Everything that isn't startup or Active is considered recovery
        public void SetActionFrames(int startUpFrames, int activeFrames)
        {
            for (var i = 0; i < Length; i++)
            {
                if (i < startUpFrames)
                    ActionFrameState[i] = ActionFrameStates.Startup;
                else if (i < (startUpFrames + activeFrames))
                    ActionFrameState[i] = ActionFrameStates.Active;
                else if (i >= (startUpFrames + activeFrames))
                    ActionFrameState[i] = ActionFrameStates.Recovery;
                else ActionFrameState[i] = ActionFrameStates.None;
            }
        }

        public void SetCancellableFrames(int start, int end, CancellabilityStates state)
        {
            for (int i = start; i < end; i++)
            {
                Cancellability[i] = state;
            }
        }
        
        public void SetInvincibleFrames(int start, int end, InvincibilityStates state)
        {
            for (int i = start; i < end; i++)
            {
                Invincibility[i] = state;
            }
        }

        public void SetAirborneFrames(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                Airborne = true;
            }
        }

        //Sets all fields to 0, for some moves that will not use this data
        public void SetFieldsZero()
        {
            Damage = 0;
            Dizzy = 0;
            MeterGain = 0;
            PushBack = 0;
            HitStun = 0;
            BlockStun = 0;
        }
        
        
    }
}