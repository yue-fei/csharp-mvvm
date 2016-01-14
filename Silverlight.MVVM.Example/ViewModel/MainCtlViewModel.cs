using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Silverlight.MVVM.Core;

namespace Silverlight.MVVM.Example.ViewModel
{
    public class MainCtlViewModel : ViewModelBase
    {
        public MainCtlViewModel() 
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
                    var firstCtl  = ViewControlLocator.Instance.RegisterViewControl<View.FirstCtl, ViewModel.FirstCtlViewModel>();
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
