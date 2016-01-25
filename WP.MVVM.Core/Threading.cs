using System;
using System.Text;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace WP.MVVM.Core
{
    #region  DispatcherHelper
    public static class DispatcherHelper
    {
        public static CoreDispatcher UIDispatcher
        {
            get;
            private set;
        }
        public async static void CheckBeginInvokeOnUI(Action action)
        {
            if (action == null)
            {
                return;
            }
            DispatcherHelper.CheckDispatcher();
            if (DispatcherHelper.UIDispatcher.HasThreadAccess)
            {
                action.Invoke();
                return;
            }
            await DispatcherHelper.UIDispatcher.RunAsync(0, delegate
            {
                action.Invoke();
            });
        }
        private static void CheckDispatcher()
        {
            if (DispatcherHelper.UIDispatcher == null)
            {
                StringBuilder stringBuilder = new StringBuilder("The DispatcherHelper is not initialized.");
                stringBuilder.AppendLine();
                stringBuilder.Append("Call DispatcherHelper.Initialize() at the end of App.OnLaunched.");
                throw new InvalidOperationException(stringBuilder.ToString());
            }
        }
        public static IAsyncAction RunAsync(Action action)
        {
            DispatcherHelper.CheckDispatcher();
            return DispatcherHelper.UIDispatcher.RunAsync(0, delegate
            {
                action.Invoke();
            });
        }
        public static void Initialize()
        {
            if (DispatcherHelper.UIDispatcher != null)
            {
                return;
            }
            DispatcherHelper.UIDispatcher = Window.Current.Dispatcher;
        }
        public static void Reset()
        {
            DispatcherHelper.UIDispatcher = null;
        }
    }
    #endregion
}
