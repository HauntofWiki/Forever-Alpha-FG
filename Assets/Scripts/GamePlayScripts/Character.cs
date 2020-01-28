using System.Collections.Generic;
using GamePlayScripts.CharacterMoves;
using UnityEngine;

/*
 * This class defines characteristics unique to the character.
 *
 * This is a generic character representation
 */
namespace GamePlayScripts
{
    public class Character
    {
        //Define Character Stats.
        public int HealthPoints {get; set;}
        public int MeterPoints { get; set; }

        public CharacterProperties Properties;
        private List<CharacterMove> _characterMoves;
    
        //Define Game related Values.
        private UnityEngine.CharacterController _characterController;
    
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

        private Animator _animator;


        public Character(UnityEngine.CharacterController characterController)
        {
            _characterController = characterController;

            HealthPoints = 100;
            MeterPoints = 0;

            _characterMoves = new List<CharacterMove>();
        
            Properties = new CharacterProperties
            {
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
                CharacterController = _characterController
            };
        
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
        
            _animator = _characterController.GetComponent<Animator>();
            
            //Initialize moves.
            foreach (var move in _characterMoves)
            {
                move.InitializeMove(ref Properties, _animator);
            }
        }

        public void Update(InputClass inputClass)
        {
            foreach (var move in _characterMoves)
            {
                move.PerformAction(inputClass);
            }
            
            ApplyMovement(Properties.CharacterOrientation);
            
        }

        public bool CanSwitchOrientation()
        {
            return (Properties.CurrentState != Properties.LastState && Properties.CurrentState != CharacterProperties.CharacterState.JumpForward );//May want to add statuses or handle more elegantly
        }
    
        public void ApplyMovement(int currentOrientation)
        {

            if(!Properties.IsIgnoringGravity)
                Properties.MoveDirection.y -= Properties.PersonalGravity * Time.deltaTime;


            Properties.MoveDirection.x *= currentOrientation;
            _characterController.Move(Properties.MoveDirection * Time.deltaTime);
        }
    }
}
