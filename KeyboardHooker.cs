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
        /// <summary>
        /// Fired whenever a property changes.  Required for
        /// INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// stores the mapping pair in order
        /// </summary>
        private List<MappingPairType> mappingPairList = new List<MappingPairType>();
        /// <summary>
        /// stores the mapping pair for fast lookup in the hook callback
        /// </summary>
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


        /// <summary>
        /// add or update a mapping pair in the mapping list
        /// </summary>
        /// <param name="oriVkCode"></param>
        /// <param name="mappingVkCode"></param>
        public void UpdateMappingPair(MappingPairType mappingPair)
        {
            Boolean exist = false;
            for (int i = 0; i < this.mappingPairList.Count; i++)
            {
                if (this.mappingPairList[i].OriginalVkCode == mappingPair.OriginalVkCode)
                {
                    this.mappingPairList[i] = mappingPair;
                    exist = true;
                    break;
                }
            }
            if (!exist)
            {
                this.mappingPairList.Add(mappingPair);
            }
            this.mappingDict[mappingPair.OriginalVkCode] = mappingPair.MappingVkCode;
            OnPropertyChanged("MappingPairs");
        }

        /// <summary>
        /// delete a mapping pair
        /// </summary>
        /// <param name="index">the index of this mapping pair</param>
        public void DeleteMappingPair(int index)
        {
            if (this.mappingPairList.Count > index)
            {
                this.mappingDict.Remove(this.mappingPairList[index].OriginalVkCode);
                this.mappingPairList.RemoveAt(index);
                OnPropertyChanged("MappingPairs");
            }
        }


        /// <summary>
        /// remove all the mapping pairs
        /// </summary>
        public void ClearMappingPairs()
        {
            this.mappingPairList.Clear();
            this.mappingDict.Clear();
            OnPropertyChanged("MappingPairs");
        }

        public void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// return a copy of mapping pair with specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MappingPairType GetMappingPair(int index)
        {
            if (this.mappingPairList.Count > index)
            {
                //copy the mapping pair, do not just return the reference
                return new MappingPairType
                    {
                        OriginalVkCode = this.mappingPairList[index].OriginalVkCode,
                        MappingVkCode = this.mappingPairList[index].MappingVkCode
                    };
            }
            else
            {
                return null;
            }
        }
        public List<MappingPairType> MappingPairs
        {
            get
            {
                List<MappingPairType> output = new List<MappingPairType>();
                for (int i = 0; i < this.mappingPairList.Count; i++)
                {
                    output.Add(new MappingPairType
                        {
                            OriginalVkCode = this.mappingPairList[i].OriginalVkCode,
                            MappingVkCode = this.mappingPairList[i].MappingVkCode 
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