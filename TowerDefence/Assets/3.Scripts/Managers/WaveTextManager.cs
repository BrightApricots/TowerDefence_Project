using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveTextManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI waveInfoText; // 웨이브 정보를 표시할 TextMeshPro
    [SerializeField]
    private GameObject displayPanel; // 정보를 표시할 패널

    private List<Wave> waves; // 웨이브 데이터를 참조할 리스트
    private int currentWaveIndex = 0; // 현재 웨이브 인덱스

    // 웨이브 데이터를 설정
    public void Initialize(List<Wave> waveData)
    {
        waves = waveData;
        UpdateWaveInfo(); // 초기 상태 업데이트
    }

    // 현재 웨이브 인덱스 설정
    public void SetCurrentWaveIndex(int index)
    {
        currentWaveIndex = index;
        UpdateWaveInfo();
    }

    private void UpdateWaveInfo()
    {
        if (currentWaveIndex >= waves.Count || waveInfoText == null || displayPanel == null) return;

        Wave nextWave = waves[currentWaveIndex];
        string info = ""; // 웨이브 번호 제거, 몬스터 정보만 표시

        foreach (var monster in nextWave.monsterSpawnData)
        {
            // monsterPrefab.name 사용
            info += $"{monster.monsterPrefab.name} x {monster.spawnCount}\n";
        }

        waveInfoText.text = info.Trim(); // 불필요한 마지막 개행 제거
    }

    // 패널을 활성화 (웨이브 대기 중)
    public void ShowWaveInfo()
    {
        displayPanel.SetActive(true);
    }

    // 패널을 비활성화 (웨이브 진행 중)
    public void HideWaveInfo()
    {
        displayPanel.SetActive(false);
    }
}
