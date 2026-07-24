using UnityEngine;

public abstract class NPCBaseState 
{
    //Protecting our reference to our player state machine
    protected NPCStateMachine _npc { get; private set; }

    //Getting/setting our player state machine to allow use to update our states while maintaining the context of the values
    public NPCBaseState(NPCStateMachine context)
    {
        _npc = context;
    }

    public virtual void IntializeState(NPCBaseState state)
    {
        _npc.currentState = state;
        _npc.currentState.EnterState(_npc);
    }

    public abstract void EnterState(NPCStateMachine state);

    public abstract void UpdateState(NPCStateMachine state);

    public abstract void ExitState(NPCStateMachine state);

    public abstract void OnCollisionEnter(NPCStateMachine state, Collision collide);

    public abstract void OnTriggerEnter(NPCStateMachine state, Collider collide);
}
