using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts.CharacterSelect
{
    public class SideSelect
    {
        private List<Player> _players;
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


        // Start is called before the first frame update
        public SideSelect()
        {
            _joystickNames = Input.GetJoystickNames();
            _centerColumn = GameObject.Find("Center Column");
            _centerText = GameObject.Find("Center Text");
            _leftText = GameObject.Find("Left Side Text");
            _rightText = GameObject.Find("Right Side Text");
            _centerTextField = _centerText.GetComponent<Text>();
            _leftTextField = _leftText.GetComponent<Text>();
            _rightTextField = _rightText.GetComponent<Text>();
            _players = new List<Player>();
            //_emptyPlayer defaults joystickNumber to -1
            _emptyPlayer = new Player(-1, "") {PlayerNumber = Player.PlayerNumbers.None};
            _sideLeftPlayer = _emptyPlayer;
            _sideRightPlayer = _emptyPlayer;

            for (var i = 0; i < _joystickNames.Length; i++)
            {
                _players.Add(new Player(i, _joystickNames[i]) {PlayerNumber = Player.PlayerNumbers.None});
            }
        }

        // Update is called once per frame
        public void Update()
        {
            _joystickNames = Input.GetJoystickNames();
            //Debug.Log(_players.Count); //[0].InputManager.Update(1).DPadX);


            //For each device determine if the DPAD moved to the left or right and assign the appropriate side
            for (var i = 0; i < _players.Count; i++)
            {
                _players[i].CurrentXInput = _players[i].InputManager.GetInput(1).DPadX;

                //Compare X input to prevent jumping across the menu too fast
                if (_players[i].CurrentXInput != _players[i].LastXInput)
                {
                    //Left input for all players
                    if (_players[i].CurrentXInput < 0)
                    {
                        //Center to Left side
                        if (_sideLeftPlayer.JoystickNumber == -1 &&
                            _sideRightPlayer.JoystickNumber != _players[i].JoystickNumber)
                        {
                            _players[i].PlayerNumber = Player.PlayerNumbers.Player1;
                            _sideLeftPlayer = _players[i];
                            _leftTextField.text = _players[i].JoystickNumber.ToString();
                        }

                        //Right side to back center
                        if (_sideRightPlayer.JoystickNumber == _players[i].JoystickNumber)
                        {
                            _sideRightPlayer = _emptyPlayer;
                            _players[i].PlayerNumber = Player.PlayerNumbers.None;
                            _rightTextField.text = "";
                        }
                    }
                    //Right input for all players
                    else if (_players[i].CurrentXInput > 0)
                    {
                        //Center to Right side
                        if (_sideRightPlayer.JoystickNumber == -1 &&
                            _sideLeftPlayer.JoystickNumber != _players[i].JoystickNumber)
                        {
                            _players[i].PlayerNumber = Player.PlayerNumbers.Player2;
                            _sideRightPlayer = _players[i];
                            _rightTextField.text = _players[i].JoystickNumber.ToString();
                        }

                        //Left side back to center
                        if (_sideLeftPlayer.JoystickNumber == _players[i].JoystickNumber)
                        {
                            _sideLeftPlayer = _emptyPlayer;
                            _players[i].PlayerNumber = Player.PlayerNumbers.None;
                            _leftTextField.text = "";
                        }
                    }
                }

                //Detect confirm or cancel to advance to the next screen or go back
                if (_players[i].InputManager.GetInput(1).SubmitButtonDown)
                {
                    //Confirmed Side, select Over
                }

                if (_players[i].InputManager.GetInput(1).CancelButtonDown)
                {
                    //Return to previous scene
                }

                _players[i].LastXInput = _players[i].CurrentXInput;
            }

            var text = "";
            for (var i = 0; i < _players.Count; i++)
            {
                if (_players[i].PlayerNumber == Player.PlayerNumbers.None)
                    text += _players[i].JoystickNumber + "\r\n";
            }

            _centerTextField.text = text;
        }
    }
}