﻿using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace Slavi
{
    public class EventQueue
    {
        private static EventQueue _instance = null;
        public static EventQueue Instance { get { return _instance ?? (_instance = new EventQueue()); } }

        private Queue<Event> _events = new Queue<Event>();

        public delegate void EventHandler<in T>(T delegateEvent) where T : Event;
        private delegate void EventDelegate(Event delegateEvent);

        private Dictionary<System.Type, EventDelegate> _delegates = new Dictionary<System.Type, EventDelegate>();
        private Dictionary<System.Delegate, EventDelegate> _handlersToDelegates = new Dictionary<System.Delegate, EventDelegate>();

        #region Listeners
        /// <summary>
        /// Adds/subscribes a listener (method) <paramref name="handler"/> to listen for (to be triggered by) event <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of event to listen for.</typeparam>
        /// <param name="handler">The handling method.</param>
        public void AddListener<T>(EventHandler<T> handler)
            where T : Event
        {
            //This is needed, because two event handlers (possibly the same method) have an equal reference/value,
            //therefore the dictionary does not store them as separate objects, therefore it is impossible to
            //assign the same event handler over and over again. Possibly could be done with the actual MS EventHandlers.
            if (_handlersToDelegates.ContainsKey(handler))
            {
                Debug.LogWarning("You cannot add the same method/event handler as a listener twice. Please use another way.");
                return;
            }

            //Create a new non-generic delegate which calls our handler.
            //This is the delegate we actually invoke.
            EventDelegate internalDelegate = addedDelegate => handler((T)addedDelegate);
            //Define relationship by assigning delegate to handler. Basically keep track of subscriptions
            _handlersToDelegates[handler] = internalDelegate;

            //Add the non-generic delegate to the 'invocation' list, if there are other 
            //delegates present for the event type, add it, otherwise assign it.
            EventDelegate temporaryDelegate;
            if (_delegates.TryGetValue(typeof(T), out temporaryDelegate))
                _delegates[typeof(T)] = temporaryDelegate += internalDelegate;
            else
                _delegates[typeof(T)] = internalDelegate;
        }

        /// <summary>
        /// Removes/unsubscribes a listener (method) <paramref name="handler"/> from event <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of event to stop listening for/unsubscribe.</typeparam>
        /// <param name="handler">The handling method.</param>
        public void RemoveListener<T>(EventHandler<T> handler)
            where T : Event
        {
            //Get the subscribed handler from the 'invocation' list
            EventDelegate internalDelegate;
            if (!_handlersToDelegates.TryGetValue(handler, out internalDelegate)) return;

            //Get the delegate we're about to edit
            EventDelegate temporaryDelegate;
            if (_delegates.TryGetValue(typeof(T), out temporaryDelegate))
            {
                //Unsubscribe the handler
                temporaryDelegate -= internalDelegate;
                //If there are no more delegates, kill the Event Type
                if (temporaryDelegate == null)
                    _delegates.Remove(typeof(T));
                else
                    _delegates[typeof(T)] = temporaryDelegate;
            }

            //Remove handler reference
            _handlersToDelegates.Remove(handler);
        }

        /// <summary>
        /// Checks if given handler/method is already listening/subscribed.
        /// </summary>
        /// <typeparam name="T">The type of event to listen for.</typeparam>
        /// <param name="handler">The handling method.</param>
        /// <returns>True if it is a listener/is subscribed, false otherwise.</returns>
        public bool IsListener<T>(EventHandler<T> handler)
            where T : Event
        {
            return _handlersToDelegates.ContainsKey(handler);
        }

        /// <summary>
        /// Checks if given handler/method is already listening/subscribed. Warning: Generic!
        /// </summary>
        /// <param name="handler">The handling method.</param>
        /// <returns>True if it is a listener/is subscribed, false otherwise.</returns>
        public bool IsListener(System.Delegate handler)
        {
            //Generic, should not happen often
            Debug.LogWarning("Generic Event Handler! Should not happen often (or at all).");
            return _handlersToDelegates.ContainsKey(handler);
        }

        /// <summary>
        /// Checks if given event type <typeparamref name="T"/> has any listeners.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>True if it has any listeners, false otherwise.</returns>
        public bool HasListeners<T>()
            where T : Event
        {
            return _delegates.ContainsKey(typeof(T));
        }

        #endregion
        #region Event Queueing and Delivery
        /// <summary>
        /// Sends an event to be delivered to subscribed listeners.
        /// </summary>
        /// <param name="sentEvent">The sent event.</param>
        public void Send(Event sentEvent)
        {
            _events.Enqueue(sentEvent);
            //Debug.Log("Sending Event. Events: " + _events.Count.ToString());
        }

        public void SendAndDeliver(Event sentEvent)
        {
            Deliver(sentEvent);
        }

        //Could also have been generic but typeof(T) is slightly faster than GetType(). ¯\_(ツ)_/¯
        //private void Deliver<T>(T deliveredEvent)
        //    where T : Event
        //{
        //    Debug.Log("Yo");
        //    EventDelegate eventDelegate;
        //    if (_delegates.TryGetValue(typeof(T), out eventDelegate))
        //    {
        //        Debug.Log("I am here");
        //        eventDelegate.Invoke(deliveredEvent);
        //    }
        //}

        private void Deliver(Event deliveredEvent)
        {
            EventDelegate eventDelegate;
            if (_delegates.TryGetValue(deliveredEvent.GetType(), out eventDelegate) && eventDelegate != null)
                eventDelegate.Invoke(deliveredEvent);
        }


        /// <summary>
        /// Delivers the next event in the queue.
        /// </summary>
        public void DeliverNext()
        {
            Deliver(_events.Dequeue());
            //Debug.Log("Delivered. Left: " + _events.Count.ToString());
        }

        /// <summary>
        /// Delivers all events in the queue.
        /// </summary>
        public void DeliverAll()
        {
            while (_events.Count != 0) DeliverNext();
        }
        #endregion
    }
}