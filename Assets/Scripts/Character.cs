using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CharacterMoves;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Experimental.PlayerLoop;

/*
 * This class defines characteristics unique to the character.
 *
 * This is a generic character representation
 */
public class Character
{
    //Define Character Stats.
    public int HealthPoints {get; set;}
    public int MeterPoints { get; set; }

    private CharacterProperties _properties;
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

    private Animation _animation;
    private Animator _animator;


    public Character(UnityEngine.CharacterController characterController)
    {
        _characterController = characterController;

        HealthPoints = 100;
        MeterPoints = 0;

        _properties = new CharacterProperties
        {
            WalkForwardXSpeed = 4.0f,
            WalkBackwardXSpeed = 4.0f,
            JumpYSpeed = 12.0f,
            PersonalGravity = 24.0f,
            DashForwardXSpeed = new float[] {7.0f, 10.0f, 3.0f},
            AirDashForwardSpeed = new float[] {10.0f, 10.0f},
            DashBackwardXSpeed = new float[] {7.0f, 20.0f, 3.0f},
            IsAirborne = false,
            IsIgnoringGravity = false,
            JumpFrameCounter = 0,
            DashFrameCounter = 0,
            AttackFrameCounter = 0,
            MoveDirection = new Vector3(0,0,0),
            CurrentState = CharacterProperties.CharacterState.Stand,
            LastState = CharacterProperties.CharacterState.Empty,
            AttackState = CharacterProperties.AttackStates.None,
            CancellableState = CharacterProperties.CancellableStates.None,
            characterController = _characterController
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
        
        //Initialize moves.
        foreach (var move in _characterMoves)
        {
            move.InitializeMove(ref _properties);
        }
        

        //BackDashDuration = _moveDashBackward.AttackStateFrames.Length;

        _animation = _characterController.GetComponent<Animation>();
        _animator = _characterController.GetComponent<Animator>();
    }

    public void Update(InputClass inputClass)
    {
        foreach (var move in _characterMoves)
        {
            move.PerformAction(inputClass);
        }
    }

    public bool CanSwitchOrientation()
    {
        return _properties.CurrentState != _properties.LastState;
    }
    
    public void ApplyMovement(Vector3 moveDirection, int currentOrientation, int lastOrientation)
    {
        //Debug.Log("CurrentState: " + _currentState + "; LastState: " + _lastState);

        if(!_properties.IsIgnoringGravity)
            _properties.MoveDirection.y -= _properties.PersonalGravity * Time.deltaTime;


       _properties.MoveDirection.x *= currentOrientation;
        //Debug.Log(_properties.MoveDirection.x);
        _characterController.Move(_properties.MoveDirection * Time.deltaTime);
    }
}
