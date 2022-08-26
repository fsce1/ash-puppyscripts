using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using FistVR;
using Cityrobo;
#if RELEASE
namespace ashpuppyscripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DumpableMagazineLatch))]
    public class DumpableMagazineLatchEditor : Editor
    {

        public void CreateHingeObject()
        {
            DumpableMagazineLatch dml = (DumpableMagazineLatch)base.target;
            GameObject hingeJoint = new("Hinge Joint");
            hingeJoint.transform.SetParent(dml.transform, false);


            BoxCollider collider = hingeJoint.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.05f, 0.05f, 0.075f);
            collider.center = new Vector3(0f, 0f, 0.0375f);
            collider.isTrigger = true;

            HingeJoint joint = hingeJoint.AddComponent<HingeJoint>();
            joint.enableCollision = true;
            joint.connectedBody = dml.Mag.GetComponent<Rigidbody>();
            dml.HingeJointRigidBody = joint.GetComponent<Rigidbody>();
            //joint.useSpring = true;
            //joint.spring.spring = 0.075f;
            //joint.spring.damper = 0.001f;
            //joint.spring.targetPosition = 90f;

            MovableWeaponPart mwp = hingeJoint.AddComponent<MovableWeaponPart>();
            dml.MovableWeaponPart = mwp;
            mwp.root = dml.Mag.transform;
            mwp.objectToMove = mwp.transform;
            mwp.upperLimit = 90f;
            mwp.mode = MovableWeaponPart.Mode.Rotation;

            GameObject meshHelper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            meshHelper.name = "Hinge Mesh Goes Here (Replace Me!)";
            meshHelper.transform.SetParent(hingeJoint.transform, false);
            meshHelper.transform.localScale = new Vector3(0.05f, 0.05f, 0.075f);
            meshHelper.transform.localPosition = new Vector3(0f, 0f, 0.0375f);

            //dml.AudioSource = dml.gameObject.GetComponent<AudioSource>(); //Unrelated to Creating HingeJoint
        }
        public void AddBoxCollider()
        {
            DumpableMagazineLatch dml = (DumpableMagazineLatch)base.target;

            BoxCollider boxCollider = dml.gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.025f, 0.025f, 0.025f);
            boxCollider.isTrigger = true;
        }
        public override void OnInspectorGUI()
        {
            #region GUIHeader
            GUIStyle GUIHeader = new GUIStyle(EditorStyles.boldLabel);
            GUIHeader.fontStyle = FontStyle.Bold;
            GUIHeader.fontSize = 15;
            GUIHeader.fixedHeight = 50;
            GUIHeader.alignment = TextAnchor.UpperCenter;
            #endregion
            #region GUIHelp
            GUIStyle GUIHelp = new GUIStyle(EditorStyles.helpBox);
            //GUIHelp.font = Font(Mac11_Stock); lol
            #endregion
            serializedObject.Update();
            var screenRect = GUILayoutUtility.GetRect(1, 1);
            var vertRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(new Rect(screenRect.x - 13, screenRect.y + 4, screenRect.width + 17, vertRect.height + 6), new Color(201f / 255f, 179f / 255f, 201f / 255f));
            DumpableMagazineLatch dml = (DumpableMagazineLatch)base.target;

            EditorGUILayout.LabelField("Dumpable Magazine Latch", GUIHeader);
            EditorGUILayout.LabelField("Please Read Tooltips!", GUIHeader);
            EditorGUILayout.LabelField("\n");

            if(dml.gameObject.GetComponent<BoxCollider>() == null)
            {
                EditorGUILayout.LabelField("Script object must have a release button collider!", GUIHelp);
                if (GUILayout.Button("Add Release Button Box Collider")){ AddBoxCollider(); }
            }


            dml.Mag = (FVRFireArmMagazine)EditorGUILayout.ObjectField(new GUIContent("Magazine", "The Magazine that gets dumped."), dml.Mag, typeof(FVRFireArmMagazine), true);
            dml.HingeJointRigidBody = (Rigidbody)EditorGUILayout.ObjectField(new GUIContent("Hinge Joint's RigidBody", "The RigidBody that is attached to the Hinge Joint."), dml.HingeJointRigidBody, typeof(Rigidbody), true);
            dml.MovableWeaponPart = (MovableWeaponPart)EditorGUILayout.ObjectField(new GUIContent("Movable Weapon Part", "Openscripts script that controls picking up mag latch once it is dropped"), dml.MovableWeaponPart, typeof(MovableWeaponPart), true);
            
            if (dml.HingeJointRigidBody == null && dml.MovableWeaponPart == null && dml.Mag != null)
            {
                if (GUILayout.Button("Create Generic Hinge Joint"))
                {
                    CreateHingeObject();
                }
            }

            dml.EjectionPoint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Ejection Point", "Point at which round get ejected. Does NOT have to be the same as the mag's existing Ejection Point."), dml.EjectionPoint, typeof(Transform), true);
            dml.EjectionSpeedInMs = EditorGUILayout.FloatField(new GUIContent("Ejection Speed (in ms)", "Time to wait in-between each round being ejected in milliseconds."), dml.EjectionSpeedInMs);

            EditorGUILayout.LabelField("Sound Config", GUIHeader);
            EditorGUILayout.LabelField("\n");
            dml.AudioSource = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Audio Source"), dml.AudioSource, typeof(AudioSource), true);
            if(dml.AudioSource != null) //add errors if these are null to stop nullref errors
            {
            dml.LatchOpen = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Latch Opening"), dml.LatchOpen, typeof(AudioClip), true);
            dml.LatchClose = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Latch Closing"), dml.LatchClose, typeof(AudioClip), true);
            }

            EditorGUILayout.LabelField("Moving Part Config", GUIHeader);
            EditorGUILayout.LabelField("\n");
            dml.MovingLatch = (Transform)EditorGUILayout.ObjectField(new GUIContent("Moving Latch", "Mesh that gets moved to simulate latch moving."), dml.MovingLatch, typeof(Transform), true);
            if (dml.MovingLatch != null)
            {
                dml.StartPos = EditorGUILayout.Vector3Field(new GUIContent("Start Pos"), dml.StartPos);
                dml.StopPos = EditorGUILayout.Vector3Field(new GUIContent("Stop Pos"), dml.StopPos);
                dml.MoveSpeed = EditorGUILayout.DelayedFloatField(new GUIContent("Move Speed", "Speed that the latch moves back and forth at (DEFAULT = 0.5"), dml.MoveSpeed);
            }

            EditorGUILayout.LabelField("\n");
            dml.UsesDebug = EditorGUILayout.Toggle(new GUIContent("Debug"), dml.UsesDebug);
            if (dml.UsesDebug)
            {
                EditorGUILayout.LabelField("Debug Options", GUIHeader);
                EditorGUILayout.LabelField("\n");
                if (GUILayout.Button("Dump Mag"))
                {
                    dml.SimpleInteraction(null);
                }
                dml.DebugMovableWeaponPartHeld = EditorGUILayout.Toggle(new GUIContent("Is Movable Weapon Part Held"), dml.DebugMovableWeaponPartHeld);
                dml.DebugRoundCount = EditorGUILayout.DelayedIntField(new GUIContent("Mag Round Count"), dml.DebugRoundCount);
            }

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
            //base.OnInspectorGUI();
        }

    }
}
#endif