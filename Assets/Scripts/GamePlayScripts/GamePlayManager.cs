﻿using System.Collections.Generic;
using GamePlayScripts.CharacterMoves;
using JetBrains.Annotations;
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
        public UnityEngine.Object characterPrefab;
        
        //Define Players and stats
        public GameObject player1Object;
        public GameObject player2Object;
        public Character player1Character;
        public Character player2Character;
        public InputManager player1InputManager;
        public InputManager player2InputManager;
        public CollisionInformation Player1CollisionInformation;
        public CollisionInformation Player2CollisionInformation;
        public float player1CurrentHealth;
        public float player2CurrentHealth;
        public float player1MaxHealth;
        public float player2MaxHealth;
        public float player1Meter;
        public float player2Meter;

        public UIManager uiManager;
        
        //Define other values
        public int gameTime;
        public int frameCount;
        
        // Start is called before the first frame update
        void Start()
        {
            //Prefabs/Characters/Player will be replaced by the actual models sent over from character select
            //The player GameObjects should be named Player1 and Player2
            characterPrefab = Resources.Load("Prefabs/Characters/Player");
            player1Object = (GameObject) GameObject.Instantiate(characterPrefab);
            player1Object.name = "Player1";
            player1Object.transform.position = new Vector3(-3, 5, 0);
            player1InputManager = new InputManager(1,Input.GetJoystickNames()[1]);//still hardcoded
            player1Character = new Character(player1Object, player1InputManager);
            
            //Set Player 1 Collision Detection Boxes
            GameObject.Find("UpperBodyHurtBox").name = "P1UpperBodyHurtBox";
            GameObject.Find("LowerBodyHurtBox").name = "P1LowerBodyHurtBox";
            GameObject.Find("HitBox").name = "P1HitBox";
            
            //Set Player 2 Objects
            characterPrefab = Resources.Load("Prefabs/Characters/Player");
            player2Object = (GameObject) GameObject.Instantiate(characterPrefab);
            player2Object.name = "Player2";
            player2Object.transform.position = new Vector3(3, 5, 0);
            player2InputManager = new InputManager(0,Input.GetJoystickNames()[0]);//still hardcoded
            player2Character = new Character(player2Object, player2InputManager);
            
            //Set Player 2 Collision Detection Boxes
            GameObject.Find("UpperBodyHurtBox").name = "P2UpperBodyHurtBox";
            GameObject.Find("LowerBodyHurtBox").name = "P2LowerBodyHurtBox";
            GameObject.Find("HitBox").name = "P2HitBox";

            player1Character.PostLoadSetup(player2Object);
            player2Character.PostLoadSetup(player1Object);
            
            //Set Player Objects and Initial values
            //Hardcoded values for now
            uiManager = new UIManager();
            player1MaxHealth = 1000;
            player1CurrentHealth = 1000;
            player1Meter = 100;
            player2MaxHealth = 1000;
            player2CurrentHealth = 1000;
            player2Meter = 100;
            
            frameCount = 0;
            gameTime = 99;
        }

        // Update is called once per frame
        void Update()
        {
            

            
            player1Character.Update();
            player2Character.Update();

            //Check and Handle Collisions
            if (player1Character.DetectCollisions(player2Character.GetHurtBoxes()))
            {
                player1Character.Properties.ComboCounter++;
                player1Character.Properties.Collided = true;
                player2Character.Properties.NewHit = true;
                
                player2Character.ApplyCollision(player1Character.Properties.FrameDataHandler);
            }

            //Check to see if an Active Combo ended
            if (player1Character.Properties.ComboActive &&
                player2Character.Properties.CurrentState == CharacterProperties.CharacterState.Stand)
            {
                player1Character.Properties.ComboActive = false;
                player1Character.Properties.ComboCounter = 0;
            }
                
            //Update game time
            if (frameCount % 60 == 0)
            {
                gameTime--;
            }
            uiManager.Update(gameTime,player1Character.Properties, player2Character.Properties);
            frameCount++;
            
        }
        
    }
}
