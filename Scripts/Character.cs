using System.Collections;
using System.Collections.Generic;
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
    //Define Character Stats
    public int HealthPoints {get; set;}
    public int MeterPoints { get; set; }
    public float WalkForwardXSpeed { get; }
    public float WalkBackwardXSpeed{ get; }
    public float DashForwardXSpeed{ get; }
    public float JumpYSpeed{ get; }
    public float JumpForwardXSpeed{ get; }
    public float JumpBackXSpeed{ get; }
    public float PersonalGravity{ get; }
    private UnityEngine.CharacterController _characterController;
    private CharacterState _characterState;
    private Vector3 _moveDirection;
    private float _minX;
    private float _maxX;
    
    //Define Character State
    private CharacterState _currentState;
    public enum CharacterState
    {
        Stand,
        Crouch,
        Jump,
        Dash,
        AirDash,
        BlockStun,
        HitStun,
        Cinematic
    }     
    private enum CharacterInvulnerability
    {
        Normal,
        UpperBody,
        LowerBody,
        Full
    }
    
    //Define Character Moves
    private ICharacterMove _moveStandIdle;
    private ICharacterMove _moveWalkForward;
    private ICharacterMove _moveWalkBackward;
    private ICharacterMove _moveJumpNeutral;
    private ICharacterMove _moveJumpForward;
    private ICharacterMove _moveJumpBackward;
    private ICharacterMove _moveDashForward;
    private ICharacterMove _moveSpecialForward;

    private Animation _animation;
    private Animator _animator;


    public Character(UnityEngine.CharacterController characterController)
    {
        _characterController = characterController;
        _moveDirection = new Vector3(0,0,0);
        HealthPoints = 100;
        MeterPoints = 0;
        WalkForwardXSpeed = 6.0f;
        WalkBackwardXSpeed = 6.0f;
        DashForwardXSpeed = 25.0f;
        JumpYSpeed = 80.0f;
        JumpForwardXSpeed = 9.0f;
        JumpBackXSpeed = 9.0f;
        PersonalGravity = 150.0f;

        _moveStandIdle = new MoveStandIdle();
        _moveWalkForward = new MoveWalkForward();
        _moveWalkBackward = new MoveWalkBackward();
        _moveJumpNeutral = new MoveJumpNeutral();
        _moveJumpForward = new MoveJumpForward();
        _moveJumpBackward = new MoveJumpBackward();
        _moveDashForward = new MoveDashForward();
        _moveSpecialForward = new MoveSpecialForward();

        _animation = _characterController.GetComponent<Animation>();
        _animator = _characterController.GetComponent<Animator>();
    }

    public int Update(InputClass inputClass)
    {
        
        _moveDirection = ReadInputQueue(inputClass);

        _moveDirection.y -= PersonalGravity * Time.deltaTime;
        _characterController.Move(_moveDirection * Time.deltaTime);
        return 0;
    }

    public Vector3 ReadInputQueue(InputClass inputClass)
    {
        
        //read single input commands ie. Walk forward/back/neutral-jump/crouch
        if (_moveStandIdle.DetectMoveInput(inputClass))
            if (_characterController.isGrounded &&
                (_currentState == CharacterState.Crouch || _currentState == CharacterState.Stand))
                _moveDirection = _moveStandIdle.PerformAction(ref _characterState);
        if (_moveWalkForward.DetectMoveInput(inputClass))
            if (_characterController.isGrounded &&
            (_currentState == CharacterState.Crouch || _currentState == CharacterState.Stand))
                _moveDirection = WalkForward();
        if (_moveWalkBackward.DetectMoveInput(inputClass))
            if (_characterController.isGrounded)
                _moveDirection = WalkBackward();
        if (_moveJumpForward.DetectMoveInput(inputClass))
            if (_characterController.isGrounded)
                _moveDirection = JumpForward();
        if (_moveJumpBackward.DetectMoveInput(inputClass))
            if (_characterController.isGrounded)
                _moveDirection = JumpBackward();
        if (_moveJumpNeutral.DetectMoveInput(inputClass))
            if (_characterController.isGrounded)
                _moveDirection = JumpNeutral();
        
        //read multi-input commands
        if (_moveDashForward.DetectMoveInput(inputClass))
        {
            return DashForward();
        }
        else
        {
            _currentState = CharacterState.Stand;
        }
        if (_moveSpecialForward.DetectMoveInput(inputClass))
        {
            _animator.Play("Punch", -1, 0f);
            Debug.Log("Hadoken");
        }

        return _moveDirection;
    }

    private Vector3 CharacterIdle()
    {
        _currentState = CharacterState.Stand;
        _moveDirection = new Vector3(0, 0, 0);

        return _moveDirection;
    }
    
    private Vector3 WalkForward()
    {

        _currentState = CharacterState.Stand;
        var xVelocity = WalkForwardXSpeed;
        var moveDirection = new Vector3(xVelocity, 0, 0);
            
        return moveDirection;
    }
    
    private Vector3 WalkBackward()
    {
        _currentState = CharacterState.Stand;
        var xVelocity = -WalkBackwardXSpeed;
        var moveDirection = new Vector3(xVelocity,0,0);
        
        return moveDirection;
    }

    private Vector3 JumpNeutral()
    {
        _currentState = CharacterState.Jump;
        var yVelocity = JumpYSpeed;
        var moveDirection = new Vector3(0,yVelocity,0);
        return moveDirection;
    }

    private Vector3 JumpForward()
    {
        _currentState = CharacterState.Jump;
        var xVelocity = JumpForwardXSpeed;
        var yVelocity = JumpYSpeed;
        var moveDirection = new Vector3(xVelocity,yVelocity,0);
        return moveDirection;
    }
    
    private Vector3 JumpBackward()
    {
        _currentState = CharacterState.Jump;
        var xVelocity = -JumpBackXSpeed;
        var yVelocity = JumpYSpeed;
        var moveDirection = new Vector3(xVelocity,yVelocity,0);
        return moveDirection;
    }

    private Vector3 DashForward()
    {
        _currentState = CharacterState.Dash;
        var xVelocity = DashForwardXSpeed;
        var moveDirection = new Vector3(xVelocity,0,0);
        return moveDirection;
    }
}
