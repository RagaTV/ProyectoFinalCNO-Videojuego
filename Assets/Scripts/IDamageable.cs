// Dice que cualquier script que lo use DEBE tener estas dos funciones.
public interface IDamageable
{
    void TakeDamage(float damageToTake);
    void TakeDamage(float damageToTake, bool shouldKnockBack);
}