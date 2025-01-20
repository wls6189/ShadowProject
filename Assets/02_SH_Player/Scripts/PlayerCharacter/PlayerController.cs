using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState // �÷��̾��� ���� �ൿ(Ȥ�� ����)
{
    IdleAndMove,
    Dash,
    Airborne,
    ShortGroggy,
    LongGroggy,
    Guard,
    GuardMove,
    Penetrate,
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
    public Collider WeaponCollider; // ������ �ݶ��̴�
    public NearbyMonsterCheck NearbyMonsterCheck; // ��ó�� ���͸� �����ϴ� Ŭ����
    public Transform CameraFocusPosition; // ī�޶��� ��Ŀ�� ��ġ

    // ������ ���� ����
    [HideInInspector] public float MoveActionValue; // ������ ����Ű�� ���� �� ��ȯ�Ǵ� ���� ĳ��
    [HideInInspector] public float MoveSpeed; // �����̴� �ӵ�

    // ���� ���� ����
    [HideInInspector] public float VerticalSpeed; // �÷��̾��� ���� �ӵ�(�߷¿��� ����)
    float gravity = 17; // �߷�
    float terminalSpeed = 20; // ���� �ӵ��� �Ѱ�
    [HideInInspector] public bool IsGrounded;

    // �뽬 ���� ����
    [HideInInspector] public float DashSpeed; // �뽬 �� �ӵ�

    // ���� ���� ����
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsAttackMoving;
    [HideInInspector] public bool CanBasicHorizonSlashCombo;
    [HideInInspector] public bool IsAttackColliderEnabled;

    // ���� ���� ����
    public bool IsParring;
    public bool IsGuarding;
    public bool IsParrySucceed;

    // ���� ���� ����
    [HideInInspector] public CharacterController CharacterController;
    InputActionAsset inputActionAsset;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAndPenetrateAction;
    InputAction attack1Action;
    InputAction attack2Action;
    InputAction attack3Action;
    InputAction guardAction;
    InputAction lockOnAction;
    InputAction interactAction;

    // �ΰ����� ����
    [HideInInspector] public Animator Animator;
    [HideInInspector] public AnimatorStateInfo StateInfo;
    [HideInInspector] public PlayerAnimationSetter AnimationSetter;
    [HideInInspector] public PlayerState CurrentPlayerState;
    [HideInInspector] public PlayerStateMachine PlayerStateMachine;
    [HideInInspector] public bool IsLockOn;
    [HideInInspector] public bool IsLookRight;
    GameObject targetMonster;
    int targetMonsterIndex = -1;
    Coroutine endParryCoroutine;
    Coroutine waitMotionCoroutine;
    Coroutine parrySucceedDurationCoroutine;

    void Awake()
    {
        MoveSpeed = 5f;
        DashSpeed = 10f;
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
        attack3Action = inputActionAsset.FindAction("Attack3");
        guardAction = inputActionAsset.FindAction("Guard");
        lockOnAction = inputActionAsset.FindAction("LockOn");
        interactAction = inputActionAsset.FindAction("Interact");
    }

    void Update()
    {
        if (transform.position.x != 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
        }

        StateInfo = Animator.GetCurrentAnimatorStateInfo(0);

        PlayerStateMachine.Execute();

        MoveActionValue = moveAction.ReadValue<float>();


        Gravity();
        ManageRotate();
        LockOnPressed();

        DashPressed();
        GuardPressed();
        BasicHorizonSlashPressed();
        BasicVerticalSlashPressed();
        ThrustPressed();
        IdleAndMovePressed();
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
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash) return; // ���� ���̰ų� �뽬 ���̶�� ȸ�� ����

        if (IsLockOn)
        {
            if (targetMonster.transform.position.z >= transform.position.z)
            {
                transform.eulerAngles = Vector3.zero;
                IsLookRight = true;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
                IsLookRight = false;
            }
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
    public void AttackMoving(float moveSpeed) // ���� �� ������ ���ݾ� �����̴� ���. ���� �� IsAttackingMoving�� true�� �Ǹ� �÷��̾ ���� �������� �����δ�.
    {
        if (IsLookRight)
        {
            Vector3 moveVector = new Vector3(0, 0, 1);
            CharacterController.Move(moveVector * moveSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 moveVector = new Vector3(0, 0, -1);
            CharacterController.Move(moveVector * moveSpeed * Time.deltaTime);
        }
    }
    void LockOnPressed() // ���� ���� ����
    {
        if (lockOnAction.WasPressedThisFrame())
        {
            if (!NearbyMonsterCheck.IsMonsterExist()) // ��ó�� ���� ���ٸ� ������ �ʱ�ȭ�ϰ� ����
            {
                IsLockOn = false;
                targetMonster = null;
                targetMonsterIndex = -1;
                CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
                return;
            }

            IsLockOn = true;

            if (targetMonsterIndex + 1 > NearbyMonsterCheck.Monsters.Count - 1) // ������ ���� ���Ͱ� ���ٸ� ������ �ʱ�ȭ�ϰ� ���� ���� ����
            {
                IsLockOn = false;
                targetMonster = null;
                targetMonsterIndex = -1;
                CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
            }
            else // ������ ���� ���Ͱ� �ִٸ� targetMonster�� ���� ���ͷ� ����
            {
                targetMonsterIndex += 1;
                targetMonster = NearbyMonsterCheck.Monsters[targetMonsterIndex];
                CameraFocusPosition.position = (transform.position + targetMonster.transform.position) / 2;
            }
        }

        if (IsLockOn && !NearbyMonsterCheck.Monsters.Contains(targetMonster)) // ���� �����ε� ���Ͱ� �ʹ� �ָ� �������ٸ�(�������� ����ٸ�) ���� ����
        {
            IsLockOn = false;
            targetMonster = null;
            targetMonsterIndex = -1;
            CameraFocusPosition.localPosition = new Vector3(0, 0, 1);
        }
    }
    void ManageParrySucceedDuration()
    {
        if (IsParrySucceed)
        {
            if (parrySucceedDurationCoroutine == null)
            {
                parrySucceedDurationCoroutine = StartCoroutine(ParrySucceedDuration());
            }
            else
            {
                StopCoroutine(parrySucceedDurationCoroutine);
                parrySucceedDurationCoroutine = StartCoroutine(ParrySucceedDuration());
            }
        }
    }
    void IdleAndMovePressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (moveAction.IsPressed() && CurrentPlayerState != PlayerState.IdleAndMove)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
        }
    }

    void DashPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (dashAndPenetrateAction.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.dashState);
        }
    }

    void BasicHorizonSlashPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack1Action.WasPressedThisFrame() && CanBasicHorizonSlashCombo)
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash2State);
        }
        else if (attack1Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicHorizonSlash1State);
        }
    }

    void BasicVerticalSlashPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack2Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.basicVerticalSlashState);
        }
    }

    void GuardPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash) return;

        if (guardAction.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.guardState);
            StartCoroutine(EndParry());
        }

        if (guardAction.WasReleasedThisFrame())
        {
            if (StateInfo.IsName("GuardHit") || StateInfo.IsName("Parry")) // ���峪 �и� �� ���� Ű�� �ٷ� ���� ����� ������ �ʵ��� �����ϴ� �ڵ�
            {
                StartCoroutine(WaitGuardHitAndParryMotion(StateInfo.length * (1 - StateInfo.normalizedTime)));
            }
            else
            {
                PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
            }
        }
    }

    void ThrustPressed()
    {
        if (IsAttacking || CurrentPlayerState == PlayerState.Dash || CurrentPlayerState == PlayerState.Guard) return;

        if (attack3Action.WasPressedThisFrame())
        {
            PlayerStateMachine.TransitionTo(PlayerStateMachine.thrustState);
        }
    }












    IEnumerator CanBasicHorizonSlashTerm()
    {
        yield return new WaitForSeconds(1f);
        CanBasicHorizonSlashCombo = false;
    }
    IEnumerator EndParry() // �и��� �����Ǵ� �ð�
    {
        yield return new WaitForSeconds(0.15f);
        IsParring = false;
    }
    IEnumerator WaitGuardHitAndParryMotion(float remainDuration) // ���� �� �и� �� ����� �����Ǵ� �ð�
    {
        yield return new WaitForSeconds(remainDuration);
        PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
    }
    IEnumerator ParrySucceedDuration()
    {
        yield return new WaitForSeconds(0.5f);
        IsParrySucceed = false;
    }
}

























//legacy
// [HideInInspector] public float JumpSpeed;

//void OnJump()
//{
//    if (jumpAction.WasPressedThisFrame() && IsGrounded && GroundCheck.IsTouching && CurrentPlayerState == PlayerState.IdleAndMove)
//    {
//        PlayerStateMachine.TransitionTo(PlayerStateMachine.airborneState);

//        VerticalSpeed = JumpSpeed;
//        IsGrounded = false;
//    }
//    else if (!GroundCheck.IsTouching && IsGrounded && CurrentPlayerState == PlayerState.IdleAndMove)
//    {
//        PlayerStateMachine.TransitionTo(PlayerStateMachine.airborneState);
//    }

//    if (GroundCheck.IsTouching && !IsGrounded && CurrentPlayerState == PlayerState.Airborne) // ���� �� (���� ����)
//    {
//        PlayerStateMachine.TransitionTo(PlayerStateMachine.idleAndMoveState);
//        Debug.Log("����?");
//    }

//    if (CurrentPlayerState == PlayerState.Airborne) // ���߿� ���� ��
//    {
//        PlayerStateMachine.Execute();
//    }

//    IsGrounded = GroundCheck.IsTouching;
//}