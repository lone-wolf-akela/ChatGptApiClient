using System;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Diagnostics;

namespace ChatGptApiClientV2;

// from https://www.wpf-controls.com/wpf-smooth-scroll-viewer/
public class SmoothScrollInfoAdapter(IScrollInfo child) : UIElement, IScrollInfo
{
    public bool AnimatedScrollingEnabled { get; set; } = true;

    private readonly IScrollInfo _child = child;
    private double _intendedVerticalOffset = 0;
    private double IntendedVerticalOffset
    {
        get => _intendedVerticalOffset;
        set
        {
            if(double.IsNaN(value)) 
            { 
                return; 
            }
            _intendedVerticalOffset = Math.Clamp(value, 0, ScrollOwner.ScrollableHeight);
        }
    }
    private double _intendedHorizontalOffset = 0;
    private double IntendedHorizontalOffset
    {
        get => _intendedHorizontalOffset;
        set
        {
            if(double.IsNaN(value)) 
            { 
                return; 
            }
            _intendedHorizontalOffset = Math.Clamp(value, 0, ScrollOwner.ScrollableWidth);
        }
    }
    internal const double _scrollLineDelta = 16.0;
    internal const double _mouseWheelBaseDelta = 48.0;

    enum MouseWheelDirection { Up, Down, Left, Right };
    double MouseWheelTargetValue(MouseWheelDirection direction)
    {
        var currentDistance = direction switch
        {
            MouseWheelDirection.Up => VerticalOffset - IntendedVerticalOffset,
            MouseWheelDirection.Down => IntendedVerticalOffset - VerticalOffset,
            MouseWheelDirection.Left => HorizontalOffset - IntendedHorizontalOffset,
            MouseWheelDirection.Right => IntendedHorizontalOffset - HorizontalOffset,
            _ => throw new NotImplementedException(),
        };

        if (currentDistance < 0)
        {
            // brake
            return direction switch
            {
                MouseWheelDirection.Up => IntendedVerticalOffset - _mouseWheelBaseDelta,
                MouseWheelDirection.Down => IntendedVerticalOffset + _mouseWheelBaseDelta,
                MouseWheelDirection.Left => IntendedHorizontalOffset - _mouseWheelBaseDelta,
                MouseWheelDirection.Right => IntendedHorizontalOffset + _mouseWheelBaseDelta,
                _ => throw new NotImplementedException(),
            };
        }

        double scrollableLength;
        if (direction == MouseWheelDirection.Up || direction == MouseWheelDirection.Down)
        {
            scrollableLength = ScrollOwner.ScrollableHeight;
        }
        else
        {
            scrollableLength = ScrollOwner.ScrollableWidth;
        }
        currentDistance = Math.Clamp(currentDistance, 0, scrollableLength);

        var extraDeltaMultiplier = (1 - Math.Exp(-currentDistance / _mouseWheelBaseDelta / 5));
        var extraDeltaBase = Math.Min(scrollableLength * 1 / 10, _mouseWheelBaseDelta * 5);

        var delta = _mouseWheelBaseDelta + extraDeltaMultiplier * extraDeltaBase;
        return direction switch
        {
            MouseWheelDirection.Up => IntendedVerticalOffset - delta,
            MouseWheelDirection.Down => IntendedVerticalOffset + delta,
            MouseWheelDirection.Left => IntendedHorizontalOffset - delta,
            MouseWheelDirection.Right => IntendedHorizontalOffset + delta,
            _ => throw new NotImplementedException(),
        };
    }

    public bool CanVerticallyScroll
    {
        get => _child.CanVerticallyScroll;
        set => _child.CanVerticallyScroll = value;
    }
    public bool CanHorizontallyScroll
    {
        get => _child.CanHorizontallyScroll;
        set => _child.CanHorizontallyScroll = value;
    }

    public double ExtentWidth => _child.ExtentWidth;

    public double ExtentHeight => _child.ExtentHeight;

    public double ViewportWidth => _child.ViewportWidth;

    public double ViewportHeight => _child.ViewportHeight;

    public double HorizontalOffset => _child.HorizontalOffset;
    public double VerticalOffset => _child.VerticalOffset;

