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
    public InputClass CurrentInput;
    
    public int CharacterId { get; set; }
    public string JoystickName { get; set; }
    public string JoystickType { get; set; }

    private int _joystickNumber;
    private string _digitalAxisXAccessName;//i.e. Joy1Axis1
    private string _digitalAxisYAccessName;
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
    public KeyCode SubmitButton { get; set; }
    public KeyCode CancelButton { get; set; }
    
    //Definition of games input

    public InputManager(int joystickNumber, string joystickName)
    {
        _inputQueueSize = 50;
        _numberOfInputs = 10;
        JoystickName = joystickName;
        //Debug.Log("Joy" + joystickNumber + ", " + Input.GetJoystickNames()[joystickNumber]);
        SetDefaultMapping(joystickNumber + 1,joystickName);
    }
    
    //Inserts inputs into Queue once per frame - inputs are currently set up for Playstation controller via project settings
    public InputClass GetInput(int characterOrientation)
    {
        //if (characterOrientation == 0)
            //Debug.Log("CharacterOrientation is 0");
        
        CurrentInput = new InputClass();
        
        //Read Direction inputs into Numpad Notation (1-9)
        if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation > 0 &&
            Input.GetAxis(_digitalAxisYAccessName) == 0)
            CurrentInput.DPadNumPad = 6;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation > 0 &&
                 Input.GetAxis(_digitalAxisYAccessName) > 0)
            CurrentInput.DPadNumPad = 9;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation == 0 &&
                 Input.GetAxis(_digitalAxisYAccessName) > 0)
            CurrentInput.DPadNumPad = 8;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation < 0 &&
                 Input.GetAxis(_digitalAxisYAccessName) > 0)
            CurrentInput.DPadNumPad = 7;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation < 0 &&
                 Input.GetAxis(_digitalAxisYAccessName) == 0)
            CurrentInput.DPadNumPad = 4;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation < 0 &&
                 Input.GetAxis(_digitalAxisYAccessName) < 0)
            CurrentInput.DPadNumPad = 1;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation == 0 &&
                 Input.GetAxis(_digitalAxisYAccessName) < 0)
            CurrentInput.DPadNumPad = 2;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation > 0 &&
                 Input.GetAxis(_digitalAxisYAccessName) < 0)
            CurrentInput.DPadNumPad = 3;
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation == 0 &&
                 Input.GetAxis(_digitalAxisXAccessName) == 0)
            CurrentInput.DPadNumPad = 5;
        else CurrentInput.DPadNumPad = 5;
        
        //Debug.Log(Input.GetAxis(_digitalAxisXAccessName));
        //Read direction inputs into tradition X/Y Axes
        if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation > 0)
        {
            CurrentInput.DPadX = 1;
        } 
        else if (Input.GetAxis(_digitalAxisXAccessName) * characterOrientation < 0)
        {
            CurrentInput.DPadX = -1;
        }
        else
        {
            CurrentInput.DPadX = 0;
        }
        //Y Axis
        if (Input.GetAxis(_digitalAxisYAccessName) > 0)
        {
            CurrentInput.DPadY = 1;
        } 
        else if (Input.GetAxis(_digitalAxisYAccessName) < 0)
        {
            CurrentInput.DPadY = -1;
        }
        else
        {
            CurrentInput.DPadY = 0;
        }

        //Read ButtonDown commands
        CurrentInput.LightAttackButtonDown = Input.GetKeyDown(LightAttackButton) ? 1 : 0;
        CurrentInput.MediumAttackButtonDown = Input.GetKeyDown(MediumAttackButton) ? 1 : 0;
        CurrentInput.HeavyAttackButtonDown = Input.GetKeyDown(HeavyAttackButton) ? 1 : 0;
        CurrentInput.SpecialAttackButtonDown = Input.GetKeyDown(SpecialAttackButton) ? 1 : 0;
        CurrentInput.Auxiliary1ButtonDown = Input.GetKeyDown(Auxiliary1Button) ? 1 : 0;
        CurrentInput.Auxiliary2ButtonDown = Input.GetKeyDown(Auxiliary2Button) ? 1 : 0;
        CurrentInput.Auxiliary3ButtonDown = Input.GetKeyDown(Auxiliary3Button) ? 1 : 0;
        CurrentInput.Auxiliary4ButtonDown = Input.GetKeyDown(Auxiliary4Button) ? 1 : 0;
        CurrentInput.StartButtonDown = Input.GetKeyDown(StartButton) ? 1 : 0;
        CurrentInput.SelectButtonDown = Input.GetKeyDown(SelectButton) ? 1 : 0;
        //Submit and Cancel buttons
        CurrentInput.SubmitButtonDown = Input.GetKeyDown(SubmitButton);
        CurrentInput.CancelButtonDown = Input.GetKeyDown(CancelButton);
        
        
        //Trigger buttons are sometimes assigned by axis
        CurrentInput.Auxiliary3AxisDown = Input.GetAxis(_auxiliaryButton3AxisName);
        CurrentInput.Auxiliary4AxisDown = Input.GetAxis(_auxiliaryButton4AxisName);
        
        //Read ButtonUp commands
        CurrentInput.LightAttackButtonUp = Input.GetKeyUp(LightAttackButton) ? 1 : 0;
        CurrentInput.MediumAttackButtonUp = Input.GetKeyUp(MediumAttackButton) ? 1 : 0;
        CurrentInput.HeavyAttackButtonUp = Input.GetKeyUp(HeavyAttackButton) ? 1 : 0;
        CurrentInput.SpecialAttackButtonUp = Input.GetKeyUp(SpecialAttackButton) ? 1 : 0;
        CurrentInput.Auxiliary1ButtonUp = Input.GetKeyUp(Auxiliary1Button) ? 1 : 0;
        CurrentInput.Auxiliary2ButtonUp = Input.GetKeyUp(Auxiliary2Button) ? 1 : 0;
        CurrentInput.Auxiliary3ButtonUp = Input.GetKeyUp(Auxiliary3Button) ? 1 : 0;
        CurrentInput.Auxiliary4ButtonUp = Input.GetKeyUp(Auxiliary4Button) ? 1 : 0;
        CurrentInput.StartButtonUp = Input.GetKeyUp(StartButton) ? 1 : 0;
        CurrentInput.SelectButtonUp = Input.GetKeyUp(SelectButton) ? 1 : 0;
        //Submit and Cancel buttons
        CurrentInput.SubmitButtonUp = Input.GetKeyUp(SubmitButton);
        CurrentInput.CancelButtonUp = Input.GetKeyUp(CancelButton);
        
        
        //showsDebug.Log(CurrentInput.SubmitButtonDown);

        return CurrentInput;
    }
    public void SetDefaultMapping(int joystickNumber, string joystickName) //GetJoystickNames[JoystickNumber + 1]
    {
        //Default Joystick mappings
        if (joystickName == "Controller (XBOX 360 For Windows)")
        {
            _digitalAxisXAccessName = GetJoystickAccessName(joystickNumber, 6);
            _digitalAxisYAccessName = GetJoystickAccessName(joystickNumber, 7);
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

            SubmitButton = GetJoystickButton(joystickNumber, 0);
            CancelButton = GetJoystickButton(joystickNumber, 1);


        } 
        else if (joystickName == "Madcatz fightstick")
        {
            _digitalAxisXAccessName = GetJoystickAccessName(joystickNumber, 7);
            _digitalAxisYAccessName = GetJoystickAccessName(joystickNumber, 8);
            _analogLeftAxisXName = "Horizontal";
            _analogLeftAxisYName = "Vertical";
            _analogRightAxisXName = GetJoystickAccessName(joystickNumber, 3);
            _analogRightAxisYName = GetJoystickAccessName(joystickNumber, 4);
            _auxiliaryButton3AxisName = GetJoystickAccessName(joystickNumber, 5);
            _auxiliaryButton4AxisName = GetJoystickAccessName(joystickNumber, 6);
            LightAttackButton = GetJoystickButton(joystickNumber,0);
            MediumAttackButton = GetJoystickButton(joystickNumber,3);
            HeavyAttackButton = GetJoystickButton(joystickNumber,5);
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
            
            SubmitButton = GetJoystickButton(joystickNumber, 1);
            CancelButton = GetJoystickButton(joystickNumber, 2);
        }
        else if (joystickName == "Keyboard")
        {
            _digitalAxisXAccessName = "Horizontal";
            _digitalAxisYAccessName = "Vertical";
            _analogLeftAxisXName = "None";
            _analogLeftAxisYName = "None";
            _analogRightAxisXName = "None";
            _analogRightAxisYName = "None";
            _auxiliaryButton3AxisName = "None";
            _auxiliaryButton4AxisName = "None";
            LightAttackButton = KeyCode.H;
            MediumAttackButton = KeyCode.J;
            HeavyAttackButton = KeyCode.K;
            SpecialAttackButton = KeyCode.L;
            StartButton = KeyCode.Escape;
            SelectButton = KeyCode.None;
            Auxiliary1Button = KeyCode.None;
            Auxiliary2Button = KeyCode.None;
            Auxiliary3Button = KeyCode.None;
            Auxiliary4Button = KeyCode.None;
            Auxiliary5Button = KeyCode.None;
            Auxiliary6Button = KeyCode.None;
            Auxiliary7Button = KeyCode.None;
            Auxiliary8Button = KeyCode.None;

            SubmitButton = KeyCode.Return;
            CancelButton = KeyCode.Escape;
        }
        else if (joystickName == "Keyboard2")
        {
            _digitalAxisXAccessName = "Horizontal2";
            _digitalAxisYAccessName = "Vertical2";
            _analogLeftAxisXName = "None";
            _analogLeftAxisYName = "None";
            _analogRightAxisXName = "None";
            _analogRightAxisYName = "None";
            _auxiliaryButton3AxisName = "None";
            _auxiliaryButton4AxisName = "None";
            LightAttackButton = KeyCode.Keypad4;
            MediumAttackButton = KeyCode.Keypad5;
            HeavyAttackButton = KeyCode.Keypad6;
            SpecialAttackButton = KeyCode.Keypad2;
            StartButton = KeyCode.End;
            SelectButton = KeyCode.Home;
            Auxiliary1Button = KeyCode.None;
            Auxiliary2Button = KeyCode.None;
            Auxiliary3Button = KeyCode.None;
            Auxiliary4Button = KeyCode.None;
            Auxiliary5Button = KeyCode.None;
            Auxiliary6Button = KeyCode.None;
            Auxiliary7Button = KeyCode.None;
            Auxiliary8Button = KeyCode.None;

            SubmitButton = KeyCode.KeypadEnter;
            CancelButton = KeyCode.Delete;
        }
        else
        {
            //Debug.Log("No Controller Connected");
            _digitalAxisXAccessName = "None";
            _digitalAxisYAccessName = "None";
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
        //Debug.Log("Joy" + (joystickNumber) + "Axis" + axisNumber);
        return "Joy" + (joystickNumber) + "Axis" + axisNumber;
    }
    
    private static KeyCode GetJoystickButton(int joystickNumber, int buttonNumber)
    {
        return (KeyCode) System.Enum.Parse(typeof(KeyCode), "Joystick" + joystickNumber + "Button" + buttonNumber);
    }
}
