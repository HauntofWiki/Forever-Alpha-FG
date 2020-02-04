using GamePlayScripts.CharacterMoves;
using UnityEngine;
using UnityEngine.Serialization;

//using UnityEngine.EventSystems;

namespace GamePlayScripts
{
    public class CharacterControllerScript : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        private CharacterProperties _properties;
        private Vector3 _moveDirection;
        private GameObject _opponentCharacter;

        public void InstantiateCharacterController(GameObject opponent, ref CharacterProperties properties)
        {
            controller = GetComponent<CharacterController>();
            controller.detectCollisions = false;
            _opponentCharacter = opponent;
            _properties = properties;
        }

        //Determine which side the player is on
        public void DeterminePlayerSide()
        {
            
            if (transform.position.x > _opponentCharacter.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(-1,1,1);
                _properties.CharacterOrientation = -1;
                transform.localScale = Vector3.Lerp(transform.localScale,flipModel, 2.0f);
            }
            else if (transform.position.x < _opponentCharacter.transform.position.x && CanSwitchOrientation())
            {
                var flipModel = new Vector3(1,1,1);
                _properties.CharacterOrientation = 1;
                transform.localScale = Vector3.Lerp(transform.localScale,flipModel, 2.0f);
            }
        }

        private bool CanSwitchOrientation()
        {
            //May want to add statuses or handle more elegantly
            return (_properties.CurrentState != _properties.LastState && _properties.CurrentState != CharacterProperties.CharacterState.JumpForward );
        }
    }
}