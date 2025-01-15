using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Enemy/Attack")]
public class Attack : ScriptableObject
{
    public string attackName;              // ���� �̸�
    public AttackType attackType;          // ���� Ÿ��
    public float attackRange;              // ���� ��Ÿ�

    [Header("(indicator Settings")]
    public int indicatorhLevel;             // ���� �ܰ� (����ǥ ����)
    public Color indicatorColor;           // ���� ����
}