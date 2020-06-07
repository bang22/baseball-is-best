using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    Boss b;

    private void Awake()
    {
        b = transform.parent.GetComponent<Boss>();
    }

    public void Trigger()
    {
        b.AnimTrigger();
    }

    public void SoundTrigger()
    {
        b.SoundTrigger();
    }
}
