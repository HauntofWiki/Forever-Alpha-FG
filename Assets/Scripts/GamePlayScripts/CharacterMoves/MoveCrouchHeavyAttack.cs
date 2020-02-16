using System;
using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveCrouchHeavyAttack : CharacterMove
    {
        public override void InitializeMove(ref CharacterManager manager, Animator animator)
        {
            Animator = animator;
            Manager = manager;
            FrameData = new FrameDataManager(35)
            {
                Damage = 50.0f,
                Dizzy = 10.0f,
                HitStun = 10.0f,
                BlockStun = 10.0f,
                MeterGain = 10.0f,
                PushBack = -3f,
                HitLevel = FrameDataManager.HitLevels.Low,
                HitType = FrameDataManager.HitTypes.HardKnockDown
            };
            FrameData.SetActionFrames(5, 5);
            FrameData.SetCancellableFrames(5, 34, FrameDataManager.CancellabilityStates.Normal);
            ActionCounter = 0;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.HeavyAttackButtonDown == 1;
        }

        public override bool DetectHoldInput(InputClass inputClass)
        {
            return false;
        }

        public override void PerformAction(InputClass inputClass)
        {
            //Detect input
            if (Manager.CurrentState == CharacterManager.CharacterState.Crouch && DetectMoveInput(inputClass))
            {
                //Attack while no other attacks are active
                if (Manager.AttackState == CharacterManager.AttackStates.None)
                {
                    ActionCounter = 0;
                    FrameData.Update(ActionCounter);
                    Animator.Play("CrouchHeavyAttack");
                    Manager.AttackState = CharacterManager.AttackStates.CrouchHeavyAttack;
                    Manager.Collided = false;
                    Manager.SetFrameDataManager(FrameData);
                    return;
                }
                
                //Detect Normal Cancelled into this move
                if (Manager.CurrentState == CharacterManager.CharacterState.Stand &&
                    DetectMoveInput(inputClass))
                {
                    if(Manager.AttackState == CharacterManager.AttackStates.MediumAttack && Manager.Collided)
                        if (FrameData.Cancellable == FrameDataManager.CancellabilityStates.Normal)
                        {
                            ActionCounter = 0;
                            FrameData.Update(ActionCounter);
                            Animator.Play("CrouchHeavyAttack");
                            Manager.AttackState = CharacterManager.AttackStates.CrouchHeavyAttack;
                            Manager.Collided = false;
                            Manager.SetFrameDataManager(FrameData);
                            return;
                        }
                }
            }

            //Play out animation and frame information per frame
            if (Manager.AttackState == CharacterManager.AttackStates.CrouchHeavyAttack)
            {
                //Startup
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Startup)
                {
                    Manager.LocalHitBoxActive = false;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Active
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Active)
                {
                    Manager.LocalHitBoxActive = true;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Recovery
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.Recovery)
                {
                    Manager.Collided = false;
                    Manager.LocalHitBoxActive = false;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                
                //Exit Move
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.None)
                {
                    Manager.AttackState = CharacterManager.AttackStates.None;
                }
            }
        }
    }
}