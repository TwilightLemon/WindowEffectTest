using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WindowEffectTest;

public class FluentPopup : Popup
{
    public enum ExPopupAnimation
    {
        None,
        SlideUp,
        SlideDown
    }
    private DoubleAnimation? _slideAni;
    static FluentPopup()
    {
        //对IsOpenProperty添加PropertyChangedCallback
        IsOpenProperty.OverrideMetadata(typeof(FluentPopup), new FrameworkPropertyMetadata(false, OnIsOpenChanged));
    }
    public FluentPopup()
    {
        Opened += FluentPopup_Opened;
        Closed += FluentPopup_Closed;
    }
    #region


    public bool FollowWindowMoving
    {
        get { return (bool)GetValue(FollowWindowMovingProperty); }
        set { SetValue(FollowWindowMovingProperty, value); }
    }

    // Using a DependencyProperty as the backing store for FollowWindowMoving.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FollowWindowMovingProperty =
        DependencyProperty.Register("FollowWindowMoving", typeof(bool), typeof(FluentPopup), new PropertyMetadata(false, OnFollowWindowMovingChanged));
    private static void OnFollowWindowMovingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is FluentPopup popup && Window.GetWindow(popup) is { } window)
        {
            if (e.NewValue is true)
            {
                window.LocationChanged += popup.AttachedWindow_LocationChanged;
                window.SizeChanged += popup.AttachedWindow_SizeChanged;
            }else
            {
                window.LocationChanged -= popup.AttachedWindow_LocationChanged;
                window.SizeChanged -= popup.AttachedWindow_SizeChanged;
            }
        }
    }


    public MaterialApis.WindowCorner WindowCorner
    {
        get { return (MaterialApis.WindowCorner)GetValue(WindowCornerProperty); }
        set { SetValue(WindowCornerProperty, value); }
    }

    public static readonly DependencyProperty WindowCornerProperty =
        DependencyProperty.Register("WindowCorner",
            typeof(MaterialApis.WindowCorner), typeof(FluentPopup),
            new PropertyMetadata(MaterialApis.WindowCorner.Round));

    public static void OnWindowCornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FluentPopup popup)
        {
            popup.ApplyWindowCorner();
        }
    }

    private void AttachedWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        FollowMove();
    }

    private void AttachedWindow_LocationChanged(object? sender, EventArgs e)
    {
        FollowMove();
    }
    private void FollowMove()
    {
        if (IsOpen)
        {
            //var mi = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //mi.Invoke(this, null);
            CallUpdatePosition(this);
        }
    }
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name="UpdatePosition")]
    static extern void CallUpdatePosition(Popup popup);

    #endregion
    #region 启动动画控制
    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FluentPopup popup)
        {
            if ((bool)e.NewValue)
            {
                popup.BuildAnimation();
            }
        }
    }
    private void FluentPopup_Closed(object? sender, EventArgs e)
    {
        ResetAnimation();
    }

    public new bool IsOpen
    {
        get => base.IsOpen;
        set
        {
            if (value)
            {
                BuildAnimation();
                base.IsOpen = value;
                // Run Animation in Opened Event
            }
            else
            {
                base.IsOpen = value;
                //closed event will reset animation
                //ResetAnimation();
            }
        }
    }
    public uint SlideAnimationOffset { get; set; } = 50;
    private void ResetAnimation()
    {
        if (ExtPopupAnimation is ExPopupAnimation.SlideUp or ExPopupAnimation.SlideDown)
        {
            BeginAnimation(VerticalOffsetProperty, null);
        }
    }
    public void BuildAnimation()
    {
        if (ExtPopupAnimation is ExPopupAnimation.SlideUp or ExPopupAnimation.SlideDown)
        {
            _slideAni = new DoubleAnimation(VerticalOffset + (ExtPopupAnimation == ExPopupAnimation.SlideUp ?
                SlideAnimationOffset : -SlideAnimationOffset), VerticalOffset, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase()
            };
        }
    }
    public void RunPopupAnimation()
    {
        if (_slideAni != null)
        {
            BeginAnimation(VerticalOffsetProperty, _slideAni);
        }
    }

    #endregion
    #region Fluent Style
    public SolidColorBrush Background
    {
        get { return (SolidColorBrush)GetValue(BackgroundProperty); }
        set { SetValue(BackgroundProperty, value); }
    }

    public static readonly DependencyProperty BackgroundProperty =
        DependencyProperty.Register("Background",
            typeof(SolidColorBrush), typeof(FluentPopup),
            new PropertyMetadata(Brushes.Transparent, OnBackgroundChanged));

    public static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FluentPopup popup)
        {
            popup.ApplyFluentHwnd();
        }
    }

    public ExPopupAnimation ExtPopupAnimation
    {
        get { return (ExPopupAnimation)GetValue(ExtPopupAnimationProperty); }
        set { SetValue(ExtPopupAnimationProperty, value); }
    }

    public static readonly DependencyProperty ExtPopupAnimationProperty =
        DependencyProperty.Register("ExtPopupAnimation", typeof(ExPopupAnimation), typeof(FluentPopup),
            new PropertyMetadata(ExPopupAnimation.None));

    private IntPtr _windowHandle = IntPtr.Zero;
    private void FluentPopup_Opened(object? sender, EventArgs e)
    {
        _windowHandle = this.GetNativeWindowHwnd();
        ApplyFluentHwnd();
        Dispatcher.Invoke(RunPopupAnimation);
    }
    public void ApplyFluentHwnd()
    {
        FluentPopupFunc.SetPopupWindowMaterial(_windowHandle, Background.Color, WindowCorner);
    }
    public void ApplyWindowCorner()
    {
        MaterialApis.SetWindowCorner(_windowHandle, WindowCorner);
    }
    #endregion
}
