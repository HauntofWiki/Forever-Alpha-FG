﻿using System;
using System.Collections.Generic;
using System.Threading;
using GamePlayScripts.CharacterMoves;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;

/*******************************************************
 * Parent class of the each character's more specific class
 ******************************************************/
namespace GamePlayScripts
{
    public class Character
    {
        //Define General objects and stats
        public CharacterManager CharManager;
        private GameObject _characterObject;
        private GameObject _opponent;
        private Animator _animator;
        private InputManager _inputManager;
        private Character _opponentCharacter;
        
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
            CharManager = new CharacterManager
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
                Height = 1.5f,
                Push = 0.0f,
                IsAirborne = false,
                IgnoringGravity = false,
                MoveDirection = new Vector3(0,0,0),
                CurrentState = CharacterManager.CharacterState.Stand,
                LastState = CharacterManager.CharacterState.None,
                AttackState = CharacterManager.AttackStates.None,
                CancellableState = CharacterManager.CancellableStates.None
            };
            
            CharManager.SetCollisionManager( new CollisionManager(_animator, ref CharManager));
            characterGameObject.GetComponentInChildren<Camera>().targetTexture =
                (RenderTexture) Resources.Load("Textures/Player 1 Render Texture");
            
            //Define and populate collision boxes
            CharManager.Add(new HurtBox(GameObject.Find("UpperBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.UpperBody));
            CharManager.Add(new HurtBox(GameObject.Find("LowerBodyHurtBox").GetComponent<BoxCollider>(), HurtBox.HurtZone.LowerBody));
            CharManager.Add(new HitBox(GameObject.Find("HitBox").GetComponent<BoxCollider>(),true));
            CharManager.SetPushBox(new PushBox(GameObject.Find("PushBox").GetComponent<BoxCollider>()));
            
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
                move.InitializeMove(ref CharManager, _animator);
            }
        }

        public void Update(Character opponent)
        {
            //Check if character was hit
            if (CharManager.CurrentState == CharacterManager.CharacterState.HitStun)
            {
                CharManager.UpdateCollision();
            }
            //Check if we need to switch orientation
            DeterminePlayerSide();
            //Get Inputs
            _inputManager.Update(CharManager.CharacterOrientation);
            //Update Moves
            foreach (var move in _characterMoves)
            {
                move.PerformAction(_inputManager.CurrentInput);
            }

            //Move Character
            ApplyMovement(CharManager.CharacterOrientation);
            //Reset NewHit
            CharManager.NewHit = false;
        }
    
        private void ApplyMovement(int currentOrientation)
        {
            //Apply gravity
            if(!CharManager.IgnoringGravity && CharManager.Grounded == false)
                CharManager.MoveDirection.y -= CharManager.PersonalGravity * Time.deltaTime;
            
            //Apply directions based on which side the character is on
            CharManager.MoveDirection.x *= currentOrientation;
            var position = _characterObject.transform.position;
            
            //Keep Object above the floor
            var potentialY = position.y + (CharManager.MoveDirection.y * Time.deltaTime);
            if (potentialY  <= Constants.Floor)
            {
                CharManager.MoveDirection.y = Constants.Floor - (position.y);
            }
            
            var potentialX = position.x + (CharManager.MoveDirection.x * Time.deltaTime);
            int trueOrientation = 0;
            if (position.x < _opponent.transform.position.x)
                trueOrientation = 1;
            if (position.x > _opponent.transform.position.x)
                trueOrientation = -1;

          
            //Use Push Box to maintain minimum distance
            if (CharManager.GetPushBox().Collider.bounds
                .Intersects(_opponentCharacter.CharManager.GetPushBox().Collider.bounds))
            {
                if (CharManager.MoveDirection.x * trueOrientation > 0)
                {
                    _opponentCharacter.CharManager.Push = CharManager.MoveDirection.x;
                }

                if (CharManager.MoveDirection.x * trueOrientation == 0 && CharManager.Push * trueOrientation == 0)
                {
                    CharManager.MoveDirection.x = -1f * trueOrientation;
                }
            }
            else
            {
                _opponentCharacter.CharManager.Push = 0;
            }
            
            //Keep characters from move through one another
            if (potentialX >= _opponent.transform.position.x && potentialY <= _opponentCharacter.CharManager.Height && trueOrientation == 1)
            {
                //_opponentCharacter.LocalManager.Push = LocalManager.MoveDirection.x;
                CharManager.MoveDirection.x = _opponent.transform.position.x - (position.x);
            }
            else if (potentialX <= _opponent.transform.position.x && potentialY <= _opponentCharacter.CharManager.Height && trueOrientation == -1)
            {
                //_opponentCharacter.LocalManager.Push = LocalManager.MoveDirection.x;
                CharManager.MoveDirection.x = _opponent.transform.position.x - (position.x);
            }
            
            //Apply any push forces
            CharManager.MoveDirection.x += CharManager.Push;

            //Keep characters within screen boundaries
            var center = (position.x + _opponent.transform.position.x) / 2;
            var leftScreenBarrier = center - Constants.MaxDistance;
            var rightScreenBarrier = center + Constants.MaxDistance;
            
            if (potentialX <= leftScreenBarrier)
            {
                CharManager.MoveDirection.x = leftScreenBarrier - (position.x);
            }
            else if (potentialX >= rightScreenBarrier)
            {
                CharManager.MoveDirection.x = rightScreenBarrier - (position.x);
            }
            
            //Keep character within stage boundaries
            var stageLeftBarrier = -Constants.MaxStageX;
            var stageRightBarrier = Constants.MaxStageX;
            
            if (potentialX <= stageLeftBarrier)
            {
                CharManager.MoveDirection.x = stageLeftBarrier - (position.x);
            }
            else if (potentialX >= stageRightBarrier)
            {
                CharManager.MoveDirection.x = stageRightBarrier - (position.x);
            }
            
            //Check if character is against the corner
            if (position.x <= stageLeftBarrier || position.x >= stageRightBarrier)
                CharManager.Cornered = true;
            else
                CharManager.Cornered = false;

            //Actually move the character according to all the previous calculations
            _characterObject.transform.position += (CharManager.MoveDirection * Time.deltaTime);
            
            //Determine is character is grounded
            CharManager.Grounded =_characterObject.transform.position.y < Constants.FloorBuffer;
        }

        public bool DetectCollisions(CharacterManager opponentManager)
        {
            //Detect HitBox Collisions
            var hurtBoxes = opponentManager.GetHurtBoxes();
            foreach (var hitBox in CharManager.GetHitBoxes())
            {
                //If a local HitBox is not active than no collision
                if (!CharManager.LocalHitBoxActive && hitBox.LocalHitBox) return false;

                //Move already collided
                if (CharManager.Collided) return false;

                foreach (var hurtBox in hurtBoxes)
                {
                    if (hitBox.Intersects(hurtBox))
                    {
                        Debug.Log("hit");
                        CharManager.ComboActive = true;
                        CharManager.Collided = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public void ApplyCollision(FrameDataManager manager)
        {
            CharManager.CurrentHealth -= manager.Damage;
            CharManager.CurrentState = CharacterManager.CharacterState.HitStun;
            CharManager.SetHitStun(manager.HitStun);
            CharManager.SetPushBack(manager.PushBack);
        }
        
        //Determine which side the player is on
        public void DeterminePlayerSide()
        {
            if (_characterObject.transform.position.x > _opponent.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(-1,1,1);
                CharManager.CharacterOrientation = -1;
                _characterObject.transform.localScale = Vector3.Lerp(_characterObject.transform.localScale,flipModel, 2.0f);
            }
            else if (_characterObject.transform.position.x < _opponent.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(1,1,1);
                CharManager.CharacterOrientation = 1;
                _characterObject.transform.localScale = Vector3.Lerp(_characterObject.transform.localScale,flipModel, 2.0f);
            }
        }

        private bool CanSwitchOrientation()
        {
            //TODO: make a list of different states, i.e grounded is usually fine, but a forward dash under a player should not instantly turn around
            return CharManager.Grounded;
        }

        public CharacterManager GetCharacterProperties()
        {
            return CharManager;
        }

        public void PostLoadSetup(GameObject opponent, Character opponentCharacter)
        {
            _opponentCharacter = opponentCharacter;
            _opponent = opponent;
            DeterminePlayerSide();
        }
    }
}
