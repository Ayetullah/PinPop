using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PinPop
{
    public sealed class PinPopGame : MonoBehaviour
    {
        public PinPopAdventure Adventure {get;private set;}
        public bool Initialized {get;private set;}
        public int HighestLevel=>economy?.Data.highestLevel??0;
        public readonly List<PinTarget> Pins = new List<PinTarget>();
        public readonly List<int> Queue = new List<int>();
        public int Level { get; private set; }
        public int Moves { get; private set; }
        public int Cleared { get; private set; }
        public int Total { get; private set; }
        public PinPopMatchTray MatchTray {get;private set;}
        public bool TrayOverflow {get;private set;}
        public bool Busy { get; private set; }
        public bool Finished { get; private set; }
        public bool Won { get; private set; }
        public bool VideoRescueUsed {get;private set;}
        public int AttemptId {get;private set;}
        public int BonusMoves=>VideoRescueUsed?0:Moves;
        public bool CanWatchForMoves=>!TrayOverflow&&Finished&&!Won&&Moves==0&&Cleared<Total&&!VideoRescueUsed&&!InMenu&&!Transitioning;
        public PinPopRewardedVideo RewardedVideo {get;private set;}
        public bool Muted { get; private set; }
        public bool InMenu { get; private set; } = true;
        public bool Transitioning { get; set; }
        public bool Paused => Time.timeScale == 0;
        bool deviceReducedMotion;
        public bool ReducedMotion => PlayerPrefs.GetInt("ReducedMotion", deviceReducedMotion?1:0) == 1;
        public const int LevelReward = 30, SwapPrice = 10, StarterSwaps = 3;
        public int Coins { get; private set; }
        public int SwapCredits { get; private set; }
        public int LastReward { get; private set; }
        public int LastBaseReward {get;private set;} public int LastMoveReward {get;private set;}
        PinPopEconomy economy;
        public int MapStars(int level){if(economy==null||level>=HighestLevel)return 0;var row=economy.Data.earnings.Find(x=>x.level==level);return row==null?1:row.bestMoves>=2?3:row.bestMoves==1?2:1;}
        public int SelectedBall=>economy?.Data.selectedBall??0;
        public bool OwnsBall(int id)=>economy!=null&&economy.Owns(id);
        public Camera Camera { get; private set; }
        public PinPopHUD Hud { get; private set; }
        public const int BallLayer=29,GrassLayer=30;
        public const float GrassHeight=-1.25f;
        public static readonly Vector3 Launch = new Vector3(0, .40f, -7.2f);
        public const float LaunchLimit=3.65f;
        float launchX;
        public Vector3 LaunchPosition=>new Vector3(launchX,Launch.y,Launch.z);
        public bool IsPositioning=>dragging&&positioning;
        public bool SetLaunchX(float x){
            if(InMenu||Transitioning||Paused||Busy||Finished||Moves<=0||!Finite(x))return false;
            float next=Mathf.Clamp(x,-LaunchLimit,LaunchLimit),delta=next-launchX;launchX=next;Aim+=Vector3.right*delta;
            if(readyBall!=null)readyBall.position=LaunchPosition;return true;
        }
        public Vector3 Aim { get; private set; } = new Vector3(0, .43f, 2);
        Transform levelRoot, readyBall, tableRoot;
        HtmlReferenceWorld htmlWorld;
        HtmlLook htmlLook;
        int shotCleared;
        readonly RaycastHit[] aimContacts=new RaycastHit[64];
        Transform[] aimDots;Material aimDotMaterial;
        GameObject reticle;
        public AimPowerRing PowerRing {get;private set;}
        
        public struct RollPrediction { public Vector3 Point; public float Time; public bool OnTable; }
        public RollPrediction PreviewRoll { get; private set; }
        public bool AimVisible => reticle != null && reticle.activeInHierarchy;
        [Header("Runtime camera framing")]
        [Range(1,179)] public float GameplayFieldOfView=42;
        [Range(1,179)] public float HomeFieldOfView=40;
        float lastCameraFov=-1,lastCameraAspect,lastRequestedFov;bool lastCameraHome;
        Vector3 cameraHome;
        float shake, impactCooldown,matchWinDelay;
        HtmlGameAudio sound;
        bool dragging,positioning,gestureChosen;
        float gestureStartX;
        int pointerId = -1;
        public float PullPower { get; private set; }
        public float LastShotPower { get; private set; }
        public bool IsPulling => dragging;
        public bool KeyboardAiming=>keyboardAim&&!Busy&&!Finished&&!InMenu;
        public float DisplayPower=>dragging?PullPower:KeyboardAiming?keyboardPower:0;
        Vector2 pullAnchor;
        Vector3 pullOrigin;
        float keyboardPower = .7f;
        bool keyboardAim;
        System.Collections.IEnumerator Start()
        {
            Application.targetFrameRate = 60; Application.runInBackground = true; Physics.defaultSolverIterations = 16; Physics.gravity=new Vector3(0,-20f,0);Time.fixedDeltaTime=1f/120;Time.maximumDeltaTime=1f/15;
#if UNITY_STANDALONE && !UNITY_EDITOR
            Screen.SetResolution(600, 1000, FullScreenMode.Windowed);
#endif
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if(PinPopSplash.Visible){yield return null;yield return new WaitForEndOfFrame();}
            PinPopSplash.Instance?.SetProgress(.08f,"Lunapark hazırlanıyor…");
            yield return WarmStartupAssets();
            PinPopSplash.Instance?.SetProgress(.56f,"Lunapark kuruluyor…");StartupTiming.Begin("world construction");BuildWorld();StartupTiming.End();
            PinPopSplash.Instance?.SetProgress(.62f,"Masa hazırlanıyor…");yield return null;
            PinPopSplash.Instance?.SetProgress(.64f,"Arayüz hazırlanıyor…");MatchTray=gameObject.AddComponent<PinPopMatchTray>();MatchTray.Build(this);MatchTray.Matched+=(item,combo)=>StartCoroutine(PlayTrayMatch(combo));MatchTray.Landed+=slot=>sound.Tone(523.25f*Mathf.Pow(2,new int[]{0,2,4,7,9,12,14,16,19,21,24,26,28}[Mathf.Clamp(slot,0,12)]/12f),.12f); Hud = gameObject.AddComponent<PinPopHUD>(); yield return Hud.BuildAsync(this);
            PinPopSplash.Instance?.SetProgress(.78f,"Festival hazırlanıyor…");yield return null;
            StartupTiming.Begin("economy and adventure");ReloadSettings();
            ReloadEconomy();
            RewardedVideo=gameObject.AddComponent<PinPopRewardedVideo>();RewardedVideo.Build(this);
            Adventure=gameObject.AddComponent<PinPopAdventure>();Adventure.Build(this);gameObject.AddComponent<AdventureHUD>().Build(this);StartupTiming.End();
            PinPopSplash.Instance?.SetProgress(.86f,"Masa hazırlanıyor…");yield return LoadLevelAsync(Mathf.Max(0,Mathf.Max(PlayerPrefs.GetInt("Level",0),economy.Data.highestLevel)));
            StartupTiming.Begin("home reveal");SetMenu(true); Hud.ShowHome();Initialized=true;StartupTiming.End();
            yield return null;StartupTiming.Begin("first home canvas");Canvas.ForceUpdateCanvases();StartupTiming.End();yield return new WaitForEndOfFrame();PinPopSplash.Instance?.MarkReady();
            Debug.Log("PINPOP: Startup complete; home ready, level="+Level);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var args = System.Environment.GetCommandLineArgs();
            if(System.Array.IndexOf(args,"-settings-language-review")>=0||System.Array.IndexOf(args,"-language-pref-verify")>=0){gameObject.AddComponent<LanguageSettingsReview>();yield break;}
            if(System.Array.IndexOf(args,"-crisp-ui-review")>=0){gameObject.AddComponent<CrispUIReview>();yield break;}
            bool test = System.Array.IndexOf(args,"-forty-level-test") >= 0 || System.Array.IndexOf(args, "-power-match-polish-test") >= 0 || System.Array.IndexOf(args, "-launch-position-test") >= 0 || System.Array.IndexOf(args, "-grass-lifetime-test") >= 0 || System.Array.IndexOf(args, "-map-polish-test") >= 0 || System.Array.IndexOf(args, "-animated-help-test") >= 0 || System.Array.IndexOf(args, "-adventure-test") >= 0 || System.Array.IndexOf(args, "-eight-color-test") >= 0 || System.Array.IndexOf(args, "-fov-layout-test") >= 0 || System.Array.IndexOf(args, "-grass-layout-test") >= 0 || System.Array.IndexOf(args, "-physics-review-test") >= 0 || System.Array.IndexOf(args, "-pin-behavior-test") >= 0 || System.Array.IndexOf(args, "-tray-polish-test") >= 0 || System.Array.IndexOf(args, "-smash-match-test") >= 0 || System.Array.IndexOf(args, "-recovery-test") >= 0 || System.Array.IndexOf(args, "-economy-test") >= 0 || System.Array.IndexOf(args, "-economy-progression-test") >= 0 || System.Array.IndexOf(args, "-all-layouts-test") >= 0 || System.Array.IndexOf(args, "-journey-test") >= 0 || System.Array.IndexOf(args, "-rolling-levels-test") >= 0 || System.Array.IndexOf(args, "-video-test") >= 0 || System.Array.IndexOf(args, "-sync-test") >= 0 || System.Array.IndexOf(args, "-transition-test") >= 0 || System.Array.IndexOf(args, "-parity-test") >= 0 || System.Array.IndexOf(args, "-collision-test") >= 0 || System.Array.IndexOf(args, "-self-test") >= 0 || System.Array.IndexOf(args, "-visual-test") >= 0;
            if (test) { SetMenu(false); Hud.ShowGame(); }
            if(System.Array.IndexOf(args,"-splash-review")>=0)gameObject.AddComponent<PinPopStartupRecorder>();
            if (test || System.Array.IndexOf(args,"-splash-test")>=0 || System.Array.IndexOf(args, "-polish-test") >= 0 || System.Array.IndexOf(args, "-pull-test") >= 0) gameObject.AddComponent<PinPopSelfTest>();
#endif
        }
        IEnumerator WarmStartupAssets()
        {
            var priority=Application.backgroundLoadingPriority;Application.backgroundLoadingPriority=ThreadPriority.Low;
            try {
                var requests=new List<ResourceRequest>{Resources.LoadAsync<GameObject>("Prepared/World"),Resources.LoadAsync<Mesh>("Prepared/pin-mesh")};
                for(int color=0;color<PinPopPalette.Count;color++){requests.Add(Resources.LoadAsync<Sprite>("PinIcons/pin-"+color));requests.Add(Resources.LoadAsync<Texture2D>("PinSkins/pin-"+color));}
                for(int style=0;style<4;style++)for(int color=0;color<PinPopPalette.Count;color++){requests.Add(Resources.LoadAsync<Sprite>("Prepared/Balls/icon-"+style+"-"+color));if(style>0)requests.Add(Resources.LoadAsync<Texture2D>("Prepared/Balls/coat-"+style+"-"+color));}
                while(true){int done=0;float progress=0;foreach(var request in requests){if(request.isDone)done++;progress+=request.progress;}PinPopSplash.Instance?.SetProgress(.12f+.40f*progress/requests.Count,"Renkler hazırlanıyor…");if(done==requests.Count)break;yield return null;}
            } finally {Application.backgroundLoadingPriority=priority;}
        }
        void BuildWorld()
        {
            gameObject.AddComponent<HtmlPhysicsContacts>();
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=Art.Hex("D6EBF5")*.9f;RenderSettings.ambientEquatorColor=Color.Lerp(Art.Hex("D6EBF5"),Art.Hex("65754D"),.5f)*.9f;RenderSettings.ambientGroundColor=Art.Hex("65754D")*.9f;
            RenderSettings.fog=true;RenderSettings.fogColor=Art.Hex("C3D8D6");RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=38;RenderSettings.fogEndDistance=100;
            Camera=new GameObject("Game camera").AddComponent<Camera>();Camera.tag="MainCamera";Camera.nearClipPlane=.1f;Camera.farClipPlane=150;Camera.backgroundColor=Art.Hex("BEDDE6");Camera.clearFlags=CameraClearFlags.SolidColor;Camera.gameObject.AddComponent<AudioListener>();htmlLook=Camera.gameObject.AddComponent<HtmlLook>();
            var light=new GameObject("HTML sun").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1f;light.color=Art.Hex("FFF3DB");light.transform.rotation=Quaternion.LookRotation(new Vector3(9,-19,12));light.shadows=LightShadows.Soft;light.shadowStrength=1;light.shadowBias=.025f;light.shadowNormalBias=.025f;light.shadowCustomResolution=4096;
            QualitySettings.pixelLightCount=2;QualitySettings.shadowResolution=ShadowResolution.VeryHigh;QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowDistance=45;QualitySettings.shadowCascades=4;QualitySettings.shadowCascade4Split=new Vector3(.20f,.45f,.70f);QualitySettings.antiAliasing=4;
            htmlWorld=new GameObject("HTML reference world").AddComponent<HtmlReferenceWorld>();htmlWorld.Model=this;htmlWorld.Build();tableRoot=htmlWorld.Table;
            // Extend the approach area toward the player; the pin deck stays in place.
            tableRoot.localScale=new Vector3(1,1,13.6f/9.6f);tableRoot.position+=Vector3.back*2;
            var tableCollider=new GameObject("Table physics");tableCollider.transform.SetParent(tableRoot);tableCollider.transform.position=new Vector3(0,-.05f,-2);var collider=tableCollider.AddComponent<BoxCollider>();collider.size=new Vector3(8.4f,.1f,13.6f);HtmlPhysicsContacts.Register(collider,HtmlPhysicsContacts.Kind.Floor);
            // Only balls land on the meadow. Pins still fall freely into the collection threshold.
            for(int layer=0;layer<32;layer++)Physics.IgnoreLayerCollision(GrassLayer,layer,layer!=BallLayer);
            var grass=new GameObject("Grass ball physics");grass.layer=GrassLayer;grass.transform.SetParent(transform,false);grass.transform.position=new Vector3(0,GrassHeight-.1f,0);var ground=grass.AddComponent<BoxCollider>();ground.size=new Vector3(400,.2f,400);HtmlPhysicsContacts.Register(ground,HtmlPhysicsContacts.Kind.Grass);
            aimDots=new Transform[27];aimDotMaterial=new Material(Resources.Load<Shader>("AimPowerOverlay")){renderQueue=3998};var outline=new Material(Resources.Load<Shader>("AimPowerOverlay")){color=Art.Hex("294B61"),renderQueue=3997};for(int i=0;i<aimDots.Length;i++){var dot=Art.Shape("Table aim dot "+i,PrimitiveType.Sphere,Vector3.zero,Vector3.one*.081f,outline,transform);dot.GetComponent<Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;Art.Shape("Dot color",PrimitiveType.Sphere,Vector3.zero,Vector3.one*.62f,aimDotMaterial,dot.transform).GetComponent<Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;aimDots[i]=dot.transform;dot.SetActive(false);}
            reticle = new GameObject("Aim target"); reticle.transform.SetParent(transform);
            var neutral=new Material(Shader.Find("Sprites/Default")){color=Art.Hex("FFF3D4")};
            var fill=new Material(Shader.Find("Sprites/Default")){color=new Color(1,.95f,.83f,.06f)};
            Art.LandingRing(reticle.transform,"Landing fill",0,.64f,fill,0);
            Art.LandingRing(reticle.transform,"Ball footprint",.365f,.405f,neutral,.006f);
            Art.LandingRing(reticle.transform,"Contact center",0,.045f,neutral,.008f);
            var ring=new GameObject("Aim power ring");ring.transform.SetParent(reticle.transform,false);ring.transform.localPosition=Vector3.up*.010f;PowerRing=ring.AddComponent<AimPowerRing>();
            reticle.SetActive(false);
            sound=gameObject.AddComponent<HtmlGameAudio>();sound.Build(this);
        }
        public void LoadLevel(int level){var build=PrepareLevel(level,false);while(build.MoveNext()){} }
        public IEnumerator LoadLevelAsync(int level)=>PrepareLevel(level,true);
        IEnumerator PrepareLevel(int level,bool spreadWork)
        {
            AttemptId++;matchWinDelay=0;TrayOverflow=false;MatchTray.Clear();VideoRescueUsed=false;if(RewardedVideo!=null)RewardedVideo.Cancel();
            Time.timeScale=1;if(Initialized)StopAllCoroutines();
            foreach (var ball in FindObjectsByType<BowlingBall>(FindObjectsSortMode.None)) Destroy(ball.gameObject);
            foreach(var ring in FindObjectsByType<HtmlImpactRing>(FindObjectsSortMode.None))Destroy(ring.gameObject);foreach(var ps in FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))Destroy(ps.gameObject);
            if (levelRoot != null) { levelRoot.gameObject.SetActive(false); Destroy(levelRoot.gameObject); }
            if (readyBall != null) Destroy(readyBall.gameObject);
            launchX=0;positioning=gestureChosen=false;Level = AdventureRules.ClampLevel(level); Cleared = LastReward = LastBaseReward = LastMoveReward = 0; PullPower = LastShotPower = 0; keyboardAim = false; Finished = Won = Busy = dragging = false; pointerId = -1;
            levelRoot = new GameObject("Level " + (Level + 1)).transform; levelRoot.SetParent(transform);levelRoot.gameObject.SetActive(false);
            Pins.Clear(); Queue.Clear();
            float sliceStarted=Time.realtimeSinceStartup;
            foreach(var pin in PinPopLayouts.Build(Level)){
                AddPin(pin.Position,pin.Color);
                if(spreadWork&&Time.realtimeSinceStartup-sliceStarted>.004f){yield return null;sliceStarted=Time.realtimeSinceStartup;}
            }
            if(Adventure!=null)Adventure.ResetLevel();
            Total = Pins.Count; Moves = PinPopEconomy.MoveBudget(Level);
            for (int i = 0; i < Moves + 1; i++) Queue.Add(PinPopLayouts.QueueColor(Level,i));
            Aim = new Vector3(-2.45f, .55f, .2f); pullOrigin = LaunchPosition; SpawnReadyBall(); Hud.Refresh(); Hud.HideResult();
            levelRoot.gameObject.SetActive(!InMenu); if (readyBall != null) readyBall.gameObject.SetActive(!InMenu);
            htmlWorld.SetLevel(Level);
            Hud.Toast(Level == 0 ? "AYNI RENK. TAM İSABET." : "MASAYI TEMİZLE!", 2f);
        }
        void AddPin(Vector3 pos, int color)
        {
            var g = new GameObject("Pin " + Art.Names[color]); g.transform.SetParent(levelRoot); g.transform.position = pos;
            var pin = g.AddComponent<PinTarget>(); pin.Setup(this, color); Pins.Add(pin);
            
        }
        void SpawnReadyBall()
        {
            if (readyBall != null) Destroy(readyBall.gameObject);
            if (Moves <= 0 || Finished || Cleared>=Total) return;
            readyBall = new GameObject("Next bowling ball").transform; readyBall.position = LaunchPosition;
            Art.BallVisual(readyBall, Queue[0], .39f,SelectedBall).transform.localRotation=Quaternion.Euler(5.72958f,-8.59437f,0);if(Adventure!=null&&Adventure.RainbowArmed)ShowRainbowReady();readyBall.gameObject.SetActive(!InMenu);
        }
        public bool Swap()
        {
            if (dragging || InMenu || Transitioning || Paused || Busy || Finished || Cleared>=Total || Moves <= 0 || Queue.Count < 2 || SwapCredits <= 0) return false;
            if(!economy.UseSwap())return false;int first = Queue[0]; Queue.RemoveAt(0); Queue.Add(first); SaveEconomy();
            SpawnReadyBall(); Hud.Refresh(); Hud.AnimateQueue(); sound.Tone(510,.12f); Hud.Toast("İLK TOP EN SONA GİTTİ · −1 SWAP", 2.3f); return true;
        }
        public bool BuySwap()
        {
            if(dragging||Transitioning||Busy||Finished||!Hud.SwapShopVisible||!economy.BuySwap())return false;
            SaveEconomy();sound.Tone(660,.15f);Hud.CloseOverlays();Hud.Toast("+1 SWAP ALINDI · −10 COIN",2.3f);return true;
        }
        public bool BuyBall(int id){if(dragging||Transitioning||Busy||Finished||!Hud.ShopVisible||!economy.BuyBall(id))return false;SaveEconomy();SpawnReadyBall();sound.Tone(660,.15f);Hud.Toast(GameLocalization.Format("{0} alındı ve seçildi",PinPopEconomy.BallNames[id]),2.3f);return true;}
        public bool EquipBall(int id){if(dragging||Transitioning||Busy||Finished||!Hud.ShopVisible||!economy.Equip(id))return false;SaveEconomy();SpawnReadyBall();sound.Tone(510,.1f);return true;}
        public void ReloadEconomy(){economy=PinPopEconomy.Load();SyncEconomy();}
        void SyncEconomy(){Coins=economy.Data.coins;SwapCredits=economy.Data.swaps;}
        void SaveEconomy(){economy.Save();SyncEconomy();if(Hud!=null)Hud.Refresh();}
        public bool SpendCoins(int amount){if(amount<0||economy.Data.coins<amount)return false;economy.Data.coins-=amount;SaveEconomy();return true;}
        public void GrantCoins(int amount){economy.Data.coins=Mathf.Clamp(economy.Data.coins+amount,0,PinPopEconomy.MaxCoins);SaveEconomy();}
        public void ChangeReadyColor(int color){Queue[0]=color;SpawnReadyBall();Hud.Refresh();Hud.AnimateQueue();}
        public void ShowRainbowReady(){if(readyBall!=null)readyBall.gameObject.AddComponent<RainbowBallFX>();}
        public void AddBoosterMoves(){Moves+=3;while(Queue.Count<Moves+1)Queue.Add(PinPopLayouts.QueueColor(Level,Queue.Count));Hud.Refresh();Hud.RescueEffect();}
        public static Vector3 ShotVelocity(Vector3 target, float power, Vector3 origin)
        {
            Vector3 direction = target - origin; direction.y = 0;
            return direction.normalized * Mathf.Lerp(4, 24, Mathf.Clamp01(power));
        }
        public RollPrediction PredictRoll(Vector3 target, float power, Vector3 origin)
        {
            Vector3 direction=ShotVelocity(target,power,origin).normalized;
            float distance=Mathf.Abs(direction.z)>.001f?((direction.z>0?4.4f:-8.4f)-origin.z)/direction.z:1000;
            if(Mathf.Abs(direction.x)>.001f)distance=Mathf.Min(distance,((direction.x>0?3.8f:-3.8f)-origin.x)/direction.x);
            foreach(var pin in Pins) {
                if(pin==null||pin.Cleared||!pin.gameObject.activeInHierarchy)continue;
                Vector3 delta=pin.Body.worldCenterOfMass-origin;delta.y=0;float along=Vector3.Dot(delta,direction);
                float side=delta.sqrMagnitude-along*along;
                if(along>0&&side<.65f*.65f)distance=Mathf.Min(distance,Mathf.Max(0,along-Mathf.Sqrt(.65f*.65f-side)));
            }
            if(Adventure!=null){int count=Physics.SphereCastNonAlloc(origin,.39f,direction,aimContacts,Mathf.Max(0,distance),~0,QueryTriggerInteraction.Ignore);for(int i=0;i<count;i++)if(Adventure.IsObstacle(aimContacts[i].collider))distance=Mathf.Min(distance,aimContacts[i].distance);}
            var point=origin+direction*Mathf.Max(0,distance);point.y=.035f;
            return new RollPrediction{Point=point,Time=distance/Mathf.Lerp(4,24,Mathf.Clamp01(power)),OnTable=true};
        }
        public bool Shoot(Vector3 target, float power = 1) => ShootFrom(target, power, LaunchPosition);
        public bool ShootFrom(Vector3 target, float power, Vector3 origin)
        {
            if (InMenu || Transitioning || Paused || Busy || Finished || Cleared>=Total || Moves <= 0 || !Finite(target.x)||!Finite(target.z)||!Finite(origin.x)||!Finite(origin.y)||!Finite(origin.z)||float.IsNaN(power) || float.IsInfinity(power) || power < .08f) return false;
            target.x = Mathf.Clamp(target.x, -100, 100); target.z = Mathf.Clamp(target.z, -100, 100); power = Mathf.Clamp01(power);
            origin=LaunchPosition;
            if(new Vector2(target.x-origin.x,target.z-origin.z).sqrMagnitude<.01f)return false;
            Vector3 velocity = ShotVelocity(target, power, origin);
            shotCleared = 0; Busy = true; dragging = false; keyboardAim = false; PullPower = 0; LastShotPower = power; Moves--; int color = Queue[0]; Queue.RemoveAt(0);
            if (readyBall != null) Destroy(readyBall.gameObject);
            var g = new GameObject("Launched " + Art.Names[color]); g.transform.position = origin; if(Adventure!=null)Adventure.BeginShot();g.AddComponent<BowlingBall>().Setup(this, color, velocity, power);
            sound.Tone(190+power*100,.16f,true); Hud.SetPower(0); Hud.Refresh(); StartCoroutine(ResolveShot()); return true;
        }
        IEnumerator ResolveShot()
        {
            float elapsed = 0;
            while(elapsed<8||MatchTray.Pending){
                if(TrayOverflow)yield break;
                if(Cleared>=Total&&MatchTray.Empty){while(!Finished)yield return null;yield break;}
                bool moving=Pins.Exists(p=>p!=null&&!p.Cleared&&!p.Body.isKinematic&&!p.Body.IsSleeping()&&(p.Body.linearVelocity.sqrMagnitude>.05f));
                if(elapsed>=2.8f&&!moving&&!MatchTray.Pending&&Cleared<Total)break;
                yield return null;elapsed+=Time.deltaTime;
            }
            if(Cleared>=Total){while(!Finished)yield return null;}
            else if(Moves==0)End(false);
            else {Busy=false;SpawnReadyBall();Hud.Refresh();}
        }
        public void OnPinCleared(PinTarget pin){
            Cleared++;shotCleared++;var point=pin.Body.worldCenterOfMass;var viewport=Camera.WorldToViewportPoint(point);MatchTray.Collect(pin.ColorId,new Vector2(viewport.x,viewport.y));
            foreach(var other in Pins)if(other!=null&&!other.Cleared&&!other.IsLocked)other.Body.WakeUp();
            Art.Burst(point,Art.Colors[pin.ColorId],6);Hud.CountEffect();Hud.Refresh();if(!TrayOverflow&&MatchTray.LiveCount>=PinPopMatchTray.Capacity){TrayOverflow=true;Busy=true;CancelPull();Time.timeScale=0;StartCoroutine(FinishOverflow(AttemptId));}
        }
        IEnumerator FinishOverflow(int attempt){
            while(attempt==AttemptId&&!Finished&&MatchTray.Pending)yield return null;
            yield return new WaitForSecondsRealtime(.25f);
            if(attempt==AttemptId&&TrayOverflow&&!Finished)End(false);
        }
        IEnumerator PlayTrayMatch(int combo){float basis=659.25f*Mathf.Pow(2,Mathf.Min(combo-1,6)*2/12f);foreach(int step in new[]{0,4,7,12}){sound.Tone(basis*Mathf.Pow(2,step/12f),.25f,true);yield return new WaitForSeconds(.035f);}}
        void End(bool won)
        {
            if (Finished || (won&&(Total<=0||Cleared<Total||!MatchTray.Empty))) return;
            Finished = true; Won = won; Busy = false; dragging = false;Time.timeScale=0;
            if (won)
            {
                LastReward = economy.Complete(Level,BonusMoves,out int basePaid,out int bonusPaid);LastBaseReward=basePaid;LastMoveReward=bonusPaid;SaveEconomy();
                if(Adventure!=null)Adventure.Complete();
                PlayerPrefs.SetInt("Level", Mathf.Max(PlayerPrefs.GetInt("Level",0),Level + 1)); PlayerPrefs.Save();
                for(int i=0;i<PinPopLayouts.ColorCount(Level);i++)Art.Burst(new Vector3(-2.5f+5f*i/(PinPopLayouts.ColorCount(Level)-1),2,1),Art.Colors[i],65,true);
            }
            sound.Chime(won);Hud.Refresh(); Hud.ShowResult(won);
        }
        public bool ClaimVideoMoves(int attempt)
        {
            if(attempt!=AttemptId||!CanWatchForMoves)return false;
            VideoRescueUsed=true;Finished=Won=Busy=false;Moves=3;PullPower=0;keyboardAim=dragging=false;pointerId=-1;
            foreach(var ball in FindObjectsByType<BowlingBall>(FindObjectsSortMode.None))if(!ball.PreserveOnNextShot)Destroy(ball.gameObject);
            while(Queue.Count<Moves+1)Queue.Add(Queue.Count==0?PinPopLayouts.QueueColor(Level,0):(Queue[Queue.Count-1]+1)%PinPopLayouts.ColorCount(Level));
            Hud.HideResult();if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);Time.timeScale=1;SpawnReadyBall();Hud.Refresh();Hud.AnimateQueue();Hud.RescueEffect();sound.Chime();Hud.Toast("+3 HAMLE · KALDIĞIN YERDEN DEVAM!",2.5f);return true;
        }
        public void PrepareShotColor(int color)
        {
            foreach (var pin in Pins) if (pin != null) pin.SetShotColor(Adventure!=null&&Adventure.ShotRainbow?-1:color);
        }
        public void BlockedImpact(PinTarget pin)
        {
            pin.BlockFlash(); shake = ReducedMotion ? 0 : .025f;
            Art.ImpactRing(new Vector3(pin.transform.position.x,.03f,pin.transform.position.z),Art.Hex("F2E4C8"),.22f);if (Time.time > impactCooldown) { sound.Tone(105,.07f); impactCooldown = Time.time + .06f; }
        }
        public void Impact(Vector3 at, int color, float power = 1)
        {
            shake = ReducedMotion ? 0 : .035f + power * .055f; Art.Burst(at + Vector3.up * .6f, Art.Colors[color], 5 + Mathf.RoundToInt(power * 8));
            Art.ImpactRing(new Vector3(at.x,.03f,at.z),Art.Colors[color],.38f);if (Time.time > impactCooldown) { sound.Tone(140+Mathf.Min(shotCleared,5)*35,.085f,true); impactCooldown = Time.time + .06f; }
        }
        public void SetMenu(bool home)
        {
            CancelPull(); InMenu = home; Time.timeScale = 1; Camera.orthographic = false;htmlLook.Home=home;
            cameraHome = home ? new Vector3(0, 4.95f, -14) : new Vector3(0, 20.2f, -18.4f);
            Camera.transform.position = cameraHome; Camera.transform.LookAt(home ? new Vector3(0, 1.25f, 0) : new Vector3(0, .2f, -2));
            if (tableRoot != null) tableRoot.gameObject.SetActive(!home);
            if (levelRoot != null) levelRoot.gameObject.SetActive(!home);
            if (readyBall != null) readyBall.gameObject.SetActive(!home);
            if(htmlWorld!=null)htmlWorld.SetHome(home);
        }
        public void ToggleReducedMotion() { PlayerPrefs.SetInt("ReducedMotion", ReducedMotion ? 0 : 1); PlayerPrefs.Save(); Hud.Refresh(); }
        public void AnimateEntrance() { if (!ReducedMotion) StartCoroutine(Entrance()); }
        IEnumerator Entrance()
        {
            float duration=.42f+Mathf.Max(0,Pins.Count-1)*.014f;
            for(float time=0;time<duration;time+=Time.unscaledDeltaTime){if(ReducedMotion)break;for(int i=0;i<Pins.Count;i++){var pin=Pins[i];if(pin==null)continue;float t=Mathf.Clamp01((time-i*.014f)/.42f),scale=1+2.18f*Mathf.Pow(t-1,3)+1.18f*Mathf.Pow(t-1,2);pin.Visual.localScale=Vector3.one*Mathf.Max(.001f,scale);}yield return null;}
            foreach(var pin in Pins)if(pin!=null)pin.Visual.localScale=Vector3.one;
        }
        public void ReloadSettings(){deviceReducedMotion=HtmlPlatformPreferences.ReduceMotion();Muted=PlayerPrefs.GetInt("Muted",1)==1;if(Hud!=null)Hud.Refresh();}
        public void RefreshLanguage(){htmlWorld?.SetLevel(Level);}
        public void ToggleSound(){Muted=!Muted;if(RewardedVideo!=null)RewardedVideo.SyncSound();PlayerPrefs.SetInt("Muted",Muted?1:0);PlayerPrefs.Save();Hud.Refresh();sound.Tone(600);}
        public void PlayEntryChime(){sound.Chime();}
        static bool Finite(float value)=>!float.IsNaN(value)&&!float.IsInfinity(value);
        static float FitFov(float value,float aspect)=>2*Mathf.Atan(Mathf.Tan(Mathf.Clamp(value,1,179)*.5f*Mathf.Deg2Rad)*Mathf.Max(1,.6f/aspect))*Mathf.Rad2Deg;
        void RefreshCameraProjection(){
            // Capture edits on the actual Camera component instead of replacing them next frame.
            float previousRequest=lastCameraHome?HomeFieldOfView:GameplayFieldOfView;
            if(lastCameraFov>0&&Mathf.Abs(Camera.fieldOfView-lastCameraFov)>.001f&&Mathf.Approximately(previousRequest,lastRequestedFov)){
                float value=2*Mathf.Atan(Mathf.Tan(Camera.fieldOfView*.5f*Mathf.Deg2Rad)/Mathf.Max(1,.6f/lastCameraAspect))*Mathf.Rad2Deg;
                if(lastCameraHome)HomeFieldOfView=value;else GameplayFieldOfView=value;
            }
            float aspect=Camera.aspect;if(!InMenu&&Hud!=null)aspect*=GetComponent<HtmlReferenceHUD>().GameplayWidthFraction;
            float requested=InMenu?HomeFieldOfView:GameplayFieldOfView;float fitted=FitFov(requested,aspect);
            if(Mathf.Abs(Camera.fieldOfView-fitted)>.0001f)Camera.fieldOfView=fitted;
            lastCameraFov=Camera.fieldOfView;lastCameraAspect=aspect;lastCameraHome=InMenu;lastRequestedFov=requested;
        }
        void OnApplicationFocus(bool focused){if(!focused&&Hud!=null)CancelPull();}
        void OnApplicationPause(bool paused){if(paused&&Hud!=null)CancelPull();}
        void Update()
        {
            if (!Initialized || Camera == null) return;
            if(!InMenu&&!Finished&&!Transitioning&&Total>0&&Cleared>=Total&&MatchTray.Empty){if(!Paused)matchWinDelay+=Time.deltaTime;if(matchWinDelay>=.7f)End(true);}else matchWinDelay=0;
            RefreshCameraProjection();
            Camera.ResetProjectionMatrix();var projection=Camera.projectionMatrix;projection.m12=InMenu?.04f:-.015f;projection.m02=InMenu?0:GetComponent<HtmlReferenceHUD>().GameplayProjectionOffsetX;Camera.projectionMatrix=projection;
            shake = Paused||ReducedMotion||InMenu?0:Mathf.MoveTowards(shake,0,Time.unscaledDeltaTime*.35f); Camera.transform.position = cameraHome + Random.insideUnitSphere * shake;
            bool ready = !InMenu && !Transitioning && !Paused && !Busy && !Finished && Moves > 0;
            RefreshAimPreview();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(DevicePolishReview.Active||AuditFixesReview.Active||PopupCloseReview.Active)return;
#endif
            if(GetComponent<AdventureHUD>()?.ModalVisible==true)return;
            if(Input.GetKeyDown(KeyCode.Escape)&&(Hud.ShopVisible||Hud.SwapShopVisible||Hud.SettingsVisible||Hud.HelpVisible)){Hud.CloseOverlays();return;}
            if (InMenu || Transitioning || Paused || Hud.ResultVisible) return;
            if (Input.GetKeyDown(KeyCode.Escape)) { if (dragging||keyboardAim) CancelPull(); else Hud.ShowSettings(); return; }
            bool uiFocus=EventSystem.current!=null&&EventSystem.current.currentSelectedGameObject!=null&&EventSystem.current.currentSelectedGameObject.activeInHierarchy&&EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>()!=null;
            if (!uiFocus&&Input.GetKeyDown(KeyCode.R)) { Hud.PlayLevel(Level); return; }
            if (!uiFocus&&Input.GetKeyDown(KeyCode.Space)) Swap();
            if (!ready) return;
            if(!uiFocus&&!dragging&&(Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.RightArrow))){
                int side=Input.GetKeyDown(KeyCode.LeftArrow)?-1:1;
                if(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)){float angle=Mathf.Atan2(Aim.x-LaunchPosition.x,Aim.z-LaunchPosition.z)+side*3*Mathf.Deg2Rad;Aim=LaunchPosition+new Vector3(Mathf.Sin(angle)*10,0,Mathf.Cos(angle)*10);keyboardAim=true;}
                else SetLaunchX(launchX+side*.35f);
            }
            if (!uiFocus&&Input.GetKeyDown(KeyCode.UpArrow)) { keyboardPower=Mathf.Min(1,keyboardPower+.1f);keyboardAim=true;Hud.SetPower(keyboardPower); }
            if (!uiFocus&&Input.GetKeyDown(KeyCode.DownArrow)) { keyboardPower=Mathf.Max(.1f,keyboardPower-.1f);keyboardAim=true;Hud.SetPower(keyboardPower); }
            if (!uiFocus&&Input.GetKeyDown(KeyCode.Return)) { Shoot(Aim,keyboardPower);return; }
            if (Input.touchCount > 0)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began) PointerDown(t.position, t.fingerId);
                if (dragging && t.fingerId == pointerId) { UpdatePull(t.position); if (t.phase == TouchPhase.Ended) PointerUp(); if (t.phase == TouchPhase.Canceled) CancelPull(); }
            }
            else
            {
                if (Input.GetMouseButtonDown(0)) PointerDown(Input.mousePosition, -1);
                if (dragging) { UpdatePull(Input.mousePosition); if (Input.GetMouseButtonUp(0)) PointerUp(); }
            }
        }
        void RefreshAimPreview()
        {
            bool ready = !InMenu && !Transitioning && !Paused && !Busy && !Finished && Moves > 0;
            bool preview = ready && ((dragging&&!positioning&&PullPower>.02f) || keyboardAim);
            foreach(var dot in aimDots)dot.gameObject.SetActive(preview);reticle.SetActive(preview);
            if(!preview){PowerRing.SetProgress(0,ReducedMotion);return;}
            Vector3 origin=dragging?pullOrigin:LaunchPosition;float power=dragging?PullPower:keyboardPower;
            var prediction=PredictRoll(Aim,power,origin);PreviewRoll=prediction;
            Color color=Art.Colors[Queue[0]];
            PowerRing.SetProgress(power,ReducedMotion);
            reticle.transform.position=prediction.Point;reticle.transform.localScale=Vector3.one*.6f;
            PowerRing.transform.rotation=Camera.transform.rotation*Quaternion.Euler(-90,0,0);
            float pixelsPerUnit=Mathf.Abs(Camera.WorldToScreenPoint(prediction.Point+Camera.transform.right).x-Camera.WorldToScreenPoint(prediction.Point).x);
            PowerRing.transform.localScale=Vector3.one*(36*(Camera.pixelWidth/390f)/Mathf.Max(.01f,pixelsPerUnit)/(1.6f*.6f));
            aimDotMaterial.SetVector("_Color",new Vector4(color.linear.r,color.linear.g,color.linear.b,.82f));
            Vector3 start=origin;start.y=.035f;start=Vector3.MoveTowards(start,prediction.Point,.46f);
            float screenLength=Vector2.Distance(Camera.WorldToScreenPoint(start),Camera.WorldToScreenPoint(prediction.Point));int count=Mathf.Clamp(Mathf.FloorToInt(screenLength/(9*Camera.pixelWidth/390f)),3,aimDots.Length);for(int i=0;i<aimDots.Length;i++){var dot=aimDots[i];dot.gameObject.SetActive(i<count);if(i>=count)continue;dot.position=Vector3.Lerp(start,prediction.Point,(float)i/(count-1));float ppu=Mathf.Abs(Camera.WorldToScreenPoint(dot.position+Camera.transform.right).x-Camera.WorldToScreenPoint(dot.position).x);dot.localScale=Vector3.one*(5*Camera.pixelWidth/390f/Mathf.Max(.01f,ppu));}
        }
        void PointerDown(Vector2 pos,int id){
            if(dragging||InMenu||Transitioning||Paused||Busy||Finished||Moves<=0||Cleared>=Total)return;
            if(EventSystem.current!=null){var evt=new PointerEventData(EventSystem.current){position=pos,pointerId=id};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(evt,hits);if(hits.Exists(h=>h.gameObject.GetComponentInParent<UnityEngine.UI.Button>()!=null))return;}
            Vector2 ball=Camera.WorldToScreenPoint(LaunchPosition);if(Vector2.Distance(pos,ball)>Mathf.Max(32,48*Camera.pixelWidth/600f))return;
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);dragging=true;keyboardAim=false;pointerId=id;positioning=gestureChosen=false;gestureStartX=launchX;pullAnchor=pos;pullOrigin=LaunchPosition;PullPower=0;Hud.SetPower(0);Hud.Refresh();
        }
        public void UpdatePull(Vector2 screen){
            if(!dragging)return;float dx=(screen.x-pullAnchor.x)*600/Camera.pixelWidth,dy=(pullAnchor.y-screen.y)*1000/Camera.pixelHeight;
            if(!gestureChosen&&Mathf.Sqrt(dx*dx+dy*dy)>12){gestureChosen=true;positioning=Mathf.Abs(dx)>Mathf.Abs(dy)*1.3f;}
            if(positioning){
                float left=Camera.WorldToScreenPoint(new Vector3(-LaunchLimit,Launch.y,Launch.z)).x,right=Camera.WorldToScreenPoint(new Vector3(LaunchLimit,Launch.y,Launch.z)).x;
                SetLaunchX(gestureStartX+(screen.x-pullAnchor.x)/(right-left)*(LaunchLimit*2));pullOrigin=LaunchPosition;PullPower=0;Hud.SetPower(0);RefreshAimPreview();return;
            }
            float length=Mathf.Sqrt(dx*dx+dy*dy);if(length>140){dx*=140/length;dy*=140/length;}PullPower=Mathf.Clamp01((Mathf.Sqrt(dx*dx+dy*dy)-12)/128);pullOrigin=LaunchPosition;Vector2 direction=new Vector2(-dx,dy).normalized;Aim=LaunchPosition+new Vector3(direction.x*10,0,direction.y*10);if(readyBall!=null)readyBall.position=LaunchPosition;Hud.SetPower(PullPower);RefreshAimPreview();
        }
        public void BeginPullForVerification(Vector2 pos) { PointerDown(pos, -1); }
        public void CancelPull()
        {
            dragging = positioning = gestureChosen = false; keyboardAim=false; pointerId = -1; PullPower = 0; pullOrigin = LaunchPosition;
            if (readyBall != null) readyBall.position = LaunchPosition; if (Hud != null) { Hud.SetPower(0); Hud.Refresh(); } if (aimDots != null) RefreshAimPreview();
        }
        public void PointerUp()
        {
            if (!dragging) return;
            bool moved=positioning;float power = PullPower; Vector3 origin = pullOrigin; CancelPull();if(moved)return;
            if (power >= .08f) ShootFrom(Aim, power, origin); else Hud.Toast("TOPU GERİYE ÇEKEREK GÜÇ VER", 1.5f);
        }
    }
}
