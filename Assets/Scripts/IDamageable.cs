// Este es el "contrato"
// Dice que cualquier script que lo use DEBE tener estas dos funciones.
public interface IDamageable
{
    // Función para tomar daño simple
    void TakeDamage(float damageToTake);
    
    // Función para tomar daño con knockback
    void TakeDamage(float damageToTake, bool shouldKnockBack);
}