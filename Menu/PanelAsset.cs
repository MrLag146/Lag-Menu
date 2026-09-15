using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LagMenu.Menu
{
    public static class PanelAsset
    {
        private static readonly HashSet<string> HiddenParts = new HashSet<string>
        {
            "row_1_label", "row_2_label", "row_3_label", "row_4_label",
            "row_5_label", "row_6_label", "row_7_label", "row_8_label",
            "row_0_label", "title_lag_menu", "label_disconnect",
            "rail_prev_arrow", "rail_next_arrow", "label_home", "back_credit",
        };

        private static float F(string s) =>
            float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);

        private static readonly Dictionary<string, Transform> _partIndex =
            new Dictionary<string, Transform>();

        private static readonly Dictionary<string, List<Renderer>> _partsByMaterial =
            new Dictionary<string, List<Renderer>>();

        private static void IndexParts(Transform root)
        {
            _partIndex.Clear();
            IndexRecursive(root);
        }

        private static void IndexRecursive(Transform t)
        {
            _partIndex[t.name] = t;

            for (int i = 0; i < t.childCount; i++)
                IndexRecursive(t.GetChild(i));
        }

        public static Transform FindPart(string name) =>
            _partIndex.TryGetValue(name, out Transform t) ? t : null;

        public static void RetintMaterial(string matName, Color color)
        {
            if (!_partsByMaterial.TryGetValue(matName, out List<Renderer> renderers))
                return;

            foreach (Renderer r in renderers)
            {
                if (r != null)
                    r.material.color = color;
            }
        }

        public static bool TryGetPartBounds(
            Transform relativeTo,
            string partName,
            out Bounds bounds)
        {
            bounds = new Bounds();

            Transform part = FindPart(partName);
            MeshFilter mf = part != null
                ? part.GetComponent<MeshFilter>()
                : null;

            if (mf == null || mf.sharedMesh == null)
                return false;

            Vector3 c = mf.sharedMesh.bounds.center;
            Vector3 e = mf.sharedMesh.bounds.extents;

            bool any = false;

            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 local = c + new Vector3(
                    (corner & 1) == 0 ? -e.x : e.x,
                    (corner & 2) == 0 ? -e.y : e.y,
                    (corner & 4) == 0 ? -e.z : e.z);

                Vector3 world = part.TransformPoint(local);
                Vector3 pt = relativeTo.InverseTransformPoint(world);

                if (!any)
                {
                    bounds = new Bounds(pt, Vector3.zero);
                    any = true;
                }
                else
                {
                    bounds.Encapsulate(pt);
                }
            }

            return true;
        }

        public static GameObject BuildPanel(Transform parent)
        {
            GameObject root = new GameObject("LagMenu_PanelMesh");
            root.transform.SetParent(parent, false);

            string obj = ReadTextAsset("panel_structural.obj");

            if (obj == null)
            {
                Debug.LogError(
                    "[LagMenu] PanelAsset: couldn't find panel_structural.obj — " +
                    "the menu will have no visible panel mesh.");
                return root;
            }

            Dictionary<string, Color> colors =
                ParseMtl(ReadTextAsset("panel_structural.mtl"));

            _partsByMaterial.Clear();
            ParseAndBuildObj(obj, colors, root.transform);

            IndexParts(root.transform);

            return root;
        }

        private static Dictionary<string, Color> ParseMtl(string mtl)
        {
            var dict = new Dictionary<string, Color>();

            if (mtl == null)
                return dict;

            string curName = null;

            foreach (string raw in mtl.Replace("\r", "").Split('\n'))
            {
                string line = raw.Trim();

                if (line.StartsWith("newmtl "))
                {
                    curName = line.Substring(7).Trim();
                }
                else if (line.StartsWith("Kd ") && curName != null)
                {
                    string[] p = line.Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);

                    if (p.Length >= 4)
                    {
                        dict[curName] = new Color(
                            F(p[1]),
                            F(p[2]),
                            F(p[3]));
                    }
                }
            }

            return dict;
        }

        private static void ParseAndBuildObj(
            string obj,
            Dictionary<string, Color> colors,
            Transform parent)
        {
            string curName = null;
            string curMat = null;

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            void Flush()
            {
                if (curName != null && verts.Count > 0)
                {
                    Color c =
                        curMat != null &&
                        colors.TryGetValue(curMat, out Color found)
                            ? found
                            : Color.white;

                    CreatePart(
                        curName,
                        verts,
                        tris,
                        c,
                        curMat,
                        parent);
                }
            }

            foreach (string raw in obj.Replace("\r", "").Split('\n'))
            {
                string line = raw.Trim();

                if (line.Length < 2)
                    continue;

                if (line[0] == 'o' && line[1] == ' ')
                {
                    Flush();

                    curName = line.Substring(2).Trim();
                    curMat = null;

                    verts = new List<Vector3>();
                    tris = new List<int>();
                }
                else if (line.StartsWith("usemtl "))
                {
                    curMat = line.Substring(7).Trim();
                }
                else if (line[0] == 'v' && line[1] == ' ')
                {
                    string[] p = line.Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);

                    if (p.Length >= 4)
                    {
                        verts.Add(new Vector3(
                            F(p[1]),
                            F(p[2]),
                            -F(p[3])));
                    }
                }
                else if (line[0] == 'f' && line[1] == ' ')
                {
                    string[] p = line.Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);

                    if (p.Length >= 4)
                    {
                        tris.Add(int.Parse(p[1]) - 1);
                        tris.Add(int.Parse(p[3]) - 1);
                        tris.Add(int.Parse(p[2]) - 1);
                    }
                }
            }

            Flush();
        }

        private static void CreatePart(
            string name,
            List<Vector3> verts,
            List<int> tris,
            Color color,
            string matName,
            Transform parent)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);

            Mesh mesh = new Mesh();

            if (verts.Count > 65000)
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();

            Material mat = new Material(
                Shader.Find("GorillaTag/UberShader"));

            mat.color = color;

            mr.sharedMaterial = mat;
            mr.enabled = !HiddenParts.Contains(name);

            if (matName != null)
            {
                if (!_partsByMaterial.TryGetValue(matName, out List<Renderer> list))
                    _partsByMaterial[matName] = list = new List<Renderer>();
                list.Add(mr);
            }
        }

        public static bool TryGetLocalBounds(
            Transform root,
            out Bounds bounds)
        {
            bounds = new Bounds();
            bool any = false;

            Matrix4x4 toRoot = root.worldToLocalMatrix;

            MeshFilter[] filters =
                root.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter filter in filters)
            {
                Mesh mesh = filter.sharedMesh;

                if (mesh == null)
                    continue;

                Matrix4x4 toRootFromMesh =
                    toRoot * filter.transform.localToWorldMatrix;

                Vector3 centre = mesh.bounds.center;
                Vector3 extents = mesh.bounds.extents;

                for (int corner = 0; corner < 8; corner++)
                {
                    Vector3 point =
                        toRootFromMesh.MultiplyPoint3x4(
                            centre + new Vector3(
                                (corner & 1) == 0
                                    ? -extents.x
                                    : extents.x,
                                (corner & 2) == 0
                                    ? -extents.y
                                    : extents.y,
                                (corner & 4) == 0
                                    ? -extents.z
                                    : extents.z));

                    if (!any)
                    {
                        bounds = new Bounds(
                            point,
                            Vector3.zero);

                        any = true;
                    }
                    else
                    {
                        bounds.Encapsulate(point);
                    }
                }
            }

            return any;
        }

        private static string ReadTextAsset(string fileName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            foreach (string res in asm.GetManifestResourceNames())
            {
                if (res.EndsWith(
                    fileName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream s =
                        asm.GetManifestResourceStream(res))
                    using (StreamReader r =
                        new StreamReader(s))
                    {
                        return r.ReadToEnd();
                    }
                }
            }

            try
            {
                string dllDir =
                    Path.GetDirectoryName(asm.Location);

                string path = Path.Combine(
                    dllDir ?? "",
                    "LagMenuAssets",
                    fileName);

                if (File.Exists(path))
                    return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[LagMenu] PanelAsset: error reading loose file " +
                    $"'{fileName}': {e}");
            }

            return null;
        }
    }
}
