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
        public CharacterController player1Controller;
        public CharacterControllerScript player1ControllerScript;
        public Animator player1Animator;
        public Player player1;
        public Character player1Character;
        public HitStunManager player1HitStunManager;
        public InputManager player1InputManager;
        public float player1CurrentHealth;
        public float player1MaxHealth;
        public float player1Meter;
        public float player1ComboCounter;
        
        //Player2
        public GameObject player2Object;
        public CharacterController player2Controller;
        public CharacterControllerScript player2ControllerScript;
        public Animator player2Animator;
        public Player player2;
        public Character player2Character;
        public HitStunManager player2HitStunManager;
        public InputManager player2InputManager;
        public float player2CurrentHealth;
        public float player2MaxHealth;
        public float player2Meter;
        public bool player1ComboActive;
        public bool player2ComboActive;
        public float player2ComboCounter;
        
        //Collision detection hit boxes
        //HurtBoxes are vulnerable collision boxes
        public List<BoxCollider> player1HurtBoxes;
        public List<BoxCollider> player2HurtBoxes;
        
        //Player 1
        public BoxCollider player1UpperBodyHurtBox;
        public BoxCollider player1LowerBodyHurtBox;
        
        //Player 2
        public BoxCollider player2UpperBodyHurtBox;
        public BoxCollider player2LowerBodyHurtBox;
        
        //Hitboxes are Offensive collision boxes
        public List<BoxCollider> player1HitBoxes;
        public List<BoxCollider> player2HitBoxes;
        public BoxCollider player1HitBox;
        public BoxCollider player2HitBox;
        
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
            //Prefabs/Characters/Player will be replaced by the actual models sent over from character select
            //The player GameObjects should be named Player1 and Player2
            characterPrefab = Resources.Load("Prefabs/Characters/Player");
            player1Object = (GameObject) GameObject.Instantiate(characterPrefab);
            player1Object.name = "Player1";
            player1Object.transform.position = new Vector3(-3, 5, 0);
            player1Controller = player1Object.GetComponent<UnityEngine.CharacterController>();
            player1ControllerScript = player1Controller.GetComponent<CharacterControllerScript>();
            player1Animator = player1Object.GetComponent<Animator>();
            player1Character = new Character(player1Controller);
            player1InputManager = new InputManager(1,Input.GetJoystickNames()[1]);//still hardcoded
            player1Object.GetComponentInChildren<Camera>().targetTexture =
                (RenderTexture) Resources.Load("Textures/Player 1 Render Texture");
            player1HitStunManager = new HitStunManager(player1Animator, ref player1Character.Properties);
            player1ComboCounter = 0;
            
            //Set Player 1 Collision Detection Boxes
            GameObject.Find("UpperBodyHurtBox").name = "P1UpperBodyHurtBox";
            GameObject.Find("LowerBodyHurtBox").name = "P1LowerBodyHurtBox";
            GameObject.Find("HitBox").name = "P1HitBox";
            player1UpperBodyHurtBox = GameObject.Find("P1UpperBodyHurtBox").GetComponent<BoxCollider>();
            player1LowerBodyHurtBox = GameObject.Find("P1LowerBodyHurtBox").GetComponent<BoxCollider>();

            player1HitBox = GameObject.Find("P1HitBox").GetComponent<BoxCollider>();
            
            
            //Set Player 2 Objects
            characterPrefab = Resources.Load("Prefabs/Characters/Player");
            player2Object = (GameObject) GameObject.Instantiate(characterPrefab);
            player2Object.name = "Player2";
            player2Object.transform.position = new Vector3(3,5,0);
            player2Controller = player2Object.GetComponent<UnityEngine.CharacterController>();
            player2ControllerScript = player2Controller.GetComponent<CharacterControllerScript>();
            player2Animator = player2Object.GetComponent<Animator>();
            player2Character = new Character(player2Controller);
            player2InputManager = new InputManager(0,Input.GetJoystickNames()[0]);//still hardcoded
            player2HitStunManager = new HitStunManager(player2Animator, ref player2Character.Properties);
            player2ComboCounter = 0;
            
            //Set Player 2 Collision Detection Boxes
            GameObject.Find("UpperBodyHurtBox").name = "P2UpperBodyHurtBox";
            GameObject.Find("LowerBodyHurtBox").name = "P2LowerBodyHurtBox";
            GameObject.Find("HitBox").name = "P2HitBox";
            player2UpperBodyHurtBox = GameObject.Find("P2UpperBodyHurtBox").GetComponent<BoxCollider>();
            player2LowerBodyHurtBox = GameObject.Find("P2LowerBodyHurtBox").GetComponent<BoxCollider>();
            player2HitBox = GameObject.Find("P2HitBox").GetComponent<BoxCollider>();
            
            player1ControllerScript.InstantiateCharacterController(player2Object, ref player1Character);
            player1ControllerScript.CustomUpdate();
            player2ControllerScript.InstantiateCharacterController(player1Object, ref player2Character);
            player2ControllerScript.CustomUpdate();
            


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
            Debug.Log(player1ComboCounter);
            DetectCollisions();
            SetHealthBars();
            SetClock();
            //Debug.Log(player2Character.Properties.CurrentState);
            if (player2Character.Properties.CurrentState == CharacterProperties.CharacterState.HitStun)
            {
                player2HitStunManager.Update();
            }
                
            
            player1Character.Update(player1InputManager.Update(player1Character.Properties.CharacterOrientation));
            player2Character.Update(player2InputManager.Update(player2Character.Properties.CharacterOrientation));
            player1ControllerScript.CustomUpdate();
            player2ControllerScript.CustomUpdate();

            if (player2Character.Properties.CurrentState == CharacterProperties.CharacterState.Stand)
                player1ComboCounter = 0;
            
            
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
        private void DetectCollisions()
        {
            if (player1HitBox.bounds.Intersects(player2UpperBodyHurtBox.bounds))
            {
                player2Character.Properties.CurrentState = CharacterProperties.CharacterState.HitStun;
                player2Character.Properties.HitStunDuration = 10;
                player1ComboActive = true;
                if (player1ComboActive)
                    player1ComboCounter++;
                
            }
            
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
