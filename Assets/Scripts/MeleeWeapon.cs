using System;
using DefaultNamespace;
using MatteoBenaissaLibrary.SpriteView;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private float _hitDetectionTime;
    [SerializeField] private SpriteView _spriteView;
    
    private float _currentHitDetectionTime;

    protected override void Update()
    {
        base.Update();
        
    }

    public override void LaunchWeapon()
    {
        base.LaunchWeapon();
        _currentHitDetectionTime = _hitDetectionTime;
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        if (_currentHitDetectionTime <= 0 || CurrentState != WeaponState.InHand)
        {
            return;
        }

        CheckForHittable(other.gameObject);
    }
    
    protected override void ManageCooldown()
    {
        LaunchTime -= Time.deltaTime;
        CurrentUseCooldown -= Time.deltaTime;
        _currentHitDetectionTime -= Time.deltaTime;
    }

    public override void UseWeapon()
    {
        if (CurrentUseCooldown > 0)
        {
            return;
        }
        
        _spriteView.PlayAction("USE");
        TotalAmmo++;
        _currentHitDetectionTime = _hitDetectionTime;
        
        base.UseWeapon();
        CurrentUseCooldown = UseCooldown;
    }

    protected override void CheckForHittable(GameObject item)
    {
        base.CheckForHittable(item);
        
        IHittable hittable = item.GetComponent<IHittable>();
        if (hittable == null)
        {
            return;
        }
        _currentHitDetectionTime = 0;
    }
}