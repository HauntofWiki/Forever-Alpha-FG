using System.Collections.Generic;
using UnityEngine;

namespace GamePlayScripts
{
    public class CollisionManager
    {
        public List<HurtBox> HurtBoxes { get; }
        public List<HitBox> HitBoxes { get; }
        public List<PushBox> PushBoxes { get; }
        
        private Animator _animator;
        private CharacterProperties _properties;
        private AnimationClip[] _animationClip;

        private int _counter = 0;
        public float PushBack { get; set; }

        public CollisionManager(Animator animator, ref CharacterProperties properties)
        {
            HurtBoxes = new List<HurtBox>();
            HitBoxes = new List<HitBox>();
            PushBoxes = new List<PushBox>();
            
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
        
        public void Add(PushBox box)
        {
            PushBoxes.Add(box);
        }
        
        public void Remove(PushBox box)
        {
            PushBoxes.Remove(box);
        }

        
        
    }
}