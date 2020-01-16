using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * This class is intermediate between Player 1 and the input system
 * as well as the system wide features/restrictions that apply to all characters.
 */
public class CharacterController : MonoBehaviour
{
    [SerializeField] private UnityEngine.CharacterController player;
    private Character _character;
    private InputManager _inputManager;
    private InputClass _currentInput;
    private GameObject _opponentCharacter;
    private GameObject _gameCamera;
    private Vector3 _moveDirection = Vector3.zero;
    private int _characterNumber; //Defines Player 1 or Player 2
    private int _characterOrientation; // this defines whether player is on the left or right, should be 1 or -1
    private int _lastChacterOrientation;
    private float _currentCharacterDistance;
    private float _maxCharacterDistance = 7.0f;

   
    public Animator animator;
    public Animation _animation;


    // Start is called before the first frame update
    private void Start()
    {
        _characterNumber = 1; //hardcoded for now
        _characterOrientation = 1; //hardcoded for now
        player = GetComponent<UnityEngine.CharacterController>();
        _character = new Character(player);
        _inputManager = new InputManager(_characterNumber);
        animator = GetComponent<Animator>();
        _animation = GetComponent<Animation>();
        
        //Find Camera
        _gameCamera = GameObject.Find("Main Camera");
        //Find Opposing Character GameObject
        if (_characterNumber == 1)
            _opponentCharacter = GameObject.Find("Player2");
    }

    // Update is called once per frame
    private void Update()
    {
        _lastChacterOrientation = _characterOrientation;
        
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

        _character.Update(_currentInput);
        
        //Apply movement to character
        _character.ApplyMovement(_moveDirection,_characterOrientation, _lastChacterOrientation);
        
    }
}