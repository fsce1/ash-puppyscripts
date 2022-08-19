using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
namespace ashpuppyscripts
{
    public class MagazineBox : FVRFireArmMagazine
    {
        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            if (!CanManuallyEjectRounds || !(RoundEjectionPos != null) || !HasARound())
                return;
            bool flag = false;
            if (m_hand.IsInStreamlinedMode && m_hand.Input.BYButtonDown)
                flag = true;
            else if (!m_hand.IsInStreamlinedMode && m_hand.Input.TouchpadDown && Vector2.Angle(m_hand.Input.TouchpadAxes, Vector2.down) < 45.0)
                flag = true;
            if (!flag)
                return;


            if (m_hand.OtherHand.CurrentInteractable == null && m_hand.OtherHand.Input.IsGrabbing && Vector3.Distance(RoundEjectionPos.position, m_hand.OtherHand.Input.Pos) < 0.150000005960464)
            {
                FVRFireArmRound component = Instantiate(RemoveRound(false), RoundEjectionPos.position, RoundEjectionPos.rotation).GetComponent<FVRFireArmRound>();
                component.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
                m_hand.OtherHand.ForceSetInteractable((FVRInteractiveObject)component);
                component.BeginInteraction(m_hand.OtherHand);

                if (m_hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).RoundType == RoundType && ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).MaxPalmedAmount && (double)Vector3.Distance(m_hand.Input.Pos, m_hand.OtherHand.Input.Pos) < 0.2
                                                                                                                                                                                                                                                                                                                         )
                {
                    while (m_numRounds != 0 && ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).MaxPalmedAmount)
                    {

                        FireArmRoundClass lrClass = LoadedRounds[m_numRounds - 1].LR_Class;
                        FVRObject lrObjectWrapper = LoadedRounds[m_numRounds - 1].LR_ObjectWrapper;
                        ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).AddProxy(lrClass, lrObjectWrapper);
                        ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
                        RemoveRound();

                    }
                    if (FireArm != null)
                        FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
                    else
                        SM.PlayGenericSound(Profile.MagazineEjectRound, transform.position);
                }
            }
            else if (m_hand.OtherHand.CurrentInteractable is FVRFireArmRound && ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).RoundType == RoundType && ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).MaxPalmedAmount && (double)Vector3.Distance(m_hand.Input.Pos, m_hand.OtherHand.Input.Pos) < 0.2
                                                                                                                                                                                                                                                                                                                         )
            {
                while (m_numRounds != 0 && ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).ProxyRounds.Count < ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).MaxPalmedAmount)
                {

                    FireArmRoundClass lrClass = LoadedRounds[m_numRounds - 1].LR_Class;
                    FVRObject lrObjectWrapper = LoadedRounds[m_numRounds - 1].LR_ObjectWrapper;
                    ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).AddProxy(lrClass, lrObjectWrapper);
                    ((FVRFireArmRound)m_hand.OtherHand.CurrentInteractable).UpdateProxyDisplay();
                    RemoveRound();


                }
                if (FireArm != null)
                    FireArm.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
                else
                    SM.PlayGenericSound(Profile.MagazineEjectRound, transform.position);
            }
        }


    }
}