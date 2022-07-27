using System;
using Blocks;
using DG.Tweening;
using UnityEngine;

namespace Controllers.Build_Voxel
{
    public class BuildVoxelControlller : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Material _material;
        private MeshFilter _meshFilter;
        private float _time;
        private Vector3 _target;

        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _time = Vector3.Distance(transform.position, _target) / _speed;
            Debug.Log(_time);
            transform.localScale *= .1f;
            // transform.DOScale(1, _time);
        }


        public void SetMesh (Block block, Vector3 targetPosition)
        {
            _target = targetPosition;
            _material.mainTexture = block.BlockProfile;

            
            // _meshFilter.mesh = block.GetVoxelMesh();
        }


        private void Update()
        {
            if (Vector3.Distance(transform.position, _target) > 0)
            {
                transform.LookAt(_target);
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
                
            }
            
            if(Vector3.Distance(transform.position, _target) < .1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
