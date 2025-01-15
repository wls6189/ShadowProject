using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackPattern", menuName = "Enemy/AttackPattern")]
public class AttackPattern : ScriptableObject
{
    public string patternName; // ���� �̸�
    public Attack[] attacks; // ���Ͽ� ���Ե� ����
}