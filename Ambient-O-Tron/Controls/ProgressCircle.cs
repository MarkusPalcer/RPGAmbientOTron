using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace AmbientOTron.Controls
{
  public class ProgressCircle : Control
  {
    public ProgressCircle()
    {
      SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      RecalculateRenderProperties();
    }

    #region Overrides of FrameworkElement

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      RecalculateRenderProperties();
    }

    #endregion

    #region Minimum Dependency Property

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      "Minimum",
      typeof(double),
      typeof(ProgressCircle),
      new UIPropertyMetadata(0.0, OnMinimumChanged, CoerceMinimum));

    private static object CoerceMinimum(DependencyObject d, object value)
    {
      var pc = (ProgressCircle) d;
      var minimum = (double) value;
      if (minimum > pc.Maximum)
        minimum = pc.Maximum;

      return minimum;
    }

    private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var pc = d as ProgressCircle;
      pc?.CoerceValue(MinimumProperty);
      pc?.CoerceValue(ValueProperty);
      pc?.RecalculateRenderProperties();
    }

    public double Minimum
    {
      get { return (double) GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    #endregion

    #region Maximum Dependency Property

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      "Maximum",
      typeof(double),
      typeof(ProgressCircle),
      new PropertyMetadata(100.0, OnMaximumChanged, CoerceMaximum));

    private static object CoerceMaximum(DependencyObject d, object basevalue)
    {
      var pc = (ProgressCircle) d;
      var maximum = (double) basevalue;

      if (maximum < pc.Minimum)
        maximum = pc.Minimum;

      return maximum;
    }

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var pc = d as ProgressCircle;
      pc?.CoerceValue(MaximumProperty);
      pc?.CoerceValue(ValueProperty);
      pc?.RecalculateRenderProperties();
    }

    public double Maximum
    {
      get { return (double) GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    #endregion

    #region Value Dependency Property

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value",
      typeof(double),
      typeof(ProgressCircle),
      new PropertyMetadata(100.0, OnValueChanged, CoerceValue));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var pc = d as ProgressCircle;
      pc?.CoerceValue(ValueProperty);
      pc?.RecalculateRenderProperties();
    }

    private static object CoerceValue(DependencyObject d, object basevalue)
    {
      var pc = (ProgressCircle) d;
      var value = (double) basevalue;

      if (value < pc.Minimum)
        value = pc.Minimum;

      if (value > pc.Maximum)
        value = pc.Maximum;

      return value;
    }

    public double Value
    {
      get { return (double) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    #endregion

    #region Thickness Dependency Property

    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
      "Thickness",
      typeof(double),
      typeof(ProgressCircle),
      new PropertyMetadata(1.0));

    public double Thickness
    {
      get { return (double) GetValue(ThicknessProperty); }
      set { SetValue(ThicknessProperty, value); }
    }

    #endregion

    #region TrackColor Dependency Property

    public static readonly DependencyProperty TrackColorProperty = DependencyProperty.Register(
      "TrackColor",
      typeof(Brush),
      typeof(ProgressCircle),
      new PropertyMetadata(Brushes.LightGray));

    public Brush TrackColor
    {
      get { return (Brush) GetValue(TrackColorProperty); }
      set { SetValue(TrackColorProperty, value); }
    }

    #endregion

    #region IsIndeterminate Property

    public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
      "IsIndeterminate",
      typeof(bool),
      typeof(ProgressCircle),
      new UIPropertyMetadata(false, OnIsIndeterminateChanged, CoerceIsIndeterminate));

    private static object CoerceIsIndeterminate(DependencyObject d, object basevalue)
    {
      var pc = (ProgressCircle)d;
      var value = (bool)basevalue;

      if (double.IsNaN(pc.Maximum)
        ||double.IsNaN(pc.Minimum)
        ||double.IsNaN(pc.Value)
        ||(pc.Maximum - pc.Minimum < double.Epsilon))
      {
        value = false;
      }

      return value;
    }

    private static void OnIsIndeterminateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var pc = d as ProgressCircle;
      pc?.CoerceValue(IsIndeterminateProperty);
    }

    public bool IsIndeterminate
    {
      get { return (bool) GetValue(IsIndeterminateProperty); }
      set { SetValue(IsIndeterminateProperty, value); }
    }

    #endregion

    #region Calculated properties

    public static readonly DependencyProperty ArcDegreesProperty = DependencyProperty.Register(
      "ArcDegrees",
      typeof(double),
      typeof(ProgressCircle),
      new PropertyMetadata(180.0));

    private double ArcDegrees
    {
      get { return (double) GetValue(ArcDegreesProperty); }
      set { SetValue(ArcDegreesProperty, value); }
    }

    private void RecalculateRenderProperties()
    {
      if ((Maximum - Minimum < double.Epsilon) || IsIndeterminate)
      {
        return;
      }

      var progress = (Value - Minimum)/(Maximum - Minimum);
      ArcDegrees = progress*360.0;
    }

    #endregion
  }
}
