using Apex.MVVM;
using Solitaire.Models;

namespace Solitaire.ViewModels
{
    /// <summary>
    /// The Playing Card represents a Card played in a game - so as
    /// well as the card type it also has the face down property etc.
    /// </summary>
    public class PlayingCard : ViewModel
    {
        #region Private Fields

        /// <summary>
        /// The card type notifying property.
        /// </summary>
        private readonly NotifyingProperty _cardTypeProperty =
            new NotifyingProperty(nameof(CardType), typeof(CardType), CardType.SA);

        /// <summary>
        /// The IsFaceDown notifying property.
        /// </summary>
        private readonly NotifyingProperty _isFaceDownProperty =
            new NotifyingProperty(nameof(IsFaceDown), typeof(bool), false);

        /// <summary>
        /// The IsPlayable notifying property.
        /// </summary>
        private readonly NotifyingProperty _isPlayableProperty =
            new NotifyingProperty(nameof(IsPlayable), typeof(bool), false);

        /// <summary>
        /// The FaceDown offset property.
        /// </summary>
        private readonly NotifyingProperty _faceDownOffsetProperty =
            new NotifyingProperty(nameof(FaceDownOffset), typeof(double), default(double));

        /// <summary>
        /// The FaceUp offset property.
        /// </summary>
        private readonly NotifyingProperty _faceUpOffsetProperty =
            new NotifyingProperty(nameof(FaceUpOffset), typeof(double), default(double));

        #endregion

        /// <summary>
        /// Gets the card suit.
        /// </summary> 
        /// <value>The card suit.</value>
        public CardSuit Suit
        {
            get
            {
                //  The suit can be worked out from the numeric value of the CardType enum.
                var enumVal = (int)CardType;

                if (enumVal < 13)
                {
                    return CardSuit.Hearts;
                }

                if (enumVal < 26)
                {
                    return CardSuit.Diamonds;
                }

                if (enumVal < 39)
                {
                    return CardSuit.Clubs;
                }

                return CardSuit.Spades;
            }
        }

        /// <summary>
        /// Gets the card value.
        /// </summary>
        public int Value => (int)CardType % 13; //  The CardType enum has 13 cards in each suit.

        /// <summary>
        /// Gets the card colour.
        /// </summary>
        public CardColour Colour =>
            (int)CardType < 26
                ? CardColour.Red
                : CardColour.Black; //  The first two suits in the CardType enum are red, the last two are black.

        /// <summary>
        /// Gets or sets the type of the card.
        /// </summary>
        public CardType CardType
        {
            get => (CardType)GetValue(_cardTypeProperty);
            set => SetValue(_cardTypeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is face down.
        /// </summary>
        public bool IsFaceDown
        {
            get => (bool)GetValue(_isFaceDownProperty);
            set => SetValue(_isFaceDownProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is playable.
        /// </summary>
        public bool IsPlayable
        {
            get => (bool)GetValue(_isPlayableProperty);
            set => SetValue(_isPlayableProperty, value);
        }


        /// <summary>
        /// Gets or sets the face down offset.
        /// </summary>
        public double FaceDownOffset
        {
            get => (double)GetValue(_faceDownOffsetProperty);
            set => SetValue(_faceDownOffsetProperty, value);
        }


        /// <summary>
        /// Gets or sets the face up offset.
        /// </summary>
        public double FaceUpOffset
        {
            get => (double)GetValue(_faceUpOffsetProperty);
            set => SetValue(_faceUpOffsetProperty, value);
        }
    }
}