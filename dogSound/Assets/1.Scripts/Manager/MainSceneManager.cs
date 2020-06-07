using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{

    public SimpleBlit blit;
    public AnimationCurve curve;

    public void ChangeScene(string name)
    {
        blit.Play(2, curve);
        StartCoroutine(Scene(name));
    }

    IEnumerator Scene(string name)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
