using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WPF.MVVM.Core;

namespace WPF.MVVM.Example.ViewModel
{
    public class FirstWindowViewModel : ViewModelBase
    {
        public FirstWindowViewModel() 
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
                    var vm = ViewModelLocator.Instance.GetViewModel<ViewModel.SecondWindowViewModel>();
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
