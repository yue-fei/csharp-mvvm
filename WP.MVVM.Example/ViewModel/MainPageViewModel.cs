using System;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

using WP.MVVM.Core;

namespace WP.MVVM.Example.ViewModel
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel() 
        {

        }

        #region  attribute
        private UserControl m_FirstContent;
        public UserControl FirstContent
        {
            get { return m_FirstContent; }
            set { m_FirstContent = value; RaisePropertyChanged("FirstContent"); }
        }

        private UserControl m_SecondContent;
        public UserControl SecondContent
        {
            get { return m_SecondContent; }
            set { m_SecondContent = value; RaisePropertyChanged("SecondContent"); }
        }
        #endregion

        #region  command
        public ICommand ShowCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var firstCtl = ViewControlLocator.Instance.RegisterViewControl<View.FirstCtl, ViewModel.FirstCtlViewModel>();
                    this.FirstContent = firstCtl;

                    var secondCtl = ViewControlLocator.Instance.RegisterViewControl<View.SecondCtl, ViewModel.SecondCtlViewModel>();
                    this.SecondContent = secondCtl;
                });
            }
        }
        #endregion

        #region  method

        #endregion
    }
}
