using GamePlayScripts.CharacterMoves;
using UnityEngine;

namespace GamePlayScripts
{
    public class HitStunManager
    {
        private Animator _animator;
        private CharacterProperties _properties;

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
                _animator.Play("HitStun");
                
            }

            if (_counter > 0 && _counter <= _properties.HitStunDuration)
            {
                
            }

            _counter++;
        }
    }
}