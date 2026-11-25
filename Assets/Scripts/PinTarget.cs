using System.Collections;
using UnityEngine;

namespace PinPop
{
    public sealed class PinTarget : MonoBehaviour
    {
        public Transform Visual {get;private set;}
        public Renderer SkinRenderer {get;private set;}
        public int ColorId { get; private set; }
        public bool Cleared { get; private set; }
        public Rigidbody Body { get; private set; }
        public Collider Hitbox { get; private set; }
        public Collider[] Hitboxes {get;private set;}
        public bool Iced {get;private set;}public bool Banded {get;private set;}bool releasedThisShot;Transform iceVisual,bandVisual;
        static Material iceMaterial;
        static Material IceMaterial {get{if(iceMaterial==null)iceMaterial=new Material(Shader.Find("Sprites/Default")){name="Shared pin ice",color=new Color(.45f,.88f,1,.38f)};return iceMaterial;}}
        PinPopGame game;
        Coroutine flash;Transform blob;
        public bool IsLocked => !Cleared && Body.isKinematic;
        public void Setup(PinPopGame owner, int color)
        {
            StartupTiming.Begin("pin setup");game = owner; ColorId = color;
            Visual=new GameObject("Printed pin visual").transform;Visual.SetParent(transform,false);Visual.gameObject.AddComponent<MeshFilter>().sharedMesh=Art.PinMesh();SkinRenderer=Visual.gameObject.AddComponent<MeshRenderer>();SkinRenderer.sharedMaterial=Art.PinCoat(color);
            StartupTiming.Begin("pin contacts");var lower=Art.PinContactHull(transform,0);var neck=Art.PinContactHull(transform,1);var head=Art.PinContactHull(transform,2);
            Hitboxes=new Collider[]{lower,neck,head};Hitbox=lower;foreach(var c in Hitboxes){c.contactOffset=.004f;c.sharedMaterial=BowlingBall.ContactMaterial;HtmlPhysicsContacts.Register(c,HtmlPhysicsContacts.Kind.Pin);}
            StartupTiming.End();StartupTiming.Begin("pin blob");blob=Art.Blob(transform.parent,.42f);StartupTiming.End();
            Body = gameObject.AddComponent<Rigidbody>(); Body.mass = 1; Body.isKinematic = false; Body.linearDamping = .04f; Body.angularDamping = .08f;
            Body.interpolation = RigidbodyInterpolation.Interpolate; Body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;Body.solverIterations=24;Body.solverVelocityIterations=12;
            Art.Shape("Pin rubber sole", PrimitiveType.Cylinder, new Vector3(0,.006f,0), new Vector3(.30f,.006f,.30f), Art.Mat(Art.Hex("46505B")), Visual);
            Body.sleepThreshold=.02f;Body.centerOfMass = new Vector3(0, .42f, 0); Body.maxAngularVelocity = 32; Body.Sleep();StartupTiming.End();
        }
        public void SetShotColor(int color)
        {
            if (Cleared) return;
            releasedThisShot=false;bool locked = color>=0&&ColorId!=color||Iced||Banded;
            if (locked && !Body.isKinematic) { Body.linearVelocity = Vector3.zero; Body.angularVelocity = Vector3.zero; }
            if(locked)Body.collisionDetectionMode=CollisionDetectionMode.ContinuousSpeculative;
            Body.isKinematic = locked;
            if(!locked)Body.collisionDetectionMode=CollisionDetectionMode.ContinuousSpeculative;
        }
        public void Protect(bool ice,bool band){Iced|=ice;Banded|=band;if(ice&&iceVisual==null){var m=IceMaterial;iceVisual=Art.Shape("Cracked ice shell",PrimitiveType.Cube,new Vector3(0,.75f,0),new Vector3(.64f,1.54f,.64f),m,Visual).transform;foreach(int side in new[]{-1,1}){var crack=Art.Shape("Ice fracture",PrimitiveType.Cube,new Vector3(side*.12f,side*.08f,-.51f),new Vector3(.022f,.30f,.008f),Art.Mat(Art.Hex("DFFAFF")),iceVisual);crack.transform.localRotation=Quaternion.Euler(0,0,side*28);}}
            if(band&&bandVisual==null)bandVisual=Art.Shape("Gold binding band",PrimitiveType.Cylinder,new Vector3(0,.44f,0),new Vector3(.62f,.07f,.62f),Art.Mat(Art.Hex("F3CA62")),Visual).transform;
        }
        public void ReleaseBand(){Banded=false;releasedThisShot=true;if(bandVisual!=null){Art.Burst(transform.position+Vector3.up*.5f,Art.Hex("F3CA62"),10);Destroy(bandVisual.gameObject);}}
        public bool BreakProtection(int color){if(Cleared||(color>=0&&color!=ColorId))return false;if(Iced){Iced=false;releasedThisShot=true;if(iceVisual!=null)Destroy(iceVisual.gameObject);Art.Burst(transform.position+Vector3.up,Art.Hex("9DE7F8"),14);game.Hud.Toast("BUZ KIRILDI · SONRAKİ ATIŞTA SERBEST",2);return true;}if(Banded){game.Adventure.BreakBand();return true;}return releasedThisShot;}
        public void BlockFlash() { if (flash != null) StopCoroutine(flash); flash = StartCoroutine(Flash()); }
        IEnumerator Flash()
        {
            var renderer = SkinRenderer; var props = new MaterialPropertyBlock(); var original = Color.black;
            for (float t = 0; t < .18f; t += Time.deltaTime) { props.SetColor("_EmissionColor", Art.Hex("FFE4B2") * (.75f * (1 - t / .18f))); renderer.SetPropertyBlock(props); yield return null; }
            props.SetColor("_EmissionColor", original); renderer.SetPropertyBlock(props); flash = null;
        }
        public void MarkContact() { if (!Cleared && !IsLocked) Body.WakeUp(); }
        public void Hit(Vector3 direction, float force)
        {
            if (Cleared || IsLocked || force <= 0) return;
            Body.WakeUp();
            Body.AddForceAtPosition(direction.normalized * force, transform.position + Vector3.up * .65f, ForceMode.Impulse);
        }
        void OnCollisionEnter(Collision collision)
        {
            if (Cleared) return;
            var other = collision.gameObject.GetComponentInParent<PinTarget>();
            if (other != null && other.ColorId == ColorId && collision.relativeVelocity.sqrMagnitude > .04f) { MarkContact(); other.MarkContact(); }
        }
        void Update()
        {
            if(blob!=null){bool onTable=PinPopGame.OnTable(transform.position);blob.position=new Vector3(transform.position.x,onTable?.008f:-1.26f,transform.position.z);blob.localScale=Vector3.one*Mathf.Max(.05f,Visual.localScale.x);}
            if(Cleared||IsLocked||game.TrayOverflow)return;
            if(Body.worldCenterOfMass.y<PinPopMatchTray.CollectHeight){
                Cleared=true;game.OnPinCleared(this);foreach(var collider in Hitboxes)collider.enabled=false;gameObject.SetActive(false);Destroy(gameObject);
            }
        }
        void OnDestroy(){if(blob!=null)Destroy(blob.gameObject);}
    }
}
