using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace Silverlight.MVVM.Core
{
    #region  DialogMessage
    /// <summary>
    /// Use this class to send a message requesting to display a message box with features
    /// corresponding to this message's properties. The Callback property should be used
    /// to notify the message's sender about the user's choice in the message box.
    /// Typically, you can use this message typ's recipient will be an element of the View,
    /// and the sender will possibly be a ViewModel.
    /// </summary>
    public class DialogMessage : GenericMessage<string>
    {
        /// <summary>
        /// Gets or sets the buttons displayed by the message box.
        /// </summary>
        public MessageBoxButton Button
        {
            get;
            set;
        }
        /// <summary>
        /// Gets a callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.
        /// </summary>
        public Action<MessageBoxResult> Callback
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets or sets the caption for the message box.
        /// </summary>
        public string Caption
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets which result is the default in the message box.
        /// </summary>
        public MessageBoxResult DefaultResult
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the DialogMessage class.
        /// </summary>
        /// <param name="content">The text displayed by the message box.</param>
        /// <param name="callback">A callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.</param>
        public DialogMessage(string content, Action<MessageBoxResult> callback)
            : base(content)
        {
            this.Callback = callback;
        }
        /// <summary>
        /// Initializes a new instance of the DialogMessage class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="content">The text displayed by the message box.</param>
        /// <param name="callback">A callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.</param>
        public DialogMessage(object sender, string content, Action<MessageBoxResult> callback)
            : base(sender, content)
        {
            this.Callback = callback;
        }
        /// <summary>
        /// Initializes a new instance of the DialogMessage class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="content">The text displayed by the message box.</param>
        /// <param name="callback">A callback method that should be executed to deliver the result
        /// of the message box to the object that sent the message.</param>
        public DialogMessage(object sender, object target, string content, Action<MessageBoxResult> callback)
            : base(sender, target, content)
        {
            this.Callback = callback;
        }
        /// <summary>
        /// Utility method, checks if the <see cref="P:GalaSoft.MvvmLight.Messaging.DialogMessage.Callback" /> property is
        /// null, and if it is not null, executes it.
        /// </summary>
        /// <param name="result">The result that must be passed
        /// to the dialog message caller.</param>
        public void ProcessCallback(MessageBoxResult result)
        {
            if (this.Callback != null)
            {
                this.Callback.Invoke(result);
            }
        }
    }
    #endregion

    #region  GenericMessage<T>
    /// <summary>
    /// Passes a generic value (Content) to a recipient.
    /// </summary>
    /// <typeparam name="T">The type of the Content property.</typeparam>
    public class GenericMessage<T> : MessageBase
    {
        /// <summary>
        /// Gets or sets the message's content.
        /// </summary>
        public T Content
        {
            get;
            protected set;
        }
        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="content">The message content.</param>
        public GenericMessage(T content)
        {
            this.Content = content;
        }
        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="content">The message content.</param>
        public GenericMessage(object sender, T content)
            : base(sender)
        {
            this.Content = content;
        }
        /// <summary>
        /// Initializes a new instance of the GenericMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="content">The message content.</param>
        public GenericMessage(object sender, object target, T content)
            : base(sender, target)
        {
            this.Content = content;
        }
    }
    #endregion

    #region  IMessenger
    /// <summary>
    /// The Messenger is a class allowing objects to exchange messages.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Registers a recipient for a type of message TMessage. The action
        /// parameter will be executed when a corresponding message is sent.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        void Register<TMessage>(object recipient, Action<TMessage> action);
        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        /// The action parameter will be executed when a corresponding 
        /// message is sent. See the receiveDerivedMessagesToo parameter
        /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
        /// messages implementing TMessage) can be received too.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        void Register<TMessage>(object recipient, object token, Action<TMessage> action);
        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        /// The action parameter will be executed when a corresponding 
        /// message is sent. See the receiveDerivedMessagesToo parameter
        /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
        /// messages implementing TMessage) can be received too.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
        /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.
        /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
        /// transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.</para>
        /// </param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action);
        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        /// The action parameter will be executed when a corresponding 
        /// message is sent. See the receiveDerivedMessagesToo parameter
        /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
        /// messages implementing TMessage) can be received too.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
        /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.
        /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
        /// transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.</para>
        /// </param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action);
        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach all recipients that registered for this message type
        /// using one of the Register methods.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        void Send<TMessage>(TMessage message);
        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach only recipients that registered for this message type
        /// using one of the Register methods, and that are
        /// of the targetType.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <typeparam name="TTarget">The type of recipients that will receive
        /// the message. The message won't be sent to recipients of another type.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        void Send<TMessage, TTarget>(TMessage message);
        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach only recipients that registered for this message type
        /// using one of the Register methods, and that are
        /// of the targetType.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        void Send<TMessage>(TMessage message, object token);
        /// <summary>
        /// Unregisters a messager recipient completely. After this method
        /// is executed, the recipient will not receive any messages anymore.
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        void Unregister(object recipient);
        /// <summary>
        /// Unregisters a message recipient for a given type of messages only. 
        /// After this method is executed, the recipient will not receive messages
        /// of type TMessage anymore, but will still receive other message types (if it
        /// registered for them previously).
        /// </summary>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        void Unregister<TMessage>(object recipient);
        /// <summary>
        /// Unregisters a message recipient for a given type of messages only and for a given token. 
        /// After this method is executed, the recipient will not receive messages
        /// of type TMessage anymore with the given token, but will still receive other message types
        /// or messages with other tokens (if it registered for them previously).
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <param name="token">The token for which the recipient must be unregistered.</param>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        void Unregister<TMessage>(object recipient, object token);
        /// <summary>
        /// Unregisters a message recipient for a given type of messages and for
        /// a given action. Other message types will still be transmitted to the
        /// recipient (if it registered for them previously). Other actions that have
        /// been registered for the message type TMessage and for the given recipient (if
        /// available) will also remain available.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <param name="action">The action that must be unregistered for
        /// the recipient and for the message type TMessage.</param>
        void Unregister<TMessage>(object recipient, Action<TMessage> action);
        /// <summary>
        /// Unregisters a message recipient for a given type of messages, for
        /// a given action and a given token. Other message types will still be transmitted to the
        /// recipient (if it registered for them previously). Other actions that have
        /// been registered for the message type TMessage, for the given recipient and other tokens (if
        /// available) will also remain available.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <param name="token">The token for which the recipient must be unregistered.</param>
        /// <param name="action">The action that must be unregistered for
        /// the recipient and for the message type TMessage.</param>
        void Unregister<TMessage>(object recipient, object token, Action<TMessage> action);

        /// <summary>
        /// Base class for all messages broadcasted by the Messenger.
        /// You can create your own message types by extending this class.
        /// </summary>
    }
    #endregion

    #region  MessageBase
    /// <summary>
    /// Base class for all messages broadcasted by the Messenger.
    /// You can create your own message types by extending this class.
    /// </summary>
    public class MessageBase
    {
        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        public object Sender
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets or sets the message's intended target. This property can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.
        /// </summary>
        public object Target
        {
            get;
            protected set;
        }
        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        public MessageBase()
        {
        }
        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        public MessageBase(object sender)
        {
            this.Sender = sender;
        }
        /// <summary>
        /// Initializes a new instance of the MessageBase class.
        /// </summary>
        /// <param name="sender">The message's original sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        public MessageBase(object sender, object target)
            : this(sender)
        {
            this.Target = target;
        }
    }
    #endregion

    #region  Messenger
    /// <summary>
    /// The Messenger is a class allowing objects to exchange messages.
    /// </summary>
    public class Messenger : IMessenger
    {
        private struct WeakActionAndToken
        {
            public WeakAction Action;
            public object Token;
        }
        private static readonly object CreationLock = new object();
        private static IMessenger _defaultInstance;
        private readonly object _registerLock = new object();
        private Dictionary<Type, List<Messenger.WeakActionAndToken>> _recipientsOfSubclassesAction;
        private Dictionary<Type, List<Messenger.WeakActionAndToken>> _recipientsStrictAction;
        private bool _isCleanupRegistered;
        /// <summary>
        /// Gets the Messenger's default instance, allowing
        /// to register and send messages in a static manner.
        /// </summary>
        public static IMessenger Default
        {
            get
            {
                if (Messenger._defaultInstance == null)
                {
                    lock (Messenger.CreationLock)
                    {
                        if (Messenger._defaultInstance == null)
                        {
                            Messenger._defaultInstance = new Messenger();
                        }
                    }
                }
                return Messenger._defaultInstance;
            }
        }
        /// <summary>
        /// Registers a recipient for a type of message TMessage. The action
        /// parameter will be executed when a corresponding message is sent.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            this.Register<TMessage>(recipient, null, false, action);
        }
        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        /// The action parameter will be executed when a corresponding 
        /// message is sent. See the receiveDerivedMessagesToo parameter
        /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
        /// messages implementing TMessage) can be received too.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
        /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.
        /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
        /// transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.</para>
        /// </param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            this.Register<TMessage>(recipient, null, receiveDerivedMessagesToo, action);
        }
        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        /// The action parameter will be executed when a corresponding 
        /// message is sent.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            this.Register<TMessage>(recipient, token, false, action);
        }
        /// <summary>
        /// Registers a recipient for a type of message TMessage.
        /// The action parameter will be executed when a corresponding 
        /// message is sent. See the receiveDerivedMessagesToo parameter
        /// for details on how messages deriving from TMessage (or, if TMessage is an interface,
        /// messages implementing TMessage) can be received too.
        /// <para>Registering a recipient does not create a hard reference to it,
        /// so if this recipient is deleted, no memory leak is caused.</para>
        /// </summary>
        /// <typeparam name="TMessage">The type of message that the recipient registers
        /// for.</typeparam>
        /// <param name="recipient">The recipient that will receive the messages.</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        /// <param name="receiveDerivedMessagesToo">If true, message types deriving from
        /// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.
        /// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
        /// transmitted to the recipient. For example, if a SendOrderMessage
        /// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
        /// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
        /// and ExecuteOrderMessage to the recipient that registered.</para>
        /// </param>
        /// <param name="action">The action that will be executed when a message
        /// of type TMessage is sent.</param>
        public virtual void Register<TMessage>(object recipient, object token, bool receiveDerivedMessagesToo, Action<TMessage> action)
        {
            lock (this._registerLock)
            {
                Type typeFromHandle = typeof(TMessage);
                Dictionary<Type, List<Messenger.WeakActionAndToken>> dictionary;
                if (receiveDerivedMessagesToo)
                {
                    if (this._recipientsOfSubclassesAction == null)
                    {
                        this._recipientsOfSubclassesAction = new Dictionary<Type, List<Messenger.WeakActionAndToken>>();
                    }
                    dictionary = this._recipientsOfSubclassesAction;
                }
                else
                {
                    if (this._recipientsStrictAction == null)
                    {
                        this._recipientsStrictAction = new Dictionary<Type, List<Messenger.WeakActionAndToken>>();
                    }
                    dictionary = this._recipientsStrictAction;
                }
                lock (dictionary)
                {
                    List<Messenger.WeakActionAndToken> list;
                    if (!dictionary.ContainsKey(typeFromHandle))
                    {
                        list = new List<Messenger.WeakActionAndToken>();
                        dictionary.Add(typeFromHandle, list);
                    }
                    else
                    {
                        list = dictionary[typeFromHandle];
                    }
                    WeakAction<TMessage> action2 = new WeakAction<TMessage>(recipient, action);
                    Messenger.WeakActionAndToken weakActionAndToken = new Messenger.WeakActionAndToken
                    {
                        Action = action2,
                        Token = token
                    };
                    list.Add(weakActionAndToken);
                }
            }
            this.RequestCleanup();
        }
        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach all recipients that registered for this message type
        /// using one of the Register methods.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        public virtual void Send<TMessage>(TMessage message)
        {
            this.SendToTargetOrType<TMessage>(message, null, null);
        }
        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach only recipients that registered for this message type
        /// using one of the Register methods, and that are
        /// of the targetType.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <typeparam name="TTarget">The type of recipients that will receive
        /// the message. The message won't be sent to recipients of another type.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        public virtual void Send<TMessage, TTarget>(TMessage message)
        {
            this.SendToTargetOrType<TMessage>(message, typeof(TTarget), null);
        }
        /// <summary>
        /// Sends a message to registered recipients. The message will
        /// reach only recipients that registered for this message type
        /// using one of the Register methods, and that are
        /// of the targetType.
        /// </summary>
        /// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
        /// <param name="message">The message to send to registered recipients.</param>
        /// <param name="token">A token for a messaging channel. If a recipient registers
        /// using a token, and a sender sends a message using the same token, then this
        /// message will be delivered to the recipient. Other recipients who did not
        /// use a token when registering (or who used a different token) will not
        /// get the message. Similarly, messages sent without any token, or with a different
        /// token, will not be delivered to that recipient.</param>
        public virtual void Send<TMessage>(TMessage message, object token)
        {
            this.SendToTargetOrType<TMessage>(message, null, token);
        }
        /// <summary>
        /// Unregisters a messager recipient completely. After this method
        /// is executed, the recipient will not receive any messages anymore.
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        public virtual void Unregister(object recipient)
        {
            Messenger.UnregisterFromLists(recipient, this._recipientsOfSubclassesAction);
            Messenger.UnregisterFromLists(recipient, this._recipientsStrictAction);
        }
        /// <summary>
        /// Unregisters a message recipient for a given type of messages only. 
        /// After this method is executed, the recipient will not receive messages
        /// of type TMessage anymore, but will still receive other message types (if it
        /// registered for them previously).
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        public virtual void Unregister<TMessage>(object recipient)
        {
            this.Unregister<TMessage>(recipient, null, null);
        }
        /// <summary>
        /// Unregisters a message recipient for a given type of messages only and for a given token. 
        /// After this method is executed, the recipient will not receive messages
        /// of type TMessage anymore with the given token, but will still receive other message types
        /// or messages with other tokens (if it registered for them previously).
        /// </summary>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <param name="token">The token for which the recipient must be unregistered.</param>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        public virtual void Unregister<TMessage>(object recipient, object token)
        {
            this.Unregister<TMessage>(recipient, token, null);
        }
        /// <summary>
        /// Unregisters a message recipient for a given type of messages and for
        /// a given action. Other message types will still be transmitted to the
        /// recipient (if it registered for them previously). Other actions that have
        /// been registered for the message type TMessage and for the given recipient (if
        /// available) will also remain available.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <param name="action">The action that must be unregistered for
        /// the recipient and for the message type TMessage.</param>
        public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
        {
            this.Unregister<TMessage>(recipient, null, action);
        }
        /// <summary>
        /// Unregisters a message recipient for a given type of messages, for
        /// a given action and a given token. Other message types will still be transmitted to the
        /// recipient (if it registered for them previously). Other actions that have
        /// been registered for the message type TMessage, for the given recipient and other tokens (if
        /// available) will also remain available.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages that the recipient wants
        /// to unregister from.</typeparam>
        /// <param name="recipient">The recipient that must be unregistered.</param>
        /// <param name="token">The token for which the recipient must be unregistered.</param>
        /// <param name="action">The action that must be unregistered for
        /// the recipient and for the message type TMessage.</param>
        public virtual void Unregister<TMessage>(object recipient, object token, Action<TMessage> action)
        {
            Messenger.UnregisterFromLists<TMessage>(recipient, token, action, this._recipientsStrictAction);
            Messenger.UnregisterFromLists<TMessage>(recipient, token, action, this._recipientsOfSubclassesAction);
            this.RequestCleanup();
        }
        /// <summary>
        /// Provides a way to override the Messenger.Default instance with
        /// a custom instance, for example for unit testing purposes.
        /// </summary>
        /// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
        public static void OverrideDefault(IMessenger newMessenger)
        {
            Messenger._defaultInstance = newMessenger;
        }
        /// <summary>
        /// Sets the Messenger's default (static) instance to null.
        /// </summary>
        public static void Reset()
        {
            Messenger._defaultInstance = null;
        }
        /// <summary>
        /// Provides a non-static access to the static <see cref="M:GalaSoft.MvvmLight.Messaging.Messenger.Reset" /> method.
        /// Sets the Messenger's default (static) instance to null.
        /// </summary>
        public void ResetAll()
        {
            Messenger.Reset();
        }
        private static void CleanupList(IDictionary<Type, List<Messenger.WeakActionAndToken>> lists)
        {
            if (lists == null)
            {
                return;
            }
            lock (lists)
            {
                List<Type> list = new List<Type>();
                using (IEnumerator<KeyValuePair<Type, List<Messenger.WeakActionAndToken>>> enumerator = lists.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<Type, List<Messenger.WeakActionAndToken>> current = enumerator.Current;
                        List<Messenger.WeakActionAndToken> list2 = new List<Messenger.WeakActionAndToken>();
                        using (List<Messenger.WeakActionAndToken>.Enumerator enumerator2 = current.Value.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Messenger.WeakActionAndToken current2 = enumerator2.Current;
                                if (current2.Action == null || !current2.Action.IsAlive)
                                {
                                    list2.Add(current2);
                                }
                            }
                        }
                        using (List<Messenger.WeakActionAndToken>.Enumerator enumerator3 = list2.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                Messenger.WeakActionAndToken current3 = enumerator3.Current;
                                current.Value.Remove(current3);
                            }
                        }
                        if (current.Value.Count == 0)
                        {
                            list.Add(current.Key);
                        }
                    }
                }
                using (List<Type>.Enumerator enumerator4 = list.GetEnumerator())
                {
                    while (enumerator4.MoveNext())
                    {
                        Type current4 = enumerator4.Current;
                        lists.Remove(current4);
                    }
                }
            }
        }
        private static void SendToList<TMessage>(TMessage message, IEnumerable<Messenger.WeakActionAndToken> list, Type messageTargetType, object token)
        {
            if (list != null)
            {
                List<Messenger.WeakActionAndToken> list2 = Enumerable.ToList<Messenger.WeakActionAndToken>(Enumerable.Take<Messenger.WeakActionAndToken>(list, Enumerable.Count<Messenger.WeakActionAndToken>(list)));
                using (List<Messenger.WeakActionAndToken>.Enumerator enumerator = list2.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Messenger.WeakActionAndToken current = enumerator.Current;
                        IExecuteWithObject executeWithObject = current.Action as IExecuteWithObject;
                        if (executeWithObject != null && current.Action.IsAlive && current.Action.Target != null && (messageTargetType == null || current.Action.Target.GetType() == messageTargetType || messageTargetType.IsAssignableFrom(current.Action.Target.GetType())) && ((current.Token == null && token == null) || (current.Token != null && current.Token.Equals(token))))
                        {
                            executeWithObject.ExecuteWithObject(message);
                        }
                    }
                }
            }
        }
        private static void UnregisterFromLists(object recipient, Dictionary<Type, List<Messenger.WeakActionAndToken>> lists)
        {
            if (recipient == null || lists == null || lists.Count == 0)
            {
                return;
            }
            lock (lists)
            {
                using (Dictionary<Type, List<Messenger.WeakActionAndToken>>.KeyCollection.Enumerator enumerator = lists.Keys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Type current = enumerator.Current;
                        using (List<Messenger.WeakActionAndToken>.Enumerator enumerator2 = lists[current].GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Messenger.WeakActionAndToken current2 = enumerator2.Current;
                                IExecuteWithObject executeWithObject = (IExecuteWithObject)current2.Action;
                                if (executeWithObject != null && recipient == executeWithObject.Target)
                                {
                                    executeWithObject.MarkForDeletion();
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void UnregisterFromLists<TMessage>(object recipient, object token, Action<TMessage> action, Dictionary<Type, List<Messenger.WeakActionAndToken>> lists)
        {
            Type typeFromHandle = typeof(TMessage);
            if (recipient == null || lists == null || lists.Count == 0 || !lists.ContainsKey(typeFromHandle))
            {
                return;
            }
            lock (lists)
            {
                using (List<Messenger.WeakActionAndToken>.Enumerator enumerator = lists[typeFromHandle].GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Messenger.WeakActionAndToken current = enumerator.Current;
                        WeakAction<TMessage> weakAction = current.Action as WeakAction<TMessage>;
                        if (weakAction != null && recipient == weakAction.Target && (action == null || action.Method.Name == weakAction.MethodName) && (token == null || token.Equals(current.Token)))
                        {
                            current.Action.MarkForDeletion();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Notifies the Messenger that the lists of recipients should
        /// be scanned and cleaned up.
        /// Since recipients are stored as <see cref="T:System.WeakReference" />, 
        /// recipients can be garbage collected even though the Messenger keeps 
        /// them in a list. During the cleanup operation, all "dead"
        /// recipients are removed from the lists. Since this operation
        /// can take a moment, it is only executed when the application is
        /// idle. For this reason, a user of the Messenger class should use
        /// <see cref="M:GalaSoft.MvvmLight.Messaging.Messenger.RequestCleanup" /> instead of forcing one with the 
        /// <see cref="M:GalaSoft.MvvmLight.Messaging.Messenger.Cleanup" /> method.
        /// </summary>
        public void RequestCleanup()
        {
            if (!this._isCleanupRegistered)
            {
                Action action = new Action(this.Cleanup);
                Deployment.Current.Dispatcher.BeginInvoke(action);
                this._isCleanupRegistered = true;
            }
        }
        /// <summary>
        /// Scans the recipients' lists for "dead" instances and removes them.
        /// Since recipients are stored as <see cref="T:System.WeakReference" />, 
        /// recipients can be garbage collected even though the Messenger keeps 
        /// them in a list. During the cleanup operation, all "dead"
        /// recipients are removed from the lists. Since this operation
        /// can take a moment, it is only executed when the application is
        /// idle. For this reason, a user of the Messenger class should use
        /// <see cref="M:GalaSoft.MvvmLight.Messaging.Messenger.RequestCleanup" /> instead of forcing one with the 
        /// <see cref="M:GalaSoft.MvvmLight.Messaging.Messenger.Cleanup" /> method.
        /// </summary>
        public void Cleanup()
        {
            Messenger.CleanupList(this._recipientsOfSubclassesAction);
            Messenger.CleanupList(this._recipientsStrictAction);
            this._isCleanupRegistered = false;
        }
        private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
        {
            Type typeFromHandle = typeof(TMessage);
            if (this._recipientsOfSubclassesAction != null)
            {
                List<Type> list = Enumerable.ToList<Type>(Enumerable.Take<Type>(this._recipientsOfSubclassesAction.Keys, Enumerable.Count<KeyValuePair<Type, List<Messenger.WeakActionAndToken>>>(this._recipientsOfSubclassesAction)));
                using (List<Type>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Type current = enumerator.Current;
                        List<Messenger.WeakActionAndToken> list2 = null;
                        if (typeFromHandle == current || typeFromHandle.IsSubclassOf(current) || current.IsAssignableFrom(typeFromHandle))
                        {
                            lock (this._recipientsOfSubclassesAction)
                            {
                                list2 = Enumerable.ToList<Messenger.WeakActionAndToken>(Enumerable.Take<Messenger.WeakActionAndToken>(this._recipientsOfSubclassesAction[current], Enumerable.Count<Messenger.WeakActionAndToken>(this._recipientsOfSubclassesAction[current])));
                            }
                        }
                        Messenger.SendToList<TMessage>(message, list2, messageTargetType, token);
                    }
                }
            }
            if (this._recipientsStrictAction != null)
            {
                lock (this._recipientsStrictAction)
                {
                    if (this._recipientsStrictAction.ContainsKey(typeFromHandle))
                    {
                        List<Messenger.WeakActionAndToken> list3 = Enumerable.ToList<Messenger.WeakActionAndToken>(Enumerable.Take<Messenger.WeakActionAndToken>(this._recipientsStrictAction[typeFromHandle], Enumerable.Count<Messenger.WeakActionAndToken>(this._recipientsStrictAction[typeFromHandle])));
                        Messenger.SendToList<TMessage>(message, list3, messageTargetType, token);
                    }
                }
            }
            this.RequestCleanup();
        }
    }
    #endregion

    #region  NotificationMessage
    /// <summary>
    /// Passes a string message (Notification) to a recipient.
    /// <para>Typically, notifications are defined as unique strings in a static class. To define
    /// a unique string, you can use Guid.NewGuid().ToString() or any other unique
    /// identifier.</para>
    /// </summary>
    public class NotificationMessage : MessageBase
    {
        /// <summary>
        /// Gets a string containing any arbitrary message to be
        /// passed to recipient(s).
        /// </summary>
        public string Notification
        {
            get;
            private set;
        }
        /// <summary>
        /// Initializes a new instance of the NotificationMessage class.
        /// </summary>
        /// <param name="notification">A string containing any arbitrary message to be
        /// passed to recipient(s)</param>
        public NotificationMessage(string notification)
        {
            this.Notification = notification;
        }
        /// <summary>
        /// Initializes a new instance of the NotificationMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="notification">A string containing any arbitrary message to be
        /// passed to recipient(s)</param>
        public NotificationMessage(object sender, string notification)
            : base(sender)
        {
            this.Notification = notification;
        }
        /// <summary>
        /// Initializes a new instance of the NotificationMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="notification">A string containing any arbitrary message to be
        /// passed to recipient(s)</param>
        public NotificationMessage(object sender, object target, string notification)
            : base(sender, target)
        {
            this.Notification = notification;
        }
    }
    #endregion

    #region  NotificationMessage<T>
    /// <summary>
    /// Passes a string message (Notification) and a generic value (Content) to a recipient.
    /// </summary>
    /// <typeparam name="T">The type of the Content property.</typeparam>
    public class NotificationMessage<T> : GenericMessage<T>
    {
        /// <summary>
        /// Gets a string containing any arbitrary message to be
        /// passed to recipient(s).
        /// </summary>
        public string Notification
        {
            get;
            private set;
        }
        /// <summary>
        /// Initializes a new instance of the NotificationMessage class.
        /// </summary>
        /// <param name="content">A value to be passed to recipient(s).</param>
        /// <param name="notification">A string containing any arbitrary message to be
        /// passed to recipient(s)</param>
        public NotificationMessage(T content, string notification)
            : base(content)
        {
            this.Notification = notification;
        }
        /// <summary>
        /// Initializes a new instance of the NotificationMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="content">A value to be passed to recipient(s).</param>
        /// <param name="notification">A string containing any arbitrary message to be
        /// passed to recipient(s)</param>
        public NotificationMessage(object sender, T content, string notification)
            : base(sender, content)
        {
            this.Notification = notification;
        }
        /// <summary>
        /// Initializes a new instance of the NotificationMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="content">A value to be passed to recipient(s).</param>
        /// <param name="notification">A string containing any arbitrary message to be
        /// passed to recipient(s)</param>
        public NotificationMessage(object sender, object target, T content, string notification)
            : base(sender, target, content)
        {
            this.Notification = notification;
        }
    }
    #endregion

    #region  NotificationMessageAction
    /// <summary>
    /// Provides a message class with a built-in callback. When the recipient
    /// is done processing the message, it can execute the callback to
    /// notify the sender that it is done. Use the <see cref="M:GalaSoft.MvvmLight.Messaging.NotificationMessageAction.Execute" />
    /// method to execute the callback.
    /// </summary>
    public class NotificationMessageAction : NotificationMessageWithCallback
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction" /> class.
        /// </summary>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageAction(string notification, Action callback)
            : base(notification, callback)
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageAction(object sender, string notification, Action callback)
            : base(sender, notification, callback)
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageAction(object sender, object target, string notification, Action callback)
            : base(sender, target, notification, callback)
        {
        }
        /// <summary>
        /// Executes the callback that was provided with the message.
        /// </summary>
        public void Execute()
        {
            base.Execute(new object[0]);
        }
    }
    #endregion

    #region  NotificationMessageAction<TCallbackParameter>
    /// <summary>
    /// Provides a message class with a built-in callback. When the recipient
    /// is done processing the message, it can execute the callback to
    /// notify the sender that it is done. Use the <see cref="M:GalaSoft.MvvmLight.Messaging.NotificationMessageAction`1.Execute(`0)" />
    /// method to execute the callback. The callback method has one parameter.
    /// <seealso cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction" />.
    /// </summary>
    /// <typeparam name="TCallbackParameter">The type of the callback method's
    /// only parameter.</typeparam>
    public class NotificationMessageAction<TCallbackParameter> : NotificationMessageWithCallback
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction`1" /> class.
        /// </summary>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageAction(string notification, Action<TCallbackParameter> callback)
            : base(notification, callback)
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction`1" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageAction(object sender, string notification, Action<TCallbackParameter> callback)
            : base(sender, notification, callback)
        {
        }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction`1" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageAction(object sender, object target, string notification, Action<TCallbackParameter> callback)
            : base(sender, target, notification, callback)
        {
        }
        /// <summary>
        /// Executes the callback that was provided with the message.
        /// </summary>
        /// <param name="parameter">A parameter requested by the message's
        /// sender and providing additional information on the recipient's
        /// state.</param>
        public void Execute(TCallbackParameter parameter)
        {
            base.Execute(new object[]
			{
				parameter
			});
        }
    }
    #endregion

    #region  NotificationMessageWithCallback
    /// <summary>
    /// Provides a message class with a built-in callback. When the recipient
    /// is done processing the message, it can execute the callback to
    /// notify the sender that it is done. Use the <see cref="M:GalaSoft.MvvmLight.Messaging.NotificationMessageWithCallback.Execute(System.Object[])" />
    /// method to execute the callback. The callback method has one parameter.
    /// <seealso cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction" /> and
    /// <seealso cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageAction`1" />.
    /// </summary>
    public class NotificationMessageWithCallback : NotificationMessage
    {
        private readonly Delegate _callback;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageWithCallback" /> class.
        /// </summary>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageWithCallback(string notification, Delegate callback)
            : base(notification)
        {
            NotificationMessageWithCallback.CheckCallback(callback);
            this._callback = callback;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageWithCallback" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageWithCallback(object sender, string notification, Delegate callback)
            : base(sender, notification)
        {
            NotificationMessageWithCallback.CheckCallback(callback);
            this._callback = callback;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.NotificationMessageWithCallback" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="notification">An arbitrary string that will be
        /// carried by the message.</param>
        /// <param name="callback">The callback method that can be executed
        /// by the recipient to notify the sender that the message has been
        /// processed.</param>
        public NotificationMessageWithCallback(object sender, object target, string notification, Delegate callback)
            : base(sender, target, notification)
        {
            NotificationMessageWithCallback.CheckCallback(callback);
            this._callback = callback;
        }
        /// <summary>
        /// Executes the callback that was provided with the message with an
        /// arbitrary number of parameters.
        /// </summary>
        /// <param name="arguments">A  number of parameters that will
        /// be passed to the callback method.</param>
        /// <returns>The object returned by the callback method.</returns>
        public virtual object Execute(params object[] arguments)
        {
            return this._callback.DynamicInvoke(arguments);
        }
        private static void CheckCallback(Delegate callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback", "Callback may not be null");
            }
        }
    }
    #endregion

    #region  PropertyChangedMessage<T>
    /// <summary>
    /// Passes a string property name (PropertyName) and a generic value
    /// (<see cref="P:GalaSoft.MvvmLight.Messaging.PropertyChangedMessage`1.OldValue" /> and <see cref="P:GalaSoft.MvvmLight.Messaging.PropertyChangedMessage`1.NewValue" />) to a recipient.
    /// This message type can be used to propagate a PropertyChanged event to
    /// a recipient using the messenging system.
    /// </summary>
    /// <typeparam name="T">The type of the OldValue and NewValue property.</typeparam>
    public class PropertyChangedMessage<T> : PropertyChangedMessageBase
    {
        /// <summary>
        /// Gets the value that the property has after the change.
        /// </summary>
        public T NewValue
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the value that the property had before the change.
        /// </summary>
        public T OldValue
        {
            get;
            private set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.PropertyChangedMessage`1" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public PropertyChangedMessage(object sender, T oldValue, T newValue, string propertyName)
            : base(sender, propertyName)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.PropertyChangedMessage`1" /> class.
        /// </summary>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public PropertyChangedMessage(T oldValue, T newValue, string propertyName)
            : base(propertyName)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.PropertyChangedMessage`1" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public PropertyChangedMessage(object sender, object target, T oldValue, T newValue, string propertyName)
            : base(sender, target, propertyName)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
    #endregion

    #region  PropertyChangedMessageBase
    /// <summary>
    /// Basis class for the <see cref="T:GalaSoft.MvvmLight.Messaging.PropertyChangedMessage`1" /> class. This
    /// class allows a recipient to register for all PropertyChangedMessages without
    /// having to specify the type T.
    /// </summary>
    public abstract class PropertyChangedMessageBase : MessageBase
    {
        /// <summary>
        /// Gets or sets the name of the property that changed.
        /// </summary>
        public string PropertyName
        {
            get;
            protected set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.PropertyChangedMessageBase" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected PropertyChangedMessageBase(object sender, string propertyName)
            : base(sender)
        {
            this.PropertyName = propertyName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.PropertyChangedMessageBase" /> class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used
        /// to give an indication as to whom the message was intended for. Of course
        /// this is only an indication, amd may be null.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected PropertyChangedMessageBase(object sender, object target, string propertyName)
            : base(sender, target)
        {
            this.PropertyName = propertyName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GalaSoft.MvvmLight.Messaging.PropertyChangedMessageBase" /> class.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected PropertyChangedMessageBase(string propertyName)
        {
            this.PropertyName = propertyName;
        }
    }
    #endregion
}
