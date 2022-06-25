using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
namespace ashpuppyscripts
{
    public class VariableFireRateClosedBolt : MonoBehaviour
    {
        public ClosedBolt ClosedBolt;
        public float minSpringStiffness = 100;
        public float maxSpringStiffness = 1000;
        private bool HasChanged = false;
        public void FixedUpdate()
        {
            if (!HasChanged && ClosedBolt.transform.position == ClosedBolt.Point_Bolt_Forward.position)
            {
                HasChanged = true;
                ClosedBolt.SpringStiffness = Random.Range(minSpringStiffness, maxSpringStiffness);

            }
            else if (HasChanged && ClosedBolt.transform.position != ClosedBolt.Point_Bolt_Forward.position)
            {
                HasChanged = false;
            }
        }

    }

}