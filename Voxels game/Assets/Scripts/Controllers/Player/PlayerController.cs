using System;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _sensivity;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }


        private void Update()
        {
            Movement();
            SetRotate();
        }

        private void Movement()
        {
            var hor = Input.GetAxis("Horizontal");
            var ver = Input.GetAxis("Vertical");

            var move = new Vector3(hor, 0, ver);
            
            _rigidbody.velocity = transform.TransformDirection(move * _speed);
        }

        private void SetRotate()
        {
            var mouseX = Input.GetAxis("Mouse X") * _sensivity;
            transform.Rotate(new Vector3(0, mouseX, 0));
        }
    }
}
