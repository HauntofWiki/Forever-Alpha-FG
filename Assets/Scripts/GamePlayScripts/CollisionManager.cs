using System.Collections.Generic;
using UnityEditor.Experimental;
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
        
        public void UpdateHitStun(float pushBack)
        {
            if (_manager.NewHit)
                _counter = 0;
            
            if (_counter == 0)
            {
                if (_manager.CurrentState == CharacterManager.CharacterState.StandingHitStun)
                {
                    _animator.SetFloat("HitStunAmount", 1);
                    _animator.Play("HitStunBlendTree");

                    _manager.MoveDirection = new Vector3(_manager.GetPushBack(), 0, 0);
                }

                if (_manager.CurrentState == CharacterManager.CharacterState.CrouchingHitStun)
                {
                    _animator.Play("CrouchHitStun");

                    _manager.MoveDirection = new Vector3(_manager.GetPushBack(), 0, 0);
                }
                
                if (_manager.CurrentState == CharacterManager.CharacterState.StandingBlockStun)
                {
                    //_animator.SetFloat("HitStunAmount", 1);
                    _animator.Play("Block");

                    _manager.MoveDirection = new Vector3(_manager.GetPushBack(), 0, 0);
                }
                
                if (_manager.CurrentState == CharacterManager.CharacterState.CrouchingBlockStun)
                {
                    _animator.Play("CrouchBlock");

                    _manager.MoveDirection = new Vector3(_manager.GetPushBack(), 0, 0);
                }
                //Debug.Log(_manager.CurrentState);
            }

            if (_counter > 0 && _counter <= _manager.GetHitStun())
            {
                _manager.MoveDirection = new Vector3(pushBack,0,0);
            }

            if (_counter >= _manager.GetHitStun())
            {
                switch (_manager.CurrentState)
                {
                    case CharacterManager.CharacterState.StandingBlockStun:
                        _manager.CurrentState = CharacterManager.CharacterState.Stand;
                        break;
                    case CharacterManager.CharacterState.CrouchingBlockStun:
                        _manager.CurrentState = CharacterManager.CharacterState.Crouch;
                        break;
                    case CharacterManager.CharacterState.StandingHitStun:
                        _manager.CurrentState = CharacterManager.CharacterState.Stand;
                        break;
                    case CharacterManager.CharacterState.CrouchingHitStun:
                        _manager.CurrentState = CharacterManager.CharacterState.Crouch;
                        break;
                }
                
                _counter = 0;
                return;
            }
            //Debug.Log(_manager.CurrentState);
            _counter++;
        }

        public void UpdateJuggle()
        {
            if (_manager.NewHit)
            {
                _counter = 0;
            }

            if (_counter == 0)
            {
                _animator.Play("Juggle");
                _manager.MoveDirection = new Vector3(0,5,0);
                
            }
            
            if (_manager.Grounded && _counter >= 20)
            {
                _manager.CurrentState = CharacterManager.CharacterState.HardKnockDown;
            }

            _counter++;
        }

        public void UpdateSoftKnockDown()
        {

        }

        public void UpdateHardKnockDown()
        {
            if (_manager.NewHit && _manager.CurrentState != CharacterManager.CharacterState.HardKnockDown)
            {
                _counter = 0;
            }

            if (_counter == 0)
            {
                _animator.Play("KnockDown");
            }

            if (_counter >= _manager.KnockDownDuration / 2 && _counter < _manager.KnockDownDuration)
            {
                //_manager.CurrentState = CharacterManager.CharacterState.WakeUp;
            }

            if (_counter >= _manager.KnockDownDuration)
            {
                _counter = 0;
                _manager.CurrentState = CharacterManager.CharacterState.Stand;
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