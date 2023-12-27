using System;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;

namespace ChatGptApiClientV2;

// from https://www.wpf-controls.com/wpf-smooth-scroll-viewer/
public class SmoothScrollInfoAdapter : UIElement, IScrollInfo
{
    public bool AnimatedScrollingEnabled { get; set; } = true;

    private readonly IScrollInfo child;
    private double intendedVerticalOffset;
    private double IntendedVerticalOffset
    {
        get => intendedVerticalOffset;
        set
        {
            if(double.IsNaN(value)) 
            { 
                return; 
            }
            intendedVerticalOffset = Math.Clamp(value, 0, ScrollOwner.ScrollableHeight);
        }
    }
    private double intendedHorizontalOffset;
    private double IntendedHorizontalOffset
    {
        get => intendedHorizontalOffset;
        set
        {
            if(double.IsNaN(value)) 
            { 
                return; 
            }
            intendedHorizontalOffset = Math.Clamp(value, 0, ScrollOwner.ScrollableWidth);
        }
    }
    private const double ScrollLineDelta = 16.0; 
    private const double MouseWheelBaseDelta = 48.0;

    private enum MouseWheelDirection { Up, Down, Left, Right }
    private double MouseWheelTargetValue(MouseWheelDirection direction)
    {
        var currentDistance = direction switch
        {
            MouseWheelDirection.Up => VerticalOffset - IntendedVerticalOffset,
            MouseWheelDirection.Down => IntendedVerticalOffset - VerticalOffset,
            MouseWheelDirection.Left => HorizontalOffset - IntendedHorizontalOffset,
            MouseWheelDirection.Right => IntendedHorizontalOffset - HorizontalOffset,
            _ => throw new InvalidOperationException(),
        };

        if (currentDistance < 0)
        {
            // brake
            return direction switch
            {
                MouseWheelDirection.Up => VerticalOffset - MouseWheelBaseDelta,
                MouseWheelDirection.Down => VerticalOffset + MouseWheelBaseDelta,
                MouseWheelDirection.Left => HorizontalOffset - MouseWheelBaseDelta,
                MouseWheelDirection.Right => HorizontalOffset + MouseWheelBaseDelta,
                _ => throw new InvalidOperationException(),
            };
        }

        var scrollableLength = direction is MouseWheelDirection.Up or MouseWheelDirection.Down 
            ? ScrollOwner.ScrollableHeight : ScrollOwner.ScrollableWidth;
        currentDistance = Math.Clamp(currentDistance, 0, scrollableLength);

        var extraDeltaMultiplier = (1 - Math.Exp(-currentDistance / MouseWheelBaseDelta / 5));
        var extraDeltaBase = Math.Min(scrollableLength * 1 / 10, MouseWheelBaseDelta * 5);

        var delta = MouseWheelBaseDelta + extraDeltaMultiplier * extraDeltaBase;
        return direction switch
        {
            MouseWheelDirection.Up => IntendedVerticalOffset - delta,
            MouseWheelDirection.Down => IntendedVerticalOffset + delta,
            MouseWheelDirection.Left => IntendedHorizontalOffset - delta,
            MouseWheelDirection.Right => IntendedHorizontalOffset + delta,
            _ => throw new InvalidOperationException(),
        };
    }

    public bool CanVerticallyScroll
    {
        get => child.CanVerticallyScroll;
        set => child.CanVerticallyScroll = value;
    }
    public bool CanHorizontallyScroll
    {
        get => child.CanHorizontallyScroll;
        set => child.CanHorizontallyScroll = value;
    }

    public double ExtentWidth => child.ExtentWidth;

    public double ExtentHeight => child.ExtentHeight;

    public double ViewportWidth => child.ViewportWidth;

    public double ViewportHeight => child.ViewportHeight;

    public double HorizontalOffset => child.HorizontalOffset;
    public double VerticalOffset => child.VerticalOffset;

