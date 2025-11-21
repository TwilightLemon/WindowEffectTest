using System;
using System.Windows;
using System.Windows.Interop;

namespace WindowEffectTest
{
    public static class DwmAnimation
    {

        public static bool GetEnableDwmAnimation(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableDwmAnimationProperty);
        }

        public static void SetEnableDwmAnimation(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableDwmAnimationProperty, value);
        }

        public static readonly DependencyProperty EnableDwmAnimationProperty =
            DependencyProperty.RegisterAttached("EnableDwmAnimation", 
                typeof(bool), typeof(DwmAnimation), 
                new PropertyMetadata(false,OnEnableDwmAnimationChanged));

        public static void OnEnableDwmAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                if ((bool)e.NewValue)
                {
                    if (window.IsLoaded)
                    {
                        EnableDwmAnimation(window);
                    }
                    else
                    {
                        window.SourceInitialized += Window_SourceInitialized;
                    }
                }
            }
        }

        private static void Window_SourceInitialized(object? sender, EventArgs e)
        {
            if(sender is Window w)
            {
                EnableDwmAnimation(w);
                w.SourceInitialized -= Window_SourceInitialized;
            }
        }

        public static void EnableDwmAnimation(Window w)
        {
            var myHWND = new WindowInteropHelper(w).Handle;
            nint myStyle = (nint)(Win32Interop.WS_CAPTION|Win32Interop.WS_THICKFRAME|Win32Interop.WS_MAXIMIZEBOX|Win32Interop.WS_MINIMIZEBOX);
            if (w.ResizeMode == ResizeMode.NoResize||w.ResizeMode==ResizeMode.CanMinimize)
            {
                myStyle = (nint)(Win32Interop.WS_CAPTION | Win32Interop.WS_MINIMIZEBOX);
            }
            Win32Interop.SetWindowLongPtr(myHWND, Win32Interop.GWL_STYLE, myStyle);
        }
    }
}
