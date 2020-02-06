﻿using GamePlayScripts.CharacterMoves;
using UnityEngine;

namespace GamePlayScripts
{
    public class CharacterProperties
    {
        public float WalkForwardXSpeed;
        public float WalkBackwardXSpeed;
        public float JumpYSpeed;
        public float PersonalGravity;
        //Speed arrays follow the frames for Startup/Active/Recovery from the move class
        public float[] DashForwardXSpeed;
        public float[] AirDashForwardSpeed;
        public float[] DashBackwardXSpeed;
        public float[] AirDashBackwardSpeed;

        public bool IsAirborne = false;
        public bool IsIgnoringGravity = false;
        public bool LocalHitBoxActive = false;
        public bool IsGrounded = false;
        
        //Collided checks if an attack has already collided to not trigger further actions
        public bool Collided = false;
    
        //Define frame tracking variables.
        public int AttackFrameCounter;
        public int CharacterOrientation;

        //Vector3 which is used for moving the Character around;
        public Vector3 MoveDirection;

        //Character tracking variables
        public float MaxHealth;
        public float CurrentHealth;
        public int Meter;
        public bool ComboActive = false;
        public int ComboCounter = 0;
        public bool NewHit = true;

        public FrameDataHandler FrameDataHandler = new FrameDataHandler(0);
        
        //Define Character State
        public CharacterState CurrentState;
        public CharacterState LastState;
        public enum CharacterState
        {
            Stand,
            Crouch,
            Jump,
            JumpForward,
            JumpBackward,
            Landing,
            LandingJumpForward,
            LandingJumpBackward,
            LandingAirDashForward,
            Dash,
            BackDash,
            CancellableAnimation,
            AirDash,
            AirBackDash,
            DoubleJump,
            BlockStun,
            KnockDown,
            HitStun,
            StandingHitStun,
            CrouchingHitStun,
            FloatingHitstun,
            Cinematic,
            None
        }

        public enum AttackStates
        {
            None,
            LightAttack,
            CrouchLightAttack,
            JumpLightAttack,
            MediumAttack,
            CrouchMediumAttack,
            JumpMediumAttack,
            HeavyAttack,
            CrouchHeavyAttack,
            JumpHeavyAttack,
            SpecialAttack,
            CrouchSpecialAttack,
            JumpSpecialAttack,
        }

        public AttackStates AttackState;

        public enum CancellableStates
        {
            None,
            EmptyCancellable,
            NormalCancellable,
            SpecialCancellable,
            SuperCancellable,
            JumpCancellable,
            AirJumpCancellable,
            DashCancellable,
            AirDashCancellable
        }

        public CancellableStates CancellableState;
    }
}
