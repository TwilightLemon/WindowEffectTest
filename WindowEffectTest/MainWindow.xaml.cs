using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static WindowEffectTest.MaterialApis;

namespace WindowEffectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDarkMode;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                windowMaterial.IsDarkMode = _isDarkMode = value;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed) 
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new ToolWindow().Show();
        }
    }
}
