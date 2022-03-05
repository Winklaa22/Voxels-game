using System;
using Management.ChunkManagement;
using Management.VoxelManagement;
using Management.WorldManagement;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _sensivity = 3;
        [SerializeField] private GameObject _block;
        public float checkIncrement = 0.1f;
        public float reach = 8f;
        public GameObject _particlePrefab;
        public Transform highlightBlock;
        public Transform placeBlock;
        private Vector2 _mouseInput;
        private Rigidbody _rigidbody;
        private Transform _cam;



        private void Start()
        {
            _cam = Camera.main.transform;
            _rigidbody = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;

        }

        private void Update()
        {
            SetMovement();
            SetJump();
            SetRotate();
        }


        private void SetMovement()
        {
            var horizontal = Input.GetAxis("Horizontal") * _speed;
            var vertical = Input.GetAxis("Vertical") * _speed;

            var velocityVector = new Vector3(horizontal, _rigidbody.velocity.y, vertical);
            _rigidbody.velocity = transform.TransformDirection(velocityVector);
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, -transform.up, 3);
        }

        private void SetJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                _rigidbody.AddForce(5f * Vector3.up, ForceMode.Impulse);
            }
        }

        private void SetRotate()
        { 
            _mouseInput.x += Input.GetAxis("Mouse X") * _sensivity;
            _mouseInput.y -= Mathf.Clamp(Input.GetAxis("Mouse Y") * _sensivity, -90, 90);
            transform.localEulerAngles = new Vector3(0, _mouseInput.x, 0);
            _cam.localEulerAngles = new Vector3(_mouseInput.y, 0, 0);
        }

        

    }
}
