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
        }
    }
}
