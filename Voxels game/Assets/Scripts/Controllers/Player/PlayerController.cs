using System;
using System.Collections;
using System.Threading;
using _3D.Mathf2;
using Inventory;
using Management.ChunkManagement;
using Management.UI;
using Management.WorldManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 5;
        private Vector2 _movementInputs;
        [SerializeField]private bool _isMoving;
        public bool IsMoving => _isMoving;
        
        
        [Header("Looking")]
        [SerializeField] private float _sensivity = 3;
        private Vector2 _mouseInput;
        
        [SerializeField] private GameObject _block;
        [SerializeField] private float _jumpForce;
        
        private ActionsManager _inputs;
        
        
        private int _scrollIndex;
        private Rigidbody _rigidbody;
        private Transform _cam;
        
        [Header("Building Raycast")]
        [SerializeField] private float _maxRaycastDistance = 3;
        private bool _canModify;
        private Chunk _detectedChunk;
        private Vector3 _voxelPos, _buildPos;

        private void Awake()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();

            _inputs.Player.Build.started += ctx => Build();
            _inputs.Player.Destroy.started += ctx => DestroyBlock();
            _inputs.Player.Jump.started += ctx => SetJump();

            WorldGenerator.Instance.OnWorldIsGenerated += OnWorldGenerated;
        }

        private void Start()
        {
            _cam = Camera.main.transform;
            _rigidbody = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;

            _rigidbody.isKinematic = true;
        }

        private void OnWorldGenerated()
        {
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
            UpdateRaycast();
        }


        public void SetMove(InputAction.CallbackContext ctx)
        {
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
            return Physics.Raycast(transform.position, -transform.up, 2);
        }

        public void SetJump()
        {
            if (IsGrounded())
            {
                _rigidbody.AddForce(_jumpForce * Vector3.up, ForceMode.Impulse);
            }
        }

        public void SetRotate(InputAction.CallbackContext ctx)
        {
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
            if (!Physics.Raycast(_cam.position, _cam.forward, out hit, _maxRaycastDistance))
            {
                _canModify = false;
                _block.SetActive(false);
                return;
            }
            
            _voxelPos = RoundToInt(hit.point - hit.normal * .5f);
            _buildPos = RoundToInt(hit.point + hit.normal * .5f);

            if (!WorldGenerator.Instance.CheckForVoxel(_voxelPos) && !hit.transform.GetComponent<Chunk>())
            {
                _canModify = false;
                return;
            }
                

            _canModify = true;
            _block.SetActive(true);
            _detectedChunk = hit.transform.GetComponent<Chunk>();
            _block.transform.position = _voxelPos;

        }

        private void DestroyBlock()
        {
            if(_canModify)
                WorldGenerator.Instance.SetVoxel(_detectedChunk, _voxelPos, 0);
        }

        private void Build()
        {
            var playersBlock = RoundToInt(transform.position);
            
            if(_buildPos.Equals(playersBlock) || _buildPos.Equals(new Vector3(playersBlock.x, playersBlock.y + 1, playersBlock.z)))
                return;

            if(_canModify)
                WorldGenerator.Instance.SetVoxel(_detectedChunk, _buildPos, InventoryManager.Instance.GetBlockIndex());
        }
        
        private void SetScrolling(InputAction.CallbackContext ctx)
        {
            var scrollValue = ctx.ReadValue<float>();
            
            if(scrollValue > 0f && _scrollIndex < 6)
            {
                _scrollIndex++;
            }

            if (scrollValue < 0f && _scrollIndex > 0)
            {
                _scrollIndex--;
            }
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
