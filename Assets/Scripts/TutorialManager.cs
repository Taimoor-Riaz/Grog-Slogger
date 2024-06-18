using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class TutorialManager : MonoBehaviour
{
    public StateMachine<TutorialManager> StateMachine { get; private set; }

    [field: SerializeField] public TutorialInstructions TutorialInstructions { get; private set; }
    [field: SerializeField] public CameraController CameraController { get; private set; }
    [field: SerializeField] public CostumersManager CostumersManager { get; private set; }
    [field: SerializeField] public TutorialCamera TutorialCamera { get; private set; }
    [field: SerializeField] public Transform CupFillerCameraTarget { get; private set; }
    [field: SerializeField] public Transform CupFillerStandTarget { get; private set; }
    [field: SerializeField] public CupFiller CupFiller { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public EnemiesManager EnemiesManager { get; private set; }

    [field: SerializeField] public Transform CostumerCameraPosition { get; private set; }
    [field: SerializeField] public Transform TableStandingPosition { get; private set; }
    [field: SerializeField] public GameObject TableStandingVisualizer { get; private set; }

    public bool IsTutorialPassed
    {
        get
        {
            return PlayerPrefs.GetInt("TutorialPassed", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("TutorialPassed", value ? 1 : 0);
        }
    }

    void Awake()
    {
        if (IsTutorialPassed)
        {
            Disable();
            return;
        }

        StateMachine = new StateMachine<TutorialManager>();
        StateMachine.AddState(typeof(TutorialMoveToFillCupState), new TutorialMoveToFillCupState(StateMachine, this));
        StateMachine.AddState(typeof(WaitBeforeTurorialState), new WaitBeforeTurorialState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialFillCupState), new TutorialFillCupState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialWaitBeforeCostumerState), new TutorialWaitBeforeCostumerState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialMoveTowardsCostumerState), new TutorialMoveTowardsCostumerState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialServeMugState), new TutorialServeMugState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialWaitForWaveState), new TutorialWaitForWaveState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialSwitchToShootState), new TutorialSwitchToShootState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialWaitForWaveEndState), new TutorialWaitForWaveEndState(StateMachine, this));
        StateMachine.AddState(typeof(TutorialSwitchToNormalCharacterState), new TutorialSwitchToNormalCharacterState(StateMachine, this));

    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (IsTutorialPassed)
        {
            Disable();
            return;
        }
        else
        {
            StateMachine.ChangeState(typeof(WaitBeforeTurorialState));
        }
    }
    void Update()
    {
        StateMachine.Tick();
    }
}
public class WaitBeforeTurorialState : State<TutorialManager>
{
    float passedTime = 0f;
    public WaitBeforeTurorialState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.Player.CanChangeController = false;
        Owner.Player.CanInteract = false;
        Owner.Player.CanMove = false;

