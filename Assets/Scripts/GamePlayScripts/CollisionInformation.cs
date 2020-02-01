using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInformation
{
    public HurtBox.HurtZone Zone;
    public int Damage;
    public int HitStunAmount;
    public int BlockStunAmount;
    //public enum HitStun type {normal, knockdown, etc}
    public bool Collided;
}
