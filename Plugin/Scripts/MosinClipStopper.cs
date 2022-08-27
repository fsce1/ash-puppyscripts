using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
namespace ashpuppyscripts
{
    public class MosinClipStopper : MonoBehaviour
    {
        public BoltActionRifle BAR;
        private BoltActionRifle_Handle BARH;

        public void Start()
        {
            BARH = BAR.BoltHandle;
        }
        public void FixedUpdate()
        {
            if (BAR.Clip != null)
            {
                BARH.SetAllCollidersToLayer(true, "NoCol");
                return;
            }
            BARH.SetAllCollidersToLayer(true, "Interactable");
        }
    }
}