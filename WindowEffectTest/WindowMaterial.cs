using EleCho.WpfSuite;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace WindowEffectTest;

public class WindowMaterial : DependencyObject
{
    /// <summary>
    /// 调用API的类型
    /// </summary>
    private enum APIType
    {
        NONE, SYSTEMBACKDROP, COMPOSITION
    }
    /// <summary>
    /// 所附加的窗口
    /// </summary>
    private Window? AttachedWindow
    {
        get => _window;
        set
        {
            _window = value;
            if (value != null)
            {
                _hWnd = new WindowInteropHelper(_window).Handle;
                if (_hWnd == IntPtr.Zero)
                    //窗口句柄未创建
                    value.SourceInitialized += AttachedWindow_SourceInitialized;
                else InitWindow();
            }
        }
    }

    private void AttachedWindow_SourceInitialized(object? sender, EventArgs e)
    {
        InitWindow();
        _window!.SourceInitialized -= AttachedWindow_SourceInitialized;
    }

    /// <summary>
    /// 初始化时调用
    /// </summary>
    private void InitWindow()
    {
        _hWnd = new WindowInteropHelper(_window).Handle;
        if (WindowChromeEx != null)
        {
            WindowChrome.SetWindowChrome(_window, WindowChromeEx);
        }
        SetDarkMode(IsDarkMode);
        Apply();
    }

    private void Apply()
    {
        if (_window == null | _hWnd == IntPtr.Zero) return;

        bool enable = MaterialMode != MaterialType.None || UseWindowComposition;
        if (enable)
        {
            //操作系统判定，如果是window10 即使使用MaterialMode也调用CompositionAPI
            var osVersion = Environment.OSVersion.Version;
            var windows10_1809 = new Version(10, 0, 17763);
            var windows11 = new Version(10, 0, 22621);
            //强制使用或仅支持CompositionAPI的系统
            if (UseWindowComposition || (osVersion >= windows10_1809 && osVersion < windows11))
            {
                SetWindowProperty(true);
                SetWindowCompositon(true);
            }
            else
            {
                //先关闭CompositionAPI 如果开启
                if (CurrentAPI == APIType.COMPOSITION)
                    SetWindowCompositon(false);
                SetWindowProperty(false);
                SetBackDropType(MaterialMode);
            }
        }
        else
        {
            if (CurrentAPI == APIType.COMPOSITION)
                SetWindowCompositon(false);
            else if (CurrentAPI == APIType.SYSTEMBACKDROP)
                SetBackDropType(MaterialMode);
        }
    }

    #region Window 附加属性
    public static WindowMaterial GetMaterial(Window obj)
    {
        return (WindowMaterial)obj.GetValue(MaterialProperty);
    }

    public static void SetMaterial(Window obj, WindowMaterial value)
    {
        obj.SetValue(MaterialProperty, value);
    }

    public static readonly DependencyProperty MaterialProperty =
        DependencyProperty.RegisterAttached("Material",
            typeof(WindowMaterial), typeof(WindowMaterial),
            new PropertyMetadata(null, OnMaterialChanged));

