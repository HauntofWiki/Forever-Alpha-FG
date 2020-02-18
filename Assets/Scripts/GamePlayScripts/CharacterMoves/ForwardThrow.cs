using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class ForwardThrow : CharacterMove
    {
        public override void InitializeMove(ref CharacterManager manager, Animator animator)
        {
            Animator = animator;
            Manager = manager;
            FrameData = new FrameDataManager(1)
            {
                Damage = 0.00f,
                Dizzy = 0.00f,
                HitStun = 0.00f,
                BlockStun = 0.00f,
                MeterGain = 0.00f,
                PushBack = 0.00f,
                HitLevel = FrameDataManager.HitLevels.Mid,
                HitType = FrameDataManager.HitTypes.Throw
            };

            FrameData.SetActionFrames(0, 1, true);
            //FrameData.SetCancellableFrames(0, 14, FrameDataManager.CancellabilityStates.Normal);

            ActionCounter = 0;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            //Debug.Log("DistanceDelta: " + Manager.DistanceDelta);
            if (inputClass.DPadX > 0 && 
                inputClass.HeavyAttackButtonDown == 1 && 
                (Manager.DistanceDelta < 0.50f && Manager.DistanceDelta > -0.50f))
            {
                Debug.Log("Detected throw!");
                return true;
            }
            else
                return false;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            //Detect input
            if (Manager.CurrentState == CharacterManager.CharacterState.Stand && DetectMoveInput(inputClass))
            {
                Debug.Log("PERFORM ACTION THROW");
                Debug.Log("Current AttackState: " + Manager.AttackState);
                if (Manager.AttackState == CharacterManager.AttackStates.None)
                {
                    Debug.Log("THROW STARTING!");
                    ActionCounter = 0;
                    FrameData.Update(ActionCounter);
                    //Animator.Play("ForwardThrow");
                    Manager.AttackState = CharacterManager.AttackStates.Throwing;
                    Manager.Collided = false;
                    Manager.SetFrameDataManager(FrameData);
                    return;
                }

            }
            if (Manager.AttackState == CharacterManager.AttackStates.Throwing)
            {
                Debug.Log("ActionState: " + FrameData.ActionState);
                //Active
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Active)
                {
                    Debug.Log("THROW ACTIVE!");
                    Manager.LocalHitBoxActive = true;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Exit Move
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.None)
                {
                    Debug.Log("EXITING THROW!");
                    Manager.AttackState = CharacterManager.AttackStates.None;
                }
            }
        }
    }
}
