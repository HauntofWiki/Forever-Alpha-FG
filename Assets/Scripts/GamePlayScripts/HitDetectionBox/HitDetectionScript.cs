using System;
using UnityEngine;

namespace GamePlayScripts.HitDetectionBox
{
    public class HitDetectionScript : MonoBehaviour
    {
        public CollisionBoxProperties properties;
        public bool collisionDetected = false;
        public GameObject gameManager;
        public GameManagerScript gameManagerScript;
        private void Start()
        {
            //gameManager = GameObject.Find("GamePlayManager");
            //gameManagerScript = gameManager.GetComponent<GameManagerScript>();

        }
        
        public void OnTriggerEnter(Collider collider)
        {
            //Debug.Log("OnTriggerEnterEvent: " + collider.GetComponent<Collider>().name);
            //gameManagerScript.ReportTriggerEnter(transform.name, collider.name);

        }
    }
}
