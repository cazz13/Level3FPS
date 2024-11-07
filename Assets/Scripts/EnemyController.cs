using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private int currentLife;
    [SerializeField] private int maxtLife;
    [SerializeField] private int enemyScorePoint;

    /// <summary>
    /// Handle when the enemy receive a bullet
    /// </summary>
    /// <param name="quantity"></param>
    public void  DamageEnemy(int quantity)
    {
        currentLife -= quantity;
        if(currentLife <= 0)
        {
            Destroy(gameObject);
        }
    }
}
