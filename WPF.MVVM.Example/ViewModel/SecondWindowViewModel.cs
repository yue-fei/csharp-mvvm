using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using WPF.MVVM.Core;

namespace WPF.MVVM.Example.ViewModel
{
    public class SecondWindowViewModel : ViewModelBase
    {
        public SecondWindowViewModel() 
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

        #endregion

        #region  method

        #endregion
    }
}
