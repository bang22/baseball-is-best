using UnityEngine;
using System.Collections;


[ExecuteInEditMode]//에디터모드일때도 돌아감
[RequireComponent(typeof(Camera))]//카메라가 있어야만 추가가능
public class SimpleBlit : MonoBehaviour
{
    public Texture transitionTex;
    public Color screenColor;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float cutOff;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float fade;

    public Material m_material;
    Material material
    {
        get
        {
            if (m_material == null)
            {
                Shader shader = Shader.Find("Custom/BattleTransitions");

                m_material = new Material(shader);
                m_material.hideFlags = HideFlags.DontSave;
            }
            return m_material;
        }
    }

    void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    public void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material)
        {
            material.SetTexture("_TransitionTex", transitionTex);//팔레트 추가
            material.SetColor("_Color", screenColor);//팔레트 추가
            material.SetFloat("_Cutoff", cutOff);//팔레트 추가
            material.SetFloat("_Fade", fade);//팔레트 추가

            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    void OnDisable()
    {
        if (m_material)
        {
            Material.DestroyImmediate(m_material);
        }
    }


    public void Play(float speed,AnimationCurve curve)
    {
        StopAllCoroutines();
        StartCoroutine(CloseEffect(speed,curve));
    }


    IEnumerator CloseEffect(float speed , AnimationCurve curve)//카메라 꺼지는 연출
    {
        float progress = 0;
        while (true)
        {
            progress += Time.unscaledDeltaTime * speed;
            
            cutOff = Mathf.Lerp(-0.1f, 1, curve.Evaluate(progress));

            if (progress > 1) yield break;

            yield return 0;
        }
    }
}