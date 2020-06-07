using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerStageManager : GameManager
{
    //사운드 이름
    [Header("사운드 이름")]
    [Space(10)]
    [Header("축구 보스 스테이지")]
    [Space(10)]

    [FMODUnity.EventRef] public string sound_Boss_Foot;
    [FMODUnity.EventRef] public string sound_Boss_Heading;
    [FMODUnity.EventRef] public string sound_Boss_Kick;
    [FMODUnity.EventRef] public string sound_Boss_Wind;
    [FMODUnity.EventRef] public string sound_Boss_Dash;
    [FMODUnity.EventRef] public string sound_Boss_Landing;

    [FMODUnity.EventRef] public string sound_BossChild_Foot;
    [FMODUnity.EventRef] public string sound_BossChild_Dirt;


    //원본
    [Header("원본 프리팹들")]
    [SerializeField] private GameObject soccerBallOrigin;
    [SerializeField] private GameObject soccerChildOrigin;

    //오브젝트 풀
    private List<Bullet> soccerBall = new List<Bullet>();
    private List<Bullet> soccerChild = new List<Bullet>();

    public Bullet GetSoccerBall()
    {
        return GetItem(soccerBall, soccerBallOrigin).GetComponent<Bullet>();
    }

    public Bullet GetSoccerChild()
    {
        return GetItem(soccerChild, soccerChildOrigin).GetComponent<Bullet>();
    }
}
