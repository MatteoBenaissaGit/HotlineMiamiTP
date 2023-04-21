using System;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private ParticleSystem _particleSystem;

    private int _damage;
    private Vector2 _baseVelocity;
    
    public void Initialize(float angle, int damage)
    {
        Vector3 force = MathTools.GetVector3DirectionToPlayerLooking(transform) * _speed;
        _damage = damage;
        
        transform.rotation = Quaternion.Euler(0,0,angle);
        _rigidbody2D.velocity = force;
        _baseVelocity = _rigidbody2D.velocity;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        DestroyBullet();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        IHittable hittable = col.gameObject.GetComponent<IHittable>();
        if (hittable != null)
        {
            hittable.GetHit(_damage);
        }
        
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        _particleSystem.Play();
        _particleSystem.transform.parent = null;
        
        Destroy(gameObject);
    }

    private void Update()
    {
        _rigidbody2D.velocity = _baseVelocity + PlayerManager.Instance.PlayerRigidbody2D.velocity;
    }
}