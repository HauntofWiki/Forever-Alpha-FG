using GamePlayScripts.CharacterMoves;
using UnityEngine;

namespace GamePlayScripts
{
    public class HitStunManager
    {
        private Animator _animator;
        private CharacterProperties _properties;
        private AnimationClip[] _animationClip;

        private int _counter = 0;
        public float PushBack { get; set; }

        public HitStunManager(Animator animator, ref CharacterProperties properties)
        {
            _animator = animator;
            _properties = properties;
        }
        
        public void Update()
        {
            if (_properties.NewHit)
                _counter = 0;
            
            if (_counter == 0)
            {
                _animator.SetFloat("HitStunAmount",1);
                _animator.Play("HitStunBlendTree");
                
                _properties.MoveDirection = new Vector3(PushBack,0,0);
            }

            if (_counter > 0 && _counter <= _properties.FrameDataHandler.HitStun)
            {
                _properties.MoveDirection = new Vector3(PushBack,0,0);
            }

            if (_counter >= _properties.FrameDataHandler.HitStun)
            {
                _properties.CurrentState = CharacterProperties.CharacterState.Stand;
                _counter = 0;
                return;
            }

            _counter++;
        }
    }
}