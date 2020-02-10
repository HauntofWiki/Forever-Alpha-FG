using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace GamePlayScripts
{
    public class PushBox
    {
        public BoxCollider Collider;

        public PushBox(BoxCollider boxCollider)
        {
            Collider = boxCollider;
        }
    }
}