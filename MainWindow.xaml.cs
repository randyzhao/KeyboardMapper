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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyboardMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardHooker kh = new KeyboardHooker();
        public void handleHookEvent(string msg)
        {
            MessageBox.Show(msg);
        }

        public MainWindow()
        {
            InitializeComponent();
            kh.HookEvent += new KeyboardHooker.HookEventHandler(this.handleHookEvent);
            this.mappingPairListView.DataContext = kh;
           
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.kh.AddMappingPair(0, 1);
            this.kh.AddMappingPair(2, 1);
            this.kh.AddMappingPair(3, 1);
        }

    }
}
