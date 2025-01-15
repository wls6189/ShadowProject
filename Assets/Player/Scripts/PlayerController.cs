using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState // �÷��̾��� ���� �ൿ(Ȥ�� ����)
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
    // �ν����Ϳ��� �Ҵ��� �ʿ��� ����
    public GroundCheck GroundCheck; // �÷��̾��� ���� ���� �پ��ִ��� üũ�ϴ� Ŭ����
    public Collider WeaponCollider;

    // ������ ���� ����
    [HideInInspector] public float MoveActionValue; // ������ ����Ű�� ���� �� ��ȯ�Ǵ� ���� ĳ��
    [HideInInspector] public float MoveSpeed; // �����̴� �ӵ�

    // ���� ���� ����
    [HideInInspector] public float JumpSpeed;
    [HideInInspector] public float VerticalSpeed; // �÷��̾��� ���� �ӵ�(�߷¿��� ����)
    float gravity = 17;
    float terminalSpeed = 20; // ���� �ӵ��� �Ѱ�
    [HideInInspector] public bool IsGrounded;

    // �뽬 ���� ����
    [HideInInspector] public float DashSpeed; // �뽬 �� �ӵ�
    float remainDashTime = 0; // ���� �뽬 �ð� (���� ��ȯ �뵵)
    float dashDuration = 0.33f; // ��ü �뽬 �ð�

    // ���� ���� ����
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsAttackMoving;
    public bool CanBasicHorizonSlashCombo;

    // ���� ���� ����
    [HideInInspector] public CharacterController CharacterController;
    InputActionAsset inputActionAsset;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAndPenetrateAction;
    InputAction attack1Action;
    InputAction attack2Action;

    // �ΰ����� ����
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
        if (CurrentPlayerState == PlayerState.Dash || IsAttacking) return; // �뽬�� ���� �߿��� ĳ������ ȸ���� ����

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

    void AttackMoving() // ���� �� ������ ���ݾ� �����̴� ���. ���� �� IsAttackingMoving�� true�� �Ǹ� �÷��̾ ���� �������� �����δ�.
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

        if (GroundCheck.IsTouching && !IsGrounded && CurrentPlayerState == PlayerState.Airborne) // ���� �� (���� ����)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
            Debug.Log("����?");
        }

        if (CurrentPlayerState == PlayerState.Airborne) // ���߿� ���� ��
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
