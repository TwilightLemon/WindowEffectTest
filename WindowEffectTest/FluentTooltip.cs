using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowEffectTest;

public static class FluentTooltip
{
    public static bool GetUseFluentStyle(DependencyObject obj)
    {
        return (bool)obj.GetValue(UseFluentStyleProperty);
    }

    public static void SetUseFluentStyle(DependencyObject obj, bool value)
    {
        obj.SetValue(UseFluentStyleProperty, value);
    }

    public static readonly DependencyProperty UseFluentStyleProperty =
        DependencyProperty.RegisterAttached("UseFluentStyle",
            typeof(bool), typeof(FluentTooltip),
            new PropertyMetadata(false, OnUseFluentStyleChanged));
    public static void OnUseFluentStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            if (obj is ToolTip tip)
            {
                if ((bool)e.NewValue)
                {
                    tip.Opened += Popup_Opened;
                }
                else
                {
                    tip.Opened -= Popup_Opened;
                }
            }
            else if (obj is ContextMenu menu)
            {
                if ((bool)e.NewValue)
                {
                    menu.Opened += Popup_Opened;
                }
                else
                {
                    menu.Opened -= Popup_Opened;
                }
            }
        }
    }

    private static void Popup_Opened(object sender, RoutedEventArgs e)
    {
        if (sender is ToolTip tip && tip.Background is SolidColorBrush cb)
        {
            var hwnd = tip.GetNativeWindowHwnd();
            FluentPopupFunc.SetPopupWindowMaterial(hwnd, cb.Color, MaterialApis.WindowCorner.RoundSmall);
        }
        else if (sender is ContextMenu menu && menu.Background is SolidColorBrush color)
        {
            var hwnd = menu.GetNativeWindowHwnd();
            Debug.WriteLine(hwnd);
            FluentPopupFunc.SetPopupWindowMaterial(hwnd, color.Color, MaterialApis.WindowCorner.Round);
        }
    }
}
