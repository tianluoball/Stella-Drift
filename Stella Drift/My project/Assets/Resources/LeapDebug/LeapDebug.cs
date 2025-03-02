using UnityEngine;
using Leap;

public class HandDebugWindow : MonoBehaviour
{
    private LeapProvider leapProvider;
    private Rect windowRect = new Rect(10, 10, 400, 500); // 增加窗口大小
    private bool showWindow = true;
    private Texture2D lineTexture;

    void Start()
    {
        leapProvider = FindFirstObjectByType<LeapProvider>();
        // 创建线条纹理
        lineTexture = new Texture2D(1, 1);
        lineTexture.SetPixel(0, 0, Color.white);
        lineTexture.Apply();
    }

    void OnGUI()
    {
        if (showWindow)
        {
            windowRect = GUILayout.Window(0, windowRect, DrawHandDebugWindow, "Hand Debug Window");
        }
    }

    void DrawHandDebugWindow(int windowID)
    {
        if (leapProvider == null)
        {
            GUILayout.Label("No Leap Provider found!");
            return;
        }

        Frame frame = leapProvider.CurrentFrame;

        // 创建绘图区域
        float drawAreaSize = 300;
        Rect drawArea = new Rect(50, 50, drawAreaSize, drawAreaSize);
        GUI.Box(drawArea, "Hand Visualization");

        foreach (Hand hand in frame.Hands)
        {
            string handType = hand.IsLeft ? "Left" : "Right";
            GUILayout.Label($"\n{handType} Hand Detected");

            // 绘制手掌中心
            Vector2 palmCenter = WorldToScreen(hand.PalmPosition, drawArea);
            DrawPoint(palmCenter, Color.red, 5);

            // 绘制每个手指
            foreach (Finger finger in hand.fingers)
            {
                // 获取每个骨节的位置
                Vector2[] positions = new Vector2[4];
                for (int i = 0; i < 4; i++)
                {
                    Bone bone = finger.GetBone((Bone.BoneType)i);
                    positions[i] = WorldToScreen(bone.NextJoint, drawArea);
                }

                // 绘制骨节连线
                Color fingerColor = GetFingerColor(finger.Type);
                for (int i = 0; i < 3; i++)
                {
                    DrawLine(positions[i], positions[i + 1], fingerColor, 2);
                }

                // 绘制关节点
                foreach (Vector2 pos in positions)
                {
                    DrawPoint(pos, fingerColor, 3);
                }
            }
        }

        // 显示实时数据
        GUILayout.BeginArea(new Rect(10, drawAreaSize + 60, windowRect.width - 20, windowRect.height - drawAreaSize - 70));
        if (frame.Hands.Count > 0)
        {
            foreach (Hand hand in frame.Hands)
            {
                GUILayout.Label($"Palm Position: {hand.PalmPosition:F1}");
                GUILayout.Label($"Palm Normal: {hand.PalmNormal:F1}");
                GUILayout.Label($"Grab Strength: {hand.GrabStrength:F2}");
            }
        }
        else
        {
            GUILayout.Label("No hands detected");
        }
        GUILayout.EndArea();

        if (GUILayout.Button("Close", GUILayout.Width(60)))
        {
            showWindow = false;
        }

        GUI.DragWindow();
    }

    Vector2 WorldToScreen(Vector3 position, Rect drawArea)
    {
        // 将Leap坐标系转换到屏幕坐标系
        return new Vector2(
            drawArea.x + (position.x + 150) * drawArea.width / 300,
            drawArea.y + drawArea.height - (position.y + 150) * drawArea.height / 300
        );
    }

    void DrawPoint(Vector2 position, Color color, float size)
    {
        Color oldColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(position.x - size/2, position.y - size/2, size, size), lineTexture);
        GUI.color = oldColor;
    }

    void DrawLine(Vector2 start, Vector2 end, Color color, float thickness)
    {
        Color oldColor = GUI.color;
        GUI.color = color;

        Vector2 direction = end - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float length = direction.magnitude;

        Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(angle, start);
        GUI.DrawTexture(new Rect(start.x, start.y - thickness/2, length, thickness), lineTexture);
        GUI.matrix = matrixBackup;

        GUI.color = oldColor;
    }

    Color GetFingerColor(Finger.FingerType fingerType)
    {
        switch (fingerType)
        {
            case Finger.FingerType.THUMB: return Color.red;
            case Finger.FingerType.INDEX: return Color.green;
            case Finger.FingerType.MIDDLE: return Color.blue;
            case Finger.FingerType.RING: return Color.yellow;
            case Finger.FingerType.PINKY: return Color.magenta;
            default: return Color.white;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            showWindow = !showWindow;
        }
    }

    void OnDestroy()
    {
        if (lineTexture != null)
        {
            Destroy(lineTexture);
        }
    }
}