        Owner.CostumersManager.Pause();
        Owner.TutorialInstructions.Disable();
        passedTime = 0f;
    }
    public override void OnExit()
    {
        Owner.Player.CanMove = true;
    }
    public override void OnUpdate()
    {
        passedTime += Time.deltaTime;
        if (passedTime >= 3f)
        {
            StateMachine.ChangeState(typeof(TutorialMoveToFillCupState));
        }
    }
}
public class TutorialMoveToFillCupState : State<TutorialManager>
{
    public TutorialMoveToFillCupState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.Player.CanMove = true;
        Owner.Player.CanInteract = false;
        Owner.TutorialInstructions.SetText("Use WSAD keys and Move Towards The Cup Filler");
        Owner.TutorialCamera.MoveTowards(Owner.CameraController.Camera.transform, Owner.CupFillerCameraTarget, 1f, 1f);
    }
    public override void OnExit()
    {
        Owner.Player.CanMove = false;
        Owner.TutorialInstructions.Disable();
    }
    public override void OnUpdate()
    {
        if (Vector3.Distance(Owner.Player.transform.position, Owner.CupFillerStandTarget.position) <= 1.0f)
        {
            Owner.Player.transform.position = Owner.CupFillerStandTarget.position;
            Owner.Player.transform.rotation = Owner.CupFillerStandTarget.rotation;

            StateMachine.ChangeState(typeof(TutorialFillCupState));
        }
    }
}
public class TutorialFillCupState : State<TutorialManager>
{
    public TutorialFillCupState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.TutorialInstructions.SetText("Hold E to Fill the Cup");
        Owner.Player.CanInteract = true;
        Owner.Player.CanMove = false;
    }
    public override void OnExit()
    {
        Owner.Player.CanMove = true;
        Owner.Player.CanInteract = false;
        Owner.TutorialInstructions.Disable();
    }
    public override void OnUpdate()
    {
        if (Owner.Player.Mug.IsFilled)
        {
            StateMachine.ChangeState(typeof(TutorialWaitBeforeCostumerState));
        }
    }
}
public class TutorialWaitBeforeCostumerState : State<TutorialManager>
{
    float timer = 0f;
    public TutorialWaitBeforeCostumerState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.CostumersManager.SpawnCostumer().SetUnlimitedWaitingTime();
        Owner.Player.CanMove = true;
        timer = 0f;
    }
    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= 2f)
        {
            StateMachine.ChangeState(typeof(TutorialMoveTowardsCostumerState));
        }
    }
}
public class TutorialMoveTowardsCostumerState : State<TutorialManager>
{
    public TutorialMoveTowardsCostumerState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.Player.CanMove = true;
        Owner.TutorialCamera.MoveTowards(Owner.CameraController.Camera.transform, Owner.CostumerCameraPosition, 0.5f, 1f);
        Owner.TutorialInstructions.SetText("Here comes your first costumer, go and serve him");
        Owner.TableStandingVisualizer.SetActive(true);
    }
    public override void OnExit()
    {
        Owner.Player.CanMove = false;
        Owner.Player.CanInteract = true;
        Owner.TutorialInstructions.Disable();
        Owner.TableStandingVisualizer.SetActive(false);
    }
    public override void OnUpdate()
    {
        if (Vector3.Distance(Owner.Player.transform.position, Owner.TableStandingPosition.position) <= 1.0f)
        {
            Owner.Player.transform.position = Owner.TableStandingPosition.position;
            Owner.Player.transform.rotation = Owner.TableStandingPosition.rotation;

            StateMachine.ChangeState(typeof(TutorialServeMugState));
        }
    }
}
public class TutorialServeMugState : State<TutorialManager>
{
    public TutorialServeMugState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.TutorialInstructions.SetText("Press E to Serve the mug to costumer.");
    }
    public override void OnExit()
    {
        Owner.Player.CanMove = true;
    }
    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Owner.CostumersManager.Resume();
            Owner.TutorialInstructions.Disable();

            StateMachine.ChangeState(typeof(TutorialWaitForWaveState));
        }
    }
}
public class TutorialWaitForWaveState : State<TutorialManager>
{
    public TutorialWaitForWaveState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.EnemiesManager.OnWaveStart += EnemiesManager_OnWaveStart;
    }
    public override void OnExit()
    {
        Owner.EnemiesManager.OnWaveStart -= EnemiesManager_OnWaveStart;
    }
    private void EnemiesManager_OnWaveStart()
    {
        StateMachine.ChangeState(typeof(TutorialSwitchToShootState));
    }
}
public class TutorialSwitchToShootState : State<TutorialManager>
{
    public TutorialSwitchToShootState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.TutorialInstructions.SetText("Sometimes Robots Try to take over your Cafe. Press C to switch to shooting mode.");
        Owner.Player.CanChangeController = true;
    }
    public override void OnExit()
    {
        Owner.TutorialInstructions.Disable();
    }
    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StateMachine.ChangeState(typeof(TutorialWaitForWaveEndState));
        }
    }
}
public class TutorialWaitForWaveEndState : State<TutorialManager>
{
    public TutorialWaitForWaveEndState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.Player.CanChangeController = false;
        Owner.EnemiesManager.OnWaveCompleted += EnemiesManager_OnWaveCompleted;
    }
    public override void OnExit()
    {
        Owner.EnemiesManager.OnWaveCompleted -= EnemiesManager_OnWaveCompleted;
    }
    private void EnemiesManager_OnWaveCompleted()
    {
        StateMachine.ChangeState(typeof(TutorialSwitchToNormalCharacterState));
    }
}
public class TutorialSwitchToNormalCharacterState : State<TutorialManager>
{
    public TutorialSwitchToNormalCharacterState(StateMachine<TutorialManager> stateMachine, TutorialManager owner) : base(stateMachine, owner)
    {
    }
    public override void OnEnter()
    {
        Owner.Player.CanChangeController = true;
        Owner.TutorialInstructions.SetText("When  you are done shooting, Press C to switch to serving mode again.");
    }
    public override void OnExit()
    {
        Owner.TutorialInstructions.Disable();
    }
    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StateMachine.StopStateMachine();
            Owner.IsTutorialPassed = true;
            Owner.enabled = false;
            Owner.Disable();
        }
    }
}