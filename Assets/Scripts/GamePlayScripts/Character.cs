using System;
using System.Collections.Generic;
using System.Threading;
using GamePlayScripts.CharacterMoves;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlayScripts
{
    public class Character
    {
        //Define General objects and stats
        private GameObject _characterObject;
        private float _leftScreenBarrier;
        private float _rightScreenBarrier;

        private GameObject _opponent;
        //private CharacterController _controller;
        //private CharacterControllerScript _controllerScript;
        private Animator _animator;
        private Player _player;
        private HitStunManager _hitStunManager;
        private InputManager _inputManager;
        public CharacterProperties Properties;
        private List<CharacterMove> _characterMoves;
        
        //Hurt Boxes are vulnerable collision boxes
        private List<HurtBox> _hurtBoxes;
        
        //Hit Boxes are Offensive collision boxes
        //Most of the time there is only one, but there will be occasions where there are multiple
        private List<Hitbox> _hitBoxes;

        //Define Character Moves
        private CharacterMove _moveStandIdle;
        private CharacterMove _moveWalkForward;
        private CharacterMove _moveWalkBackward;
        private CharacterMove _moveJumpNeutral;
        private CharacterMove _moveJumpForward;
        private CharacterMove _moveJumpBackward;
        private CharacterMove _moveDashForward;
        private CharacterMove _moveDashBackward;
        private CharacterMove _moveAirDashForward;
        private CharacterMove _moveLightAttack;

        public Character(GameObject characterGameObject, InputManager inputManager)
        {
            _characterObject = characterGameObject;
            _animator = characterGameObject.GetComponent<Animator>();
            _inputManager = inputManager;
            Properties = new CharacterProperties
            {
                MaxHealth = 1000,
                CurrentHealth = 1000,
                WalkForwardXSpeed = 3.5f,
                WalkBackwardXSpeed = 3.25f,
                JumpYSpeed = 8f,
                PersonalGravity = 15f,
                DashForwardXSpeed = new float[] {5f, 8f, 3f},
                AirDashForwardSpeed = new float[] {15.0f, 8.0f},
                DashBackwardXSpeed = new float[] {15f, 15f, 8f},
                IsAirborne = false,
                IsIgnoringGravity = false,
                AttackFrameCounter = 0,
                MoveDirection = new Vector3(0,0,0),
                CurrentState = CharacterProperties.CharacterState.Stand,
                LastState = CharacterProperties.CharacterState.None,
                AttackState = CharacterProperties.AttackStates.None,
                CancellableState = CharacterProperties.CancellableStates.None
            };
            
            characterGameObject.GetComponentInChildren<Camera>().targetTexture =
                (RenderTexture) Resources.Load("Textures/Player 1 Render Texture");
            _hitStunManager = new HitStunManager(_animator, ref Properties);
            
            //Define and populate collision boxes
            //HurtBoxes - Boxes which receive attacks
            _hurtBoxes = new List<HurtBox>();
            _hurtBoxes.Add(new HurtBox(GameObject.Find("UpperBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.UpperBody));
            _hurtBoxes.Add(new HurtBox(GameObject.Find("LowerBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.LowerBody));
            //HitBoxes
            _hitBoxes = new List<Hitbox>();
            _hitBoxes.Add(new Hitbox(GameObject.Find("HitBox").GetComponent<BoxCollider>(),true));
            //PushBox
            
            
            
            _characterMoves = new List<CharacterMove>();

            //Add Moves to List. Order can effect priority.
            _characterMoves.Add(new MoveStandIdle());
            _characterMoves.Add(new MoveWalkForward());
            _characterMoves.Add(new MoveWalkBackward());
            _characterMoves.Add(new MoveJumpForward());
            _characterMoves.Add(new MoveJumpBackward());
            _characterMoves.Add(new MoveJumpNeutral());
            _characterMoves.Add(new MoveDashForward());
            _characterMoves.Add(new MoveDashBackward());
            _characterMoves.Add(new MoveAirDashForward());
            _characterMoves.Add(new MoveLightAttack());
            _characterMoves.Add(new MoveMediumAttack());
        
            //Initialize moves.
            foreach (var move in _characterMoves)
            {
                move.InitializeMove(ref Properties, _animator);
            }
        }

        public void Update()
        {
            //Check if character was hit
            if (Properties.CurrentState == CharacterProperties.CharacterState.HitStun)
            {
                _hitStunManager.Update();
            }
            //Check if we need to switch orientation
            DeterminePlayerSide();
            //Get Inputs
            _inputManager.Update(Properties.CharacterOrientation);
            //Update Moves
            foreach (var move in _characterMoves)
            {
                move.PerformAction(_inputManager.CurrentInput);
            }
            //Move Character
            ApplyMovement(Properties.CharacterOrientation);
            //Reset NewHit
            Properties.NewHit = false;
        }
    
        private void ApplyMovement(int currentOrientation)
        {
            //Apply gravity
            if(!Properties.IsIgnoringGravity && Properties.IsGrounded == false)
                Properties.MoveDirection.y -= Properties.PersonalGravity * Time.deltaTime;

            /*
            //Keep character on the Z-Axis
            if (_controller.transform.position.z != 0)
                Properties.MoveDirection.z = (0 - _characterObject.transform.position.z);
            */

            var position = _characterObject.transform.position;
            Properties.MoveDirection.x *= currentOrientation;
            
            //Keep Object above the floor
            var potentialY = position.y + (Properties.MoveDirection.y * Time.deltaTime);
            if (potentialY  <= Constants.Floor)
            {
                Properties.MoveDirection.y = Constants.Floor - (position.y);
            }

            //Keep characters within screen boundaries
            var center = (position.x + _opponent.transform.position.x) / 2;
            _leftScreenBarrier = center - Constants.MaxDistance;
            _rightScreenBarrier = center + Constants.MaxDistance;

            var potentialX = position.x + (Properties.MoveDirection.x * Time.deltaTime);
            if (potentialX <= _leftScreenBarrier)
            {
                Properties.MoveDirection.x = _leftScreenBarrier - (position.x);
            }
            else if (potentialX >= _rightScreenBarrier)
            {
                Properties.MoveDirection.x = _rightScreenBarrier - (position.x);
            }
            
            //Keep character within stage boundaries
            var stageLeftBarrier = -Constants.MaxStage;
            var stageRightBarrier = Constants.MaxStage;
            
            if (potentialX <= stageLeftBarrier)
            {
                Properties.MoveDirection.x = stageLeftBarrier - (position.x);
            }
            else if (potentialX >= stageRightBarrier)
            {
                Properties.MoveDirection.x = stageRightBarrier - (position.x);
            }
            
            //Actually move the character according to all the previous calculations
            _characterObject.transform.position += (Properties.MoveDirection * Time.deltaTime);
            
            //Determine is character is grounded
            Properties.IsGrounded = _characterObject.transform.position.y < Constants.FloorBuffer;
        }

        public bool DetectCollisions(List<HurtBox> hurtBoxes)
        {
            foreach (var v in _hitBoxes)
            {
                //If a local hitbox is not active than no collision
                if (!Properties.LocalHitBoxActive && v.LocalHitBox) return false;
                
                //Move already collided
                if (Properties.Collided) return false;
                
                foreach (var w in hurtBoxes)
                {
                    if (v.Intersects(w.GetHurtBoxBounds()))
                    {
                        Properties.ComboActive = true;
                        Properties.Collided = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public void ApplyCollision(FrameDataHandler handler)
        {
            Properties.CurrentHealth -= handler.Damage;
            Properties.CurrentState = CharacterProperties.CharacterState.HitStun;
            Properties.FrameDataHandler.HitStun = handler.HitStun;
            _hitStunManager.PushBack = handler.PushBack;
        }
        
        //Determine which side the player is on
        public void DeterminePlayerSide()
        {
            
            if (_characterObject.transform.position.x > _opponent.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(-1,1,1);
                Properties.CharacterOrientation = -1;
                _characterObject.transform.localScale = Vector3.Lerp(_characterObject.transform.localScale,flipModel, 2.0f);
            }
            else if (_characterObject.transform.position.x < _opponent.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(1,1,1);
                Properties.CharacterOrientation = 1;
                _characterObject.transform.localScale = Vector3.Lerp(_characterObject.transform.localScale,flipModel, 2.0f);
            }
        }

        private bool CanSwitchOrientation()
        {
            //TODO: make a list for different states, i.e grounded is usually fine, but a forward dash under a player should not instantly turn around
            return Properties.IsGrounded;
        }

        public CharacterProperties GetCharacterProperties()
        {
            return Properties;
        }
        
        public List<HurtBox> GetHurtBoxes()
        {
            return _hurtBoxes;
        }

        public void PostLoadSetup(GameObject opponent)
        {
            _opponent = opponent;
            DeterminePlayerSide();
        }
    }
}
