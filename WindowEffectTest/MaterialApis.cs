using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;

namespace WindowEffectTest;

public static class MaterialApis
{
    public static int ToHexColor(this Color value)
    {
        return value.R << 0 | value.G << 8 | value.B << 16 | value.A << 24;
    }
    public static void SetWindowProperties(HwndSource hwndSource,int margin)
    {
        hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
        var margins = new Win32Interop.Margins()
        {
            LeftWidth = margin,
            TopHeight = margin,
            RightWidth = margin,
            BottomHeight = margin
        };

        Win32Interop.DwmExtendFrameIntoClientArea(hwndSource.Handle, ref margins);
    }

    internal static int SetWindowAttribute(IntPtr hwnd, Win32Interop.DWMWINDOWATTRIBUTE attribute, int parameter)
        => Win32Interop.DwmSetWindowAttribute(hwnd, attribute, ref parameter, Marshal.SizeOf<int>());

    public enum WindowCorner
    {
        Default = 0,
        DoNotRound = 1,
        Round = 2,
        RoundSmall = 3
    }
    public static void SetWindowCorner(IntPtr handle,WindowCorner corner)
    {
        SetWindowAttribute(handle, Win32Interop.DWMWINDOWATTRIBUTE.WINDOW_CORNER_PREFERENCE,(int)corner);
    }

    public static void SetWindowComposition(IntPtr handle, bool enable, int? hexColor = null)
    {
        var accent = new Win32Interop.AccentPolicy();
        if (!enable)
        {
            accent.AccentState = Win32Interop.AccentState.ACCENT_DISABLED;
        }
        else
        {
            accent.AccentState = Win32Interop.AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            accent.GradientColor = hexColor ?? 0x00000000;
        }
        var data = new Win32Interop.WindowCompositionAttributeData
        {
            Attribute = Win32Interop.WindowCompositionAttribute.WCA_ACCENT_POLICY,
            SizeOfData = Marshal.SizeOf<Win32Interop.AccentPolicy>(),
            Data = Marshal.AllocHGlobal(Marshal.SizeOf<Win32Interop.AccentPolicy>())
        };
        Marshal.StructureToPtr(accent, data.Data, false);
        Win32Interop.SetWindowCompositionAttribute(handle, ref data);
        Marshal.FreeHGlobal(data.Data);
    }
    public static void SetBackDropType(IntPtr handle, MaterialType mode)
    {
        SetWindowAttribute(
            handle,
            Win32Interop.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            (int)mode);
    }

    public static void SetDarkMode(IntPtr handle, bool isDarkMode)
    {
        SetWindowAttribute(
                    handle,
                    Win32Interop.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                    isDarkMode ? 1 : 0);
    }
}
