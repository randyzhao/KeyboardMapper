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
        public void handleHookEvent(string msg)
        {
            MessageBox.Show(msg);
        }

        public MainWindow()
        {
            InitializeComponent();
            KeyboardHooker kh = new KeyboardHooker();
            kh.HookEvent += new KeyboardHooker.HookEventHandler(this.handleHookEvent);
        }

    }
}
