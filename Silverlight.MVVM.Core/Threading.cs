using System;
using System.Windows;
using System.Windows.Threading;

namespace Silverlight.MVVM.Core
{
    #region  DispatcherHelper
    /// <summary>
    /// Helper class for dispatcher operations on the UI thread.
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Gets a reference to the UI thread's dispatcher, after the
        /// <see cref="M:GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize" /> method has been called on the UI thread.
        /// </summary>
        public static Dispatcher UIDispatcher
        {
            get;
            private set;
        }
        /// <summary>
        /// Executes an action on the UI thread. If this method is called
        /// from the UI thread, the action is executed immendiately. If the
        /// method is called from another thread, the action will be enqueued
        /// on the UI thread's dispatcher and executed asynchronously.
        /// <para>For additional operations on the UI thread, you can get a
        /// reference to the UI thread's dispatcher thanks to the property
        /// <see cref="P:GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher" /></para>.
        /// </summary>
        /// <param name="action">The action that will be executed on the UI
        /// thread.</param>
        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (DispatcherHelper.UIDispatcher.CheckAccess())
            {
                action.Invoke();
                return;
            }
            DispatcherHelper.UIDispatcher.BeginInvoke(action);
        }
        /// <summary>
        /// Invokes an action asynchronously on the UI thread.
        /// </summary>
        /// <param name="action">The action that must be executed.</param>
        public static void InvokeAsync(Action action)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(action);
        }
        /// <summary>
        /// This method should be called once on the UI thread to ensure that
        /// the <see cref="P:GalaSoft.MvvmLight.Threading.DispatcherHelper.UIDispatcher" /> property is initialized.
        /// <para>In a Silverlight application, call this method in the
        /// Application_Startup event handler, after the MainPage is constructed.</para>
        /// <para>In WPF, call this method on the static App() constructor.</para>
        /// </summary>
        public static void Initialize()
        {
            if (DispatcherHelper.UIDispatcher != null)
            {
                return;
            }
            DispatcherHelper.UIDispatcher = Deployment.Current.Dispatcher;
        }
    }
    #endregion
}
