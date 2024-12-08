using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    [SerializeField]
    private GameObject parentObject; // 부모 오브젝트

    [SerializeField]
    private Transform targetChildObject; // 교체 대상 자식 오브젝트

    [SerializeField]
    private List<GameObject> replacementPrefabs; // 교체할 프리팹 리스트

    private GameObject lastReplacedPrefab; // 마지막으로 교체된 프리팹
    private bool isReplaced = false; // 프리팹 교체 여부 플래그

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 후에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 자식 오브젝트를 프리팹으로 교체하는 메서드
    public void ReplaceChildWithPrefab()
    {
        // 이미 교체되었으면 실행 중지
        if (isReplaced)
        {
            Debug.LogWarning("프리팹 교체는 한 번만 가능합니다.");
            return;
        }

        if (parentObject == null || targetChildObject == null || replacementPrefabs == null || replacementPrefabs.Count == 0)
        {
            Debug.LogError("부모 오브젝트, 교체 대상 자식 오브젝트 또는 교체할 프리팹 리스트가 설정되지 않았습니다.");
            return;
        }

        // 사용 가능한 프리팹 필터링
        List<GameObject> availablePrefabs = replacementPrefabs.FindAll(prefab => prefab != lastReplacedPrefab);

        if (availablePrefabs.Count == 0)
        {
            Debug.LogWarning("사용 가능한 프리팹이 없습니다.");
            return;
        }

        // 교체할 프리팹 랜덤 선택 
        GameObject selectedPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];

        if (selectedPrefab != null)
        {
            // 기존 자식이 이미 삭제되었는지 확인
            if (targetChildObject == null)
            {
                Debug.LogWarning("교체 대상 자식 오브젝트가 이미 삭제되었습니다.");
                return;
            }

            // 기존 자식 정보 저장
            Vector3 originalPosition = targetChildObject.localPosition;
            Quaternion originalRotation = targetChildObject.localRotation;
            Vector3 originalScale = targetChildObject.localScale;

            // 기존 자식 삭제
            Destroy(targetChildObject.gameObject);

            // 프리팹 생성 및 부모에 추가
            GameObject newChild = Instantiate(selectedPrefab, parentObject.transform);

            // 기존 자식의 위치, 회전, 스케일 유지
            newChild.transform.localPosition = originalPosition;
            newChild.transform.localRotation = originalRotation;
            newChild.transform.localScale = originalScale;

            // 마지막 교체된 프리팹 업데이트
            lastReplacedPrefab = selectedPrefab;

            // 교체 완료 플래그 설정
            isReplaced = true;

            Debug.Log($"'{targetChildObject.name}' 자식을 '{selectedPrefab.name}' 프리팹으로 교체했습니다.");
        }
        else
        {
            Debug.LogWarning("선택된 프리팹이 없습니다.");
        }
    }
}
