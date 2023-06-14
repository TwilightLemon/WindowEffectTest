using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowEffectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        WindowAccentCompositor wac = null;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            wac = new(this,false, (c) =>
            {
                //没有可用的模糊特效
                c.A = 255;
                Background = new SolidColorBrush(c);
            });
            wac.Color = (bool)DarkModeCheck.IsChecked ? Color.FromArgb(180, 0, 0, 0) : Color.FromArgb(180, 255, 255, 255);
            wac.IsEnabled = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed) 
                this.DragMove();
        }

        private void DarkModeCheck_Click(object sender, RoutedEventArgs e)
        {
            if (wac != null)
            {
                wac.Color = (bool)DarkModeCheck.IsChecked ? Color.FromArgb(180, 0, 0, 0) : Color.FromArgb(180, 255, 255, 255);
                wac.DarkMode = (bool)DarkModeCheck.IsChecked;
                wac.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new ToolWindow().Show();
        }
    }
}
