using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Solitaire.Models;

namespace Solitaire.Converters
{
    /// <summary>
    /// Converter to get the brush for a playing card.
    /// </summary>
    public class PlayingCardToBrushConverter : IMultiValueConverter
    {
        /// <summary>
        /// A dictionary of brushes for card types.
        /// </summary>
        private static readonly Dictionary<string, Brush> Brushes = new Dictionary<string, Brush>();

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                return null;
            }

            //  Cast the values.
            var cardType = (CardType) values[0];
            var faceDown = (bool) values[1];

            //  We're going to create an image source.

            //  If the card is face down, we're using the 'Rear' image.
            //  Otherwise it's just the enum value (e.g. C3, SA).
            var imageSource = faceDown ? "Back" : cardType.ToString();

            //  Turn this string into a proper path.
            imageSource = $"pack://application:,,,/Solitaire;component/Resources/Cards/{imageSource}.bmp";

            //  Add this brush to the static dictionary
            if (Brushes.ContainsKey(imageSource) == false)
            {
                Brushes.Add(imageSource, new ImageBrush(new BitmapImage(new Uri(imageSource))));
            }

            //  Return the brush.
            return Brushes[imageSource];
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}