using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, IDamagable
{
    [field: SerializeField] public List<Transform> Waypoints { get; private set; }
    [field: SerializeField] public Transform RobotPosition { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float DropSpeed { get; private set; }
    [field: SerializeField] public Robot Robot { get; private set; }

    public StateMachine<Drone> StateMachine { get; private set; }

    public float Health { get; private set; }
    public bool CanTakeDamage => Health > 0;

    public void TakeDamage(float amount)
    {
        if (CanTakeDamage)
        {
            Health -= amount;
            Health = Mathf.Max(0, Health);
            if (Health <= 0)
            {
                Robot.InvokeOnDeathEvent();
                Destroy(gameObject);
            }
        }
    }

    void Awake()
    {
        Health = 50f;

        StateMachine = new StateMachine<Drone>();
        StateMachine.AddState(typeof(DroneMoveState), new DroneMoveState(StateMachine, this));
        StateMachine.AddState(typeof(DroneDroppingRobotState), new DroneDroppingRobotState(StateMachine, this));
        StateMachine.ChangeState(typeof(DroneMoveState));
    }
    private void Update()
    {
        StateMachine.Tick();
    }
}
public class DroneMoveState : State<Drone>
{
    private int currentWaypointIndex;
    public DroneMoveState(StateMachine<Drone> stateMachine, Drone owner) : base(stateMachine, owner)
    {
    }
    public override void OnUpdate()
    {
        if (Vector3.Distance(Owner.transform.position, Owner.Waypoints[currentWaypointIndex].position) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= Owner.Waypoints.Count)
            {
                OnPathFinished();
                return;
            }
        }
        Owner.transform.position =
            Vector3.MoveTowards(
                Owner.transform.position, Owner.Waypoints[currentWaypointIndex].position, Owner.MoveSpeed * Time.deltaTime);

    }

    private void OnPathFinished()
    {
        StateMachine.ChangeState(typeof(DroneDroppingRobotState));
    }
}
public class DroneDroppingRobotState : State<Drone>
{
    public DroneDroppingRobotState(StateMachine<Drone> stateMachine, Drone owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
    }
    public override void OnUpdate()
    {
        Owner.Robot.transform.position = Vector3.MoveTowards(Owner.Robot.transform.position, Owner.RobotPosition.position, Owner.DropSpeed * Time.deltaTime);
        if (Vector3.Distance(Owner.Robot.transform.position, Owner.RobotPosition.position) < 0.1f)
        {
            Owner.Robot.transform.parent = null;
            Owner.Robot.Activate();

            GameObject.Destroy(Owner.gameObject);
        }
    }
}