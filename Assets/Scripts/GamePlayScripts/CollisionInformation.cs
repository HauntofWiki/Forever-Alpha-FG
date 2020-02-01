using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionInformation
{
    public HurtBox.HurtZone Zone;
    public int Attacker;
    public float Receiver;
    public float Damage;
    public float Pushback;
    public int HitStunAmount;
    public int BlockStunAmount;

    public enum HitTypes
    {
        None,
        Low,
        Mid,
        High,
        Throw,
    }

    public enum HitStunTypes
    {
        None,
        Normal,
        KnockDown,
        Float
    }

    public HitTypes HitType;
    public HitStunTypes HitStunType;
    public bool Collided;
}
