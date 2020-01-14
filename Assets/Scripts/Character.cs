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
    private const float WalkForwardXSpeed = 4.0f;
    private const float WalkBackwardXSpeed = 4.0f;
    //Speed arrays follow the frames for Startup/Active/Recovery from the move class
    private readonly float[] DashForwardXSpeed = {7.0f, 10.0f, 3.0f};
    private readonly float[] AirDashForwardSpeed = {10.0f, 10.0f};
    private readonly float[] DashBackwardXSpeed = {7.0f, 20.0f, 3.0f};
    private const float AirDashBackwardSpeed = 8.0f;
    private const float JumpYSpeed = 12.0f;
    private const float JumpForwardXSpeed = 5.0f;
    private const float JumpBackXSpeed = 5.0f;
    private const float PersonalGravity = 24.0f;

    private bool _isIgnoringGravity = false;
    private bool _isAirborne = false;

    private const int DashBrakeDuration = 20;
    private readonly int BackDashDuration;
    private const int AirDashDuration = 18;
    
    //Define tracking variables.
    private int _inputFrameCounter;
    private int _jumpFrameCounter = 0;
    private int _dashFrameCounter = 0;
    private int _attackFrameCounter = 0;
    
    //Define Game related Values.
    private UnityEngine.CharacterController _characterController;
    //private CharacterState _characterState;
    private Vector3 _moveDirection;
    private float _minX;
    private float _maxX;
    
    //Define Character State
    private CharacterState _currentState;
    private CharacterState _lastState;
    public enum CharacterState
    {
        Stand,
        Crouch,
        Jump,
        JumpForward,
        JumpBackward,
        Landing,
        LandingJumpForward,
        LandingJumpBackward,
        Dash,
        BackDash,
        CancellableAnimation,
        AirDash,
        AirBackDash,
        DoubleJump,
        BlockStun,
        HitStun,
        Cinematic,
        Empty
    }

    public enum AttackState
    {
        None,
        LightAttack,
        CrouchLightAttack,
        JumpLightAttack,
        MediumAttack,
        CrouchMediumAttack,
        JumpMediumAttack,
        HeavyAttack,
        CrouchHeavyAttack,
        JumpHeavyAttack,
        SpecialAttack,
        CrouchSpecialAttack,
        JumpSpecialAttack,
    }

    private AttackState _attackState;

    private enum CancellableState
    {
        None,
        EmptyCancellable,
        NormalCancellable,
        SpecialCancellable,
        SuperCancellable,
        JumpCancellable,
        AirJumpCancellable,
        DashCancellable,
        AirDashCancellable
    }

    private CancellableState _cancellableState;
    
    //Define Character Moves
    private ICharacterMove _moveStandIdle;
    private ICharacterMove _moveWalkForward;
    private ICharacterMove _moveWalkBackward;
    private MoveJumpNeutral _moveJumpNeutral;
    private MoveJumpForward _moveJumpForward;
    private MoveJumpBackward _moveJumpBackward;
    private MoveDashForward _moveDashForward;
    private MoveDashBackward _moveDashBackward;
    private ICharacterMove _moveSpecialForward;
    private MoveAirDashForward _moveAirDashForward;
    private MoveLightAttack _moveLightAttack;

    private Animation _animation;
    private Animator _animator;


    public Character(UnityEngine.CharacterController characterController)
    {
        _characterController = characterController;
        _moveDirection = new Vector3(0,0,0);
        HealthPoints = 100;
        MeterPoints = 0;
        _currentState = CharacterState.Stand;
        _lastState = CharacterState.Empty;
        _attackState = AttackState.None;
        _cancellableState = CancellableState.None;
        
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
        _moveLightAttack = new MoveLightAttack();
        

        BackDashDuration = _moveDashBackward.AttackStateFrames.Length;

        _animation = _characterController.GetComponent<Animation>();
        _animator = _characterController.GetComponent<Animator>();
    }

    public bool CanSwitchOrientation()
    {
        return _currentState != _lastState;
    }
    
    public void ApplyMovement(Vector3 moveDirection, int currentOrientation, int lastOrientation)
    {
        Debug.Log("CurrentState: " + _currentState + "; LastState: " + _lastState);

        if(!_isIgnoringGravity)
            _moveDirection.y -= PersonalGravity * Time.deltaTime;


        _moveDirection.x *= currentOrientation;
        Debug.Log(_moveDirection.x);
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    public void CharacterIdle(InputClass inputClass)
    {
        if (!_moveStandIdle.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand) return;
        _dashFrameCounter = 0;

        _lastState = _currentState;
        _currentState = CharacterState.Stand;
        _moveDirection = new Vector3(0, 0, 0);
    }
    
    public void WalkForward(InputClass inputClass)
    {
        if (!_moveWalkForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand) return;
        
        _lastState = _currentState;
        _currentState = CharacterState.Stand;
        _moveDirection = new Vector3(WalkForwardXSpeed, 0, 0);
    }
    
    public void WalkBackward(InputClass inputClass)
    {
        if (!_moveWalkBackward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand) return;
        
        _lastState = _currentState;
        _currentState = CharacterState.Stand;
        _moveDirection = new Vector3(-WalkBackwardXSpeed,0,0);
    }
    
    public void JumpForward(InputClass inputClass)
    {
        //Use Character controller to determine if animation have reached the ground
        if (_currentState == CharacterState.JumpForward && _characterController.isGrounded && _moveJumpForward.AttackStateFrames[_jumpFrameCounter] != 0)
        {
            _jumpFrameCounter++;
            _lastState = _currentState;
            _currentState = CharacterState.LandingJumpForward; 
        }
            
        //Advance Jump Frame Counter and assess Startup/Recovery
        if (_currentState == CharacterState.JumpForward && _moveJumpForward.AttackStateFrames[_jumpFrameCounter] == 0)
        {
            _jumpFrameCounter++;
            if (_moveJumpForward.AttackStateFrames[_jumpFrameCounter] == 0)
            {
                _lastState = _currentState;
                return;
            }

            if (_moveJumpForward.AttackStateFrames[_jumpFrameCounter] == 1)
            {
                _lastState = _currentState;
                _moveDirection = new Vector3(JumpForwardXSpeed, JumpYSpeed, 0);
            }
        }
        
        if (_currentState == CharacterState.LandingJumpForward)
        {
            _jumpFrameCounter++;
            if (_moveJumpForward.AttackStateFrames[_jumpFrameCounter] == 3)
            {
                _jumpFrameCounter = 0;
                _lastState = _currentState;
                _currentState = CharacterState.Stand;
            }
        }


        //Detect proper state and detect input
        if (_currentState == CharacterState.JumpForward && _moveJumpForward.AttackStateFrames[_jumpFrameCounter] == 1)
        {
            _moveDirection.x = JumpForwardXSpeed;
            return;
        }

        if (!_moveJumpForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;
        
        _jumpFrameCounter = 0;
        _lastState = _currentState;
        _currentState = CharacterState.JumpForward;
    }
    
    public void JumpBackward(InputClass inputClass)
    {
        //Use Character controller to determine if animation have reached the ground
        if (_currentState == CharacterState.JumpBackward && _characterController.isGrounded && _moveJumpBackward.AttackStateFrames[_jumpFrameCounter] != 0)
        {
            _jumpFrameCounter++;
            _lastState = _currentState;
            _currentState = CharacterState.LandingJumpBackward; 
        }
            
        //Advance Jump Frame Counter and assess Startup/Recovery
        if (_currentState == CharacterState.JumpBackward && _moveJumpBackward.AttackStateFrames[_jumpFrameCounter] == 0)
        {
            _jumpFrameCounter++;
            if (_moveJumpBackward.AttackStateFrames[_jumpFrameCounter] == 0)
                return;
            if (_moveJumpBackward.AttackStateFrames[_jumpFrameCounter] == 1)
                _moveDirection = new Vector3(-JumpBackXSpeed,JumpYSpeed,0);
        }
        
        if (_currentState == CharacterState.LandingJumpBackward)
        {
            _jumpFrameCounter++;
            if (_moveJumpBackward.AttackStateFrames[_jumpFrameCounter] == 3)
            {
                _jumpFrameCounter = 0;
                _lastState = _currentState;
                _currentState = CharacterState.Stand;
            }
        }


        //Detect proper state and detect input
        if (_currentState == CharacterState.JumpBackward && _moveJumpBackward.AttackStateFrames[_jumpFrameCounter] == 1)
        {
            _moveDirection.x = -JumpBackXSpeed;
            return;
        }

        if (!_moveJumpBackward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;
        
        _jumpFrameCounter = 0;
        _lastState = _currentState;
        _currentState = CharacterState.JumpBackward;
    }
    
    public void JumpNeutral(InputClass inputClass)
    {
        //Use Character controller to determine if animation have reached the ground
        if (_currentState == CharacterState.Jump && _characterController.isGrounded && _moveJumpNeutral.AttackStateFrames[_jumpFrameCounter] != 0)
        {
            _jumpFrameCounter++;
            _lastState = _currentState;
            _currentState = CharacterState.Landing; 
        }
            
        //Advance Jump Frame Counter and assess Startup/Recovery
        if (_currentState == CharacterState.Jump && _moveJumpNeutral.AttackStateFrames[_jumpFrameCounter] == 0)
        {
            _jumpFrameCounter++;
            if (_moveJumpNeutral.AttackStateFrames[_jumpFrameCounter] == 0)
                return;
            if (_moveJumpNeutral.AttackStateFrames[_jumpFrameCounter] == 1)
                _moveDirection = new Vector3(0,JumpYSpeed,0);
        }

        if (_currentState == CharacterState.Landing)
        {
            _jumpFrameCounter++;
            if (_moveJumpNeutral.AttackStateFrames[_jumpFrameCounter] == 3)
            {
                _jumpFrameCounter = 0;
                _lastState = _currentState;
                _currentState = CharacterState.Stand;
            }
        }

        if (_currentState == CharacterState.Jump && _moveJumpNeutral.AttackStateFrames[_jumpFrameCounter] == 1)
            return;
        
        //Detect proper state and detect input
        if (!_moveJumpNeutral.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Crouch && _currentState != CharacterState.Stand && _currentState != CharacterState.CancellableAnimation) return;

        _jumpFrameCounter = 0;
        _lastState = _currentState;
        _currentState = CharacterState.Jump;
    }

    public void DashForward(InputClass inputClass)
    {

        //Detect whether the Forward is held for a long dash
        if (_currentState == CharacterState.Dash && _moveDashForward.DetectHoldInput(inputClass) &&
            _moveDashForward.AttackStateFrames[_dashFrameCounter] == 1)
        {
            _moveDirection = new Vector3(DashForwardXSpeed[_moveDashForward.AttackStateFrames[_dashFrameCounter]],0,0);
            return;            
        }


        //Play out dash animation
        if (_currentState == CharacterState.Dash && _moveDashForward.AttackStateFrames[_dashFrameCounter] < 3)
        {
            _dashFrameCounter++;
            _moveDirection = new Vector3(DashForwardXSpeed[_moveDashForward.AttackStateFrames[_dashFrameCounter]],0,0);
            if (_moveDashForward.AttackStateFrames[_dashFrameCounter] >= 2)
            {
                _lastState = _currentState;
                _currentState = CharacterState.CancellableAnimation;
            }

        }

        if (_currentState == CharacterState.CancellableAnimation &&_moveDashForward.AttackStateFrames[_dashFrameCounter] < 3)
        {
            _dashFrameCounter++;
        }
        
        //Exit dash animation
        if (_currentState == CharacterState.CancellableAnimation && _moveDashForward.AttackStateFrames[_dashFrameCounter] >= 3)
        {
            _lastState = _currentState;
            _currentState = CharacterState.Stand;
            _dashFrameCounter = 0;
        }
        
        //Begin Dash Detection
        if (!_moveDashForward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Stand && _currentState != CharacterState.Crouch) return;
        _dashFrameCounter = 0;
        _lastState = _currentState;
        _currentState = CharacterState.Dash;
        _moveDirection = new Vector3(DashForwardXSpeed[0], 0, 0);
    }
    
    public void DashBackward(InputClass inputClass)
    {
        //Play out BackDash animation
        if (_currentState == CharacterState.BackDash && _dashFrameCounter < BackDashDuration - 1)
        {
            _dashFrameCounter++;
            if (_moveDashBackward.AttackStateFrames[_dashFrameCounter] == 1)
                _moveDirection.x = -DashBackwardXSpeed[1];
            else if (_moveDashBackward.AttackStateFrames[_dashFrameCounter] == 2)
                _moveDirection.x = -DashBackwardXSpeed[2];
        }
        
        //Exit dash animation
        if (_currentState == CharacterState.BackDash && _dashFrameCounter >= BackDashDuration - 1)
            _currentState = CharacterState.Stand;
        
        //Begin Dash Detection
        if (!_moveDashBackward.DetectMoveInput(inputClass)) return;
        if (_currentState != CharacterState.Stand) return;
        _dashFrameCounter = 0;
        _lastState = _currentState;
        _currentState = CharacterState.BackDash;
        _moveDirection = new Vector3(-DashBackwardXSpeed[0], 0, 0);
    }

    public void AirDashForward(InputClass inputClass)
    {
        //Play out AirDash Duration
        if (_currentState == CharacterState.AirDash && _dashFrameCounter < AirDashDuration)
        {
            _dashFrameCounter++;
            if (_dashFrameCounter > AirDashDuration * 0.75)
                _moveDirection.x = AirDashForwardSpeed[1];
        }
        
        //Exit dash animation
        if (_currentState == CharacterState.AirDash && _dashFrameCounter >= AirDashDuration)
        {
            _lastState = _currentState;
            _currentState = CharacterState.JumpForward;
            _dashFrameCounter = 0;
            _isIgnoringGravity = false;
        }
            
        
        //Begin AirDash Detection
        if (!_moveAirDashForward.DetectMoveInput(inputClass)) return;
        if (_currentState == CharacterState.Stand) return;
        _dashFrameCounter = 0;
        _lastState = _currentState;
        _currentState = CharacterState.AirDash;
        _isIgnoringGravity = true;
        _moveDirection = new Vector3(AirDashForwardSpeed[0],0,0);
    }

    public void LightAttack(InputClass inputClass)
    {
        if (_currentState == CharacterState.Stand && _moveLightAttack.DetectMoveInput(inputClass) &&
            _attackState == AttackState.None)
        {
            _attackFrameCounter = 0;
            _animator.Play("LightAttack");
            _attackState = AttackState.LightAttack;
        }

        if (_attackState == AttackState.LightAttack)
        {
            _attackFrameCounter++;
            switch (_moveLightAttack.Cancellability[_attackFrameCounter])
            {
                case 0:
                    _cancellableState = CancellableState.None;
                    break;
                case 1:
                    _cancellableState = CancellableState.EmptyCancellable;
                    break;
                case 2:
                    _cancellableState = CancellableState.NormalCancellable;
                    break;
                case 3:
                    _cancellableState = CancellableState.SpecialCancellable;
                    break;
                case 4:
                    _cancellableState = CancellableState.SuperCancellable;
                    break;
            }
        }
    }
    
    public void SpecialForward(InputClass inputClass)
    {
        if (!_moveSpecialForward.DetectMoveInput(inputClass)) return;
        
        Debug.Log("Hadoken");
    }
    public void SpecialBackward(InputClass inputClass)
    {
        if (!_moveSpecialForward.DetectMoveInput(inputClass)) return;
        
        Debug.Log("Hadoken");
    }
}
