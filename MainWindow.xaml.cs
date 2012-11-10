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
using System.Globalization;

namespace KeyboardMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardHooker keyboardHooker = new KeyboardHooker();
        private AddMappingWindow addWin = new AddMappingWindow();
        public void handleHookEvent(string msg)
        {
            MessageBox.Show(msg);
        }

        public MainWindow()
        {
            InitializeComponent();
            keyboardHooker.HookEvent += new KeyboardHooker.HookEventHandler(this.handleHookEvent);
            this.mappingPairListView.DataContext = keyboardHooker;

        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            this.addWin.Init(null);
            this.addWin.ShowDialog();
            if (this.addWin.ClickConfirm)
            {
                this.keyboardHooker.UpdateMappingPair(this.addWin.MappingPair);
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            this.keyboardHooker.MappingOn = true;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            this.keyboardHooker.MappingOn = false;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }

}
