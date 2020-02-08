using UnityEngine;

namespace GamePlayScripts
{
    public class CameraScript : MonoBehaviour
    {
        private GameObject _player1;
        private GameObject _player2;

        // Start is called before the first frame update
        void Start()
        {
            _player1 = GameObject.Find("Player1");
            _player2 = GameObject.Find("Player2");
            
            transform.position = new Vector3(0, Constants.CameraYOffset, Constants.CameraZ);
        }

        // Update is called once per frame
        void Update()
        {
            //Find Players
            if (_player1 == null)
                _player1 = GameObject.Find("Player1");
            if (_player2 == null)
                _player2 = GameObject.Find("Player2");

            var p1 = _player1.transform.position;
            var p2 = _player2.transform.position;
            const float minX = -Constants.MaxCameraStage;
            const float maxX = Constants.MaxCameraStage;
            var moveCamera = new Vector3(0, 0, 0);
            var position = transform.position;
            var potentialX = (((p1.x + p2.x) / 2)) - position.x;
            var potentialY = (((p1.y + p2.y) / 2) + Constants.CameraYOffset) - position.y;
            
            //Detect Max Camera X bounds
            if (position.x + potentialX <= minX)
            {
                //Stop at left Bound
                moveCamera.x = minX - transform.position.x;
            }
            else if (position.x + potentialX >= maxX)
            {
                //Stop at Right Bound
                moveCamera.x = maxX - transform.position.x;
            }
            else
            {
                //Move Camera normally
                moveCamera.x = potentialX;
            }
            
            //No bounding rules for Camera Y yet, may never be any
            moveCamera.y = potentialY;
            
            //Debug.Log(transform.position.x + ", " + moveCamera.x + ", " + potentialX);
            //Move Camera
            transform.position += moveCamera;
        }
    }
}
