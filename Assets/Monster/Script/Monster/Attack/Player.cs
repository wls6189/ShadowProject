using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsGrabbed { get; private set; }

    public void SetGrabbedState(bool grabbed)
    {
        IsGrabbed = grabbed;

        // �׷� ���¿��� �̵��� ���� �ڵ� �߰�
        if (grabbed)
        {
            // �߰������� �ִϸ��̼��̳� �ٸ� ó���� �ʿ��� �� �ֽ��ϴ�.
        }
    }

    private void Update()
    {
        if (!IsGrabbed)
        {
            // �̵� ó�� �ڵ�
        }
    }
}
