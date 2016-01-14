using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Silverlight.MVVM.Core;

namespace Silverlight.MVVM.Example.ViewModel
{
    public class FirstCtlViewModel : ViewModelBase
    {
        public FirstCtlViewModel() 
        {

        }

        #region  attribute
        private string m_TextContent;
        public string TextContent
        {
            get { return m_TextContent; }
            set { m_TextContent = value; RaisePropertyChanged("TextContent"); }
        }
        #endregion

        #region  command
        public ICommand TextChangedCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var vm = ViewModelLocator.Instance.GetViewModel<ViewModel.SecondCtlViewModel>();
                    if (vm != null)
                    {
                        vm.TextContent = this.FindViewControlChild<TextBox>("tb").Text;
                    }
                });
            }
        }

        #endregion

        #region  method

        #endregion
    }
}
