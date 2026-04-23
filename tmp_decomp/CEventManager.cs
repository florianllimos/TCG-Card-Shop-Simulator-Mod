using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventManager : CSingleton<CEventManager>
{
	public delegate void EventDelegate<T>(T e) where T : CEvent;

	private delegate void EventDelegate(CEvent e);

	public bool m_limitQueueProcessing;

	public float m_queueProcessTime;

	private Queue m_queueEvent = new Queue();

	private Dictionary<Type, EventDelegate> m_listeners = new Dictionary<Type, EventDelegate>();

	private Dictionary<Delegate, EventDelegate> m_listenerLookup = new Dictionary<Delegate, EventDelegate>();

	protected CEventManager()
	{
	}

	private void Update()
	{
		float num = 0f;
		while (m_queueEvent.Count > 0 && (!m_limitQueueProcessing || !(num > m_queueProcessTime)))
		{
			CEvent evt = m_queueEvent.Dequeue() as CEvent;
			OnNotify(evt);
			if (m_limitQueueProcessing)
			{
				num += Time.deltaTime;
			}
		}
	}

	public static void QueueEvent(CEvent evt)
	{
		if ((Application.isPlaying || Application.isMobilePlatform) && !CSingleton<CEventManager>.IsQuitting)
		{
			CSingleton<CEventManager>.Instance.QueueEventPrivate(evt);
		}
	}

	public static void AddListener<T>(EventDelegate<T> listener) where T : CEvent
	{
		if ((Application.isPlaying || Application.isMobilePlatform) && !CSingleton<CEventManager>.IsQuitting)
		{
			CSingleton<CEventManager>.Instance.AddListenerPrivate(listener);
		}
	}

	public static void RemoveListener<T>(EventDelegate<T> listener) where T : CEvent
	{
		if ((Application.isPlaying || Application.isMobilePlatform) && !CSingleton<CEventManager>.IsQuitting)
		{
			CSingleton<CEventManager>.Instance.RemoveListenerPrivate(listener);
		}
	}

	private void AddListenerPrivate<T>(EventDelegate<T> listener) where T : CEvent
	{
		if (!m_listenerLookup.ContainsKey(listener))
		{
			EventDelegate eventDelegate = delegate(CEvent e)
			{
				listener((T)e);
			};
			m_listenerLookup[listener] = eventDelegate;
			if (m_listeners.TryGetValue(typeof(T), out var value))
			{
				value = (m_listeners[typeof(T)] = (EventDelegate)Delegate.Combine(value, eventDelegate));
			}
			else
			{
				m_listeners[typeof(T)] = eventDelegate;
			}
		}
	}

	public void RemoveListenerPrivate<T>(EventDelegate<T> listener) where T : CEvent
	{
		if (!m_listenerLookup.TryGetValue(listener, out var value))
		{
			return;
		}
		if (m_listeners.TryGetValue(typeof(T), out var value2))
		{
			value2 = (EventDelegate)Delegate.Remove(value2, value);
			if (value2 == null)
			{
				m_listeners.Remove(typeof(T));
			}
			else
			{
				m_listeners[typeof(T)] = value2;
			}
		}
		m_listenerLookup.Remove(listener);
	}

	public bool HasListener<T>(EventDelegate<T> listener) where T : CEvent
	{
		return m_listenerLookup.ContainsKey(listener);
	}

	private void OnNotify(CEvent evt)
	{
		if (m_listeners.TryGetValue(evt.GetType(), out var value))
		{
			value?.Invoke(evt);
		}
	}

	private bool QueueEventPrivate(CEvent evt)
	{
		if (!m_listeners.ContainsKey(evt.GetType()))
		{
			return false;
		}
		m_queueEvent.Enqueue(evt);
		return true;
	}

	private void RemoveAll()
	{
		m_listeners.Clear();
		m_listenerLookup.Clear();
	}
}
