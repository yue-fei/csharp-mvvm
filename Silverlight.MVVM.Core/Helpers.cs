using System;
using System.Reflection;

namespace Silverlight.MVVM.Core
{
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

    #region  IExecuteWithObject
    /// <summary>
    /// This interface is meant for the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakFunc`1" /> class and can be 
    /// useful if you store multiple WeakFunc{T} instances but don't know in advance
    /// what type T represents.
    /// </summary>
    public interface IExecuteWithObjectAndResult
    {
        /// <summary>
        /// Executes a func and returns the result.
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        object ExecuteWithObject(object parameter);
    }
    #endregion

    #region  WeakAction
    /// <summary>
    /// Stores an <see cref="T:System.Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        private Action _action;
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
                    return this._staticAction.Method.Name;
                }
                if (this._action != null)
                {
                    return this._action.Method.Name;
                }
                if (this.Method != null)
                {
                    return this.Method.Name;
                }
                return string.Empty;
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
                return (this._action != null && this._action.Target == null) || this._staticAction != null;
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
        ///
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
            : this(action.Target, action)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Helpers.WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action action)
        {
            if (action.Method.IsStatic)
            {
                this._staticAction = action;
                if (target != null)
                {
                    this.Reference = new WeakReference(target);
                }
                return;
            }
            if (!action.Method.IsPublic || (action.Target != null && !action.Target.GetType().IsPublic && !action.Target.GetType().IsNestedPublic))
            {
                this._action = action;
            }
            else
            {
                string name = action.Method.Name;
                if (name.Contains("<") && name.Contains(">"))
                {
                    this._action = action;
                }
                else
                {
                    this.Method = action.Method;
                    this.ActionReference = new WeakReference(action.Target);
                }
            }
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
                this._staticAction.Invoke();
                return;
            }
            object actionTarget = this.ActionTarget;
            if (this.IsAlive)
            {
                if (this.Method != null && this.ActionReference != null && actionTarget != null)
                {
                    this.Method.Invoke(this.ActionTarget, null);
                    return;
                }
                if (this._action != null)
                {
                    this._action.Invoke();
                }
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
            this._action = null;
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
        private Action<T> _action;
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
                    return this._staticAction.Method.Name;
                }
                if (this._action != null)
                {
                    return this._action.Method.Name;
                }
                if (base.Method != null)
                {
                    return base.Method.Name;
                }
                return string.Empty;
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
            : this(action.Target, action)
        {
        }
        /// <summary>
        /// Initializes a new instance of the WeakAction class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action<T> action)
        {
            if (action.Method.IsStatic)
            {
                this._staticAction = action;
                if (target != null)
                {
                    base.Reference = new WeakReference(target);
                }
                return;
            }
            if (!action.Method.IsPublic || (target != null && !target.GetType().IsPublic && !target.GetType().IsNestedPublic))
            {
                this._action = action;
            }
            else
            {
                string name = action.Method.Name;
                if (name.Contains("<") && name.Contains(">"))
                {
                    this._action = action;
                }
                else
                {
                    base.Method = action.Method;
                    base.ActionReference = new WeakReference(action.Target);
                }
            }
            base.Reference = new WeakReference(target);
        }
        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive. The action's parameter is set to default(T).
        /// </summary>
        public void Execute()
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
                this._staticAction.Invoke(parameter);
                return;
            }
            if (this.IsAlive)
            {
                if (base.Method != null && base.ActionReference != null)
                {
                    base.Method.Invoke(base.ActionTarget, new object[]
					{
						parameter
					});
                }
                if (this._action != null)
                {
                    this._action.Invoke(parameter);
                }
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
            T parameter2 = (T)((object)parameter);
            this.Execute(parameter2);
        }
        /// <summary>
        /// Sets all the actions that this WeakAction contains to null,
        /// which is a signal for containing objects that this WeakAction
        /// should be deleted.
        /// </summary>
        public void MarkForDeletion()
        {
            this._action = null;
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
    public class WeakFunc<TResult>
    {
        private Func<TResult> _func;
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
                return (this._func != null && this._func.Target == null) || this._staticFunc != null;
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
                    return this._staticFunc.Method.Name;
                }
                if (this._func != null)
                {
                    return this._func.Method.Name;
                }
                if (this.Method != null)
                {
                    return this.Method.Name;
                }
                return string.Empty;
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
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakFunc(Func<TResult> func)
            : this(func.Target, func)
        {
        }
        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The func's owner.</param>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakFunc(object target, Func<TResult> func)
        {
            if (func.Method.IsStatic)
            {
                this._staticFunc = func;
                if (target != null)
                {
                    this.Reference = new WeakReference(target);
                }
                return;
            }
            if (!func.Method.IsPublic || (target != null && !target.GetType().IsPublic && !target.GetType().IsNestedPublic))
            {
                this._func = func;
            }
            else
            {
                string name = func.Method.Name;
                if (name.Contains("<") && name.Contains(">"))
                {
                    this._func = func;
                }
                else
                {
                    this.Method = func.Method;
                    this.FuncReference = new WeakReference(func.Target);
                }
            }
            this.Reference = new WeakReference(target);
        }
        /// <summary>
        /// Executes the action. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        public TResult Execute()
        {
            if (this._staticFunc != null)
            {
                return this._staticFunc.Invoke();
            }
            if (this.IsAlive)
            {
                if (this.Method != null && this.FuncReference != null)
                {
                    return (TResult)((object)this.Method.Invoke(this.FuncTarget, null));
                }
                if (this._func != null)
                {
                    return this._func.Invoke();
                }
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
            this._func = null;
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
        private Func<T, TResult> _func;
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
                    return this._staticFunc.Method.Name;
                }
                if (this._func != null)
                {
                    return this._func.Method.Name;
                }
                if (base.Method != null)
                {
                    return base.Method.Name;
                }
                return string.Empty;
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
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakFunc(Func<T, TResult> func)
            : this(func.Target, func)
        {
        }
        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The func's owner.</param>
        /// <param name="func">The func that will be associated to this instance.</param>
        public WeakFunc(object target, Func<T, TResult> func)
        {
            if (func.Method.IsStatic)
            {
                this._staticFunc = func;
                if (target != null)
                {
                    base.Reference = new WeakReference(target);
                }
                return;
            }
            if (!func.Method.IsPublic || (target != null && !target.GetType().IsPublic && !target.GetType().IsNestedPublic))
            {
                this._func = func;
            }
            else
            {
                string name = func.Method.Name;
                if (name.Contains("<") && name.Contains(">"))
                {
                    this._func = func;
                }
                else
                {
                    base.Method = func.Method;
                    base.FuncReference = new WeakReference(func.Target);
                }
            }
            base.Reference = new WeakReference(target);
        }
        /// <summary>
        /// Executes the func. This only happens if the func's owner
        /// is still alive. The func's parameter is set to default(T).
        /// </summary>
        public TResult Execute()
        {
            return this.Execute(default(T));
        }
        /// <summary>
        /// Executes the func. This only happens if the func's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        public TResult Execute(T parameter)
        {
            if (this._staticFunc != null)
            {
                return this._staticFunc.Invoke(parameter);
            }
            if (this.IsAlive)
            {
                if (base.Method != null && base.FuncReference != null)
                {
                    return (TResult)((object)base.Method.Invoke(base.FuncTarget, new object[]
					{
						parameter
					}));
                }
                if (this._func != null)
                {
                    return this._func.Invoke(parameter);
                }
            }
            return default(TResult);
        }
        /// <summary>
        /// Executes the func with a parameter of type object. This parameter
        /// will be casted to T. This method implements <see cref="M:GalaSoft.MvvmLight.Helpers.IExecuteWithObject.ExecuteWithObject(System.Object)" />
        /// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the func after
        /// being casted to T.</param>
        /// <returns>The result of the execution as object, to be casted to T.</returns>
        public object ExecuteWithObject(object parameter)
        {
            T parameter2 = (T)((object)parameter);
            return this.Execute(parameter2);
        }
        /// <summary>
        /// Sets all the funcs that this WeakFunc contains to null,
        /// which is a signal for containing objects that this WeakFunc
        /// should be deleted.
        /// </summary>
        public void MarkForDeletion()
        {
            this._func = null;
            this._staticFunc = null;
            base.MarkForDeletion();
        }
    }
    #endregion
}
