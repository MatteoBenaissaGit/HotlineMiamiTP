using System;
using UnityEngine;

public class ShootWeapon : Weapon
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletLaunchPosition;

    public override void UseWeapon()
    {
        base.UseWeapon();
        
        if (TotalAmmo <= 0 || CurrentUseCooldown > 0)
        {
            return;
        }
        
        Bullet bullet = Instantiate(_bulletPrefab);
        bullet.transform.position = _bulletLaunchPosition.position;
        bullet.Initialize(transform.rotation.eulerAngles.z, Damage);
        
        CurrentUseCooldown = UseCooldown;
    }
}