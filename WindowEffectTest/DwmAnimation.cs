﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Threading;

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

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        public const int GWL_STYLE = -16;
        public const long WS_CAPTION = 0x00C00000L,
                WS_MAXIMIZEBOX = 0x00010000L,
            WS_MINIMIZEBOX = 0x00020000L,
            WS_THICKFRAME = 0x00040000L;

        public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            }
        }
        public static void EnableDwmAnimation(Window w)
        {
            var myHWND = new WindowInteropHelper(w).Handle;
            IntPtr myStyle = new(WS_CAPTION|WS_THICKFRAME|WS_MAXIMIZEBOX|WS_MINIMIZEBOX);
            if (w.ResizeMode == ResizeMode.NoResize)
            {
                myStyle = new(WS_CAPTION);
            }
            SetWindowLongPtr(new HandleRef(null, myHWND), GWL_STYLE, myStyle);
        }
    }
}