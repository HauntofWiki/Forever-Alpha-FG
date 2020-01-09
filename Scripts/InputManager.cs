using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class InputManager
{
    private int _inputQueueSize; //Length of input Queue
    private int _numberOfInputs; //Number of inputs to monitor
    private int _playerNumber; //1: Player1 2: Player
    private string _playerPrefix; //P1 or P2
    private InputClass _currentInput;

    private enum PlayerActionEnum
    {
        Neutral,
        WalkForward,
        WalkBackward,
        DashForward,
        DashBackward,
        JumpNeutral,
        JumpForward,
        JumpBack
    };
    
    //Definition of games input

    public InputManager(int playerNumber)
    {
        _playerNumber = playerNumber; 
        _inputQueueSize = 50;
        _numberOfInputs = 10;

        switch (playerNumber)
        {
            case 1:
                _playerPrefix = "P1";
                break;
            case 2:
                _playerPrefix = "P2";
                break;
            default:
                Debug.Log("Invalid Player Number");
                break;
        }
    }
    
    //Inserts inputs into Queue once per frame - inputs are currently set up for Playstation controller via project settings
    public InputClass Update(int characterOrientation)
    {
        _currentInput = new InputClass();
        
        //Read Direction inputs into Numpad Notation (1-9)
        if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation > 0 &&
            Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation == 0)
            _currentInput.DPadNumPad = 6;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation > 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation > 0)
            _currentInput.DPadNumPad = 9;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation == 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation > 0)
            _currentInput.DPadNumPad = 8;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation < 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation > 0)
            _currentInput.DPadNumPad = 7;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation < 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation == 0)
            _currentInput.DPadNumPad = 4;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation < 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation < 0)
            _currentInput.DPadNumPad = 1;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation == 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation < 0)
            _currentInput.DPadNumPad = 2;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation > 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation < 0)
            _currentInput.DPadNumPad = 3;
        else if (Input.GetAxis(_playerPrefix + "DPadX") * characterOrientation == 0 &&
                 Input.GetAxis(_playerPrefix + "DPadY") * characterOrientation == 0)
            _currentInput.DPadNumPad = 5;
        else _currentInput.DPadNumPad = 5;
        
        //Read direction inputs into tradition X/Y Axes
        _currentInput.DPadX = Input.GetAxis(_playerPrefix + "DPadX");
        _currentInput.DPadY = Input.GetAxis(_playerPrefix + "DPadY");
        
        //Read ButtonDown commands
        _currentInput.LightAttackButtonDown = Input.GetButtonDown(_playerPrefix + "LightAttackButton") ? 1 : 0;
        _currentInput.MediumAttackButtonDown = Input.GetButtonDown(_playerPrefix + "MediumAttackButton") ? 1 : 0;
        _currentInput.HeavyAttackButtonDown = Input.GetButtonDown(_playerPrefix + "HeavyAttackButton") ? 1 : 0;
        _currentInput.SpecialAttackButtonDown = Input.GetButtonDown(_playerPrefix + "SpecialAttackButton") ? 1 : 0;
        _currentInput.Auxiliary1ButtonDown = Input.GetButtonDown(_playerPrefix + "Auxiliary1Button") ? 1 : 0;
        _currentInput.Auxiliary2ButtonDown = Input.GetButtonDown(_playerPrefix + "Auxiliary2Button") ? 1 : 0;
        _currentInput.Auxiliary3ButtonDown = Input.GetAxis(_playerPrefix + "Auxiliary3Button");
        _currentInput.Auxiliary4ButtonDown = Input.GetAxis(_playerPrefix + "Auxiliary4Button");
        _currentInput.StartButtonDown = Input.GetButtonDown(_playerPrefix + "StartButton") ? 1 : 0;
        _currentInput.SelectButtonDown = Input.GetButtonDown(_playerPrefix + "SelectButton") ? 1 : 0;
        
        //Read ButtonUp commands
        _currentInput.LightAttackButtonUp = Input.GetButtonDown(_playerPrefix + "LightAttackButton") ? 1 : 0;
        _currentInput.MediumAttackButtonUp = Input.GetButtonDown(_playerPrefix + "MediumAttackButton") ? 1 : 0;
        _currentInput.HeavyAttackButtonUp = Input.GetButtonDown(_playerPrefix + "HeavyAttackButton") ? 1 : 0;
        _currentInput.SpecialAttackButtonUp = Input.GetButtonDown(_playerPrefix + "SpecialAttackButton") ? 1 : 0;
        _currentInput.Auxiliary1ButtonUp = Input.GetButtonDown(_playerPrefix + "Auxiliary1Button") ? 1 : 0;
        _currentInput.Auxiliary2ButtonUp = Input.GetButtonDown(_playerPrefix + "Auxiliary2Button") ? 1 : 0;
        _currentInput.Auxiliary3ButtonUp = Input.GetAxis(_playerPrefix + "Auxiliary3Button");
        _currentInput.Auxiliary4ButtonUp = Input.GetAxis(_playerPrefix + "Auxiliary4Button");
        _currentInput.StartButtonUp = Input.GetButtonDown(_playerPrefix + "StartButton") ? 1 : 0;
        _currentInput.SelectButtonUp = Input.GetButtonDown(_playerPrefix + "SelectButton") ? 1 : 0;
        
        return _currentInput;
    }
}
