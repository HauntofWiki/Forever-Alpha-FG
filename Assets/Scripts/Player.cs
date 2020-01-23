using System.Collections;
using System.Collections.Generic;
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