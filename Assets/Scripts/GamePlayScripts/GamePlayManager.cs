using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace GamePlayScripts
{
    public class GamePlayManager : MonoBehaviour
    {
        public UnityEngine.Object prefab;
        public GameObject MainCamera;
        
        //Define Players and stats
        public GameObject Player1Object;
        public UnityEngine.CharacterController player1Controller;
        public Player player1;
        public Character player1Character;
        public InputManager player1InputManager;
        public float player1CurrentHealth;
        public float player1MaxHealth;
        public float player1Meter;
        public GameObject Player2Object;
        public UnityEngine.CharacterController player2Controller;
        public Player player2;
        public Character player2Character;
        public InputManager player2InputManager;
        public float player2CurrentHealth;
        public float player2MaxHealth;
        public float player2Meter;
        public bool player1ComboActive;
        public bool player2ComboActive;
        
        //Collision detection hit boxes
        public List<CollisionBoxProperties> hitBoxes;
    
        //Define other values
        public int gameTime;
        public int frameCounter;
    
        //Define UI elements
        public Image player1HealthBarEmpty;
        public Image player1HealthBarDifferential;
        public Image player1HealthBarLow;
        public Image player1HealthBarMiddle;
        public Image player1HealthBarFull;
        public Image player2HealthBarEmpty;
        public Image player2HealthBarDifferential;
        public Image player2HealthBarLow;
        public Image player2HealthBarMiddle;
        public Image player2HealthBarFull;
        public Image player1MeterBar;
        public Image player2MeterBar;
        public Text gameTimer;
        public Image leftPanel;
        public Image rightPanel;
        
        // Start is called before the first frame update
        void Start()
        {
            InstantiateCharacterGameObjects();
            
            //Set UI OBjects
            player1HealthBarEmpty = GameObject.Find("Player 1 Empty").GetComponent<Image>();
            player1HealthBarDifferential = GameObject.Find("Player 1 Differential").GetComponent<Image>();
            player1HealthBarLow = GameObject.Find("Player 1 Low").GetComponent<Image>();
            player1HealthBarMiddle = GameObject.Find("Player 1 Middle").GetComponent<Image>();
            player1HealthBarFull = GameObject.Find("Player 1 Full").GetComponent<Image>();
            player2HealthBarEmpty = GameObject.Find("Player 2 Empty").GetComponent<Image>();
            player2HealthBarDifferential = GameObject.Find("Player 2 Differential").GetComponent<Image>();
            player2HealthBarLow = GameObject.Find("Player 2 Low").GetComponent<Image>();
            player2HealthBarMiddle = GameObject.Find("Player 2 Middle").GetComponent<Image>();
            player2HealthBarFull = GameObject.Find("Player 2 Full").GetComponent<Image>();
            //player1MeterBar = GameObject.Find("Player 1 Meter").GetComponent<Image>();
            //player2MeterBar = GameObject.Find("Player 2 Meter").GetComponent<Image>();
            gameTimer = GameObject.Find("Text Timer").GetComponent<Text>();
            leftPanel = GameObject.Find("Panel Portrait Left").GetComponent<Image>();
            leftPanel.material = (Material) Resources.Load("Materials/Portrait Camera Player 1");
            rightPanel = GameObject.Find("Panel Portrait Right").GetComponent<Image>();
            rightPanel.material = (Material) Resources.Load("Materials/Portrait Camera Player 1");
            
            //Set initial Health Bar values
            player1HealthBarEmpty.fillAmount = 1f;
            player1HealthBarDifferential.fillAmount = 1f;
            player1HealthBarLow.fillAmount = 0.25f;
            player1HealthBarMiddle.fillAmount = 0.99f;
            player1HealthBarFull.fillAmount = 1f;
            player2HealthBarEmpty.fillAmount = 1f;
            player2HealthBarDifferential.fillAmount = 1f;
            player2HealthBarLow.fillAmount = 0.25f;
            player2HealthBarMiddle.fillAmount = 0.99f;
            player2HealthBarFull.fillAmount = 1f;
            
            //Set Player Objects and Initial values
            //Hardcoded values for now
            player1MaxHealth = 1000;
            player1CurrentHealth = 1000;
            player1Meter = 100;
            player2MaxHealth = 1000;
            player2CurrentHealth = 1000;
            player2Meter = 100;

            player1ComboActive = true;//false;
            player2ComboActive = true;//false;
            
            frameCounter = 0;
            gameTime = 99;

        }

        // Update is called once per frame
        void Update()
        {
            frameCounter++;

            DetectCollisions();
            SetHealthBars();
            SetClock();
            
            //simulating/testing life drain
            if (frameCounter % 60 == 0)
            {
                player1CurrentHealth -= 5;
                player2CurrentHealth -= 5;
            }

            if (frameCounter % 240 == 0)
            {
                player1ComboActive = false;
                player2ComboActive = false;
            }
            if (frameCounter % 480 == 0)
            {
                player1ComboActive = true;
                player2ComboActive = true;
            }
        }

        private void InstantiateCharacterGameObjects()
        {
            //Prefabs/Characters/Player will be replaced by the actual models sent over from character select
            //The player GameObjects should be named Player1 and Player2
            prefab = Resources.Load("Prefabs/Characters/Player");
            Player1Object = (GameObject) GameObject.Instantiate(prefab);
            Player1Object.name = "Player1";
            Player1Object.transform.position = new Vector3(-3,5,0);
            Player1Object.GetComponentInChildren<Camera>().targetTexture =
                (RenderTexture) Resources.Load("Textures/Player 1 Render Texture");

            prefab = Resources.Load("Prefabs/Characters/Player");
            Player2Object = (GameObject) GameObject.Instantiate(prefab);
            Player2Object.name = "Player2";
            Player2Object.transform.position = new Vector3(3,5,0);
//            Player1Object.GetComponent<Camera>().targetTexture =
//                (RenderTexture) Resources.Load("Textures/Player 2 Render Texture");
        }
                private void DetectCollisions()
        {
            
        }
        
        private void SetHealthBars()
        {
            //Player 1 health bars
            if (player1CurrentHealth / player1MaxHealth <= 0.25)
            {
                player1HealthBarFull.fillAmount = 0;
                player1HealthBarMiddle.fillAmount = 0;
                player1HealthBarLow.fillAmount = player1CurrentHealth / player1MaxHealth;
            }
            else if (player1CurrentHealth / player1MaxHealth < 1)
            {
                player1HealthBarFull.fillAmount = 0;
                player1HealthBarMiddle.fillAmount = player1CurrentHealth / player1MaxHealth;
                player1HealthBarLow.fillAmount = 0;
            }
            else if (player1CurrentHealth / player1MaxHealth >= 1)
            {
                player1HealthBarFull.fillAmount = player1CurrentHealth / player1MaxHealth;
                player1HealthBarMiddle.fillAmount = 0;
                player1HealthBarLow.fillAmount = 0;
            }
            
            //Player 2 health bars
            if (player2CurrentHealth / player2MaxHealth <= 0.25)
            {
                player2HealthBarFull.fillAmount = 0;
                player2HealthBarMiddle.fillAmount = 0;
                player2HealthBarLow.fillAmount = player2CurrentHealth / player2MaxHealth;
            }
            else if (player2CurrentHealth / player2MaxHealth < 1)
            {
                player2HealthBarFull.fillAmount = 0;
                player2HealthBarMiddle.fillAmount = player2CurrentHealth / player2MaxHealth;
                player2HealthBarLow.fillAmount = 0;
            }
            else if (player2CurrentHealth / player2MaxHealth >= 1)
            {
                player2HealthBarFull.fillAmount = player2CurrentHealth / player2MaxHealth;
                player2HealthBarMiddle.fillAmount = 0;
                player2HealthBarLow.fillAmount = 0;
            }
            
            //Update differential is a combo is not active
            if (!player2ComboActive)
                player1HealthBarDifferential.fillAmount = player1CurrentHealth / player1MaxHealth;
            
            if (!player1ComboActive)
                player2HealthBarDifferential.fillAmount = player2CurrentHealth / player2MaxHealth;
        }

        private void SetClock()
        {
            //Decrement clock every 60 frames
            if (frameCounter % 60 == 0)
            {
                gameTime--;
                gameTimer.text = gameTime.ToString();
            }
        }

        private void PopulateHitDetectionBoxes()
        {


        }
    
    }
}
