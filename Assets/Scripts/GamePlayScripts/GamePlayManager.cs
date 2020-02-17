using System.Collections.Generic;
using GamePlayScripts.CharacterMoves;
using JetBrains.Annotations;
using MenuScripts.GamePlay;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace GamePlayScripts
{
    public class GamePlayManager : MonoBehaviour
    {
        //Define Players and stats
        public GameObject player1Object;
        public GameObject player2Object;
        public Character player1Character;
        public Character player2Character;
        public InputManager player1InputManager;
        public InputManager player2InputManager;
        public InputManager pauseOwner;
        public UIManager uiManager;
        public PauseMenu pauseMenu;
        public bool pauseAble;
        public bool longPause;

        //Define other values
        public int gameTime;
        public int frameCount;
        public int player1WinCount = 0;
        public int player2WinCount = 0;
        public int roundCount = 0;


        //Define Game-States
        public enum GameStates
        {
            PreRound,
            RoundActive,
            PostRound,
            PauseMenu,
            PostMatch
        }

        public enum GameMode
        {
            None,
            Arcade,
            Versus,
            Training,
            Network,
            Debug
        }

        public GameStates gameState;
        public GameMode gameMode;

        // Start is called before the first frame update
        void Start()
        {
            //Prefabs/Characters/Player will be replaced by the actual models sent over from character select
            //The player GameObjects should be named Player1 and Player2
            var characterPrefab = Resources.Load("Prefabs/Characters/Player");
            player1Object = (GameObject) Instantiate(characterPrefab);
            player1Object.name = "Player1";
            player1Object.transform.position = new Vector3(-3, 0, 0);
            player1InputManager = new InputManager(0, "Keyboard");//(1, Input.GetJoystickNames()[1]); //still hardcoded
            player1Character = new Character(player1Object, player1InputManager);

            //Set Player 1 Collision Detection Boxes
            GameObject.Find("UpperBodyHurtBox").name = "P1UpperBodyHurtBox";
            GameObject.Find("LowerBodyHurtBox").name = "P1LowerBodyHurtBox";
            GameObject.Find("HitBox").name = "P1HitBox";
            GameObject.Find("PushBox").name = "P1PushBox";

            //Set Player 2 Objects
            characterPrefab = Resources.Load("Prefabs/Characters/Player");
            player2Object = (GameObject) GameObject.Instantiate(characterPrefab);
            player2Object.name = "Player2";
            player2Object.transform.position = new Vector3(3, 0, 0);
            player2InputManager = new InputManager(0, "none");//(0, Input.GetJoystickNames()[0]); //still hardcoded
            player2Character = new Character(player2Object, player2InputManager);

            //Set Player 2 Collision Detection Boxes
            GameObject.Find("UpperBodyHurtBox").name = "P2UpperBodyHurtBox";
            GameObject.Find("LowerBodyHurtBox").name = "P2LowerBodyHurtBox";
            GameObject.Find("HitBox").name = "P2HitBox";
            GameObject.Find("PushBox").name = "P2PushBox";

            player1Character.PostLoadSetup(player2Object, player2Character);
            player2Character.PostLoadSetup(player1Object, player1Character);

            uiManager = new UIManager();
            pauseMenu = new PauseMenu();
            //Hide PauseMenu from UI
            pauseMenu.Disable();

            frameCount = 0;
            gameTime = Constants.MaxGameClock;

            gameState = GameStates.PreRound;
            gameMode = GameMode.Debug;
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(player1InputManager.CurrentInput.DPadX + ", " + player1InputManager.CurrentInput.DPadNumPad);
            if (gameState == GameStates.PreRound)
            {
                if (gameMode == GameMode.Debug)
                    gameState = GameStates.RoundActive;
                
                //Character Intros, Announcer and UI stuff
                uiManager.Reset();
                uiManager.Update(gameState, frameCount);
                roundCount++;

                if (frameCount >= 150)
                {
                    frameCount = 0;
                    uiManager.Reset();
                    gameState = GameStates.RoundActive;
                }

                frameCount++;
            }
            else if (gameState == GameStates.RoundActive)
            {
                //Debug.Log(player1Character.CharManager.CurrentState + ", " + player1Character.InputManager.GetInput(1).DPadX);
                
                
                if (player1Character.InputManager.GetInput(0).StartButtonDown == 1)
                {
                    pauseOwner = player1InputManager;
                    gameState = GameStates.PauseMenu;
                }
                if (player2Character.InputManager.GetInput(0).StartButtonDown == 1)
                {
                    pauseOwner = player2InputManager;
                    gameState = GameStates.PauseMenu;
                }

                //Update characters
                player1Character.Update(player2Character);
                player2Character.Update(player1Character);

                //Check and Handle Collisions
                if (player1Character.DetectCollisions(player2Character.CharManager))
                {
                    player1Character.CharManager.ComboCounter++;
                    player1Character.CharManager.Collided = true;
                    player2Character.CharManager.NewHit = true;

                    player2Character.ApplyCollision(player1Character.CharManager.GetFrameDataManager());
                }

                if (player2Character.DetectCollisions(player1Character.CharManager))
                {
                    player2Character.CharManager.ComboCounter++;
                    player2Character.CharManager.Collided = true;
                    player1Character.CharManager.NewHit = true;

                    player1Character.ApplyCollision(player2Character.CharManager.GetFrameDataManager());
                }

                //Check to see if an Active Combo ended
                if (player1Character.CharManager.ComboActive
                    && (player2Character.CharManager.CurrentState == CharacterManager.CharacterState.Stand
                    || player2Character.CharManager.CurrentState == CharacterManager.CharacterState.Crouch))
                {
                    player1Character.CharManager.ComboActive = false;
                    player1Character.CharManager.ComboCounter = 0;
                }

                if (player2Character.CharManager.ComboActive &&
                    player1Character.CharManager.CurrentState == CharacterManager.CharacterState.Stand)
                {
                    player2Character.CharManager.ComboActive = false;
                    player2Character.CharManager.ComboCounter = 0;
                }

                //Update game time
                if (frameCount % 60 == 0)
                {
                    gameTime--;
                }

                uiManager.Update(gameTime, player1Character.CharManager, player2Character.CharManager);
                frameCount++;

                if (gameTime <= 0)
                {
                    if (player1Character.CharManager.CurrentHealth > player2Character.CharManager.CurrentHealth)
                    {
                        player1WinCount++;
                        frameCount = 0;
                    }
                    else if (player2Character.CharManager.CurrentHealth > player2Character.CharManager.CurrentHealth)
                    {
                        player2WinCount++;
                        frameCount = 0;
                    }
                    else if (player1Character.CharManager.CurrentHealth == player2Character.CharManager.CurrentHealth)
                    {
                        player1WinCount++;
                        player2WinCount++;
                        frameCount = 0;
                    }

                    gameState = GameStates.PostRound;
                }

                if (player1Character.CharManager.CurrentHealth <= 0 && player2Character.CharManager.CurrentHealth <= 0)
                {
                    player1WinCount++;
                    player2WinCount++;
                    frameCount = 0;
                    gameState = GameStates.PostRound;
                }
                else if (player1Character.CharManager.CurrentHealth <= 0)
                {
                    player2WinCount++;
                    frameCount = 0;
                    gameState = GameStates.PostRound;
                }
                else if (player2Character.CharManager.CurrentHealth <= 0)
                {
                    player1WinCount++;
                    frameCount = 0;
                    gameState = GameStates.PostRound;
                }
            }
            else if (gameState == GameStates.PostRound)
            {
                if (gameMode != GameMode.Debug)
                {
                    //Between rounds, Win poses, Announcer and UI stuff
                    uiManager.Update(gameState, frameCount);

                    if (player1WinCount == 2)
                    {
                        gameState = GameStates.PostMatch;
                    }

                    if (player2WinCount == 2)
                    {
                        gameState = GameStates.PostMatch;
                    }

                    if (frameCount >= 150)
                    {
                        //Reset settings
                        //frameCount = 0;
                        Reset();
                        uiManager.Reset();
                        player1Character.Reset();
                        player2Character.Reset();
                        gameState = GameStates.PreRound;
                    }

                    frameCount++;
                }
            }
            else if (gameState == GameStates.PauseMenu)
            {
                var input = pauseOwner.GetInput(1);

                //Show menu
                pauseMenu.Enable();

                //Unpause
                if (input.StartButtonDown == 1 || input.CancelButtonDown)
                {
                    pauseMenu.Disable();
                    gameState = GameStates.RoundActive;
                }

                //Check menu state every frame
                PauseMenu.MenuOptions state = pauseMenu.Update(pauseOwner.CurrentInput);
                //Debug.Log(input.SubmitButtonDown);
                if (input.SubmitButtonDown)
                {
                    switch (state)
                    {
                        case PauseMenu.MenuOptions.Close:
                            pauseMenu.Disable();
                            gameState = GameStates.RoundActive;
                            break;
                        case PauseMenu.MenuOptions.ButtonConfig:
                            //ToDo ButtonConfig menu
                            break;
                        case PauseMenu.MenuOptions.MoveList:
                            //ToDo Show MoveList
                            break;
                        case PauseMenu.MenuOptions.CharacterSelect:
                            //ToDo Go to Character Select
                            break;
                        case PauseMenu.MenuOptions.StageSelect:
                            //ToDo Go to StageSelect
                            break;
                        case PauseMenu.MenuOptions.Exit:
                            Application.Quit();
                            break;
                    }
                }
            }
            else if (gameState == GameStates.PostMatch)
            {
                //Display scores

                //Rematch -> GameState to PreRound

                //Character Select -> Go to Character Select Scene

                //Exit -> Go to Main Menu
            }
        }

        public void Reset()
        {
        }
    }
}