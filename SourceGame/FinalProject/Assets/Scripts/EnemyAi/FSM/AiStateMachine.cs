using System;
using System.Diagnostics;



public class AiStateMachine
{
	
	public AiStateMachine(AiAgent agent)
	{
		this.agent = agent;
		int num = Enum.GetNames(typeof(AIStateID)).Length;
		this.states = new AIState[num];
	}

	
	public void RegisterState(AIState state)
	{
		int id = (int)state.GetID();
		this.states[id] = state;
	}

	
	public AIState GetState(AIStateID stateID)
	{
		return this.states[(int)stateID];
	}

	
	public void Update()
	{
		AIState state = this.GetState(this.currentState);
		
		if (state == null)
		{
			return;
		}
		state.Update(this.agent);
	}

	
	public void ChangeState(AIStateID newState)
	{
		AIState state = this.GetState(this.currentState);
		if (state != null)
		{
			state.Exit(this.agent);
		}
		this.currentState = newState;
		AIState state2 = this.GetState(this.currentState);
		if (state2 == null)
		{
			return;
		}
		state2.Enter(this.agent);
	}

	
	public AIState[] states;

	
	public AIStateID currentState;

	
	public AiAgent agent;
}
