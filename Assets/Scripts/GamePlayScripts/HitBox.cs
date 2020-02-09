using UnityEngine;

namespace GamePlayScripts
{
    public class HitBox
    {
        public BoxCollider Collider;
        public bool LocalHitBox;
        public bool Active;
        public bool HitOccured;

        public HitBox(BoxCollider collider, bool localHitBox)
        {
            Collider = collider;
            Active = true;
            LocalHitBox = localHitBox;
            HitOccured = false;
        }

        public bool Intersects(HurtBox hurtBox)
        {
            if (!Active) return false;

            return Collider.bounds.Intersects(hurtBox.GetHurtBoxBounds());
        }
    

    }
}
