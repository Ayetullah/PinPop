using UnityEngine;

namespace PinPop
{
    public sealed class BowlingBall : MonoBehaviour
    {
        public const float Radius=.39f;
        public bool Rainbow {get;private set;}
        public int ColorId { get; private set; }
        PinPopGame game;
        float strength;
        Vector3 preStepVelocity;
        public int ContactCount { get; private set; }
        public Vector3 IncomingVelocity { get; private set; }
        public Vector3 FirstIncomingVelocity { get; private set; }
        public Vector3 OutgoingVelocity { get; private set; }
        public Vector3 LastContactNormal { get; private set; }
        Rigidbody body;Collider ballCollider;bool grassContact;float lastGrassContact=-10;
        public bool HasReachedGrass {get;private set;}
        public bool PreserveOnNextShot=>HasReachedGrass||(body!=null&&body.position.y<-.1f&&!PinPopGame.OnTable(body.position));
        public bool OnGrass=>grassContact&&Time.fixedTime-lastGrassContact<Time.fixedDeltaTime*3;
        public bool OnTable=>!HasReachedGrass&&tableContact&&body!=null&&PinPopGame.OnTable(body.position)&&Mathf.Abs(body.position.y-Radius)<.08f;
        public const float GrassRollingResistance=5f;
        public const float GrassSpeedLimit=3.5f, EdgeSpeedLimit=4.5f, GrassLifetime=2f;
        float grassAge,tableRest;bool tableContact;
        public float GrassAge=>grassAge;
        public bool IsDespawning {get;private set;}
        void Update(){if(IsDespawning)return;if(HasReachedGrass){grassAge+=Time.deltaTime;if(grassAge>=GrassLifetime)Despawn();}}
        public bool Despawn(){
            if(IsDespawning)return false;IsDespawning=true;
            Vector3 at=transform.position;
            if(ballCollider!=null)ballCollider.enabled=false;
            if(body!=null){body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;body.useGravity=false;body.isKinematic=true;body.position=at;}
            transform.position=at;
            var trail=GetComponent<TrailRenderer>();if(trail!=null)trail.emitting=false;
            var rainbow=GetComponent<RainbowBallFX>();if(rainbow!=null)rainbow.enabled=false;
            gameObject.AddComponent<BallDespawnFX>().Build(transform.Find("Polished bowling ball"),Art.Colors[ColorId],Rainbow);return true;
        }
        void LimitHorizontalSpeed(float limit){var v=body.linearVelocity;var h=Vector3.ClampMagnitude(new Vector3(v.x,0,v.z),limit);body.linearVelocity=new Vector3(h.x,v.y,h.z);body.angularVelocity=new Vector3(h.z,0,-h.x)/Radius;}
        readonly System.Collections.Generic.HashSet<PinTarget> hit = new System.Collections.Generic.HashSet<PinTarget>();
        static PhysicsMaterial contactMaterial;
        public static PhysicsMaterial ContactMaterial
        {
            get
            {
                if (contactMaterial == null) contactMaterial = new PhysicsMaterial("Bowling contact") { dynamicFriction = .45f, staticFriction = .45f, bounciness = .05f, frictionCombine = PhysicsMaterialCombine.Average, bounceCombine = PhysicsMaterialCombine.Average };
                return contactMaterial;
            }
        }
        public void Setup(PinPopGame owner, int color, Vector3 velocity, float power)
        {
            game = owner; ColorId = color; Rainbow=owner.Adventure!=null&&owner.Adventure.ShotRainbow; strength = power;gameObject.layer=PinPopGame.BallLayer;
            Art.BallVisual(transform, color, Radius,owner.SelectedBall);
            var c = gameObject.AddComponent<SphereCollider>(); ballCollider=c;c.radius = Radius; c.sharedMaterial = ContactMaterial;HtmlPhysicsContacts.Register(c,HtmlPhysicsContacts.Kind.Ball);
            body = gameObject.AddComponent<Rigidbody>(); body.mass = 6f; // Bowling weight stays constant; pull controls speed.
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; body.interpolation = RigidbodyInterpolation.Interpolate;
            body.linearDamping = .015f; body.angularDamping = .08f;body.maxAngularVelocity=100;
            foreach (var previous in FindObjectsByType<BowlingBall>(FindObjectsSortMode.None)) if (previous != this&&!previous.PreserveOnNextShot) previous.Despawn();
            if(Rainbow)gameObject.AddComponent<RainbowBallFX>();
            owner.PrepareShotColor(color);
            body.linearVelocity = velocity;
            body.angularVelocity = new Vector3(velocity.z, 0, -velocity.x) / Radius;
            var trail = gameObject.AddComponent<TrailRenderer>(); trail.time = .22f; trail.startWidth = .24f; trail.endWidth = 0;
            trail.sharedMaterial = Art.SpriteMaterial; trail.startColor = new Color(Art.Colors[color].r, Art.Colors[color].g, Art.Colors[color].b, .65f); trail.endColor = Color.clear;

        }
        void FixedUpdate() {
            if(body==null||IsDespawning)return;
            var position=body.position;
            // Only a supported, settled ball is retired. A slow moving or airborne shot stays playable.
            bool resting=OnTable&&(body.IsSleeping()||(body.linearVelocity.sqrMagnitude<.0144f&&body.angularVelocity.sqrMagnitude<.49f));
            tableRest=resting?tableRest+Time.fixedDeltaTime:0;
            if(tableRest>=.08f){Despawn();return;}
            // Slow only balls that have left the wooden playing surface.
            if(!HasReachedGrass&&position.y<.4f&&!PinPopGame.OnTable(position))LimitHorizontalSpeed(EdgeSpeedLimit);
            if(OnGrass&&body.position.y<=PinPopGame.GrassHeight+.47f){
                var v=body.linearVelocity;var horizontal=new Vector3(v.x,0,v.z);var next=Vector3.MoveTowards(horizontal,Vector3.zero,GrassRollingResistance*Time.fixedDeltaTime);body.linearVelocity=new Vector3(next.x,v.y,next.z);
                body.angularVelocity=new Vector3(next.z,0,-next.x)/Radius;
                if(next.sqrMagnitude<.0004f&&Mathf.Abs(v.y)<.1f){body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;body.Sleep();}
            }
            preStepVelocity=body.linearVelocity;
        }
        void TrackGrass(Collision collision){if(IsDespawning||collision.gameObject.layer!=PinPopGame.GrassLayer)return;if(!HasReachedGrass){HasReachedGrass=true;grassAge=0;LimitHorizontalSpeed(GrassSpeedLimit);foreach(var pin in game.Pins)if(pin!=null&&!pin.Cleared)foreach(var collider in pin.Hitboxes)Physics.IgnoreCollision(ballCollider,collider);}grassContact=true;lastGrassContact=Time.fixedTime;}
        void TrackTable(Collision collision){if(collision.gameObject.name=="Table physics")tableContact=true;}
        void OnCollisionStay(Collision collision){TrackTable(collision);TrackGrass(collision);}
        void OnCollisionExit(Collision collision){if(collision.gameObject.name=="Table physics"){tableContact=false;tableRest=0;}if(collision.gameObject.layer==PinPopGame.GrassLayer)grassContact=false;}
        void OnCollisionEnter(Collision collision)
        {
            if(IsDespawning)return;
            TrackTable(collision);
            TrackGrass(collision);
            var pin = collision.gameObject.GetComponentInParent<PinTarget>();
            if (pin == null || pin.Cleared || !hit.Add(pin)) return;
            // PhysX has resolved both bodies using the actual contact normal and inertia.
            // Do not redirect velocity or add a second, artificial impact impulse here.
            if(ContactCount==0)FirstIncomingVelocity=preStepVelocity;
            ContactCount++; IncomingVelocity = preStepVelocity; OutgoingVelocity = body.linearVelocity; LastContactNormal = collision.GetContact(0).normal;
            if(pin.BreakProtection(Rainbow?-1:ColorId))return;
            if (Rainbow||pin.ColorId == ColorId) { pin.MarkContact(); game.Impact(collision.GetContact(0).point, ColorId, strength); }
            else game.BlockedImpact(pin);
        }
    }
}
