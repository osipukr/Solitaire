using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Apex.DragAndDrop;
using Solitaire.ViewModels;

namespace Solitaire
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            //  Wire up the drag and drop host.
            DragAndDropHost.DragAndDropStart += Instance_DragAndDropStart;
            DragAndDropHost.DragAndDropContinue += Instance_DragAndDropContinue;
            DragAndDropHost.DragAndDropEnd += Instance_DragAndDropEnd;
        }

        void Instance_DragAndDropEnd(object sender, DragAndDropEventArgs args)
        {
            //  We've put cards temporarily in the drag stack, put them in the 
            //  source stack again.                
            foreach (var dragCard in draggingCards)
                ((ObservableCollection<PlayingCard>)((ItemsControl)args.DragSource).ItemsSource).Add(dragCard);

            //  If we have a drop target, move the card.
            if (args.DropTarget != null)
            {
                //  Move the card.
                ViewModel.MoveCard(
                    (ObservableCollection<PlayingCard>)((ItemsControl)args.DragSource).ItemsSource,
                    (ObservableCollection<PlayingCard>)((ItemsControl)args.DropTarget).ItemsSource,
                    (PlayingCard)args.DragData, false);
            }
        }

        void Instance_DragAndDropContinue(object sender, DragAndDropEventArgs args)
        {
            args.Allow = true;
        }

        void Instance_DragAndDropStart(object sender, DragAndDropEventArgs args)
        {
            //  The data should be a playing card.
            var card = args.DragData as PlayingCard;
            if (card == null || card.IsPlayable == false)
            {
                args.Allow = false;
                return;
            }
            args.Allow = true;

            //  If the card is draggable, we're going to want to drag the whole
            //  stack.
            var cards = ViewModel.GetCardCollection(card);
            draggingCards = new List<PlayingCard>();
            var start = cards.IndexOf(card);
            for (var i = start; i < cards.Count; i++)
                draggingCards.Add(cards[i]);

            //  Clear the drag stack.
            DragStack.ItemsSource = draggingCards;
            DragStack.UpdateLayout();
            args.DragAdorner = new Apex.Adorners.VisualAdorner(DragStack);

            //  Hide each dragging card.
            var sourceStack = args.DragSource as ItemsControl;

            foreach (var dragCard in draggingCards)
            {
                ((ObservableCollection<PlayingCard>)sourceStack.ItemsSource).Remove(dragCard);
            }
        }


        /// <summary>
        /// The ViewModel dependency property.
        /// </summary>
        private static readonly DependencyProperty ViewModelProperty =
          DependencyProperty.Register(nameof(ViewModel), typeof(MainWindowViewModel), typeof(MainWindow),
          new PropertyMetadata(new MainWindowViewModel()));

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public MainWindowViewModel ViewModel
        {
            get => (MainWindowViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Handles the MouseRightButtonDown event of the dragAndDropHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void dragAndDropHost_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.TryMoveAllCardsToAppropriateFoundations();
        }

        /// <summary>
        /// Temporary storage for cards being dragged.
        /// </summary>
        private List<PlayingCard> draggingCards;

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the CardStackControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void CardStackControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.TurnStockCommand.Execute(null);
        }
    }
}