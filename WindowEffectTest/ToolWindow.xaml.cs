using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace WindowEffectTest
{
    /// <summary>
    /// ToolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToolWindow : Window
    {
        public ToolWindow()
        {
            InitializeComponent();
            Loaded += ToolWindow_Loaded;
            MouseLeftButtonDown += ToolWindow_MouseLeftButtonDown;
            MouseRightButtonDown += ToolWindow_MouseRightButtonDown;
        }

        private void ToolWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ToolWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void ToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowMaterial.SetMaterial(this,new WindowMaterial()
            {
                UseWindowComposition=true,
                CompositonColor=Color.FromArgb(230,0,0,0)
            });
        }
    }
}
