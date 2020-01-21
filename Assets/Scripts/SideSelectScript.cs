using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class SideSelectScript : MonoBehaviour
{
    private List<InputManager> _inputDevices;
    private string[] _joystickNames;
    private List<String> _undecidedPlayers;
    private string _sideLeftPlayer = "";
    private string _sideRightPlayer = "";
    private GameObject _centerColumn;
    private GameObject _centerText;
    private GameObject _LeftText;
    private GameObject _RightText;
    private Text _centerTextField;
    private Text _LeftTextField;
    private Text _RightTextField;
    private int LastInput;
    
    // Start is called before the first frame update
    void Start()
    {
        _inputDevices = new List<InputManager>();
        _joystickNames = Input.GetJoystickNames();
        _centerColumn = GameObject.Find("Center Column");
        _centerText = GameObject.Find("Center Text");
        _LeftText = GameObject.Find("Left Side Text");
        _RightText = GameObject.Find("Right Side Text");
        _centerTextField = _centerText.GetComponent<Text>();
        _LeftTextField = _LeftText.GetComponent<Text>();
        _RightTextField = _RightText.GetComponent<Text>();

        for (var i = 0; i <  _joystickNames.Length; i++)
        {
            var inputManager = new InputManager(i+1, _joystickNames[i]);
            _inputDevices.Add(inputManager);
        }
        
        
        _undecidedPlayers = new List<string>();
        for (var i = 0; i < _inputDevices.Count; i++)
        {
            Debug.Log("Added");
            _undecidedPlayers.Add("Player" + (i + 1));
        }

    }

    // Update is called once per frame
    void Update()
    {
        _joystickNames = Input.GetJoystickNames();
        Debug.Log(_inputDevices[0].Update(1).DPadX);
        
        

        for (int i = 0; i < _inputDevices.Count; i++)
        {
            //_inputDevices[i].Update(1).DPadX;
            
            
            if (_inputDevices[i].Update(1).DPadX < 0 && _sideLeftPlayer.Equals("") && _sideRightPlayer != "Player" + (i+1))
            {
                MovePlayerToSideLeft("Player" + (i+1));
                _undecidedPlayers.Remove("Player" + (i+1));
            }
            else if (_inputDevices[i].Update(1).DPadX > 0 && _sideRightPlayer.Equals("") && _sideLeftPlayer != "Player" + (i+1))
            {
                MovePlayerToSideRight("Player" + (i+1));
               _undecidedPlayers.Remove("Player" + (i+1));
            }
            
            if (_inputDevices[i].Update(1).DPadX > 0 && _sideLeftPlayer.Equals("Player" + (i+1)))
            {
                _undecidedPlayers.Add("Player" + (i+1));
                _sideLeftPlayer = "";
            }
            else if (_inputDevices[i].Update(1).DPadX < 0 && _sideRightPlayer.Equals("Player" + (i+1)))
            {
                _undecidedPlayers.Add("Player" + (i+1));
                _sideRightPlayer = "";
            }
        }
        
        var text = "";
        for (int i = 0; i < _undecidedPlayers.Count; i++)
        {
            text += _undecidedPlayers[i] + "\r\n";
        }

        _LeftTextField.text = _sideLeftPlayer;
        _centerTextField.text = text;
        _RightTextField.text = _sideRightPlayer;
    }

    private void MovePlayerToSideLeft(String Player)
    {
        _sideLeftPlayer = Player;
    }
    
    private void MovePlayerToSideRight(String Player)
    {
        _sideRightPlayer = Player;
    }
}
