using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace WindowEffectTest;

internal static class FluentPopupFunc
{
    public const BindingFlags privateInstanceFlag = BindingFlags.NonPublic | BindingFlags.Instance;
    public static IntPtr GetNativeWindowHwnd(this ToolTip tip) => GetPopup(tip).GetNativeWindowHwnd();
    public static IntPtr GetNativeWindowHwnd(this ContextMenu menu) => GetPopup(menu).GetNativeWindowHwnd();

    [UnsafeAccessor(UnsafeAccessorKind.Field,Name ="_parentPopup")]
    private static extern Popup GetPopup(ToolTip tip);
    [UnsafeAccessor(UnsafeAccessorKind.Field,Name ="_parentPopup")]
    private static extern Popup GetPopup(ContextMenu tip);

    public static IntPtr GetNativeWindowHwnd(this Popup popup)
    {
        var field = typeof(Popup).GetField("_secHelper", privateInstanceFlag);
        if (field != null)
        {
            if (field.GetValue(popup) is { } _secHelper)
            {
                if (_secHelper.GetType().GetProperty("Handle", privateInstanceFlag) is { } prop)
                {
                    if (prop.GetValue(_secHelper) is IntPtr handle)
                    {
                        return handle;
                    }
                }
            }
        }
        return IntPtr.Zero;
    }

    public static void SetPopupWindowMaterial(IntPtr hwnd, Color compositionColor,
        MaterialApis.WindowCorner corner = MaterialApis.WindowCorner.Round)
    {
        if (hwnd != IntPtr.Zero)
        {
            int hexColor = compositionColor.ToHexColor();
            var hwndSource = HwndSource.FromHwnd(hwnd);
            MaterialApis.SetWindowProperties(hwndSource, 1);
            MaterialApis.SetWindowComposition(hwnd, true, hexColor);
            MaterialApis.SetWindowCorner(hwnd, corner);
        }
    }
}