    private static void OnMaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window w && e.NewValue is WindowMaterial m)
        {
            m.AttachedWindow = w;
        }
    }
    #endregion
    #region WindowMaterial 依赖属性
    /// <summary>
    /// 是否启用暗色模式
    /// </summary>
    public bool IsDarkMode
    {
        get { return (bool)GetValue(IsDarkModeProperty); }
        set { SetValue(IsDarkModeProperty, value); }
    }

    public static readonly DependencyProperty IsDarkModeProperty =
        DependencyProperty.Register("IsDarkMode",
            typeof(bool), typeof(WindowMaterial),
            new PropertyMetadata(false, OnIsDarkModeChanged));
    private static void OnIsDarkModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WindowMaterial w)
        {
            w.SetDarkMode((bool)e.NewValue);
        }
    }

    /// <summary>
    /// 指定窗口的材质类型
    /// </summary>
    public MaterialType MaterialMode
    {
        get { return (MaterialType)GetValue(MaterialModeProperty); }
        set { SetValue(MaterialModeProperty, value); }
    }

    public static readonly DependencyProperty MaterialModeProperty =
        DependencyProperty.Register("MaterialMode",
            typeof(MaterialType), typeof(WindowMaterial),
            new PropertyMetadata(MaterialType.None, OnMaterialModeChanged));
    private static void OnMaterialModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WindowMaterial m)
        {
            m.Apply();
        }
    }

    /// <summary>
    /// 另需指定的WindowChrome
    /// </summary>
    public WindowChrome WindowChromeEx
    {
        get { return (WindowChrome)GetValue(WindowChromeExProperty); }
        set { SetValue(WindowChromeExProperty, value); }
    }

    public static readonly DependencyProperty WindowChromeExProperty =
        DependencyProperty.Register("WindowChromeEx",
            typeof(WindowChrome), typeof(WindowMaterial),
            new PropertyMetadata(null, OnWindowChromeExChanged));

    private static void OnWindowChromeExChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WindowMaterial { } m && e.NewValue is WindowChrome { } wc && m._window != null)
        {
            //如果WindowChrome直接附加在窗口上会覆盖掉我们设置的GlassFrameThickness
            //故这里的设计是将WindowChrome附加在WindowMaterial上进行管理
            WindowChrome.SetWindowChrome(m._window, wc);
            m.Apply();
        }
    }

    public bool UseWindowComposition
    {
        get { return (bool)GetValue(UseWindowCompositionProperty); }
        set { SetValue(UseWindowCompositionProperty, value); }
    }

    public static readonly DependencyProperty UseWindowCompositionProperty =
        DependencyProperty.Register("UseWindowComposition",
            typeof(bool), typeof(WindowMaterial),
            new PropertyMetadata(false, OnUseWindowCompositionChanged));

    private static void OnUseWindowCompositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WindowMaterial m)
        {
            m.Apply();
        }
    }

    public Color CompositonColor
    {
        get { return (Color)GetValue(CompositonColorProperty); }
        set { SetValue(CompositonColorProperty, value); }
    }

    public static readonly DependencyProperty CompositonColorProperty =
        DependencyProperty.Register("CompositonColor",
            typeof(Color), typeof(WindowMaterial),
            new PropertyMetadata(Color.FromArgb(180, 0, 0, 0), OnCompositionColorChanged));
    private static void OnCompositionColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WindowMaterial m)
        {
            m.SetCompositionColor((Color)e.NewValue);
            m.Apply();
        }
    }
    #endregion

    private IntPtr _hWnd = IntPtr.Zero;
    private Window? _window = null;
    private APIType CurrentAPI = APIType.NONE;
    private int _blurColor;
    private void SetCompositionColor(Color value)
    {
        _blurColor = value.ToHexColor();
    }
    private void SetDarkMode(bool isDarkMode)
    {
        if (_hWnd == IntPtr.Zero) return;
        MaterialApis.SetDarkMode(_hWnd, isDarkMode);
    }
    private void SetBackDropType(MaterialType blurMode)
    {
        if (_hWnd == IntPtr.Zero) return;
        MaterialApis.SetBackDropType(_hWnd, blurMode);
        CurrentAPI = blurMode == MaterialType.None ? APIType.NONE : APIType.SYSTEMBACKDROP;
    }
    private void SetWindowCompositon(bool enable)
    {
        if (_hWnd == IntPtr.Zero) return;
        MaterialApis.SetWindowComposition(_hWnd, enable, _blurColor);
        CurrentAPI = enable ? APIType.COMPOSITION : APIType.NONE;
    }
    private void SetWindowProperty(bool isLagcy = false)
    {
        if (_hWnd == IntPtr.Zero) return;
        var hwndSource = (HwndSource)PresentationSource.FromVisual(_window);
        int margin = isLagcy ? 0 : -1;   //lagcy 0 ?? 1
        MaterialApis.SetWindowProperties(hwndSource, margin);
    }
}

/// <summary>
/// 材质类型
/// </summary>
public enum MaterialType
{
    None = 1,
    Acrylic = 3,
    Mica = 2,
    MicaAlt = 4
}
