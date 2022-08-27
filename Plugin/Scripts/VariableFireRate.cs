using FistVR;
using UnityEngine;
namespace ashpuppyscripts
{
    public class VariableFireRate : MonoBehaviour
    {
        public ClosedBolt ClosedBolt;
        public OpenBoltReceiverBolt OpenBolt;
        public float minForwardSpeed = 100;
        public float maxForwardSpeed = 1000;
        public float minRearwardSpeed = -100;
        public float maxRearwardSpeed = -1000;
        public float minSpringStiffness = 100;
        public float maxSpringStiffness = 1000;
        private bool HasChanged = false;
#if !DEBUG
        public void FixedUpdate()
        {
            if (ClosedBolt != null)
            {
                if (!HasChanged && ClosedBolt.transform.position == ClosedBolt.Point_Bolt_Forward.position)
                {
                    ClosedBolt.SpringStiffness = Random.Range(minSpringStiffness, maxSpringStiffness);
                    ClosedBolt.Speed_Forward = Random.Range(minForwardSpeed, maxForwardSpeed);
                    ClosedBolt.Speed_Rearward = Random.Range(minRearwardSpeed, maxRearwardSpeed);
                    HasChanged = true;
                    return;
                }
                else if (HasChanged && ClosedBolt.transform.position != ClosedBolt.Point_Bolt_Forward.position)
                {
                    HasChanged = false;
                    return;
                }

            }
            else if(OpenBolt != null)
            {
                if (!HasChanged && OpenBolt.transform.position == OpenBolt.Point_Bolt_Forward.position)
                {
                    OpenBolt.BoltSpringStiffness = Random.Range(minSpringStiffness, maxSpringStiffness);
                    OpenBolt.BoltSpeed_Forward = Random.Range(minForwardSpeed, maxForwardSpeed);
                    OpenBolt.BoltSpeed_Rearward = Random.Range(minRearwardSpeed, maxRearwardSpeed);
                    HasChanged = true;
                    return;
                }
                else if (HasChanged && OpenBolt.transform.position != OpenBolt.Point_Bolt_Forward.position)
                {
                    HasChanged = false;
                    return;
                }
            }
            else { Debug.Log("No Bolt assigned to VariableFireRate.cs!"); }
        }
#endif
    }

}