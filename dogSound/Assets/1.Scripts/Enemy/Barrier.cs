using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{

    private Animator anim;

    public int maxHp;
    private int hp;
    

    void Start()
    {
        hp = maxHp;

        anim = GetComponent<Animator>();

        //anim.SetTrigger("Up");
    }


    public void Hit(int damage)
    {
        hp--;

        Debug.Log("adasd");
        anim.SetTrigger("Hit");

        if (hp <= 0)
        {
            anim.ResetTrigger("Hit");
            anim.ResetTrigger("Up");

            anim.SetTrigger("Down");

            StopCoroutine("RevSeq");
            StartCoroutine(RevSeq());

            return;
        }

    }
    
    IEnumerator RevSeq()//부활
    {
        yield return new WaitForSeconds(5);

        anim.SetTrigger("Up");
        hp = maxHp;
    }
}
