using System;
using UnityEngine;

namespace GamePlayScripts.HitDetectionBox
{
    public class HitDetectionScript : MonoBehaviour
    {
        public string invaderName;
        public CollisionBoxProperties properties;
        public bool collisionDetected = false;
        private void Start()
        {
            properties = new CollisionBoxProperties();
            
            //Assign type of this CollisionBox
            if (name.Equals("LowerBodyHurtBox"))
                properties.CollisionBoxType = CollisionBoxProperties.CollisionBoxTypes.LowerBodyHurtBox;
            else if (name.Equals("UpperBodyHurtBox"))
                properties.CollisionBoxType = CollisionBoxProperties.CollisionBoxTypes.UpperBodyHurtBox;
            else if (name.Equals("HeadHurtBox"))
                properties.CollisionBoxType = CollisionBoxProperties.CollisionBoxTypes.HeadHurtBox;
            else if (name.Equals("ThrowBox"))
                properties.CollisionBoxType = CollisionBoxProperties.CollisionBoxTypes.ThrowBox;
            else if (name.Equals("PushBox"))
                properties.CollisionBoxType = CollisionBoxProperties.CollisionBoxTypes.PushBox;
            else
                properties.CollisionBoxType = CollisionBoxProperties.CollisionBoxTypes.None;
        }
        
        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnterEvent: " + other.GetComponent<Collider>().name);
            
        }
    }
}
