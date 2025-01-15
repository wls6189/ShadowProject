using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;



public class Enemy : MonoBehaviour
{
    private Transform player;
    public float detectionRange = 10f; // Ž�� ����
    public float standoffRange = 5f; // ��ġ ����
    public float attackRange = 2f; // ���� ����
    public float attackCooldown; // ���� ��Ÿ��   
    public float guardDuration = 3f; // ���� ���� ���� �ð�
    private float guardStartTime; // ���� ���� ���� �ð�
    public float parryDuration = 3f;
    public float parryStartTime;
    public int successfulGuardsToParry = 3; // �и� ���·� ��ȯ�� ���� ���� Ƚ��
    private int guardSuccessCount = 0; // ���� ���� Ƚ��
    private Collider guardCollider; // ���� �ݶ��̴�
    private Collider parryCollider; // �и� �ݶ��̴�
    private bool hasEngaged = false;// ó�� �����ߴ��� ����


    public AttackPattern[] attackPatterns; // ����� ���� ���� �迭
   

    private float lastAttackTime; // ������ ���� �ð�

    private NavMeshAgent navMeshAgent; // NavMeshAgent ������Ʈ
    private bool isPlayerOnNavMesh = true; // �÷��̾ NavMesh�� �ִ��� ����
    
    private Animator animator; // Animator ������Ʈ


    private int currentPatternIndex = 0; // ���� ��� ���� ���� �ε���
    private int currentAttackIndex = 0;  // ���� ���� �� ���� �ε���
    public enum State { Idle, Chasing, Guard, Parry }
    public State currentState;



    public TextMeshProUGUI indicatorText;
    public float indicatorDuration = 1f; // �ε������Ͱ� ǥ�õǴ� �ð�
    private bool isIndicatorActive = false;


    //��������϶� ��������� ����Ǯ�� ���� �����ؾ���


