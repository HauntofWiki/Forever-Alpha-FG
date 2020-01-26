using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using MenuScripts.CharacterSelect;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GameManagerScript : MonoBehaviour
{
    private SideSelect _sideSelect;
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

    public void Start()
    {
        _sideSelect = new SideSelect();
        _gameState = GameStates.MatchActive;//temporarily hardcoded
    }

    public void Update()
    {
        switch (_gameState)
        {
            case GameStates.SideSelect:
                SideSelect();
                break;
            default:
                break;
        }
    }

    public void MainMenu()
    {
        
    }

    public void SideSelect()
    {
        //Debug.Log("sideselect");
        _sideSelect.Update();
    }
    
    public void GamePlay()
    {
        
    }
}
