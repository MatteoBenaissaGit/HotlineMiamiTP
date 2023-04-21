using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using MatteoBenaissaLibrary.TopDownCharacterController;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Melee = 0,
        Shoot = 1
    }
    
    public enum WeaponState
    {
        OnGround = 0,
        InHand = 1,
        LaunchedInAir = 2
    }

    [SerializeField] private Rigidbody2D _weaponRigidbody;
    [SerializeField] private Collider2D _weaponTriggerCollider;
    [field:SerializeField] public WeaponState CurrentState { get; private set; }
    [field:SerializeField] public WeaponType Type { get; private set; }
    [field:SerializeField] protected int TotalAmmo { get; set; }
    [field:SerializeField] protected int Damage { get; set; }
    [field:SerializeField] protected float UseCooldown { get; set; }
    [field: SerializeField] protected float LaunchForce { get; set; } = 150f;

    protected float CurrentUseCooldown;
    protected float LaunchTime = 0.5f;

    protected virtual void Start()
    {
        _weaponTriggerCollider.isTrigger = true;

        switch (CurrentState)
        {
            case WeaponState.OnGround:
                PutWeaponOnGround();
                break;
            case WeaponState.InHand:
                break;
        }
    }
    
    protected virtual void Update()
    {
        const float magnitudeToStopWeapon = 0.5f;
        if (CurrentState == WeaponState.LaunchedInAir && LaunchTime <= 0)
        {
            _weaponTriggerCollider.isTrigger = false;
            if (_weaponRigidbody.velocity.magnitude <= magnitudeToStopWeapon)
            {
                PutWeaponOnGround();
            }
        }
        
        if (CurrentState == WeaponState.LaunchedInAir)
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x,rotation.y,rotation.z+10);
        }
    }

    private void LateUpdate()
    {
        ManageCooldown();
    }

    protected virtual void PutWeaponOnGround()
    {
        CurrentState = WeaponState.OnGround;
        _weaponRigidbody.bodyType = RigidbodyType2D.Kinematic;
        _weaponRigidbody.velocity = Vector2.zero;
        _weaponTriggerCollider.isTrigger = true;
    }
    
    public virtual void UseWeapon()
    {
        if (TotalAmmo <= 0 || CurrentUseCooldown > 0)
        {
            return;
        }
        
        TotalAmmo--;
    }

    public virtual void LaunchWeapon()
    {
        CurrentState = WeaponState.LaunchedInAir;
        _weaponRigidbody.bodyType = RigidbodyType2D.Dynamic;
        _weaponTriggerCollider.isTrigger = true;

        LaunchTime = 0.1f;

        
        Vector3 force = MathTools.GetVector3DirectionToPlayerLooking(transform) * LaunchForce;
        _weaponRigidbody.AddForce(force);
    }
    

    protected virtual void ManageCooldown()
    {
        
        LaunchTime -= Time.deltaTime;
        CurrentUseCooldown -= Time.deltaTime;
    }

    public virtual void PickUpWeapon()
    {
        CurrentState = WeaponState.InHand;
        _weaponRigidbody.bodyType = RigidbodyType2D.Kinematic;
    }
    
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (CurrentState != WeaponState.LaunchedInAir)
        {
            return;
        }

        CheckForHittable(other.gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        CheckForHittable(col.gameObject);
        PutWeaponOnGround();
    }

    protected virtual void CheckForHittable(GameObject item)
    {
        IHittable hittable = item.GetComponent<IHittable>();
        if (hittable == null)
        {
            return;
        }

        int damage = CurrentState == WeaponState.LaunchedInAir ? Damage * 6 : Damage;
        hittable.GetHit(damage);
        Debug.Log($"Hit {item.name} : {damage}");

        if (CurrentState == WeaponState.LaunchedInAir)
        {
            PutWeaponOnGround();
        }
    }

    public int GetAmmoAmount()
    {
        return TotalAmmo;
    }
}