    private void Start()
    {

        guardCollider = transform.Find("Guard").GetComponent<Collider>();
        parryCollider = transform.Find("Parry").GetComponent<Collider>();

        
        guardCollider.gameObject.SetActive(false);
        parryCollider.gameObject.SetActive(false);
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); 
        GameObject playerObject = GameObject.FindWithTag("Player"); 
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found.");
        }


    }

    private void Update()
    {


        if (player == null) return;

        
        UpdatePlayerNavMeshStatus();

        switch (currentState)
        {
            case State.Idle:
                animator.SetTrigger("Idle");

                CheckForChase();
                break;

            case State.Chasing:
                navMeshAgent.isStopped = false;
                animator.SetTrigger("Run");

                if (isPlayerOnNavMesh)
                {
                    FollowPlayer();
                    CheckForGuard(); // ���� ���� ���� ���� Ȯ��
                }
                break;

            case State.Guard:
                animator.SetTrigger("Guard");
                navMeshAgent.isStopped = true;

                // ���� ���� �ð��� ������ �ٽ� �߰� ���·� ��ȯ
                if (Time.time - guardStartTime >= guardDuration)
                {
                    currentState = State.Chasing;
                    guardCollider.gameObject.SetActive(false); // ���� �ݶ��̴� ��Ȱ��ȭ                                       
                    navMeshAgent.isStopped = false;
                    
                    Debug.Log("Guard ended, switching to Chasing.");
                }
                break;

            case State.Parry:
                animator.SetTrigger("Parry");
                navMeshAgent.isStopped = true;
                // �и� ���� �ٽ� �߰� ���·� ��ȯ
                if (Time.time - parryStartTime >= guardDuration)
                {
                    currentState = State.Chasing;
                    parryCollider.gameObject.SetActive(false); // �и� �ݶ��̴� ��Ȱ��ȭ
                    navMeshAgent.isStopped = false;
                    Debug.Log("Parry executed, switching to Chasing.");
                }
                break;
        }

        TryAttack();
    }


    private void UpdatePlayerNavMeshStatus()
    {
        NavMeshHit hit;
        isPlayerOnNavMesh = NavMesh.SamplePosition(player.position, out hit, 1.0f, NavMesh.AllAreas);

    }


    private void CheckForChase()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Ž�� ���� �ȿ� ������ �߰� ����
        if (distance <= detectionRange)
        {
            currentState = State.Chasing;

            // �̵� Ȱ��ȭ
            navMeshAgent.isStopped = false;
            // ������ �ʱ�ȭ
            navMeshAgent.SetDestination(player.position);

            Debug.Log("Player detected, starting chase.");
        }
    }

    private void FollowPlayer()
    {
        if (navMeshAgent == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // ���� ������ ������ ���� ��� ��� �߰�
        if (distance > attackRange)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            navMeshAgent.ResetPath(); // ���� ���� �ȿ����� �̵� ����
        }
    }
    private void CheckForGuard()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // ��ġ ������ ó�� ������ ���� ���·� ��ȯ
        if (!hasEngaged && currentState == State.Chasing && distance <= standoffRange && distance > attackRange)
        {
            currentState = State.Guard;
            guardStartTime = Time.time; // ���� ���� �ð� ���
            guardCollider.gameObject.SetActive(true); // ���� �ݶ��̴� Ȱ��ȭ
            hasEngaged = true; // ó�� ���� ���� ���

            Debug.Log("Guard state triggered on first engagement.");
            return; // ���� ���·� ��ȯ �� �� �̻� ���� Ȯ������ ����
        }

        // �÷��̾ �����Ϸ��� �� �� ���� ���·� ��ȯ
        if (IsPlayerAttacking())
        {
            currentState = State.Guard;
            guardStartTime = Time.time; // ���� ���� �ð� ���
            guardCollider.gameObject.SetActive(true); // ���� �ݶ��̴� Ȱ��ȭ
            
        }
    }

    private bool IsPlayerAttacking()
    {
        if (player == null) return false;

        // Ž�� ���� ������ "PlayerAttack" �±׸� ���� �ݶ��̴��� ã��
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider collider in colliders)
        {
            // "PlayerAttack" �±׸� ���� Ȱ��ȭ�� �ݶ��̴��� �ִ��� Ȯ��
            if (collider.CompareTag("PlayerAttack") && collider.enabled)
            {
                return true; // ���� �ݶ��̴��� Ȱ��ȭ�� ���
            }
        }

        return false; // Ȱ��ȭ�� ���� �ݶ��̴��� ������ false
    }
    //���߿� ������ �ؿ� �̰ɷ� �ٲ����
    //private bool IsPlayerAttacking()
    //{
    //    return player != null && player.GetComponent<PlayerController>().IsAttacking;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (currentState == State.Guard && other.CompareTag("PlayerAttack"))
        {
            guardSuccessCount++;
            Debug.Log($"Guard successful! Total: {guardSuccessCount}");
            animator.SetTrigger("GuardSuccess");

            if (guardSuccessCount >= successfulGuardsToParry)
            {
                // �и� ���·� ��ȯ
                currentState = State.Parry;
                parryStartTime = Time.time;
                guardSuccessCount = 0; // ī��Ʈ �ʱ�ȭ
                Debug.Log("Switching to Parry State.");
                guardCollider.gameObject.SetActive(false); // ���� �ݶ��̴� ��Ȱ��ȭ
                parryCollider.gameObject.SetActive(true); // �и� �ݶ��̴� Ȱ��ȭ
            }
            else
            {
                Debug.Log("Guarded successfully.");
            }
        }

        // �и� ���¿��� �÷��̾� ���� ����
        if (currentState == State.Parry && other.CompareTag("PlayerAttack"))
        {
            Debug.Log("Parry successful! Player attack was countered.");
            animator.SetTrigger("ParrySuccess");        
            // �и� ���� ����
            currentState = State.Chasing;
            parryCollider.gameObject.SetActive(false); // �и� �ݶ��̴� ��Ȱ��ȭ
        }
    }



    private void TryAttack()
    {

        if (currentState != State.Chasing) return; // �߰� ���¿����� ����

        // ��ٿ� Ȯ��
        if (Time.time - lastAttackTime < attackCooldown) return;

        // �÷��̾ ���� ���� �ȿ� ���� ���
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            ExecuteAttack(); // ���� ����
        }
    }

    private void ExecuteAttack()
    {
        if (attackPatterns.Length == 0) return;

        // ���� ���ϰ� ���� ��������
        AttackPattern currentPattern = attackPatterns[currentPatternIndex];
        if (currentPattern.attacks.Length == 0) return;

        Attack currentAttack = currentPattern.attacks[currentAttackIndex];
        ShowAttackIndicator(currentAttack);
        // ���� �ִϸ��̼� Ʈ���� �߰�
        animator.SetTrigger(currentAttack.attackName);  // ��: "Heavy Strike" �Ǵ� "Quick Parry" ��
        lastAttackTime = Time.time; // ���� �ð� ����

        //���� �ε������� �߰�����
        

        // ���� ����
        


        // ���� �������� �̵�
        currentAttackIndex = (currentAttackIndex + 1) % currentPattern.attacks.Length;

        // ���� ���� ���� (���ϴ� ������)
        if (currentAttackIndex == 0)
        {
            currentPatternIndex = (currentPatternIndex + 1) % attackPatterns.Length;
        }

    }
    private void ShowAttackIndicator(Attack attack)
    {
        if (indicatorText == null)
        {
            
            return;
        }
        if (isIndicatorActive) return;

        // �ε������Ͱ� ���� Ȱ��ȭ�Ǿ����Ƿ�, ���� ����
        isIndicatorActive = true;
        // ���� ���� �ؽ�Ʈ ����
        indicatorText.text = new string('!', attack.indicatorhLevel); // ����ǥ ����
        indicatorText.color = attack.indicatorColor; // ���� ���� ����

       

        // ���� �ð� �� �ε������� �����
        StartCoroutine(HideAttackIndicatorAfterTime(indicatorDuration));
    }

    private IEnumerator HideAttackIndicatorAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        // �ؽ�Ʈ�� �����
        if (indicatorText != null)
        {
            indicatorText.text = ""; // �ؽ�Ʈ ����
            isIndicatorActive = false; // �ε������Ͱ� ��Ȱ��ȭ��
            
        }
    }




}









