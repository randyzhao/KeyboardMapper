using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;

namespace KeyboardMapper
{
    public class KeyboardHooker : INotifyPropertyChanged
    {
        public class MappingPairType : INotifyPropertyChanged
        {
            /// <summary>
            /// Fired whenever a property changes.  Required for
            /// INotifyPropertyChanged interface
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(String name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            private int originalVkCode;
            private int mappingVkCode;
            public int OriginalVkCode
            {
                set
                {
                    this.originalVkCode = value;
                    this.OnPropertyChanged("OriginalVkCode");
                    this.OnPropertyChanged("OriginalKeyName");
                }
                get
                {
                    return this.originalVkCode;
                }
            }
            public int MappingVkCode
            {
                set
                {
                    this.mappingVkCode = value;
                    this.OnPropertyChanged("MappingKeyName");
                    this.OnPropertyChanged("MappingVkCode");
                }
                get
                {
                    return this.mappingVkCode;
                }
            }
            public String OriginalKeyName
            {
                get
                {
                    return KeyInterop.KeyFromVirtualKey(OriginalVkCode).ToString();
                }
                set
                {
                    this.OriginalVkCode = KeyInterop.VirtualKeyFromKey((Key)Enum.Parse(typeof(Key), value));
                    this.OnPropertyChanged("OriginalKeyName");
                    this.OnPropertyChanged("OriginalVkCode");
                }
            }
            public String MappingKeyName
            {
                get
                {
                    return KeyInterop.KeyFromVirtualKey(MappingVkCode).ToString();
                }
                set
                {
                    this.MappingVkCode = KeyInterop.VirtualKeyFromKey((Key)Enum.Parse(typeof(Key), value));
                    this.OnPropertyChanged("MappingKeyName");
                    this.OnPropertyChanged("MappingVkCode");
                }
            }
            public MappingPairType()
            {
                this.OriginalVkCode = 0;
                this.MappingVkCode = 0;
            }
        }
        

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
            OnPropertyChanged("MappingPairs");
        }

        public void DeleteMappingPair(int oriVkCode)
        {
            if (this.mappingDict.ContainsKey(oriVkCode))
            {
                this.mappingDict.Remove(oriVkCode);
                OnPropertyChanged("MappingPairs");
            }
        }

        public void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public List<MappingPairType> MappingPairs
        {
            get
            {
                var output = new List<MappingPairType>();
                foreach (var k in this.mappingDict.Keys)
                {
                    output.Add(new MappingPairType
                        {
                            OriginalVkCode = k,
                            MappingVkCode = this.mappingDict[k]
                        });
                }
                return output;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (this.MappingOn && nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN) && this.mappingDict.ContainsKey(lParam.vkCode))
            {
                keybd_event((byte)this.mappingDict[lParam.vkCode], (byte)lParam.scanCode, 0, lParam.dwExtraInfo);
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
                    this.hookID = SetWindowsHookEx(
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
}