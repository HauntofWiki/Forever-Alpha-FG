using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlayScripts
{
    public class PushBox
    {
        public BoxCollider Collider;
        
        public PushBox(BoxCollider pushBox)
        {
            Collider = pushBox;
        }

        public bool Intersects(PushBox pushBox)
        {
            return Collider.bounds.Intersects(pushBox.Collider.bounds);
        }
    }
}