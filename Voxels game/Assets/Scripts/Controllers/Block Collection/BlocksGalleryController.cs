using Assets.Scripts.Management.Blocks_Collection;
using Assets.Scripts.UI.Gallery;
using Controllers.Pause;
using Inventory;
using Management.UI;
using Management.VoxelManagement;
using UnityEngine;

namespace Assets.Scripts.Controllers.Block_Collection
{
    public class BlocksGalleryController : MonoBehaviour
    {
        [SerializeField] private GallerySlot[] _slots;

        private void Start()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                GallerySlot slot = _slots[i];
                slot.ChangeBlockPicture(VoxelProperties.GetBlockByType(InventoryManager.Instance.Slots[i]));
            }

            foreach (var item in BlocksGalleryManager.Instance.Items)
            {
                UIManager.Instance.AddGallerySlot(item);
            }
        }


    }
}
