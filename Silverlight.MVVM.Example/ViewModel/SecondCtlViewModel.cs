using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Silverlight.MVVM.Core;

namespace Silverlight.MVVM.Example.ViewModel
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
