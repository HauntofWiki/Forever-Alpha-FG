using UnityEngine;

namespace GamePlayScripts
{
    public class CollisionBoxProperties
    {
        public GameObject HitBoxObject;
        public Player OwnerPlayer;
        public bool isActive;

        public enum CollisionBoxTypes
        {
            None,
            LowerBodyHurtBox,
            UpperBodyHurtBox,
            HeadHurtBox,
            ThrowBox,
            PushBox,
            Attack,
        }

        public CollisionBoxTypes CollisionBoxType;
    }
}