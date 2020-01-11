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
    //Speed arrays follow the frames for Startup/Active/Recovery from the move class
    private readonly float[] DashForwardXSpeed = {7.0f, 10.0f, 3.0f};
    private readonly float[] AirDashForwardSpeed = {10.0f, 7.0f};
    private readonly float[] DashBackwardXSpeed = {10.0f, 4.0f, 1.0f};
    private const float AirDashBackwardSpeed = 8.0f;
    private const float JumpYSpeed = 12.0f;
    private const float JumpForwardXSpeed = 5.0f;
    private const float JumpBackXSpeed = 5.0f;
    private const float PersonalGravity = 24.0f;

    private bool _isIgnoringGravity = false;
    private bool _isAirborne = false;

    private const int DashBrakeDuration = 20;
    private const int BackDashDuration = 15;
    private const int AirDashDuration = 18;
    
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
        BackDash,
        CancellableAnimation,
        AirDash,
        AirBackDash,
        DoubleJump,
        BlockStun,
        HitStun,
        Cinematic
    }     
    
    //Define Character Moves
    private ICharacterMove _moveStandIdle;
    private ICharacterMove _moveWalkForward;
    private ICharacterMove _moveWalkBackward;
    private ICharacterMove _moveJumpNeutral;
    private ICharacterMove _moveJumpForward;
    private ICharacterMove _moveJumpBackward;
    private MoveDashForward _moveDashForward;
    private ICharacterMove _moveDashBackward;
    private ICharacterMove _moveSpecialForward;
    private ICharacterMove _moveAirDashForward;

    private Animation _animation;
    private Animator _animator;


    public Character(UnityEngine.CharacterController characterController)
    {
        _characterController = characterController;
        _moveDirection = new Vector3(0,0,0);
        HealthPoints = 100;
        MeterPoints = 0;
        _actionFrameCounter = 0;
        _currentState = CharacterState.Stand;

        _moveStandIdle = new MoveStandIdle();
        _moveWalkForward = new MoveWalkForward();
        _moveWalkBackward = new MoveWalkBackward();
        _moveJumpNeutral = new MoveJumpNeutral();
        _moveJumpForward = new MoveJumpForward();
        _moveJumpBackward = new MoveJumpBackward();
        _moveDashForward = new MoveDashForward();
        _moveDashBackward = new MoveDashBackward();
        _moveSpecialForward = new MoveSpecialForward();
        _moveAirDashForward = new MoveAirDashForward();

        _animation = _characterController.GetComponent<Animation>();
        _animator = _characterController.GetComponent<Animator>();
    }

    public int Update(InputClass inputClass)
    {
        return 0;
    }

    
    public void ApplyMovement(Vector3 moveDirection)
    {
        Debug.Log(_moveDirection.x + "," + _actionFrameCounter + "," + _currentState + "," + _moveDashForward.AttackStateFrames[_actionFrameCounter]);
        if(!_isIgnoringGravity)
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
        if (_currentState == CharacterState.Jump && _characterController.isGrounded)
            _currentState = CharacterState.Stand;
        
        if (!_moveJumpForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;

        _currentState = CharacterState.Jump;
        _moveDirection = new Vector3(JumpForwardXSpeed,JumpYSpeed,0);
    }
    
    public void JumpBackward(InputClass inputClass)
    {
        if (_currentState == CharacterState.Jump && _characterController.isGrounded)
            _currentState = CharacterState.Stand;
        
        if (!_moveJumpBackward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;

        _currentState = CharacterState.Jump;
        _moveDirection = new Vector3(-JumpBackXSpeed,JumpYSpeed,0);
    }
    
    public void JumpNeutral(InputClass inputClass)
    {
        if (_currentState == CharacterState.Jump && _characterController.isGrounded)
            _currentState = CharacterState.Stand;
        
        if (!_moveJumpNeutral.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;

        _currentState = CharacterState.Jump;
        _moveDirection = new Vector3(0,JumpYSpeed,0);
    }

    public void DashForward(InputClass inputClass)
    {

        //Detect whether the Forward is held for a long dash
        if (_currentState == CharacterState.Dash && _moveDashForward.DetectHoldInput(inputClass) && _moveDashForward.AttackStateFrames[_actionFrameCounter] == 1)
            return;

        //Play out dash animation
        if (_currentState == CharacterState.Dash && _moveDashForward.AttackStateFrames[_actionFrameCounter] < 3)
        {
            _actionFrameCounter++;
            _moveDirection = new Vector3(DashForwardXSpeed[_moveDashForward.AttackStateFrames[_actionFrameCounter]],0,0);
            if (_moveDashForward.AttackStateFrames[_actionFrameCounter] >= 2)
                _currentState = CharacterState.CancellableAnimation;
            
        }

        if (_currentState == CharacterState.CancellableAnimation &&_moveDashForward.AttackStateFrames[_actionFrameCounter] < 3)
        {
            _actionFrameCounter++;
        }
        
        //Exit dash animation
        if (_currentState == CharacterState.CancellableAnimation && _moveDashForward.AttackStateFrames[_actionFrameCounter] >= 3)
        {
            _currentState = CharacterState.Stand;
            _actionFrameCounter = 0;
        }
        
        //Begin Dash Detection
        if (!_moveDashForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Stand && _currentState != CharacterState.Crouch) return;
        _currentState = CharacterState.Dash;
        _moveDirection = new Vector3(DashForwardXSpeed[0], 0, 0);
    }
    
    public void DashBackward(InputClass inputClass)
    {
        //Play out BackDash animation
        if (_currentState == CharacterState.BackDash && _actionFrameCounter < BackDashDuration)
        {
            _actionFrameCounter++;
            if (_actionFrameCounter > BackDashDuration * .80)
                _moveDirection.x = -DashBackwardXSpeed[1];
            else if (_actionFrameCounter > BackDashDuration * .95)
                _moveDirection.x = -DashBackwardXSpeed[2];
        }
        
        //Exit dash animation
        if (_currentState == CharacterState.BackDash && _actionFrameCounter >= BackDashDuration)
            _currentState = CharacterState.Stand;
        
        //Begin Dash Detection
        if (!_moveDashBackward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Stand) return;
        _actionFrameCounter = 0;
        _currentState = CharacterState.BackDash;
        _moveDirection = new Vector3(-DashBackwardXSpeed[0], 0, 0);
    }

    public void AirDashForward(InputClass inputClass)
    {
        //Play out AirDash Duration
        if (_currentState == CharacterState.AirDash && _actionFrameCounter < AirDashDuration)
        {
            _actionFrameCounter++;
            if (_actionFrameCounter > AirDashDuration * 0.75)
                _moveDirection.x = AirDashForwardSpeed[1];
        }
        
        //Exit dash animation
        if (_currentState == CharacterState.AirDash && _actionFrameCounter >= AirDashDuration)
        {
            _currentState = CharacterState.Jump;
            _actionFrameCounter = 0;
            _isIgnoringGravity = false;
        }
            
        
        //Begin AirDash Detection
        if (!_moveAirDashForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Jump) return;
        _actionFrameCounter = 0;
        _currentState = CharacterState.AirDash;
        _isIgnoringGravity = true;
        _moveDirection = new Vector3(AirDashForwardSpeed[0],0,0);
    }
    
    public void SpecialForward(InputClass inputClass)
    {
        if (!_moveSpecialForward.DetectMoveInput(inputClass)) return;
        
        _animator.Play("Punch", -1, 0f);
        Debug.Log("Hadoken");
    }
    public void SpecialBackward(InputClass inputClass)
    {
        if (!_moveSpecialForward.DetectMoveInput(inputClass)) return;
        
        _animator.Play("Punch", -1, 0f);
        Debug.Log("Hadoken");
    }
}
