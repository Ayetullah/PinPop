using System.Collections.Generic;
using UnityEngine;

namespace PinPop
{
    public static class Art
    {
        public static readonly Color[] Colors = PinPopPalette.Colors;
        public static readonly string[] Names = PinPopPalette.Names;
        public static Color Hex(string s) { ColorUtility.TryParseHtmlString("#" + s, out var c); return c; }
        static Mesh pinMesh;
        static Material spriteMaterial;
        public static Material SpriteMaterial {get {if(spriteMaterial==null)spriteMaterial=new Material(Shader.Find("Sprites/Default")){name="PinPop shared effects"};return spriteMaterial;}}
        static readonly Dictionary<Vector2,Mesh> ringMeshes=new Dictionary<Vector2,Mesh>();
        static readonly Dictionary<Vector4,Mesh> contactMeshes=new Dictionary<Vector4,Mesh>();
        static readonly Dictionary<float,Mesh> blobMeshes=new Dictionary<float,Mesh>();
        static readonly Dictionary<string, Material> mats = new Dictionary<string, Material>();
        public static Material Mat(Color color, float gloss = .35f)
        {
            string key = ColorUtility.ToHtmlStringRGBA(color) + gloss;
            if (mats.TryGetValue(key, out var found)) return found;
            var m = new Material(Shader.Find("Standard"));
            m.color = color; m.SetFloat("_Glossiness", gloss); m.SetFloat("_Metallic", .02f);
            mats[key] = m; return m;
        }
        public static GameObject Shape(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat, Transform parent = null, bool collider = false)
        {
            var g = GameObject.CreatePrimitive(type); g.name = name;
            g.transform.SetParent(parent, false); g.transform.localPosition = pos; g.transform.localScale = scale;
            g.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collider) { var c = g.GetComponent<Collider>(); c.enabled = false; Object.Destroy(c); }
            return g;
        }
        public static GameObject RoundBox(string name, Vector3 pos, Vector3 size, float radius, Material mat, Transform parent, bool collider = false)
        {
            var g = new GameObject(name); g.transform.SetParent(parent, false); g.transform.localPosition = pos;
            var v = new List<Vector3>(); var tri = new List<int>();
            const int steps = 8; int count = (steps + 1) * 4;
            for (int layer = 0; layer < 2; layer++)
                for (int corner = 0; corner < 4; corner++)
                    for (int i = 0; i <= steps; i++)
                    {
                        float a = (corner * 90 + i * 90f / steps) * Mathf.Deg2Rad;
                        float x = (corner == 0 || corner == 3 ? 1 : -1) * (size.x / 2 - radius);
                        float z = (corner < 2 ? 1 : -1) * (size.z / 2 - radius);
                        v.Add(new Vector3(x + Mathf.Cos(a) * radius, (layer == 0 ? -.5f : .5f) * size.y, z + Mathf.Sin(a) * radius));
                    }
            v.Add(new Vector3(0, -size.y / 2, 0)); v.Add(new Vector3(0, size.y / 2, 0));
            for (int i = 0; i < count; i++)
            {
                int n = (i + 1) % count;
                tri.AddRange(new[] { i, n, i + count, n, n + count, i + count, count * 2, n, i, count * 2 + 1, i + count, n + count });
            }
            tri.Reverse(); // Perimeter runs counterclockwise in XZ; flip to outward-facing surfaces.
            var mesh = new Mesh { vertices = v.ToArray(), triangles = tri.ToArray() }; mesh.RecalculateNormals();
            g.AddComponent<MeshFilter>().sharedMesh = mesh; g.AddComponent<MeshRenderer>().sharedMaterial = mat;
            if (collider) g.AddComponent<BoxCollider>().size = size;
            return g;
        }
        public static Mesh PinMesh()
        {
            if (pinMesh != null) return pinMesh;
            // Smooth regulation-style silhouette: weighted belly, narrow neck, rounded crown.
            Vector2[] profile = { new Vector2(.15f,0),new Vector2(.185f,.04f),new Vector2(.23f,.14f),new Vector2(.265f,.30f),new Vector2(.27f,.45f),new Vector2(.245f,.60f),new Vector2(.19f,.75f),new Vector2(.13f,.90f),new Vector2(.105f,1.02f),new Vector2(.12f,1.11f),new Vector2(.17f,1.20f),new Vector2(.19f,1.30f),new Vector2(.165f,1.40f),new Vector2(.10f,1.49f),new Vector2(.045f,1.523f),new Vector2(0,1.53f) };
            var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>(); const int sides = 64, rings = 96;
            for (int y = 0; y <= rings; y++)
            {
                float t = y * (profile.Length - 1f) / rings; int k = Mathf.Min(Mathf.FloorToInt(t), profile.Length - 2); float f = t - k;
                Vector2 a = profile[Mathf.Max(0,k-1)], b = profile[k], c = profile[k+1], d = profile[Mathf.Min(profile.Length-1,k+2)];
                Vector2 point = .5f * ((2*b) + (-a+c)*f + (2*a-5*b+4*c-d)*f*f + (-a+3*b-3*c+d)*f*f*f);
                float radius = Mathf.Max(0, point.x), height = Mathf.Clamp(point.y,0,1.53f);
                for (int i = 0; i <= sides; i++)
                {
                    float angle = i * Mathf.PI * 2 / sides; vertices.Add(new Vector3(Mathf.Cos(angle)*radius,height,Mathf.Sin(angle)*radius));
                    uv.Add(new Vector2(i/(float)sides-.25f,height/1.53f));
                    if (y < rings && i < sides) { int n = y*(sides+1)+i; triangles.AddRange(new [] {n,n+sides+1,n+1,n+1,n+sides+1,n+sides+2}); }
                }
            }
            pinMesh = new Mesh { name = "Printed bowling pin / smooth lathe", vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = uv.ToArray() }; pinMesh.RecalculateNormals(); pinMesh.RecalculateBounds(); return pinMesh;
        }
        static readonly Material[] pinCoats = new Material[PinPopPalette.Count];
        public static Material PinCoat(int color)
        {
            if (pinCoats[color] != null) return pinCoats[color];
            var texture = Resources.Load<Texture2D>("PinSkins/pin-" + color);
            var coat = new Material(Shader.Find("Standard")); coat.name = "Printed lacquer pin " + color; coat.color = Color.white; coat.mainTexture = texture;
            if (texture != null) { texture.wrapMode = TextureWrapMode.Repeat; texture.anisoLevel = 8; texture.filterMode = FilterMode.Trilinear; }
            coat.SetFloat("_Glossiness", .77f); coat.SetFloat("_Metallic", .01f); coat.EnableKeyword("_EMISSION"); coat.SetColor("_EmissionColor", Color.black); pinCoats[color] = coat; return coat;
        }
        public static GameObject BallVisual(Transform parent, int color, float radius,int style=0)
        {
            var g = Shape("Polished bowling ball", PrimitiveType.Sphere, Vector3.zero, Vector3.one * radius * 2, BallLooks.Material(style,color), parent);
            Vector3[] holes = { new Vector3(-.20f, .70f, -.66f), new Vector3(.20f, .70f, -.66f), new Vector3(0, .42f, -.90f) };
            foreach (var p in holes) Shape("Finger hole", PrimitiveType.Sphere, p.normalized * .48f, Vector3.one * .105f, Mat(Hex("263051")), g.transform);
            return g;
        }
        public static void LandingRing(Transform parent, string name, float inner, float outer, Material material, float y)
        {
            var key=new Vector2(inner,outer);
            if(!ringMeshes.TryGetValue(key,out var mesh)||mesh==null){
                const int segments=64;var vertices=new List<Vector3>();var triangles=new List<int>();
                for(int i=0;i<=segments;i++){float angle=i*Mathf.PI*2/segments;vertices.Add(new Vector3(Mathf.Cos(angle)*inner,0,Mathf.Sin(angle)*inner));vertices.Add(new Vector3(Mathf.Cos(angle)*outer,0,Mathf.Sin(angle)*outer));if(i<segments){int n=i*2;triangles.AddRange(new[]{n,n+1,n+2,n+1,n+3,n+2});}}
                mesh=new Mesh{name="PinPop shared ring",vertices=vertices.ToArray(),triangles=triangles.ToArray()};ringMeshes[key]=mesh;
            }
            var obj=new GameObject(name);obj.transform.SetParent(parent,false);obj.transform.localPosition=Vector3.up*y;obj.AddComponent<MeshFilter>().sharedMesh=mesh;
            var renderer=obj.AddComponent<MeshRenderer>(); renderer.sharedMaterial=material; renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off; renderer.receiveShadows=false;
        }
        static readonly Mesh[] pinContactHulls=new Mesh[3];
        public static MeshCollider PinContactHull(Transform parent,int section)
        {
            if(pinContactHulls[section]==null){
                Vector2[][] profiles={
                    new[]{new Vector2(.158f,0),new Vector2(.193f,.04f),new Vector2(.238f,.14f),new Vector2(.273f,.30f),new Vector2(.278f,.45f),new Vector2(.253f,.60f),new Vector2(.198f,.75f)},
                    new[]{new Vector2(.198f,.75f),new Vector2(.128f,1.11f)},
                    new[]{new Vector2(.128f,1.11f),new Vector2(.178f,1.20f),new Vector2(.198f,1.30f),new Vector2(.173f,1.40f),new Vector2(.108f,1.49f),new Vector2(.053f,1.523f),new Vector2(.008f,1.53f)}
                };
                const int sides=16;var profile=profiles[section];var vertices=new List<Vector3>();var triangles=new List<int>();
                foreach(var point in profile)for(int i=0;i<sides;i++){float angle=i*Mathf.PI*2/sides;float r=point.x/Mathf.Cos(Mathf.PI/sides);vertices.Add(new Vector3(Mathf.Cos(angle)*r,point.y,Mathf.Sin(angle)*r));}
                for(int ring=0;ring<profile.Length-1;ring++)for(int i=0;i<sides;i++){int a=ring*sides+i,b=ring*sides+(i+1)%sides;triangles.AddRange(new[]{a,a+sides,b,b,a+sides,b+sides});}
                for(int i=1;i<sides-1;i++){triangles.AddRange(new[]{0,i,i+1});int top=(profile.Length-1)*sides;triangles.AddRange(new[]{top,top+i+1,top+i});}
                var mesh=new Mesh{name="Pin contact hull "+section};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();pinContactHulls[section]=mesh;
            }
            var child=new GameObject(new[]{"Pin lower contact","Pin neck contact","Pin head contact"}[section]);child.transform.SetParent(parent,false);var collider=child.AddComponent<MeshCollider>();collider.sharedMesh=pinContactHulls[section];collider.convex=true;return collider;
        }

