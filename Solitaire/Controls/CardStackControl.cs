using System.Windows;
using System.Windows.Controls;
using Solitaire.Models;

namespace Solitaire.Controls
{
    public class CardStackControl : ItemsControl
    {
        private static readonly DependencyProperty FaceDownOffsetProperty =
            DependencyProperty.Register(nameof(FaceDownOffset), typeof(double), typeof(CardStackControl));

        private static readonly DependencyProperty FaceUpOffsetProperty =
            DependencyProperty.Register(nameof(FaceUpOffset), typeof(double), typeof(CardStackControl));

        private static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(CardStackControl),
                new PropertyMetadata(Orientation.Horizontal));

        private static readonly DependencyProperty OffsetModeProperty =
            DependencyProperty.Register(nameof(OffsetMode), typeof(OffsetMode), typeof(CardStackControl),
                new PropertyMetadata(OffsetMode.EveryCard));

        private static readonly DependencyProperty NValueProperty =
            DependencyProperty.Register(nameof(NValue), typeof(int), typeof(CardStackControl), new PropertyMetadata(1));

        static CardStackControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardStackControl),
                new FrameworkPropertyMetadata(typeof(CardStackControl)));
        }

        public double FaceDownOffset
        {
            get => (double) GetValue(FaceDownOffsetProperty);
            set => SetValue(FaceDownOffsetProperty, value);
        }

        public double FaceUpOffset
        {
            get => (double) GetValue(FaceUpOffsetProperty);
            set => SetValue(FaceUpOffsetProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation) GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public OffsetMode OffsetMode
        {
            get => (OffsetMode) GetValue(OffsetModeProperty);
            set => SetValue(OffsetModeProperty, value);
        }

        public int NValue
        {
            get => (int) GetValue(NValueProperty);
            set => SetValue(NValueProperty, value);
        }
    }
}