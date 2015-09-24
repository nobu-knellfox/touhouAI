using System.Windows;
using System.Windows.Forms;
//using Microsoft.Win32;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

namespace TouhouAIUI
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public short wScan;
            public uint dwFlags;
            public uint time;
            public  IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)]
            public uint type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        // 仮想キーコードをスキャンコードに変換
        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private extern static int MapVirtualKey(
            int wCode, int wMapType);

        private const int INPUT_MOUSE = 0;                  // マウスイベント
        private const int INPUT_KEYBOARD = 1;               // キーボードイベント
        private const int INPUT_HARDWARE = 2;               // ハードウェアイベント

        private const int MOUSEEVENTF_MOVE = 0x1;           // マウスを移動する
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;    // 絶対座標指定
        private const int MOUSEEVENTF_LEFTDOWN = 0x2;       // 左　ボタンを押す
        private const int MOUSEEVENTF_LEFTUP = 0x4;         // 左　ボタンを離す
        private const int MOUSEEVENTF_RIGHTDOWN = 0x8;      // 右　ボタンを押す
        private const int MOUSEEVENTF_RIGHTUP = 0x10;       // 右　ボタンを離す
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;    // 中央ボタンを押す
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;      // 中央ボタンを離す
        private const int MOUSEEVENTF_WHEEL = 0x800;        // ホイールを回転する
        private const int WHEEL_DELTA = 120;                // ホイール回転値

        private const int KEYEVENTF_KEYDOWN = 0x0;          // キーを押す
        private const int KEYEVENTF_KEYUP = 0x2;            // キーを離す
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;      // 拡張コード
        private const int KEYEVENTF_SCANCODE = 0x0008;
        private const int VK_SHIFT = 0x10;                  // SHIFTキー
        private const int VK_RETURN = 0x0D;                 // ENTER
        private const int VK_UP = 0x26;                     // UP
        private const int VK_RIGHT = 0x27;                  // RIGHT
        private const int VK_LEFT = 0x25;                   // LEFT
        private const int VK_DOWN = 0x26;                   // DOWN

        private OpenFileDialog ofd;
        System.Diagnostics.Process p;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
             p = System.Diagnostics.Process.Start(ofd.FileName);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ofd =  new OpenFileDialog();
            ofd.FileName = "";
            ofd.DefaultExt = "*.*";
            ofd.ShowDialog();
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            Interaction.AppActivate(p.Id);
            System.Threading.Thread.Sleep(30);

            const int num = 2;
            INPUT[] inp = new INPUT[num];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_UP;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_SCANCODE;
            inp[0].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[0].ki.time = 0;

            SendInput(1, ref inp[0], Marshal.SizeOf(inp[0]));

            System.Threading.Thread.Sleep(20);

            inp[1].type = INPUT_KEYBOARD;
            inp[1].ki.wVk = VK_UP;
            inp[1].ki.wScan = (short)MapVirtualKey(inp[1].ki.wVk, 0);
            //inp[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp[1].ki.dwFlags = KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP;
            inp[1].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[1].ki.time = 0;

            SendInput(1, ref inp[1], Marshal.SizeOf(inp[0]));
            //System.Threading.Thread.Sleep(10);
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            Interaction.AppActivate(p.Id);
            System.Threading.Thread.Sleep(100);

            const int num = 2;
            INPUT[] inp = new INPUT[num];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_RETURN;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[0].ki.time = 0;


            inp[1].type = INPUT_KEYBOARD;
            inp[1].ki.wVk = VK_RETURN;
            inp[1].ki.wScan = (short)MapVirtualKey(inp[1].ki.wVk, 0);
            inp[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp[1].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[1].ki.time = 0;

            var a = SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            System.Threading.Thread.Sleep(1000);
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            Interaction.AppActivate(p.Id);
            System.Threading.Thread.Sleep(100);

            const int num = 2;
            INPUT[] inp = new INPUT[num];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_RIGHT;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[0].ki.time = 0;


            inp[1].type = INPUT_KEYBOARD;
            inp[1].ki.wVk = VK_RIGHT;
            inp[1].ki.wScan = (short)MapVirtualKey(inp[1].ki.wVk, 0);
            inp[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp[1].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[1].ki.time = 0;

            var a = SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            
            //SendKeys.SendWait("{RIGHT}");
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            Interaction.AppActivate(p.Id);
            System.Threading.Thread.Sleep(100);

            const int num = 2;
            INPUT[] inp = new INPUT[num];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_LEFT;
            inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[0].ki.time = 0;


            inp[1].type = INPUT_KEYBOARD;
            inp[1].ki.wVk = VK_LEFT;
            inp[1].ki.wScan = (short)MapVirtualKey(inp[1].ki.wVk, 0);
            inp[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp[1].ki.dwExtraInfo = GetMessageExtraInfo();
            inp[1].ki.time = 0;

            var a = SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            
            Interaction.AppActivate(p.Id);
            System.Threading.Thread.Sleep(100);
            /*
            const int num = 2;
            INPUT[] inp = new INPUT[num];

            inp[0].type = INPUT_KEYBOARD;
            inp[0].ki.wVk = VK_DOWN;
            inp[0].ki.wScan = (ushort)MapVirtualKey(inp[0].ki.wVk, 0);
            inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inp[0].ki.dwExtraInfo = 0;
            inp[0].ki.time = 0;


            inp[1].type = INPUT_KEYBOARD;
            inp[1].ki.wVk = VK_DOWN;
            inp[1].ki.wScan = (ushort)MapVirtualKey(inp[1].ki.wVk, 0);
            inp[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inp[1].ki.dwExtraInfo = 0;
            inp[1].ki.time = 0;

            var a = SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));
            */
            SendKeys.SendWait("{DOWN}");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //p = Process.GetProcessesByName("th15")[0];
            TouhouAILogic.Class1 cls = new TouhouAILogic.Class1();
            x.Content = cls.GetString();
        }
    }
}
