using UnityEngine;

public interface IEnemy
{
    EnemySO EnemyData { get; set; }

    KillMoveData KillMoveData { get; set; }

    GameObject gameObject { get; }

    void TakeDamage(int damage);

    public void Die(IEnemy enemy);

    public void ActivateGauge();

    public void DeActivateHealthBar();
}