    public ScrollViewer ScrollOwner
    {
        get => _child.ScrollOwner;
        set => _child.ScrollOwner = value;
    }

    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
        return _child.MakeVisible(visual, rectangle);
    }

    public void LineUp()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(IntendedVerticalOffset - _scrollLineDelta);
        }
        else
        {
            _child.LineUp();
        }
    }

    public void LineDown()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(IntendedVerticalOffset + _scrollLineDelta);
        }
        else
        {
            _child.LineDown();
        }
    }

    public void LineLeft()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(IntendedHorizontalOffset - _scrollLineDelta);
        }
        else
        {
            _child.LineLeft();
        }
    }

    public void LineRight()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(IntendedHorizontalOffset + _scrollLineDelta);
        }
        else
        {
            _child.LineRight();
        }
    }

    public void MouseWheelUp()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(MouseWheelTargetValue(MouseWheelDirection.Up));
        }
        else
        {
            _child.MouseWheelUp();
        }
    }

    public void MouseWheelDown()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(MouseWheelTargetValue(MouseWheelDirection.Down));
        }
        else
        {
            _child.MouseWheelDown();
        }
    }

    public void MouseWheelLeft()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(MouseWheelTargetValue(MouseWheelDirection.Left));
        }
        else
        {
            _child.MouseWheelLeft();
        }
    }

    public void MouseWheelRight()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(MouseWheelTargetValue(MouseWheelDirection.Right));
        }
        else
        {
            _child.MouseWheelRight();
        }
    }

    public void PageUp()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(IntendedVerticalOffset - ViewportHeight);
        }
        else
        {
            _child.PageUp();
        }
    }

    public void PageDown()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(IntendedVerticalOffset + ViewportHeight);
        }
        else
        {
            _child.PageDown();
        }
    }

    public void PageLeft()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(IntendedHorizontalOffset - ViewportWidth);
        }
        else
        {
            _child.PageLeft();
        }
    }

    public void PageRight()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(IntendedHorizontalOffset + ViewportWidth);
        }
        else
        {
            _child.PageRight();
        }
    }

    public void ToHome()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(0);
            HorizontalScroll(0);
        }
        else
        {
            ScrollOwner.ScrollToHome();
        }
    }

    public void ToEnd()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(ScrollOwner.ScrollableHeight);
            HorizontalScroll(ScrollOwner.ScrollableWidth);
        }
        else
        {
            ScrollOwner.ScrollToEnd();
        }
    }

    public void SetHorizontalOffset(double offset)
    {
        if (AnimatedScrollingEnabled)
        {
            IntendedHorizontalOffset = offset;
            // here we use 0 duration, or it will lag when user drags scrollbar thumb
            Animate(AnimateAxis.Horizontal, 0);
        }
        else
        {
            _child.SetHorizontalOffset(offset);
        }
    }

    public void SetVerticalOffset(double offset)
    {
        if (AnimatedScrollingEnabled)
        {
            IntendedVerticalOffset = offset;
            // here we use 0 duration, or it will lag when user drags scrollbar thumb
            Animate(AnimateAxis.Vertical, 0); 
        }
        else
        {
            _child.SetVerticalOffset(offset);
        }
    }

    public void AnimatedScrollToHorizontalOffset(double offset)
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(offset);
        }
        else
        {
            _child.SetHorizontalOffset(offset);
        }
    }

    #region not exposed methods
    private enum AnimateAxis { Horizontal, Vertical }
    private readonly Storyboard _horizontalStoryboard = new();
    private readonly Storyboard _verticalStoryboard = new();
    private double _lastHorizontalAnimationStartOffset = 0;
    private double LastHorizontalAnimationStartOffset
    {
        get => _lastHorizontalAnimationStartOffset;
        set
        {
            if (double.IsNaN(value)) 
            { 
                return;
            }
            _lastHorizontalAnimationStartOffset = Math.Clamp(value, 0, ScrollOwner.ScrollableWidth);
        }
    }
    private double _lastVerticalAnimationStartOffset = 0;
    private double LastVerticalAnimationStartOffset
    {
        get => _lastVerticalAnimationStartOffset;
        set
        {
            if (double.IsNaN(value)) 
            { 
                return;
            }
            _lastVerticalAnimationStartOffset = Math.Clamp(value, 0, ScrollOwner.ScrollableHeight);
        }
    }
    private void Animate(AnimateAxis axis, double duration = -1)
    { 
        DependencyProperty property = axis == AnimateAxis.Horizontal ? HorizontalScrollOffsetProperty : VerticalScrollOffsetProperty;
        var storyboard = axis == AnimateAxis.Horizontal ? _horizontalStoryboard : _verticalStoryboard;
        var lastStartOffset = axis == AnimateAxis.Horizontal ? LastHorizontalAnimationStartOffset : LastVerticalAnimationStartOffset;
        var targetValue = axis == AnimateAxis.Horizontal ? IntendedHorizontalOffset : IntendedVerticalOffset;

        double currentValue;
        if (axis == AnimateAxis.Horizontal)
        {
            LastHorizontalAnimationStartOffset = (double)GetValue(property);
            currentValue = LastHorizontalAnimationStartOffset;
        }
        else
        {
            LastVerticalAnimationStartOffset = (double)GetValue(property);
            currentValue = LastVerticalAnimationStartOffset;
        }

        if (Math.Abs(targetValue - currentValue) < 0.1)
        {
            return;
        }

        if (duration < 0)
        {
            var durationFactor = 1 - Math.Exp(-Math.Abs(targetValue - currentValue) / _mouseWheelBaseDelta / 5);
            durationFactor = double.IsNaN(durationFactor) ? 0 : Math.Clamp(durationFactor, 0, 1);
            duration = 300 + 300 * durationFactor;
        }
        var isAnimationRunning = storyboard.Children.Count != 0 && storyboard.GetCurrentState() == ClockState.Active;
        var currentSpeed = 0.0;
        if (isAnimationRunning)
        {
            var previousAnimation = (DoubleAnimationUsingKeyFrames)storyboard.Children[0];
            var previousDuration = previousAnimation.Duration.TimeSpan.TotalMilliseconds;
            var previousSplineKeyFrame = previousAnimation.KeyFrames[0] as SplineDoubleKeyFrame;
            var previousProgress = storyboard.GetCurrentProgress();
            const double tick = 0.01;
            var splineProgressTickLater = previousSplineKeyFrame?.KeySpline.GetSplineProgress(previousProgress + tick);
            var valueTickLater = lastStartOffset * (1 - splineProgressTickLater) + previousSplineKeyFrame?.Value * splineProgressTickLater;
            currentSpeed = ((valueTickLater - currentValue) / (tick * previousDuration)) ?? 0.0;
            currentSpeed = double.IsNaN(currentSpeed) ? 0.0 : currentSpeed;
        }

        const double firstControlPointX = 0.5;
        var firstControlPointY = (currentSpeed * firstControlPointX * duration) / (targetValue - currentValue);
        const double secondControlPointX = 0.5;

        //make a smooth animation that starts and ends slowly
        var keyFramesAnimation = new DoubleAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromMilliseconds(duration)
        };
        var spline = new SplineDoubleKeyFrame(
                targetValue,
                KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration)),
                new KeySpline(firstControlPointX, firstControlPointY, secondControlPointX, 1.0)
                );
        keyFramesAnimation.KeyFrames.Add(spline);
        storyboard.Stop();
        storyboard.Children.Clear();
        storyboard.Children.Add(keyFramesAnimation);
        Storyboard.SetTarget(storyboard, this);
        Storyboard.SetTargetProperty(storyboard, new PropertyPath(property));
        storyboard.Begin();
        Application.Current.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
    }

    private void VerticalScroll(double val)
    {
        if (Math.Abs(val - IntendedVerticalOffset) < 0.1)
        {
            return;
        }
        IntendedVerticalOffset = val;
        Animate(AnimateAxis.Vertical);
    }

    private void HorizontalScroll(double val)
    {
        if (Math.Abs(val - IntendedHorizontalOffset) < 0.1)
        {
            return;
        }
        IntendedHorizontalOffset = val;
        Animate(AnimateAxis.Horizontal);
    }
    #endregion

    #region helper dependency properties as scrollbars are not animatable by default
    internal double VerticalScrollOffset
    {
        get => (double)GetValue(VerticalScrollOffsetProperty);
        set
        {
            if (double.IsNaN(value))
            { 
                return;
            }
            var newValue = Math.Clamp(value, 0, ScrollOwner.ScrollableHeight);
            SetValue(VerticalScrollOffsetProperty, newValue);
        }
    }
    internal static readonly DependencyProperty VerticalScrollOffsetProperty =
        DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(SmoothScrollInfoAdapter),
        new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalScrollOffsetChanged)));

    private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var smoothScrollViewer = (SmoothScrollInfoAdapter)d;
        var newValue = (double)e.NewValue;
        smoothScrollViewer._child.SetVerticalOffset(newValue);
    }

    internal double HorizontalScrollOffset
    {
        get => (double)GetValue(HorizontalScrollOffsetProperty);
        set
        {
            if (double.IsNaN(value)) 
            { 
                return; 
            }
            var newValue = Math.Clamp(value, 0, ScrollOwner.ScrollableWidth);
            SetValue(HorizontalScrollOffsetProperty, newValue);
        }
    }
    internal static readonly DependencyProperty HorizontalScrollOffsetProperty =
        DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(SmoothScrollInfoAdapter),
        new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalScrollOffsetChanged)));
    private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var smoothScrollViewer = (SmoothScrollInfoAdapter)d;
        var newValue = (double)e.NewValue;
        smoothScrollViewer._child.SetHorizontalOffset(newValue);
    }
    #endregion
}

public static class SmoothScrolling
{
    public static SmoothScrollInfoAdapter? Enable(ScrollViewer? scrollViewer)
    {
        var property = scrollViewer?.GetType().GetProperty("ScrollInfo", BindingFlags.NonPublic | BindingFlags.Instance);
        if (property?.GetValue(scrollViewer) is not IScrollInfo scrollInfo)
        {
            return null;
        }
        if (scrollInfo is SmoothScrollInfoAdapter oldAdaptor)
        {
            return oldAdaptor;
        }
        var newAdaptor = new SmoothScrollInfoAdapter(scrollInfo);
        property.SetValue(scrollViewer, newAdaptor);
        return newAdaptor;
    }
}