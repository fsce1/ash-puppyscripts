using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FistVR;

namespace ashpuppyscripts
{
    public class ModifyOnAttached : MonoBehaviour
    {
        #region 
        bool HasModifiedGun = false;
        bool HasDemodifiedGun = false;
        public FVRFireArmAttachmentMount AttachmentMount;
        [Tooltip("The Firearm you want to affect on attach.")]
        public FVRFireArm FireArm;
        //Non-functional, shelved for now
        //[Tooltip("Vanilla-like behaviour of modifying the weapon when the attachment visually snaps to the mount, instead of after you let go of attachment. Modify on Hover requires the Vanilla Atachment Mount to have a Hover Disable Piece (It can just be an Empty GameObject).")]
        //public bool ModifyOnHover;
        [Tooltip("Only modify weapon when specific attachments (set in the ItemIDsToModifyOn List) are attached to the gun. Allows some attachments Modify the gun, while others do nothing.")]
        public bool OnlyModifyOnSpecificAttachment;
        [Tooltip("The List of Item IDs (I.E. FSCE.GrozaSuppressor) that modify the gun when attached. Only attachments in this list will modify the Firearm.")]
        public List<string> ItemIdsToModifyOn;
        [Tooltip("If ObjectsToDisable/Enable references another instance of a ModifyOnAttached Script, that script will run the Detach code (resetting values to default) when it is disabled by this script.")]
        public bool DisableDetachesOtherModifyOnAttached;
        [Header("Generic Modifications")]
        #region Disables/Enables Objects
        [Tooltip("GameObjects placed into the ObjectsToDisable list will be Disabled when attachment is attached, and Enabled when detached.")]
        public bool DisablesObjects;
        public List<GameObject> ObjectsToDisable;
        public bool EnablesObjects;
        [Tooltip("GameObjects placed into the ObjectsToEnable list will be Enabled when attachment is attached, and Disabled when detached.")]
        public List<GameObject> ObjectsToEnable;
        #endregion
        #region Round and Mag Type
        [Tooltip("Changes the chamber and firearm round types (for manually chambering rounds")]
        public bool ChangesRoundType;
        public FireArmRoundType RoundType;
        FireArmRoundType startRoundType;
        [Tooltip("Changes the firearm's magazine type, and drops the current mag when changed.")]
        public bool ChangesMagType;
        public FireArmMagazineType MagType;
        FireArmMagazineType startMagType;
        [Tooltip("Changes the reference for the Firearms Magazine_Mount and Magazine_Eject between 2 different GameObjects.")]
        public bool ChangesMagPose;
        public Transform OriginalMagMountPose;
        public Transform OriginalMagEjectPose;
        public Transform NewMagMountPose;
        public Transform NewMagEjectPose;

        #endregion
        #region ChangesPoseOverride
        [Tooltip("Changes the reference for the Firearms PoseOverride and PoseOverrideTouch between 2 different GameObjects. [NEEDS TO BE UNDER THE SAME PARENT IN HIERARCHY]")]
        public bool ChangesPoseOverride;
        Vector3 StartRecoilHolderPos;
        Vector3 StartPoseOverridePos;
        Quaternion StartPoseOverrideRot;
        Vector3 StartPoseOverrideTouchPos;
        Quaternion StartPoseOverrideTouchRot;
        public Transform NewRecoilHolder;
        public Transform NewPoseOverride;
        public Transform NewPoseOverrideTouch;
        #endregion
        [Header("Closed-Bolt Specific Modifications")]
        [Tooltip("Inverts the ability for the bolt to be released with a controller")]
        public bool TogglesBoltReleaseButton;
        bool startBoltRelease;
        [Tooltip("Inverts the ability for the magazine to be released with a controller")]
        public bool TogglesMagReleaseButton;
        bool startMagRelease;
        [Header("Bolt-Action Specific Modifications")]
        [Tooltip("Changes the BaseRotOffset of the BoltHandle. Unattached BaseRotOffset is set to current firearm's BaseRotOffset on start")]
        public bool ChangesBaseRotOffset;
        public float BaseRotOffsetAttached;
        float startBaseRotOffset;
        #endregion
#if !DEBUG

