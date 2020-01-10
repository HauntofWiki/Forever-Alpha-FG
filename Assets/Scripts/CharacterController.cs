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
    private Vector3 _moveDirection = Vector3.zero;
    private int _characterNumber; //Defines Player 1 or Player 2
    private int _characterOrientation; // this defines whether player is on the left or right, should be 1 or -1

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

    }

    // Update is called once per frame
    private void Update()
    {
        _currentInput = _inputManager.Update(_characterOrientation);
        //Debug.Log(_currentInput.DPadX);
        _character.CharacterIdle(_currentInput);
        _character.WalkForward(_currentInput);
        _character.WalkBackward(_currentInput);
        _character.JumpForward(_currentInput);
        _character.JumpBackward(_currentInput);
        _character.JumpNeutral(_currentInput);
        _character.DashForward(_currentInput);
        _character.SpecialForward(_currentInput);
        _character.ApplyMovement(_moveDirection);
    }
}