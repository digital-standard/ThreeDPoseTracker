using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using uWindowCapture;

public class PowerPointController
{
    public UwcWindowTexture uwcWindowTexture;


    [DllImport("user32.dll")]
    public static extern int PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    private const uint WM_LBUTTONDOWN = 0x0201;
    private const uint WM_LBUTTONUP = 0x0202;
    private const uint BM_CLICK = 0xF5;
    private const uint WM_CHAR = 0x0102;
    private const uint WM_KEYDOWN = 0x0100;
    private const uint WM_KEYUP = 0x0101;
    public static int GWL_STYLE = -16;

    IntPtr ButtonHandle = IntPtr.Zero;
    IntPtr MDIHandle = IntPtr.Zero;

    private uint MAKELPARAM(int p, int p_2)
    {
        return (uint)((p_2 << 16) | (p & 0xFFFF));
    }
    public PowerPointController(UwcWindowTexture window)
    {
        uwcWindowTexture = window;
    }

    public void Next()
    {
        var uwc = UwcManager.Find(uwcWindowTexture.partialWindowTitle, uwcWindowTexture.altTabWindow);

        var w = GetWindow(uwc.handle);
        MDIHandle = FindTargetMDIClient(w);
        PostMessage(MDIHandle, 0x0100, 0x0D, 0);
    }

    public void Prev()
    {
        var uwc = UwcManager.Find(uwcWindowTexture.partialWindowTitle, uwcWindowTexture.altTabWindow);

        var w = GetWindow(uwc.handle);
        ButtonHandle = FindTargetButton(w);
        SendMessage(ButtonHandle, WM_LBUTTONDOWN, 0, MAKELPARAM(0, 0));
        SendMessage(ButtonHandle, WM_LBUTTONUP, 0, MAKELPARAM(0, 0));
    }

    public static IntPtr FindTargetButton(Window top)
    {
        var all = GetAllChildWindows(top, new List<Window>());
        return all.Where(x => x.ClassName.Substring(0, Math.Min(x.ClassName.Length, 9)) == "F3 Server").First().hWnd;
    }

    public static IntPtr FindTargetMDIClient(Window top)
    {
        var all = GetAllChildWindows(top, new List<Window>());
        return all.Where(x => x.ClassName == "mdiClass").First().hWnd;
    }

    // 指定したウィンドウの全ての子孫ウィンドウを取得し、リストに追加する
    public static List<Window> GetAllChildWindows(Window parent, List<Window> dest)
    {
        dest.Add(parent);
        EnumChildWindows(parent.hWnd).ToList().ForEach(x => GetAllChildWindows(x, dest));
        return dest;
    }
    // 与えた親ウィンドウの直下にある子ウィンドウを列挙する（孫ウィンドウは見つけてくれない）
    public static IEnumerable<Window> EnumChildWindows(IntPtr hParentWindow)
    {
        IntPtr hWnd = IntPtr.Zero;
        while ((hWnd = FindWindowEx(hParentWindow, hWnd, null, null)) != IntPtr.Zero) { yield return GetWindow(hWnd); }
    }
    // ウィンドウハンドルを渡すと、ウィンドウテキスト（ラベルなど）、クラス、スタイルを取得してWindowsクラスに格納して返す
    public static Window GetWindow(IntPtr hWnd)
    {
        int textLen = GetWindowTextLength(hWnd);
        string windowText = null;
        if (0 < textLen)
        {
            //ウィンドウのタイトルを取得する
            StringBuilder windowTextBuffer = new StringBuilder(textLen + 1);
            GetWindowText(hWnd, windowTextBuffer, windowTextBuffer.Capacity);
            windowText = windowTextBuffer.ToString();
        }

        //ウィンドウのクラス名を取得する
        StringBuilder classNameBuffer = new StringBuilder(256);
        GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);

        //Debug.Log(windowText + "   " + classNameBuffer.ToString());
        // スタイルを取得する
        int style = GetWindowLong(hWnd, GWL_STYLE);
        return new Window() { hWnd = hWnd, Title = windowText, ClassName = classNameBuffer.ToString(), Style = style };
    }

    public class Window
    {
        public string ClassName;
        public string Title;
        public IntPtr hWnd;
        public int Style;
    }
}
