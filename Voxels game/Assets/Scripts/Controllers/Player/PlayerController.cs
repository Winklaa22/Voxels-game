using System.Collections;
using Management.UI;
using Management.WorldManagement;
using Player.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    public class PlayerController : MonoBehaviour
    {
        private bool _isActive = true;

        public delegate void OnSpawn();
        public event OnSpawn OnSpawnEntity;
        
        [Header("Movement")]
        [SerializeField] private float _speed = 5;
        private Vector2 _movementInputs;
        [SerializeField] private bool _isMoving;

        [Header("Looking")] 
        private bool _canLooking;

        [SerializeField] private float _sensivity = 3;
        private Vector2 _mouseInput;
        
        [SerializeField] private float _jumpForce;


        [Header("Animations")] 
        [SerializeField] private Animator _animator;
        private float _movementTransition;
        private Vector2 _animInputs;


        private ActionsManager _inputs;

        private int _scrollIndex;
        private Rigidbody _rigidbody;
        private Transform _cam;

        private void Awake()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();
            
            _inputs.Player.Jump.started += ctx => SetJump();

            WorldGenerator.Instance.OnWorldIsGenerated += OnWorldGenerated;
        }

        private void Start()
        {
            
            
            Application.targetFrameRate = 60;

            _isActive = true;
            if (Camera.main is not null) _cam = Camera.main.transform;
            _rigidbody = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;

            _rigidbody.isKinematic = true;
        }

     

        private void OnWorldGenerated()
        {
            _canLooking = true;
            _rigidbody.isKinematic = false;
            UIManager.Instance.SetScreen(ScreenType.HUD);
        }

        private void FixedUpdate()
        {
            var horizontal = _movementInputs.x * _speed;
            var vertical = _movementInputs.y * _speed;
                
            var velocityVector = transform.TransformDirection(new Vector3(horizontal, _rigidbody.velocity.y, vertical));
                
            _rigidbody.velocity = velocityVector;
        }

        private void Update()
        {
            UpdateAnimations();
        }

        public void SetActive(bool active)
        {
            _isActive = active;
            
            if(!active)
                return;
            
            _movementInputs = Vector2.zero;
            _rigidbody.velocity = Vector3.zero;
            _isMoving = false;
        }

        public void Spawn()
        {
            OnSpawnEntity?.Invoke();
        }

        public void SetMove(InputAction.CallbackContext ctx)
        {
            if(!_isActive)
                return;
            
            _movementInputs = ctx.ReadValue<Vector2>();
            _isMoving = _movementInputs.magnitude > 0;
            StartCoroutine(Movement());
        }
        

        private IEnumerator Movement()
        {
            while (_isMoving)
            {
                yield return WorldGenerator.Instance.UpdateRenderChunks();;
            }
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, -transform.up, 1);
        }

        public void SetJump()
        {
            if(!_isActive)
                return;
            
            if (IsGrounded())
            {
                _rigidbody.AddForce(_jumpForce * Vector3.up, ForceMode.Impulse);
            }
        }

        private void UpdateAnimations()
        {
            _animInputs = _isMoving ? Vector2.Lerp(_animInputs, _movementInputs, .1f) : Vector2.zero;

            _movementTransition = Mathf.Clamp(_movementTransition += _isMoving ? Time.deltaTime : -Time.deltaTime, 0, .3f);
            
            _animator.SetFloat("Speed", _movementTransition);
            
            // directions
            _animator.SetFloat("Horizontal", _animInputs.x);
            _animator.SetFloat("Vertical", _animInputs.y);
            
            //falling
            _animator.SetBool("IsGrounded", IsGrounded());
        }

        public void SetRotate(InputAction.CallbackContext ctx)
        {
            if(!_isActive)
                return;
            
            var inputs = ctx.ReadValue<Vector2>();
            _mouseInput.x += inputs.x * _sensivity;
            _mouseInput.y -= inputs.y *  _sensivity;
            _mouseInput.y = Mathf.Clamp(_mouseInput.y, -90, 90);
            transform.localEulerAngles = new Vector3(0, _mouseInput.x, 0);
            _cam.localEulerAngles = new Vector3(_mouseInput.y, 0, 0);
        }
    }
}
