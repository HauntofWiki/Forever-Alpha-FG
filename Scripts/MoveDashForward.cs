using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDashForward : ICharacterMove
{
    public string MoveName = "ForwardDash";
    
    private int _slowDownFriction;
    private int _minDuration;
    private int _actionCounter;
    private bool _dashIsActive;
    
    
    private readonly int _inputLimit;
    private readonly int[] _movePattern = {1,0,1}; //Dash uses X-axis only
    private readonly bool[] _patternMatch = {false, false, false};
    private int _lastMove;
    private int _moveDetectCounter;
    
    public MoveDashForward()
    {
        _lastMove = -1;
        _moveDetectCounter = 0;
        _inputLimit = 20;
        _slowDownFriction = 5;
        _minDuration = 20;
        _actionCounter = 0;
        _dashIsActive = false;
    }
    
    
    public bool DetectMoveInput(InputClass inputClass)
    {
        if (_dashIsActive)
            _actionCounter++;
        else
            _actionCounter = 0;
        
        if (_dashIsActive && inputClass.DPadNumPad == 6)
            return true;
        if (_actionCounter >= _minDuration && inputClass.DPadNumPad != 6)
        {
            _dashIsActive = false;
            return false;
        }
        
        _moveDetectCounter++;
        
        if (_moveDetectCounter >= _inputLimit)
            ResetInputDetect();
        
        if (_lastMove == inputClass.DPadNumPad)
            return false;
        if (inputClass.DPadX == _movePattern[0] && !_patternMatch[0])
        {
            _moveDetectCounter = 0;
            _patternMatch[0] = true;            
        }
        if (inputClass.DPadX == _movePattern[1] && _patternMatch[0])
            _patternMatch[1] = true;
        if (inputClass.DPadX == _movePattern[2] && _patternMatch[1])
            _patternMatch[2] = true;
        if (_patternMatch[0] && _patternMatch[1] && _patternMatch[2])
        {
            _dashIsActive = true;
            ResetInputDetect();
            return true;
        }
        

        
        _lastMove = inputClass.DPadNumPad;
        return false;
    }

    public Vector3 PerformAction(ref Character.CharacterState characterState)
    {

        characterState = Character.CharacterState.Dash;
        var xVelocity = 1;//DashForwardXSpeed;
        var moveDirection = new Vector3(xVelocity,0,0);
        return moveDirection;
    }

    public void ResetInputDetect()
    {
        _lastMove = -1;
        _moveDetectCounter = 0;
        _patternMatch[0] = false;
        _patternMatch[1] = false;
        _patternMatch[2] = false;
    }
}
