using Management.ChunkManagement;
using Management.WorldManagement;
using NewMathf;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _speed = 5;
        [SerializeField] private float _sensivity = 3;
        [SerializeField] private GameObject _block;
        [SerializeField] private float _jumpForce;

        [SerializeField] private float _maxRaycastDistance = 3;
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
            UpdateRaycast();
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
            return Physics.Raycast(transform.position, -transform.up, 2);
        }

        private void SetJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                _rigidbody.AddForce(_jumpForce * Vector3.up, ForceMode.Impulse);
            }
        }

        private void SetRotate()
        { 
            _mouseInput.x += Input.GetAxis("Mouse X") * _sensivity;
            _mouseInput.y -= Input.GetAxis("Mouse Y") *  _sensivity;
            _mouseInput.y = Mathf.Clamp(_mouseInput.y, -90, 90);
            transform.localEulerAngles = new Vector3(0, _mouseInput.x, 0);
            _cam.localEulerAngles = new Vector3(_mouseInput.y, 0, 0);
        }


        private void UpdateRaycast()
        {
            
            // var hit = new RaycastHit();
            // if (!Physics.Raycast(_cam.position, _cam.forward, out hit, _maxRaycastDistance))
            //     return;
            //
            // var hitPoint = new IntVector(hit.point - hit.normal * .5f).ToVector3() ;
            //
            // if(!WorldGeneration.Instance.CheckForVoxel(hitPoint) && !hit.transform.GetComponent<Chunk>())
            //     return;
            //
            // var chunk = hit.transform.GetComponent<Chunk>();
            //
            // if (Input.GetMouseButtonDown(0))
            // {
            //     WorldGeneration.Instance.SetVoxel(chunk ,hitPoint, 0);
            // }
            //
            // if (Input.GetMouseButtonDown(1))
            // {
            //     WorldGeneration.Instance.SetVoxel(chunk ,new IntVector(hit.point + hit.normal * .5f).ToVector3(), 1);
            // }
                
            
            
            // _block.transform.position = hitPoint;

        }


    }
}
