using FistVR;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cityrobo;
namespace ashpuppyscripts
{ //to-do, fix timer
    public class DumpableMagazineLatch : FVRInteractiveObject
    {
        // Essential Vars
        public FVRFireArmMagazine Mag;
        public Rigidbody HingeJointRigidBody;
        public MovableWeaponPart MovableWeaponPart;
        public float EjectionSpeedInMs;
        public Transform EjectionPoint;
        // Audio Vars
        public AudioSource AudioSource;
        public AudioClip LatchOpen;
        public AudioClip LatchClose;
        // Moving Latch Vars
        public Transform MovingLatch;
        public Vector3 StartPos;
        public Vector3 StopPos;
        public float MoveSpeed;
        private bool animateLatch = false;
        private bool hasHitStopPoint = false;
        // Private / Debug Vars
        bool IsLocked = true;
        private float StartXRot;
        private System.Timers.Timer t;
        [HideInInspector]
        public bool DebugMovableWeaponPartHeld = false;
        [HideInInspector]
        public int DebugRoundCount;
        [HideInInspector]
        public bool UsesDebug = false;

#if !DEBUG
        public override void Start()//Start Params
        {
            base.Start();

            this.IsSimpleInteract = true;
            StartXRot = HingeJointRigidBody.gameObject.transform.localEulerAngles.x;
            MovableWeaponPart.SetAllCollidersToLayer(true, "NoCol");
            HingeJointRigidBody.isKinematic = true;

            if (!UsesDebug) { DebugMovableWeaponPartHeld = false; }

            t = new System.Timers.Timer(EjectionSpeedInMs);
            t.Elapsed += TimerElapsed;
            t.AutoReset = true;
            t.Enabled = true;
        }
        public virtual void FVRLateUpdate()//Previously FixedUpdate()
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

            //if (IsLocked) this.SetAllCollidersToLayer(true, "Interactable");
            //else this.SetAllCollidersToLayer(true, "NoCol");

            if (MovableWeaponPart.IsHeld || DebugMovableWeaponPartHeld)
            {
                HingeJointRigidBody.isKinematic = true; //Allows grabbing to properly move latch
                if (HingeJointRigidBody.gameObject.transform.localEulerAngles.x == StartXRot) //
                {
                    IsLocked = true; //Locks latch
                    MovableWeaponPart.SetAllCollidersToLayer(true, "NoCol");
                    MovableWeaponPart.ForceBreakInteraction();
                    if (AudioSource != null) AudioSource.PlayOneShot(LatchClose);
                }
            }
            else if (!IsLocked) { HingeJointRigidBody.isKinematic = false; this.SetAllCollidersToLayer(true, "NoCol"); }
            else this.SetAllCollidersToLayer(true, "Interactable");
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
        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)//When timer elapsed
        {
            if (!UsesDebug)
            {
                if (Mag.HasARound() && !IsLocked)
                {
                    GameObject roundPrefab = Instantiate(Mag.RemoveRound(false), EjectionPoint.position, EjectionPoint.rotation);/*Mag.RemoveRound(true);*/
                    //Instantiate(Prefab, EjectionPoint);
                    //Prefab.transform.parent = null;
                    //t.Start();
                }
                else { t.Stop(); }
            }
            if (DebugRoundCount > 0 && !IsLocked)
            {
                Debug.Log("Ejected One Round!");
                DebugRoundCount -= 1;
                //t.Start();
            }
            else { t.Stop(); }
        }
#endif
    }

}