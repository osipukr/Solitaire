using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Apex.Extensions;
using Apex.MVVM;
using Solitaire.Models;

namespace Solitaire.ViewModels
{
    public class MainWindowViewModel : CardGameViewModel
    {
        //  For ease of access we have arrays of the foundations and tableaus.
        private readonly List<ObservableCollection<PlayingCard>> _foundations = new List<ObservableCollection<PlayingCard>>();
        private readonly List<ObservableCollection<PlayingCard>> _tableaus = new List<ObservableCollection<PlayingCard>>();

        public MainWindowViewModel()
        {
            //  Create the quick access arrays.
            _foundations.Add(Foundation1);
            _foundations.Add(Foundation2);
            _foundations.Add(Foundation3);
            _foundations.Add(Foundation4);

            _tableaus.Add(Tableau1);
            _tableaus.Add(Tableau2);
            _tableaus.Add(Tableau3);
            _tableaus.Add(Tableau4);
            _tableaus.Add(Tableau5);
            _tableaus.Add(Tableau6);
            _tableaus.Add(Tableau7);

            //  Create the turn stock command.
            TurnStockCommand = new Command(DoTurnStock);
        }

        public ObservableCollection<PlayingCard> Foundation1 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Foundation2 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Foundation3 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Foundation4 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Tableau1 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Tableau2 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Tableau3 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Tableau4 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Tableau5 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Tableau6 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Tableau7 { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Stock { get; } = new ObservableCollection<PlayingCard>();

        public ObservableCollection<PlayingCard> Waste { get; } = new ObservableCollection<PlayingCard>();

        /// <summary>
        /// Gets the turn stock command.
        /// </summary>
        public ICommand TurnStockCommand { get; }

        /// <summary>
        /// Gets the card collection for the specified card.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        public IList<PlayingCard> GetCardCollection(PlayingCard card)
        {
            if (Stock.Contains(card))
            {
                return Stock;
            }

            if (Waste.Contains(card))
            {
                return Waste;
            }

            foreach (var foundation in _foundations.Where(foundation => foundation.Contains(card)))
            {
                return foundation;
            }

            return _tableaus.FirstOrDefault(tableau => tableau.Contains(card));
        }

        /// <summary>
        /// Deals a new game.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void NewGameCommandExecute(object parameter)
        {
            //  Call the base, which stops the timer, clears
            //  the score etc.
            base.NewGameCommandExecute(parameter);

            //  Clear everything.
            Stock.Clear();
            Waste.Clear();

            foreach (var tableau in _tableaus)
            {
                tableau.Clear();
            }

            foreach (var foundation in _foundations)
            {
                foundation.Clear();
            }

            //  Create a list of card types.
            var eachCardType = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();

            //  Create a playing card from each card type.
            var playingCards = eachCardType.Select(cardType => new PlayingCard
            {
                CardType = cardType,
                IsFaceDown = true
            }).ToList();

            //  Shuffle the playing cards.
            playingCards.Shuffle();

            //  Now distribute them - do the tableaus first.
            for (var i = 0; i < 7; i++)
            {
                //  We have i face down cards and 1 face up card.
                for (var j = 0; j < i; j++)
                {
                    var faceDownCard = playingCards.First();

                    playingCards.Remove(faceDownCard);

                    faceDownCard.IsFaceDown = true;

                    _tableaus[i].Add(faceDownCard);
                }

                //  Add the face up card.
                var faceUpCard = playingCards.First();

                playingCards.Remove(faceUpCard);

                faceUpCard.IsFaceDown = false;
                faceUpCard.IsPlayable = true;

                _tableaus[i].Add(faceUpCard);
            }

            //  Finally we add every card that's left over to the stock.
            foreach (var playingCard in playingCards)
            {
                playingCard.IsFaceDown = true;
                playingCard.IsPlayable = false;

                Stock.Add(playingCard);
            }

            playingCards.Clear();
        }

        /// <summary>
        /// Turns cards from the stock into the waste.
        /// </summary>
        private void DoTurnStock()
        {
            //  If the stock is empty, put every card from the waste back into the stock.
            if (Stock.Count == 0)
            {
                foreach (var card in Waste)
                {
                    card.IsFaceDown = true;
                    card.IsPlayable = false;

                    Stock.Insert(0, card);
                }

                Waste.Clear();
            }
            else
            {
                //  Everything in the waste so far must now have no offset.
                foreach (var wasteCard in Waste)
                {
                    wasteCard.FaceUpOffset = 0;
                }

                if (Stock.Count > 0)
                {
                    var card = Stock.Last();

                    Stock.Remove(card);

                    card.IsFaceDown = false;
                    card.IsPlayable = false;
                    card.FaceUpOffset = 30;

                    Waste.Add(card);
                }
            }

            //  Everything in the waste must be not playable,
            //  apart from the top card.
            foreach (var wasteCard in Waste)
            {
                wasteCard.IsPlayable = wasteCard == Waste.Last();
            }
        }

        /// <summary>
        /// Tries the move all cards to appropriate foundations.
        /// </summary>
        public void TryMoveAllCardsToAppropriateFoundations()
        {
            //  Go through the top card in each tableau - keeping
            //  track of whether we moved one.
            var keepTrying = true;
            while (keepTrying)
            {
                var movedACard = false;
                if (Waste.Count > 0)
                {
                    if (TryMoveCardToAppropriateFoundation(Waste.Last()))
                    {
                        movedACard = true;
                    }
                }

                foreach (var tableau in _tableaus.Where(tableau => tableau.Count > 0)
                    .Where(tableau => TryMoveCardToAppropriateFoundation(tableau.Last())))
                {
                    movedACard = true;
                }

                //  We'll keep trying if we moved a card.
                keepTrying = movedACard;
            }
        }

        /// <summary>
        /// Tries the move the card to its appropriate foundation.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns>True if card moved.</returns>
        public bool TryMoveCardToAppropriateFoundation(PlayingCard card)
        {
            //  Try the top of the waste first.
            if (Waste.LastOrDefault() == card)
            {
                if (_foundations.Any(foundation => MoveCard(Waste, foundation, card, false)))
                {
                    return true;
                }
            }

            //  Is the card in a tableau?
            var inTableau = false;
            var i = 0;

            for (; i < _tableaus.Count && inTableau == false; i++)
            {
                inTableau = _tableaus[i].Contains(card);
            }

            //  It's if its not in a tablea and it's not the top
            //  of the waste, we cannot move it.
            if (inTableau == false)
            {
                return false;
            }

            //  Try and move to each foundation.
            return _foundations.Any(foundation => MoveCard(_tableaus[i - 1], foundation, card, false));
        }

        /// <summary>
        /// Moves the card.
        /// </summary>
        /// <param name="from">The set we're moving from.</param>
        /// <param name="to">The set we're moving to.</param>
        /// <param name="card">The card we're moving.</param>
        /// <param name="checkOnly">if set to <c>true</c> we only check if we CAN move, but don't actually move.</param>
        /// <returns>True if a card was moved.</returns>
        public bool MoveCard(
            ObservableCollection<PlayingCard> from,
            ObservableCollection<PlayingCard> to,
            PlayingCard card, bool checkOnly)
        {
            //  The trivial case is where from and to are the same.
            if (from == to)
            {
                return false;
            }

            //  Are we moving from the waste?
            if (from == Waste)
            {
                //  Are we moving to a foundation?
                if (_foundations.Contains(to))
                {
                    //  We can move to a foundation only if:
                    //  1. It is empty and we are an ace.
                    //  2. It is card SN and we are suit S and Number N+1
                    if ((to.Count != 0 || card.Value != 0) &&
                        (to.Count <= 0 || to.Last().Suit != card.Suit || to.Last().Value != card.Value - 1))
                    {
                        return false;
                    }
                }
                //  Are we moving to a tableau?
                else if (_tableaus.Contains(to))
                {
                    //  We can move to a tableau only if:
                    //  1. It is empty and we are a king.
                    //  2. It is card CN and we are color !C and Number N-1
                    if ((to.Count != 0 || card.Value != 12) &&
                        (to.Count <= 0 || to.Last().Colour == card.Colour || to.Last().Value != card.Value + 1))
                    {
                        return false;
                    }
                }
                //  Any other move from the waste is wrong.
                else
                {
                    return false;
                }
            }
            //  Are we moving from a tableau?
            else if (_tableaus.Contains(from))
            {
                //  Are we moving to a foundation?
                if (_foundations.Contains(to))
                {
                    //  We can move to a foundation only if:
                    //  1. It is empty and we are an ace.
                    //  2. It is card SN and we are suit S and Number N+1
                    if ((to.Count != 0 || card.Value != 0) &&
                        (to.Count <= 0 || to.Last().Suit != card.Suit || to.Last().Value != card.Value - 1))
                    {
                        return false;
                    }
                }
                //  Are we moving to another tableau?
                else if (_tableaus.Contains(to))
                {
                    //  We can move to a tableau only if:
                    //  1. It is empty and we are a king.
                    //  2. It is card CN and we are color !C and Number N-1
                    if ((to.Count != 0 || card.Value != 12) &&
                        (to.Count <= 0 || to.Last().Colour == card.Colour || to.Last().Value != card.Value + 1))
                    {
                        return false;
                    }
                }
                //  Any other move from a tableau is wrong.
                else
                {
                    return false;
                }
            }
            //  Are we moving from a foundation?
            else if (_foundations.Contains(from))
            {
                //  Are we moving to a tableau?
                if (_tableaus.Contains(to))
                {
                    //  We can move to a tableau only if:
                    //  1. It is empty and we are a king.
                    //  2. It is card CN and we are color !C and Number N-1
                    if ((to.Count != 0 || card.Value != 12) &&
                        (to.Count <= 0 || to.Last().Colour == card.Colour || to.Last().Value != card.Value + 1))
                    {
                        return false;
                    }
                }
                //  Are we moving to another foundation?
                else if (_foundations.Contains(to))
                {
                    //  We can move from a foundation to a foundation only 
                    //  if the source foundation has one card (the ace) and the
                    //  destination foundation has no cards).
                    if (from.Count != 1 || to.Count != 0)
                    {
                        return false;
                    }
                }
                //  Any other move from a foundation is wrong.
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            //  If we were just checking, we're done.
            if (checkOnly)
            {
                return true;
            }

            //  If we've got here we've passed all tests
            //  and move the card and update the score.
            DoMoveCard(from, to, card);

            //  If we have moved from the waste, we must 
            //  make sure that the top of the waste is playable.
            if (from == Waste && Waste.Count > 0)
            {
                Waste.Last().IsPlayable = true;
            }

            //  Check for victory.
            CheckForVictory();

            return true;
        }

        /// <summary>
        /// Actually moves the card.
        /// </summary>
        /// <param name="from">The stack to move from.</param>
        /// <param name="to">The stack to move to.</param>
        /// <param name="card">The card.</param>
        private void DoMoveCard(
            ObservableCollection<PlayingCard> from,
            ObservableCollection<PlayingCard> to,
            PlayingCard card)
        {
            //  Indentify the run of cards we're moving.
            var run = new List<PlayingCard>();

            for (var i = from.IndexOf(card); i < from.Count; i++)
            {
                run.Add(from[i]);
            }

            //  This function will move the card, as well as setting the 
            //  playable properties of the cards revealed.
            foreach (var runCard in run)
            {
                from.Remove(runCard);
            }

            foreach (var runCard in run)
            {
                to.Add(runCard);
            }

            //  Are there any cards left in the from pile?
            if (from.Count > 0)
            {
                //  Reveal the top card and make it playable.
                var topCard = from.Last();

                topCard.IsFaceDown = false;
                topCard.IsPlayable = true;
            }
        }

        /// <summary>
        /// Checks for victory.
        /// </summary>
        public void CheckForVictory()
        {
            //  We've won if every foundation is full.
            if (_foundations.Any(foundation => foundation.Count < 13))
            {
                return;
            }

            //  We've won.
            IsGameWon = true;
        }

        /// <summary>
        /// The right click card command.
        /// </summary>
        /// <param name="parameter">The parameter (should be a playing card).</param>
        protected override void RightClickCardCommandExecute(object parameter)
        {
            base.RightClickCardCommandExecute(parameter);

            //  Cast the card.
            if (!(parameter is PlayingCard card))
            {
                return;
            }

            //  Try and move it to the appropriate foundation.
            TryMoveCardToAppropriateFoundation(card);
        }
    }
}