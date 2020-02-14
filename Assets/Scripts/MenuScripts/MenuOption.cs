using MenuScripts.GamePlay;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts
{
    public class MenuOption
    {
        private GameObject _menuOption;
        private Image _menuOptionImage;
        private readonly Text _menuOptionText;
        public PauseMenu.MenuOptions MenuOptionType { get; }

        public MenuOption(PauseMenu.MenuOptions type)
        {
            var prefab = Resources.Load("Prefabs/UI/PanelMenuOption");
            var menuContent = GameObject.Find("MenuContent");
            _menuOption = (GameObject) Object.Instantiate(prefab, menuContent.transform);
            _menuOptionImage = _menuOption.GetComponent<Image>();
            _menuOptionText = _menuOption.GetComponentInChildren<Text>();
            //_menuOptionImage.rectTransform.
            

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

        public void Select()
        {
            
        }

        public void Deselect()
        {
            
        }
    }
}