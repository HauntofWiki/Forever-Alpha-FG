using UnityEngine;

namespace GamePlayScripts.CharacterMoves
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

        public int BackDashDuration;
        public int AirDashDuration;

        public bool IsAirborne;
        public bool IsIgnoringGravity;
    
        //Define frame tracking variables.
        public int InputFrameCounter;
        public int JumpFrameCounter;
        public int DashFrameCounter;
        public int AttackFrameCounter;
        public int CharacterOrientation;
        
        //Vector3 which is used for moving the player Around;
        public Vector3 MoveDirection;
        public UnityEngine.CharacterController CharacterController;
    
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
            Dash,
            BackDash,
            CancellableAnimation,
            AirDash,
            AirBackDash,
            DoubleJump,
            BlockStun,
            HitStun,
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
