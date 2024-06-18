using UnityEngine;

[System.Serializable]
public class FirstPersonController : ControllerBase
{
    public float MoveSpeed;
    public float RotationSpeed;

    public override void Enter()
    {
        player.Mug.Deactivate();
        player.FPItems.Activate();
    }
    public override void Exit()
    {
        player.Mug.Deactivate();
        player.FPItems.Deactivate();
    }
    public override void Update()
    {
        if (player.CanMove)
        {
            // Rotation
            float mouseX = Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime;
            player.transform.Rotate(Vector3.up * mouseX);

            // Movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = player.transform.right * x + player.transform.forward * z;

            Vector3 gravity = Vector3.zero;
            if (!player.Controller.isGrounded)
            {
                gravity.y += Physics.gravity.y * Time.deltaTime;
            }

            player.Controller.Move(move * MoveSpeed * Time.deltaTime + gravity);
        }

        if (Input.GetMouseButtonDown(0))
        {
            player.FPItems.Gun.MouseButtonDown();
        }
        if (Input.GetMouseButton(0))
        {
            player.FPItems.Gun.MouseButtonPressed();
        }
        if (Input.GetMouseButtonUp(0))
        {
            player.FPItems.Gun.MouseButtonUp();
        }

        if (Input.GetMouseButtonDown(1))
        {
            player.FPItems.Gun.AimDown();
        }
        if (Input.GetMouseButtonUp(1))
        {
            player.FPItems.Gun.AimUp();
        }
    }
}
