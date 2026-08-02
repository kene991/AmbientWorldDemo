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

    [Header("NPC Object Ref")]
    [SerializeField] private GameObject npcModel;
    public GameObject NPCModel { get { return npcModel; } set { npcModel = value; } }
    //public AnimationClip[] idleAnimations;

    [Header("Traversal Settings Variables")]
    public float MaxSpeed;
    public PathNode currentPathNode;

    private void Awake()
    {
        npcModel = transform.GetChild(0).gameObject;
        _npcAnimator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        routine = GetComponent<NPCRoutine>();
    }

    private void Start()
    {
        IntializeStates();
        currentState.EnterState(this);
    }

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

    public bool HasReachedDestination()
    {
        if (_agent.pathPending)
            return false;

        if (_agent.remainingDistance > Agent.stoppingDistance)
            return false;

        if (_agent.hasPath && Agent.velocity.sqrMagnitude > 0.01f)
            return false;

        return true;
    }

    #region Free Roam Nodes
    public PathNode GetClosestNode()
    {
        PathNode[] pathNodes = FindObjectsByType<PathNode>(FindObjectsSortMode.None);

        PathNode closestNode = null;
        float closestDistance = Mathf.Infinity;

        foreach (PathNode node in pathNodes)
        {
            float distance = Vector3.SqrMagnitude(transform.position - node.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    public PathNode SetNextNode()
    {
        if (currentPathNode == null)
            return null;

        print("next");

        currentPathNode = ChooseNextNode(currentPathNode);
        return currentPathNode;
    }

    public void UpdateNodePath()
    {
        if (currentPathNode == null)
        {
            currentPathNode = GetClosestNode();
            MoveToPosition(currentPathNode.transform.position);
            return;
        }

        currentPathNode = SetNextNode();
        MoveToPosition(currentPathNode.transform.position);
    }

    private PathNode ChooseNextNode(PathNode node)
    {
        if (node.waypointBranches.Count > 0 && Random.value <= node.branchRatio)
        {
            return node.waypointBranches[Random.Range(0, node.waypointBranches.Count)];
        }

        print("next again");
        return node.nextWaypoint;
    }
    #endregion

    #region Animations

    public void ReplaceAnimationClip(AnimationClip animationClip, string overrideClip)
    {
        AnimatorOverrideController animatorOverride = new(_npcAnimator.runtimeAnimatorController);
        _npcAnimator.runtimeAnimatorController = animatorOverride;

        animatorOverride[overrideClip] = animationClip;
    }
    public void RandomizeReplaceAnimationClips(AnimationClip[] animationClips, string overrideClip)
    {
        AnimatorOverrideController animatorOverride = new(_npcAnimator.runtimeAnimatorController);
        _npcAnimator.runtimeAnimatorController = animatorOverride;

        animatorOverride[overrideClip] = animationClips[Random.Range(0, animationClips.Length)];
    }
    #endregion

}