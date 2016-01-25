using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace WP.MVVM.Core
{
    #region  ICleanup
    /// <summary>
    /// Defines a common interface for classes that should be cleaned up,
    /// but without the implications that IDisposable presupposes. An instance
    /// implementing ICleanup can be cleaned up without being
    /// disposed and garbage collected.
    /// </summary>
    public interface ICleanup
    {
        /// <summary>
        /// Cleans up the instance, for example by saving its state,
        /// removing resources, etc...
        /// </summary>
        void Cleanup();

        void Init(Dictionary<string, object> args);

        bool Load();

        void SetViewControl(Control tctl);
    }
    #endregion

    #region  ObservableObject
    /// <summary>
    /// A base class for objects of which the properties must be observable.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return this.PropertyChanged;
            }
        }

        /// <summary>
        /// Verifies that a property name exists in this ViewModel. This method
        /// can be called before the property is used, for instance before
        /// calling RaisePropertyChanged. It avoids errors when a property name
        /// is changed but some places are missed.
        /// </summary>
        /// <remarks>This method is only active in DEBUG mode.</remarks>
        /// <param name="propertyName">The name of the property that will be
        /// checked.</param>
        [Conditional("DEBUG"), DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            Type type = base.GetType();
            if (!string.IsNullOrEmpty(propertyName) && type.GetTypeInfo().GetDeclaredProperty(propertyName) == null)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                string propertyName = ObservableObject.GetPropertyName<T>(propertyExpression);
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Extracts the name of a property from an expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">An expression returning the property's name.</param>
        /// <returns>The name of the property returned by the expression.</returns>
        /// <exception cref="T:System.ArgumentNullException">If the expression is null.</exception>
        /// <exception cref="T:System.ArgumentException">If the expression does not represent a property.</exception>
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }
            return propertyInfo.Name;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            field = newValue;
            this.RaisePropertyChanged<T>(propertyExpression);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        protected bool Set<T>(string propertyName, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            field = newValue;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            return this.Set<T>(propertyName, ref field, newValue);
        }
    }
    #endregion

    #region  ViewModelBase
    /// <summary>
    /// A base class for the ViewModel classes in the MVVM pattern.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, ICleanup
    {
        private Control m_ViewControl;
        public Control ViewControl
        {
            get { return m_ViewControl; }
        }

        private Dictionary<string, object> m_InitArgs;
        public Dictionary<string, object> InitArgs
        {
            get { return m_InitArgs; }
        }

        private static bool? _isInDesignMode;
        private IMessenger _messengerInstance;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running under Blend or Visual Studio).
        /// </summary>
        public bool IsInDesignMode
        {
            get
            {
                return ViewModelBase.IsInDesignModeStatic;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!ViewModelBase._isInDesignMode.HasValue)
                {
                    ViewModelBase._isInDesignMode = new bool?(ViewModelBase.IsInDesignModePortable());
                }
                return ViewModelBase._isInDesignMode.Value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of a <see cref="T:GalaSoft.MvvmLight.Messaging.IMessenger" /> used to
        /// broadcast messages to other objects. If null, this class will
        /// attempt to broadcast using the Messenger's default instance.
        /// </summary>
        protected IMessenger MessengerInstance
        {
            get
            {
                return this._messengerInstance ?? Messenger.Default;
            }
            set
            {
                this._messengerInstance = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public ViewModelBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        /// <param name="messenger">An instance of a <see cref="T:GalaSoft.MvvmLight.Messaging.Messenger" />
        /// used to broadcast messages to other objects. If null, this class
        /// will attempt to broadcast using the Messenger's default
        /// instance.</param>
        public ViewModelBase(IMessenger messenger)
        {
            this.MessengerInstance = messenger;
        }
        private static bool IsInDesignModePortable()
        {
            DesignerPlatformLibrary detectedDesignerLibrary = DesignerLibrary.DetectedDesignerLibrary;
            if (detectedDesignerLibrary == DesignerPlatformLibrary.WinRT)
            {
                return ViewModelBase.IsInDesignModeMetro();
            }
            if (detectedDesignerLibrary == DesignerPlatformLibrary.Silverlight)
            {
                bool flag = ViewModelBase.IsInDesignModeSilverlight();
                if (!flag)
                {
                    flag = ViewModelBase.IsInDesignModeNet();
                }
                return flag;
            }
            return detectedDesignerLibrary == DesignerPlatformLibrary.Net && ViewModelBase.IsInDesignModeNet();
        }
        private static bool IsInDesignModeSilverlight()
        {
            bool result;
            try
            {
                Type type = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
                PropertyInfo declaredProperty = type.GetTypeInfo().GetDeclaredProperty("IsInDesignTool");
                result = (bool)declaredProperty.GetValue(null, null);
            }
            catch 
            {
                result = false;
            }
            return result;
        }
        private static bool IsInDesignModeMetro()
        {
            bool result;
            try
            {
                Type type = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
                PropertyInfo declaredProperty = type.GetTypeInfo().GetDeclaredProperty("DesignModeEnabled");
                result = (bool)declaredProperty.GetValue(null, null);
            }
            catch
            {
                result = false;
            }
            return result;
        }
        private static bool IsInDesignModeNet()
        {
            bool result;
            try
            {
                Type type = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                object value = type.GetTypeInfo().GetDeclaredField("IsInDesignModeProperty").GetValue(null);
                Type type2 = Type.GetType("System.ComponentModel.DependencyPropertyDescriptor, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                Type type3 = Type.GetType("System.Windows.FrameworkElement, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                IEnumerable<MethodInfo> declaredMethods = type2.GetTypeInfo().GetDeclaredMethods("FromProperty");
                MethodInfo methodInfo = null;
                foreach (MethodInfo current in declaredMethods)
                {
                    if (current.IsPublic && current.IsStatic && current.GetParameters().Length == 2)
                    {
                        methodInfo = current;
                        break;
                    }
                }
                if (methodInfo == null)
                {
                    result = false;
                }
                else
                {
                    object obj = methodInfo.Invoke(null, new object[]
					{
						value,
						type3
					});
                    PropertyInfo declaredProperty = type2.GetTypeInfo().GetDeclaredProperty("Metadata");
                    object value2 = declaredProperty.GetValue(obj, null);
                    Type type4 = Type.GetType("System.Windows.PropertyMetadata, WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                    PropertyInfo declaredProperty2 = type4.GetTypeInfo().GetDeclaredProperty("DefaultValue");
                    bool flag = (bool)declaredProperty2.GetValue(value2, null);
                    result = flag;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Unregisters this instance from the Messenger class.
        /// <para>To cleanup additional resources, override this method, clean
        /// up and then call base.Cleanup().</para>
        /// </summary>
        public virtual void Cleanup()
        {
            this.MessengerInstance.Unregister(this);
        }

        /// <summary>
        /// Broadcasts a PropertyChangedMessage using either the instance of
        /// the Messenger that was passed to this class (if available) 
        /// or the Messenger's default instance.
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="oldValue">The value of the property before it
        /// changed.</param>
        /// <param name="newValue">The value of the property after it
        /// changed.</param>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        protected virtual void Broadcast<T>(T oldValue, T newValue, string propertyName)
        {
            PropertyChangedMessage<T> message = new PropertyChangedMessage<T>(this, oldValue, newValue, propertyName);
            this.MessengerInstance.Send<PropertyChangedMessage<T>>(message);
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        protected virtual void RaisePropertyChanged<T>([CallerMemberName] string propertyName = null, T oldValue = default(T), T newValue = default(T), bool broadcast = false)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("This method cannot be called with an empty string", "propertyName");
            }
            this.RaisePropertyChanged(propertyName);
            if (broadcast)
            {
                this.Broadcast<T>(oldValue, newValue, propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            PropertyChangedEventHandler propertyChangedHandler = base.PropertyChangedHandler;
            if (propertyChangedHandler != null || broadcast)
            {
                string propertyName = ObservableObject.GetPropertyName<T>(propertyExpression);
                if (propertyChangedHandler != null)
                {
                    propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
                }
                if (broadcast)
                {
                    this.Broadcast<T>(oldValue, newValue, propertyName);
                }
            }
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue, bool broadcast)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            T oldValue = field;
            field = newValue;
            this.RaisePropertyChanged<T>(propertyExpression, oldValue, field, broadcast);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        protected bool Set<T>(string propertyName, ref T field, T newValue = default(T), bool broadcast = false)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            T oldValue = field;
            field = newValue;
            this.RaisePropertyChanged<T>(propertyName, oldValue, field, broadcast);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <param name="propertyName">(optional) The name of the property that
        /// changed.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        protected bool Set<T>(ref T field, T newValue = default(T), bool broadcast = false, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            T oldValue = field;
            field = newValue;
            this.RaisePropertyChanged<T>(propertyName, oldValue, field, broadcast);
            return true;
        }

        public virtual void Init(Dictionary<string, object> args)
        {
            m_InitArgs = args;
        }

        public virtual bool Load()
        {
            bool rst = false;
            rst = true;
            return rst;
        }

        public virtual void SetViewControl(Control tctl)
        {
            m_ViewControl = tctl;
        }

        public TControl FindViewControlChild<TControl>(string controlname) where TControl : DependencyObject
        {
            TControl rst = null;
            if (m_ViewControl != null && m_ViewControl.GetType().GetTypeInfo().IsSubclassOf(typeof(Control)))
            {
                object obj = ((Control)m_ViewControl).FindName(controlname);
                if (obj != null)
                {
                    rst = (TControl)obj;
                }
            }
            return rst;
        }
    }
    #endregion
}