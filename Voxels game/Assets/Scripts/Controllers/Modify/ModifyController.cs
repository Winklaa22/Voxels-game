using System;
using Chunks;
using Controllers.Build_Voxel;
using Inventory;
using Management.Game;
using Management.WorldManagement;
using UnityEngine;

namespace Controllers.Modify
{
    public class ModifyController : MonoBehaviour
    {
        [Header("Building Raycast")]
        [SerializeField] private float _maxRaycastDistance = 3;
        [SerializeField] private Transform _cameraTransform;
        
        [Header("Inputs")]
        private ActionsManager _inputs;
        [SerializeField] private GameObject _buildparticle;

        [Header("Select cube")]
        [SerializeField] private Material _selectCubeMaterial;
        [SerializeField] private BuildVoxelControlller _cube;
        
        private bool _canModify;
        private Chunk _detectedChunk;
        private Vector3 _hitPoint, _hitNormal, _voxelPos, _buildPos;
        
        private MeshFilter _block;


        private void SetInputs()
        {
            _inputs = new ActionsManager();
            _inputs.Enable();
            _inputs.Player.Build.started += ctx => TryBuild();
            _inputs.Player.Destroy.started += ctx => DestroyBlock();
        }

        private void Awake()
        {
            SetInputs();
        }

        private void CreateSelectBox()
        {
            var box = new GameObject
            {
                transform =
                {
                    name = "Select cube"
                }
            };

            box.AddComponent<MeshRenderer>().material = _selectCubeMaterial;
            _block = box.AddComponent<MeshFilter>();
        }

        private void Start()
        {
            CreateSelectBox();
        }

        private void UpdateRaycast()
        {
            var hit = new RaycastHit();
            var selectHit = new RaycastHit();
            if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, _maxRaycastDistance))
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

        private void Update()
        {
            UpdateRaycast();
        }

        private void DestroyBlock()
        {
            if(!_canModify || GameManager.Instance.CantBuild)
                return;
            
            WorldGenerator.Instance.CreateDestroyParticle(_voxelPos);
            WorldGenerator.Instance.SetVoxel(_detectedChunk, _voxelPos, 0);
        }

        private void TryBuild()
        {
            if(GameManager.Instance.CantBuild)
                return;

            var playersBlock = RoundToInt(transform.position);
            
            if(_buildPos.Equals(playersBlock) || _buildPos.Equals(new Vector3(playersBlock.x, playersBlock.y + 1, playersBlock.z)) || !_canModify)
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
