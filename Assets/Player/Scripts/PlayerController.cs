using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState // 플레이어의 현재 행동(혹은 상태)
{
    IdleAndMove,
    Dash,
    Airborne,
    ShortGroggy,
    LongGroggy,
    GuardIdle,
    GuardHit,
    Penetrate,
    GuardMoveForward,
    GuardMoveBackward,
    BasicHorizonSlash1,
    BasicHorizonSlash2,
    BasicVerticalSlash,
    Thrust,
    CounterPosture,
    RetreatSlash,
    PowerfulThrust,
}

public class PlayerController : MonoBehaviour
{
    // 인스펙터에서 할당이 필요한 변수
    public GroundCheck GroundCheck; // 플레이어의 발이 땅에 붙어있는지 체크하는 클래스
    public Collider WeaponCollider;

    // 움직임 관련 변수
    [HideInInspector] public float MoveActionValue; // 움직임 방향키를 누를 때 반환되는 값을 캐싱
    [HideInInspector] public float MoveSpeed; // 움직이는 속도

    // 점프 관련 변수
    [HideInInspector] public float JumpSpeed;
    [HideInInspector] public float VerticalSpeed; // 플레이어의 수직 속도(중력에서 관리)
    float gravity = 17;
    float terminalSpeed = 20; // 수직 속도의 한계
    [HideInInspector] public bool IsGrounded;

    // 대쉬 관련 변수
    [HideInInspector] public float DashSpeed; // 대쉬 시 속도
    float remainDashTime = 0; // 남은 대쉬 시간 (상태 전환 용도)
    float dashDuration = 0.33f; // 전체 대쉬 시간

    // 공격 관련 변수
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsAttackMoving;
    public bool CanBasicHorizonSlashCombo;

    // 조작 관련 변수
    [HideInInspector] public CharacterController CharacterController;
    InputActionAsset inputActionAsset;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAndPenetrateAction;
    InputAction attack1Action;
    InputAction attack2Action;

    // 부가적인 변수
    [HideInInspector] public Animator Animator;
    [HideInInspector] public PlayerAnimationSetter AnimationSetter;
    public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;
    [HideInInspector] public bool IsLockOn;
    [HideInInspector] public bool IsLookRight;


    void Awake()
    {
        MoveSpeed = 5f;
        JumpSpeed = 9f;
        DashSpeed = 15f;
        IsLockOn = false;
        IsGrounded = true;

        Animator = GetComponent<Animator>();
        AnimationSetter = GetComponent<PlayerAnimationSetter>();
        CharacterController = GetComponent<CharacterController>();
        inputActionAsset = GetComponent<PlayerInput>().actions;

        PlayerStateMachine = new PlayerStateMachine(this);
        PlayerStateMachine.Initialize(PlayerStateMachine.idleAndMoveState);

        moveAction = inputActionAsset.FindAction("Move");
        jumpAction = inputActionAsset.FindAction("Jump");
        dashAndPenetrateAction = inputActionAsset.FindAction("DashAndPenetrate");
        attack1Action = inputActionAsset.FindAction("Attack1");
        attack2Action = inputActionAsset.FindAction("Attack2");
    }
    void Start()
    {

    }

    void Update()
    {
        if (transform.position.x != 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }

        MoveActionValue = moveAction.ReadValue<float>();

        Gravity();
        ManageRotate();
        AttackMoving();

        OnIdleAndMove();
        OnJump();
        OnDash();
        OnBasicHorizonSlash1();
        OnBasicHorizonSlash2();
        OnBasicVerticalSlash();
    }

    void Gravity()
    {
        VerticalSpeed -= gravity * Time.deltaTime;

        VerticalSpeed = Mathf.Clamp(VerticalSpeed, -terminalSpeed, terminalSpeed);

        Vector3 verticalMove = new Vector3(0, VerticalSpeed, 0);

        CollisionFlags flag = CharacterController.Move(verticalMove * Time.deltaTime);

        if ((flag & CollisionFlags.Below) != 0)
        {
            VerticalSpeed = 0;
        }
    }

    void ManageRotate()
    {
        if (CurrentPlayerState == PlayerState.Dash || IsAttacking) return; // 대쉬나 공격 중에는 캐릭터의 회전을 방지

        if (IsLockOn)
        {

        }
        else
        {
            if (MoveActionValue > 0)
            {
                transform.eulerAngles = Vector3.zero;
                IsLookRight = true;
            }
            else if (MoveActionValue < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                IsLookRight = false;
            }
        }
    }

    void AttackMoving() // 공격 시 앞으로 조금씩 움직이는 기능. 공격 시 IsAttackingMoving이 true가 되면 플레이어가 공격 방향으로 움직인다.
    {
        if (IsAttackMoving)
        {
            if (IsLookRight)
            {
                Vector3 moveVector = new Vector3(0, 0, 1);
                CharacterController.Move(moveVector * DashSpeed * Time.deltaTime / 4);
            }
            else
            {
                Vector3 moveVector = new Vector3(0, 0, -1);
                CharacterController.Move(moveVector * DashSpeed * Time.deltaTime / 4);
            }
        }
    }
    void OnIdleAndMove()
    {
        if (IsAttacking) return;

        if (MoveActionValue != 0 && CurrentPlayerState != PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
        if (CurrentPlayerState == PlayerState.IdleAndMove)
        {
            PlayerStateMachine.Execute();
        }
    }
    void OnJump()
    {
        if (jumpAction.WasPressedThisFrame() && IsGrounded && GroundCheck.IsTouching && CurrentPlayerState == PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.airborneState);

            VerticalSpeed = JumpSpeed;
            IsGrounded = false;
        }
        else if (!GroundCheck.IsTouching && IsGrounded && CurrentPlayerState == PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.airborneState);
        }

        if (GroundCheck.IsTouching && !IsGrounded && CurrentPlayerState == PlayerState.Airborne) // 착지 시 (에러 있음)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
            Debug.Log("범인?");
        }

        if (CurrentPlayerState == PlayerState.Airborne) // 공중에 있을 때
        {
            PlayerStateMachine.Execute();
        }

        IsGrounded = GroundCheck.IsTouching;
    }
    void OnDash()
    {
        if (dashAndPenetrateAction.WasPressedThisFrame() && (CurrentPlayerState == PlayerState.IdleAndMove))
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.dashState);
            remainDashTime = dashDuration;
        }
        else if (CurrentPlayerState == PlayerState.Dash)
        {
            if (remainDashTime > 0)
            {
                remainDashTime -= Time.deltaTime;
                PlayerStateMachine.Execute();
            }
            else
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
            }
        }
    }

    void OnBasicHorizonSlash1()
    {
        if (attack1Action.WasPressedThisFrame() && !IsAttacking && CurrentPlayerState == PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash1State);
        }
    }

    void OnBasicHorizonSlash2()
    {
        if (attack1Action.WasPressedThisFrame() && CanBasicHorizonSlashCombo)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash2State);
        }
    }

    void OnBasicVerticalSlash()
    {
        if (attack2Action.WasPressedThisFrame() && !IsAttacking && CurrentPlayerState == PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicVerticalSlashState);
        }
    }

}
