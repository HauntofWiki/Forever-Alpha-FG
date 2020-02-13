using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts.GamePlay
{
    public class PauseMenu
    {
        private readonly GameObject _pauseMenuObject;
        
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
        private readonly List<MenuOption> _menuOptions;

        public PauseMenu()
        {
            _pauseMenuObject = GameObject.Find("PauseMenu");

            var prefab = Resources.Load("Prefabs/UI/PanelMenuOption");
            _menuOptions = new List<MenuOption>()
            {
                new MenuOption(_pauseMenuObject, MenuOptions.Close),
                new MenuOption(_pauseMenuObject, MenuOptions.ButtonConfig),
                new MenuOption(_pauseMenuObject, MenuOptions.MoveList),
                new MenuOption(_pauseMenuObject, MenuOptions.CharacterSelect),
                new MenuOption(_pauseMenuObject, MenuOptions.StageSelect),
                new MenuOption(_pauseMenuObject, MenuOptions.Exit)
            };
        }

        public MenuOptions Update(InputClass inputClass)
        {
            //Debug.Log(_highlightedIndex);
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
            
            var select = new Color(3,137,209,100);
            var unSelect = new Color(255,255,255,100);
            //_optionClose.color = unSelect;
            //Debug.Log(_menuOptionsList[_highlightedIndex]);
            switch (_menuOptions[_highlightedIndex].MenuOptionType)
            {
                case MenuOptions.Close:
                    //_optionClose.color = select;
                    break;
                case MenuOptions.ButtonConfig:
                    //_optionButtonConfig.color = select;
                    break;
                case MenuOptions.MoveList:
                    //_optionMoveList.color = select;
                    break;
                case MenuOptions.CharacterSelect:
                    //_optionCharacterSelect.color = select;
                    break;
                case MenuOptions.StageSelect:
                   // _optionStageSelect.color = select;
                    break;
                case MenuOptions.Exit:
                    //_optionExit.color = select;
                    break;
                default:
                    //_optionClose.color = select;
                    break;
            }

            _lastInput = inputClass.DPadY;
            return _menuOptions[_highlightedIndex].MenuOptionType;
        }

        public void Enable()
        {
            _pauseMenuObject.SetActive(true);
        }
        
        public void Disable()
        {
            _pauseMenuObject.SetActive(false);
        }
    }
}