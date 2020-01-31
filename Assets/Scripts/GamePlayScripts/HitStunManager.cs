using GamePlayScripts.CharacterMoves;
using UnityEngine;

namespace GamePlayScripts
{
    public class HitStunManager
    {
        private Animator _animator;
        private CharacterProperties _properties;
        private AnimationClip[] _animationClip;
        private float _hitstunLength;

        private int _counter = 0;

        public HitStunManager(Animator animator, ref CharacterProperties properties)
        {
            _animator = animator;
            _properties = properties;
        }
        
        public void Update()
        {
            if (_counter == 0)
            {
                _animator.SetFloat("HitStunAmount",1);
                _animator.Play("HitStunBlendTree");
            }

            if (_counter > 0 && _counter <= _properties.HitStunDuration)
            {
                
            }

            if (_counter >= _properties.HitStunDuration)
            {
                _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                _counter = 0;
                return;
            }

            _counter++;
        }
    }
}