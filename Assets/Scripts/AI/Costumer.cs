using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class Costumer : MonoBehaviour
{
    [field: Header("References")]
    [field: SerializeField] public ProgressBar DrinkingProgress { get; private set; }
    [field: SerializeField] public ProgressBar WaitingProgress { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private Coin coinPrefab;

    [field: Header("Setup")]
    [field: SerializeField] public float TableWaitTime { get; private set; } = 20f;
    [field: SerializeField] public float DrinkingTime { get; private set; } = 15f;

    private Coin spawnedCoin;

    public Chair TargetTable { get; private set; }
    public Vector3 FinalPosition { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public StateMachine<Costumer> StateMachine { get; private set; }

    public event Action OnCostumerReturned;

    public void Initialize(Chair targetTable, Vector3 finalPosition)
    {
        TargetTable = targetTable;
        FinalPosition = finalPosition;

        DrinkingProgress.Deactivate();
        WaitingProgress.Deactivate();

        TargetTable.Acquire();

        NavMeshAgent = GetComponent<NavMeshAgent>();

        StateMachine = new StateMachine<Costumer>();
        StateMachine.AddState(typeof(CostumerWaitingForServingState), new CostumerWaitingForServingState(StateMachine, this));
        StateMachine.AddState(typeof(CostumerGoingToTableState), new CostumerGoingToTableState(StateMachine, this));
        StateMachine.AddState(typeof(CostumerGoingBackState), new CostumerGoingBackState(StateMachine, this));
        StateMachine.AddState(typeof(CostumerDrinkingState), new CostumerDrinkingState(StateMachine, this));
        StateMachine.AddState(typeof(CostumerDuringWaveState), new CostumerDuringWaveState(StateMachine, this));

        StateMachine.ChangeState(typeof(CostumerGoingToTableState));
    }
    void Update()
    {
        StateMachine.Tick();
    }
    public void OnReturned()
    {
        OnCostumerReturned?.Invoke();
        Destroy(gameObject);
    }

    public void OnGotServed()
    {
        if (spawnedCoin == null)
        {
            spawnedCoin = Instantiate(coinPrefab, TargetTable.TipCoinsPosition, Quaternion.identity);
            spawnedCoin.SetAmount(1);
        }
    }
    public void SetUnlimitedWaitingTime()
    {
        TableWaitTime = float.MaxValue;
    }
}
public class CostumerGoingToTableState : State<Costumer>
{
    public CostumerGoingToTableState(StateMachine<Costumer> stateMachine, Costumer owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.NavMeshAgent.SetDestination(Owner.TargetTable.CostumerSitPosition);
    }
    public override void OnUpdate()
    {
        if (Owner.NavMeshAgent.HasReachedDestination())
        {
            StateMachine.ChangeState(typeof(CostumerWaitingForServingState));
        }
    }
}
public class CostumerWaitingForServingState : State<Costumer>
{
    private float timePassed;
    public CostumerWaitingForServingState(StateMachine<Costumer> stateMachine, Costumer owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.WaitingProgress.Activate();

        Vector3 adjustedLookPosition = Owner.TargetTable.CostumerSitPosition;
        adjustedLookPosition.y = Owner.NavMeshAgent.transform.position.y;
        Owner.NavMeshAgent.transform.LookAt(adjustedLookPosition);
    }
    public override void OnExit()
    {
        Owner.WaitingProgress.Deactivate();
    }
    public override void OnUpdate()
    {
        timePassed += Time.deltaTime;
        Owner.WaitingProgress.SetProgress(timePassed, Owner.TableWaitTime);

        if (timePassed >= Owner.TableWaitTime)
        {
            StateMachine.ChangeState(typeof(CostumerGoingBackState));
        }
        if (Owner.TargetTable.HasMug)
        {
            StateMachine.ChangeState(typeof(CostumerDrinkingState));
        }
    }
}
public class CostumerDrinkingState : State<Costumer>
{
    private float timePassed;
    public CostumerDrinkingState(StateMachine<Costumer> stateMachine, Costumer owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.DrinkingProgress.Activate();
    }
    public override void OnExit()
    {
        Owner.DrinkingProgress.Deactivate();
    }
    public override void OnUpdate()
    {
        timePassed += Time.deltaTime;
        Owner.DrinkingProgress.SetProgress(timePassed, Owner.DrinkingTime);

        if (timePassed >= Owner.DrinkingTime)
        {
            Owner.TargetTable.ServeMug();
            Owner.OnGotServed();

            StateMachine.ChangeState(typeof(CostumerGoingBackState));
        }
    }
}
public class CostumerGoingBackState : State<Costumer>
{
    public CostumerGoingBackState(StateMachine<Costumer> stateMachine, Costumer owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.NavMeshAgent.SetDestination(Owner.FinalPosition);
        Owner.TargetTable.Release();
    }
    public override void OnUpdate()
    {
        if (Owner.NavMeshAgent.HasReachedDestination())
        {
            Owner.OnReturned();
        }
    }
}
public class CostumerDuringWaveState : State<Costumer>
{
    private Vector3 position;
    public CostumerDuringWaveState(StateMachine<Costumer> stateMachine, Costumer owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        position = Shelter.Instance.GetPosition();
        Owner.NavMeshAgent.SetDestination(position);
    }
}