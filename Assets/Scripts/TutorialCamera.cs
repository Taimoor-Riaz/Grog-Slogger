using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TutorialCamera : MonoBehaviour
{
    public Transform TargetPosition { get; private set; }
    public Transform OriginalPosition { get; private set; }
    public float MoveDuration { get; private set; }
    public float StayTime { get; private set; }
    public float ElapsedTime { get; private set; }
    public Camera Camera { get; private set; }

    private bool isMovingTowardsTarget = false;
    private bool isAtTargetPosition = false;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    public void MoveTowards(Transform originalCamera, Transform finalPosition, float moveTime, float stayTime)
    {
        OriginalPosition = originalCamera;
        TargetPosition = finalPosition;
        MoveDuration = moveTime;
        StayTime = stayTime;
        ElapsedTime = 0f;
        isMovingTowardsTarget = true;
        isAtTargetPosition = false;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isMovingTowardsTarget && !isAtTargetPosition)
        {
            ElapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(ElapsedTime / MoveDuration);
            transform.position = Vector3.Lerp(OriginalPosition.position, TargetPosition.position, t);
            transform.rotation = Quaternion.Lerp(OriginalPosition.rotation, TargetPosition.rotation, t);

            if (t >= 1f)
            {
                isAtTargetPosition = true;
                ElapsedTime = 0f;
            }
        }
        else if (isAtTargetPosition)
        {
            ElapsedTime += Time.deltaTime;
            if (ElapsedTime >= StayTime)
            {
                float t = Mathf.Clamp01((ElapsedTime - StayTime) / MoveDuration);
                transform.position = Vector3.Lerp(TargetPosition.position, OriginalPosition.position, t);
                transform.rotation = Quaternion.Lerp(TargetPosition.rotation, OriginalPosition.rotation, t);

                if (t >= 1f)
                {
                    isMovingTowardsTarget = false;
                    isAtTargetPosition = false;

                    gameObject.SetActive(false);
                }
            }
        }
    }
}
