using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class BossMonster : Monster
{
    private Animator anim;
    private bool isStart = false;
    [SerializeField] WaveManager waveInfo;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        
    }


    private void Start()
    {
        GameManager.Instance.BossCount = 1;
    }
    protected override void Update()
    {
        if(waveInfo.isFirstBattleClicked == true && isStart == false)
        {
            StartCoroutine(DoRoar());
            isStart = true;
        }
        base.Update();
    }

    IEnumerator DoRoar()
    {
        anim.SetBool("IsScream", true);
        yield return new WaitForSeconds(3f);
        anim.SetBool("IsWalk", true);
        Initialize(spawnPos);
        isMoving = true;
        yield return new WaitForSeconds(1f);
    }
    //보스몬스터가 일반 몬스터랑 다른점?
    //웨이브 매니저와 상관 없이 나옴.
    //보스가 죽기 전까진 웨이브가 끝나면 안됨. 겜메에 보스 카운트 넣어서 웨이브 매니저 클리어 조건에 추가
    //예상 순서 : 배틀 버튼 -> 보스가 스폰1위치에 소환 -> 카메라 이동 -> 카메라 원점 -> 웨이브 시작
    //
}
