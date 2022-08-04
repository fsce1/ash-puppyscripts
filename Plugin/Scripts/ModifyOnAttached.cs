using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FistVR;
using System.Linq;

namespace ashpuppyscripts
{
    public class ModifyOnAttached : MonoBehaviour
    {
        [Header("MODIFY ON HOVER REQUIRES THE VANILLA ATTACHMENT MOUNT TO HAVE A HOVER DISABLE PIECE (IT CAN JUST BE AN EMPTY GAMEOBJECT")]
        private bool willChange = true;
        public FVRFireArmAttachmentMount AttachmentMount;
        public bool ModifyOnHover;
        public bool OnlyModifyOnSpecificAttachment;
        public List<string> ItemIdsToModifyOn;
        [Header("Disables Objects")]
        public bool DisablesObjects;
        public List<GameObject> ObjectsToDisable;
        [Header("Enables Objects")]
        public bool EnablesObjects;
        public List<GameObject> ObjectsToEnable;
        [Header("Closed Bolt Weapon Modifiers")]
        public ClosedBoltWeapon ClosedBoltWeapon;
        [Header("Toggles Bolt Release Button")]
        public bool TogglesBoltRelease;
        private bool startBoltRelease;
        [Header("Toggles Mag Release Button")]
        public bool TogglesMagRelease;
        private bool startMagRelease;
        [Header("Bolt Action Weapon Modifiers")]
        public BoltActionRifle_Handle BoltActionBolt;
        [Header("Changes Base Rot Offset")]
        public bool ChangesBaseRotOffset;
        public float BaseRotOffsetUnattached;
        public float BaseRotOffsetAttached;
#if !DEBUG

        public void Start()
        {
            startBoltRelease = ClosedBoltWeapon.HasBoltReleaseButton;
            startMagRelease = ClosedBoltWeapon.HasMagReleaseButton;
        }
        public void FixedUpdate()
        {
            willChange = true;
            foreach (string Id in AttachmentMount.AttachmentsList.Select(item => item.ObjectWrapper.ItemID))
            {
                if (OnlyModifyOnSpecificAttachment &&  !ItemIdsToModifyOn.Contains(Id))
                {
                    willChange = false;
                }
            }
            if (!willChange){
                return;
            }

            if (ModifyOnHover)
            {
                if (AttachmentMount.DisableOnHover.activeInHierarchy == false)
                {
                    OnAttach();
                    return;
                }
                else
                {
                    OnDetach();
                    return;
                }
            }

            if (AttachmentMount.HasAttachmentsOnIt())
            {
                OnAttach();
                return;
            }
            OnDetach();
 
        }
        public void OnAttach()
        {
            if (DisablesObjects){
                foreach (GameObject gobj in ObjectsToDisable)
                {
                    gobj.SetActive(false);
                }
            }

            if (EnablesObjects){
                foreach (GameObject gobj in ObjectsToEnable)
                {
                    gobj.SetActive(true);
                }
            }

            if (TogglesBoltRelease){
                ClosedBoltWeapon.HasBoltReleaseButton = !startBoltRelease;
                }

            if (TogglesMagRelease)
            {
                ClosedBoltWeapon.HasMagReleaseButton = !startMagRelease;
            }

            if (ChangesBaseRotOffset){
                BoltActionBolt.BaseRotOffset = BaseRotOffsetAttached;
                }
        }
        public void OnDetach()
        {
            foreach (GameObject gobj in ObjectsToDisable)
            {
                gobj.SetActive(true);
            }
            foreach (GameObject gobj in ObjectsToEnable)
            {
                gobj.SetActive(false);
            }
            ClosedBoltWeapon.HasBoltReleaseButton = startBoltRelease;
            ClosedBoltWeapon.HasMagReleaseButton= startMagRelease;
            BoltActionBolt.BaseRotOffset = BaseRotOffsetUnattached;

        }
#endif
    }
}
