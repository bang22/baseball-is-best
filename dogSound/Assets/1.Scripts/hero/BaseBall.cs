using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBall : Bullet
{
    public float radius;//반지름

    public float time;//시간 제한
    public float curTime;//지금 남은 시간

    public int reflecTime;//반사 제한
    private int curReflecTime;//반사 제한

    public LayerMask mask;//충돌체크 마스크
    public AnimationCurve curve;//속도랑 크기 변화


    private float curSpeed;//지금속도
    private Vector3 startScale;//처음 크기

    private SpriteRenderer spriteR;
    private Color curColor;

    private void Start()
    {
        curSpeed = speed;

        spriteR = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
    }

    public void Init(Vector2 pos, Vector2 dir, float speed, float time, int reflecTime)
    {
        this.reflecTime = reflecTime;
        curReflecTime = 0;

        transform.position = pos;
        this.dir = dir;
        this.speed = speed;
        curSpeed = speed;
        curColor = Color.white;

        this.time = time;
        curTime = time;

        lastcoll = null;
    }

    private Collider2D lastcoll;//마지막으로 만난 콜리다
    private Vector2 lastnormal;//마지막으로 만났던 노말

    private void LateUpdate()
    {
        curTime -= Time.deltaTime;
        if (curTime <= 0) gameObject.SetActive(false);

        curSpeed = Mathf.Lerp(0, speed, curve.Evaluate(curTime / time));

        curColor.a = Mathf.Lerp(0, 1, curve.Evaluate(curTime / time));
        spriteR.color = curColor;

        transform.localScale = Vector3.Lerp(Vector3.zero, startScale, curve.Evaluate(curTime / time));

        
        transform.Translate(dir * curSpeed * Time.deltaTime, Space.World);//움직임

        if (Mathf.Abs(transform.position.x) > SoccerStageManager.I().area.x || Mathf.Abs(transform.position.y) > SoccerStageManager.I().area.y) gameObject.SetActive(false);//사망원인


        //RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, Vector2.zero, radius + speed * Time.deltaTime, mask);//충돌체크 어디가서 끼이지 않게 충돌범위 넉넉하게 줌(속도에 따라서)
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + (dir * (radius * 0.5f)), dir, radius + curSpeed * Time.deltaTime, mask);
        if (hit)
        {
            if (lastnormal == hit.normal) return;//저번 노말과 같으면 리턴
            if (hit.collider.gameObject.layer == 14 && dir.y > 0) return;//플레이어 플랫폼에 부디치는데 올라가고 있다면

            lastnormal = hit.normal;

            //노말과 이동각도의 차이가 90도 이하면 무시하고 리턴
            float Bangle = Mathf.Atan2(dir.x, dir.y) * -180 / Mathf.PI;//공 각도
            float Nangle = Mathf.Atan2(lastnormal.x, lastnormal.y) * -180 / Mathf.PI;//충돌 각도
            //angle = Bangle;

            //Debug.Log(Bangle.ToString() + "  " + Nangle.ToString());
            //Debug.Log(Mathf.DeltaAngle(Bangle, Nangle));
            //Debug.Log(Mathf.Abs(Bangle - Nangle));
            //Debug.Break();

            if (Mathf.Abs(Bangle - Nangle) < 90) return;

            dir = dir + hit.normal * (-2 * Vector2.Dot(dir, hit.normal));//반사각 구함
            dir.Normalize();

            transform.Translate(dir * curSpeed * Time.deltaTime, Space.World);//추가 움직임

            //전에 부닷친게 이번이랑 같은 콜리더라면 방향은 꺽되 데미지는 주지말자 
            if (lastcoll != null && hit.collider.gameObject == lastcoll.gameObject) return;
            lastcoll = hit.collider;


            //적과의 조우
            if (hit.collider.gameObject.layer == 12)
                if (hit.collider.CompareTag("Boss"))//보스
                {
                    GameManager.I().boss.Hit(1);
                    GameManager.I().AddCombo(5);
                }
                else if (hit.collider.CompareTag("Barrier"))//커여운 배리어씨
                    hit.collider.GetComponent<Barrier>().Hit(1);



            //반사 횟수 제한
            curReflecTime++;
            if (curReflecTime >= reflecTime) gameObject.SetActive(false);


            GameManager.I().PlaySound(GameManager.I().sound_Ball_Hit);//사운드
        }
        else//뒤에서 오는 물체
        {
            hit = Physics2D.Raycast(transform.position * (-dir * (radius * 0.5f)), -dir, radius * 0.5f, mask);
            if (hit)
            {
                if (lastnormal == hit.normal) return;//저번 노말과 같으면 리턴

                lastnormal = hit.normal;

                dir = Vector2.Lerp(dir, hit.normal, 0.5f);
                dir.Normalize();
                //transform.position = (hit.point + dir * radius);
            }
        }

    }

}




//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Ball : MonoBehaviour
//{
//    public float speed;//속도
//    public float radius;//반지름
//    public LayerMask mask;//충돌체크 마스크

//    public Vector2 die;

//    [SerializeField]
//    private Vector2 dir;//날라가고 있는 방향


//    public float angle;

//    public void Init(Vector2 pos, Vector2 dir, float speed)
//    {
//        transform.position = pos;
//        this.dir = dir;
//        this.speed = speed;
//    }


//    private Vector2 lastnormal;//마지막으로 만났던 노말
//    private void LateUpdate()
//    {
//        if (angle != 0)
//        {
//            angle *= Mathf.Deg2Rad;//라디안 값으로 변환

//            //계산 후 노말라이즈
//            Vector2 vector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
//            dir = vector.normalized;
//            angle = 0;
//        }




//        transform.Translate(dir * speed * Time.deltaTime, Space.World);//움직임

//        if (Mathf.Abs(transform.position.x) > die.x || Mathf.Abs(transform.position.y) > die.y) gameObject.SetActive(false);//사망원인

//        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, dir, speed * Time.deltaTime, mask);//충돌체크 어디가서 끼이지 않게 충돌범위 넉넉하게 줌(속도에 따라서)

//        if (hit)
//        {
//            if (lastnormal == hit.normal) return;//저번 노말과 같으면 리턴
//            lastnormal = hit.normal;

//            //노말과 이동각도의 차이가 90도 이하면 무시하고 리턴
//            float Bangle = Mathf.Atan2(dir.x, dir.y) * -180 / Mathf.PI;//공 각도
//            float Nangle = Mathf.Atan2(lastnormal.x, lastnormal.y) * -180 / Mathf.PI;//충돌 각도
//            //angle = Bangle;

//            //Debug.Log(Bangle.ToString() + "  " + Nangle.ToString());
//            //Debug.Log(Mathf.DeltaAngle(Bangle, Nangle));
//            //Debug.Log(Mathf.Abs(Bangle - Nangle));
//            //Debug.Break();

//            if (Mathf.Abs(Bangle - Nangle) < 90) return;


//            dir = dir + hit.normal * (-2 * Vector2.Dot(dir, hit.normal));//반사각 구함
//            dir.Normalize();


//            transform.Translate(dir * -(radius - (Vector2.Distance(transform.position, hit.point))));//낑기지 마라
//        }
//    }
//}
