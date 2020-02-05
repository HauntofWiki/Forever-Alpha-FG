﻿using UnityEngine;
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
                PushBack = -5f
            };

            FrameData.SetActionFrames(3, 4);
            FrameData.SetCancellableFrames(2, 12, FrameDataHandler.CancellabilityStates.Normal);
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
                    Animator.Play("LightAttack");
                    Properties.AttackState = CharacterProperties.AttackStates.LightAttack;
                    Properties.Collided = false;
                    Properties.FrameDataHandler = FrameData;
                }
                
                //Detect LightAttack Normal Cancelled into itself
                if (Properties.CurrentState == CharacterProperties.CharacterState.Stand &&
                    DetectMoveInput(inputClass))
                {
                    if(Properties.AttackState == CharacterProperties.AttackStates.LightAttack && Properties.Collided)
                        if (FrameData.Cancellable == FrameDataHandler.CancellabilityStates.Normal)
                        {
                            Properties.AttackFrameCounter = 0;
                            Animator.Play("LightAttack");
                            Properties.AttackState = CharacterProperties.AttackStates.LightAttack;
                            Properties.Collided = false;
                            Properties.FrameDataHandler = FrameData;
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
                    Properties.AttackFrameCounter++;
                }
                //Active
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Active)
                {
                    Properties.LocalHitBoxActive = true;
                    Properties.AttackFrameCounter++;
                }
                //Recovery
                if (FrameData.ActionState == FrameDataHandler.ActionFrameStates.Recovery)
                {
                    Properties.Collided = false;
                    Properties.LocalHitBoxActive = false;
                    Properties.AttackFrameCounter++;
                }
                
                //Exit Move
                if (Properties.AttackFrameCounter >= FrameData.Length)
                {
                    Properties.AttackState = CharacterProperties.AttackStates.None;
                }
                
                FrameData.Update(Properties.AttackFrameCounter);
            }
        }
    }
}