using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Apex.Adorners;
using Apex.DragAndDrop;
using Solitaire.ViewModels;
using Solitaire.Windows;

namespace Solitaire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Temporary storage for cards being dragged.
        /// </summary>
        private List<PlayingCard> _draggingCards;

        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = (MainWindowViewModel)DataContext;

            //  Wire up the drag and drop host.
            DragAndDropHost.DragAndDropStart += Instance_DragAndDropStart;
            DragAndDropHost.DragAndDropContinue += Instance_DragAndDropContinue;
            DragAndDropHost.DragAndDropEnd += Instance_DragAndDropEnd;
        }

        private void Instance_DragAndDropEnd(object sender, DragAndDropEventArgs args)
        {
            var itemsControl = (ItemsControl)args.DragSource;
            var playingCards = (ObservableCollection<PlayingCard>)itemsControl.ItemsSource;

            //  We've put cards temporarily in the drag stack, put them in the 
            //  source stack again.                
            foreach (var dragCard in _draggingCards)
            {
                playingCards.Add(dragCard);
            }

            //  If we have a drop target, move the card.
            if (args.DropTarget != null)
            {
                var dropTarget = (ItemsControl)args.DropTarget;
                var dragPayingCards = (ObservableCollection<PlayingCard>)dropTarget.ItemsSource;
                var dragPlayingCard = (PlayingCard)args.DragData;

                //  Move the card.
                _viewModel.MoveCard(playingCards, dragPayingCards, dragPlayingCard, false);
            }
        }

        private void Instance_DragAndDropContinue(object sender, DragAndDropEventArgs args)
        {
            args.Allow = true;
        }

        private void Instance_DragAndDropStart(object sender, DragAndDropEventArgs args)
        {
            //  The data should be a playing card.

            if (!(args.DragData is PlayingCard card) || card.IsPlayable == false)
            {
                args.Allow = false;

                return;
            }

            args.Allow = true;

            //  If the card is draggable, we're going to want to drag the whole stack.
            var cards = _viewModel.GetCardCollection(card);

            _draggingCards = new List<PlayingCard>();

            var start = cards.IndexOf(card);

            for (var i = start; i < cards.Count; i++)
            {
                _draggingCards.Add(cards[i]);
            }

            //  Clear the drag stack.
            DragStack.ItemsSource = _draggingCards;

            DragStack.UpdateLayout();

            args.DragAdorner = new VisualAdorner(DragStack);

            var sourceStack = (ItemsControl)args.DragSource;
            var playingCards = (ObservableCollection<PlayingCard>)sourceStack.ItemsSource;

            //  Hide each dragging card.
            foreach (var dragCard in _draggingCards)
            {
                playingCards.Remove(dragCard);
            }
        }

        /// <summary>
        /// Handles the MouseRightButtonDown event of the dragAndDropHost control.
        /// </summary>
        private void DragAndDropHost_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.TryMoveAllCardsToAppropriateFoundations();
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the CardStackControl control.
        /// </summary>
        private void CardStackControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _viewModel.TurnStockCommand.Execute(null);
        }

        private void CloseEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void AboutGameEventHandler(object sender, RoutedEventArgs e)
        {
            var window = new AboutGame
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            try
            {
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "О игре", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AboutDeveloperEventHandler(object sender, RoutedEventArgs e)
        {
            var window = new AboutDeveloper
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            try
            {
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "О разработчике", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}