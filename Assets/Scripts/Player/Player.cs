using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionHeight = 1f;
    [SerializeField] private float interactionDistance = 1.5f;

    [Header("Collection")]
    [SerializeField] private Vector3 collectOffset = new Vector3();
    [SerializeField] private float collectRadius = 1f;

    [field: Header("State")]
    [field: SerializeField] public ControllerState CurrentState { get; private set; }

    [field: Header("Controllers")]
    [field: SerializeField] public FirstPersonController FirstPerson { get; private set; }
    [field: SerializeField] public TopDownController TopDown { get; private set; }

    [field: Header("References")]
    [field: SerializeField] public Mug Mug { get; private set; }
    [field: SerializeField] public FPItems FPItems { get; private set; }
    [field: SerializeField] public ProgressBar ProgressBar { get; private set; }
    [field: SerializeField] public DamageVisualizer damageVisualizer { get; private set; }

    [field: Header("Actions")]
    [field: SerializeField] public bool CanChangeController { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; }
    [field: SerializeField] public bool CanMove { get; set; }


    public float Health { get; private set; }
    public CharacterController Controller { get; private set; }
    public BaseInteractable CurrentInteractable { get; private set; }

    public event EventHandler<OnInteractableChangedEventArgs> OnSelectedInteractableChanged;
    public event EventHandler<OnControllerStateChangedEventArgs> OnControllerStateChanged;

    public event EventHandler<OnCoinCollectedEventArgs> OnCoinCollected;
    public event Action OnPlayerHealthChanged;
    public event Action OnPlayerDeath;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        ProgressBar.Deactivate();

        Health = 100f;
        CanMove = true;
        CanInteract = true;
        CanChangeController = true;

        FirstPerson.Init(this);
        TopDown.Init(this);
    }

    private void Start()
    {
        ChangeControllerState(ControllerState.TopDown);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && CanChangeController)
        {
            ChangeControllerState(CurrentState == ControllerState.FirstPerson ? ControllerState.TopDown : ControllerState.FirstPerson);
        }

        if (CurrentState == ControllerState.TopDown)
        {
            TopDown.Update();
            HandleInteraction();
        }
        else if (CurrentState == ControllerState.FirstPerson)
        {
            FirstPerson.Update();
        }

        CheckForCollect();
    }
    public void TakeDamage(float amount)
    {
        if (Health > 0)
        {
            Health -= amount;
            Health = Mathf.Clamp(Health, 0, 100);
            damageVisualizer.VisualizeDamage();
            OnPlayerHealthChanged?.Invoke();

            if (Health <= 0)
            {
                Debug.Log("On Player Death Event Invoked");
                OnPlayerDeath?.Invoke();
            }
        }
    }
    public void Revive()
    {
        Health = 100f;
        Debug.Log("Player Revived");
        OnPlayerHealthChanged?.Invoke();
    }
    private void CheckForCollect()
    {
        Collider[] overlapingColliders = Physics.OverlapSphere(transform.position + collectOffset, collectRadius);
        if (overlapingColliders.Length > 0)
        {
            for (int i = 0; i < overlapingColliders.Length; i++)
            {
                Coin coin = overlapingColliders[i].GetComponent<Coin>();
                if (coin != null)
                {
                    OnCoinCollected?.Invoke(this, new OnCoinCollectedEventArgs(coin));
                    Destroy(coin.gameObject);
                }
            }
        }
    }

    private void HandleInteraction()
    {
        // Raycast in front of player
        RaycastHit hit;

        Ray ray = new Ray(transform.position + new Vector3(0, interactionHeight, 0), transform.forward);
        Debug.DrawRay(transform.position + new Vector3(0, interactionHeight, 0), transform.forward);

        if (Physics.Raycast(ray, out hit, interactionDistance) && CanInteract)
        {
            BaseInteractable interactableInFront = hit.collider.GetComponent<BaseInteractable>();
            if (interactableInFront != null)
            {
                if (interactableInFront == CurrentInteractable)
                {
                    CurrentInteractable.OnStay(this);
                    // Keep looking at the same counter
                    // Nothing to be done here
                }
                else
                {
                    ChangeSelectedInteractable(interactableInFront);
                }
            }
            else
            {
                ChangeSelectedInteractable(null);
            }
        }
        else
        {
            ChangeSelectedInteractable(null);
        }

        if (CurrentInteractable != null && CanInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CurrentInteractable.OnInteractButtonDown(this);
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                CurrentInteractable.OnInteractButtonUp(this);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                CurrentInteractable.OnInteractButton(this);
            }
        }
    }
    private void ChangeSelectedInteractable(BaseInteractable newInteractable)
    {
        CurrentInteractable?.OnExit(this);
        CurrentInteractable = newInteractable;

        CurrentInteractable?.OnEnter(this);

        OnSelectedInteractableChanged?.Invoke(this, new OnInteractableChangedEventArgs(CurrentInteractable));
    }
    public void ChangeControllerState(ControllerState newState)
    {
        if (CurrentState == newState) return;

        if (CurrentState == ControllerState.FirstPerson)
        {
            FirstPerson.Exit();
        }
        else if (CurrentState == ControllerState.TopDown)
        {
            ChangeSelectedInteractable(null);
            TopDown.Exit();
        }

        CurrentState = newState;
        if (CurrentState == ControllerState.FirstPerson)
        {
            FirstPerson.Enter();
        }
        else if (CurrentState == ControllerState.TopDown)
        {
            TopDown.Enter();
        }

        OnControllerStateChanged?.Invoke(this, new OnControllerStateChangedEventArgs(CurrentState));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + collectOffset, collectRadius);
    }
}

public enum ControllerState
{
    FirstPerson,
    TopDown
}