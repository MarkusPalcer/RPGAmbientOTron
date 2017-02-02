﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AmbientOTron.Controls
{
    public partial class IconContainer
    {
        public IconContainer()
        {
            InitializeComponent();
        }

        private enum DesiredSize
        {
            Large,
            Medium,
            Small
        }

        public static readonly DependencyProperty IconNameProperty = DependencyProperty.Register("IconName", typeof(string), typeof(IconContainer), new PropertyMetadata(default(string), OnIconNameChanged));

        private static void OnIconNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = d as IconContainer;

            container?.OnIconNameChanged();
        }

        public string IconName
        {
            get { return (string) GetValue(IconNameProperty); }
            set { SetValue(IconNameProperty, value); }
        }

        private Viewbox large;
        private Viewbox medium;
        private Viewbox small;

        private DesiredSize size;

        private DesiredSize Size
        {
            get { return size; }
            set
            {
                if (size == value)
                    return;

                size = value;
                SelectContent();
            }
        }

        public void OnIconNameChanged()
        {
            large = TryFindResource($"{IconName}Large") as Viewbox;
            medium = TryFindResource($"{IconName}Medium") as Viewbox;
            small = TryFindResource($"{IconName}Small") as Viewbox;

            SelectContent();
        }

        private void SelectContent()
        {
            switch (Size)
            {
                case DesiredSize.Large:
                    Content = large ?? medium ?? small;
                    break;
                case DesiredSize.Medium:
                    Content = medium ?? small ?? large;
                    break;
                case DesiredSize.Small:
                    Content = small ?? medium ?? large;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newSize = Math.Min(e.NewSize.Height, e.NewSize.Width);

            if (newSize < 32)
            {
                Size = DesiredSize.Small;
            }
            else if (newSize > 64)
            {
                Size = DesiredSize.Large;
            }
            else
            {
                Size = DesiredSize.Medium;
            }
        }
    }
}
