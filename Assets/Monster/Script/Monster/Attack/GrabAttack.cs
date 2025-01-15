using System.Collections;
using UnityEngine;

public class GrabAttack : MonoBehaviour
{
    public Transform grabPosition;  // 그랩 위치
    public float grabDuration = 3f;  // 그랩 지속 시간 (3초)
    public string grabAnimationTrigger = "GrabSuccess";  // 몬스터의 잡기 성공 애니메이션 트리거 이름

    private Animator animator;  // 몬스터의 Animator (자식 콜라이더가 아닌 몬스터 객체의 애니메이터 사용)

    private void Start()
    {
        // 몬스터의 Animator 컴포넌트를 찾습니다.
        animator = GetComponentInParent<Animator>();  // 몬스터의 애니메이터를 부모에서 찾습니다
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && !player.IsGrabbed)
            {
                player.SetGrabbedState(true);
                StartCoroutine(GrabPlayer(player));

                // 잡기 성공 시 몬스터 애니메이션 재생
                if (animator != null)
                {
                    // 애니메이션 트리거 호출
                    animator.SetTrigger(grabAnimationTrigger);
                }
                else
                {
                    Debug.LogError("Animator not found on monster or its parent!");
                }
            }
        }
    }

    private IEnumerator GrabPlayer(Player player)
    {
        // 그랩 위치로 이동
        player.transform.position = grabPosition.position;
        player.transform.SetParent(grabPosition);  // 그랩 위치의 자식으로 설정

        // 3초 대기
        yield return new WaitForSeconds(grabDuration);

        // 3초 뒤에 그랩 해제
        player.transform.SetParent(null);  // 그랩 위치에서 벗어남
        player.SetGrabbedState(false);  // 그랩 상태 해제

        // 회전 초기화 (기울어짐 방지)
        player.transform.rotation = Quaternion.identity;  // 회전 값 초기화

        // 필요에 따라 Player가 원래 위치로 돌아가게 하거나, 다른 처리를 추가할 수 있습니다.
    }
}
