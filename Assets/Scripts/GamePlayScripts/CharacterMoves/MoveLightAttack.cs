using System;
using UnityEngine;
using UnityEngine.Playables;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveLightAttack : CharacterMove
    {
        public override void InitializeMove(ref CharacterManager manager, Animator animator)
        {
            Animator = animator;
            Manager = manager;
            FrameData = new FrameDataManager(15)
            {
                Damage = 35f,
                Dizzy = 10.0f,
                HitStun = 5.0f,
                BlockStun = 8.0f,
                MeterGain = 10.0f,
                PushBack = -2.5f,
                HitLevel = FrameDataManager.HitLevels.Mid,
                HitType = FrameDataManager.HitTypes.HitStun
            };

            FrameData.SetActionFrames(3, 4);
            FrameData.SetCancellableFrames(0, 14, FrameDataManager.CancellabilityStates.Normal);

            ActionCounter = 0;
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.LightAttackButtonDown == 1;
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
                //Attack while no other attacks are active
                if (Manager.AttackState == CharacterManager.AttackStates.None)
                {
                    FrameData.Update(ActionCounter);
                    Animator.Play("LightAttack");
                    Manager.AttackState = CharacterManager.AttackStates.LightAttack;
                    Manager.Collided = false;
                    Manager.SetFrameDataManager(FrameData);
                    return;
                }
                
                //Detect LightAttack Normal Cancelled into itself
                if (Manager.CurrentState == CharacterManager.CharacterState.Stand &&
                    DetectMoveInput(inputClass))
                {
                    if(Manager.AttackState == CharacterManager.AttackStates.LightAttack && Manager.Collided)
                        if (FrameData.Cancellable == FrameDataManager.CancellabilityStates.Normal)
                        {
                            ActionCounter = 0;
                            FrameData.Update(ActionCounter);
                            Animator.Play("LightAttack");
                            Manager.AttackState = CharacterManager.AttackStates.LightAttack;
                            Manager.Collided = false;
                            Manager.SetFrameDataManager(FrameData);
                            return;
                        }
                }
            }

            //Play out animation and frame information per frame
            if (Manager.AttackState == CharacterManager.AttackStates.LightAttack)
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
                    Manager.LocalHitBoxActive = false;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                
                //Exit Move
                if (FrameData.ActionState == FrameDataManager.ActionFrameStates.None)
                {
                    ActionCounter = 0;
                    Manager.AttackState = CharacterManager.AttackStates.None;
                }
            }
        }
    }
}