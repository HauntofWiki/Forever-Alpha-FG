using System.Collections.Generic;
using GamePlayScripts.CharacterMoves;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace GamePlayScripts
{
    public class CharacterManager
    {
        //Movement
        public float WalkForwardXSpeed;
        public float WalkBackwardXSpeed;
        public float JumpYSpeed;
        //Speed arrays follow the frames for Startup/Active/Recovery from the move class
        public float[] DashForwardXSpeed;
        public float[] AirDashForwardSpeed;
        public float[] DashBackwardXSpeed;
        public float[] AirDashBackwardSpeed;
        public int CharacterOrientation;

        //Air-related properties
        public float PersonalGravity;
        public bool IsAirborne = false;
        public bool IgnoringGravity = false;
        public bool LocalHitBoxActive = false;
        public bool Grounded = false;
        
        //Collision Detection
        private CollisionManager _collisionManager;
        public bool IgnoringPushBox = false;
        //Collided checks if an attack has already collided to not trigger further actions
        public bool Collided = false;
    
        //Vector3 which is used for moving the Character around;
        public Vector3 MoveDirection;

        //Character tracking variables
        public float MaxHealth;
        public float CurrentHealth;
        public int Meter;
        public bool ComboActive = false;
        public int ComboCounter = 0;
        public bool NewHit = true;

        //Frame data and animation
        private FrameDataManager _frameDataManager = new FrameDataManager(0);
        
        //Character State
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
        
        ///<summary>
        ///Sets the CollisionManager
        ///</summary>
        public void SetCollisionManager(CollisionManager collisionManager)
        {
            _collisionManager = collisionManager;
        }

        public void UpdateCollision()
        {
            _collisionManager.Update(_frameDataManager.PushBack);
        }
        public void Add(HurtBox box)
        {
            _collisionManager.HurtBoxes.Add(box);
        }

        public void Remove(HurtBox box)
        {
            _collisionManager.HurtBoxes.Remove(box);
        }
        
        public List<HurtBox> GetHurtBoxes()
        {
            return _collisionManager.HurtBoxes;
        }
        
        public void Add(HitBox box)
        {
            _collisionManager.HitBoxes.Add(box);
        }
        
        public void Remove(HitBox box)
        {
            _collisionManager.HitBoxes.Remove(box);
        }
        
        public List<HitBox> GetHitBoxes()
        {
            return _collisionManager.HitBoxes;
        }

        public void SetFrameDataManager(FrameDataManager frameDataManager)
        {
            _frameDataManager = frameDataManager;
        }

        public FrameDataManager GetFrameDataManager()
        {
            return _frameDataManager;
        }

        public float GetPushBack()
        {
            return _frameDataManager.PushBack;
        }
        
        public void SetPushBack(float pushBack)
        {
            _frameDataManager.PushBack = pushBack;
        }

        public float GetHitStun()
        {
            return _frameDataManager.HitStun;
        }
        
        
        public void SetHitStun(float hitStun)
        {
            _frameDataManager.HitStun = hitStun;
        }
    }
}
