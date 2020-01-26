using UnityEngine;
using UnityEngine.Serialization;

//using UnityEngine.EventSystems;

/*
 * This class is intermediate between Player 1 and the input system
 * as well as the system wide features/restrictions that apply to all characters.
 */
namespace GamePlayScripts
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.CharacterController player;
        private Character _character;
        private InputManager _inputManager;
        private InputClass _currentInput;
        private GameObject _opponentCharacter;
        private GameObject _gameManager;
        private GamePlayManager _gamePlayerManager;
        private Vector3 _moveDirection = Vector3.zero;
        private int _playerNumber; //Defines Player 1 or Player 2
        private int _characterOrientation; // this defines whether player is on the left or right, should be 1 or -1
        private int _lastCharacterOrientation;
   
        public Animator animator;
        public Animation animation;

        // Start is called before the first frame update
        private void Start()
        {
            //Get GameManager Object
            _gameManager = GameObject.Find("GamePlayManager");
            _gamePlayerManager = _gameManager.GetComponent<GamePlayManager>();

            //Find whether the controller is Player1 or Player2
            _playerNumber = transform.name == "Player2" ? 2 : 1;

            _characterOrientation = 0; //hardcoded for now
            player = GetComponent<UnityEngine.CharacterController>();
            _character = new Character(player);
            animator = GetComponent<Animator>();
            animation = GetComponent<Animation>();

            
            //Find Opposing Character GameObject
            if (_playerNumber == 1)
            {
                _opponentCharacter = GameObject.Find("Player2");
                _inputManager = new InputManager(0, Input.GetJoystickNames()[0]); //Hardcoded for now
            }
            else 
            {
                _opponentCharacter = GameObject.Find("Player1");
                _inputManager = new InputManager(1, Input.GetJoystickNames()[1]); //Hardcoded for now
            }
        }

        // Update is called once per frame
        private void Update()
        {
            _lastCharacterOrientation = _characterOrientation;
        
            if (transform.position.x > _opponentCharacter.transform.position.x && _character.CanSwitchOrientation())
            {
                var flipModel = new Vector3(-1,1,1);
                _characterOrientation = -1;
                transform.localScale = Vector3.Lerp(transform.localScale,flipModel, 2.0f);
            }
            else if (transform.position.x < _opponentCharacter.transform.position.x &&_character.CanSwitchOrientation())
            {
                var flipModel = new Vector3(1,1,1);
                _characterOrientation = 1;
                transform.localScale = Vector3.Lerp(transform.localScale,flipModel, 2.0f);
            }
        
            _currentInput = _inputManager.Update(_characterOrientation);
            //Debug.Log(_currentInput.DPadNumPad + ", " + _currentInput.LightAttackButtonDown + ", " + _currentInput.MediumAttackButtonDown + ", " + _currentInput.HeavyAttackButtonDown + ", " + _currentInput.SpecialAttackButtonDown + ", ");
            _character.Update(_currentInput);
        
            //Apply movement to character
            _character.ApplyMovement(_moveDirection,_characterOrientation, _lastCharacterOrientation);
        
        }
    }
}