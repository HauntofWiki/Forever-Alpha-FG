using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts.GamePlay
{
    public class PauseMenu
    {
        private readonly GameObject _menuObject;
        private Image _imageClose;
        private Image _imageExit;
        private Text _optionClose;
        private Text _optionExit;

        private int _highlightedIndex = 0;
        private float _lastInput;

        public enum MenuOptions
        {
            None,
            Close,
            Exit,
            ButtonConfig,
            MoveList,
            CharacterSelect,
            StageSelect
        };

        private MenuOptions _highlightedMenuOption;
        private readonly List<MenuOptions> _menuOptions;

        public PauseMenu()
        {
            _menuObject = GameObject.Find("PauseMenu");
            _imageClose = GameObject.Find("PanelClose").GetComponent<Image>();
            _imageExit = GameObject.Find("PanelExit").GetComponent<Image>();
            
            _menuOptions = new List<MenuOptions>()
            {
                MenuOptions.Close,
                MenuOptions.Exit
            };
        }

        public MenuOptions Update(InputClass inputClass)
        {
            if (_lastInput != inputClass.DPadY)
            {
                if (inputClass.DPadY < 0)
                {
                    if (_highlightedIndex < _menuOptions.Count - 1) 
                        _highlightedIndex++;
                    else
                    {
                        _highlightedIndex = 0;
                    }
                }
                else if (inputClass.DPadY > 0)
                {
                    if (_highlightedIndex > 0)
                        _highlightedIndex--;
                    else
                    {
                        _highlightedIndex = _menuOptions.Count - 1;
                    }
                }
            }

            var select = new Color(.03f,.3f,.8f,.8f);
            var unSelect = new Color(1,1,1,.8f);
            _imageClose.color = unSelect;
            _imageExit.color = unSelect;
            //Debug.Log(_menuOptionsList[_highlightedIndex]);
            switch (_menuOptions[_highlightedIndex])
            {
                case MenuOptions.Close:
                    _imageClose.color = select;
                    break;
                case MenuOptions.ButtonConfig:
                    //_imageClose.color = select;
                    break;
                case MenuOptions.MoveList:
                    //_imageClose.color = select;
                    break;
                case MenuOptions.CharacterSelect:
                    //_imageClose.color = select;
                    break;
                case MenuOptions.StageSelect:
                   // _imageClose.color = select;
                    break;
                case MenuOptions.Exit:
                    _imageExit.color = select;
                    break;
                default:
                    _imageClose.color = select;
                    break;
            }

            _lastInput = inputClass.DPadY;
            return _menuOptions[_highlightedIndex];
        }

        public void Enable()
        {
            _menuObject.SetActive(true);
        }
        
        public void Disable()
        {
            _menuObject.SetActive(false);
        }
    }
}