        public static MeshCollider ContactCylinder(Transform parent,string name,float top,float bottom,float height,float center,int segments)
        {
            var key=new Vector4(top,bottom,height,segments);
            if(!contactMeshes.TryGetValue(key,out var mesh)||mesh==null){
                var vertices=new List<Vector3>();for(int i=0;i<segments;i++){float a=i*Mathf.PI*2/segments;vertices.Add(new Vector3(Mathf.Cos(a)*bottom,-height/2,Mathf.Sin(a)*bottom));vertices.Add(new Vector3(Mathf.Cos(a)*top,height/2,Mathf.Sin(a)*top));}vertices.Add(Vector3.down*height/2);vertices.Add(Vector3.up*height/2);var triangles=new List<int>();for(int i=0;i<segments;i++){int a=i*2,b=((i+1)%segments)*2;triangles.AddRange(new[]{a,a+1,b,b,a+1,b+1,segments*2,b,a,segments*2+1,a+1,b+1});}mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();contactMeshes[key]=mesh;
            }
            var root=new GameObject(name);root.transform.SetParent(parent,false);root.transform.localPosition=Vector3.up*center;var collider=root.AddComponent<MeshCollider>();collider.sharedMesh=mesh;collider.convex=true;return collider;
        }

        static Material blobMaterial;
        public static Transform Blob(Transform parent,float radius)
        {
            if(blobMaterial==null){var tex=new Texture2D(128,128,TextureFormat.RGBA32,false);for(int y=0;y<128;y++)for(int x=0;x<128;x++){float r=Mathf.Clamp01((Vector2.Distance(new Vector2(x,y),new Vector2(64,64))-4)/60);float alpha=r<.45f?Mathf.Lerp(.33f,.13f,r/.45f):Mathf.Lerp(.13f,0,(r-.45f)/.55f);tex.SetPixel(x,y,new Color(42/255f,35/255f,26/255f,alpha));}tex.Apply();blobMaterial=new Material(Shader.Find("PinPop/HTML Unlit"));blobMaterial.mainTexture=tex;}
            var root=new GameObject("Soft contact shadow").transform;root.SetParent(parent,false);if(!blobMeshes.TryGetValue(radius,out var mesh)||mesh==null){mesh=new Mesh{name="PinPop shared shadow"};mesh.vertices=new[]{new Vector3(-radius,0,-radius),new Vector3(radius,0,-radius),new Vector3(radius,0,radius),new Vector3(-radius,0,radius)};mesh.uv=new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up};mesh.triangles=new[]{0,2,1,0,3,2};blobMeshes[radius]=mesh;}root.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;var renderer=root.gameObject.AddComponent<MeshRenderer>();renderer.sharedMaterial=blobMaterial;renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;return root;
        }
        public static void ImpactRing(Vector3 at,Color color,float duration){var root=new GameObject("HTML impact halo");root.transform.position=at;root.AddComponent<HtmlImpactRing>().Build(color,duration);}
        public static void Burst(Vector3 at, Color color, int count = 18, bool confetti = false)
        {
            var root = new GameObject(confetti ? "Celebration confetti" : "Color burst"); root.transform.position = at;
            var ps = root.AddComponent<ParticleSystem>(); ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); var main = ps.main;
            main.useUnscaledTime=true;main.simulationSpace=ParticleSystemSimulationSpace.World;main.startColor = color; main.startLifetime = confetti ? 2.5f : .55f; main.startSpeed = confetti ? 5 : 2.8f;
            main.startSize = confetti ? .12f : .09f; main.gravityModifier = confetti ? .65f : .4f; main.loop = false; main.duration = .1f;
            var em = ps.emission; em.rateOverTime = 0; em.SetBursts(new[] { new ParticleSystem.Burst(0, (short)count) });
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = .12f;
            var rend = ps.GetComponent<ParticleSystemRenderer>(); rend.sharedMaterial = SpriteMaterial;
            var size = ps.sizeOverLifetime; size.enabled = true; size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, 1, 1, 0));
            ps.Play(); Object.Destroy(root, 4);
        }
    }
}
