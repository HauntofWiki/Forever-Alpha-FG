using System.Collections.Generic;
using GamePlayScripts.CharacterMoves;
using UnityEngine;

namespace GamePlayScripts
{
    public class Character
    {
        //Define General objects and stats
        private GameObject _characterObject;

        private GameObject _opponent;
        //private CharacterController _controller;
        //private CharacterControllerScript _controllerScript;
        private Animator _animator;
        private Player _player;
        private HitStunManager _hitStunManager;
        private InputManager _inputManager;
        public CharacterProperties Properties;
        private List<CharacterMove> _characterMoves;
        
        //Hurtboxes are vulnerable collision boxes
        private List<HurtBox> _hurtBoxes;
        
        
        //Hitboxes are Offensive collision boxes
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
            //_controller = characterGameObject.GetComponent<CharacterController>();
            //_controllerScript = characterGameObject.GetComponent<CharacterControllerScript>();
            _characterObject = characterGameObject;
            _animator = characterGameObject.GetComponent<Animator>();
            _inputManager = inputManager;
            Properties = new CharacterProperties
            {
                MaxHealth = 1000,
                CurrentHealth = 1000,
                WalkForwardXSpeed = .05f,
                WalkBackwardXSpeed = .05f,
                JumpYSpeed = 1.0f,
                PersonalGravity = 24.0f,
                DashForwardXSpeed = new float[] {.1f, .2f, .05f},
                AirDashForwardSpeed = new float[] {2.0f, 2.0f},
                DashBackwardXSpeed = new float[] {.15f, .5f, .1f},
                IsAirborne = false,
                IsIgnoringGravity = false,
                JumpFrameCounter = 0,
                DashFrameCounter = 0,
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
            _hurtBoxes = new List<HurtBox>();
            _hurtBoxes.Add(new HurtBox(GameObject.Find("UpperBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.UpperBody));
            _hurtBoxes.Add(new HurtBox(GameObject.Find("LowerBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.LowerBody));
            
            _hitBoxes = new List<Hitbox>();
            _hitBoxes.Add(new Hitbox(GameObject.Find("HitBox").GetComponent<BoxCollider>(),true));
            
            //Debug.Log(_hitBoxes.Count);
            
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
        
            //_animator = _controller.GetComponent<Animator>();
            
            //Initialize moves.
            foreach (var move in _characterMoves)
            {
                move.InitializeMove(ref Properties, _animator);
            }
        }

        public void Update()
        {
            
            
            if (Properties.CurrentState == CharacterProperties.CharacterState.HitStun)
            {
                _hitStunManager.Update();
            }
            DeterminePlayerSide();
            _inputManager.Update(Properties.CharacterOrientation);
            
            foreach (var move in _characterMoves)
            {
                move.PerformAction(_inputManager.CurrentInput);
            }
            
            ApplyMovement(Properties.CharacterOrientation);
            
            Properties.NewHit = false;
        }
    
        private void ApplyMovement(int currentOrientation)
        {
            if(!Properties.IsIgnoringGravity)
                Properties.MoveDirection.y -= Properties.PersonalGravity * Time.deltaTime;
/*
            //Keep character on the Z-Axis
            if (_controller.transform.position.z != 0)
                Properties.MoveDirection.z = (0 - _controller.transform.position.z); */
            
            Properties.MoveDirection.x *= currentOrientation;
            
            
            var oldPosition = _characterObject.transform.position;
            var newPosition = oldPosition + Properties.MoveDirection;
            if (newPosition.y <= 0f) newPosition.y = 0f;
            
            //Debug.Log(oldPosition + ", " + newPosition);
            _characterObject.transform.position = Vector3.Lerp(oldPosition,newPosition,1f);
            //_controller.Move(Properties.MoveDirection * Time.deltaTime);
            if (_characterObject.transform.position.y == 0)
                Properties.IsGrounded = true;
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
                        Debug.Log(_hitBoxes.Count);
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
            //May want to add statuses or handle more elegantly
            return (Properties.CurrentState != Properties.LastState && Properties.CurrentState != CharacterProperties.CharacterState.JumpForward );
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
