using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WPF.MVVM.Example
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.MainWindow = ViewControlLocator.Instance.RegisterWindow<View.MainWindow, ViewModel.MainWindowViewModel>();
            this.MainWindow.Show();
        }
    }
}
