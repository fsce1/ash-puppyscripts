using FistVR;
using UnityEngine;
using Cityrobo;
namespace ashpuppyscripts
{
    public class SlidingAmmoBoxFeatures : MonoBehaviour
    {
        public FVRFireArmMagazine Magazine;
        public MovableWeaponPart MovableWeaponPart;
        public GameObject GlowingMesh;
        //private Vector3 StartingPosition;
        private bool HasBeenMoved = false;
#if !DEBUG
        //public void Start()
        //{
        //    StartingPosition = MovableWeaponPart.transform.localPosition;
        //}
        public void FixedUpdate()
        {
            if (!HasBeenMoved && MovableWeaponPart.transform.localPosition.z == MovableWeaponPart.upperLimit)
            {
                GlowingMesh.SetActive(false);
                HasBeenMoved = true;
            }
            if (MovableWeaponPart.transform.localPosition.z != MovableWeaponPart.upperLimit)
            {
                Magazine.CanManuallyEjectRounds = false;
                return;
            }
            Magazine.CanManuallyEjectRounds = true;
        }
#endif
    }

}