using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx;
using GorillaLocomotion;
using GorillaTag;
using LagMenu.Menu;
using LagMenu.Mods.Walker;
using LagMenu.Utilities;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Valve.VR;
using static LagMenu.Mods.Settings;
using static LagMenu.Main;
using static LagMenu.Utilities.GunLib;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LagMenu.Mods
{
    public class Movement
    {

        private static float _rotStartYaw = -1f;
        private static float _rotStartPitch = -1f;
        private static float _mouseDragX = 0f;
        private static float _mouseDragY = 0f;
        public static bool menu = false;
        public static bool wasdEnabled = false;


        public static void Fly(float flyspeed)
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * flyspeed;
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }


        private static bool wasHeld = false;
        private static Vector3 lastVelocity = Vector3.zero;

        public static void LJFly(float flyspeed)
        {
            bool isHeld = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);

            if (isHeld)
            {
                lastVelocity = GorillaTagger.Instance.headCollider.transform.forward * flyspeed;
                GTPlayer.Instance.transform.position += lastVelocity * Time.deltaTime;
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                wasHeld = true;
            }
            else if (wasHeld)
            {
                GorillaTagger.Instance.rigidbody.linearVelocity = lastVelocity;
                wasHeld = false;
            }
        }


        public static void NoClip()
        {
            bool triggerHeld = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;
            MeshCollider[] colliders = Object.FindObjectsOfType<MeshCollider>();
            foreach (MeshCollider col in colliders)
                col.enabled = !triggerHeld;
        }

        public static void NoClipRJ()
        {
            bool held = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
            MeshCollider[] colliders = Object.FindObjectsOfType<MeshCollider>();
            foreach (MeshCollider col in colliders)
                col.enabled = !held;
        }


        public static GameObject platL, platR;
        public static int platMode = 1;
        public static int platInput = 0;

        public static void Platforms()
        {
            bool leftInput = (platInput == 0) ? ControllerInputPoller.instance.leftGrab : ControllerInputPoller.instance.leftControllerTriggerButton;
            bool rightInput = (platInput == 0) ? ControllerInputPoller.instance.rightGrab : ControllerInputPoller.instance.rightControllerTriggerButton;

            if (leftInput)
            {
                if (platL == null)
                {
                    platL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platL.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platL.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    platL.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                    platL.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
                }
                Renderer rendL = platL.GetComponent<Renderer>();
                if (platMode == 0) rendL.enabled = false;
                else
                {
                    rendL.enabled = true;
                    rendL.material.color = new Color(Main.BaseColor.r, Main.BaseColor.g, Main.BaseColor.b,
                                                     platMode == 1 ? 0.5f : 1f);
                }
            }
            else if (platL != null) { Object.Destroy(platL); platL = null; }

            if (rightInput)
            {
                if (platR == null)
                {
                    platR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platR.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platR.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    platR.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    platR.GetComponent<Renderer>().material.shader = Shader.Find("Sprites/Default");
                }
                Renderer rendR = platR.GetComponent<Renderer>();
                if (platMode == 0) rendR.enabled = false;
                else
                {
                    rendR.enabled = true;
                    rendR.material.color = new Color(Main.BaseColor.r, Main.BaseColor.g, Main.BaseColor.b,
                                                     platMode == 1 ? 0.5f : 1f);
                }
            }
            else if (platR != null) { Object.Destroy(platR); platR = null; }
        }

        public static void BarkFly()
        {
            GTPlayer.Instance.AddForce(-Physics.gravity, ForceMode.Acceleration);

            Vector2 xz = ControllerInputPoller.instance.leftControllerPrimary2DAxis;
            float y = ControllerInputPoller.instance.rightControllerPrimary2DAxis.y;

            Vector3 playerForward = GTPlayer.Instance.bodyCollider.transform.forward;
            playerForward.y = 0f;

            Vector3 playerRight = GTPlayer.Instance.bodyCollider.transform.right;
            playerRight.y = 0f;

            Vector3 velocity = xz.x * playerRight
                             + y * Vector3.up
                             + xz.y * playerForward;

            velocity *= flyspeed;

            GorillaTagger.Instance.rigidbody.linearVelocity =
                Vector3.Lerp(GorillaTagger.Instance.rigidbody.linearVelocity, velocity, 0.125f);
        }



        private static Vector3? oldLocalPosition;
        private static Camera _pcButtonCam;











        public static void PCButtonClickGun()
        {
            GunLib.StartPointerSystem(onTrigger: () =>
            {
                if (GunLib.raycastHit.collider == null) return;

                var handTransform = GorillaTagger.Instance.rightHandTriggerCollider.transform;
                var follow = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>();

                follow.enabled = false;
                handTransform.position = GunLib.raycastHit.point;

                CoroutineManager.instance.StartCoroutine(RestoreHandCoroutine(handTransform, follow));
                ResourceManager.PlayGunSound();
            });
        }

        private static IEnumerator RestoreHandCoroutine(Transform handTransform, TransformFollow follow)
        {
            yield return null;
            follow.enabled = true;
        }


        public static void WASDFly()
        {

            Keyboard kb = Keyboard.current;
            if (kb == null) return;

            bool moveForward = kb[Key.W].isPressed;
            bool moveBack = kb[Key.S].isPressed;
            bool moveLeft = kb[Key.A].isPressed;
            bool moveRight = kb[Key.D].isPressed;
            bool moveUp = kb[Key.Space].isPressed;
            bool moveDown = kb[Key.LeftCtrl].isPressed;
            bool sprint = kb[Key.LeftShift].isPressed;
            bool slow = kb[Key.LeftAlt].isPressed;

            bool turnLeft = kb[Key.LeftArrow].isPressed;
            bool turnRight = kb[Key.RightArrow].isPressed;
            bool pitchUp = kb[Key.UpArrow].isPressed;
            bool pitchDown = kb[Key.DownArrow].isPressed;


            GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;

            if (menu) return;


            Transform camRoot = GTPlayer.Instance.GetControllerTransform(false).parent;
            Rigidbody rb = GorillaTagger.Instance.rigidbody;

            const float TurnSpeed = 250f;

            if (turnLeft) camRoot.eulerAngles += new Vector3(0f, -TurnSpeed, 0f) * Time.deltaTime;
            if (turnRight) camRoot.eulerAngles += new Vector3(0f, TurnSpeed, 0f) * Time.deltaTime;
            if (pitchUp) camRoot.eulerAngles += new Vector3(-TurnSpeed, 0f, 0f) * Time.deltaTime;
            if (pitchDown) camRoot.eulerAngles += new Vector3(TurnSpeed, 0f, 0f) * Time.deltaTime;

            if (Mouse.current.rightButton.isPressed)
            {
                Vector3 euler = camRoot.rotation.eulerAngles;
                float mouseNormX = Mouse.current.position.value.x / Screen.width;
                float mouseNormY = Mouse.current.position.value.y / Screen.height;

                if (_rotStartYaw < 0f) { _rotStartYaw = euler.y; _mouseDragX = mouseNormX; }
                if (_rotStartPitch < 0f) { _rotStartPitch = euler.x; _mouseDragY = mouseNormY; }

                const float MouseSensitivity = 360f * 1.33f;

                float pitch = _rotStartPitch - (mouseNormY - _mouseDragY) * MouseSensitivity;
                float yaw = _rotStartYaw + (mouseNormX - _mouseDragX) * MouseSensitivity;

                if (pitch > 180f) pitch -= 360f;
                pitch = Mathf.Clamp(pitch, -90f, 90f);

                camRoot.rotation = Quaternion.Euler(pitch, yaw, euler.z);
            }
            else
            {
                _rotStartYaw = -1f;
                _rotStartPitch = -1f;
            }

            float speed = flyspeed;
            if (sprint) speed *= 2f;
            else if (slow) speed *= 0.5f;

            float delta = Time.deltaTime * speed;

            if (moveForward) rb.transform.position += camRoot.forward * delta;
            if (moveBack) rb.transform.position += -camRoot.forward * delta;
            if (moveRight) rb.transform.position += camRoot.right * delta;
            if (moveLeft) rb.transform.position += -camRoot.right * delta;
            if (moveUp) rb.transform.position += Vector3.up * delta;
            if (moveDown) rb.transform.position += -Vector3.up * delta;

            VRRig.LocalRig.head.rigTarget.transform.rotation =
                GorillaTagger.Instance.headCollider.transform.rotation;
        }

        public static void Tick()
        {
            if (wasdEnabled) WASDFly();
        }


        private static float _walkPitch;
        private static float _walkYaw;

        public static void PCWalking()
        {
            Vector3 movement = Vector3.zero;
            float movementSpeed = 500f * Time.deltaTime;

            Keyboard kb = Keyboard.current;
            if (kb == null) return;

            if (kb[Key.LeftShift].isPressed) movementSpeed *= 3f;

            if (kb[Key.W].isPressed) movement += VRRig.LocalRig.bodyRenderer.transform.forward;
            if (kb[Key.S].isPressed) movement -= VRRig.LocalRig.bodyRenderer.transform.forward;
            if (kb[Key.A].isPressed) movement -= VRRig.LocalRig.bodyRenderer.transform.right;
            if (kb[Key.D].isPressed) movement += VRRig.LocalRig.bodyRenderer.transform.right;

            movement.Normalize();
            movement *= movementSpeed;

            Rigidbody rb = GorillaTagger.Instance.rigidbody;
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                float sensitivity = 6f;

                _walkYaw += mouseDelta.x * sensitivity * Time.deltaTime;
                _walkPitch -= mouseDelta.y * sensitivity * Time.deltaTime;
                _walkPitch = Mathf.Clamp(_walkPitch, -89f, 89f);

                GorillaTagger.Instance.headCollider.transform.rotation = Quaternion.Euler(_walkPitch, _walkYaw, 0f);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }


            Ray ray = new Ray(GTPlayer.Instance.bodyCollider.transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f, GTPlayer.Instance.locomotionEnabledLayers))
            {
                float targetDist = kb[Key.LeftCtrl].isPressed ? 0.6f : 0.8f;
                targetDist += Mathf.Sin(Time.time * 10f) * 0.01f;

                float difference = hit.distance - targetDist;
                float verticalVelocity = Vector3.Dot(rb.linearVelocity, Vector3.up);
                float forceAmount = -difference * 50f - verticalVelocity * 5f;

                if (kb[Key.Space].isPressed) forceAmount = 100f;

                rb.AddForce(Vector3.up * forceAmount, ForceMode.Acceleration);
            }
        }

        public static void EnablePCWalking()
        {
            Vector3 euler = GorillaTagger.Instance.headCollider.transform.eulerAngles;
            _walkYaw = euler.y;
            _walkPitch = euler.x;

            HandAnimator left = GTPlayer.Instance.leftHand.controllerTransform.gameObject.AddComponent<HandAnimator>();
            left.Body = GTPlayer.Instance.bodyCollider.transform;
            left.TerrainLayer = GTPlayer.Instance.locomotionEnabledLayers;
            left.FootSpacing = -0.3f;
            left.IsLeftHand = true;

            HandAnimator right = GTPlayer.Instance.rightHand.controllerTransform.gameObject.AddComponent<HandAnimator>();
            right.Body = GTPlayer.Instance.bodyCollider.transform;
            right.TerrainLayer = GTPlayer.Instance.locomotionEnabledLayers;
            right.FootSpacing = 0.3f;
            right.IsLeftHand = false;

            left.OtherHandAnimator = right;
            right.OtherHandAnimator = left;
        }

        public static void DisablePCWalking()
        {
            var left = GTPlayer.Instance.leftHand.controllerTransform.GetComponent<HandAnimator>();
            var right = GTPlayer.Instance.rightHand.controllerTransform.GetComponent<HandAnimator>();
            if (left) Object.Destroy(left);
            if (right) Object.Destroy(right);
            Cursor.lockState = CursorLockMode.None;
        }


        public static void SteamLongArms()
        {
            GTPlayer.Instance.transform.localScale = new Vector3(armsLength, armsLength, armsLength);
        }

        public static void NormalArms()
        {
            GTPlayer.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
        }





        public static void UncapMaxVelocity()
        {
            GTPlayer.Instance.maxJumpSpeed = float.MaxValue;
        }


        static bool speedBoostActive;

        public static void SpeedBoost()
        {
            if (speedBoostActive) return;
            foreach (GorillaSurfaceOverride surf in Resources.FindObjectsOfTypeAll<GorillaSurfaceOverride>())
            {
                surf.extraVelMaxMultiplier = speedBoostMax;
                surf.extraVelMultiplier = speedBoost;
            }
            speedBoostActive = true;
        }

        public static void OffSpeedBoost()
        {
            if (!speedBoostActive) return;
            foreach (GorillaSurfaceOverride surf in Resources.FindObjectsOfTypeAll<GorillaSurfaceOverride>())
            {
                surf.extraVelMaxMultiplier = 1f;
                surf.extraVelMultiplier = 1f;
            }
            speedBoostActive = false;
        }


        static Vector3 wallPos = Vector3.zero, wallNormal;

        public static void WallWalk()
        {
            if (GTPlayer.Instance.leftHand.wasColliding || GTPlayer.Instance.rightHand.wasColliding)
            {
                FieldInfo field = typeof(GTPlayer).GetField("lastHitInfoHand", BindingFlags.Instance | BindingFlags.NonPublic);
                RaycastHit hit = (RaycastHit)field.GetValue(GTPlayer.Instance);
                wallPos = hit.point;
                wallNormal = hit.normal;
            }

            if (wallPos != Vector3.zero && RightGrip)
            {
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(wallNormal * -5f, ForceMode.Acceleration);
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.deltaTime * (9.81f / Time.deltaTime)), ForceMode.Acceleration);
            }
        }


        static Vector3 spiderPos = Vector3.zero, spiderNormal;

        public static void SpiderWalk()
        {
            if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
            {
                FieldInfo field = typeof(GTPlayer).GetField("lastHitInfoHand", BindingFlags.Instance | BindingFlags.NonPublic);
                RaycastHit hit = (RaycastHit)field.GetValue(GTPlayer.Instance);
                spiderPos = hit.point;
                spiderNormal = hit.normal;
            }

            if (spiderPos != Vector3.zero)
            {
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(spiderNormal * -9.81f, ForceMode.Acceleration);
                GTPlayer.Instance.rightHand.controllerTransform.parent.rotation = Quaternion.Lerp(
                    GTPlayer.Instance.rightHand.controllerTransform.parent.rotation,
                    Quaternion.LookRotation(spiderNormal) * Quaternion.Euler(90f, 0f, 0f), Time.deltaTime);
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(Vector3.up * (Time.unscaledDeltaTime * (9.81f / Time.unscaledDeltaTime)), ForceMode.Acceleration);
            }
        }

        public static void SpiderWalkOff()
        {
            spiderPos = Vector3.zero;
            GTPlayer.Instance.rightHand.controllerTransform.parent.rotation = Quaternion.identity;
        }


        static bool lastTouchL, lastTouchR;

        public static void PullMod()
        {
            if ((!GTPlayer.Instance.IsHandTouching(true) && lastTouchL && RightGrip) ||
                (!GTPlayer.Instance.IsHandTouching(false) && lastTouchR && RightGrip))
            {
                Vector3 vel = GTPlayer.Instance.GetComponent<Rigidbody>().velocity;
                GTPlayer.Instance.transform.position += new Vector3(vel.x * pullPower, 0f, vel.z * pullPower);
            }

            lastTouchL = GTPlayer.Instance.IsHandTouching(true);
            lastTouchR = GTPlayer.Instance.IsHandTouching(false);
        }


   


        static float teleGunDelay;

        public static void TeleportGun()
        {
            var data = GunLib.ShootLock();
            if (data.isShooting && data.hitPosition != Vector3.zero && Time.time > teleGunDelay)
            {
                teleGunDelay = Time.time + 0.5f;
                GTPlayer.Instance.TeleportTo(data.hitPosition, GTPlayer.Instance.transform.rotation);
            }
        }



        public static void TpToStump()
        {
            GTPlayer.Instance.TeleportTo(new Vector3(-68.5887f, 12.0845f, -83.9583f), GTPlayer.Instance.transform.rotation);
        }


        public static void HandFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GTPlayer.Instance.rightHand.controllerTransform.forward * Time.deltaTime * flyspeed;
                GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void BodyFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * flyspeed;
                GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void SlingshotFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.GetComponent<Rigidbody>().velocity += GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * (flyspeed * 2f);
            }
        }

        public static void JoystickFly()
        {
            float speedMul = (LeftStickClick || RightStickClick) ? 3f : 1f;
            if (Mathf.Abs(LeftJoystick.x) > 0.3f || Mathf.Abs(LeftJoystick.y) > 0.3f || Mathf.Abs(RightJoystick.y) > 0.3f)
            {
                Vector3 dir = GorillaTagger.Instance.headCollider.transform.forward * (LeftJoystick.y * 8f)
                            + GorillaTagger.Instance.headCollider.transform.right * (LeftJoystick.x * 8f)
                            + Vector3.up * (RightJoystick.y * 8f);
                dir *= speedMul;
                GTPlayer.Instance.transform.position += dir * Time.deltaTime;
            }
            GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        static bool noclipFlyActive;
        public static void NoClipFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * flyspeed;
                GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                if (!noclipFlyActive) { noclipFlyActive = true; }
            }
        }

        public static void NoClipFlyOff()
        {
            noclipFlyActive = false;
        }



        public static void IronMonkey()
        {
            if (A_Button)
            {
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(flyspeed * -GorillaTagger.Instance.leftHandTransform.right, ForceMode.Acceleration);
                GorillaTagger.Instance.StartVibration(true,
                    GorillaTagger.Instance.tapHapticStrength / 50f * GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity.magnitude,
                    GorillaTagger.Instance.tapHapticDuration);
            }
            if (X_Button)
            {
                GTPlayer.Instance.bodyCollider.attachedRigidbody.AddForce(flyspeed * GorillaTagger.Instance.rightHandTransform.right, ForceMode.Acceleration);
                GorillaTagger.Instance.StartVibration(false,
                    GorillaTagger.Instance.tapHapticStrength / 50f * GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity.magnitude,
                    GorillaTagger.Instance.tapHapticDuration);
            }
        }


        static Vector2 carVel;

        public static void BananaCar()
        {
            Vector2 stick = LeftJoystick;
            carVel = Vector2.Lerp(carVel, stick, 0.05f);

            Vector3 addition = GorillaTagger.Instance.bodyCollider.transform.forward * carVel.y +
                                GorillaTagger.Instance.bodyCollider.transform.right * carVel.x;
            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, out var ray, 512f);

            if (ray.distance < 0.2f && (Mathf.Abs(carVel.x) > 0.05f || Mathf.Abs(carVel.y) > 0.05f))
            {
                GorillaTagger.Instance.bodyCollider.attachedRigidbody.velocity = addition * 10f;
            }
        }


        static bool dashed;

        public static void DashMod()
        {
            if (A_Button && !dashed)
                GorillaTagger.Instance.rigidbody.linearVelocity += GTPlayer.Instance.headCollider.transform.forward * 5f;
            dashed = A_Button;
        }


        public static void Strafe()
        {
            Physics.Raycast(GorillaTagger.Instance.bodyCollider.transform.position - new Vector3(0f, 0.2f, 0f),
                Vector3.down, out var hit, 512f, GTPlayer.Instance.locomotionEnabledLayers);
            if (hit.distance < 0.15f)
            {
                GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(
                    GorillaTagger.Instance.rigidbody.linearVelocity.x, GTPlayer.Instance.jumpMultiplier * 2.7272727f,
                    GorillaTagger.Instance.rigidbody.linearVelocity.z);
            }

            Vector3 fwd = GorillaTagger.Instance.bodyCollider.transform.forward * GTPlayer.Instance.maxJumpSpeed;
            GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(fwd.x, GorillaTagger.Instance.rigidbody.linearVelocity.y, fwd.z);
        }


        public static void Bouncy()
        {
            GorillaTagger.Instance.bodyCollider.material.bounciness = 1f;
            GorillaTagger.Instance.bodyCollider.material.bounceCombine = PhysicsMaterialCombine.Maximum;
            GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0f;
        }

        public static void BouncyOff()
        {
            GorillaTagger.Instance.bodyCollider.material.bounciness = 0f;
            GorillaTagger.Instance.bodyCollider.material.bounceCombine = PhysicsMaterialCombine.Average;
            GorillaTagger.Instance.bodyCollider.material.dynamicFriction = 0.6f;
        }


        static GameObject swimWater;

        public static void AirSwim()
        {
            if (swimWater == null)
            {
                var src = GameObject.Find("Environment Objects/LocalObjects_Prefab/ForestToBeach/ForestToBeach_Prefab_V4/ForestToBeach_Geo/CaveWaterVolume");
                if (src == null) return;
                swimWater = Object.Instantiate(src);
                swimWater.transform.localScale = new Vector3(5f, 5f, 5f);
                swimWater.GetComponent<Renderer>().enabled = false;
                return;
            }

            GTPlayer.Instance.audioManager.UnsetMixerSnapshot(0.1f);
            swimWater.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 2.5f, 0f);
        }

        public static void AirSwimOff()
        {
            if (swimWater != null) { Object.Destroy(swimWater); swimWater = null; }
        }


        static float sizeScale = 1f;

        public static void SizeChanger()
        {
            float step = 0.05f;
            if (ControllerInputPoller.instance.leftControllerTriggerButton) step = 0.2f;
            if (ControllerInputPoller.instance.leftGrab) step = 0.01f;

            if (ControllerInputPoller.instance.rightControllerTriggerButton) sizeScale += step;
            if (ControllerInputPoller.instance.rightGrab) sizeScale -= step;
            if (ControllerInputPoller.instance.rightControllerPrimaryButton) sizeScale = 1f;
            if (sizeScale < 0.05f) sizeScale = 0.05f;

            VRRig.LocalRig.transform.localScale = Vector3.one * sizeScale;
            VRRig.LocalRig.NativeScale = sizeScale;
            GTPlayer.Instance.nativeScale = sizeScale;
        }

        public static void DisableSizeChanger()
        {
            sizeScale = 1f;
            VRRig.LocalRig.transform.localScale = Vector3.one;
            VRRig.LocalRig.NativeScale = 1f;
            GTPlayer.Instance.nativeScale = 1f;
        }


        static readonly Vector3[] lastRightFist = new Vector3[10];
        static readonly Vector3[] lastLeftFist = new Vector3[10];

        public static void PunchMod()
        {
            int i = -1;
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (rig.isLocal) continue;
                Vector3 headPos = VRRig.LocalRig.head.rigTarget.position;

                if (rig.IsMakingFistRight())
                {
                    i++;
                    if (Vector3.Distance(rig.rightHandTransform.position, headPos) < 0.25f)
                        GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.Normalize(rig.rightHandTransform.position - lastRightFist[i]) * 10f;
                    lastRightFist[i] = rig.rightHandTransform.position;
                }

                if (rig.IsMakingFistLeft())
                {
                    i++;
                    if (Vector3.Distance(rig.leftHandTransform.position, headPos) < 0.25f)
                        GorillaTagger.Instance.rigidbody.linearVelocity += Vector3.Normalize(rig.leftHandTransform.position - lastLeftFist[i]) * 10f;
                    lastLeftFist[i] = rig.leftHandTransform.position;
                }
            }
        }


        static VRRig grabbedRig;

        public static void GrabMod()
        {
            foreach (var rig in VRRigCache.ActiveRigs)
            {
                if (rig.isLocal) continue;
                Vector3 headPos = VRRig.LocalRig.head.rigTarget.position;

                if (rig.IsMakingFistRight() && grabbedRig == null && Vector3.Distance(rig.rightHandTransform.position, headPos) < 0.25f)
                {
                    GTPlayer.Instance.TeleportTo(rig.rightHandTransform.position, GTPlayer.Instance.transform.rotation);
                    grabbedRig = rig;
                }
                else if (rig.IsMakingFistLeft() && grabbedRig == null && Vector3.Distance(rig.leftHandTransform.position, headPos) < 0.25f)
                {
                    GTPlayer.Instance.TeleportTo(rig.leftHandTransform.position, GTPlayer.Instance.transform.rotation);
                    grabbedRig = rig;
                }

                if (grabbedRig == rig)
                {
                    if (rig.IsMakingFistLeft())
                        GTPlayer.Instance.TeleportTo(rig.rightHandTransform.position, GTPlayer.Instance.transform.rotation);
                    else if (rig.IsMakingFistRight())
                        GTPlayer.Instance.TeleportTo(rig.leftHandTransform.position, GTPlayer.Instance.transform.rotation);

                    if (!rig.IsMakingFistRight() && !rig.IsMakingFistLeft())
                        grabbedRig = null;
                }
            }
        }


        public static void TeleportToLockedPlayer()
        {
            var data = GunLib.ShootLock();
            if (data.isShooting && data.isLocked && data.lockedPlayer != null)
            {
                GTPlayer.Instance.TeleportTo(data.lockedPlayer.transform.position, GTPlayer.Instance.transform.rotation);
            }
        }

        public static void CopyLockedPlayerId()
        {
            var data = GunLib.ShootLock();
            if (data.isLocked && data.lockedPlayer != null && !data.lockedPlayer.isLocal)
            {
                string id = RigManager.GetPlayerFromRig(data.lockedPlayer)?.UserId ?? "unknown";
                GUIUtility.systemCopyBuffer = id;
            }
        }

    }
}
