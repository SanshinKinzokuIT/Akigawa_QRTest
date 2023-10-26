using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;

namespace QRTest
{
    public partial class Form1 : Form
    {

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        // Store a reference to Form1
        // フォーム1への参照を保存
        private static Form1 currentForm;

        public Form1()
        {
            InitializeComponent();
            _hookID = SetHook(_proc);
            currentForm = this;
        }

        // Form1 loading event
        // Form1 ロードイベント
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // Struct to store window text information
        // ウィンドウテキスト情報を格納する構造体
        private struct WinText
        {
            public IntPtr hWnd;
            public string Text;
        }

        // Button click event
        // ボタンクリックイベント
        private void button1_Click(object sender, EventArgs e)
        {
        }

        // Delegate for low-level keyboard procedure
        // 低レベルキーボードプロシージャのためのデリゲート
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Set the keyboard hook
        // キーボードフックを設定する
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // Callback function for the keyboard hook
        // キーボードフックのためのコールバック関数
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                char keyPressed = (char)vkCode;

                if (currentForm != null)
                {
                    currentForm.Invoke(new Action(() => UpdateLabel2(keyPressed.ToString())));
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        // P/Invoke to set a Windows hook
        // Windowsフックを設定するためのP/Invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        // P/Invoke to remove a Windows hook
        // Windowsフックを解除するためのP/Invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // P/Invoke to call the next hook in the chain
        // チェーン内の次のフックを呼び出すためのP/Invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // P/Invoke to get the module handle
        // モジュールハンドルを取得するためのP/Invoke
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Form closing event to unhook the keyboard
        // キーボードをアンフックするためのフォームクロージングイベント
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
        }

        // Get the last character from a string
        // 文字列から最後の文字を取得する
        private char GetLastChar(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input[input.Length - 1];
            }

            return '\0';
        }

        // Update label2 text
        // ラベル2のテキストを更新する
        private static void UpdateLabel2(string newText)
        {
            if (currentForm != null)
            {
                Label label2 = currentForm.Controls["label2"] as Label;
                if (label2 != null)
                {
                    label2.Text = newText;
                }
            }
        }
    }
}