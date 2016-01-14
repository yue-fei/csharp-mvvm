using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Reflection;
using System.Linq;

using Silverlight.MVVM.Core;

namespace Silverlight.MVVM.Example
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

        #region  register control
        public T RegisterViewControl<T>(string vcKey) where T : UserControl, new()
        {
            return innerRegisterUI<T>(vcKey);
        }

        public T RegisterViewControl<T>() where T : UserControl, new()
        {
            return innerRegisterUI<T>(string.Empty);
        }

         public TControl RegisterViewControl<TControl, TViewModel>(Dictionary<string, object> vmArgs, bool isAutoLoad, string vcKey)
            where TControl : UserControl, new()
            where TViewModel : ViewModelBase, new()
        {
            TViewModel vm = ViewModelLocator.Instance.RegisterViewModel<TViewModel>(vcKey);
            TControl rst = RegisterViewControl<TControl>(vcKey);
            if (rst != null)
            {
                vm.SetViewControl(rst);
                vm.Init(vmArgs);
                if (isAutoLoad) vm.Load();
                rst.DataContext = vm;
            }
            return rst;
         }

        public TControl RegisterViewControl<TControl, TViewModel>(Dictionary<string, object> vmArgs, bool isAutoLoad = false)
            where TControl : UserControl, new()
            where TViewModel : ViewModelBase, new()
        {
            return RegisterViewControl<TControl, TViewModel>(vmArgs, isAutoLoad, string.Empty);
        }

        public TControl RegisterViewControl<TControl, TViewModel>(bool isAutoLoad = false)
            where TControl : UserControl, new()
            where TViewModel : ViewModelBase, new()
        {
            return RegisterViewControl<TControl, TViewModel>(null, isAutoLoad, string.Empty);
        }
        #endregion

        #region  unregister control
        public void UnRegisterViewControl<T>(string vcKey, bool isUnRegisterViewModel) where T : UserControl, new()
        {
            InnerUnRegisterUI(typeof(T), vcKey, isUnRegisterViewModel);
        }

        public void UnRegisterViewControl<T>(bool isUnRegisterViewModel) where T : UserControl, new()
        {
            InnerUnRegisterUI(typeof(T), string.Empty, isUnRegisterViewModel);
        }

        public void UnRegisterViewControl(ref UserControl _uc, string vcKey, bool isUnRegisterViewModel)
        {
            InnerUnRegisterUI(_uc.GetType(), vcKey, isUnRegisterViewModel);
        }

        public void UnRegisterViewControl(ref UserControl _uc, bool isUnRegisterViewModel)
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
                    if (t is UserControl)
                    {
                        #region  usercontrol
                        UserControl uc = t as UserControl;
                        UnRegisterRelationUserControl(ref uc, vcKey, isUnRegisterVM);
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
                    if (t is UserControl)
                    {
                        #region  usercontrol
                        UserControl uc = t as UserControl;
                        UnRegisterRelationUserControl(ref uc, vcKey, isUnRegisterVM);
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

        private void UnRegisterRelationUserControl(ref UserControl uc, string vcKey, bool isUnRegisterVM)
        {
            if (uc == null) return;
            var ucchilds = uc.Descendents();
            foreach (UserControl t in ucchilds.OfType<UserControl>())
            {
                UserControl t2 = t;
                string t2key = GetViewControlKey(t2.GetType(), vcKey);
                UnRegisterRelationUserControl(ref t2, vcKey, isUnRegisterVM);
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
            if (t.IsSubclassOf(typeof(UserControl)))
            {
                UserControl uc = rst as UserControl;
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
        public T GetViewControl<T>(string vcKey = "") where T : UserControl, new()
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
