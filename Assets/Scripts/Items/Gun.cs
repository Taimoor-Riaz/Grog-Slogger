using GAP_LaserSystem;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private Vector3 normalPosition;
    [SerializeField] private Vector3 aimedPosition;
    [SerializeField] private float aimTime;

    [Header("Stats")]
    [SerializeField] private float damagePerSecond;

    [Header("References")]
    [SerializeField] private LaserScript laser;

    private Vector3 positionOnAimChange;
    private bool isAimed = false;
    private float aimChangeTime;

    public void AimDown()
    {
        isAimed = true;
        aimChangeTime = Time.time;
        positionOnAimChange = transform.localPosition;
    }
    public void AimUp()
    {
        isAimed = false;
        aimChangeTime = Time.time;
        positionOnAimChange = transform.localPosition;
    }
    void Update()
    {
        float t = (Time.time - aimChangeTime) / aimTime;

        if (isAimed)
        {
            transform.localPosition = Vector3.Lerp(positionOnAimChange, aimedPosition, t);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(positionOnAimChange, normalPosition, t);
        }
    }
    public void MouseButtonDown()
    {
        laser.EnableLaser();
        laser.gameObject.SetActive(true);
    }
    public void MouseButtonPressed()
    {
        laser.UpdateLaser();
        Fire();
    }
    public void MouseButtonUp()
    {
        laser.DisableLaserCaller(laser.disableDelay);
    }
    private void Fire()
    {
        Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0f);
        Ray ray = Camera.main.ViewportPointToRay(viewportCenter);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }

    }
}
