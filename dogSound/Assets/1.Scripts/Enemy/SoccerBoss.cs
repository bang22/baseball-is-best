using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//public enum SoccerBossAnimState
//{
//    IDLE,//0
//    WALK,//1
//    HEADING,//2
//    KICK,//3
//    DASH,//4
//    STOMP//5
//}



[System.Serializable]
public class SoccerHeading : BossPattern//헤딩 패턴
{
    public float ballSpeed;

    Boss boss;
    
    public override void ActBegin()//액션 시작
    {
        isNewBall = false;
        boss = GameManager.I().boss;
        startPos.x = Mathf.Sign(SoccerStageManager.I().player.position.x) * 4.5f;
    }

    public override void ActAllWay()//업데이트
    {
        Vector2 bp = boss.transform.position;
        Vector2 pp = boss.transform.position;
        pp.x = SoccerStageManager.I().player.position.x;

        boss.transform.position = Vector2.MoveTowards(bp, pp,5 * Time.deltaTime);
    }

    bool isNewBall;
    Bullet ball;
    public override void ActTrigger()//새 공을 생성하거나 발사하거나
    {
        isNewBall = !isNewBall;
        if (isNewBall)//공 생성
        {
            GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Wind);
            ball = (GameManager.I() as SoccerStageManager).GetSoccerBall();
            ball.transform.parent = boss.transform;
            ball.transform.position = GameManager.I().boss.transform.position + (Vector3.up * 10);
            ball.Init(Vector2.zero, 0,false);

            return;
        }
        //발사
        ball.transform.parent = null;
        ball.Init(Vector2.down, ballSpeed,true);
        GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Heading);
    }
    
    public override void ActSoundTrigger()//걷는 사운드 재생
    {
        GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Foot);
    }
}


[System.Serializable]
public class SoccerKick : BossPattern
{
    public float ballSpeed;

    Boss boss;

    public override void ActBegin()
    {
        boss = GameManager.I().boss;
        isNewChild = false;
    }

    bool isNewChild;
    Bullet soccerC;
    public override void ActTrigger()//새 칠드런을 생성하거나 발사하거나
    {
        //자식 만들기
        isNewChild = !isNewChild;
        if (isNewChild)
        {
            soccerC = (GameManager.I() as SoccerStageManager).GetSoccerChild();
            soccerC.Init(Vector2.zero, 0,false);
            soccerC.transform.position = Vector2.zero;

            GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_BossChild_Dirt);//자식 나오는 소리
            return;
        }
        
        //발차기
        Vector2 dir = (SoccerStageManager.I().player.position - soccerC.transform.position).normalized;
        soccerC.Init(dir, ballSpeed,true);
    }

    public override void ActSoundTrigger()
    {
        GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Kick);//발차는 사운드
    }

}


[System.Serializable]
public class SoccerDash : BossPattern//대쉬 패턴
{
    public bool isStartOnPlayer;

    bool Dash;
    public override void ActBegin()//시작 위치
    {
        Dash = true;
        if (!isStartOnPlayer)
            startPos.x = Mathf.Sign(SoccerStageManager.I().player.position.x) * 4.5f;
        else
            startPos.x = GameManager.I().player.position.x;
    }

    public override void ActSoundTrigger()//걷는 소리
    {
        GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Foot);
    }

    public override void ActTrigger()
    {
        if (Dash) GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Dash);
        else GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Landing);
        Dash = false;
    }

    public override void ActEnd()
    {
        GameManager.I().KillBaseBall(GameManager.I().boss.coll);//끝나고 남아있는 자리에 있는 공 삭제
    }
}


[System.Serializable]
public class SoccerStomping : BossPattern//우당탕패턴
{
    public int ballCount;
    public float ballSpeed;

    Boss boss;

    public override void ActBegin()//액션 시작
    {
        isNewBall = false;
        boss = GameManager.I().boss;
        startPos = boss.GetRandomArea(0);
    }

    Vector2 bossDest;
    public override void ActAllWay()//업데이트
    {
        Vector2 bp = boss.transform.position;

        boss.transform.position = Vector2.MoveTowards(bp, bossDest, 5 * Time.deltaTime);
    }

    bool isNewBall;
    Bullet[] balls;
    public override void ActTrigger()//새 공을 생성하거나 발사하거나
    {
        isNewBall = !isNewBall;

        Vector2 dest;
        if (isNewBall)//공 생성
        {
            GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Wind);

            balls = new Bullet[ballCount];
            for (int i = 0; i < balls.Length; i++)
            {
                Bullet ball = (GameManager.I() as SoccerStageManager).GetSoccerChild();
                dest = GameManager.I().boss.transform.position;
                dest.x += -(balls.Length/2 * 4) + (i * 4);
                ball.transform.position = dest;

                ball.Init(Vector2.zero, 0,false,2);
                balls[i] = ball;
            }
            return;
        }

        //발4
        for (int i = 0; i < balls.Length; i++)
        {
            dest = GameManager.I().player.position;
            dest.x += -(balls.Length / 2 * 4) + (i * 4);

            balls[i].Init( (dest-(Vector2)boss.transform.position).normalized, ballSpeed,true);
        }
        
        bossDest = boss.GetRandomArea(0);

        //발사

        //GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Heading);
    }

    public override void ActSoundTrigger()//사운드 재생
    {
        //GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_Boss_Foot);
    }
}





public class SoccerBoss : Boss
{
    [Header("일반모드")]
    [SerializeField]
    SoccerHeading heading;
    [SerializeField]
    SoccerKick kick;
    [SerializeField]
    SoccerDash dash;
    [SerializeField]
    SoccerStomping stomp;


    [Header("분노모드")]
    [SerializeField]
    SoccerHeading RageHeading;
    [SerializeField]
    SoccerKick RageKick;
    [SerializeField]
    SoccerDash RageDash;
    [SerializeField]
    SoccerStomping RageStomp;

    private void Awake()
    {
        patterns = new BossPattern[4];
        patterns[0] = heading;
        patterns[1] = kick;
        patterns[2] = dash;
        patterns[3] = stomp;


        ragePatterns = new BossPattern[4];
        ragePatterns[0] = RageHeading;
        ragePatterns[1] = RageKick;
        ragePatterns[2] = RageDash;
        ragePatterns[3] = RageStomp;


        //patterns = new BossPattern[1];
        //patterns[0] = stomp;

        //RagePatterns = new BossPattern[1];
        //RagePatterns[0] = RageStomp;
    }

}
