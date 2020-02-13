using MenuScripts.GamePlay;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts
{
    public class MenuOption
    {
        private readonly GameObject _menuOptionObject;
        private Image _menuOptionImage { get; set; }
        private readonly Text _menuOptionText;
        public PauseMenu.MenuOptions MenuOptionType { get; }

        public MenuOption(GameObject menuOptionObject, PauseMenu.MenuOptions type)
        {
            _menuOptionObject = menuOptionObject;
            _menuOptionImage = _menuOptionObject.GetComponent<Image>();
            _menuOptionText = _menuOptionObject.GetComponentInChildren<Text>();

            switch (type)
            {
                case PauseMenu.MenuOptions.Close:
                    MenuOptionType = PauseMenu.MenuOptions.Close;
                    _menuOptionText.text = "Close";
                    break;
                case PauseMenu.MenuOptions.ButtonConfig:
                    MenuOptionType = PauseMenu.MenuOptions.ButtonConfig;
                    _menuOptionText.text = "Button Configuration";
                    break;
                case PauseMenu.MenuOptions.MoveList:
                    MenuOptionType = PauseMenu.MenuOptions.MoveList;
                    _menuOptionText.text = "Move List";
                    break;
                case PauseMenu.MenuOptions.CharacterSelect:
                    MenuOptionType = PauseMenu.MenuOptions.CharacterSelect;
                    _menuOptionText.text = "Character Select";
                    break;
                case PauseMenu.MenuOptions.StageSelect:
                    MenuOptionType = PauseMenu.MenuOptions.StageSelect;
                    _menuOptionText.text = "Stage Select";
                    break;
                case PauseMenu.MenuOptions.Exit:
                    MenuOptionType = PauseMenu.MenuOptions.Exit;
                    _menuOptionText.text = "Exit";
                    break;
                default:
                    MenuOptionType = PauseMenu.MenuOptions.None;
                    _menuOptionText.text = "None";
                    break;
            }
        }
    }
}