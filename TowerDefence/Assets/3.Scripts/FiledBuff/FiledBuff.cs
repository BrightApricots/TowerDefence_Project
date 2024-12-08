using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffField : MonoBehaviour
{
    [Header("버프 효과")]
    public float damageMultiplier = 2f;    // 공격력 30% 증가
    public float rangeMultiplier = 2f;     // 범위 20% 증가

    [Header("감지 설정")]
    public float checkInterval = 0.2f;        // 체크 주기
    public float checkHeight = 2f;          // 체크 높이
    public float heightOffset = 0.1f;         // 버프필드가 타워 아래에 위치할 높이
    public Vector3 checkSize = new Vector3(1f, 2f, 1f);  // 체크 영역 크기
    public LayerMask checkLayers;            // Inspector에서 Unwalkable 레이어 설정

    private Tower currentBuffedTower;
    private float originalHeight;
    private bool isChecking = false;  // 체크 중복 방지용 플래그

    private void Start()
    {
        originalHeight = transform.position.y;
        checkLayers = LayerMask.GetMask("Unwalkable");
        StartCoroutine(CheckForTowerAndBlock());
    }

    private IEnumerator CheckForTowerAndBlock()
    {
        while (true)
        {
            if (!isChecking)
            {
                isChecking = true;
                Vector3 currentPos = transform.position;
                Vector3 checkCenter = new Vector3(currentPos.x, originalHeight + checkHeight * 0.5f, currentPos.z);

                // 먼저 블록 높이 체크
                float blockHeight = originalHeight;
                Collider[] blockColliders = Physics.OverlapBox(
                    checkCenter,
                    checkSize * 0.5f,
                    Quaternion.identity,
                    checkLayers
                );

                foreach (Collider collider in blockColliders)
                {
                    float colliderTop = collider.bounds.max.y;
                    if (colliderTop > blockHeight)
                    {
                        blockHeight = colliderTop;
                    }
                }

                // 버프 필드를 블록 위로 이동
                transform.position = new Vector3(
                    currentPos.x,
                    blockHeight + heightOffset,
                    currentPos.z
                );

                // 타워 감지 (위치는 변경하지 않고 버프만 적용)
                Collider[] towerColliders = Physics.OverlapBox(
                    new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    new Vector3(2f, checkHeight, 2f),  // 감지 범위를 좀 더 넓게
                    Quaternion.identity
                );

                bool foundTower = false;
                Tower newTower = null;

                foreach (var col in towerColliders)
                {
                    Tower tower = col.GetComponentInParent<Tower>();
                    if (tower != null && !tower.isPreview)
                    {
                        newTower = tower;
                        foundTower = true;
                        Debug.Log($"타워 감지: {tower.name}");
                        break;
                    }
                }

                // 타워 버프 처리
                if (foundTower && newTower != currentBuffedTower)
                {
                    Debug.Log($"새로운 타워에 버프 적용: {newTower.name}");
                    if (currentBuffedTower != null)
                    {
                        RemoveBuff(currentBuffedTower);
                    }
                    currentBuffedTower = newTower;
                    ApplyBuff(currentBuffedTower);
                }
                else if (!foundTower && currentBuffedTower != null)
                {
                    Debug.Log("타워가 없어져 버프 제거");
                    RemoveBuff(currentBuffedTower);
                    currentBuffedTower = null;
                }

                isChecking = false;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void ApplyBuff(Tower tower)
    {
        if (tower != null)
        {
            Debug.Log($"Before buff - Damage: {tower.Damage}, Range: {tower.Range}");
            tower.ApplyBuff(this);
            Debug.Log($"After buff - Damage: {tower.Damage}, Range: {tower.Range}");
            ShowBuffEffect(true);
        }
    }

    private void RemoveBuff(Tower tower)
    {
        if (tower != null)
        {
            Debug.Log($"Before remove buff - Damage: {tower.Damage}, Range: {tower.Range}");
            tower.RemoveBuff(this);
            Debug.Log($"After remove buff - Damage: {tower.Damage}, Range: {tower.Range}");
            ShowBuffEffect(false);
        }
    }

    private void ShowBuffEffect(bool isActive)
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        Color targetColor = isActive ?
            new Color(1f, 1f, 1f, 0.8f) :
            new Color(1f, 1f, 1f, 0.5f);

        foreach (var sprite in sprites)
        {
            sprite.color = targetColor;
        }
    }

    private void OnDestroy()
    {
        if (currentBuffedTower != null)
        {
            //RemoveBuff(currentBuffedTower);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Vector3 checkCenter = new Vector3(
                transform.position.x,
                originalHeight + checkHeight * 0.5f,
                transform.position.z
            );
            Gizmos.DrawWireCube(checkCenter, checkSize);

            if (currentBuffedTower != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, currentBuffedTower.transform.position);
            }
        }
    }
}