using UnityEngine;

namespace PinPop
{
    // Original procedural carnival scenery; decorative meshes have no colliders.
    public sealed class CarnivalWorld : MonoBehaviour
    {
        public Transform Cart { get; private set; }
        Transform wheel, flag;
        float timer;
        public void Build()
        {
            var green = Art.Mat(Art.Hex("83AA6D"), .08f);
            Art.Shape("Festival meadow", PrimitiveType.Cube, new Vector3(0, -1.28f, -4.5f), new Vector3(32, .2f, 21), green, transform);
            for (int i = 0; i < 10; i++)
                Art.Shape("Rolling green hill", PrimitiveType.Sphere, new Vector3((i - 5) * 4, -1.6f, 9 + i % 2), new Vector3(7, 2.5f + i % 3 * .2f, 6), Art.Mat(Art.Hex(i % 2 == 0 ? "8EAC7C" : "769369")), transform);
            Tent(new Vector3(-5.8f, -1.1f, 7.4f), .8f); Tent(new Vector3(7, -1.1f, 9), .8f);
            Ferris(new Vector3(4.3f, 1.6f, 7.5f));
            for (int i = 0; i < 8; i++)
            {
                var cloud = new GameObject("Soft cloud").transform; cloud.SetParent(transform); cloud.position = new Vector3((i - 4) * 4.4f, 6 + i % 3, 18 + i % 2);
                for (int j = 0; j < 3; j++) Art.Shape("Cloud puff", PrimitiveType.Sphere, new Vector3((j - 1) * .8f, j == 1 ? .35f : 0, 0), new Vector3(1.9f, 1.1f, .8f), Art.Mat(Art.Hex("E5EFF1")), cloud);
            }
            var balloon = new GameObject("Balloon").transform; balloon.SetParent(transform); balloon.position = new Vector3(-5, 5, 16);
            Art.Shape("Blue balloon", PrimitiveType.Sphere, Vector3.zero, new Vector3(1.2f, 1.6f, 1.2f), Art.Mat(Art.Hex("45D4F3")), balloon);
            Art.RoundBox("Balloon basket", new Vector3(0, -1.2f, 0), new Vector3(.45f, .3f, .4f), .08f, Art.Mat(Art.Hex("FFCF58")), balloon);
            for (int i = 0; i < 40; i++)
            {
                float x = Mathf.Sin(i * 83.3f) * 12, z = Mathf.Cos(i * 31.1f) * 12;
                if (Mathf.Abs(x) < 4.6f && Mathf.Abs(z) < 5.5f) continue;
                Art.Shape("Meadow flower", PrimitiveType.Sphere, new Vector3(x, -1.13f, z), new Vector3(.10f, .035f, .10f), Art.Mat(Art.Hex(i % 2 == 0 ? "FFF8BC" : "FFABC1")), transform);
            }
            BuildCart();
        }
        void Tent(Vector3 at, float s)
        {
            var root = new GameObject("Festival tent").transform; root.SetParent(transform); root.position = at; root.localScale = Vector3.one * s;
            Art.Shape("Red pavilion", PrimitiveType.Cylinder, new Vector3(0, 1.2f, 0), new Vector3(3.1f, 1.2f, 3.1f), Art.Mat(Art.Hex("B56F68")), root);
            Roof(root, new Vector3(0, 2.4f, 0), 2.1f, 2.3f);
            Art.Shape("Door", PrimitiveType.Cube, new Vector3(0, .8f, -1.55f), new Vector3(.85f, 1.6f, .1f), Art.Mat(Art.Hex("852D48")), root);
            Art.Shape("Flag pole", PrimitiveType.Cylinder, new Vector3(0, 5, 0), new Vector3(.06f, .42f, .06f), Art.Mat(Art.Hex("FFDB5C")), root);
            Art.Shape("Red pennant", PrimitiveType.Cube, new Vector3(.4f, 5.2f, 0), new Vector3(.8f, .35f, .06f), Art.Mat(Art.Hex("FF527A")), root);
        }
        void Roof(Transform parent, Vector3 at, float radius, float height)
        {
            var root = new GameObject("Striped carnival roof"); root.transform.SetParent(parent, false); root.transform.localPosition = at;
            const int sides = 32; var verts = new Vector3[sides * 3]; var red = new System.Collections.Generic.List<int>(); var cream = new System.Collections.Generic.List<int>();
            for (int i = 0; i < sides; i++)
            {
                float a = i * Mathf.PI * 2 / sides, b = (i + 1) * Mathf.PI * 2 / sides;
                verts[i * 3] = Vector3.up * height; verts[i * 3 + 1] = new Vector3(Mathf.Cos(b), 0, Mathf.Sin(b)) * radius; verts[i * 3 + 2] = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * radius;
                (i / 4 % 2 == 0 ? red : cream).AddRange(new[] { i * 3, i * 3 + 1, i * 3 + 2 });
            }
            var mesh = new Mesh { vertices = verts, subMeshCount = 2 }; mesh.SetTriangles(red, 0); mesh.SetTriangles(cream, 1); mesh.RecalculateNormals();
            root.AddComponent<MeshFilter>().sharedMesh = mesh; root.AddComponent<MeshRenderer>().sharedMaterials = new[] { Art.Mat(Art.Hex("BA746F")), Art.Mat(Art.Hex("EEE4D0")) };
        }
        void Ferris(Vector3 at)
        {
            var root = new GameObject("Ferris wheel").transform; root.SetParent(transform); root.position = at;
            for (int side = -1; side <= 1; side += 2)
            {
                var leg = Art.Shape("Wheel support", PrimitiveType.Cylinder, new Vector3(side * 1.0f, -1.8f, .3f), new Vector3(.15f, 2.0f, .15f), Art.Mat(Art.Hex("FFF0AB")), root);
                leg.transform.localRotation = Quaternion.Euler(0, 0, side * 28);
            }
            wheel = new GameObject("Rotating wheel").transform; wheel.SetParent(root, false);
            var line = wheel.gameObject.AddComponent<LineRenderer>(); line.useWorldSpace = false; line.loop = true; line.positionCount = 64; line.startWidth = line.endWidth = .10f; line.sharedMaterial = Art.Mat(Art.Hex("CCF4FF"));
            for (int i = 0; i < 64; i++) { float a = i * Mathf.PI * 2 / 64; line.SetPosition(i, new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * 2.1f); }
            for (int i = 0; i < 10; i++)
            {
                float a = i * Mathf.PI * 2 / 10; Vector3 p = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * 2.1f;
                var spoke = new GameObject("Spoke").AddComponent<LineRenderer>(); spoke.transform.SetParent(wheel, false); spoke.useWorldSpace = false; spoke.positionCount = 2; spoke.SetPosition(0, Vector3.zero); spoke.SetPosition(1, p); spoke.startWidth = spoke.endWidth = .035f; spoke.sharedMaterial = Art.Mat(Art.Hex("EDFAFF"));
                Art.RoundBox("Gondola", p, new Vector3(.38f, .3f, .36f), .08f, Art.Mat(Art.Colors[i % 3]), wheel);
            }
            Art.Shape("Golden hub", PrimitiveType.Sphere, Vector3.zero, Vector3.one * .65f, Art.Mat(Art.Hex("FFCD53")), root);
        }
        void BuildCart()
        {
            Cart = new GameObject("Prize cart").transform; Cart.SetParent(transform); Cart.localPosition = new Vector3(0, .6f, -3);
            Art.RoundBox("Golden cart trim", new Vector3(0, .4f, 0), new Vector3(5.5f, 2.8f, 2.6f), .25f, Art.Mat(Art.Hex("CDB784")), Cart);
            Art.RoundBox("Red cart body", new Vector3(0, .45f, -.12f), new Vector3(5.15f, 2.4f, 2.6f), .2f, Art.Mat(Art.Hex("AA6F64")), Cart);
            for (int i = 0; i < 5; i++) Art.RoundBox("Wood plank", new Vector3(0, -.45f + i * .45f, -1.45f), new Vector3(4.9f, .035f, .05f), .01f, Art.Mat(Art.Hex("906058")), Cart);
            for (int x = -1; x <= 1; x += 2)
            {
                Art.Shape("Canopy pole", PrimitiveType.Cylinder, new Vector3(x * 2.45f, 2.5f, 0), new Vector3(.14f, 1.6f, .14f), Art.Mat(Art.Hex("DBCAA4")), Cart);
                var tyre = Art.Shape("Cart wheel", PrimitiveType.Cylinder, new Vector3(x * 1.9f, -.95f, -1.45f), new Vector3(1.55f, .16f, 1.55f), Art.Mat(Art.Hex("62544A")), Cart); tyre.transform.localRotation = Quaternion.Euler(90, 0, 0);
                var hub = Art.Shape("Cart wheel hub", PrimitiveType.Cylinder, new Vector3(x * 1.9f, -.95f, -1.64f), new Vector3(1.1f, .035f, 1.1f), Art.Mat(Art.Hex("BEA36C")), Cart); hub.transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
            Roof(Cart, new Vector3(0, 3.8f, 0), 3.3f, 2.0f);
            for (int i = 0; i < 5; i++)
            {
                var pin = new GameObject("Bowling prize"); pin.transform.SetParent(Cart, false); pin.transform.localPosition = new Vector3((i - 2) * .8f, 1.3f, 0); pin.transform.localRotation = Quaternion.Euler(0, 0, (i - 2) * 8);
                pin.AddComponent<MeshFilter>().sharedMesh = Art.PinMesh(); pin.AddComponent<MeshRenderer>().sharedMaterial = Art.Mat(Art.Colors[i % 3], .65f);
            }
            flag = Art.Shape("Cart pennant", PrimitiveType.Cube, new Vector3(.5f, 6.05f, 0), new Vector3(1.0f, .4f, .04f), Art.Mat(Art.Hex("BC7874")), Cart).transform;
        }
        public void SetHome(bool home) { if (Cart != null) Cart.gameObject.SetActive(home); }
        void Update()
        {
            if (PlayerPrefs.GetInt("ReducedMotion", 0) == 1) return;
            timer += Time.unscaledDeltaTime;
            if (wheel != null) wheel.localRotation = Quaternion.Euler(0, 0, timer * 5);
            if (Cart != null && Cart.gameObject.activeSelf) Cart.localPosition = new Vector3(0, .6f + Mathf.Sin(timer * 1.4f) * .035f, -3);
            if (flag != null) flag.localRotation = Quaternion.Euler(0, Mathf.Sin(timer * 2) * 12, 0);
        }
    }
}
