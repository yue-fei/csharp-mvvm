using System;
using System.Windows;
using System.Windows.Input;

using WP.MVVM.Core;

namespace WP.MVVM.Example.ViewModel
{
    public class SecondCtlViewModel : ViewModelBase
    {
        public SecondCtlViewModel() 
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
