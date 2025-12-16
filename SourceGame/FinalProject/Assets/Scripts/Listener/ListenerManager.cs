using System;
using System.Collections.Generic;


public class ListenerManager : BaseManager<ListenerManager>
{
	
	public void BroadCast(ListenType listenType, object value = null)
	{
		if (this.listeners.ContainsKey(listenType) && this.listeners[listenType] != null)
		{
			this.listeners[listenType].BroadCast(value);
		}
	}

	
	public void Register(ListenType listenType, Action<object> action)
	{
		if (!this.listeners.ContainsKey(listenType))
		{
			this.listeners.Add(listenType, new ListenerGroup());
		}
		if (this.listeners.ContainsKey(listenType))
		{
			this.listeners[listenType].Attach(action);
		}
	}

	
	public void Unregister(ListenType listenType, Action<object> action)
	{
		if (this.listeners.ContainsKey(listenType))
		{
			this.listeners[listenType].Detach(action);
		}
	}

	
	public void UnregisterAll(Action<object> action)
	{
		foreach (ListenType listenType in this.listeners.Keys)
		{
			this.Unregister(listenType, action);
		}
	}

	
	public Dictionary<ListenType, ListenerGroup> listeners = new Dictionary<ListenType, ListenerGroup>();
}
