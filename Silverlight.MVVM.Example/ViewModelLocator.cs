using System;
using System.Collections.Generic;

using Silverlight.MVVM.Core;
using Silverlight.MVVM.Example.ViewModel;

namespace Silverlight.MVVM.Example
{
    public partial class ViewModelLocator
    {
        public Dictionary<string, ViewModelBase> ViewModels { get; set; }

        private static ViewModelLocator m_Instance = null;
        public static ViewModelLocator Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = App.Current.Resources["Locator"] as ViewModelLocator;
                return m_Instance;
            }
        }

        public ViewModelLocator()
        {
            ViewModels = new Dictionary<string, ViewModelBase>();
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool) Init();
        }

        #region  register model
        public TViewModel RegisterViewModel<TViewModel>(string vmKey) where TViewModel : ViewModelBase, new()
        {
            TViewModel rst;
            string typekey = GetViewModelKey(typeof(TViewModel), vmKey);
            if (ViewModels.ContainsKey(typekey))
            {
                rst = ViewModels[typekey] as TViewModel;
            }
            else
            {
                rst = new TViewModel();
                ViewModels.Add(typekey, rst);
            }
            return rst;
        }

        public TViewModel RegisterViewModel<TViewModel>() where TViewModel : ViewModelBase, new()
        {
            return RegisterViewModel<TViewModel>(string.Empty);
        }
        #endregion

        #region  init method
        public bool Init<TViewModel>(Dictionary<string, object> args, bool isAutoLoad, string vmKey) where TViewModel : ViewModelBase, new()
        {
            bool rst = false;
            TViewModel tvm = GetViewModel<TViewModel>(vmKey);
            if (tvm == null)
            {
                tvm = RegisterViewModel<TViewModel>(vmKey);
            }
            if (tvm != null)
            {
                tvm.Init(args);
                if (isAutoLoad)
                {
                    rst = tvm.Load();
                }
                else
                {
                    rst = true;
                }
            }
            return rst;
        }

        public bool Init<TViewModel>(Dictionary<string, object> args, bool isAutoLoad = false) where TViewModel : ViewModelBase, new()
        {
            return Init<TViewModel>(args, isAutoLoad, string.Empty);
        }

        public bool Init<TViewModel>(bool isAutoLoad = false) where TViewModel : ViewModelBase, new()
        {
            return Init<TViewModel>(null, isAutoLoad, string.Empty);
        }

        public bool Load<TViewModel>(string vmKey = "") where TViewModel : ViewModelBase, new()
        {
            TViewModel tvm = GetViewModel<TViewModel>(vmKey);
            if (tvm != null)
            {
                return tvm.Load();
            }
            else
            {
                throw new Exception("尚未初始化一个类型" + typeof(TViewModel).Name + "的对象");
            }
        }
        #endregion

        #region  unregister model
        public void UnRegisterViewModel(Type vmtype, string vmKey)
        {
            string typekey = GetViewModelKey(vmtype, vmKey);
            if (ViewModels.ContainsKey(typekey))
            {
                ViewModelBase vm = ViewModels[typekey];
                vm.Cleanup();
                ViewModels.Remove(typekey);
                GC.SuppressFinalize(vm);
            }
        }
        #endregion

        #region  get model key
        private string GetViewModelKey(Type vmtype, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return string.Format("{0}___{1}", vmtype.Name, key);
            }
            return vmtype.Name;
        }
        #endregion

        #region   get model method
        public TViewModel GetViewModel<TViewModel>(string vmKey) where TViewModel : ViewModelBase
        {
            TViewModel rst = null;
            string typekey = GetViewModelKey(typeof(TViewModel), vmKey);
            if (ViewModels.ContainsKey(typekey)) rst = ViewModels[typekey] as TViewModel;
            return rst;
        }

        public TViewModel GetViewModel<TViewModel>() where TViewModel : ViewModelBase
        {
            return GetViewModel<TViewModel>(string.Empty);
        }

        public ViewModelBase GetViewModel(Type vmtype, string key)
        {
            ViewModelBase rst = null;
            string vmkey = GetViewModelKey(vmtype, key);
            if (ViewModels.ContainsKey(vmkey)) rst = ViewModels[vmkey];
            return rst;
        }

        public ViewModelBase GetViewModel(Type vmtype)
        {
            return GetViewModel(vmtype, string.Empty);
        }
        #endregion
    }

    public partial class ViewModelLocator
    {
        public Dictionary<string, string> Configuration { get; set; }

        private void Init()
        {

        }

        public MainCtlViewModel MainCtlViewModel
        {
            get
            {
                string key = GetViewModelKey(typeof(MainCtlViewModel), string.Empty);
                if (ViewModels.ContainsKey(key))
                {
                    return (MainCtlViewModel)ViewModels[key];
                }
                else
                {
                    return RegisterViewModel<MainCtlViewModel>();
                }
            }
        }

        public FirstCtlViewModel FirstCtlViewModel
        {
            get
            {
                string key = GetViewModelKey(typeof(FirstCtlViewModel), string.Empty);
                if (ViewModels.ContainsKey(key))
                {
                    return (FirstCtlViewModel)ViewModels[key];
                }
                else
                {
                    return RegisterViewModel<FirstCtlViewModel>();
                }
            }
        }

        public SecondCtlViewModel SecondCtlViewModel
        {
            get
            {
                string key = GetViewModelKey(typeof(SecondCtlViewModel), string.Empty);
                if (ViewModels.ContainsKey(key))
                {
                    return (SecondCtlViewModel)ViewModels[key];
                }
                else
                {
                    return RegisterViewModel<SecondCtlViewModel>();
                }
            }
        }
    }

}
