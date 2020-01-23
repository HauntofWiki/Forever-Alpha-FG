using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    private List<Player> _undecidedPlayers;
    private string[] _joystickNames;
    private Player _sideLeftPlayer;
    private Player _sideRightPlayer;
    //Empty player is a place holder and will be used to pass in a single player to the next scene
    private Player _emptyPlayer;
    private GameObject _centerColumn;
    private GameObject _centerText;
    private GameObject _leftText;
    private GameObject _rightText;
    private Text _centerTextField;
    private Text _leftTextField;
    private Text _rightTextField;
    private int _lastInput;
    private Player _player1;
    private Player _player2;
    private bool _player1Confirmed = false;
    private bool _player2Confirmed = false;
    
    private enum GameStates
    {
        None,
        MainMenu,
        Options,
        SideSelect,
        CharacterSelect,
        MatchLoadScreen,
        PreMatch,
        MatchActive,
        PostMatch,
        WinScreen
    }

    private GameStates _gameState;
    
    // Start is called before the first frame update
    void Start()
    {

        _joystickNames = Input.GetJoystickNames();
        _centerColumn = GameObject.Find("Center Column");
        _centerText = GameObject.Find("Center Text");
        _leftText = GameObject.Find("Left Side Text");
        _rightText = GameObject.Find("Right Side Text");
        _centerTextField = _centerText.GetComponent<Text>();
        _leftTextField = _leftText.GetComponent<Text>();
        _rightTextField = _rightText.GetComponent<Text>();
        _gameState = GameStates.SideSelect;//Temporarily starting at side select
        _undecidedPlayers = new List<Player>();

        _emptyPlayer = new Player(-1, "") {PlayerNumber = Player.PlayerNumbers.None};
        _sideLeftPlayer = _emptyPlayer;
        _sideRightPlayer = _emptyPlayer;
        
        for (var i = 0; i <  _joystickNames.Length; i++)
        {    
            _undecidedPlayers.Add(new Player(i,_joystickNames[i]));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameState == GameStates.SideSelect)
        {
            _joystickNames = Input.GetJoystickNames();
            /*_undecidedPlayers = new List<InputDeviceClass>();
            
            //need to also check if the Player is already moved to a side before re-adding
            for (var i = 0; i <  _joystickNames.Length; i++)
            {    
                _undecidedDevices.Add(new InputDeviceClass(i,_joystickNames[i]));
            }*/

                Debug.Log(_undecidedPlayers[0].InputManager.Update(1).DPadX);



            for (int i = 0; i < _undecidedPlayers.Count; i++)
            {

                if (_undecidedPlayers[i].InputManager.Update(1).DPadX < 0 && _sideLeftPlayer.PlayerNumber == Player.PlayerNumbers.None &&
                    _sideRightPlayer != _undecidedPlayers[i])
                {
                    MovePlayerToSideLeft(_undecidedPlayers[i]);
                    _undecidedPlayers.Remove(_undecidedPlayers[i]);
                }
                else if (_undecidedPlayers[i].InputManager.Update(1).DPadX > 0 && _sideRightPlayer.PlayerNumber == Player.PlayerNumbers.None &&
                         _sideLeftPlayer !=  _undecidedPlayers[i])
                {
                    MovePlayerToSideRight(_undecidedPlayers[i]);
                    _undecidedPlayers.Remove(_undecidedPlayers[i]);
                }

                if (_undecidedPlayers[i].InputManager.Update(1).DPadX > 0 && _sideLeftPlayer.PlayerNumber == Player.PlayerNumbers.Player1)
                {
                    _undecidedPlayers.Add(_undecidedPlayers[i]);
                    _sideLeftPlayer.PlayerNumber = Player.PlayerNumbers.None;
                }
                else if (_undecidedPlayers[i].InputManager.Update(1).DPadX < 0 && _sideRightPlayer == _undecidedPlayers[i])
                {
                    _undecidedPlayers.Add(_undecidedPlayers[i]);
                    _sideRightPlayer.PlayerNumber = Player.PlayerNumbers.None;
                }

                if (_sideRightPlayer == _undecidedPlayers[i] && _undecidedPlayers[i].InputManager.Update(1).LightAttackButtonDown == 1)
                {
                   // _player1.ControllerNumber == 
                }
            }

            var text = "";
            for (int i = 0; i < _undecidedPlayers.Count; i++)
            {
                text += _undecidedPlayers[i].JoystickNumber + "\r\n";
            }

            if (_sideLeftPlayer.PlayerNumber != Player.PlayerNumbers.None)
                _leftTextField.text = _sideLeftPlayer.JoystickNumber.ToString();
            if (_sideRightPlayer.PlayerNumber != Player.PlayerNumbers.None)
                _rightTextField.text = _sideRightPlayer.JoystickNumber.ToString();
            _centerTextField.text = text;
            
        }
    }

    private void MovePlayerToSideLeft(Player player)
    {
        player.PlayerNumber = Player.PlayerNumbers.Player1;
        _sideLeftPlayer = player;
    }
    
    private void MovePlayerToSideRight(Player player)
    {
        player.PlayerNumber = Player.PlayerNumbers.Player2;
        _sideRightPlayer = player;
    }
}