        public void Start()
        {
            startRoundType = FireArm.RoundType;
            startMagType = FireArm.MagazineType;

            StartRecoilHolderPos = FireArm.RecoilingPoseHolder.transform.localPosition;
            StartPoseOverridePos = FireArm.PoseOverride.transform.localPosition;
            StartPoseOverrideRot = FireArm.PoseOverride.transform.localRotation;
            StartPoseOverrideTouchPos = FireArm.PoseOverride_Touch.transform.localPosition;
            StartPoseOverrideTouchRot = FireArm.PoseOverride_Touch.transform.localRotation;

            if (FireArm is ClosedBoltWeapon cbw)
            {
                startBoltRelease = cbw.HasBoltReleaseButton;
                startMagRelease = cbw.HasMagReleaseButton;
            }
            else if (FireArm is BoltActionRifle bar)
            {
                startBaseRotOffset = bar.BoltHandle.BaseRotOffset;
            }


        }
        public void FixedUpdate()
        {
            Debug.Log(FireArm.m_recoilLinearXYBase);
            Debug.Log(FireArm.m_recoilPoseHolderLocalPosStart);
            //Non-functional, shelved for now
            //if (ModifyOnHover)
            //{
            //    if (AttachmentMount.DisableOnHover.activeInHierarchy == false)
            //    {
            //        WhileAttached();
            //        return;
            //    }
            //    else
            //    {
            //        WhileDetached();
            //        return;
            //    }
            //}

            if (OnlyModifyOnSpecificAttachment && AttachmentMount.HasAttachmentsOnIt())
            {
                foreach (FVRFireArmAttachment attachment in AttachmentMount.AttachmentsList)
                {
                    if (ItemIdsToModifyOn.Contains(attachment.ObjectWrapper.ItemID))
                    {
                        WhileAttached();
                        return;
                    }
                    WhileDetached();
                    return;
                }
            }
            if (AttachmentMount.HasAttachmentsOnIt())
            {
                WhileAttached();
                return;
            }
            WhileDetached();
        }
        public void WhileAttached()
        {
            if (HasModifiedGun) { return; }
            HasDemodifiedGun = false;
            HasModifiedGun = true;
            //Generic Modifications
            if (DisablesObjects)
            {
                foreach (GameObject gobj in ObjectsToDisable)
                {
                    ModifyOnAttached moa = gobj.GetComponent<ModifyOnAttached>();
                    if (DisableDetachesOtherModifyOnAttached && moa != null)
                    {
                        //perhaps put an if statement in here with the Hover stuff?
                        moa.WhileDetached();
                    }
                    gobj.SetActive(false);
                }
            }
            if (EnablesObjects)
            {
                foreach (GameObject gobj in ObjectsToEnable)
                {
                    gobj.SetActive(true);
                }
            }
            if (ChangesRoundType)
            {
                foreach (FVRFireArmChamber chamber in FireArm.GetChambers())
                {
                    chamber.RoundType = RoundType;
                }
                FireArm.RoundType = RoundType;
            }
            if (ChangesMagType)
            {
                if (FireArm.Magazine != null && !FireArm.Magazine.IsIntegrated) //drops mag on magtype change
                {
                    FireArm.EjectMag();
                }
                FireArm.MagazineType = MagType;
            }
            if (ChangesMagPose)
            {
                FireArm.MagazineMountPos = NewMagMountPose;
                FireArm.MagazineEjectPos = NewMagEjectPose;
            }
            if (ChangesPoseOverride)
            {
                FireArm.m_recoilLinearXYBase = new Vector2(NewRecoilHolder.localPosition.x, NewRecoilHolder.localPosition.y);
                FireArm.m_recoilPoseHolderLocalPosStart = NewRecoilHolder.localPosition;
                FireArm.RecoilingPoseHolder.localPosition = NewRecoilHolder.localPosition;
                FireArm.PoseOverride.localPosition = NewPoseOverride.localPosition;
                FireArm.PoseOverride.localRotation = NewPoseOverride.localRotation;
                FireArm.PoseOverride_Touch.localPosition = NewPoseOverrideTouch.localPosition;
                FireArm.PoseOverride_Touch.localRotation = NewPoseOverrideTouch.localRotation;
            }
            //Specific Firearm Type Modifications
            if (FireArm is ClosedBoltWeapon cbw)
            {
                if (TogglesBoltReleaseButton)
                {
                    cbw.HasBoltReleaseButton = !startBoltRelease;
                }

                if (TogglesMagReleaseButton)
                {
                    cbw.HasMagReleaseButton = !startMagRelease;
                }
            }
            else if (FireArm is BoltActionRifle bar)
            {
                if (ChangesBaseRotOffset)
                {
                    bar.BoltHandle.BaseRotOffset = BaseRotOffsetAttached;
                }
            }
        }
        public void WhileDetached()
        {
            if (HasDemodifiedGun) { return; }
            HasDemodifiedGun = true;
            HasModifiedGun = false;
            //Generic Modifications
            if (DisablesObjects)
            {
                foreach (GameObject gobj in ObjectsToDisable)
                {
                    gobj.SetActive(true);
                }
            }
            if (EnablesObjects)
            {
                foreach (GameObject gobj in ObjectsToEnable)
                {
                    ModifyOnAttached moa = gobj.GetComponent<ModifyOnAttached>();
                    if (DisableDetachesOtherModifyOnAttached && moa != null)
                    {
                        moa.WhileDetached();
                    }
                    gobj.SetActive(false);
                }
            }
            if (ChangesRoundType)
            {
                FireArm.RoundType = startRoundType;
                foreach (FVRFireArmChamber chamber in FireArm.GetChambers())
                {
                    chamber.RoundType = startRoundType;
                }
            }
            if (ChangesMagType)
            {
                if (AttachmentMount.HasAttachmentsOnIt() && FireArm.Magazine != null && !FireArm.Magazine.IsIntegrated) //drops mag on magtype change
                {
                    FireArm.EjectMag();
                }
                FireArm.MagazineType = startMagType;
            }
            if (ChangesMagPose)
            {
                FireArm.MagazineMountPos = OriginalMagMountPose;
                FireArm.MagazineEjectPos = OriginalMagEjectPose;
            }
            if (ChangesPoseOverride)
            {
                FireArm.m_recoilLinearXYBase = new Vector2(StartRecoilHolderPos.x, StartRecoilHolderPos.y);
                FireArm.m_recoilPoseHolderLocalPosStart = StartRecoilHolderPos;
                FireArm.RecoilingPoseHolder.localPosition = StartRecoilHolderPos;
                FireArm.PoseOverride.localPosition = StartPoseOverridePos;
                FireArm.PoseOverride.localRotation = StartPoseOverrideRot;
                FireArm.PoseOverride_Touch.localPosition = StartPoseOverrideTouchPos;
                FireArm.PoseOverride_Touch.localRotation = StartPoseOverrideTouchRot;

            }
            //Specific Firearm Type Modifications
            if (FireArm is ClosedBoltWeapon cbw)
            {

                if (TogglesBoltReleaseButton)
                {
                    cbw.HasBoltReleaseButton = startBoltRelease;
                }
                if (TogglesMagReleaseButton)
                {
                    cbw.HasMagReleaseButton = startMagRelease;
                }

            }
            else if (FireArm is BoltActionRifle bar)
            {
                if (ChangesBaseRotOffset)
                {
                    bar.BoltHandle.BaseRotOffset = startBaseRotOffset;
                }
            }



        }
#endif
    }
}
