using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class DepthSortByY : MonoBehaviour
{
    private const int IsometricRangePerYUnit = 100;
    
    //이 객체를 이용해 z order를 결정함
    public Transform Target;
    
    //타겟과의 거리
    public int TargetOffset = 0;

    private Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Target == null)
            Target = transform;

        renderer.sortingOrder = -(int)(Target.position.y * IsometricRangePerYUnit) + TargetOffset;
    }
}