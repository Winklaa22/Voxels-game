using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _3D.Mathf2;
using Inventory;
using Management.ChunkManagement;
using Management._Cursor;
using Management.UI;
using Management.WorldManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    public class PlayerController : MonoBehaviour
    {
        private bool _isActive = true;

        [Header("Movement")]
        [SerializeField] private float _speed = 5;
        private Vector2 _movementInputs;
        [SerializeField]private bool _isMoving;
        public bool IsMoving => _isMoving;

        [Header("Looking")] 
        private bool _canLooking;
        public bool CanLooking => _canLooking;
        [SerializeField] private float _sensivity = 3;
        private Vector2 _mouseInput;
        
        [SerializeField] private MeshFilter _block;
        [SerializeField] private float _jumpForce;


        [Header("Animations")] 
        [SerializeField] private Animator _animator;
        private float _movementTransition;
        private Vector2 _animInputs;
        
        private ActionsManager _inputs;

        private int _scrollIndex;
        private Rigidbody _rigidbody;
        private Transform _cam;

        [Header("Building Raycast")] [SerializeField]
        private GameObject _buildparticle;
        [SerializeField] private float _maxRaycastDistance = 3;
        private bool _canModify;
        private Chunk _detectedChunk;
        private Vector3 _hitPoint, _hitNormal, _voxelPos, _buildPos;

        private void Awake()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();

            _inputs.Player.Build.started += ctx => TryBuild();
            _inputs.Player.Destroy.started += ctx => DestroyBlock();
            _inputs.Player.Jump.started += ctx => SetJump();

            WorldGenerator.Instance.OnWorldIsGenerated += OnWorldGenerated;
        }

        private void Start()
        {
            _isActive = true;
            _cam = Camera.main.transform;
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
            UpdateRaycast();
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

        public void SetMove(InputAction.CallbackContext ctx)
        {
            if(!_isActive)
                return;
            
            
            _movementInputs = ctx.ReadValue<Vector2>();
            StartCoroutine(SetInputs(ctx.ReadValue<Vector2>()));
            _isMoving = _movementInputs.magnitude > 0;
            StartCoroutine(Movement());
        }

        private IEnumerator SetInputs(Vector2 value)
        {
            if(!_animInputs.Equals(value))
            {
            }

            yield return null;
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


        private void UpdateRaycast()
        {
            var hit = new RaycastHit();
            var selectHit = new RaycastHit();
            if (!Physics.Raycast(_cam.position, _cam.forward, out hit, _maxRaycastDistance))
            {
                _canModify = false;
                _block.gameObject.SetActive(false);
                return;
            }
            
            _hitPoint = hit.point;
            
            _voxelPos = RoundToInt(hit.point - hit.normal * .5f);
            _buildPos = RoundToInt(hit.point + hit.normal * .5f);
            
            if (!WorldGenerator.Instance.CheckForVoxel(_voxelPos) && !hit.transform.GetComponent<Chunk>())
            {
                _canModify = false;
                return;
            }

            _canModify = true;
            _block.mesh = WorldGenerator.Instance.GetVoxelType(_voxelPos).GetVoxelMesh();
            _block.gameObject.SetActive(true);
            _detectedChunk = hit.transform.GetComponent<Chunk>();
            _block.transform.position = _voxelPos;
        }

        private void DestroyBlock()
        {
            if(!_canModify || !_isActive)
                return;
            
            WorldGenerator.Instance.CreateDestroyParticle(_voxelPos);
            WorldGenerator.Instance.SetVoxel(_detectedChunk, _voxelPos, 0);
        }

        private void TryBuild()
        {
            var playersBlock = RoundToInt(transform.position);
            
            if(!_isActive || _buildPos.Equals(playersBlock) || _buildPos.Equals(new Vector3(playersBlock.x, playersBlock.y + 1, playersBlock.z)) || !_canModify)
                return;
            
            WorldGenerator.Instance.SetVoxel(_detectedChunk, _buildPos, InventoryManager.Instance.GetBlockIndex());
            Instantiate(_buildparticle, _buildPos - new Vector3(0, .5f, 0), Quaternion.identity);

        }


        private Vector3 RoundToInt(Vector3 vector)
        {
            var x = Mathf.RoundToInt(vector.x);
            var y = Mathf.RoundToInt(vector.y);
            var z = Mathf.RoundToInt(vector.z);
            
            return new Vector3(x, y, z);
        }


    }
}
