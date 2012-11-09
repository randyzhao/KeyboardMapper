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
using System.Windows.Input;
namespace KeyboardMapper
{
    /// <summary>
    /// Interaction logic for AddMappingWindow.xaml
    /// </summary>
    public partial class AddMappingWindow : Window
    {
        public AddMappingWindow()
        {
            InitializeComponent();
            foreach (var name in Enum.GetNames(typeof(Key)))
            {
                this.oriComboBox.Items.Add(name);
                this.mappingComboBox.Items.Add(name);
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
