using System;
using System.Reflection;
using System.Threading.Tasks;

namespace WP.MVVM.Core
{
    #region  DesignerLibrary
    /// <summary>
    /// Helper class for platform detection.
    /// </summary>
    internal static class DesignerLibrary
    {
        private static DesignerPlatformLibrary? _detectedDesignerPlatformLibrary;
        internal static DesignerPlatformLibrary DetectedDesignerLibrary
        {
            get
            {
                if (!DesignerLibrary._detectedDesignerPlatformLibrary.HasValue)
                {
                    DesignerLibrary._detectedDesignerPlatformLibrary = new DesignerPlatformLibrary?(DesignerLibrary.GetCurrentPlatform());
                }
                return DesignerLibrary._detectedDesignerPlatformLibrary.Value;
            }
        }
        private static DesignerPlatformLibrary GetCurrentPlatform()
        {
            Type type = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
            if (type != null)
            {
                return DesignerPlatformLibrary.Silverlight;
            }
            Type type2 = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            if (type2 != null)
            {
                return DesignerPlatformLibrary.Net;
            }
            Type type3 = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
            if (type3 != null)
            {
                return DesignerPlatformLibrary.WinRT;
            }
            return DesignerPlatformLibrary.Unknown;
        }
    }
    #endregion

    #region  DesignerPlatformLibrary
    internal enum DesignerPlatformLibrary
    {
        Unknown,
        Net,
        WinRT,
        Silverlight
    }
    #endregion

    #region  Empty
    /// <summary>
    /// Helper class used when an async method is required,
    /// but the context is synchronous.
    /// </summary>
    public static class Empty
    {
        private static readonly Task ConcreteTask = new Task(delegate
        {
        });
        /// <summary>
        /// Gets the empty task.
        /// </summary>
        public static Task Task
        {
            get
            {
                return Empty.ConcreteTask;
            }
        }
    }
    #endregion

    #region  FeatureDetection/// <summary>
    /// Helper class for platform and feature detection.
    /// </summary>
    internal static class FeatureDetection
    {
        private class ReflectionDetectionClass
        {
            private void Method()
            {
            }
        }
        private static bool? _isPrivateReflectionSupported;
        public static bool IsPrivateReflectionSupported
        {
            get
            {
                if (!FeatureDetection._isPrivateReflectionSupported.HasValue)
                {
                    FeatureDetection._isPrivateReflectionSupported = new bool?(FeatureDetection.ResolveIsPrivateReflectionSupported());
                }
                return FeatureDetection._isPrivateReflectionSupported.Value;
            }
        }
        private static bool ResolveIsPrivateReflectionSupported()
        {
            FeatureDetection.ReflectionDetectionClass obj = new FeatureDetection.ReflectionDetectionClass();
            try
            {
                MethodInfo declaredMethod = typeof(FeatureDetection.ReflectionDetectionClass).GetTypeInfo().GetDeclaredMethod("Method");
                declaredMethod.Invoke(obj, null);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
    #endregion

    #region  IExecuteWithObject
    /// <summary>
    /// This interface is meant for the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction`1" /> class and can be 
    /// useful if you store multiple WeakAction{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    public interface IExecuteWithObject
    {
        /// <summary>
        /// The target of the WeakAction.
        /// </summary>
        object Target
        {
            get;
        }

        /// <summary>
        /// Executes an action.
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        void ExecuteWithObject(object parameter);

        /// <summary>
        /// Deletes all references, which notifies the cleanup method
        /// that this entry must be deleted.
        /// </summary>
        void MarkForDeletion();
    }
    #endregion

    #region  IExecuteWithObjectAndResult
    /// <summary>
    /// This interface is meant for the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc`1" /> class and can be 
    /// useful if you store multiple WeakFunc{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    public interface IExecuteWithObjectAndResult
    {
        /// <summary>
        /// Executes a Func and returns the result.
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        /// <returns>The result of the operation.</returns>
        object ExecuteWithObject(object parameter);
    }
    #endregion

    #region  WeakAction
    /// <summary>
    /// Stores an <see cref="T:System.Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <summary>
    /// Stores an <see cref="T:System.Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        private Action _staticAction;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakAction's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (this._staticAction != null)
                {
                    return this._staticAction.GetMethodInfo().Name;
                }
                return this.Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakAction's action's target.
        /// This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakAction.Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference ActionReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakAction. This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakAction.ActionReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference
        {
            get;
            set;
        }
        /// <summary>
        /// Gets a value indicating whether the WeakAction is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return this._staticAction != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (this._staticAction == null && this.Reference == null)
                {
                    return false;
                }
                if (this._staticAction != null)
                {
                    return this.Reference == null || this.Reference.IsAlive;
                }
                return this.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a 
        /// <see cref="T:System.WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (this.Reference == null)
                {
                    return null;
                }
                return this.Reference.Target;
            }
        }

