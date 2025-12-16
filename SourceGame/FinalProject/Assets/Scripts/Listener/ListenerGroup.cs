using System;
using System.Collections.Generic;


public class ListenerGroup
{
	
	public void BroadCast(object value)
	{
		foreach (Action<object> action in this.actions)
		{
			action(value);
		}
	}

	
	public void Attach(Action<object> action)
	{
		using (List<Action<object>>.Enumerator enumerator = this.actions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Equals(action))
				{
					return;
				}
			}
		}
		this.actions.Add(action);
	}

	
	public void Detach(Action<object> action)
	{
		using (List<Action<object>>.Enumerator enumerator = this.actions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Equals(action))
				{
					this.actions.Remove(action);
					break;
				}
			}
		}
	}

	
	private List<Action<object>> actions = new List<Action<object>>();
}
