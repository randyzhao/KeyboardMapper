using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace KeyboardMapper
{
    public class MappingPairType
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
                if (this.OriginalVkCode == 0)
                {
                    return "None";
                }
                else
                {
                    return KeyInterop.KeyFromVirtualKey(OriginalVkCode).ToString();
                }
            }
            
        }
        public String MappingKeyName
        {
            get
            {
                if (this.MappingVkCode == 0)
                {
                    return "None";
                }
                else
                {
                    return KeyInterop.KeyFromVirtualKey(MappingVkCode).ToString();
                }
            }
            
        }
        public MappingPairType()
        {
            this.OriginalVkCode = 0;
            this.MappingVkCode = 0;
        }
    }
}
