using System.Collections.Generic;
using UnityEngine;

namespace GamePlayScripts
{
    public class CollisionManager
    {
        public List<HurtBox> HurtBoxes { get; }
        public List<HitBox> HitBoxes { get; }
        public PushBox PushBox { get; set; }
        
        private Animator _animator;
        private CharacterManager _manager;
        private AnimationClip[] _animationClip;

        private int _counter = 0;

        public CollisionManager(Animator animator, ref CharacterManager manager)
        {
            HurtBoxes = new List<HurtBox>();
            HitBoxes = new List<HitBox>();
            
            _animator = animator;
            _manager = manager;
            
        }
        
        public void Update(float pushBack)
        {
            if (_manager.NewHit)
                _counter = 0;
            
            if (_counter == 0)
            {
                _animator.SetFloat("HitStunAmount",1);
                _animator.Play("HitStunBlendTree");
                
                _manager.MoveDirection = new Vector3(_manager.GetPushBack(),0,0);
            }

            if (_counter > 0 && _counter <= _manager.GetHitStun())
            {
                _manager.MoveDirection = new Vector3(pushBack,0,0);
            }

            if (_counter >= _manager.GetHitStun())
            {
                _manager.CurrentState = CharacterManager.CharacterState.Stand;
                _counter = 0;
                return;
            }

            _counter++;
        }
        
        public void Add(HurtBox box)
        {
            HurtBoxes.Add(box);
        }
        
        public void Remove(HurtBox box)
        {
            HurtBoxes.Remove(box);
        }
        
        public void Add(HitBox box)
        {
            HitBoxes.Add(box);
        }
        
        public void Remove(HitBox box)
        {
            HitBoxes.Remove(box);
        }
    }
}