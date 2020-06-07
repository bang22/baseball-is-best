using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerChild : Bullet
{
    public void AnimTrigger()
    {
        GameManager.I().PlaySound((GameManager.I() as SoccerStageManager).sound_BossChild_Foot);
    }
}