        /// <summary>
        /// The target of the weak reference.
        /// </summary>
        protected object ActionTarget
        {
            get
            {
                if (this.ActionReference == null)
                {
                    return null;
                }
                return this.ActionReference.Target;
            }
        }

        /// <summary>
        /// Initializes an empty instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
        /// </summary>
        protected WeakAction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Action action)
            : this((action == null) ? null : action.Target, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action action)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                this._staticAction = action;
                if (target != null)
                {
                    this.Reference = new WeakReference(target);
                }
                return;
            }
            this.Method = action.GetMethodInfo();
            this.ActionReference = new WeakReference(action.Target);
            this.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (this._staticAction != null)
            {
                this._staticAction();
                return;
            }
            object actionTarget = this.ActionTarget;
            if (this.IsAlive && this.Method != null && this.ActionReference != null && actionTarget != null)
            {
                this.Method.Invoke(actionTarget, null);
            }
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            this.Reference = null;
            this.ActionReference = null;
            this.Method = null;
            this._staticAction = null;
        }
    }
    #endregion

    #region  WeakAction<T>
    /// <summary>
    /// Stores an Action without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Action's parameter.</typeparam>
    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        private Action<T> _staticAction;
        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (this._staticAction != null)
                {
                    return this._staticAction.GetMethodInfo().Name;
                }
                return base.Method.Name;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (this._staticAction == null && base.Reference == null)
                {
                    return false;
                }
                if (this._staticAction != null)
                {
                    return base.Reference == null || base.Reference.IsAlive;
                }
                return base.Reference.IsAlive;
            }
        }
        /// <summary>
        /// Initializes a new instance of the WeakAction class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Action<T> action)
            : this((action == null) ? null : action.Target, action)
        {
        }
        /// <summary>
        /// Initializes a new instance of the WeakAction class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action<T> action)
        {
            if (action.GetMethodInfo().IsStatic)
            {
                this._staticAction = action;
                if (target != null)
                {
                    base.Reference = new WeakReference(target);
                }
                return;
            }
            base.Method = action.GetMethodInfo();
            base.ActionReference = new WeakReference(action.Target);
            base.Reference = new WeakReference(target);
        }
        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive. The action's parameter is set to default(T).
        /// </summary>
        public new void Execute()
        {
            this.Execute(default(T));
        }
        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        public void Execute(T parameter)
        {
            if (this._staticAction != null)
            {
                this._staticAction(parameter);
                return;
            }
            object actionTarget = base.ActionTarget;
            if (this.IsAlive && base.Method != null && base.ActionReference != null && actionTarget != null)
            {
                base.Method.Invoke(actionTarget, new object[]
				{
					parameter
				});
            }
        }
        /// <summary>
        /// Executes the action with a parameter of type object. This parameter
        /// will be casted to T. This method implements <see cref="M:GalaSoft.MvvmLight.Helpers.IExecuteWithObject.ExecuteWithObject(System.Object)" />
        /// and can be useful if you store multiple WeakAction{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the action after
        /// being casted to T.</param>
        public void ExecuteWithObject(object parameter)
        {
            T parameterCasted = (T)((object)parameter);
            this.Execute(parameterCasted);
        }
        /// <summary>
        /// Sets all the actions that this WeakAction contains to null,
        /// which is a signal for containing objects that this WeakAction
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            this._staticAction = null;
            base.MarkForDeletion();
        }
    }
    #endregion

    #region  WeakFunc<TResult>
    /// <summary>
    /// Stores a Func&lt;T&gt; without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the Func that will be stored
    /// by this weak reference.</typeparam>
    public class WeakFunc<TResult>
    {
        private Func<TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakFunc's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// Get a value indicating whether the WeakFunc is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return this._staticFunc != null;
            }
        }

        /// <summary>
        /// Gets the name of the method that this WeakFunc represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (this._staticFunc != null)
                {
                    return this._staticFunc.GetMethodInfo().Name;
                }
                return this.Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakFunc's action's target.
        /// This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference FuncReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakFunc. This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.FuncReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (this._staticFunc == null && this.Reference == null)
                {
                    return false;
                }
                if (this._staticFunc != null)
                {
                    return this.Reference == null || this.Reference.IsAlive;
                }
                return this.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Func's owner. This object is stored as a 
        /// <see cref="T:System.WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (this.Reference == null)
                {
                    return null;
                }
                return this.Reference.Target;
            }
        }

        /// <summary>
        /// Gets the owner of the Func that was passed as parameter.
        /// This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.Target" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected object FuncTarget
        {
            get
            {
                if (this.FuncReference == null)
                {
                    return null;
                }
                return this.FuncReference.Target;
            }
        }

        /// <summary>
        /// Initializes an empty instance of the WeakFunc class.
        /// </summary>
        protected WeakFunc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(Func<TResult> func)
            : this((func == null) ? null : func.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(object target, Func<TResult> func)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                this._staticFunc = func;
                if (target != null)
                {
                    this.Reference = new WeakReference(target);
                }
                return;
            }
            this.Method = func.GetMethodInfo();
            this.FuncReference = new WeakReference(func.Target);
            this.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the action. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute()
        {
            if (this._staticFunc != null)
            {
                return this._staticFunc();
            }
            object funcTarget = this.FuncTarget;
            if (this.IsAlive && this.Method != null && this.FuncReference != null && funcTarget != null)
            {
                return (TResult)((object)this.Method.Invoke(funcTarget, null));
            }
            return default(TResult);
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            this.Reference = null;
            this.FuncReference = null;
            this.Method = null;
            this._staticFunc = null;
        }
    }
    #endregion

    #region  WeakFunc<T, TResult>
    /// <summary>
    /// Stores an Func without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Func's parameter.</typeparam>
    /// <typeparam name="TResult">The type of the Func's return value.</typeparam>
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {
        private Func<T, TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the name of the method that this WeakFunc represents.
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (this._staticFunc != null)
                {
                    return this._staticFunc.GetMethodInfo().Name;
                }
                return base.Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (this._staticFunc == null && base.Reference == null)
                {
                    return false;
                }
                if (this._staticFunc != null)
                {
                    return base.Reference == null || base.Reference.IsAlive;
                }
                return base.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(Func<T, TResult> func)
            : this((func == null) ? null : func.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(object target, Func<T, TResult> func)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                this._staticFunc = func;
                if (target != null)
                {
                    base.Reference = new WeakReference(target);
                }
                return;
            }
            base.Method = func.GetMethodInfo();
            base.FuncReference = new WeakReference(func.Target);
            base.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive. The Func's parameter is set to default(T).
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public new TResult Execute()
        {
            return this.Execute(default(T));
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute(T parameter)
        {
            if (this._staticFunc != null)
            {
                return this._staticFunc(parameter);
            }
            object funcTarget = base.FuncTarget;
            if (this.IsAlive && base.Method != null && base.FuncReference != null && funcTarget != null)
            {
                return (TResult)((object)base.Method.Invoke(funcTarget, new object[]
				{
					parameter
				}));
            }
            return default(TResult);
        }

        /// <summary>
        /// Executes the Func with a parameter of type object. This parameter
        /// will be casted to T. This method implements <see cref="M:GalaSoft.MvvmLight.Helpers.IExecuteWithObject.ExecuteWithObject(System.Object)" />
        /// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the Func after
        /// being casted to T.</param>
        /// <returns>The result of the execution as object, to be casted to T.</returns>
        public object ExecuteWithObject(object parameter)
        {
            T parameterCasted = (T)((object)parameter);
            return this.Execute(parameterCasted);
        }

        /// <summary>
        /// Sets all the funcs that this WeakFunc contains to null,
        /// which is a signal for containing objects that this WeakFunc
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            this._staticFunc = null;
            base.MarkForDeletion();
        }
    }
    #endregion
}
