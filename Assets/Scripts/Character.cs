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
    private const float WalkForwardXSpeed = 4.0f;
    private const float WalkBackwardXSpeed = 4.0f;
    private readonly float[] DashForwardXSpeed = {10.0f, 5.0f, 1.0f};
    private const float JumpYSpeed = 12.0f;
    private const float JumpForwardXSpeed = 5.0f;
    private const float JumpBackXSpeed = 5.0f;
    private const float PersonalGravity = 24.0f;

    private const int DashBrakeDuration = 20;
    
    //Define tracking variables.
    private int _inputFrameCounter;
    private int _actionFrameCounter;
    
    //Define Game related Values.
    private UnityEngine.CharacterController _characterController;
    //private CharacterState _characterState;
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
        CancellableAnimation,
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
        _actionFrameCounter = 0;

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
        return 0;
    }
    
    public void ApplyMovement(Vector3 moveDirection)
    {

        _moveDirection.y -= PersonalGravity * Time.deltaTime;
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    public void CharacterIdle(InputClass inputClass)
    {
        if (!_moveStandIdle.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand) return;
        _actionFrameCounter = 0;
        _currentState = CharacterState.Stand;
        _moveDirection = new Vector3(0, 0, 0);
    }
    
    public void WalkForward(InputClass inputClass)
    {
        if (!_moveWalkForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand) return;
        
        _currentState = CharacterState.Stand;
        _moveDirection = new Vector3(WalkForwardXSpeed, 0, 0);
    }
    
    public void WalkBackward(InputClass inputClass)
    {
        if (!_moveWalkBackward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand) return;
        

        _currentState = CharacterState.Stand;
        _moveDirection = new Vector3(-WalkBackwardXSpeed,0,0);
    }
    
    public void JumpForward(InputClass inputClass)
    {
        if (!_moveJumpForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;

        _currentState = CharacterState.Jump;
        _moveDirection = new Vector3(JumpForwardXSpeed,JumpYSpeed,0);
    }
    
    public void JumpBackward(InputClass inputClass)
    {
        if (!_moveJumpBackward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;

        _currentState = CharacterState.Jump;
        _moveDirection = new Vector3(-JumpBackXSpeed,JumpYSpeed,0);
    }
    
    public void JumpNeutral(InputClass inputClass)
    {
        if (!_moveJumpNeutral.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;

        _currentState = CharacterState.Jump;
        _moveDirection = new Vector3(0,JumpYSpeed,0);
    }

    public void DashForward(InputClass inputClass)
    {
        //Detect whether the Forward is held for a long dash
        if (_currentState == CharacterState.Dash && _moveDashForward.DetectHoldInput(inputClass))
            return;

        //Detect if the dash has ended
        if (_currentState == CharacterState.Dash && inputClass.DPadX == 0)
        {
            _currentState = CharacterState.CancellableAnimation;
            return;
        }
        
        //Brake animation when a Dash ends
        if (_currentState == CharacterState.CancellableAnimation && _actionFrameCounter < DashBrakeDuration)
        {
            _actionFrameCounter++;
            if (DashBrakeDuration - _actionFrameCounter > DashBrakeDuration / 2)
                _moveDirection.x = DashForwardXSpeed[1];
            else if (DashBrakeDuration - _actionFrameCounter > DashBrakeDuration / 4)
                _moveDirection.x = DashForwardXSpeed[2];
            else
                _moveDirection.x = 0;
        }
        
        //Exit dash animation
        if (_currentState == CharacterState.CancellableAnimation && _actionFrameCounter == DashBrakeDuration)
            _currentState = CharacterState.Stand;
        
        //Begin Dash Detection
        if (!_moveDashForward.DetectMoveInput(inputClass)) return;
        _currentState = CharacterState.Dash;
        _moveDirection = new Vector3(DashForwardXSpeed[0], 0, 0);
    }
    
    public void SpecialForward(InputClass inputClass)
    {
        if (!_moveSpecialForward.DetectMoveInput(inputClass)) return;
        
        _animator.Play("Punch", -1, 0f);
        Debug.Log("Hadoken");
    }
}
