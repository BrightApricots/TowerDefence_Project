using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossMonster : Monster
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(DoRoar());
        
    }

    IEnumerator DoRoar()
    {
        anim.SetBool("IsRoar", true);
        yield return new WaitForSeconds(5f);
        anim.SetBool("IsMove", true);
        Initialize(spawnPos);
        isMoving = true;
        yield return new WaitForSeconds(3f);
    }

    //보스몬스터가 일반 몬스터랑 다른점?
    //웨이브 매니저와 상관 없이 나옴.
    //보스가 죽기 전까진 웨이브가 끝나면 안됨. 겜메에 보스 카운트 넣어서 웨이브 매니저 클리어 조건에 추가
    //예상 순서 : 배틀 버튼 -> 보스가 스폰1위치에 소환 -> 카메라 이동 -> 카메라 원점 -> 웨이브 시작
    //
}