    public ScrollViewer ScrollOwner
    {
        get => child.ScrollOwner;
        set => child.ScrollOwner = value;
    }

    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
        return child.MakeVisible(visual, rectangle);
    }

    public void LineUp()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(IntendedVerticalOffset - ScrollLineDelta);
        }
        else
        {
            child.LineUp();
        }
    }

    public void LineDown()
    {
        if (AnimatedScrollingEnabled)
        {
            VerticalScroll(IntendedVerticalOffset + ScrollLineDelta);
        }
        else
        {
            child.LineDown();
        }
    }

    public void LineLeft()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(IntendedHorizontalOffset - ScrollLineDelta);
        }
        else
        {
            child.LineLeft();
        }
    }

    public void LineRight()
    {
        if (AnimatedScrollingEnabled)
        {
            HorizontalScroll(IntendedHorizontalOffset + ScrollLineDelta);
        }
        else
        {
            child.LineRight();
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
            child.MouseWheelUp();
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
            child.MouseWheelDown();
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
            child.MouseWheelLeft();
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
            child.MouseWheelRight();
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
            child.PageUp();
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
            child.PageDown();
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
            child.PageLeft();
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
            child.PageRight();
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
            child.SetHorizontalOffset(offset);
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
            child.SetVerticalOffset(offset);
        }
    }

    #region not exposed methods
    private enum AnimateAxis { Horizontal, Vertical }

    private class AnimationInfo(SmoothScrollInfoAdapter owner, AnimateAxis axis)
    {
        public readonly Storyboard Storyboard = new();
        private double startOffset;
        public double StartOffset
        {
            get => startOffset;
            set
            {
                if (double.IsNaN(value))
                {
                    return;
                }
                startOffset = Math.Clamp(value, 0, axis == AnimateAxis.Horizontal ? owner.ScrollOwner.ScrollableWidth : owner.ScrollOwner.ScrollableHeight);
            }
        }
        public DependencyProperty Property => axis == AnimateAxis.Horizontal ? HorizontalScrollOffsetProperty : VerticalScrollOffsetProperty;
        public double TargetValue => axis == AnimateAxis.Horizontal ? owner.IntendedHorizontalOffset : owner.IntendedVerticalOffset;
    }

    private readonly AnimationInfo horizontalAnimation;
    private readonly AnimationInfo verticalAnimation;

    public SmoothScrollInfoAdapter(IScrollInfo child)
    {
        this.child = child;
        horizontalAnimation = new AnimationInfo(this, AnimateAxis.Horizontal);
        verticalAnimation = new AnimationInfo(this, AnimateAxis.Vertical);
    }
    
    private void Animate(AnimateAxis axis, double duration = -1)
    {
        var animation = axis == AnimateAxis.Horizontal ? horizontalAnimation : verticalAnimation;
        var lastStartOffset = animation.StartOffset;
        animation.StartOffset = (double)GetValue(animation.Property);
        var moveVector = animation.TargetValue - animation.StartOffset;
        var moveDistance = Math.Abs(moveVector);
        if (moveDistance < 0.1)
        {
            return;
        }

        if (duration < 0)
        {
            var durationFactor = 1 - Math.Exp(-moveDistance / MouseWheelBaseDelta / 5);
            durationFactor = double.IsNaN(durationFactor) ? 0 : Math.Clamp(durationFactor, 0, 1);
            duration = 300 + 300 * durationFactor;
        }
        var isAnimationRunning = animation.Storyboard.Children.Count != 0 && animation.Storyboard.GetCurrentState() == ClockState.Active;
        var currentSpeed = 0.0;
        if (isAnimationRunning)
        {
            var previousAnimation = (DoubleAnimationUsingKeyFrames)animation.Storyboard.Children[0];
            var previousDuration = previousAnimation.Duration.TimeSpan.TotalMilliseconds;
            var previousSplineKeyFrame = previousAnimation.KeyFrames[0] as SplineDoubleKeyFrame;
            var previousProgress = animation.Storyboard.GetCurrentProgress();
            const double tick = 0.01;
            var splineProgressTickLater = previousSplineKeyFrame?.KeySpline.GetSplineProgress(previousProgress + tick);
            var valueTickLater = lastStartOffset * (1 - splineProgressTickLater) + previousSplineKeyFrame?.Value * splineProgressTickLater;
            currentSpeed = ((valueTickLater - animation.StartOffset) / (tick * previousDuration)) ?? 0.0;
            currentSpeed = double.IsNaN(currentSpeed) ? 0.0 : currentSpeed;
        }

        const double firstControlPointX = 0.5;
        var firstControlPointY = (currentSpeed * firstControlPointX * duration) / moveVector;
        const double secondControlPointX = 0.5;

        //make a smooth animation that starts and ends slowly
        var keyFramesAnimation = new DoubleAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromMilliseconds(duration)
        };
        var spline = new SplineDoubleKeyFrame(
            animation.TargetValue,
            KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration)),
            new KeySpline(firstControlPointX, firstControlPointY, secondControlPointX, 1.0)
            );
        keyFramesAnimation.KeyFrames.Add(spline);
        keyFramesAnimation.Freeze();
        animation.Storyboard.Children.Clear();
        animation.Storyboard.Children.Add(keyFramesAnimation);
        Storyboard.SetTarget(animation.Storyboard, this);
        Storyboard.SetTargetProperty(animation.Storyboard, new PropertyPath(animation.Property));
        animation.Storyboard.Begin();
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
        DependencyProperty.Register(nameof(VerticalScrollOffset), typeof(double), typeof(SmoothScrollInfoAdapter),
        new PropertyMetadata(0.0, OnVerticalScrollOffsetChanged));

    private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var smoothScrollViewer = (SmoothScrollInfoAdapter)d;
        var newValue = (double)e.NewValue;
        smoothScrollViewer.child.SetVerticalOffset(newValue);
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
        DependencyProperty.Register(nameof(HorizontalScrollOffset), typeof(double), typeof(SmoothScrollInfoAdapter),
        new PropertyMetadata(0.0, OnHorizontalScrollOffsetChanged));
    private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var smoothScrollViewer = (SmoothScrollInfoAdapter)d;
        var newValue = (double)e.NewValue;
        smoothScrollViewer.child.SetHorizontalOffset(newValue);
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