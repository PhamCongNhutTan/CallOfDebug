


public interface AIState
{
	
	AIStateID GetID();

	
	void Enter(AiAgent agent);

	
	void Update(AiAgent agent);

	
	void Exit(AiAgent agent);
}
