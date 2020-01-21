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
    private InputClass _currentInput;
    
    public int CharacterId { get; set; }
    public string JoystickName { get; set; }
    public string JoystickType { get; set; }

    private int _joystickNumber;
    private string _DigitalAxisXAccessName;//i.e. Joy1Axis1
    private string _DigitalAxisYAccessName;
    private int _digitalAxisXNumber;
    private int _digitalAxisYNumber;
    private string _analogLeftAxisXName;
    private string _analogLeftAxisYName;
    private string _analogRightAxisXName;
    private string _analogRightAxisYName;
    private int _analogRightAxisXNumber;
    private int _analogRightAxisYNumber;
    private string _auxiliaryButton3AxisName; //L trigger button
    private string _auxiliaryButton4AxisName; //R trigger button
    private int _auxiliaryButton3AxisNumber; //L trigger button
    private int _auxiliaryButton4AxisNumber; //R trigger button
    public KeyCode LightAttackButton { get; set; } //i.e. KeyBode.Joystick1Button0
    public KeyCode MediumAttackButton { get; set; }
    public KeyCode HeavyAttackButton { get; set; } 
    public KeyCode SpecialAttackButton { get; set; } 
    public KeyCode Auxiliary1Button { get; set; }
    public KeyCode Auxiliary2Button { get; set; }
    public KeyCode Auxiliary3Button { get; set; } //L trigger button
    public KeyCode Auxiliary4Button { get; set; } //R trigger button
    public KeyCode Auxiliary5Button { get; set; }
    public KeyCode Auxiliary6Button { get; set; }
    public KeyCode Auxiliary7Button { get; set; }
    public KeyCode Auxiliary8Button { get; set; }
    public KeyCode StartButton { get; set; } 
    public KeyCode SelectButton { get; set; }
    
    //Definition of games input

    public InputManager(int joystickNumber, string joystickName)
    {
        _inputQueueSize = 50;
        _numberOfInputs = 10;
        
        SetDefaultMapping(joystickNumber,joystickName);
    }
    
    //Inserts inputs into Queue once per frame - inputs are currently set up for Playstation controller via project settings
    public InputClass Update(int characterOrientation)
    {
        _currentInput = new InputClass();
        
        //Read Direction inputs into Numpad Notation (1-9)
        if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation > 0 &&
            Input.GetAxis(_DigitalAxisYAccessName) == 0)
            _currentInput.DPadNumPad = 6;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation > 0 &&
                 Input.GetAxis(_DigitalAxisYAccessName) > 0)
            _currentInput.DPadNumPad = 9;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation == 0 &&
                 Input.GetAxis(_DigitalAxisYAccessName) > 0)
            _currentInput.DPadNumPad = 8;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation < 0 &&
                 Input.GetAxis(_DigitalAxisYAccessName) > 0)
            _currentInput.DPadNumPad = 7;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation < 0 &&
                 Input.GetAxis(_DigitalAxisYAccessName) == 0)
            _currentInput.DPadNumPad = 4;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation < 0 &&
                 Input.GetAxis(_DigitalAxisYAccessName) < 0)
            _currentInput.DPadNumPad = 1;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation == 0 &&
                 Input.GetAxis(_DigitalAxisYAccessName) < 0)
            _currentInput.DPadNumPad = 2;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation > 0 &&
                 Input.GetAxis(_DigitalAxisYAccessName) < 0)
            _currentInput.DPadNumPad = 3;
        else if (Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation == 0 &&
                 Input.GetAxis(_DigitalAxisXAccessName) == 0)
            _currentInput.DPadNumPad = 5;
        else _currentInput.DPadNumPad = 5;
        
        //Read direction inputs into tradition X/Y Axes
        _currentInput.DPadX = Input.GetAxis(_DigitalAxisXAccessName) * characterOrientation;
        _currentInput.DPadY = Input.GetAxis(_DigitalAxisYAccessName);
        
        //Read ButtonDown commands
        _currentInput.LightAttackButtonDown = Input.GetKeyDown(LightAttackButton) ? 1 : 0;
        _currentInput.MediumAttackButtonDown = Input.GetKeyDown(MediumAttackButton) ? 1 : 0;
        _currentInput.HeavyAttackButtonDown = Input.GetKeyDown(HeavyAttackButton) ? 1 : 0;
        _currentInput.SpecialAttackButtonDown = Input.GetKeyDown(SpecialAttackButton) ? 1 : 0;
        _currentInput.Auxiliary1ButtonDown = Input.GetKeyDown(Auxiliary1Button) ? 1 : 0;
        _currentInput.Auxiliary2ButtonDown = Input.GetKeyDown(Auxiliary2Button) ? 1 : 0;
        _currentInput.Auxiliary3ButtonDown = Input.GetKeyDown(Auxiliary3Button) ? 1 : 0;
        _currentInput.Auxiliary4ButtonDown = Input.GetKeyDown(Auxiliary4Button) ? 1 : 0;
        _currentInput.StartButtonDown = Input.GetKeyDown(StartButton) ? 1 : 0;
        _currentInput.SelectButtonDown = Input.GetKeyDown(SelectButton) ? 1 : 0;
        
        //Trigger buttons are sometimes assigned by axis
        _currentInput.Auxiliary3AxisDown = Input.GetAxis(_auxiliaryButton3AxisName);
        _currentInput.Auxiliary4AxisDown = Input.GetAxis(_auxiliaryButton4AxisName);
        
        //Read ButtonDown commands
        _currentInput.LightAttackButtonDown = Input.GetKeyUp(LightAttackButton) ? 1 : 0;
        _currentInput.MediumAttackButtonDown = Input.GetKeyUp(MediumAttackButton) ? 1 : 0;
        _currentInput.HeavyAttackButtonDown = Input.GetKeyUp(HeavyAttackButton) ? 1 : 0;
        _currentInput.SpecialAttackButtonDown = Input.GetKeyUp(SpecialAttackButton) ? 1 : 0;
        _currentInput.Auxiliary1ButtonDown = Input.GetKeyUp(Auxiliary1Button) ? 1 : 0;
        _currentInput.Auxiliary2ButtonDown = Input.GetKeyUp(Auxiliary2Button) ? 1 : 0;
        _currentInput.Auxiliary3ButtonDown = Input.GetKeyUp(Auxiliary3Button) ? 1 : 0;
        _currentInput.Auxiliary4ButtonDown = Input.GetKeyUp(Auxiliary4Button) ? 1 : 0;
        _currentInput.StartButtonDown = Input.GetKeyUp(StartButton) ? 1 : 0;
        _currentInput.SelectButtonDown = Input.GetKeyUp(SelectButton) ? 1 : 0;
        //Debug.Log(_currentInput.LightAttackButtonDown + " " + Input.GetKeyDown(LightAttackButton));
        return _currentInput;
    }
    public void SetDefaultMapping(int joystickNumber, string joystickName) //GetJoystickNames[JoystickNumber + 1]
    {
        //Default Joystick mappings
        if (joystickName == "Controller (XBOX 360 For Windows)")
        {
            _DigitalAxisXAccessName = GetJoystickAccessName(joystickNumber, 6);
            _DigitalAxisYAccessName = GetJoystickAccessName(joystickNumber, 7);
            _analogLeftAxisXName = "Horizontal";
            _analogLeftAxisYName = "Vertical";
            _analogRightAxisXName = GetJoystickAccessName(joystickNumber, 4);
            _analogRightAxisYName = GetJoystickAccessName(joystickNumber, 5);
            _auxiliaryButton3AxisName = GetJoystickAccessName(joystickNumber, 9);
            _auxiliaryButton4AxisName = GetJoystickAccessName(joystickNumber, 10);
            LightAttackButton = GetJoystickButton(joystickNumber,2);
            MediumAttackButton = GetJoystickButton(joystickNumber,3);
            HeavyAttackButton = GetJoystickButton(joystickNumber,1);
            SpecialAttackButton = GetJoystickButton(joystickNumber,0);
            StartButton = GetJoystickButton(joystickNumber,7);
            SelectButton = GetJoystickButton(joystickNumber,6);
            Auxiliary1Button = GetJoystickButton(joystickNumber,4);
            Auxiliary2Button = GetJoystickButton(joystickNumber,5);
            Auxiliary5Button = GetJoystickButton(joystickNumber,8);
            Auxiliary6Button = GetJoystickButton(joystickNumber,9);
            Auxiliary3Button = KeyCode.None;
            Auxiliary4Button = KeyCode.None;
            Auxiliary7Button = KeyCode.None;
            Auxiliary8Button = KeyCode.None;
        } 
        else if (joystickName == "Madcatz fightstick")
        {
            _DigitalAxisXAccessName = GetJoystickAccessName(joystickNumber, 7);
            _DigitalAxisYAccessName = GetJoystickAccessName(joystickNumber, 8);
            _analogLeftAxisXName = "Horizontal";
            _analogLeftAxisYName = "Vertical";
            _analogRightAxisXName = GetJoystickAccessName(joystickNumber, 3);
            _analogRightAxisYName = GetJoystickAccessName(joystickNumber, 4);
            _auxiliaryButton3AxisName = GetJoystickAccessName(joystickNumber, 5);
            _auxiliaryButton4AxisName = GetJoystickAccessName(joystickNumber, 6);
            LightAttackButton = GetJoystickButton(joystickNumber,0);
            MediumAttackButton = GetJoystickButton(joystickNumber,3);
            HeavyAttackButton = GetJoystickButton(joystickNumber,2);
            SpecialAttackButton = GetJoystickButton(joystickNumber,1);
            StartButton = GetJoystickButton(joystickNumber,9);
            SelectButton = GetJoystickButton(joystickNumber,8);
            Auxiliary1Button = GetJoystickButton(joystickNumber,4);
            Auxiliary2Button = GetJoystickButton(joystickNumber,5);
            Auxiliary3Button = GetJoystickButton(joystickNumber,6);
            Auxiliary4Button = GetJoystickButton(joystickNumber,7);
            Auxiliary5Button = GetJoystickButton(joystickNumber,10);
            Auxiliary6Button = GetJoystickButton(joystickNumber,11);
            Auxiliary7Button = GetJoystickButton(joystickNumber,12);
            Auxiliary8Button = GetJoystickButton(joystickNumber,13);
        }
        else if (joystickName == "Keyboard")
        {
            _DigitalAxisXAccessName = GetJoystickAccessName(joystickNumber, 7);
            _DigitalAxisYAccessName = GetJoystickAccessName(joystickNumber, 8);
            _analogLeftAxisXName = "Horizontal";
            _analogLeftAxisYName = "Vertical";
            _analogRightAxisXName = GetJoystickAccessName(joystickNumber, 3);
            _analogRightAxisYName = GetJoystickAccessName(joystickNumber, 4);
            _auxiliaryButton3AxisName = GetJoystickAccessName(joystickNumber, 5);
            _auxiliaryButton4AxisName = GetJoystickAccessName(joystickNumber, 6);
            LightAttackButton = GetJoystickButton(joystickNumber,0);
            MediumAttackButton = GetJoystickButton(joystickNumber,3);
            HeavyAttackButton = GetJoystickButton(joystickNumber,2);
            SpecialAttackButton = GetJoystickButton(joystickNumber,1);
            StartButton = GetJoystickButton(joystickNumber,9);
            SelectButton = GetJoystickButton(joystickNumber,8);
            Auxiliary1Button = GetJoystickButton(joystickNumber,4);
            Auxiliary2Button = GetJoystickButton(joystickNumber,5);
            Auxiliary3Button = GetJoystickButton(joystickNumber,6);
            Auxiliary4Button = GetJoystickButton(joystickNumber,7);
            Auxiliary5Button = GetJoystickButton(joystickNumber,10);
            Auxiliary6Button = GetJoystickButton(joystickNumber,11);
            Auxiliary7Button = GetJoystickButton(joystickNumber,12);
            Auxiliary8Button = GetJoystickButton(joystickNumber,13);
        }
        else
        {
            Debug.Log("No Controller Connected");
            _DigitalAxisXAccessName = "None";
            _DigitalAxisYAccessName = "None";
            _analogLeftAxisXName = "None";
            _analogLeftAxisYName = "None";
            _analogRightAxisXName = "None";
            _analogRightAxisYName = "None";
            _auxiliaryButton3AxisName = "None";
            _auxiliaryButton4AxisName = "None";
            LightAttackButton = KeyCode.None;
            MediumAttackButton = KeyCode.None;
            HeavyAttackButton = KeyCode.None;
            SpecialAttackButton = KeyCode.None;
            StartButton = KeyCode.None;
            SelectButton = KeyCode.None;
            Auxiliary1Button = KeyCode.None;
            Auxiliary2Button = KeyCode.None;
            Auxiliary3Button = KeyCode.None;
            Auxiliary4Button = KeyCode.None;
            Auxiliary5Button = KeyCode.None;
            Auxiliary6Button = KeyCode.None;
            Auxiliary7Button = KeyCode.None;
            Auxiliary8Button = KeyCode.None;
        }
    }

    private static string GetJoystickAccessName(int joystickNumber, int axisNumber)
    {
        Debug.Log("Joy" + (joystickNumber) + "Axis" + axisNumber);
        return "Joy" + (joystickNumber) + "Axis" + axisNumber;
    }
    
    private static KeyCode GetJoystickButton(int joystickNumber, int buttonNumber)
    {
        return (KeyCode) System.Enum.Parse(typeof(KeyCode), "Joystick" + joystickNumber + "Button" + buttonNumber);
    }
}
