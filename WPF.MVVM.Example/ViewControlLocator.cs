using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Linq;

using WPF.MVVM.Core;

namespace WPF.MVVM.Example
{
    public class ViewControlLocator
    {
        public Dictionary<string, object> ViewControls { get; set; }

        private static ViewControlLocator m_Instance;
        public static ViewControlLocator Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new ViewControlLocator();
                return m_Instance;
            }
        }

        private ViewControlLocator()
        {
            ViewControls = new Dictionary<string, object>();
        }

        #region  register window
        public T RegisterWindow<T>(string vcKey) where T : Window, new()
        {
            T t = innerRegisterUI<T>(vcKey);
            if (t != null)
            {
                t.Closed += (s, e) =>
                {
                    UnRegisterWindow<T>(true);
                };
            }
            return t;
        }

        public T RegisterWindow<T>() where T : Window, new()
        {
            return innerRegisterUI<T>(string.Empty);
        }

         public TControl RegisterWindow<TControl, TViewModel>(Dictionary<string, object> vmArgs, bool isAutoLoad, string vcKey)
            where TControl : Window, new()
            where TViewModel : ViewModelBase, new()
        {
            TViewModel vm = ViewModelLocator.Instance.RegisterViewModel<TViewModel>(vcKey);
            TControl rst = RegisterWindow<TControl>(vcKey);
            if (rst != null)
            {
                vm.SetViewControl(rst);
                vm.Init(vmArgs);
                if (isAutoLoad) vm.Load();
                rst.DataContext = vm;
            }
            return rst;
         }

        public TControl RegisterWindow<TControl, TViewModel>(Dictionary<string, object> vmArgs, bool isAutoLoad = false)
            where TControl : Window, new()
            where TViewModel : ViewModelBase, new()
        {
            return RegisterWindow<TControl, TViewModel>(vmArgs, isAutoLoad, string.Empty);
        }

        public TControl RegisterWindow<TControl, TViewModel>(bool isAutoLoad = false)
            where TControl : Window, new()
            where TViewModel : ViewModelBase, new()
        {
            return RegisterWindow<TControl, TViewModel>(null, isAutoLoad, string.Empty);
        }
        #endregion

        #region  unregister window
        public void UnRegisterWindow<T>(string vcKey, bool isUnRegisterViewModel) where T : Window, new()
        {
            InnerUnRegisterUI(typeof(T), vcKey, isUnRegisterViewModel);
        }

        public void UnRegisterWindow<T>(bool isUnRegisterViewModel) where T : Window, new()
        {
            InnerUnRegisterUI(typeof(T), string.Empty, isUnRegisterViewModel);
        }

        public void UnRegisterWindow(ref Window _uc, string vcKey, bool isUnRegisterViewModel)
        {
            InnerUnRegisterUI(_uc.GetType(), vcKey, isUnRegisterViewModel);
        }

        public void UnRegisterWindow(ref Window _uc, bool isUnRegisterViewModel)
        {
            InnerUnRegisterUI(_uc.GetType(), string.Empty, isUnRegisterViewModel);
        }
        #endregion

        #region  register method
        private T innerRegisterUI<T>(string vcKey) where T : new()
        {
            T rst;
            string typekey = GetViewControlKey(typeof(T), vcKey);

            if (ViewControls.ContainsKey(typekey))
            {
                if (ViewControls[typekey] != null)
                {
                    rst = (T)ViewControls[typekey];
                }
                else
                {
                    rst = CreateUserControl<T>();
                    ViewControls[typekey] = rst;
                }
            }
            else
            {
                rst = CreateUserControl<T>();
                ViewControls.Add(typekey, rst);
            }
            return rst;
        }
        #endregion

        #region  unregister method
        private void InnerUnRegisterUI(string vcKey, bool isUnRegisterVM)
        {
            if (ViewControls.ContainsKey(vcKey))
            {
                object t = ViewControls[vcKey];
                if (t != null)
                {
                    if (t is Window)
                    {
                        #region  Window
                        Window uc = t as Window;
                        UnRegisterRelationWindow(ref uc, vcKey, isUnRegisterVM);
                        if (uc.Content != null)
                        {
                            GC.SuppressFinalize(uc.Content);
                            uc.Content = null;
                        }
                        if (uc.DataContext != null)
                        {
                            if (isUnRegisterVM)
                            {
                                if (uc.DataContext is ViewModelBase)
                                {
                                    ViewModelLocator.Instance.UnRegisterViewModel(uc.DataContext.GetType(), vcKey);
                                }
                            }
                            uc.DataContext = null;
                        }
                        #endregion
                    }
                }
                ViewControls.Remove(vcKey);
                if (t != null) GC.SuppressFinalize(t);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void InnerUnRegisterUI(Type type, string vcKey, bool isUnRegisterVM)
        {
            string vcTypeKey = GetViewControlKey(type, vcKey);
            if (ViewControls.ContainsKey(vcTypeKey))
            {
                object t = ViewControls[vcTypeKey];
                if (t != null)
                {
                    if (t is Window)
                    {
                        #region  usercontrol
                        Window uc = t as Window;
                        UnRegisterRelationWindow(ref uc, vcKey, isUnRegisterVM);
                        if (uc.Content != null)
                        {
                            GC.SuppressFinalize(uc.Content);
                            uc.Content = null;
                        }
                        if (uc.DataContext != null)
                        {
                            if (isUnRegisterVM)
                            {
                                if (uc.DataContext is ViewModelBase)
                                {
                                    ViewModelLocator.Instance.UnRegisterViewModel(uc.DataContext.GetType(), vcKey);
                                }
                            }
                            uc.DataContext = null;
                        }
                        #endregion
                    }
                }
                ViewControls.Remove(vcTypeKey);
                if (t != null) GC.SuppressFinalize(t);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void UnRegisterRelationWindow(ref Window uc, string vcKey, bool isUnRegisterVM)
        {
            if (uc == null) return;
            var ucchilds = uc.Descendents();
            foreach (Window t in ucchilds.OfType<Window>())
            {
                Window t2 = t;
                string t2key = GetViewControlKey(t2.GetType(), vcKey);
                UnRegisterRelationWindow(ref t2, vcKey, isUnRegisterVM);
                if (t2 != null)
                {
                    if (t2.DataContext != null)
                    {
                        if (isUnRegisterVM)
                        {
                            if (t2.DataContext is ViewModelBase)
                            {
                                ViewModelLocator.Instance.UnRegisterViewModel(t2.DataContext.GetType(), vcKey);
                            }
                        }
                        t2.DataContext = null;
                    }
                    if (t2.Content != null)
                    {
                        GC.SuppressFinalize(t2.Content);
                        t2.Content = null;
                    }
                }
                if (ViewControls.ContainsKey(t2key))
                {
                    ViewControls.Remove(t2key);
                }
                if (t2 != null) GC.SuppressFinalize(t2);
            }
        }
        #endregion

        #region  create control
        private T CreateUserControl<T>() where T : new()
        {
            T rst = new T();
            Type t = typeof(T);
            if (t.IsSubclassOf(typeof(Window)))
            {
                Window uc = rst as Window;
                if (uc.Content == null)
                {
                    MethodInfo mi = typeof(T).GetMethod("InitializeComponent");  
                    mi.Invoke(uc, null);
                }
            }
            return rst;
        }
        #endregion

        #region  get control method
        public T GetViewControl<T>(string vcKey = "") where T : Window, new()
        {
            T rst = null;
            if (ViewControls != null)
            {
                string typekey = GetViewControlKey(typeof(T), vcKey);
                if (ViewControls.ContainsKey(typekey) && ViewControls[typekey] != null)
                {
                    rst = ViewControls[typekey] as T;
                }
            }
            return rst;
        }
        #endregion

        #region  get control key
        private string GetViewControlKey(Type vctype, string key)
        {
            if (!string.IsNullOrEmpty(key) && key.Length >= 1)
            {
                return string.Format("{0}___{1}", vctype.Name, key);
            }
            return vctype.Name;
        }
        #endregion   

        #region  destroy control
        public void DestroyCtl()
        {
            if (this.ViewControls != null && this.ViewControls.Count > 0)
            {
                List<string> destoryList = new List<string>();

                foreach (var item in this.ViewControls)
                {
                    destoryList.Add(item.Key);
                }

                foreach (var item in destoryList)
                {
                    this.InnerUnRegisterUI(item, true);
                }
            }
        }
        #endregion
    }
}
