using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveMediumAttack : CharacterMove
    {
        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            Animator = animator;
            Properties = properties;
            FrameData = new FrameDataHandler(30)
            {
                Damage = 50.0f,
                Dizzy = 10.0f,
                HitStun = 10.0f,
                BlockStun = 10.0f,
                MeterGain = 10.0f,
                PushBack = -5f
            };
            FrameData.SetActionFrames(6, 4);
            FrameData.SetCancellableFrames(6, 20, FrameDataHandler.CancellabilityStates.Normal);
        }

        public override bool DetectMoveInput(InputClass inputClass)
        {
            return inputClass.MediumAttackButtonDown == 1;
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
                    Animator.Play("MediumAttack");
                    Properties.AttackState = CharacterProperties.AttackStates.MediumAttack;
                    Properties.Collided = false;
                    Properties.FrameDataHandler = FrameData;
                }
                
                //Detect MediumAttack Normal Cancelled into itself
                if (Properties.CurrentState == CharacterProperties.CharacterState.Stand &&
                    DetectMoveInput(inputClass))
                {
                    if(Properties.AttackState == CharacterProperties.AttackStates.LightAttack && Properties.Collided)
                        if (FrameData.Cancellable == FrameDataHandler.CancellabilityStates.Normal)
                        {
                            Properties.AttackFrameCounter = 0;
                            Animator.Play("MediumAttack");
                            Properties.AttackState = CharacterProperties.AttackStates.MediumAttack;
                            Properties.Collided = false;
                            Properties.FrameDataHandler = FrameData;
                        }
                }
            }

            //Play out animation and frame information per frame
            if (Properties.AttackState == CharacterProperties.AttackStates.MediumAttack)
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