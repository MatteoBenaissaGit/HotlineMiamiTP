using UnityEngine;

public class EnemyMelee : Enemy
{
    [SerializeField] private float _attackRadiusRange = 1f;
    
    protected override void CheckForPlayer()
    {
        base.CheckForPlayer();

        if (Player == null || Player.IsDead)
        {
            return;
        }
        
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        Rigidbody2DProperty.velocity = direction * AttackSpeed;

        if (Vector2.Distance(transform.position, Player.transform.position) <= _attackRadiusRange)
        {
            SpriteViewProperty.PlayAction("ATTACK");
            Player.Death();
        }
    }
    
    #if UNITY_EDITOR

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackRadiusRange);
    }

#endif
}