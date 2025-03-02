using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WindowController : MonoBehaviour
{
    public Vector2 windowSize = new Vector2(1280, 720);

    // Windows API 相关常量
    private const int GWL_STYLE = -16;
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_VISIBLE = 0x10000000;
    private const int HWND_TOP = 0;
    private const uint SWP_SHOWWINDOW = 0x0040;

    void Start()
    {
        // 设置窗口为无边框窗口模式
        Screen.fullScreen = false;
        Screen.SetResolution((int)windowSize.x, (int)windowSize.y, FullScreenMode.Windowed);

        #if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            // 获取窗口句柄
            IntPtr handle = GetActiveWindow();
            
            // 移除窗口边框
            SetWindowLong(handle, GWL_STYLE, WS_POPUP | WS_VISIBLE);
            
            // 设置窗口位置和大小
            SetWindowPos(handle, (IntPtr)HWND_TOP, 0, 0, 
                        (int)windowSize.x, (int)windowSize.y, 
                        SWP_SHOWWINDOW);
        #endif
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        #if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        if (Input.GetMouseButton(0))
        {
            ReleaseCapture();
            SendMessage(GetActiveWindow(), 0xA1, 0x2, 0);
        }
        #endif
    }

    #if !UNITY_EDITOR && UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, 
        int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
    #endif
}