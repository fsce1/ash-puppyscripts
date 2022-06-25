using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
namespace ashpuppyscripts
{
    public class ChangeBaseRotOffsetOnAttach : MonoBehaviour
    {
        public FVRFireArmAttachmentMount attachmentMount;
        public BoltActionRifle_Handle boltActionHandle;
        public float baseRotOffsetUnattached;
        public float baseRotOffsetAttached;
        void FixedUpdate()
        {
            if (this.attachmentMount.HasAttachmentsOnIt())
            {
                boltActionHandle.BaseRotOffset = baseRotOffsetAttached;
                return;
            }
                boltActionHandle.BaseRotOffset = baseRotOffsetUnattached;
        }
    }
}