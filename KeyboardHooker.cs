using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;

public class KeyboardHooker : INotifyPropertyChanged
{
    /// <summary>
    /// Fired whenever a property changes.  Required for
    /// INotifyPropertyChanged interface
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private IntPtr hookID = IntPtr.Zero;

    //key is original key virtual code
    //value is mapping key virtual code
    private Dictionary<int, int> mappingDict = new Dictionary<int, int>();

    //whether the keyboard mapping function is on
    public Boolean MappingOn { set; get; }

    private delegate IntPtr HookHandlerDelegate(
        int nCode,
        IntPtr wParam,
        ref KBDLLHOOKSTRUCT lParam);

    public delegate void HookEventHandler(String msg);
    public event HookEventHandler HookEvent;
    private HookHandlerDelegate proc;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(
        int idHook,
        HookHandlerDelegate lpfn,
        IntPtr hMod,
        uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(
        IntPtr hhk,
        int nCode,
        IntPtr wParam,
        ref KBDLLHOOKSTRUCT lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    private static extern void keybd_event(
        byte bVk,
        byte bScan,
        int dwFlags,
        int dwExtraInfo
    );

    private struct KBDLLHOOKSTRUCT
    {
        //virtual keyborad code
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    
    //add a mapping pair
    public void AddMappingPair(int oriVkCode, int mappingVkCode)
    {
        this.mappingDict[oriVkCode] = mappingVkCode;
        //notify property changed
        if (this.PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("MappingPairs"));
        }
    }

    public void DeleteMappingPair(int oriVkCode)
    {
        if (this.mappingDict.ContainsKey(oriVkCode))
        {
            this.mappingDict.Remove(oriVkCode);
            //notify property changed
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("MappingPairs"));
            }
        }
    }

    public List<Tuple<int, int>> MappingPairs
    {
        get
        {
            var output = new List<Tuple<int, int>>();
            foreach (var k in this.mappingDict.Keys)
            {
                output.Add(new Tuple<int, int>(k, this.mappingDict[k]));
            }
            return output;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
    {
        if (this.MappingOn && nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN))
        {
            keybd_event((byte)lParam.vkCode, (byte)lParam.scanCode, 0, lParam.dwExtraInfo);
            return (IntPtr)1;
        }
        else
        {
            return CallNextHookEx(hookID, nCode, wParam, ref lParam);
        }
    }

    public KeyboardHooker()
    {
        this.MappingOn = false;
        proc = new HookHandlerDelegate(this.HookCallback);
        using (Process curProcss = Process.GetCurrentProcess())
        {
            using (ProcessModule curModule = curProcss.MainModule)
            {
                this.hookID =  SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    proc,
                    GetModuleHandle(curModule.ModuleName),
                    0);
            }
        }
    }

    ~KeyboardHooker()
    {
        UnhookWindowsHookEx(this.hookID);
    }
}