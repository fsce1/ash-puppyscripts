using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
namespace ashpuppyscripts
{
    public class VariableFireRateOpenBolt : MonoBehaviour
    {
        public OpenBoltReceiverBolt OpenBolt;
        public float minSpringStiffness = 100;
        public float maxSpringStiffness = 1000;
        private bool HasChanged = false;
#if !DEBUG
        public void FixedUpdate()
        {
            if (!HasChanged && OpenBolt.transform.position == OpenBolt.Point_Bolt_Forward.position)
            {
                HasChanged = true;
                OpenBolt.BoltSpringStiffness = Random.Range(minSpringStiffness, maxSpringStiffness);

            }
            else if (HasChanged && OpenBolt.transform.position != OpenBolt.Point_Bolt_Forward.position)
            {
                HasChanged = false;
            }
        }
#endif
    }

}