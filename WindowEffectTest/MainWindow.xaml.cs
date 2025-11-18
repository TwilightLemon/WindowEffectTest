using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WindowEffectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDarkMode = false;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.SetDarkMode(IsDarkMode);
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                windowMaterial.IsDarkMode = _isDarkMode = value;
                App.SetDarkMode(value);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed) 
                this.DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaxmizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void ShowPopupBtn_Click(object sender, RoutedEventArgs e)
        {
            await Task.Yield();
            testPopup.IsOpen = true;
        }
    }
}
