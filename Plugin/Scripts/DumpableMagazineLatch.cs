using FistVR;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cityrobo;
namespace ashpuppyscripts
{ //to-do, fix timer
    public class DumpableMagazineLatch : FVRInteractiveObject
    {
        public FVRFireArmMagazine Mag;
        public Rigidbody HingeJointRigidBody;
        public MovableWeaponPart MovableWeaponPart;
        public float EjectionSpeedInMs;
        public Transform EjectionPoint;
        public AudioSource AudioSource;
        public AudioClip LatchOpen;
        public AudioClip LatchClose;

        bool IsLocked = true;
        private float StartXRot;
        private System.Timers.Timer t;
        [HideInInspector]
        public bool DebugMovableWeaponPartHeld = false;
        [HideInInspector]
        public int DebugRoundCount;
        [HideInInspector]
        public bool UsesDebug = false;


        public Transform MovingLatch;
        public Vector3 StartPos;
        public Vector3 StopPos;
        public float MoveSpeed;
        private bool animateLatch = false;
        private bool hasHitStopPoint = false;



#if !DEBUG
        public override void Start()
        {
            base.Start();
            this.IsSimpleInteract = true;
            StartXRot = HingeJointRigidBody.gameObject.transform.localEulerAngles.x;
            MovableWeaponPart.SetAllCollidersToLayer(true, "NoCol");
            HingeJointRigidBody.isKinematic = true;
            if (!UsesDebug) { DebugMovableWeaponPartHeld = false; }
            t = new System.Timers.Timer(EjectionSpeedInMs);
            t.Elapsed += EjectRound();
            t.AutoReset = true;
            t.Enabled = true;
        }



        public void FixedUpdate()
        {


            if (animateLatch && MovingLatch != null)
            {
                if (!hasHitStopPoint)
                {
                    if (MovingLatch.localPosition != StopPos)
                    {
                        //Debug.Log("A");
                        MovingLatch.localPosition = Vector3.MoveTowards(MovingLatch.localPosition, StopPos, MoveSpeed * Time.deltaTime);
                    }
                    else { hasHitStopPoint = true; }
                }
                else if (MovingLatch.localPosition != StartPos)
                {
                    //Debug.Log("B");
                    MovingLatch.localPosition = Vector3.MoveTowards(MovingLatch.localPosition, StartPos, MoveSpeed * Time.deltaTime);
                }
                else { animateLatch = false; hasHitStopPoint = false; }
            }//Moving Part Code, maybe can be cleaned up a bit more?



            this.SetAllCollidersToLayer(true, "NoCol");
            if (IsLocked) { this.SetAllCollidersToLayer(true, "Interactable"); }

            if (MovableWeaponPart.IsHeld || DebugMovableWeaponPartHeld)
            {
                HingeJointRigidBody.isKinematic = true;
                if (HingeJointRigidBody.gameObject.transform.localEulerAngles.x == StartXRot)
                {
                    //possibly re-write so all of the locking stuff happens in the if(IsLocked) bool instead of inside here (so its cleaner)
                    IsLocked = true;
                    MovableWeaponPart.SetAllCollidersToLayer(true, "NoCol");
                    MovableWeaponPart.ForceBreakInteraction();
                    if (AudioSource != null) { AudioSource.PlayOneShot(LatchClose); }
                    return;
                }
                return;
            }
            else if (!IsLocked) { HingeJointRigidBody.isKinematic = false; }
        }
        public override void SimpleInteraction(FVRViveHand hand) //When Latch is pressed
        {
            base.SimpleInteraction(hand);
            animateLatch = true;
            hasHitStopPoint = false;

            IsLocked = false;
            MovableWeaponPart.SetAllCollidersToLayer(true, "Interactable");
            HingeJointRigidBody.isKinematic = false;
            if (AudioSource != null) { AudioSource.PlayOneShot(LatchOpen); }

            t.Start();
        }
        public System.Timers.ElapsedEventHandler EjectRound()
        {
            if (!UsesDebug)
            {
                if (Mag.HasARound() && !IsLocked)
                {
                    GameObject Prefab = Mag.RemoveRound(true);
                    Instantiate(Prefab, EjectionPoint);
                    Prefab.transform.parent = null;
                    t.Start();
                    return null;
                }
                t.Stop();
                return null;
            }
            if (DebugRoundCount > 0 && !IsLocked)
            {
                Debug.Log("Ejected One Round!");
                DebugRoundCount -= 1;
                t.Start();
                return null;
            }
            t.Stop();
            return null;
        }


#endif
    }

}