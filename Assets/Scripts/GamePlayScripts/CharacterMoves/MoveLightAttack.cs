using System;
using UnityEngine;
using UnityEngine.Playables;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveLightAttack : CharacterMove
    {
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Animator = animator;
            Properties = properties;
            FrameData = new FrameDataHandler(15)
            {
                Damage = 35.0f,
                Dizzy = 10.0f,
                HitStun = 5.0f,
                BlockStun = 10.0f,
                MeterGain = 10.0f,
                PushBack = -2.5f
            };

            FrameData.SetActionFrames(3, 4);
            FrameData.SetCancellableFrames(2, 12, FrameDataHandler.CancellabilityStates.Normal);

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
            if (Properties.CurrentState == CharacterProperties.CharacterState.Stand && DetectMoveInput(inputClass))
            {
                //Attack while no other attacks are active
                if (Properties.AttackState == CharacterProperties.AttackStates.None)
                {
                    Properties.AttackFrameCounter = 0;
                    FrameData.Update(ActionCounter);
                    Animator.Play("LightAttack");
                    Properties.AttackState = CharacterProperties.AttackStates.LightAttack;
                    Properties.Collided = false;
                    Properties.FrameDataHandler = FrameData;
                    return;
                }
                
                //Detect LightAttack Normal Cancelled into itself
                if (Properties.CurrentState == CharacterProperties.CharacterState.Stand &&
                    DetectMoveInput(inputClass))
                {
                    if(Properties.AttackState == CharacterProperties.AttackStates.LightAttack && Properties.Collided)
                        if (FrameData.Cancellable == FrameDataHandler.CancellabilityStates.Normal)
                        {
                            ActionCounter = 0;
                            FrameData.Update(ActionCounter);
                            Animator.Play("LightAttack");
                            Properties.AttackState = CharacterProperties.AttackStates.LightAttack;
                            Properties.Collided = false;
                            Properties.FrameDataHandler = FrameData;
                            return;
                        }
                }
            }

            //Play out animation and frame information per frame
            if (Properties.AttackState == CharacterProperties.AttackStates.LightAttack)
            {
                //Startup
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Startup)
                {
                    Properties.LocalHitBoxActive = false;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Active
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Active)
                {
                    Properties.LocalHitBoxActive = true;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                //Recovery
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Recovery)
                {
                    Properties.Collided = false;
                    Properties.LocalHitBoxActive = false;
                    FrameData.Update(ActionCounter++);
                    return;
                }
                
                //Exit Move
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.None)
                {
                    ActionCounter = 0;
                    Properties.AttackState = CharacterProperties.AttackStates.None;
                }
            }
        }
    }
}