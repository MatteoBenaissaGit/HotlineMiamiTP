using System;
using DefaultNamespace;
using MatteoBenaissaLibrary.SpriteView;
using UnityEngine;

public class Enemy : MonoBehaviour, IHittable
{
    public enum EnemyState
    {
        Walking = 0,
        Attacking = 1
    }

    [Serializable]
    public struct EnemyPath
    {
        public Vector2 PointA;
        public Vector2 PointB;
    }

    [field: SerializeField] protected int Life { get; set; }
    [field: SerializeField] protected int PointsGiven { get; set; }
    [field: SerializeField] protected SpriteView SpriteViewProperty { get; set; }
    [field: SerializeField] protected SpriteRenderer SpriteRendererProperty { get; set; }
    [field: SerializeField] protected Collider2D Collider2DProperty { get; set; }
    [field: SerializeField] public Rigidbody2D Rigidbody2DProperty { get; set; }
    [field: SerializeField] public Weapon WeaponToDrop { get; private set; }
    [field: SerializeField] public float RadiusDetectionRange { get; private set; }
    [field: SerializeField] public EnemyState CurrentState { get; private set; }
    [field: SerializeField] protected PlayerManager Player { get; private set; }
    [field: SerializeField] protected float BaseSpeed { get; private set; }
    [field: SerializeField] protected float AttackSpeed { get; private set; }
    [field: SerializeField] public bool IsDead { get; private set; }
    [field: SerializeField] protected EnemyPath Path { get; private set; }

    private int _currentPointToGoTo = 1;
    
    private void Start()
    {
        CurrentState = EnemyState.Walking;
    }

    protected virtual void Update()
    {
        if (IsDead)
        {
            return;
        }
        
        GoToPoint();
        CheckForPlayer();
        HandleEnemyWalk();
        
        float angle = Mathf.Atan2(Rigidbody2DProperty.velocity.y, Rigidbody2DProperty.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void GoToPoint()
    {
        if (CurrentState == EnemyState.Attacking)
        {
            return;
        }
        
        Debug.Log("enemy is walking");
        switch (_currentPointToGoTo)
        {
            case 0:
                Vector2 directionToA = (Path.PointA - (Vector2)transform.position).normalized;
                Debug.Log(directionToA);
                Rigidbody2DProperty.velocity = directionToA * BaseSpeed;
                if (Vector2.Distance(transform.position, Path.PointA) <= 0.1f)
                {
                    _currentPointToGoTo = 1;
                }
                break; 
            case 1:
                Vector2 directionToB = (Path.PointB - (Vector2)transform.position).normalized;
                Debug.Log(directionToB);
                Rigidbody2DProperty.velocity = directionToB * BaseSpeed;
                if (Vector2.Distance(transform.position, Path.PointB) <= 0.1f)
                {
                    _currentPointToGoTo = 0;
                }
                break;
        }
    }

    public void GetHit(int value)
    {
        Life -= value;
        if (Life <= 0)
        {
            Die();
        }

        Debug.Log("velocity to 0");
        Rigidbody2DProperty.velocity = Vector2.zero;
    }

    protected virtual void HandleEnemyWalk()
    {
        if (CurrentState != EnemyState.Walking)
        {
            return;
        }
    }

    protected virtual void CheckForPlayer()
    {
        if (Vector2.Distance(transform.position, PlayerManager.Instance.transform.gameObject.transform.position) <= RadiusDetectionRange)
        {
            Player = PlayerManager.Instance;
            CurrentState = EnemyState.Attacking;
        }
        else
        {
            Player = null;
        }
    }

    protected void Die()
    {
        PlayerManager.Instance.AddPoints(PointsGiven);
        PlayerManager.Instance.AddMultiplier(1);

        Weapon weapon = Instantiate(WeaponToDrop);
        weapon.transform.position = transform.position;

        SpriteViewProperty.PlayState("DEAD");
        Destroy(Rigidbody2DProperty);
        Destroy(Collider2DProperty);

        SpriteRendererProperty.sortingOrder = 1;
        
        IsDead = true;
    }
    
    

#if UNITY_EDITOR

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RadiusDetectionRange);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(Path.PointA, Vector3.one * 0.1f);
        Gizmos.DrawCube(Path.PointB, Vector3.one * 0.1f);
    }

#endif
}