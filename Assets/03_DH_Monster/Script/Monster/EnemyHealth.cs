using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;        // �ִ� ü��
    public float currentHealth;           // ���� ü��
    private Enemy enemy;
    public float maxSoulGauge;     // �ִ� ��ȥ ������
    public float currentSoulGauge;        // ���� ��ȥ ������
    public float toughness = 10f;          // ������ ������ (��: 10) 
    private Animator animator;             // ���� �ִϸ�����
   

    // �׷α� ���º� ���� �ð� ����
    
    [System.Serializable]
    public class ItemDrop
    {
        public GameObject itemPrefab; // ����� ������ ������
        public float dropChance; // ��� Ȯ��
    }
    public ItemDrop[] itemDrops; // ����� �����۵�

    private void Start()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� �ʱ�ȭ
        currentHealth = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ
        currentSoulGauge = maxSoulGauge; // ���� ��ȥ �������� �ִ� ��ȥ �������� �ʱ�ȭ
    }
    // �������� �޾��� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage, float soulDamage, float attackPower)
    {
        DetermineGroggyState(attackPower); // �׷α� ���� ����

        currentHealth -= damage;          // ü�� ����
        currentSoulGauge -= soulDamage;   // ��ȥ ������ ����

        if (currentHealth <= 0)
        {
            Die();  // ü���� 0�̸� ���� ó��
        }

        if (currentSoulGauge <= 0)
        {
            EnterKnockdown() ;  // ��ȥ �������� 0�̸� �׷α� ����
        }

        currentHealth = Mathf.Max(currentHealth, 0);
        currentSoulGauge = Mathf.Max(currentSoulGauge, 0);
    }

  

    private void DetermineGroggyState(float attackPower)
    {
        if (attackPower <= toughness * 0.25f)
        {
            
        }
        else if (attackPower <= toughness * 0.5f)
        {
            animator.SetTrigger("ShortGroggy");
        }
        else
        {
            animator.SetTrigger("Knockdown");
        }
    }


    // ���� ó��
    private void Die()
    {
        DropItem();
        animator.SetTrigger("Die");
    }

    // ��ȥ ������ ȸ�� �޼���

    public void EnterKnockdown()//�ܺ�ȣ���
    {
        animator.SetTrigger("Knockdown");



        // �ҿ� �������� 0�� ���� ȸ�� (������ 0���� ���� Knockdown�� ���)
        if (currentSoulGauge <= 0)
        {
            currentSoulGauge = maxSoulGauge; // �ҿ� �������� �ִ밪���� ȸ��
            
        }
    }
    public void EnterShortGroggy()//�ܺ�ȣ���
    {
        animator.SetTrigger("ShortGroggy");
    }


    private void DropItem()
    {
        foreach (var itemDrop in itemDrops)
        {
            if (Random.value <= itemDrop.dropChance) // �� �����ۺ� ��� Ȯ�� üũ
            {
                Instantiate(itemDrop.itemPrefab, transform.position, Quaternion.identity);
                
            }
        }
    }
   
    
    public void OnDeathAnimationEnd()//�ִϸ��̼� �̺�Ʈ �߰�
    {
        Destroy(gameObject);
    }
  
}

