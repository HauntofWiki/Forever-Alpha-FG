using System.Collections.Generic;
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
            player1Object.transform.position = new Vector3(-3, 0, 0);
            player1InputManager = new InputManager(1,Input.GetJoystickNames()[1]);//still hardcoded
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
            player2InputManager = new InputManager(0,Input.GetJoystickNames()[0]);//still hardcoded
            player2Character = new Character(player2Object, player2InputManager);
            
            //Set Player 2 Collision Detection Boxes
            GameObject.Find("UpperBodyHurtBox").name = "P2UpperBodyHurtBox";
            GameObject.Find("LowerBodyHurtBox").name = "P2LowerBodyHurtBox";
            GameObject.Find("HitBox").name = "P2HitBox";
            GameObject.Find("PushBox").name = "P2PushBox";

            player1Character.PostLoadSetup(player2Object, player2Character);
            player2Character.PostLoadSetup(player1Object, player1Character);
            
            uiManager = new UIManager();
            
            frameCount = 0;
            gameTime = Constants.MaxGameClock;
        }

        // Update is called once per frame
        void Update()
        { 
          //Debug.Log(player2Object.transform.position);  
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
            if (player1Character.CharManager.ComboActive &&
                player2Character.CharManager.CurrentState == CharacterManager.CharacterState.Stand)
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
            uiManager.Update(gameTime,player1Character.CharManager, player2Character.CharManager);
            frameCount++;
            
        }
        
    }
}
