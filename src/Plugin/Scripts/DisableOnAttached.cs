using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
namespace ashpuppyscripts
{
    public class DisableOnAttached : MonoBehaviour
    {
        public List<GameObject> ObjectsToDisable;
        public FVRFireArmAttachmentMount AttachmentMount;
        public void FixedUpdate()
        {
            foreach (GameObject gobj in ObjectsToDisable)
            {
                gobj.SetActive(!AttachmentMount.HasAttachmentsOnIt());
            }

        }

    }
}