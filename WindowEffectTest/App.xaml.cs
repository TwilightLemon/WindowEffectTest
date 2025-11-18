using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WindowEffectTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void SetDarkMode(bool isDarkMode)
        {
            App.Current.Resources["ForeColor"]=isDarkMode?Brushes.White: Brushes.Black;
            App.Current.Resources["PopupWindowBackground"] = new SolidColorBrush(isDarkMode ? Color.FromArgb(0x6C, 0, 0, 0) : Color.FromArgb(0x6C, 0xFF, 0xFF, 0xFF));

        }
    }
}
