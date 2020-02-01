using System.Collections.Generic;
using GamePlayScripts.CharacterMoves;
using UnityEngine;

namespace GamePlayScripts
{
    public class Character
    {
        //Define General objects and stats
        private GameObject _characterObject;
        private CharacterController _controller;
        private CharacterControllerScript _controllerScript;
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
        private CharacterMove _moveSpecialForward;
        private CharacterMove _moveAirDashForward;
        private CharacterMove _moveLightAttack;

        public Character(GameObject characterGameObject, InputManager inputManager)
        {
            _controller = characterGameObject.GetComponent<CharacterController>();
            _controllerScript = characterGameObject.GetComponent<CharacterControllerScript>();
            _animator = characterGameObject.GetComponent<Animator>();
            _inputManager = inputManager;
            Properties = new CharacterProperties
            {
                MaxHealth = 1000,
                CurrentHealth = 1000,
                WalkForwardXSpeed = 4.0f,
                WalkBackwardXSpeed = 4.0f,
                JumpYSpeed = 15.0f,
                PersonalGravity = 24.0f,
                DashForwardXSpeed = new float[] {5.5f, 9.75f, 2.0f},
                AirDashForwardSpeed = new float[] {15.0f, 15.0f},
                DashBackwardXSpeed = new float[] {7.0f, 20.0f, 3.0f},
                IsAirborne = false,
                IsIgnoringGravity = false,
                JumpFrameCounter = 0,
                DashFrameCounter = 0,
                AttackFrameCounter = 0,
                MoveDirection = new Vector3(0,0,0),
                CurrentState = CharacterProperties.CharacterState.Stand,
                LastState = CharacterProperties.CharacterState.None,
                AttackState = CharacterProperties.AttackStates.None,
                CancellableState = CharacterProperties.CancellableStates.None,
                CharacterController = _controller
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
            
            Debug.Log(_hitBoxes.Count);
            
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
            _characterMoves.Add(new MoveSpecialForward());
            _characterMoves.Add(new MoveAirDashForward());
            _characterMoves.Add(new MoveLightAttack());
        
            _animator = _controller.GetComponent<Animator>();
            
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
            _controllerScript.DeterminePlayerSide();
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


            Properties.MoveDirection.x *= currentOrientation;
            _controller.Move(Properties.MoveDirection * Time.deltaTime);
        }

        public CollisionInformation DetectCollisions(List<HurtBox> hurtBoxes)
        {
            foreach (var v in _hitBoxes)
            {
                //If a local hitbox is not active than no collision
                if (!Properties.localHitBoxActive && v.LocalHitBox) return new CollisionInformation(){ Collided = false };
                //Move already collided
                if (Properties.Collided) return new CollisionInformation(){ Collided = false };
                foreach (var w in hurtBoxes)
                {
                    
                    if (v.Intersects(w.GetHurtBoxBounds()))
                    {
                        Properties.ComboActive = true;
                        Properties.Collided = true;
                        return new CollisionInformation()
                        {
                            Damage = 100,
                            HitStunAmount = 30,
                            BlockStunAmount = 8,
                            Zone = HurtBox.HurtZone.UpperBody,
                            Collided = true
                        };
                    }
                }
            }

            return new CollisionInformation(){ Collided = false };
        }

        public void ApplyCollision(CollisionInformation collisionInformation)
        {
            Properties.CurrentHealth -= collisionInformation.Damage;
            Properties.CurrentState = CharacterProperties.CharacterState.HitStun;
            Properties.HitStunDuration = collisionInformation.HitStunAmount;
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
            _controllerScript.InstantiateCharacterController(opponent, ref Properties);
            _controllerScript.DeterminePlayerSide();
        }
    }
}
