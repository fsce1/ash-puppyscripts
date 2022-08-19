using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using FistVR;
#if RELEASE
namespace ashpuppyscripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ModifyOnAttached))]
    public class ModifyOnAttachedEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var screenRect = GUILayoutUtility.GetRect(1, 1);
            var vertRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(new Rect(screenRect.x - 13, screenRect.y + 4, screenRect.width + 17, vertRect.height + 6), new Color(201f / 255f, 179f / 255f, 201f / 255f));
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
            ModifyOnAttached moa = (ModifyOnAttached)base.target;

            EditorGUILayout.LabelField("Modify On Attached", GUIHeader);
            EditorGUILayout.LabelField("Please Read Tooltips!", GUIHeader);

            moa.AttachmentMount = (FVRFireArmAttachmentMount)EditorGUILayout.ObjectField(new GUIContent("Attachment Mount", "The Attachment Mount you want to read Attachments from."), moa.AttachmentMount, typeof(FVRFireArmAttachmentMount), true);
            moa.FireArm = (FVRFireArm)EditorGUILayout.ObjectField(new GUIContent("Firearm", "The Firearm you want to affect on attach."), moa.FireArm, typeof(FVRFireArm), true);
            if (moa.AttachmentMount == null || moa.FireArm == null)
            {
                EditorGUILayout.LabelField("Add an Attachment Mount and Firearm", GUIHeader);
                return;
            }
            //Non-functional, shelved for now
            //moa.ModifyOnHover = EditorGUILayout.Toggle(new GUIContent("Modify on Hover", "Vanilla-like behaviour of modifying the weapon when the attachment visually snaps to the mount, instead of after you let go of attachment."), moa.ModifyOnHover);

            //if (moa.ModifyOnHover && moa.AttachmentMount.DisableOnHover == null)
            //{
            //    EditorGUILayout.HelpBox("Modify on Hover requires the Vanilla Atachment Mount to have a Hover Disable Piece (It can just be an Empty GameObject).", MessageType.Error);
            //}
            moa.OnlyModifyOnSpecificAttachment = EditorGUILayout.Toggle(new GUIContent("Only Modify on Specific Attachment", "Only modify weapon when specific attachments (set in the ItemIDsToModifyOn List) are attached to the gun. Allows some attachments Modify the gun, while others do nothing."), moa.OnlyModifyOnSpecificAttachment);
            if (moa.OnlyModifyOnSpecificAttachment)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemIdsToModifyOn"), true);
                EditorGUI.indentLevel = 0;
            }
            moa.DisableDetachesOtherModifyOnAttached = EditorGUILayout.Toggle(new GUIContent("Disable Detaches Other ModifyOnAttached", "If ObjectsToDisable/Enable references another instance of a ModifyOnAttached Script, that script will run the Detach code (resetting values to default) when it is disabled by this script."), moa.DisableDetachesOtherModifyOnAttached);
            EditorGUILayout.LabelField("Generic Modifications", GUIHeader);

            #region Disables/Enables Objects
            moa.DisablesObjects = EditorGUILayout.Toggle(new GUIContent("Disables Objects", "GameObjects placed into the ObjectsToDisable list will be Disabled when attachment is attached, and Enabled when detached."), moa.DisablesObjects);
            if (moa.DisablesObjects)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectsToDisable"), true);
                EditorGUI.indentLevel = 0;
            }
            moa.EnablesObjects = EditorGUILayout.Toggle(new GUIContent("Enables Objects", "GameObjects placed into the ObjectsToEnable list will be Enabled when attachment is attached, and Disabled when detached."), moa.EnablesObjects);
            if (moa.EnablesObjects)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ObjectsToEnable"), true);
                EditorGUI.indentLevel = 0;
            }
            #endregion
            #region Round+Mag Type
            moa.ChangesRoundType = EditorGUILayout.Toggle(new GUIContent("Changes Round Type", "Changes the chamber and firearm round types (for manually chambering rounds)"), moa.ChangesRoundType);
            if (moa.ChangesRoundType)
            {
                EditorGUI.indentLevel = 2;
                moa.RoundType = (FireArmRoundType)EditorGUILayout.EnumPopup(moa.RoundType);
                EditorGUI.indentLevel = 0;
            }
            moa.ChangesMagType = EditorGUILayout.Toggle(new GUIContent("Changes Magazine Type", "Changes the firearm's magazine type"), moa.ChangesMagType);
            if (moa.ChangesMagType)
            {
                EditorGUI.indentLevel = 2;
                moa.MagType = (FireArmMagazineType)EditorGUILayout.EnumPopup(moa.MagType);
                EditorGUI.indentLevel = 0;
            }
            moa.ChangesMagPose = EditorGUILayout.Toggle(new GUIContent("Changes Magazine Pose", "Changes the reference for the Firearms Magazine_Mount and Magazine_Eject between 2 different GameObjects."), moa.ChangesMagPose);
            if (moa.ChangesMagPose)
            {
                EditorGUI.indentLevel = 2;
                moa.OriginalMagMountPose = (Transform)EditorGUILayout.ObjectField(new GUIContent("Original Mount Pos"), moa.OriginalMagMountPose, typeof(Transform), true);
                moa.NewMagMountPose = (Transform)EditorGUILayout.ObjectField(new GUIContent("New Mount Pos"), moa.NewMagMountPose, typeof(Transform), true);
                EditorGUILayout.LabelField("\n");
                moa.OriginalMagEjectPose = (Transform)EditorGUILayout.ObjectField(new GUIContent("Original Eject Pos"), moa.OriginalMagEjectPose, typeof(Transform), true);
                moa.NewMagEjectPose = (Transform)EditorGUILayout.ObjectField(new GUIContent("New Eject Pos"), moa.NewMagEjectPose, typeof(Transform), true);
                EditorGUI.indentLevel = 0;

            }
            #endregion
            moa.ChangesPoseOverride = EditorGUILayout.Toggle(new GUIContent("Changes PoseOverrides", "Changes the PoseOverride and PoseOverrideTouch on Attach, and reverts changes on Detach"), moa.ChangesPoseOverride);
            if (moa.ChangesPoseOverride)
            {
                EditorGUI.indentLevel = 2;
                moa.NewRecoilHolder = (Transform)EditorGUILayout.ObjectField(new GUIContent("Recoil Holder"), moa.NewRecoilHolder, typeof(Transform), true);
                EditorGUILayout.HelpBox("Recoil Holder can be same as Original, or a new object. Please make sure your new PoseOverrides are childs of the Recoil Holder you set in this script.", MessageType.Info);
                moa.NewPoseOverride = (Transform)EditorGUILayout.ObjectField(new GUIContent("New Pose Override"), moa.NewPoseOverride, typeof(Transform), true);
                moa.NewPoseOverrideTouch = (Transform)EditorGUILayout.ObjectField(new GUIContent("New Pose Override Touch"), moa.NewPoseOverrideTouch, typeof(Transform), true);
                EditorGUI.indentLevel = 0;
            }

            #region ChangesPoseOverride

            #endregion

            switch (moa.FireArm)
            {
                case ClosedBoltWeapon:
                    EditorGUILayout.LabelField("Closed-Bolt Specific Modifications", GUIHeader);
                    moa.TogglesBoltReleaseButton = EditorGUILayout.Toggle(new GUIContent("Toggles Bolt Release Button", "Inverts the ability for the bolt to be released with a controller"), moa.TogglesBoltReleaseButton);
                    moa.TogglesMagReleaseButton = EditorGUILayout.Toggle(new GUIContent("Toggles Magazine Release Button", "Inverts the ability for the magazine to be released with a controller"), moa.TogglesMagReleaseButton);
                    break;
                case BoltActionRifle:
                    EditorGUILayout.LabelField("Bolt-Action Specific Modifications", GUIHeader);
                    moa.ChangesBaseRotOffset = EditorGUILayout.Toggle(new GUIContent("Modifies Base Rot Offset", "Changes the BaseRotOffset value of a bolt handle and back to its original on detach."), moa.ChangesBaseRotOffset);
                    if (moa.ChangesBaseRotOffset)
                    {
                        EditorGUI.indentLevel = 2;
                        moa.BaseRotOffsetAttached = EditorGUILayout.DelayedFloatField(new GUIContent("Base Rot Offset (Attached)"), moa.BaseRotOffsetAttached);
                        EditorGUILayout.LabelField("Unattached BaseRotOffset is set to current firearm's BaseRotOffset on start", GUIHelp);
                        EditorGUI.indentLevel = 0;
                    }
                    break;
            }



            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
            //base.OnInspectorGUI();
        }

    }

}
#endif