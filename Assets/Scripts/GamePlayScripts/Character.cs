using System;
using System.Collections.Generic;
using System.Threading;
using GamePlayScripts.CharacterMoves;
using UnityEngine;
using UnityEngine.EventSystems;

/*******************************************************
 *
 ******************************************************/
namespace GamePlayScripts
{
    public class Character
    {
        //Define General objects and stats
        public CharacterManager LocalManager;
        private GameObject _characterObject;
        private GameObject _opponent;
        private Animator _animator;
        private InputManager _inputManager;
        
        private List<CharacterMove> _characterMoves;
        
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
            LocalManager = new CharacterManager
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
                MoveDirection = new Vector3(0,0,0),
                CurrentState = CharacterManager.CharacterState.Stand,
                LastState = CharacterManager.CharacterState.None,
                AttackState = CharacterManager.AttackStates.None,
                CancellableState = CharacterManager.CancellableStates.None
            };
            
            LocalManager.SetCollisionManager( new CollisionManager(_animator, ref LocalManager));
            characterGameObject.GetComponentInChildren<Camera>().targetTexture =
                (RenderTexture) Resources.Load("Textures/Player 1 Render Texture");
            
            //Define and populate collision boxes
            LocalManager.Add(new HurtBox(GameObject.Find("UpperBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.UpperBody));
            LocalManager.Add(new HurtBox(GameObject.Find("LowerBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.LowerBody));
            LocalManager.Add(new HitBox(GameObject.Find("HitBox").GetComponent<BoxCollider>(),true));
            //Add Pushbox
            
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
                move.InitializeMove(ref LocalManager, _animator);
            }
        }

        public void Update(Character opponent)
        {
            //Check if character was hit
            if (LocalManager.CurrentState == CharacterManager.CharacterState.HitStun)
            {
                LocalManager.UpdateCollision();
            }
            //Check if we need to switch orientation
            DeterminePlayerSide();
            //Get Inputs
            _inputManager.Update(LocalManager.CharacterOrientation);
            //Update Moves
            foreach (var move in _characterMoves)
            {
                move.PerformAction(_inputManager.CurrentInput);
            }

            //Move Character
            ApplyMovement(LocalManager.CharacterOrientation);
            //Reset NewHit
            LocalManager.NewHit = false;
        }
    
        private void ApplyMovement(int currentOrientation)
        {
            //Apply gravity
            if(!LocalManager.IgnoringGravity && LocalManager.Grounded == false)
                LocalManager.MoveDirection.y -= LocalManager.PersonalGravity * Time.deltaTime;
            
            var position = _characterObject.transform.position;
            LocalManager.MoveDirection.x *= currentOrientation;
            
            //Keep Object above the floor
            var potentialY = position.y + (LocalManager.MoveDirection.y * Time.deltaTime);
            if (potentialY  <= Constants.Floor)
            {
                LocalManager.MoveDirection.y = Constants.Floor - (position.y);
            }
            
            var potentialX = position.x + (LocalManager.MoveDirection.x * Time.deltaTime);


            //Keep characters within screen boundaries
            var center = (position.x + _opponent.transform.position.x) / 2;
            var leftScreenBarrier = center - Constants.MaxDistance;
            var rightScreenBarrier = center + Constants.MaxDistance;
            
            if (potentialX <= leftScreenBarrier)
            {
                LocalManager.MoveDirection.x = leftScreenBarrier - (position.x);
            }
            else if (potentialX >= rightScreenBarrier)
            {
                LocalManager.MoveDirection.x = rightScreenBarrier - (position.x);
            }
            
            //Keep character within stage boundaries
            var stageLeftBarrier = -Constants.MaxStageX;
            var stageRightBarrier = Constants.MaxStageX;
            
            if (potentialX <= stageLeftBarrier)
            {
                LocalManager.MoveDirection.x = stageLeftBarrier - (position.x);
            }
            else if (potentialX >= stageRightBarrier)
            {
                LocalManager.MoveDirection.x = stageRightBarrier - (position.x);
            }
            
            //Actually move the character according to all the previous calculations
            _characterObject.transform.position += (LocalManager.MoveDirection * Time.deltaTime);
            
            //Determine is character is grounded
            LocalManager.Grounded =_characterObject.transform.position.y < Constants.FloorBuffer;
        }

        public bool DetectCollisions(CharacterManager opponentManager)
        {
            //Detect HitBox Collisions
            var hurtBoxes = opponentManager.GetHurtBoxes();
            foreach (var hitBox in LocalManager.GetHitBoxes())
            {
                //If a local HitBox is not active than no collision
                if (!LocalManager.LocalHitBoxActive && hitBox.LocalHitBox) return false;

                //Move already collided
                if (LocalManager.Collided) return false;

                foreach (var hurtBox in hurtBoxes)
                {
                    if (hitBox.Intersects(hurtBox))
                    {
                        Debug.Log("hit");
                        LocalManager.ComboActive = true;
                        LocalManager.Collided = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public void Push(float directionX)
        {
            LocalManager.MoveDirection.x = directionX;
        }

        public void ApplyCollision(FrameDataManager manager)
        {
            LocalManager.CurrentHealth -= manager.Damage;
            LocalManager.CurrentState = CharacterManager.CharacterState.HitStun;
            LocalManager.SetHitStun(manager.HitStun);
            LocalManager.SetPushBack(manager.PushBack);
        }
        
        //Determine which side the player is on
        public void DeterminePlayerSide()
        {
            if (_characterObject.transform.position.x > _opponent.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(-1,1,1);
                LocalManager.CharacterOrientation = -1;
                _characterObject.transform.localScale = Vector3.Lerp(_characterObject.transform.localScale,flipModel, 2.0f);
            }
            else if (_characterObject.transform.position.x < _opponent.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(1,1,1);
                LocalManager.CharacterOrientation = 1;
                _characterObject.transform.localScale = Vector3.Lerp(_characterObject.transform.localScale,flipModel, 2.0f);
            }
        }

        private bool CanSwitchOrientation()
        {
            //TODO: make a list of different states, i.e grounded is usually fine, but a forward dash under a player should not instantly turn around
            return LocalManager.Grounded;
        }

        public CharacterManager GetCharacterProperties()
        {
            return LocalManager;
        }

        public void PostLoadSetup(GameObject opponent)
        {
            _opponent = opponent;
            DeterminePlayerSide();
        }
    }
}
