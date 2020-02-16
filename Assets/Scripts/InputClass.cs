using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputClass
{
    public string AxisName;
    public string Action;
    public float Value;
    public int DPadNumPad; //directional commands stored in numpad notation
    public float DPadX; //some commands will benefit from the X and Y axes being separate
    public float DPadY;
    public float Analog;
    public int LightAttackButtonDown;
    public int MediumAttackButtonDown;
    public int HeavyAttackButtonDown;
    public int SpecialAttackButtonDown;
    public int Auxiliary1ButtonDown;
    public int Auxiliary2ButtonDown;
    public float Auxiliary3ButtonDown;
    public float Auxiliary4ButtonDown;
    public float Auxiliary3AxisDown;
    public float Auxiliary4AxisDown;
    public int StartButtonDown;
    public int SelectButtonDown;
    public bool SubmitButtonDown;
    public bool CancelButtonDown;
    
    public int LightAttackButtonUp;
    public int MediumAttackButtonUp;
    public int HeavyAttackButtonUp;
    public int SpecialAttackButtonUp;
    public int Auxiliary1ButtonUp;
    public int Auxiliary2ButtonUp;
    public float Auxiliary3ButtonUp;
    public float Auxiliary4ButtonUp;
    public int StartButtonUp;
    public int SelectButtonUp;
    public bool SubmitButtonUp;
    public bool CancelButtonUp;

}
