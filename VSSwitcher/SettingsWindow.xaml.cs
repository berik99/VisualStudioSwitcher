using System.Windows;
using VSSwitcher.Services;

namespace VSSwitcher
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            VSGrid.ItemsSource = Application.Configuration.VSList;
            SLNGrid.ItemsSource = Application.Configuration.Solutions;
        }
    }
}
