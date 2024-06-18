using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private FirstPersonCameraData firstPerson;
    [SerializeField] private TopDownCameraData topDown;
    [SerializeField] private Player player;

    private ControllerState currentState = ControllerState.TopDown;
    public Camera Camera { get; private set; }

    private void Awake()
    {
        Camera = GetComponentInChildren<Camera>();
    }
    private void OnEnable()
    {
        player.OnControllerStateChanged += Player_OnControllerStateChanged;
    }

    private void Player_OnControllerStateChanged(object sender, OnControllerStateChangedEventArgs e)
    {
        currentState = e.controllerState;
    }

    void Update()
    {
        if (currentState == ControllerState.TopDown)
        {
            topDown.Update(player, this, Camera);
        }
        else if (currentState == ControllerState.FirstPerson)
        {
            firstPerson.Update(player, this, Camera);
        }
    }
    void LateUpdate()
    {
        if (currentState == ControllerState.TopDown)
        {
            topDown.LateUpdate(player, this, Camera);
        }
        else if (currentState == ControllerState.FirstPerson)
        {
            firstPerson.LateUpdate(player, this, Camera);
        }
    }
}
[System.Serializable]
public class FirstPersonCameraData
{
    public Vector3 TargetOffset;
    public Vector2 MinMaxRotation;
    public float RotationSpeed = 3f;

    private float xRotation;

    public void Update(Player player, CameraController controller, Camera camera)
    {
        controller.transform.rotation = player.transform.rotation;

        float mouseY = Input.GetAxis("Mouse Y") * RotationSpeed * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, MinMaxRotation.x, MinMaxRotation.y);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        camera.transform.localPosition = Vector3.zero;
    }
    public void LateUpdate(Player player, CameraController controller, Camera camera)
    {
        controller.transform.position = player.transform.position + TargetOffset;
    }
}
[System.Serializable]
public class TopDownCameraData
{
    public Vector3 ControllerPosition;
    public Vector3 ControllerRotation;

    public Vector3 CameraPosition;
    public Vector3 CameraRotation;

    public void Update(Player player, CameraController controller, Camera camera)
    {
        controller.transform.position = ControllerPosition;
        controller.transform.rotation = Quaternion.Euler(ControllerRotation);

        camera.transform.localPosition = CameraPosition;
        camera.transform.localRotation = Quaternion.Euler(CameraRotation);
    }
    public void LateUpdate(Player player, CameraController controller, Camera camera)
    {
    }
}
