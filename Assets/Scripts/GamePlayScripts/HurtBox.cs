﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox
{
    public GameObject HurtBoxObject;
    public BoxCollider Collider;
    
    public enum HurtZone
    {
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

    public Bounds GetHurtBoxBounds()
    {
        return Collider.bounds;
    }
}
