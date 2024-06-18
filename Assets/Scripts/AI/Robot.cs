using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour, IDamagable
{
    [field: Header("Attack")]
    [field: SerializeField] public float AttackDamage { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public float ChaseRange { get; private set; }
    [field: SerializeField] public float AttackRate { get; private set; }
    [field: SerializeField] public float ChargeTime { get; private set; }
    [field: SerializeField] public Projectile bulletPrefab { get; private set; }
    [field: SerializeField] public Transform bulletSpawnPosition { get; private set; }

    [field: Header("Effects")]
    [field: SerializeField] public ParticleSystem DeathEffect { get; private set; }
    [field: SerializeField] public ParticleSystem ChargeEffect { get; private set; }
    [field: SerializeField] public ParticleSystem ShotEffect { get; private set; }

    public StateMachine<Robot> StateMachine { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Animator Animator { get; private set; }
    public Player Target { get; private set; }

    public float Health { get; private set; }

    public event Action<Robot> OnRobotDeath;

    public bool IsInAttackRange
    {
        get
        {
            return Target == null ? false : Vector3.Distance(transform.position, Target.transform.position) <= AttackRange;
        }
    }
    public bool IsInChaseRange
    {
        get
        {
            return Target == null ? false : Vector3.Distance(transform.position, Target.transform.position) <= ChaseRange;
        }
    }

    public bool CanTakeDamage => Health > 0;

    public void Activate()
    {
        Animator = GetComponentInChildren<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        GetComponent<Collider>().enabled = true;
        NavMeshAgent.enabled = true;
        Health = 100f;

        StateMachine = new StateMachine<Robot>();

        StateMachine.AddState(typeof(RobotIdleState), new RobotIdleState(StateMachine, this));
        StateMachine.AddState(typeof(RobotDeathState), new RobotDeathState(StateMachine, this));
        StateMachine.AddState(typeof(RobotChaseTargetState), new RobotChaseTargetState(StateMachine, this));
        StateMachine.AddState(typeof(RobotAttackTargetState), new RobotAttackTargetState(StateMachine, this));
        StateMachine.AddState(typeof(RobotRepositionDuringAttackState), new RobotRepositionDuringAttackState(StateMachine, this));

        StateMachine.ChangeState(typeof(RobotIdleState));
    }
    void Update()
    {
        if (Target == null)
        {
            Target = FindObjectOfType<Player>();
        }
        if (StateMachine != null)
        {
            StateMachine.Tick();
        }
    }
    public void TakeDamage(float amount)
    {
        if (CanTakeDamage)
        {
            Health -= amount;
            Health = Mathf.Clamp(Health, 0, 100);
            if (Health <= 0)
            {
                StateMachine.ChangeState(typeof(RobotDeathState));
            }
        }
    }
    public void InvokeOnDeathEvent()
    {
        OnRobotDeath?.Invoke(this);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
public class RobotIdleState : State<Robot>
{
    public RobotIdleState(StateMachine<Robot> stateMachine, Robot owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.NavMeshAgent.SetDestination(Owner.NavMeshAgent.transform.position);
        Owner.Animator.SetFloat("Speed", 0f);
    }
    public override void OnUpdate()
    {
        if (Owner.IsInChaseRange)
        {
            StateMachine.ChangeState(typeof(RobotChaseTargetState));
        }
    }
}
public class RobotChaseTargetState : State<Robot>
{
    public RobotChaseTargetState(StateMachine<Robot> stateMachine, Robot owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        if (!Owner.IsInChaseRange)
        {
            StateMachine.ChangeState(typeof(RobotIdleState));
            return;
        }
        Owner.Animator.SetFloat("Speed", 1f);
    }
    public override void OnUpdate()
    {
        if (Owner.IsInChaseRange)
        {
            Owner.NavMeshAgent.SetDestination(Owner.Target.transform.position);
        }
        if (Owner.IsInAttackRange)
        {
            StateMachine.ChangeState(typeof(RobotAttackTargetState));
        }
    }
}
public class RobotAttackTargetState : State<Robot>
{
    private float lastAttackTime;
    private float chargeStartTime;
    private float timeBetweenAttacks;

    private bool isCharging = false;
    private bool isQueuedForReposition;

    private Vector3 targetPosition;

    private bool isCharged => isCharging && (Time.time >= chargeStartTime + 2f);

    public RobotAttackTargetState(StateMachine<Robot> stateMachine, Robot owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        if (Owner.Target == null)
        {
            StateMachine.ChangeState(typeof(RobotIdleState));
        }
        else if (!Owner.IsInAttackRange)
        {
            StateMachine.ChangeState(typeof(RobotChaseTargetState));
        }
        else
        {
            Owner.NavMeshAgent.SetDestination(Owner.NavMeshAgent.transform.position);
            timeBetweenAttacks = 1f / Owner.AttackRate;
            Owner.Animator.SetFloat("Speed", 0f);
            isQueuedForReposition = false;
        }
    }
    public override void OnUpdate()
    {
        if (Owner.Target == null)
        {
            StateMachine.ChangeState(typeof(RobotIdleState));
            return;
        }
        else if (!Owner.IsInAttackRange)
        {
            StateMachine.ChangeState(typeof(RobotChaseTargetState));
            return;
        }

        if (!isCharging && !isQueuedForReposition)
        {
            Vector3 adjustedTargetPosition = new Vector3(Owner.Target.transform.position.x, Owner.transform.position.y, Owner.Target.transform.position.z);
            Owner.transform.rotation = Quaternion.LookRotation(adjustedTargetPosition - Owner.transform.position);
        }

        if (isQueuedForReposition && (Time.time >= (lastAttackTime + 1f)))
        {
            StateMachine.ChangeState(typeof(RobotRepositionDuringAttackState));
        }
        else
        {
            if (isCharged)
            {
                Attack();
            }
            else if (!isCharging && Time.time >= (lastAttackTime + timeBetweenAttacks))
            {
                Charge();
            }
        }
    }
    private void Charge()
    {
        isCharging = true;
        Owner.ChargeEffect.Play();
        chargeStartTime = Time.time;

        targetPosition = Owner.Target.Controller.center + Owner.Target.Controller.transform.position;
    }
    private void Attack()
    {
        // Owner.Target.TakeDamage(Owner.AttackDamage);

        var bullet = GameObject.Instantiate(Owner.bulletPrefab, Owner.bulletSpawnPosition.position, Quaternion.identity);
        bullet.Launch(targetPosition, Owner.AttackDamage);

        Owner.Animator.SetTrigger("Fire");
        Owner.ShotEffect.Play();
        isCharging = false;
        isQueuedForReposition = true;
        lastAttackTime = Time.time;
    }
}
public class RobotRepositionDuringAttackState : State<Robot>
{
    public RobotRepositionDuringAttackState(StateMachine<Robot> stateMachine, Robot owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Vector3 result = Vector3.zero;
        if (NavmeshUtilities.RandomPoint(Owner.Target.transform.position, Owner.AttackRange / 2, Owner.AttackRange, out result))
        {
            Owner.NavMeshAgent.SetDestination(result);
            Owner.Animator.SetFloat("Speed", 1f);
        }
        else
        {
            Owner.NavMeshAgent.SetDestination(Owner.transform.position);
        }
    }
    public override void OnUpdate()
    {
        if (Owner.NavMeshAgent.HasReachedDestination())
        {
            StateMachine.ChangeState(typeof(RobotAttackTargetState));
        }
    }
}

public class RobotDeathState : State<Robot>
{
    private float timer = 0f;
    public RobotDeathState(StateMachine<Robot> stateMachine, Robot owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.NavMeshAgent.SetDestination(Owner.NavMeshAgent.transform.position);
        Owner.Animator.SetTrigger("Death");
    }
    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 2f)
        {
            var particles = GameObject.Instantiate(Owner.DeathEffect, Owner.transform.position, Quaternion.identity);
            Owner.InvokeOnDeathEvent();

            GameObject.Destroy(Owner.gameObject);
        }
    }
}