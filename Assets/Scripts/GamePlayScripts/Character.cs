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
        private InputManager _inputManager;
        public CharacterProperties Properties;
        private List<CharacterMove> _characterMoves;
        
        //Hurt Boxes are vulnerable collision boxes
        private List<HurtBox> _hurtBoxes;
        
        //Hit Boxes are Offensive collision boxes
        //Most of the time there is only one, but there will be occasions where there are multiple
        private List<HitBox> _hitBoxes;
        
        //Push Box is a box that doesnt let characters pass through one another
        private PushBox _pushBox;

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
                IgnoringGravity = false,
                AttackFrameCounter = 0,
                MoveDirection = new Vector3(0,0,0),
                CurrentState = CharacterProperties.CharacterState.Stand,
                LastState = CharacterProperties.CharacterState.None,
                AttackState = CharacterProperties.AttackStates.None,
                CancellableState = CharacterProperties.CancellableStates.None
            };
            
            Properties.CollisionManager = new CollisionManager(_animator, ref Properties);
            characterGameObject.GetComponentInChildren<Camera>().targetTexture =
                (RenderTexture) Resources.Load("Textures/Player 1 Render Texture");
            
            //Define and populate collision boxes
            Properties.CollisionManager.Add(new HurtBox(GameObject.Find("UpperBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.UpperBody));
            Properties.CollisionManager.Add(new HurtBox(GameObject.Find("LowerBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.LowerBody));
            Properties.CollisionManager.Add(new HitBox(GameObject.Find("HitBox").GetComponent<BoxCollider>(),true));
            Properties.CollisionManager.Add(new PushBox(GameObject.Find("PushBox").GetComponent<BoxCollider>()));
            
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

        public void Update(Character opponent)
        {
            //Check if character was hit
            if (Properties.CurrentState == CharacterProperties.CharacterState.HitStun)
            {
                Properties.CollisionManager.Update();
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
            
            //Detect Push
            DetectPush(opponent);
            //Move Character
            ApplyMovement(Properties.CharacterOrientation);
            //Reset NewHit
            Properties.NewHit = false;
        }
    
        private void ApplyMovement(int currentOrientation)
        {
            //Apply gravity
            if(!Properties.IgnoringGravity && Properties.Grounded == false)
                Properties.MoveDirection.y -= Properties.PersonalGravity * Time.deltaTime;
            
            var position = _characterObject.transform.position;
            Properties.MoveDirection.x *= currentOrientation;
            
            //Keep Object above the floor
            var potentialY = position.y + (Properties.MoveDirection.y * Time.deltaTime);
            if (potentialY  <= Constants.Floor)
            {
                Properties.MoveDirection.y = Constants.Floor - (position.y);
            }
            
            var potentialX = position.x + (Properties.MoveDirection.x * Time.deltaTime);

            //Keep characters within screen boundaries
            var center = (position.x + _opponent.transform.position.x) / 2;
            _leftScreenBarrier = center - Constants.MaxDistance;
            _rightScreenBarrier = center + Constants.MaxDistance;

            
            if (potentialX <= _leftScreenBarrier)
            {
                Properties.MoveDirection.x = _leftScreenBarrier - (position.x);
            }
            else if (potentialX >= _rightScreenBarrier)
            {
                Properties.MoveDirection.x = _rightScreenBarrier - (position.x);
            }
            
            //Keep character within stage boundaries
            var stageLeftBarrier = -Constants.MaxStageX;
            var stageRightBarrier = Constants.MaxStageX;
            
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
            Properties.Grounded =_characterObject.transform.position.y < Constants.FloorBuffer;
        }

        public bool DetectCollisions(CollisionManager opponentCollisionManager)
        {
            //Detect HitBox Collisions
            var hurtBoxes = opponentCollisionManager.HurtBoxes;
            foreach (var hitBox in Properties.CollisionManager.HitBoxes)
            {
                //If a local HitBox is not active than no collision
                if (!Properties.LocalHitBoxActive && hitBox.LocalHitBox) return false;

                //Move already collided
                if (Properties.Collided) return false;

                foreach (var hurtBox in hurtBoxes)
                {
                    if (hitBox.Intersects(hurtBox))
                    {
                        //Debug.Log("hit");
                        Properties.ComboActive = true;
                        Properties.Collided = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public void DetectPush(Character opponent)
        {
            //Check for PushBox collision
            foreach (var localPushBox in Properties.CollisionManager.PushBoxes)
            {
                foreach (var opponentPushBox in opponent.Properties.CollisionManager.PushBoxes)
                {
                    if (localPushBox.Intersects(opponentPushBox))
                    {
                        Debug.Log(Properties.MoveDirection.x + ", " + opponent.Properties.MoveDirection.x);
                        opponent.Push(Properties.MoveDirection.x);
                    }
                }
            }
        }

        public void Push(float directionX)
        {
            Properties.MoveDirection.x = directionX;
        }

        public void ApplyCollision(FrameDataHandler handler)
        {
            Properties.CurrentHealth -= handler.Damage;
            Properties.CurrentState = CharacterProperties.CharacterState.HitStun;
            Properties.FrameDataHandler.HitStun = handler.HitStun;
            Properties.CollisionManager.PushBack = handler.PushBack;
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
            //TODO: make a list of different states, i.e grounded is usually fine, but a forward dash under a player should not instantly turn around
            return Properties.Grounded;
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
