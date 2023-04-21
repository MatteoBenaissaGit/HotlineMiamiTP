using System;
using MatteoBenaissaLibrary.ReadOnly;
using UnityEngine;

namespace MatteoBenaissaLibrary.TopDownCharacterController
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownCharacterController : MonoBehaviour
    {
        [SerializeField, Range(0,20)] private float _speed = 5;
        [SerializeField, Range(0,1)] private float _deceleration = 0.5f;
        [SerializeField, Range(0,1)] private float _acceleration = 0.5f;
        
        [ReadOnly] public bool CanMove = true;
        
        private Vector2 _inputs;
        private Rigidbody2D _rigidbody;
        [SerializeField] private SpriteView.SpriteView _spriteView;
        [SerializeField] private PlayerManager _characterManager;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            
            _spriteView.OnActionEnd.AddListener(EndAction);
        }

        private void OnDestroy()
        {
            _spriteView.OnActionEnd.RemoveListener(EndAction);
        }

        private void Update()
        {
            if (_characterManager.IsDead)
            {
                return;
            }
            
            HandleMovementInputs();
            RotationTowardMouse();
        }

        private void FixedUpdate()
        {
            ApplyAnimation();
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            float speed = CanMove && _inputs.magnitude > 0.1f ? _speed : 0;
            _rigidbody.velocity = _inputs.normalized * speed;
        }

        private void HandleMovementInputs()
        {
            //get inputs
            float inputQ = Input.GetKey(KeyCode.Q) ? -1 : 0;
            float inputD = Input.GetKey(KeyCode.D) ? 1 : 0;
            float rawInputX = inputQ + inputD;
            float inputZ = Input.GetKey(KeyCode.Z) ? 1 : 0;
            float inputS = Input.GetKey(KeyCode.S) ? -1 : 0;
            float rawInputY = inputZ + inputS;
            //lerp current value toward raw
            float lerpValue = Mathf.Abs(rawInputX) + Mathf.Abs(rawInputY) < 0.1f ? _deceleration : _acceleration;
            float lerpX = Mathf.Lerp(_inputs.x, rawInputX, lerpValue);
            float lerpY = Mathf.Lerp(_inputs.y, rawInputY, lerpValue);
            //assign input value
            _inputs = new Vector2(lerpX, lerpY);
        }
        
        private void ApplyAnimation()
        {
            if (_characterManager.IsDead)
            {
                return;
            }
            
            _spriteView.PlayState(_rigidbody.velocity.magnitude > 0.1f ? "WALK" : "IDLE");
        }

        private void EndAction()
        {
            CanMove = true;
        }

        private void RotationTowardMouse()
        {
            transform.rotation = Quaternion.AngleAxis(GetDirectionToMousePositionFromPlayer(transform), Vector3.forward);
        }

        public static float GetDirectionToMousePositionFromPlayer(Transform transform)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mouseWorldPosition.z = transform.position.z; // Make sure the z-coordinate is the same as the sprite's
            Vector3 direction = mouseWorldPosition - transform.position;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
    }
}