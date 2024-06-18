using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rbody;
    private float damage;
    private float launchTime;
    public void Launch(Vector3 target, float damage)
    {
        this.damage = damage;
        launchTime = Time.time;

        Vector3 direction = (target - transform.position).normalized;
        rbody.velocity = direction * speed;
    }
    void Update()
    {
        if (Time.time >= launchTime + 10f)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null)
        {
            Destroy(gameObject);
        }

        Player player = collision.collider.GetComponent<Player>();

        if (player == null)
        {
            Destroy(gameObject);
        }
        else
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
