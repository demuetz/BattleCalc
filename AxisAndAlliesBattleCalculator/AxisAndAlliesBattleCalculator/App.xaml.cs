using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using AxisAndAlliesBattleCalculator.ViewModel;
using System.Windows.Controls;
using Microsoft.Windows.Controls;

namespace AxisAndAlliesBattleCalculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            EventManager.RegisterClassHandler(typeof(IntegerUpDown), UIElement.GotFocusEvent,
                                     new RoutedEventHandler(SelectAllText), true);

            base.OnStartup(e);

            MainWindow window = new MainWindow();

            var viewModel = new MainWindowViewModel();

            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            window.DataContext = viewModel;

            window.Show();
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var control = e.OriginalSource as IntegerUpDown;
            if (control != null)
                control.Focus();
        }
    }
}
