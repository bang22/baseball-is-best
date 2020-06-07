using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region 싱글톤
    private static GameManager instance;
    public static GameManager I()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            if (!instance)
                Debug.LogError("버그났데여 ㅋ ㅋ ㅋ ㅋ ㅋ \n\n씬파일에 게임매니저있는지 확인!");
        }

        return instance;
    }
    #endregion

    [Header("조이스틱")]
    public Joystick MoveJoyStick;

    [Header("총알,플레이어 영역")]
    public Vector2 area;

    [Header("보스,플레이어")]
    public Boss boss;
    public Transform player;


    //스탯
    [Header("스탯")]
    public int BossMaxHP;
    private int BossHP;

    public int BossMaxCombo;
    private int BossCombo;

    public int maxBaseBall;//플레이어가 쓸 수 있는 공 최대


    //UI
    [Header("UI")]
    [SerializeField]
    private Image HPTop;//hp의 뻘건 부분
    [SerializeField]
    private Image HPMid;//hp의 중간 부분


    [SerializeField]
    private Image ComboTop;//콤보의 뻘건 부분
    [SerializeField]
    private Image ComboMid;//콤보의 중간 부분


    public SimpleBlit blit;
    public AnimationCurve fadeInCurve;
    public AnimationCurve fadeOutCurve;

    FMOD.Studio.EventInstance bgm;
    private void Start()
    {
        bgm = PlaySound(sound_BGM);


        BossHP = BossMaxHP;
        BossCombo = BossMaxCombo;

        StartCoroutine(StartSeq());
    }

    IEnumerator StartSeq()
    {
        blit.Play(2, fadeInCurve);

        yield return new WaitForSeconds(1);

        boss.Init();
        StartCoroutine(ComboReduce());
    }


    #region UI

    public GameObject gameOver;
    public Image result;
    public Sprite win;
    public Sprite lose;

    public void PlayerDie()
    {
        gameOver.SetActive(true);
        result.sprite = lose;
    }

    public void playerWin()
    {
        gameOver.SetActive(true);
        result.sprite = win;
    }


    public int ReduceHP(int hp)//보스 Hp깍기
    {
        BossHP -= hp;
        
        StopCoroutine("HPUIEffect");
        StartCoroutine(HPUIEffect((float)BossHP / (float)BossMaxHP));

        //반피 까이면 분노모드
        if(BossHP <= BossMaxHP * 0.5f)
            bgm.setParameterValue("Rage", 1);

        //승리
        if (BossHP <= 0)
            playerWin();

        return BossHP;
    }

    IEnumerator HPUIEffect(float target)//보스 Hp UI 애니메이션
    {
        float cur = HPTop.fillAmount;
        float progress = 0;
        while (true)
        {
            yield return 0;

            progress += 2*Time.deltaTime;

            if(progress < 1) HPTop.fillAmount = Mathf.Lerp(cur, target,progress);
            else HPMid.fillAmount = Mathf.Lerp(cur, target, progress-1);

            if (progress > 2) break;
        }
    }

    public void AddCombo(int combo)
    {
        BossCombo += combo;

        if (BossCombo >= BossMaxCombo) BossCombo = BossMaxCombo;

        StopCoroutine("ComboUIEffect");
        StartCoroutine(ComboUIEffect((float)BossCombo / (float)BossMaxCombo));
    }

    IEnumerator ComboReduce()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            BossCombo -= 1;
            if(BossCombo <= 0)
            {
                BossCombo = 0;
                continue;
            }

            StopCoroutine("ComboUIEffect");
            StartCoroutine(ComboUIEffect((float)BossCombo / (float)BossMaxCombo));
        }
    }

    IEnumerator ComboUIEffect(float target)//보스 Hp UI 애니메이션
    {
        float cur = ComboTop.fillAmount;
        float progress = 0;
        while (true)
        {
            yield return 0;

            progress +=  Time.deltaTime;

            ComboTop.fillAmount = Mathf.Lerp(cur, target, progress);

            if (progress > 2) break;
        }
    }

    //IEnumerator ComboUIEffect(float target)//보스 Hp UI 애니메이션
    //{
    //    float cur = ComboTop.fillAmount;
    //    float progress = 0;
    //    while (true)
    //    {
    //        yield return 0;

    //        progress += 2 * Time.deltaTime;

    //        if (progress < 1) ComboMid.fillAmount = Mathf.Lerp(cur, target, progress);
    //        else ComboTop.fillAmount = Mathf.Lerp(cur, target, progress - 1);

    //        if (progress > 2) break;
    //    }
    //}


    #endregion

    //오브젝트 풀
    #region ObjectPool
    [Header("원본 프리팹들")]
    [SerializeField] private GameObject baseBallOrigin;

    private List<Bullet> baseBalls = new List<Bullet>();


    //오브젝트 풀에서 아이템 가지고 오기
    protected Bullet GetItem(List<Bullet> pool, GameObject origin, int max = 0)
    {
        Bullet obj = null;
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
            {
                obj = pool[i];
                break;
            }
        }

        if (!obj)
        {
            if (max != 0 && pool.Count > max) return null;//최대치가 정해져있으면 그만

            obj = Instantiate(origin).GetComponent<Bullet>();
            pool.Add(obj);
        }

        obj.gameObject.SetActive(true);

        return obj;
    }

    public BaseBall GetBaseBall()//야구공 가지고 오기
    {
        return GetItem(baseBalls, baseBallOrigin, maxBaseBall) as BaseBall;
    }

    public void KillBaseBall(Collider2D coll)
    {
       //Debug.Log("KKIIINNNOLLL");
        for (int i = 0; i < baseBalls.Count; i++)
        {
            if (!baseBalls[i].gameObject.activeInHierarchy)//공이 살아있으면
            {
                baseBalls[i].gameObject.SetActive(baseBalls[i].CollOverLap(coll));//콜리더랑 겹쳐있는지 확인하고 킬
            }
        }
    }
    #endregion


    //fmod
    #region Sound

    public FMOD.Studio.EventInstance PlaySound(string name)
    {
        FMOD.Studio.EventInstance sound = new FMOD.Studio.EventInstance();
        sound = FMODUnity.RuntimeManager.CreateInstance(name);
        sound.start();

        return sound;
    }

    [Header("사운드 이름")]

    [FMODUnity.EventRef] public string sound_BGM;
    [FMODUnity.EventRef] public string sound_Ball_Shoot;
    [FMODUnity.EventRef] public string sound_Ball_Hit;
    #endregion



    public void ChangeScene(string name)
    {
        blit.Play(2,fadeOutCurve);
        StartCoroutine(Scene(name));
    }

    IEnumerator Scene(string name)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(name);
    }


}
