using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachine : MonoBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent { get { return _agent; } set { _agent = value; } }

    private Animator _npcAnimator;
    public Animator Animator { get { return _npcAnimator; } set { _npcAnimator = value; } }

    public NPCBaseState currentState;
    public NPC_IdlingState idleState;
    public NPC_MovingState movingState;

    private AnimatorOverrideController overrideController;
    public AnimatorOverrideController AnimatorOverrideController { get { return overrideController; } set {  overrideController = value; } }

    private NPCRoutine routine;
    public NPCRoutine Routine { get { return routine; } }

    [Header("Tweakable Variables")]
    public float MinSpeed;
    public float MaxSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        _npcAnimator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        routine = GetComponent<NPCRoutine>();

        IntializeStates();
        currentState.EnterState(this);
    }

    // Update is called once per frame
    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void IntializeStates()
    {
        idleState = new NPC_IdlingState(this);
        movingState = new NPC_MovingState(this);

        currentState = idleState;
        currentState.IntializeState(currentState);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(this, other);
    }
    public void OnStateSwitch(NPCBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        state.EnterState(this);
    }
    public void CallCoroutineInState(IEnumerator stateCoroutine)
    {
        StartCoroutine(stateCoroutine);
    }

    public void StopNPC()
    {
        Agent.isStopped = true;
    }

    public void ResumeNPC()
    {
        Agent.isStopped = false;
    }

    public void MoveToPosition(Vector3 transform)
    {
        ResumeNPC();
        Agent.SetDestination(transform);
    }

}

