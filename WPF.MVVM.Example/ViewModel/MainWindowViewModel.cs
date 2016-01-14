using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WPF.MVVM.Core;

namespace WPF.MVVM.Example.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel() 
        {

        }

        #region  attribute
        #endregion

        #region  command
        public ICommand ShowCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var firstW = ViewControlLocator.Instance.RegisterWindow<View.FirstWindow, ViewModel.FirstWindowViewModel>();
                    firstW.Show();

                    var secondW = ViewControlLocator.Instance.RegisterWindow<View.SecondWindow, ViewModel.SecondWindowViewModel>();
                    secondW.Show();
                });
            }
        }
        #endregion

        #region  method

        #endregion
    }
}
