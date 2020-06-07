using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class hero : MonoBehaviour
{
    enum DragState
    {
        MOVE,
        SLIDE,
        SHOOT
    }
    
    public float moveSpeed;//움직이는 속도
    private const float moveArea = 6;

    public float shootAngle;//발사하는 각도
    public float shootSpeed;//발사간격

    public float ballSpeed;//공 속도
    public float ballTime;//공 수명
    public int ballReflecTime;//공 반사수명


    [Header("부속품")]
    [SerializeField]
    private Transform arrow;//발사 궤적을 보여주는 화살표
    [SerializeField]
    private Transform ShootPoint;//발사 시작 위치
    [SerializeField]
    private GameObject Shield;

    [Header("물리")]
    [SerializeField]
    private Collider2D coll;//플레이어의 영역구분 콜리더
    [SerializeField]
    private LayerMask playerMask;//플레이어 레이어 마스크


    private int MoveID = -1;
    private float startTime=0;
    private bool isDash = false;
    private int ShootID = -1;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    //private void Update()
    //{
    //    Touch t;
    //    Vector2 tPos;
    //    for (int i = 0; i < Input.touchCount; i++)
    //    {
    //        t = Input.GetTouch(i);
    //        tPos = Camera.main.ScreenToWorldPoint(t.position);//터치한 포지션

    //        switch (t.phase)
    //        {
    //            case TouchPhase.Began:
    //                if (ShootID == -1)
    //                    if (Physics2D.OverlapPoint(tPos, playerMask))
    //                    {
    //                        ShootID = t.fingerId;
    //                        Shield.SetActive(false);
    //                        break;//움직이기와 발사는 동시에 이뤄질 수 없는 꿈
    //                    }

    //                if (MoveID == -1)//움직임에 사용하는 터치가 비어있고 조건이 맞으면 새로 넣어줌
    //                    if (Screen.height * 0.25f > t.position.y)//4분에 1 지점
    //                    {
    //                        MoveID = t.fingerId;
    //                        if (Time.time-startTime < 0.2f)
    //                        {
    //                            Debug.Log(startTime - Time.time);
    //                            isDash = true;
    //                        }
    //                        else
    //                            startTime = Time.time;
    //                    }

    //                break;

    //            case TouchPhase.Ended:

    //                if (t.fingerId == MoveID)//움직이기
    //                {
    //                    MoveID = -1;
    //                }

    //                if (t.fingerId == ShootID)//발사
    //                {
    //                    Shield.SetActive(true);
    //                    ShootID = -1;
    //                }

    //                break;


    //            default:

    //                if (t.fingerId == MoveID)//움직이기
    //                {
    //                    Move(tPos.x);
    //                }

    //                if (t.fingerId == ShootID)//발사
    //                {
    //                    float degree = Mathf.Atan2(ShootPoint.position.y - tPos.y, ShootPoint.position.x - tPos.x) * 180 / Mathf.PI + 90;//발사 각도

    //                    Shoot(degree);
    //                }

    //                break;
    //        }

    //    }

    //}

    private void Update()
    {
        //Debug.Log(input);
        //anim.SetFloat("dir", input + 0.5f);
        //anim.SetFloat("speed", Mathf.Abs(input));


        float input = GameManager.I().MoveJoyStick.Horizontal;
        if (input == 0) input = Input.GetAxis("Horizontal");

        Move(input);

        anim.SetFloat("Speed", input);

        if (input != 0)
        {
            Aim(input);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();


        //if(input == 0)
        //{
        //    anim.SetTrigger("idle");
        //}
        //else
        //{
        //    anim.SetTrigger("walk");
        //    anim.speed = Mathf.Abs(input);
        //    Move(input);

        //}
    }

    private void Move(float target)//움직임
    {
        Vector2 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -moveArea, moveArea);
        //if (Mathf.Abs(pos.x - target) < moveSpeed * (isDash ? 5 : 1) * Time.deltaTime) return;//데드존

        //if (target < pos.x)
        //    pos.x -= moveSpeed * (isDash ? 5 : 1) * Time.deltaTime;
        //else
        //    pos.x += moveSpeed * (isDash ? 5 : 1) * Time.deltaTime;

        pos.x += moveSpeed * target * Time.deltaTime;

        transform.position = pos;
    }


    //private float ShootTime;

    //private float degree;

    //private void Aim(float d)//발사 준비
    //{
    //    degree += d; 


    //    //degree *= -shootAngle;
    //    degree = Mathf.Clamp(degree, -shootAngle, shootAngle);//회전 각 제한

    //    arrow.rotation = Quaternion.Euler(0, 0, degree);//회전
    //}

    //public void Shoot(float d)
    //{
    //    anim.SetTrigger("Shoot");

    //    degree += d;

    //    //degree *= -shootAngle;
    //    BaseBall slime = SoccerStageManager.I().GetSlime();//슬라임 가지고 오기

    //    //degree -= 90;//계산을 편하게 하기 위해 더해두었던 90도 다시 빼기

    //    Debug.Log(degree);
    //    float rad = (degree- 90) * Mathf.Deg2Rad;//라디안 값으로 변환

    //    //계산 후 노말라이즈
    //    Vector2 vector = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    //    vector = vector.normalized;

    //    slime.Init(ShootPoint.position, -vector, ballSpeed, ballTime);//위치,방향,속도 초기화
    //}


    private float degree;
   private void Aim(float degree)//발사 준비
   {
       degree *= -shootAngle;
       degree = Mathf.Clamp(degree, -shootAngle, shootAngle);//회전 각 제한

        this.degree = degree;

       arrow.rotation = Quaternion.Euler(0, 0, degree);//회전
   }

    //float t=0;
    //private void ShootTimer(float degree)
    //{
    //    t += Time.deltaTime;
    //    if (t >= shootSpeed)
    //    {
    //        t = 0;
    //        Shoot(degree);
    //    }
    //}

   private void Shoot()
   {
        float d = degree;
        BaseBall baseBall = GameManager.I().GetBaseBall();//공 가지고 오기


        if (baseBall == null) return;//공을 더 이상 발사할 수 없으면 그만

        anim.SetTrigger("Shoot");


        d -= 90;//계산을 편하게 하기 위해 더해두었던 90도 다시 빼기
        d *= Mathf.Deg2Rad;//라디안 값으로 변환

       //계산 후 노말라이즈
       Vector2 vector = new Vector2(Mathf.Cos(d), Mathf.Sin(d));
       vector = vector.normalized;

        baseBall.Init(ShootPoint.position, -vector, ballSpeed, ballTime, ballReflecTime);//위치,방향,속도,반사 횟수 초기화

        GameManager.I().PlaySound(GameManager.I().sound_Ball_Shoot);//발사 소리
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.I().PlayerDie();//hp 깍는다
    }
}
