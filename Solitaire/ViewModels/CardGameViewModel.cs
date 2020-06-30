using System.Windows.Input;
using Apex.MVVM;

namespace Solitaire.ViewModels
{
    /// <summary>
    /// Base class for a ViewModel for a card game.
    /// </summary>
    public abstract class CardGameViewModel : ViewModel
    {
        #region Private Fields

        /// <summary>
        /// The victory flag.
        /// </summary>
        private readonly NotifyingProperty _isGameWonProperty =
            new NotifyingProperty(nameof(IsGameWon), typeof(bool), false);

        #endregion

        protected CardGameViewModel()
        {
            LeftClickCardCommand = new Command(LeftClickCardCommandExecute);
            RightClickCardCommand = new Command(RightClickCardCommandExecute);
            NewGameCommand = new Command(NewGameCommandExecute);
        }

        #region Public Preperties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is game won.
        /// </summary>
        public bool IsGameWon
        {
            get => (bool)GetValue(_isGameWonProperty);
            set => SetValue(_isGameWonProperty, value);
        }

        /// <summary>
        /// Gets the left click card command.
        /// </summary>
        public ICommand LeftClickCardCommand { get; private set; }

        /// <summary>
        /// Gets the right click card command.
        /// </summary>
        public ICommand RightClickCardCommand { get; private set; }

        /// <summary>
        /// Gets the deal new game command.
        /// </summary>
        public ICommand NewGameCommand { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// The left click card command.
        /// </summary>
        protected virtual void LeftClickCardCommandExecute(object parameter)
        {
        }

        /// <summary>
        /// The right click card command.
        /// </summary>
        protected virtual void RightClickCardCommandExecute(object parameter)
        {
        }

        /// <summary>
        /// Deals a new game.
        /// </summary>
        protected virtual void NewGameCommandExecute(object parameter)
        {
            IsGameWon = false;
        }

        #endregion
    }
}