using FistVR;
using UnityEngine;
namespace ashpuppyscripts
{
    public class SlidingAmmoBoxFeatures : MonoBehaviour
    {
        public FVRFireArmMagazine Magazine;
        public GameObject MovableWeaponPart;
        public GameObject GlowingMesh;
        private Vector3 StartingPosition;
        private bool HasBeenMoved = false;
#if !DEBUG
        public void Start()
        {
            StartingPosition = MovableWeaponPart.transform.localPosition;
        }
        public void FixedUpdate()
        {
            if (!HasBeenMoved && MovableWeaponPart.transform.localPosition != StartingPosition)
            {
                GlowingMesh.SetActive(false);
                HasBeenMoved = true;
            }
            if (MovableWeaponPart.transform.localPosition == StartingPosition)
            {
                Magazine.CanManuallyEjectRounds = false;
                return;
            }
            Magazine.CanManuallyEjectRounds = true;
        }
#endif
    }

}