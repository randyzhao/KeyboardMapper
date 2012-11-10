using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
namespace KeyboardMapper
{
    /// <summary>
    /// Interaction logic for AddMappingWindow.xaml
    /// </summary>
    public partial class AddMappingWindow : Window, INotifyPropertyChanged
    {
        public AddMappingWindow()
        {
            InitializeComponent();
            foreach (var name in Enum.GetNames(typeof(Key)))
            {
                this.oriComboBox.Items.Add(name);
                this.mappingComboBox.Items.Add(name);
            }
            this.oriComboBox.SelectedIndex = 0;
            this.mappingComboBox.SelectedIndex = 0;
            this.MappingPair = new MappingPairType();
            this.DataContext = this.MappingPair;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            this.ClickConfirm = false;
            this.Hide();
        }
        public void SetOriVkCode(int vkcode)
        {
            this.MappingPair.OriginalVkCode = vkcode;
            this.OnPropertyChanged("MappingPair");
        }

        public void SetMappingVkCode(int vkcode)
        {
            this.MappingPair.MappingVkCode = vkcode;
            this.OnPropertyChanged("MappingPair");
        }

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

        public MappingPairType MappingPair
        {
            set;
            get;
        }
        /// <summary>
        /// whether the user click 'Confirm' button or the 'Cancle' button
        /// </summary>
        public Boolean ClickConfirm
        {
            set;
            get;
        }

        /// <summary>
        /// call this functio before call Show() in the main window
        /// init the controllers to display right contents
        /// </summary>
        /// <param name="mappingPair">
        /// null means adding a new MappingPair
        /// not null means editing a existing pair
        /// </param>
        public void Init(MappingPairType mappingPair)
        {
            this.ClickConfirm = false;
            if (mappingPair == null)
            {
                this.MappingPair = new MappingPairType();
                this.Title = "新建";
            }
            else
            {
                this.MappingPair = mappingPair;
                this.Title = "编辑";
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClickConfirm = true;
            this.Hide();
        }

        private void cancleButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClickConfirm = false;
            this.Hide();
        }

 
    }
}
