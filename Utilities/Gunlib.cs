using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using GorillaLocomotion;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LagMenu.Utilities
{
    public enum GunLineStyle
    {
        Bezier,
        Straight,
        Wiggly,
        Segmented
    }

    public static class GunLib
    {
        public static GameObject pointer;
        public static LineRenderer line;
        public static RaycastHit raycastHit;

        public static bool gunLocked;
        public static VRRig lockTarget;

        public static bool isGripping;
        public static bool isTrigger;
        public static bool lastTrigger;

        public static bool GunParticles = true;
        public static GunLineStyle LineStyle = GunLineStyle.Bezier;

        public static Vector3 _smoothMid;
        public const int LineCurve = 60;

        public static Camera _desktopCam;


        private static Shader _guiShader;
        private static Material _pointerMaterial;
        private static Material _particleMaterialTemplate;

        private static Shader GuiShader => _guiShader ??= Shader.Find("GUI/Text Shader");

        public static void CycleGunStyle()
        {
            int next = ((int)LineStyle + 1) % Enum.GetValues(typeof(GunLineStyle)).Length;
            LineStyle = (GunLineStyle)next;
        }

        public static void StartPointerSystem(Action onTrigger, bool rightHand = true)
            => StartPointerSystem(onTrigger, null, rightHand);

        public static void StartPointerSystem(Action onTrigger, Action onGrip, bool rightHand = true)
            => StartPointerSystem(
                Mods.ThemeChanger.CurrentAccent,
                new Vector3(0.1f, 0.1f, 0.1f),
                PrimitiveType.Sphere,
                rightHand, onTrigger, onGrip);

        public static void StartPointerSystem(Color color, Vector3 pointerSize,
            PrimitiveType pointerShape, bool rightHand,
            Action onTrigger, Action onGrip = null)
        {
            bool usingXR = XRSettings.isDeviceActive;

            Vector3 origin;
            Vector3 dir;

            if (usingXR)
            {
                Transform arm = rightHand
                    ? GTPlayer.Instance.RightHand.controllerTransform
                    : GTPlayer.Instance.LeftHand.controllerTransform;

                origin = arm.position;
                dir = -arm.up;

                isGripping = rightHand
                    ? ControllerInputPoller.instance.rightGrab
                    : ControllerInputPoller.instance.leftGrab;

                isTrigger = rightHand
                    ? ControllerInputPoller.instance.rightControllerIndexFloat >= 0.8f
                    : ControllerInputPoller.instance.leftControllerIndexFloat >= 0.8f;
            }
            else
            {
                if (_desktopCam == null)
                {
                    GameObject camObj = GameObject.Find("Shoulder Camera");
                    if (camObj != null) _desktopCam = camObj.GetComponent<Camera>();

                    if (_desktopCam == null)
                    {
                        _desktopCam = Camera.main;
                        if (_desktopCam != null)
                            Debug.LogWarning("[GunLib] 'Shoulder Camera' not found, falling back to Camera.main");
                    }

                    if (_desktopCam == null)
                    {
                        Debug.LogWarning("[GunLib] No desktop camera found. Pointer system disabled on desktop.");
                        if (pointer != null) pointer.SetActive(false);
                        if (line != null) line.enabled = false;
                        return;
                    }
                }

                Mouse mouse = Mouse.current;
                if (mouse == null)
                {
                    Debug.LogWarning("[GunLib] No Mouse device found via Input System.");
                    if (pointer != null) pointer.SetActive(false);
                    if (line != null) line.enabled = false;
                    return;
                }

                Vector2 mousePos = mouse.position.ReadValue();
                Ray mouseRay = _desktopCam.ScreenPointToRay(mousePos);
                origin = mouseRay.origin;
                dir = mouseRay.direction;


                ControllerInputPoller.instance.rightControllerIndexFloat = mouse.leftButton.isPressed ? 1f : 0f;
                ControllerInputPoller.instance.rightGrab = mouse.rightButton.isPressed;

                isGripping = ControllerInputPoller.instance.rightGrab;
                isTrigger = ControllerInputPoller.instance.rightControllerIndexFloat >= 0.8f;
            }

#if DEBUG_GUNLIB
            Debug.Log($"[GunLib] usingXR={usingXR} isGripping={isGripping} isTrigger={isTrigger} origin={origin} dir={dir}");
#endif

            if (!isGripping)
            {
                if (pointer != null) pointer.SetActive(false);
                if (line != null) line.enabled = false;

                gunLocked = false;
                lockTarget = null;
                lastTrigger = false;
                _smoothMid = Vector3.zero;
                return;
            }

            if (pointer == null)
            {
                pointer = GameObject.CreatePrimitive(pointerShape);
                Object.Destroy(pointer.GetComponent<Collider>());
                Object.Destroy(pointer.GetComponent<Rigidbody>());

                _pointerMaterial = new Material(GuiShader);
                pointer.GetComponent<Renderer>().material = _pointerMaterial;

                GameObject lineObj = new GameObject("LagMenu_GunLine");
                Object.DontDestroyOnLoad(lineObj);
                line = lineObj.AddComponent<LineRenderer>();
                line.material = new Material(GuiShader);
                line.useWorldSpace = true;
                line.positionCount = LineCurve;
            }

            pointer.SetActive(true);

            Physics.Raycast(origin, dir, out raycastHit, 100f);

            if (isTrigger && !lastTrigger)
            {
                RaycastHit[] hits = Physics.RaycastAll(origin, dir, 50f);
                foreach (RaycastHit h in hits)
                {
                    VRRig rig = h.collider.GetComponentInParent<VRRig>();
                    if (rig != null && !rig.isLocal)
                    {
                        lockTarget = rig;
                        gunLocked = true;
                        break;
                    }
                }

                ResourceManager.PlayGunSound();
            }

            if (!isTrigger && lastTrigger)
            {
                gunLocked = false;
                lockTarget = null;
            }

            lastTrigger = isTrigger;

            Vector3 targetPos;
            if (gunLocked && lockTarget != null)
            {
                targetPos = lockTarget.transform.position;
                Vector3 lockDir = (targetPos - origin).normalized;
                Physics.Raycast(origin, lockDir, out raycastHit, 50f);
            }
            else
            {
                targetPos = raycastHit.collider != null
                    ? raycastHit.point
                    : origin + (dir * 10f);
            }

            pointer.transform.position = targetPos;
            pointer.transform.localScale = pointerSize;

            Color themeColor = Mods.ThemeChanger.CurrentAccent;
            Color lockedColor = new Color(
                Mathf.Min(themeColor.r + 0.5f, 1f),
                Mathf.Min(themeColor.g + 0.3f, 1f),
                Mathf.Min(themeColor.b + 0.3f, 1f)
            );

            Color pointerColor = gunLocked ? lockedColor : themeColor;

            _pointerMaterial.color = pointerColor;

            if (line != null)
            {
                line.enabled = usingXR;
                line.startColor = pointerColor;
                line.endColor = pointerColor;
                line.startWidth = 0.008f;
                line.endWidth = 0.008f;
                line.material.color = pointerColor;

                Vector3 handPos = origin;
                Vector3 spherePos = pointer.transform.position;

                DrawLine(LineStyle, line, handPos, spherePos);
            }

            if (GunParticles && (isTrigger || gunLocked))
            {
                if (_particleMaterialTemplate == null)
                    _particleMaterialTemplate = new Material(GuiShader);

                GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                particle.transform.position = pointer.transform.position;
                particle.transform.localScale = Vector3.one * 0.025f;

                Material particleMat = new Material(_particleMaterialTemplate);
                particleMat.color = pointerColor;
                particle.GetComponent<Renderer>().material = particleMat;

                particle.AddComponent<CustomParticle>();
                Object.Destroy(particle.GetComponent<Collider>());
            }

            if (isTrigger)
            {
                if (usingXR) GorillaTagger.Instance.StartVibration(!rightHand, 0.5f, 0.1f);
                onTrigger?.Invoke();
            }
            else
            {
                if (usingXR) GorillaTagger.Instance.StartVibration(!rightHand, 0f, 0f);
                onGrip?.Invoke();
            }
        }

        public struct GunShootData
        {
            public bool isShooting;
            public bool isLocked;
            public Vector3 hitPosition;
            public VRRig lockedPlayer;
        }

        public static GunShootData ShootLock(bool rightHand = true)
        {
            StartPointerSystem(
                Mods.ThemeChanger.CurrentAccent,
                new Vector3(0.1f, 0.1f, 0.1f),
                PrimitiveType.Sphere,
                rightHand, null, null);

            return new GunShootData
            {
                isShooting = isTrigger,
                isLocked = gunLocked,
                hitPosition = raycastHit.collider != null ? raycastHit.point : Vector3.zero,
                lockedPlayer = GetTargetRig()
            };
        }



        public static void DrawLine(GunLineStyle style, LineRenderer lr, Vector3 start, Vector3 end)
        {
            switch (style)
            {
                case GunLineStyle.Straight:
                    DrawStraightLine(lr, start, end);
                    break;
                case GunLineStyle.Wiggly:
                    DrawWigglyLine(lr, start, end);
                    break;
                case GunLineStyle.Segmented:
                    DrawSegmentedLine(lr, start, end);
                    break;
                case GunLineStyle.Bezier:
                default:
                    Vector3 rawMid = (start + end) / 2f;
                    _smoothMid = _smoothMid == Vector3.zero
                        ? rawMid
                        : Vector3.Lerp(_smoothMid, rawMid, Time.deltaTime * 6f);
                    DrawBezierLine(lr, start, _smoothMid, end);
                    break;
            }
        }

        public static void DrawBezierLine(LineRenderer lr, Vector3 start, Vector3 mid, Vector3 end)
        {
            lr.positionCount = LineCurve;
            for (int i = 0; i < LineCurve; i++)
            {
                float t = (float)i / (LineCurve - 1);
                lr.SetPosition(i, BezierPoint(start, mid, end, t));
            }
            lr.SetPosition(0, start);
            lr.SetPosition(LineCurve - 1, end);
        }

        public static void DrawStraightLine(LineRenderer lr, Vector3 start, Vector3 end)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }

        public static void DrawWigglyLine(LineRenderer lr, Vector3 start, Vector3 end)
        {
            const int segments = 40;
            lr.positionCount = segments;

            Vector3 axis = (end - start);
            Vector3 perp = Vector3.Cross(axis, Vector3.up);
            if (perp.sqrMagnitude < 0.0001f)
                perp = Vector3.Cross(axis, Vector3.right);
            perp.Normalize();

            float time = Time.time;
            for (int i = 0; i < segments; i++)
            {
                float t = (float)i / (segments - 1);
                Vector3 basePos = Vector3.Lerp(start, end, t);
                float wave = Mathf.Sin(time * 7.5f + t * Mathf.PI * 4f) * 0.05f;
                lr.SetPosition(i, basePos + perp * wave);
            }
        }

        public static void DrawSegmentedLine(LineRenderer lr, Vector3 start, Vector3 end)
        {

            const int dashCount = 10;
            const float dashLength = 0.6f;
            float scroll = (Time.time * 1.5f) % 1f;

            lr.positionCount = dashCount * 2;
            for (int i = 0; i < dashCount; i++)
            {
                float segStart = (float)i / dashCount;
                float segEnd = segStart + (1f / dashCount) * dashLength;


                float tStart = (segStart + scroll) % 1f;
                float tEnd = (segEnd + scroll) % 1f;
                if (tEnd < tStart) tEnd = tStart;

                Vector3 p0 = Vector3.Lerp(start, end, Mathf.Clamp01(tStart));
                Vector3 p1 = Vector3.Lerp(start, end, Mathf.Clamp01(tEnd));

                lr.SetPosition(i * 2, p0);
                lr.SetPosition(i * 2 + 1, p1);
            }
        }

        public static Vector3 BezierPoint(Vector3 start, Vector3 mid, Vector3 end, float t)
        {
            float u = 1f - t;
            return (u * u * start) + (2f * u * t * mid) + (t * t * end);
        }

        public static Photon.Realtime.Player GetTargetPlayer()
        {
            if (gunLocked && lockTarget != null)
                return GetPlayerFromVRRig(lockTarget);

            if (raycastHit.collider != null)
            {
                VRRig rig = raycastHit.collider.GetComponentInParent<VRRig>();
                if (rig != null) return GetPlayerFromVRRig(rig);
            }

            return null;
        }

        public static VRRig GetTargetRig()
        {
            if (gunLocked && lockTarget != null)
                return lockTarget;

            if (raycastHit.collider != null)
                return raycastHit.collider.GetComponentInParent<VRRig>();

            return null;
        }

        public static Photon.Realtime.Player GetPlayerFromVRRig(VRRig rig)
        {
            if (rig == null) return null;
            foreach (var p in Photon.Pun.PhotonNetwork.PlayerList)
                if (p != null && p.TagObject == rig)
                    return p;
            return null;
        }
    }

    public class CustomParticle : MonoBehaviour
    {
        public float spawnTime;
        public float startScale;
        public new Renderer renderer;
        public Vector3 velocity;

        public void Awake()
        {
            spawnTime = Time.time;
            startScale = transform.localScale.x;
            renderer = gameObject.GetComponent<Renderer>();
            velocity = Random.insideUnitSphere * 2f;
            Update();
        }

        public void Update()
        {
            if (Time.time > spawnTime + 1f)
            {
                Destroy(gameObject);
                return;
            }

            transform.position += velocity * Time.unscaledDeltaTime;
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, 0f, Time.time - spawnTime);
        }
    }
}
