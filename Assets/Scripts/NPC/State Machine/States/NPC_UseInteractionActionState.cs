using System.Collections;
using UnityEngine;

public class NPC_UseInteractionActionState : NPCBaseState
{
    public NPC_UseInteractionActionState(NPCStateMachine context) : base(context)
    {
    }

    bool _timerActive;
    private float _interactionTimer;
    Coroutine _interactionCoroutine;

    // caching all interaction variables
    InteractionAction _interactionAction;
    InteractionAction.InteractionSlot _interactionSlot;

    public override void EnterState(NPCStateMachine state)
    {
        _interactionAction = state.Interaction.currentInteractionObject;
        _interactionSlot = state.Interaction.CurrentSlot;

        if (_interactionCoroutine == null)
            _interactionCoroutine = state.StartCoroutine(InteractionIntializeRoutine(state));
    }

    public override void ExitState(NPCStateMachine state)
    {
        _interactionTimer = 0;
        _timerActive = false;
    }

    public override void OnCollisionEnter(NPCStateMachine state, Collision collide)
    {
        
    }

    public override void OnTriggerEnter(NPCStateMachine state, Collider collide)
    {
       
    }

    public override void UpdateState(NPCStateMachine state)
    {
        if (Mathf.Abs(_npc.Agent.velocity.sqrMagnitude) > 0.05f)
        {
            ExitInteractionToState(state.movingState);
            return;
        }

        if (!_interactionAction)
            return;

        if (!_timerActive)
            return;

         _interactionTimer -= Time.deltaTime;

        if (_interactionTimer <= 0f && _timerActive)
        {
            ExitInteractionToState(state.idleState);
        }

    }

    private void ExitInteractionToState(NPCBaseState nextState)
    {
        _npc.StartCoroutine(_npc.RunCoroutineBeforeNextState(InteractionExitRoutine(_npc), nextState));
    }
    private IEnumerator InteractionIntializeRoutine(NPCStateMachine state)
    {
        // intializing 
        state.StopNPC(false);
        state.Obstacle.enabled = true;
        state.OrientToPosition(_interactionSlot.interactionMarker);
        state.Interaction.isAtInteractionMarker = true;

        //checking if npc needs to be seated before hand
        if (_interactionSlot.requiredSeated)
        {
            state.Animator.SetTrigger("SetSeated");
            yield return new WaitForSeconds(2f);
        }

        //can call any interaction state
        _interactionAction.OnInteractionStart(state.Interaction);

        _interactionTimer = _interactionAction.interactionDuration;
        _timerActive = true;
    }
    private IEnumerator InteractionExitRoutine(NPCStateMachine state)
    {
        // Stop the interaction timer
        _timerActive = false;

        // Stop any interaction steup
        if (_interactionCoroutine != null)
        {
            state.StopCoroutine(_interactionCoroutine);
            _interactionCoroutine = null;
        }

        // if seated stand up!
        if (_interactionSlot.requiredSeated)
        {
             state.Animator.SetTrigger("SetStandUp");
             yield return new WaitForSeconds(2f);
        }

        // NOW release the interaction action (SmartObject)
        if (_interactionAction != null)
        {
           _interactionAction.OnInteractionEnd(state.Interaction);
        }
    }

}
