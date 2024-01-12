using Assets.Scripts.Management.Blocks_Collection;
using Controllers.Pause;
using Management.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers.Block_Collection
{
    public class BlocksGalleryController : MonoBehaviour
    {
        private void Start()
        {
            foreach (var item in BlocksGalleryManager.Instance.Items)
            {
                UIManager.Instance.AddGallerySlot(item);
            }
        }


    }
}
