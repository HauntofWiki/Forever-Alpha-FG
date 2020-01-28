using GamePlayScripts.CharacterMoves;
using UnityEngine;
using UnityEngine.Serialization;

//using UnityEngine.EventSystems;

namespace GamePlayScripts
{
    public class CharacterControllerScript : MonoBehaviour
    {
        [SerializeField] private UnityEngine.CharacterController player;
        private Character _character;
        private Vector3 _moveDirection;
        private GameObject _opponentCharacter;

        public void InstantiateCharacterController(GameObject opponent, ref Character character)
        {
            _opponentCharacter = opponent;
            _character = character;
        }

        //CustomUpdate is to Update from the GamePlayManager and not from Unity automatically
        public void CustomUpdate()
        {
            if (transform.position.x > _opponentCharacter.transform.position.x && _character.CanSwitchOrientation())
            {
                var flipModel = new Vector3(-1,1,1);
                _character.Properties.CharacterOrientation = -1;
                transform.localScale = Vector3.Lerp(transform.localScale,flipModel, 2.0f);
            }
            else if (transform.position.x < _opponentCharacter.transform.position.x &&_character.CanSwitchOrientation())
            {
                var flipModel = new Vector3(1,1,1);
                _character.Properties.CharacterOrientation = 1;
                transform.localScale = Vector3.Lerp(transform.localScale,flipModel, 2.0f);
            }
        }
    }
}