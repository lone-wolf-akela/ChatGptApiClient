using System;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using SharpVectors.Dom.Css;

namespace ChatGptApiClientV2;

// from https://www.wpf-controls.com/wpf-smooth-scroll-viewer/
public class SmoothScrollInfoAdapter(IScrollInfo child) : UIElement, IScrollInfo
{
    public bool AnimatedScrollingEnabled { get; set; } = true;

    private readonly IScrollInfo _child = child;
    private double _computedVerticalOffset = 0;
    private double _computedHorizontalOffset = 0;
    internal const double _scrollLineDelta = 16.0;
    internal const double _mouseWheelDelta = 48.0;

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

    public System.Windows.Controls.ScrollViewer ScrollOwner
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
            VerticalScroll(_computedVerticalOffset - _scrollLineDelta);
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
            VerticalScroll(_computedVerticalOffset + _scrollLineDelta);
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
            HorizontalScroll(_computedHorizontalOffset - _scrollLineDelta);
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
            HorizontalScroll(_computedHorizontalOffset + _scrollLineDelta);
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
            VerticalScroll(_computedVerticalOffset - _mouseWheelDelta);
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
            VerticalScroll(_computedVerticalOffset + _mouseWheelDelta);
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
            HorizontalScroll(_computedHorizontalOffset - _mouseWheelDelta);
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
            HorizontalScroll(_computedHorizontalOffset + _mouseWheelDelta);
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
            VerticalScroll(_computedVerticalOffset - ViewportHeight);
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
            VerticalScroll(_computedVerticalOffset + ViewportHeight);
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
            HorizontalScroll(_computedHorizontalOffset - ViewportWidth);
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
            HorizontalScroll(_computedHorizontalOffset + ViewportWidth);
        }
        else
        {
            _child.PageRight();
        }
    }

    public void SetHorizontalOffset(double offset)
    {
        if (AnimatedScrollingEnabled)
        {
            _computedHorizontalOffset = offset;
            // here we use 0 duration, or it will lag when user drags scrollbar thumb
            Animate(HorizontalScrollOffsetProperty, offset, AnimateAxis.Horizontal, 0);
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
            _computedVerticalOffset = offset;
            // here we use 0 duration, or it will lag when user drags scrollbar thumb
            Animate(VerticalScrollOffsetProperty, offset, AnimateAxis.Vertical, 0); 
        }
        else
        {
            _child.SetVerticalOffset(offset);
        }
    }

    public void AnimatedScrollToVerticalOffset(double offset)
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(offset);
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
    private double _previousHorizontalAnimationStartOffset = 0;
    private double _previousVerticalAnimationStartOffset = 0;
    private void Animate(DependencyProperty property, double targetValue, AnimateAxis axis, double duration = -1)
    {
        var storyboard = axis == AnimateAxis.Horizontal ? _horizontalStoryboard : _verticalStoryboard;
        var previousBaseValue = axis == AnimateAxis.Horizontal ? _previousHorizontalAnimationStartOffset : _previousVerticalAnimationStartOffset;

        var currentValue = (double)GetValue(property);
        if (axis == AnimateAxis.Horizontal)
        {
            _previousHorizontalAnimationStartOffset = currentValue;
        }
        else
        {
            _previousVerticalAnimationStartOffset = currentValue;
        }

        if (duration < 0)
        {
            var durationFactor = 1 - Math.Exp((1 - Math.Abs(targetValue - currentValue) / _mouseWheelDelta) / 5);
            duration = 300 + 300 * durationFactor;
        }
        var isAnimationRunning = storyboard.Children.Count != 0 && storyboard.GetCurrentState() == ClockState.Active;
        var currentSpeed = 0.0;
        if (isAnimationRunning)
        {
            var previousAnimation = (DoubleAnimationUsingKeyFrames)storyboard.Children[0];
            var previousDuration = previousAnimation.Duration.TimeSpan.TotalMilliseconds;
            var previousSplineKeyFrame = previousAnimation.KeyFrames[0];
            var previousProgress = storyboard.GetCurrentProgress();
            var InterpolateValueCore = typeof(DoubleKeyFrame).GetMethod("InterpolateValueCore", BindingFlags.NonPublic | BindingFlags.Instance);
            const double tick = 0.01;
            var valueTickLater = (double?)InterpolateValueCore?.Invoke(previousSplineKeyFrame, new object[] { previousBaseValue, previousProgress + tick });
            currentSpeed = ((valueTickLater - currentValue) / (tick * previousDuration)) ?? 0.0;
        }

        const double firstControlPointX = 0.3;
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
        storyboard.Children.Clear();
        storyboard.Children.Add(keyFramesAnimation);
        Storyboard.SetTarget(storyboard, this);
        Storyboard.SetTargetProperty(storyboard, new PropertyPath(property));
        storyboard.Begin();
    }

    private void VerticalScroll(double val)
    {
        if (Math.Abs(_computedVerticalOffset - ValidateVerticalOffset(val)) > 0.1)//prevent restart of animation in case of frequent event fire
        {
            _computedVerticalOffset = ValidateVerticalOffset(val);
            Animate(VerticalScrollOffsetProperty, _computedVerticalOffset, AnimateAxis.Vertical);
        }
    }

    private void HorizontalScroll(double val)
    {
        if (Math.Abs(_computedHorizontalOffset - ValidateHorizontalOffset(val)) > 0.1)//prevent restart of animation in case of frequent event fire
        {
            _computedHorizontalOffset = ValidateHorizontalOffset(val);
            Animate(HorizontalScrollOffsetProperty, _computedHorizontalOffset, AnimateAxis.Horizontal);
        }
    }

    private double ValidateVerticalOffset(double verticalOffset)
    {
        if (verticalOffset < 0)
        {
            return 0;
        }
        if (verticalOffset > _child.ScrollOwner.ScrollableHeight)
        {
            return _child.ScrollOwner.ScrollableHeight;
        }
        return verticalOffset;
    }

    private double ValidateHorizontalOffset(double horizontalOffset)
    {
        if (horizontalOffset < 0)
        {
            return 0;
        }
        if (horizontalOffset > _child.ScrollOwner.ScrollableWidth)
        {
            return _child.ScrollOwner.ScrollableWidth;
        }
        return horizontalOffset;
    }
    #endregion

    #region helper dependency properties as scrollbars are not animatable by default
    internal double VerticalScrollOffset
    {
        get => (double)GetValue(VerticalScrollOffsetProperty);
        set => SetValue(VerticalScrollOffsetProperty, value);
    }
    internal static readonly DependencyProperty VerticalScrollOffsetProperty =
        DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(SmoothScrollInfoAdapter),
        new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalScrollOffsetChanged)));

    private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var smoothScrollViewer = (SmoothScrollInfoAdapter)d;
        var newValue = smoothScrollViewer.ValidateVerticalOffset((double)e.NewValue);
        smoothScrollViewer._child.SetVerticalOffset(newValue);
    }

    internal double HorizontalScrollOffset
    {
        get => (double)GetValue(HorizontalScrollOffsetProperty);
        set => SetValue(HorizontalScrollOffsetProperty, value);
    }
    internal static readonly DependencyProperty HorizontalScrollOffsetProperty =
        DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(SmoothScrollInfoAdapter),
        new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalScrollOffsetChanged)));
    private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var smoothScrollViewer = (SmoothScrollInfoAdapter)d;
        var newValue = smoothScrollViewer.ValidateHorizontalOffset((double)e.NewValue);
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