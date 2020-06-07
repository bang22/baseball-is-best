using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    protected Vector2 dir;

    [SerializeField]
    protected float speed=0;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Init(Vector2 dir, float speed,bool shoot,float animspeed=1)
    {
        this.dir = dir;
        this.speed = speed;

        if(anim)
            anim.speed = animspeed;

        if (shoot)
            anim.SetTrigger("Shoot");
    }

    void LateUpdate()
    {
        transform.position += (Vector3)dir * speed * Time.deltaTime;

        if (Mathf.Abs(transform.position.x) > SoccerStageManager.I().area.x || Mathf.Abs(transform.position.y) > SoccerStageManager.I().area.y)
            gameObject.SetActive(false);//사망원인
    }


    public bool CollOverLap(Collider2D coll)
    {
        Debug.Log(coll.name);
        return coll.OverlapPoint(transform.position);
    }
}
