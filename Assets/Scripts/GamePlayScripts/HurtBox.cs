using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox
{
    public GameObject HurtBoxObject;
    public GameObject DebugHurtBox;
    public BoxCollider Collider;
    public Bounds Bounds;
    
    public enum HurtZone
    {
        General,
        UpperBody,
        LowerBody,
        Head,
        Extended
    }

    public HurtZone Zone;

    public HurtBox(BoxCollider hurtBox, HurtZone zone)
    {
        Collider = hurtBox.GetComponent<BoxCollider>();
        Zone = zone;

    }
    
    public HurtBox(Bounds bounds, HurtZone zone)
    {
        Bounds = bounds;
        Zone = zone;
    }

    public Bounds GetHurtBoxBounds()
    {
        return Collider.bounds;
    }


}
