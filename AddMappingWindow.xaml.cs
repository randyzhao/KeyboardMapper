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
            this.MappingPair = new KeyboardHooker.MappingPairType();
            this.DataContext = this.MappingPair;
          //  this.oriComboBox.SelectionChanged += new SelectionChangedEventHandler
          //      (this.OnOriComboBoxSelectionChanged);
          //  this.mappingComboBox.SelectionChanged += new SelectionChangedEventHandler
          //      (this.OnMappingComboBoxSelectionChanged);

        }

        public void OnOriComboBoxSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            this.MappingPair.OriginalKeyName = (String)this.oriComboBox.SelectedValue;
        }

        public void OnMappingComboBoxSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            this.MappingPair.MappingKeyName = (String)this.mappingComboBox.SelectedValue;
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

        public KeyboardHooker.MappingPairType MappingPair
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

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClickConfirm = true;
            this.Close();
        }

        private void cancleButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClickConfirm = false;
            this.Close();
        }

 
    }
}
