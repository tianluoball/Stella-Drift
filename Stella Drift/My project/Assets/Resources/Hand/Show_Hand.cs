using UnityEngine;
using Leap;

public class HandVisualizer : MonoBehaviour
{
    private LeapProvider provider;
    public GameObject targetObject;
    
    [Header("Debug Settings")]
    public bool showDebugSphere = true;
    private GameObject debugSphere;
    
    [Header("Leap Motion Tracking Range")]
    [Tooltip("最小跟踪范围（左下前）")]
    public Vector3 leapMin = new Vector3(-3f, 0.7f, -5f);
    [Tooltip("最大跟踪范围（右上后）")]
    public Vector3 leapMax = new Vector3(-2.5f, 1.1f, -4.3f);
    
    [Header("Target Mapping Range")]
    [Tooltip("目标空间最小范围（左下前）")]
    public Vector3 targetMin = new Vector3(-5f, -5f, 0f);
    [Tooltip("目标空间最大范围（右上后）")]
    public Vector3 targetMax = new Vector3(5f, 5f, 10f);
    
    void Start()
    {
        provider = FindFirstObjectByType<LeapProvider>();
        if (provider == null)
        {
            Debug.LogError("Scene is missing a LeapProvider!");
            enabled = false;
            return;
        }
        
        if (targetObject == null)
        {
            Debug.LogWarning("Target object is not assigned!");
        }

        if (showDebugSphere)
        {
            debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugSphere.name = "DebugSphere";
            debugSphere.transform.localScale = Vector3.one * 0.1f;
            Renderer renderer = debugSphere.GetComponent<Renderer>();
            renderer.material.color = Color.red;
            
            Collider collider = debugSphere.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
    
    Vector3 MapToTargetSpace(Vector3 leapPosition)
    {
        // 将坐标归一化到 0-1 范围
        Vector3 normalizedPos = new Vector3(
            Mathf.InverseLerp(leapMin.x, leapMax.x, leapPosition.x),
            Mathf.InverseLerp(leapMin.y, leapMax.y, leapPosition.y),
            Mathf.InverseLerp(leapMin.z, leapMax.z, leapPosition.z)
        );
        
        // 将归一化坐标映射到目标空间
        return new Vector3(
            Mathf.Lerp(targetMin.x, targetMax.x, normalizedPos.x),
            Mathf.Lerp(targetMin.y, targetMax.y, normalizedPos.y),
            Mathf.Lerp(targetMin.z, targetMax.z, normalizedPos.z)
        );
    }
    
    void Update()
    {
        Frame frame = provider.CurrentFrame;
        if (frame != null && frame.Hands.Count > 0)
        {
            foreach (Hand hand in frame.Hands)
            {
                if (hand.IsRight)
                {
                    Vector3 palmPosition = new Vector3(
                        hand.PalmPosition.x,
                        hand.PalmPosition.y,
                        hand.PalmPosition.z
                    );
                    
                    Vector3 mappedPosition = MapToTargetSpace(palmPosition);
                    //Debug.Log($"原始位置: {palmPosition}, 映射后位置: {mappedPosition}");
                    
                    if (showDebugSphere && debugSphere != null)
                    {
                        debugSphere.transform.position = mappedPosition;
                        debugSphere.SetActive(true);
                    }
                    
                    if (targetObject != null)
                    {
                        targetObject.transform.position = mappedPosition;
                        targetObject.SetActive(true);
                    }
                    return;
                }
            }
        }
        
        if (showDebugSphere && debugSphere != null)
        {
            debugSphere.SetActive(false);
        }
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (debugSphere != null)
        {
            Destroy(debugSphere);
        }
    }
}