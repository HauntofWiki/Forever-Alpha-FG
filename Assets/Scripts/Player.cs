using System.Collections;
using System.Collections.Generic;
using GamePlayScripts;
using UnityEngine;

public class Player
{
    public enum PlayerNumbers
    {
        None,
        Player1,
        Player2
    }

    public PlayerNumbers PlayerNumber;

    public enum JoystickTypes
    {
        None,
        Joystick,
        Controller,
        Keyboard
    }

    public JoystickTypes JoystickType;
    
    public enum Characters
    {
        Robot,
        Zeus,
        Ella,
        Frank,
        Fluffy,
        MadScientist
    }

    public InputManager InputManager;
    //Tracking current and last input to help prevent jumping around the menus too fast.
    public float CurrentXInput = 0;
    public float LastXInput = 0;
    public float CurrentYInput = 0;
    public float LastYInput = 0;
    public Characters CurrentCharacter;
    public Character FavoriteCharacter;
    
    public int JoystickNumber;
    public string JoystickName;

    public string PlayerName;
    public int WinCount;
    public int LossCount;
    public int Ranking;
    
    public Player( int joystickNumber, string joystickName)
    {
        JoystickNumber = joystickNumber;
        JoystickName = joystickName;
        InputManager = new InputManager(joystickNumber, joystickName);
    }

}