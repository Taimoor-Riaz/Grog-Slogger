using UnityEngine;

[System.Serializable]
public class TopDownController : ControllerBase
{
    public float RotationSpeed;
    public float MoveSpeed;

    private float currentRotation = 90f;
    public override void Enter()
    {
        player.Mug.Deactivate();
        player.FPItems.Deactivate();
    }
    public override void Exit()
    {
        player.Mug.Deactivate();
        player.FPItems.Deactivate();
    }
    public override void Update()
    {
        if (player.Controller != null && player.CanMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            Vector3 cameraRight = Camera.main.transform.right;

            Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

            if (moveDirection.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

                float angle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetAngle, ref currentRotation, 1 / RotationSpeed);
                player.transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Apply movement with gravity
                Vector3 gravity = Vector3.zero;

                if (!player.Controller.isGrounded)
                {
                    gravity.y += Physics.gravity.y * Time.deltaTime;
                }

                player.Controller.Move(moveDirection * MoveSpeed * Time.deltaTime + gravity);
            }
        }
    }
}
