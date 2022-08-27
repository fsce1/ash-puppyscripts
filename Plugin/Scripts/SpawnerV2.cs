using System;
using BepInEx;
using FistVR;
using Sodalite;
using Sodalite.Api;
using UnityEngine;
namespace ashpuppyscripts
{
    [BepInPlugin("maiq.PortableItemSpawner", "PortableItemSpawner", "1.5.0")]
    [BepInDependency("nrgill28.Sodalite")]
    [BepInProcess("h3vr.exe")]
    public class SpawnerV2 : BaseUnityPlugin
    {


        public FVRPhysicalObject PortableItemSpawner
        {
            get
            {
                if (this._portableItemSpawner != null)
                {
                    return this._portableItemSpawner;
                }
                GameObject cleanLockablePanel = LockablePanelAPI.GetCleanLockablePanel();
                cleanLockablePanel.name = "PortableItemSpawner";
                Transform transform = cleanLockablePanel.transform;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                Transform transform2 = transform.Find("LockButton");
                transform2.localPosition = new Vector3(0.2185f, 0.1135f, -0.12f);
                transform2.rotation = Quaternion.Euler(270f, 0f, 180f);
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._itemSpawner, new Vector3(0f, -0.021f, -0.1775f), Quaternion.Euler(270f, 180f, 0f), transform);
                gameObject.transform.localScale = new Vector3(0.47f, 0.47f, 0.47f);
                UnityEngine.Object.Destroy(gameObject.transform.Find("ItemSpawnerMover").gameObject);
                this._portableItemSpawner = cleanLockablePanel.GetComponent<FVRPhysicalObject>();
                this._portableItemSpawner.SetIsKinematicLocked(true);
                this._portableItemSpawner.m_colliders = this._portableItemSpawner.GetComponentsInChildren<Collider>(true);
                this._portableItemSpawner.PoseOverride.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                return this._portableItemSpawner;
            }
        }
        public void Start()
        {
            WristMenuAPI.Buttons.Add(new WristMenuButton("Spawn ItemSpawner Panel", new ButtonClickEvent(this.SpawnItemSpawner)));
            this.Hook();
        }
        private void Hook()
        {
            ItemSpawnerUI.Start += delegate (ItemSpawnerUI.orig_Start orig, ItemSpawnerUI self)
            {
                orig.Invoke(self);
                this._itemSpawner = self.transform.parent.parent.gameObject;
            };
            TNH_Manager.PlayerDied += delegate (TNH_Manager.orig_PlayerDied orig, TNH_Manager self)
            {
                orig.Invoke(self);
                self.ItemSpawner.SetActive(true);
            };
        }
        private void SpawnItemSpawner(object sender, ButtonClickEventArgs args)
        {
            FVRWristMenu instance = WristMenuAPI.Instance;
            if (this._itemSpawner != null)
            {
                instance.m_currentHand.RetrieveObject(this.PortableItemSpawner);
                return;
            }
            instance.Aud.PlayOneShot(instance.AudClip_Err);
        }
        private GameObject _itemSpawner;
        private FVRPhysicalObject _portableItemSpawner;
    }
}