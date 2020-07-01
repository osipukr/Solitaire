using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Solitaire.Windows
{
    /// <summary>
    /// Interaction logic for AboutDeveloper.xaml
    /// </summary>
    public partial class AboutDeveloper
    {
        public AboutDeveloper()
        {
            InitializeComponent();
        }

        private void HyperlinkRequestNavigateEventHandler(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "О разработчике", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            e.Handled = true;
        }
    }
}