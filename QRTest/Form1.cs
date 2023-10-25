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

        // Lưu trữ tham chiếu đến Form1
        private static Form1 currentForm;
        public Form1()
        {
            InitializeComponent();
            _hookID = SetHook(_proc);
            currentForm = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private struct WinText
        {
            public IntPtr hWnd;
            public string Text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
          
        }
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                char keyPressed = (char)vkCode;

                // Gọi hàm UpdateLabel2 và truyền giá trị newText
                if (currentForm != null)
                {
                    currentForm.Invoke(new Action(() => UpdateLabel2(keyPressed.ToString())));
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
        }
        private char GetLastChar(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input[input.Length - 1];
            }

            // Trả về một giá trị mặc định hoặc xử lý khác tùy thuộc vào yêu cầu của bạn.
            return '\0';
        }
        private static void UpdateLabel2(string newText)
        {

            // Sử dụng label2 trực tiếp từ ActiveForm
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