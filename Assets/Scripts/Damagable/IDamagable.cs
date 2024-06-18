public interface IDamagable
{
    bool CanTakeDamage { get; }
    void TakeDamage(float amount);
}
