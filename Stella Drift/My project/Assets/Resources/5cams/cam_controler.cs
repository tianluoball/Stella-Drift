using UnityEngine;

public class MultiCameraRenderer : MonoBehaviour
{
    [System.Serializable]
    public struct CameraResolution
    {
        public int width;
        public int height;
        
        public CameraResolution(int w, int h)
        {
            width = w;
            height = h;
        }
    }

    [Header("Resolution Settings")]
    public CameraResolution singleCameraResolution = new CameraResolution(256, 512);
    public int cameraCount = 5;
    
    [Header("Camera Settings")]
    public Camera[] layeredCameras = new Camera[5];
    public float layerSpacing = 0.1f;

    private void Awake()
    {
        SetupCameras();
    }

    private void SetupCameras()
    {
        float viewportWidth = 1f / cameraCount;
        
        for (int i = 0; i < cameraCount; i++)
        {
            if (layeredCameras[i] == null)
            {
                GameObject cameraObj = new GameObject($"LayerCamera_{i}");
                cameraObj.transform.parent = transform;
                layeredCameras[i] = cameraObj.AddComponent<Camera>();
            }
            
            Camera cam = layeredCameras[i];
            
            // 基础设置
            cam.depth = i;
            cam.rect = new Rect(i * viewportWidth, 0, viewportWidth, 1);
            
            // 位置设置
            if (i == 0)
            {
                cam.transform.position = transform.position;
                cam.transform.rotation = transform.rotation;
            }
            else
            {
                cam.transform.position = layeredCameras[0].transform.position + 
                    Vector3.forward * (i * layerSpacing);
                cam.transform.rotation = layeredCameras[0].transform.rotation;
            }
        }
    }

    private void OnDestroy()
    {
        // 清理相机对象
        if (layeredCameras != null)
        {
            foreach (var cam in layeredCameras)
            {
                if (cam != null && cam.gameObject != gameObject)
                {
                    DestroyImmediate(cam.gameObject);
                }
            }
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        
        UnityEditor.EditorApplication.delayCall += () => {
            if (this == null) return;
            SetupCameras();
            SetGameViewResolution();
        };
    }

    private void SetGameViewResolution()
    {
        var assembly = typeof(UnityEditor.EditorWindow).Assembly;
        var gameViewType = assembly.GetType("UnityEditor.GameView");
        var gameView = UnityEditor.EditorWindow.GetWindow(gameViewType);

        // 计算总分辨率
        int totalWidth = singleCameraResolution.width * cameraCount;
        int totalHeight = singleCameraResolution.height;

        // 设置Game视图分辨率
        var serializedObject = new UnityEditor.SerializedObject(gameView);
        var gameViewSize = gameViewType.GetProperty("currentGameViewSize",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (gameViewSize != null)
        {
            var gvsize = gameViewSize.GetValue(gameView, null);
            var gvSizeType = gvsize.GetType();

            var width = gvSizeType.GetProperty("width");
            var height = gvSizeType.GetProperty("height");
            
            if (width != null && height != null)
            {
                width.SetValue(gvsize, totalWidth, null);
                height.SetValue(gvsize, totalHeight, null);
            }
        }

        serializedObject.ApplyModifiedProperties();
        gameView.Repaint();
    }
    #endif
}