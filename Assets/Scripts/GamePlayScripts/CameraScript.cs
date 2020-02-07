using UnityEngine;

namespace GamePlayScripts
{
    public class CameraScript : MonoBehaviour
    {
        private GameObject _player1;
        private GameObject _player2;
        private Camera _camera;
        private float _cameraHeightOffset = 0.8f;
        private float _cameraDefaultZ = -9;
        private float _heightOfCamera;
        private float _centerXBetweenCharacters;
        private Vector3 _moveCameraPosition;
        private float _stageMaxX = 4;
        private float _stageMinX = -4;
        private float _stageMaxY;
        private float _stageMinY;

        // Start is called before the first frame update
        void Start()
        {
            _player1 = GameObject.Find("Player1");
            _player2 = GameObject.Find("Player2");
        }

        // Update is called once per frame
        void Update()
        {
            if (_player1 == null)
                _player1 = GameObject.Find("Player1");
            if (_player2 == null)
                _player2 = GameObject.Find("Player2");
            
            _heightOfCamera = ((_player1.transform.position.y + _player2.transform.position.y) / 2) + _cameraHeightOffset;
            _centerXBetweenCharacters = ((_player1.transform.position.x + _player2.transform.position.x) / 2);
            _moveCameraPosition = new Vector3(_centerXBetweenCharacters,_heightOfCamera,_cameraDefaultZ);

//            if (_moveCameraPosition.x <= _stageMinX)
//            {
//                _moveCameraPosition.x = _stageMinX - transform.position.x;
//            }
//            else if (_moveCameraPosition.x >= _stageMaxX)
//            {
//                _moveCameraPosition.x = _stageMaxX - transform.position.x;
//            }
            Debug.Log(_moveCameraPosition.x);
            transform.position = _moveCameraPosition;
        }
    }
}
