using UnityEngine;

namespace GamePlayScripts.CharacterMoves
{
    public class MoveMediumAttack : CharacterMove
    {
        private CharacterProperties _properties;
        private Animator _animator;
        private FrameDataHandler _frameData;

        public override void InitializeMove(ref CharacterProperties properties, Animator animator)
        {
            _animator = animator;
            _properties = properties;
            _frameData = new FrameDataHandler(30)
            {
                Damage = 50.0f,
                Dizzy = 10.0f,
                HitStun = 10.0f,
                BlockStun = 10.0f,
                MeterGain = 10.0f,
                PushBack = -5f
            };
            _frameData.SetAttackFrames(6, 4);
            _frameData.SetCancellableFrames(6, 20, FrameDataHandler.CancellabilityStates.Normal);
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
            if (_properties.CurrentState == CharacterProperties.CharacterState.Stand && DetectMoveInput(inputClass))
            {
                //Attack while no other attacks are active
                if (_properties.AttackState == CharacterProperties.AttackStates.None)
                {
                    _properties.AttackFrameCounter = 0;
                    _animator.Play("MediumAttack");
                    _properties.AttackState = CharacterProperties.AttackStates.MediumAttack;
                    _properties.Collided = false;
                    _properties.FrameDataHandler = _frameData;
                }
                
                //Detect MediumAttack Normal Cancelled into itself
                if (_properties.CurrentState == CharacterProperties.CharacterState.Stand &&
                    DetectMoveInput(inputClass))
                {
                    if(_properties.AttackState == CharacterProperties.AttackStates.LightAttack && _properties.Collided)
                        if (_frameData.Cancellable == FrameDataHandler.CancellabilityStates.Normal)
                        {
                            _properties.AttackFrameCounter = 0;
                            _animator.Play("MediumAttack");
                            _properties.AttackState = CharacterProperties.AttackStates.MediumAttack;
                            _properties.Collided = false;
                            _properties.FrameDataHandler = _frameData;
                        }
                }
            }

            //Play out animation and frame information per frame
            if (_properties.AttackState == CharacterProperties.AttackStates.MediumAttack)
            {
                //Startup
                if (_frameData.AttackState == FrameDataHandler.AttackFrameStates.Startup)
                {
                    _properties.LocalHitBoxActive = false;
                    _properties.AttackFrameCounter++;
                }
                //Active
                if (_frameData.AttackState == FrameDataHandler.AttackFrameStates.Active)
                {
                    _properties.LocalHitBoxActive = true;
                    _properties.AttackFrameCounter++;
                }
                //Recovery
                if (_frameData.AttackState == FrameDataHandler.AttackFrameStates.Recovery)
                {
                    _properties.Collided = false;
                    _properties.LocalHitBoxActive = false;
                    _properties.AttackFrameCounter++;
                }
                
                //Exit Move
                if (_properties.AttackFrameCounter >= _frameData.Length)
                {
                    _properties.AttackState = CharacterProperties.AttackStates.None;
                }
                
                _frameData.Update(_properties.AttackFrameCounter);
            }
            
            
        }
    }
}