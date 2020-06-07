using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum State
{
    MOVE,
    REST,
    ATKREADY,
    ATTACK,

    NOTING
}

[System.Serializable]
public class BossPattern
{
    public Vector2 startPos;//시작위치

    public int animNum;//애니메이션 번호
    public float animTime;//애니메이션 총 재생시간
    
    public virtual void ActBegin() { }//처음 동작
    public virtual void ActTrigger() { }//애니메이션 트리거 동작
    public virtual void ActSoundTrigger() { }//사운드 트리거 동작
    public virtual void ActAllWay() { }//언제나 동작
    public virtual void ActEnd() { }//끝나고 동작
}


public class Boss : MonoBehaviour
{
    protected State state;

    [SerializeField]
    private Vector2 moveArea;

    [SerializeField]
    public float speed;
    private Vector2 dest;

    private int patternIndex = 0;//지금까지 몇번 움직였는가

    //몇번 움직이니
    public int maxMoveCount;
    private int movCount = 0;
    //중간에 쉬는 시간
    public int maxRestTime;
    private float restTime;

    private bool rageMode = false;


    [HideInInspector]
    public Animator anim;
    [SerializeField]
    private GameObject explosion;

    private SpriteRenderer spriteR;
    public Color highlightColor;

    [HideInInspector]
    public Collider2D coll;

    [SerializeField]
    protected BossPattern[] patterns;
    protected BossPattern[] ragePatterns;

    protected BossPattern curPattern;

    private void Start()
    {
        state = State.NOTING;
    }


    public void Init()
    {
        anim = GetComponentInChildren<Animator>();
        spriteR = GetComponentInChildren<SpriteRenderer>();
        coll = GetComponentInChildren<Collider2D>();
        
        anim.SetInteger("State", -1);//애니메이션 상태
        StartCoroutine(StartCounter());
    }

    IEnumerator StartCounter()
    {

        yield return new WaitForSeconds(3);
        
        state = State.MOVE;
        //패턴 섞고 시작
        PatternShuffle();
        BossPattern();
    }


    public Vector2 GetRandomArea(float yClamp)//보스 활동 영역안에서 랜덤
    {
        return new Vector2(Random.Range(-moveArea.x, moveArea.x), Random.Range(yClamp, moveArea.y));
    }

    private float time;
    private void Update()
    {
        if (state == State.NOTING) return;

        switch (state)
        {
            case State.MOVE:
            case State.ATKREADY:

                transform.position = Vector2.MoveTowards(transform.position, dest, speed * Time.deltaTime);

                Vector2 diff = dest - (Vector2)transform.position;
                if (diff.sqrMagnitude < 0.1f * 0.1f)
                    BossPattern();

                break;


            case State.ATTACK:
                curPattern.ActAllWay();//패턴 업뎃
                anim.SetInteger("State", -1);//애니메이션 상태

                time += Time.deltaTime;
                if (time > restTime) BossPattern();
                break;

            default:
                time += Time.deltaTime;
                if (time > restTime) BossPattern();
                break;

        }
    }

    void BossPattern()
    {
        time = 0;
        switch (state)
        {
            case State.MOVE://이동이 끝나고 휴식
                anim.SetInteger("State", 0);
                restTime = Random.Range(0.1f, maxRestTime);
                state = State.REST;

                break;

            case State.REST://휴식이 끝나고 이동이나 공격준비

                anim.SetInteger("State", 1);

                if (movCount <= 0)//이동이 모두 끝나고 공격준비
                {
                    curPattern = patterns[patternIndex];
                    curPattern.ActBegin();

                    dest = curPattern.startPos;
                    state = State.ATKREADY;

                    patternIndex++;//패턴 카운트 +1
                    if (patternIndex >= patterns.Length) PatternShuffle();//꽉차면 셔플
                    break;
                }
                movCount--;

                //이동
                dest = GetRandomArea(-moveArea.y);//랜덤 위치 정하기
                state = State.MOVE;
                break;

            case State.ATKREADY://공격준비가 끝나고 공격으로
                anim.SetInteger("State",curPattern.animNum);
                state = State.ATTACK;
                
                restTime = curPattern.animTime;

                break;

            case State.ATTACK://공격이 끝나고 움직임으로

                curPattern.ActEnd();//종료 패턴
                
                movCount = Random.Range(1, maxMoveCount);
                state = State.MOVE;
                break;
        }
    }

    private void PatternShuffle()//패턴 섞기
    {
        patternIndex = 0;
        int n;
        for (int i = 0; i < patterns.Length; i++)
        {
            n = Random.Range(0, patterns.Length);

            var temp = patterns[i];
            patterns[i] = patterns[n];
            patterns[n] = temp;
        }
    }



    public void AnimTrigger()//애니메이션 트리거
    {
        curPattern.ActTrigger();
    }

    public void SoundTrigger()
    {
        if (state != State.ATTACK)//현재 나오는 패턴이 없으면 대신 재생
        {
            GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Foot);
            return;
        }

        curPattern.ActSoundTrigger();
    }


    //공격 받음
    public void Hit(int damage)
    {
        int hp = GameManager.I().ReduceHP(1);

        if (!rageMode && hp <= GameManager.I().BossMaxHP * 0.5f)//반피 까이면 분노모드
        {
            anim.speed = 1.5f;
            anim.SetBool("Rage", true);
            speed *= 1.5f;
            rageMode = true;

            patterns = ragePatterns;
        }

        if (hp <= 0)
        {
            explosion.gameObject.SetActive(true);

            anim.SetInteger("State",-1);
            anim.SetTrigger("Die");

            this.enabled = false;
        }

        StopCoroutine(HitCor());
        StartCoroutine(HitCor());
    }

    IEnumerator HitCor()
    {
        spriteR.color = highlightColor;
        yield return new WaitForSeconds(0.1f);
        spriteR.color = new Color(0,0,0,1);
    }
}
