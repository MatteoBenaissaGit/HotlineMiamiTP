using System;
using System.Drawing;
using DefaultNamespace;
using DG.Tweening;
using MatteoBenaissaLibrary.ReadOnly;
using MatteoBenaissaLibrary.SingletonClassBase;
using MatteoBenaissaLibrary.SpriteView;
using MatteoBenaissaLibrary.TopDownCharacterController;
using TMPro;
using UnityEngine;

[Serializable]
public struct TriggerZone
{
    public Vector2 Size;
    public float Distance;
}

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private Transform _weaponPositionPoint;

    [field: SerializeField] public Rigidbody2D PlayerRigidbody2D { get; private set; }
    [field: SerializeField] public SpriteView SpriteViewProperty { get; private set; }
    [field: SerializeField] public TopDownCharacterController CharacterController { get; private set; }
    [field: SerializeField] public GameObject AmmoUI { get; private set; }
    [field: SerializeField] public TMP_Text AmmoText { get; private set; }
    [field: SerializeField] public GameObject MultiplierUI { get; private set; }
    [field: SerializeField] public TMP_Text MultiplierText { get; private set; }
    [field: SerializeField] public TMP_Text PointsText { get; private set; }
    [field: SerializeField, ReadOnly] public int Points { get; private set; }
    [field: SerializeField, ReadOnly] public int CurrentMultiplier { get; private set; }
    [field: SerializeField] public TriggerZone KickZoneTrigger { get; private set; }
    [field: SerializeField] public bool IsDead { get; private set; }
    
    private Weapon _currentWeapon;
    private float _currentMuliplierCooldown;
    private bool _checkForKickTrigger;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        Weapon weapon = col.gameObject.GetComponent<Weapon>();
        if (weapon == null || _currentWeapon != null ||
            weapon.CurrentState == Weapon.WeaponState.LaunchedInAir)
        {
            return;
        }

        _currentWeapon = weapon;
        _currentWeapon.PickUpWeapon();
        Debug.Log($"picked up {weapon.gameObject.name}");
    }

    private void Update()
    {
        if (IsDead)
        {
            return;
        }
        
        ManageCurrentWeapon();
        ManageUI();

        HandleKick();
    }

    private void ManageCurrentWeapon()
    {
        if (_currentWeapon == null)
        {
            return;
        }

        _currentWeapon.transform.position = _weaponPositionPoint.position;
        _currentWeapon.transform.rotation = _weaponPositionPoint.rotation;
        
        if (Input.GetMouseButton(0))
        {
            _currentWeapon.UseWeapon();
        }
        if (Input.GetMouseButton(1))
        {
            _currentWeapon.LaunchWeapon();
            _currentWeapon = null;
        }
    }

    private void HandleKick()
    {
        if (Input.GetMouseButtonDown(0) && _currentWeapon == null)
        {
            SpriteViewProperty.PlayAction("KICK");
            CharacterController.CanMove = false;
            SpriteViewProperty.OnActionEnd.AddListener(AllowMovement);
            _checkForKickTrigger = true;
        }

        RaycastHit2D[] hits = new RaycastHit2D[20];
        if (_checkForKickTrigger)
        {
            Vector2 position = MathTools.GetPointAtDistanceAndAngle((Vector2)transform.position, KickZoneTrigger.Distance, transform.rotation.eulerAngles.z);
            
            int numHits = Physics2D.BoxCastNonAlloc( position,
                KickZoneTrigger.Size,
                0,
                Vector2.zero, hits);

            foreach (var item in hits)
            {
                if (item.collider == null || item.collider.gameObject == null)
                {
                    continue;
                }
                
                IHittable hittable = item.collider.gameObject.GetComponent<IHittable>();
                if (hittable == null)
                {
                    continue;
                }
                
                hittable.GetHit(10);
            }
        }
    }

    private void AllowMovement()
    {
        CharacterController.CanMove = true;
        _checkForKickTrigger = false;
        SpriteViewProperty.OnActionEnd.RemoveListener(AllowMovement);
    }

    private void ManageUI()
    {
        AmmoUI.SetActive(_currentWeapon != null && _currentWeapon.Type == Weapon.WeaponType.Shoot);
        if (_currentWeapon != null)
        {
            AmmoText.text = _currentWeapon.GetAmmoAmount().ToString();
        }

        PointsText.text = $"{Points}pts";
        
        //multiplier
        _currentMuliplierCooldown -= Time.deltaTime;
        MultiplierUI.SetActive(CurrentMultiplier > 0);
        MultiplierText.text = $"x{CurrentMultiplier}";
        if (_currentMuliplierCooldown <= 0)
        {
            CurrentMultiplier = 0;
        }
    }

    public void AddPoints(int value)
    {
        Points += value;
        PointsText.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
    }

    public void AddMultiplier(int value)
    {
        CurrentMultiplier += value;
        _currentMuliplierCooldown = 5;
        MultiplierUI.transform.DOPunchScale(Vector3.one * 0.1f, 0.15f);
    }

    public void Death()
    {
        SpriteViewProperty.PlayState("DEATH");
        IsDead = true;
        CharacterController.CanMove = false;
        GameManager.Instance.Lose();
    }
    
    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Vector2 position = MathTools.GetPointAtDistanceAndAngle((Vector2)transform.position, KickZoneTrigger.Distance, transform.rotation.eulerAngles.z);
        Gizmos.DrawWireCube(position, KickZoneTrigger.Size);
    }

#endif
}