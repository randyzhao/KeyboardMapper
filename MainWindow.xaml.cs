﻿using System;
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
        private KeyboardHooker kh = new KeyboardHooker();
        private AddMappingWindow addWin = new AddMappingWindow();
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

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            this.addWin.Show();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            this.kh.MappingOn = true;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            this.kh.MappingOn = false;
        }
    }
#region Converters
    [ValueConversion(typeof(Int32), typeof(String))]
    public class KeyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Key k = KeyInterop.KeyFromVirtualKey((int)value);
            return k.ToString();
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

#endregion
}
