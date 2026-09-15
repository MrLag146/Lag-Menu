using GorillaLocomotion;
using LagMenu.Menu;
using LagMenu.Patches;
using LagMenu.Utilities;
using Oculus.Platform;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static LagMenu.Patches.TorsoPatch;
using static LagMenu.Mods.Saftey;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace LagMenu.Mods
{
    internal class RIg
    {


        static bool _sub_UpsideDown, _sub_SidewaysLeft, _sub_SidewaysRight, _sub_TiltForward, _sub_TiltBackward;
        static bool _sub_SpinX, _sub_SpinY, _sub_SpinZ, _sub_Tornado, _sub_Drunk, _sub_Wobble, _sub_Shake, _sub_OrbitRoll;
        static bool _sub_BarrelRoll, _sub_Backflip;
        public static void TiltForward() { if (_sub_TiltForward) return; _sub_TiltForward = true; LagMenu.Patches.TorsoPatch.VRRigLateUpdate += VRRigLateUpdate_TiltForward; }
        public static void UnTiltForward() { if (!_sub_TiltForward) return; _sub_TiltForward = false; LagMenu.Patches.TorsoPatch.VRRigLateUpdate -= VRRigLateUpdate_TiltForward; }
        public static void VRRigLateUpdate_TiltForward() =>
           VRRig.LocalRig.transform.RotateAround(VRRig.LocalRig.bodyTransform.position, Camera.main.transform.right, 45f);
        private static VRRig GetClosestPlayer()
        {
            VRRig closestRig = null;
            float closestDistance = float.MaxValue;
            Vector3 myPosition = VRRig.LocalRig.transform.position;
            foreach (VRRig rig in VRRigCache.ActiveRigs.Where(r => r != VRRig.LocalRig))
            {
                if (rig != null)
                {
                    float distance = Vector3.Distance(myPosition, rig.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestRig = rig;
                    }
                }
            }
            return closestRig;
        }




        public static void StareAtClosestPlayer()
        {
            VRRig closestRig = GetClosestPlayer();
            if (closestRig != null)
            {
                VRRig.LocalRig.headConstraint.LookAt(closestRig.transform.position + new Vector3(0f, 0.4f, 0f));
            }
        }




        public static bool IsGhostMonkey;
        public static bool ghostToggle;
        public static Vector3 GhostPos;


        public static void GhostMonkey()
        {
            if (IsGhostMonkey)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GhostPos;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }

            bool button = ControllerInputPoller.instance.rightControllerSecondaryButton;
            if (button && !ghostToggle)
            {
                ghostToggle = true;
                IsGhostMonkey = !IsGhostMonkey;
                GhostPos = GorillaTagger.Instance.offlineVRRig.transform.position;
            }
            else if (!button)
            {
                ghostToggle = false;
            }
        }




        public static void InvisibleMonke()
        {

            if (ControllerInputPoller.instance.leftControllerPrimaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(999f, 999f, 999f);
            }
            else
            {
               GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void JumpscareGun()
        {
            GunLib.StartPointerSystem(() =>
            {

                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }


                if (GunLib.gunLocked && GunLib.lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    VRRig.LocalRig.transform.position =
                        GunLib.lockTarget.headMesh.transform.position +
                        GunLib.lockTarget.headMesh.transform.forward * Random.Range(0.1f, 0.5f);

                    VRRig.LocalRig.head.rigTarget.transform.LookAt(
                        GunLib.lockTarget.headMesh.transform.position);

                    Quaternion dirLook = VRRig.LocalRig.head.rigTarget.transform.rotation;
                    VRRig.LocalRig.transform.rotation = dirLook;

                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        GunLib.lockTarget.headMesh.transform.position +
                        GunLib.lockTarget.headMesh.transform.right * 0.2f;

                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        GunLib.lockTarget.headMesh.transform.position +
                        GunLib.lockTarget.headMesh.transform.right * -0.2f;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = dirLook;

                    Quaternion handRot = Quaternion.Euler(
                        VRRig.LocalRig.transform.rotation.eulerAngles + new Vector3(0f, 180f, 0f));

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = handRot;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = handRot;


                    VRRig.LocalRig.leftIndex.calcT = 0f;
                    VRRig.LocalRig.leftMiddle.calcT = 0f;
                    VRRig.LocalRig.leftThumb.calcT = 0f;
                    VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                    VRRig.LocalRig.rightIndex.calcT = 0f;
                    VRRig.LocalRig.rightMiddle.calcT = 0f;
                    VRRig.LocalRig.rightThumb.calcT = 0f;
                    VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }




        public static void SexGun()
        {
            var data = GunLib.ShootLock();

            if (data.isShooting && data.isLocked)
            {
                if (data.lockedPlayer != null)
                {
                    if (!data.lockedPlayer.isOfflineVRRig)
                    {
                        VRRig whoCopy = data.lockedPlayer;

                        GorillaTagger.Instance.offlineVRRig.enabled = false;

                        Vector3 offset = whoCopy.transform.forward *
                            -(0.2f + (Mathf.Sin(Time.frameCount / 8f) * 0.1f));

                        GorillaTagger.Instance.offlineVRRig.transform.position =
                            whoCopy.transform.position + offset;

                        GorillaTagger.Instance.myVRRig.transform.position =
                            whoCopy.transform.position + offset;

                        GorillaTagger.Instance.offlineVRRig.transform.rotation =
                            whoCopy.transform.rotation;

                        GorillaTagger.Instance.myVRRig.transform.rotation =
                            whoCopy.transform.rotation;

                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position =
                            (whoCopy.transform.position + whoCopy.transform.right * -0.2f) +
                            whoCopy.transform.up * -0.4f;

                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position =
                            (whoCopy.transform.position + whoCopy.transform.right * 0.2f) +
                            whoCopy.transform.up * -0.4f;

                        GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.rotation =
                            whoCopy.transform.rotation;

                        GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.rotation =
                            whoCopy.transform.rotation;

                        GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.rotation =
                            whoCopy.transform.rotation;

                        if (Time.frameCount % 45 == 0)
                        {
                            GorillaTagger.Instance.myVRRig.SendRPC(
                                "RPC_PlayHandTap",
                                RpcTarget.All,
                                new object[]
                                {
                            64,
                            false,
                            999999f
                                });

                            Vector3 pos = whoCopy.transform.position -
                                          new Vector3(0f, 0.5f, 0f);

                            GorillaTagger.Instance.myVRRig.SendRPC(
                                "RPC_PlaySplashEffect",
                                0,
                                new object[]
                                {
                            pos,
                            Quaternion.identity,
                            1.5f,
                            1.5f,
                            false,
                            true
                                });
                        }
                    }
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;

                if (GorillaLocomotion.GTPlayer.Instance != null)
                {
                    GorillaTagger.Instance.offlineVRRig.headConstraint.rotation =
                        GorillaLocomotion.GTPlayer.Instance.headCollider.transform.rotation;
                }
            }
        }

        public static void FollowGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }

                if (GunLib.gunLocked && GunLib.lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;


                    Vector3 behind = GunLib.lockTarget.transform.position
                                   - GunLib.lockTarget.transform.forward * 0.5f
                                   + Vector3.up * 0.1f;

                    VRRig.LocalRig.transform.position = Vector3.Lerp(
                        VRRig.LocalRig.transform.position,
                        behind,
                        Time.deltaTime * 8f);

                    VRRig.LocalRig.transform.LookAt(GunLib.lockTarget.headMesh.transform.position);

                    Quaternion look = VRRig.LocalRig.transform.rotation;
                    VRRig.LocalRig.head.rigTarget.transform.rotation = look;
                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = look;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = look;

                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -0.2f;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 0.2f;
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }
        public static void VRRigLateUpdate_Backflip()
        {
            if (!ControllerInputPoller.instance.rightControllerSecondaryButton) return;
            VRRig.LocalRig.transform.RotateAround(VRRig.LocalRig.bodyTransform.position, Camera.main.transform.right, (Time.time * 720f) % 360f);
        }

        public static void VRRigLateUpdate_SpinY() =>
           VRRig.LocalRig.transform.RotateAround(VRRig.LocalRig.bodyTransform.position, Vector3.up, (Time.time * 360f) % 360f);

        public static void VRRigLateUpdate_SpinX() =>
            VRRig.LocalRig.transform.RotateAround(VRRig.LocalRig.bodyTransform.position, Camera.main.transform.right, (Time.time * 360f) % 360f);

        public static void VRRigLateUpdate_SpinZ() =>
            VRRig.LocalRig.transform.RotateAround(VRRig.LocalRig.bodyTransform.position, Camera.main.transform.forward, (Time.time * 360f) % 360f);
        public static void OrbitPlayerGun()
        {
            GunLib.StartPointerSystem(() =>
            {

                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }


                if (GunLib.gunLocked && GunLib.lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    VRRig.LocalRig.transform.position =
                        GunLib.lockTarget.transform.position +
                        new Vector3(
                            Mathf.Cos(Time.frameCount / 20f),
                            0.5f,
                            Mathf.Sin(Time.frameCount / 20f));

                    VRRig.LocalRig.transform.LookAt(GunLib.lockTarget.transform.position);

                    Quaternion look = VRRig.LocalRig.transform.rotation;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = look;

                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -1f;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 1f;

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = look;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = look;


                    VRRig.LocalRig.leftIndex.calcT = 0f;
                    VRRig.LocalRig.leftMiddle.calcT = 0f;
                    VRRig.LocalRig.leftThumb.calcT = 0f;
                    VRRig.LocalRig.leftIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.leftMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.leftThumb.LerpFinger(1f, false);

                    VRRig.LocalRig.rightIndex.calcT = 0f;
                    VRRig.LocalRig.rightMiddle.calcT = 0f;
                    VRRig.LocalRig.rightThumb.calcT = 0f;
                    VRRig.LocalRig.rightIndex.LerpFinger(1f, false);
                    VRRig.LocalRig.rightMiddle.LerpFinger(1f, false);
                    VRRig.LocalRig.rightThumb.LerpFinger(1f, false);
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }





        public static void MirrorGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }

                if (GunLib.gunLocked && GunLib.lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;


                    VRRig.LocalRig.transform.position = GunLib.lockTarget.transform.position;
                    VRRig.LocalRig.transform.rotation = GunLib.lockTarget.transform.rotation;


                    VRRig.LocalRig.head.rigTarget.transform.position =
                        GunLib.lockTarget.head.rigTarget.transform.position;
                    VRRig.LocalRig.head.rigTarget.transform.rotation =
                        GunLib.lockTarget.head.rigTarget.transform.rotation;


                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        GunLib.lockTarget.leftHand.rigTarget.transform.position;
                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation =
                        GunLib.lockTarget.leftHand.rigTarget.transform.rotation;


                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        GunLib.lockTarget.rightHand.rigTarget.transform.position;
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation =
                        GunLib.lockTarget.rightHand.rigTarget.transform.rotation;


                    VRRig.LocalRig.leftIndex.calcT = GunLib.lockTarget.leftIndex.calcT;
                    VRRig.LocalRig.leftMiddle.calcT = GunLib.lockTarget.leftMiddle.calcT;
                    VRRig.LocalRig.leftThumb.calcT = GunLib.lockTarget.leftThumb.calcT;

                    VRRig.LocalRig.leftIndex.LerpFinger(GunLib.lockTarget.leftIndex.calcT, false);
                    VRRig.LocalRig.leftMiddle.LerpFinger(GunLib.lockTarget.leftMiddle.calcT, false);
                    VRRig.LocalRig.leftThumb.LerpFinger(GunLib.lockTarget.leftThumb.calcT, false);

                    VRRig.LocalRig.rightIndex.calcT = GunLib.lockTarget.rightIndex.calcT;
                    VRRig.LocalRig.rightMiddle.calcT = GunLib.lockTarget.rightMiddle.calcT;
                    VRRig.LocalRig.rightThumb.calcT = GunLib.lockTarget.rightThumb.calcT;

                    VRRig.LocalRig.rightIndex.LerpFinger(GunLib.lockTarget.rightIndex.calcT, false);
                    VRRig.LocalRig.rightMiddle.LerpFinger(GunLib.lockTarget.rightMiddle.calcT, false);
                    VRRig.LocalRig.rightThumb.LerpFinger(GunLib.lockTarget.rightThumb.calcT, false);
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }



        public static void UpsideDownHead() =>
            VRRig.LocalRig.transform.RotateAround(VRRig.LocalRig.bodyTransform.position, Camera.main.transform.forward, 180f);
        public static void ShadowCloneGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();

                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }

                if (GunLib.gunLocked && GunLib.lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    Vector3 offset = GunLib.lockTarget.transform.right * 0.9f;

                    VRRig.LocalRig.transform.position =
                        GunLib.lockTarget.transform.position + offset;

                    VRRig.LocalRig.transform.rotation =
                        GunLib.lockTarget.transform.rotation;

                    VRRig.LocalRig.head.rigTarget.transform.position =
                        GunLib.lockTarget.head.rigTarget.transform.position + offset;

                    VRRig.LocalRig.head.rigTarget.transform.rotation =
                        GunLib.lockTarget.head.rigTarget.transform.rotation;

                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        GunLib.lockTarget.leftHand.rigTarget.transform.position + offset;

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation =
                        GunLib.lockTarget.leftHand.rigTarget.transform.rotation;

                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        GunLib.lockTarget.rightHand.rigTarget.transform.position + offset;

                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation =
                        GunLib.lockTarget.rightHand.rigTarget.transform.rotation;

                    VRRig.LocalRig.leftIndex.calcT =
                        GunLib.lockTarget.leftIndex.calcT;

                    VRRig.LocalRig.leftMiddle.calcT =
                        GunLib.lockTarget.leftMiddle.calcT;

                    VRRig.LocalRig.leftThumb.calcT =
                        GunLib.lockTarget.leftThumb.calcT;

                    VRRig.LocalRig.leftIndex.LerpFinger(
                        GunLib.lockTarget.leftIndex.calcT,
                        false);

                    VRRig.LocalRig.leftMiddle.LerpFinger(
                        GunLib.lockTarget.leftMiddle.calcT,
                        false);

                    VRRig.LocalRig.leftThumb.LerpFinger(
                        GunLib.lockTarget.leftThumb.calcT,
                        false);

                    VRRig.LocalRig.rightIndex.calcT =
                        GunLib.lockTarget.rightIndex.calcT;

                    VRRig.LocalRig.rightMiddle.calcT =
                        GunLib.lockTarget.rightMiddle.calcT;

                    VRRig.LocalRig.rightThumb.calcT =
                        GunLib.lockTarget.rightThumb.calcT;

                    VRRig.LocalRig.rightIndex.LerpFinger(
                        GunLib.lockTarget.rightIndex.calcT,
                        false);

                    VRRig.LocalRig.rightMiddle.LerpFinger(
                        GunLib.lockTarget.rightMiddle.calcT,
                        false);

                    VRRig.LocalRig.rightThumb.LerpFinger(
                        GunLib.lockTarget.rightThumb.calcT,
                        false);
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }





        public enum RigMode
        {
            Skellon,

        }


        public static RigMode CurrentRigMode = RigMode.Skellon;
        public static bool PCRigEnabled;
        private static Vector3 skellonLeftPos;
        private static Vector3 skellonRightPos;

        public static void PCRig()
        {



            Vector3 originLeft = VRRig.LocalRig.bodyRenderer.transform.position +
                                 VRRig.LocalRig.bodyRenderer.transform.right * -0.25f;

            Vector3 originRight = VRRig.LocalRig.bodyRenderer.transform.position +
                                  VRRig.LocalRig.bodyRenderer.transform.right * 0.25f;

            bool leftRaycast = Physics.Raycast(originLeft, Vector3.down, out RaycastHit leftHitInfo, 0.7f,
                GTPlayer.LocomotionEnabledLayers);

            bool rightRaycast = Physics.Raycast(originRight, Vector3.down, out RaycastHit rightHitInfo, 0.7f,
                GTPlayer.LocomotionEnabledLayers);

            Vector3 leftPos = leftRaycast ? leftHitInfo.point : originLeft + Vector3.down * 0.7f;
            Vector3 rightPos = rightRaycast ? rightHitInfo.point : originRight + Vector3.down * 0.7f;

            skellonLeftPos = Vector3.Lerp(
                skellonLeftPos,
                leftPos,
                Time.deltaTime * PCRigPatch.SkellonRotationSpeed);

            skellonRightPos = Vector3.Lerp(
                skellonRightPos,
                rightPos,
                Time.deltaTime * PCRigPatch.SkellonRotationSpeed);

            GTPlayer.Instance.leftHand.controllerTransform.position = skellonLeftPos;
            GTPlayer.Instance.rightHand.controllerTransform.position = skellonRightPos;

            GTPlayer.Instance.leftHand.controllerTransform.rotation =
                VRRig.LocalRig.bodyRenderer.transform.rotation;

            GTPlayer.Instance.rightHand.controllerTransform.rotation =
                VRRig.LocalRig.bodyRenderer.transform.rotation;
        }

        public static void OnEnable() => PCRigEnabled = true;

        public static void OnDisable() => PCRigEnabled = false;

        public static void RigGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig.LocalRig.enabled = false;

                VRRig.LocalRig.transform.position = GunLib.pointer.transform.position;
                VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position =
                    VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * -0.3f;

                VRRig.LocalRig.rightHand.rigTarget.transform.position =
                    VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.right * 0.3f;
            },
            onGrip: () =>
            {
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }

        public static void EnableRig()
        {
            VRRig.LocalRig.enabled = true;
        }
        

        private static Vector3 spazHeadOffset;
        private static Vector3 spazLeftOffset;
        private static Vector3 spazRightOffset;
        public static void SpazRig()
        {
            float jitter = 0.08f;

            spazHeadOffset = Vector3.Lerp(spazHeadOffset, RandomVector3(jitter), Time.deltaTime * 20f);
            spazLeftOffset = Vector3.Lerp(spazLeftOffset, RandomVector3(jitter), Time.deltaTime * 20f);
            spazRightOffset = Vector3.Lerp(spazRightOffset, RandomVector3(jitter), Time.deltaTime * 20f);

            VRRig.LocalRig.head.trackingPositionOffset = spazHeadOffset;
            VRRig.LocalRig.leftHand.trackingPositionOffset = spazLeftOffset;
            VRRig.LocalRig.rightHand.trackingPositionOffset = spazRightOffset;
        }

        public static void DisableSpazRig()
        {
            spazHeadOffset = Vector3.zero;
            spazLeftOffset = Vector3.zero;
            spazRightOffset = Vector3.zero;

            VRRig.LocalRig.head.trackingPositionOffset = Vector3.zero;
            VRRig.LocalRig.leftHand.trackingPositionOffset = Vector3.zero;
            VRRig.LocalRig.rightHand.trackingPositionOffset = Vector3.zero;
        }

        public static void SpazHands()
        {
            VRRig.LocalRig.enabled = false;

            VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position;
            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();

            VRRig.LocalRig.leftHand.rigTarget.transform.position =
                GorillaTagger.Instance.leftHandTransform.position + VRRig.LocalRig.leftHand.rigTarget.transform.forward * 2f;

            VRRig.LocalRig.rightHand.rigTarget.transform.position =
                GorillaTagger.Instance.rightHandTransform.position + VRRig.LocalRig.rightHand.rigTarget.transform.forward * 2f;
        }

        public static void DisableSpazHands() => VRRig.LocalRig.enabled = true;

        public static void AmputateRig()
        {
            VRRig.LocalRig.enabled = false;

            Transform body = GorillaTagger.Instance.bodyCollider.transform;

            VRRig.LocalRig.transform.position = body.position;
            VRRig.LocalRig.transform.rotation = body.rotation;

            VRRig.LocalRig.head.rigTarget.transform.rotation =
                body.rotation * Quaternion.Euler(150f, 90f, 0f);

            VRRig.LocalRig.leftHand.rigTarget.transform.position =
                body.position + body.right * -0.05f + body.up * 0.05f;

            VRRig.LocalRig.rightHand.rigTarget.transform.position =
                body.position + body.right * 0.05f + body.up * 0.05f;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation =
                body.rotation * Quaternion.Euler(0f, 180f, 180f);

            VRRig.LocalRig.rightHand.rigTarget.transform.rotation =
                body.rotation * Quaternion.Euler(0f, 180f, 180f);
        }

        public static void DisableAmputateRig() => VRRig.LocalRig.enabled = true;

        public static void PiggybackGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }

                if (GunLib.gunLocked && GunLib.lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    Vector3 ridePos = GunLib.lockTarget.transform.position +
                        GunLib.lockTarget.transform.up * 0.55f -
                        GunLib.lockTarget.transform.forward * 0.05f;

                    VRRig.LocalRig.transform.position = ridePos;
                    VRRig.LocalRig.transform.rotation = GunLib.lockTarget.transform.rotation;

                    VRRig.LocalRig.head.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        GunLib.lockTarget.transform.position + GunLib.lockTarget.transform.up * 0.3f + GunLib.lockTarget.transform.right * -0.2f;

                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        GunLib.lockTarget.transform.position + GunLib.lockTarget.transform.up * 0.3f + GunLib.lockTarget.transform.right * 0.2f;
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }

        public static void OrbitAllPlayers()
        {
            VRRig target = GetClosestPlayer();

            if (target == null)
                return;

            Vector3 offset = new Vector3(
                Mathf.Cos(Time.frameCount / 20f) * 1.5f,
                0.4f,
                Mathf.Sin(Time.frameCount / 20f) * 1.5f);

            VRRig.LocalRig.enabled = false;

            VRRig.LocalRig.transform.position = target.transform.position + offset;
            VRRig.LocalRig.transform.LookAt(target.transform.position);

            VRRig.LocalRig.head.rigTarget.transform.rotation =
                VRRig.LocalRig.transform.rotation;

            VRRig.LocalRig.leftHand.rigTarget.transform.position =
                VRRig.LocalRig.transform.position +
                VRRig.LocalRig.transform.right * -1f;

            VRRig.LocalRig.rightHand.rigTarget.transform.position =
                VRRig.LocalRig.transform.position +
                VRRig.LocalRig.transform.right * 1f;

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation =
                VRRig.LocalRig.transform.rotation;

            VRRig.LocalRig.rightHand.rigTarget.transform.rotation =
                VRRig.LocalRig.transform.rotation;

            RPCProtection();
        }

        public static void DisableOrbitAllPlayers()
        {
           
            VRRig.LocalRig.enabled = true;
        }

        public static void JumpscareAll()
        {
            VRRig target = GetClosestPlayer();

            if (target == null)
                return;

            VRRig.LocalRig.enabled = false;

            Vector3 facePos =
                target.headMesh.transform.position +
                target.headMesh.transform.forward * Random.Range(0.1f, 0.4f);

            VRRig.LocalRig.transform.position = facePos;
            VRRig.LocalRig.transform.LookAt(target.headMesh.transform.position);

            Quaternion faceLook = VRRig.LocalRig.transform.rotation;

            VRRig.LocalRig.head.rigTarget.transform.rotation = faceLook;

            VRRig.LocalRig.leftHand.rigTarget.transform.position =
                target.headMesh.transform.position +
                target.headMesh.transform.right * 0.2f;

            VRRig.LocalRig.rightHand.rigTarget.transform.position =
                target.headMesh.transform.position -
                target.headMesh.transform.right * 0.2f;

            Quaternion handRotation =
                Quaternion.Euler(faceLook.eulerAngles + new Vector3(0f, 180f, 0f));

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation = handRotation;
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation = handRotation;

            RPCProtection();
        }


        public static void DisableJumpscareAll()
        {
            VRRig.LocalRig.enabled = true;
        }

        private static float overstimulateGunDelay;
        public static void OverstimulateGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }

                if (GunLib.gunLocked && GunLib.lockTarget != null && Time.time > overstimulateGunDelay)
                {
                    overstimulateGunDelay = Time.time + 0.08f;

                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position =
                        GunLib.lockTarget.headMesh.transform.position + RandomVector3(0.5f);

                    VRRig.LocalRig.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();

                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        GunLib.lockTarget.headMesh.transform.position + RandomVector3(0.5f);

                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        GunLib.lockTarget.headMesh.transform.position + RandomVector3(0.5f);

                    RPCProtection();
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }

        private static float overstimulateAllDelay;

        public static void OverstimulateAll()
        {
            if (Time.time < overstimulateAllDelay)
                return;

            overstimulateAllDelay = Time.time + 0.08f;

            VRRig target = GetClosestPlayer();

            if (target == null)
                return;

            VRRig.LocalRig.enabled = false;

            VRRig.LocalRig.transform.position =
                target.headMesh.transform.position + RandomVector3(0.5f);

            VRRig.LocalRig.transform.rotation = RandomQuaternion();

            VRRig.LocalRig.head.rigTarget.transform.rotation =
                RandomQuaternion();

            VRRig.LocalRig.leftHand.rigTarget.transform.position =
                target.headMesh.transform.position + RandomVector3(0.5f);

            VRRig.LocalRig.rightHand.rigTarget.transform.position =
                target.headMesh.transform.position + RandomVector3(0.5f);

            RPCProtection();
        }

        public static void DisableOverstimulateAll()
        {
            VRRig.LocalRig.enabled = true;
        }

        public static void AnnoyGun()
        {
            GunLib.StartPointerSystem(() =>
            {
                VRRig target = GunLib.GetTargetRig();
                if (target != null && !target.isLocal)
                {
                    GunLib.gunLocked = true;
                    GunLib.lockTarget = target;
                }

                if (GunLib.gunLocked && GunLib.lockTarget != null)
                {
                    VRRig.LocalRig.enabled = false;

                    VRRig.LocalRig.transform.position = GunLib.lockTarget.transform.position + RandomVector3(0.6f);
                    VRRig.LocalRig.transform.LookAt(GunLib.lockTarget.transform.position);

                    VRRig.LocalRig.head.rigTarget.transform.rotation = RandomQuaternion();

                    VRRig.LocalRig.leftHand.rigTarget.transform.position =
                        GunLib.lockTarget.transform.position + RandomVector3(0.6f);

                    VRRig.LocalRig.rightHand.rigTarget.transform.position =
                        GunLib.lockTarget.transform.position + RandomVector3(0.6f);

                    VRRig.LocalRig.leftHand.rigTarget.transform.rotation = RandomQuaternion();
                    VRRig.LocalRig.rightHand.rigTarget.transform.rotation = RandomQuaternion();
                }
            },
            onGrip: () =>
            {
                GunLib.gunLocked = false;
                GunLib.lockTarget = null;
                VRRig.LocalRig.enabled = true;
            },
            rightHand: true);
        }

        public static void AnnoyAll()
        {
            VRRig target = GetClosestPlayer();

            if (target == null)
                return;

            VRRig.LocalRig.enabled = false;

            Vector3 position =
                target.transform.position + RandomVector3();

            VRRig.LocalRig.transform.position = position;
            VRRig.LocalRig.transform.LookAt(target.transform.position);

            VRRig.LocalRig.head.rigTarget.transform.rotation =
                RandomQuaternion();

            VRRig.LocalRig.leftHand.rigTarget.transform.position =
                target.transform.position + RandomVector3();

            VRRig.LocalRig.rightHand.rigTarget.transform.position =
                target.transform.position + RandomVector3();

            VRRig.LocalRig.leftHand.rigTarget.transform.rotation =
                RandomQuaternion();

            VRRig.LocalRig.rightHand.rigTarget.transform.rotation =
                RandomQuaternion();

            RPCProtection();
        }

        public static void DisableAnnoyAll()
        {
            VRRig.LocalRig.enabled = true;
        }

        private static Vector3 RandomVector3(float range = 1f) =>
            new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));

        private static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(Random.Range(0f, range), Random.Range(0f, range), Random.Range(0f, range));
    }

}
