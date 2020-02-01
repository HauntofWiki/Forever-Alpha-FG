using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox
{
    public BoxCollider Collider;
    public bool Active;
    public bool HitOccured;

    public Hitbox(BoxCollider collider)
    {
        Collider = collider;
        Active = false;
        HitOccured = false;
    }

    public bool Intersects(Bounds bounds)
    {
        if (!Active) return false;

        return Collider.bounds.Intersects(bounds);
    }
    

}
