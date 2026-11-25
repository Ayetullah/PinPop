#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PinPop
{
    // Invoked only by the explicit -self-test launch flag; tests real runtime physics.
    public sealed class PinPopSelfTest : MonoBehaviour
    {
        PinPopGame game;
        int failures, assertions, savedLevel;
        string folder;
        int savedCoins, savedSwaps;
        string savedAdventure;bool hadAdventure;
        bool hadTutorial;int savedTutorial;
        bool walletBackedUp;bool hadCoins, hadSwaps;bool hadEconomySave;string savedEconomySave;
        IEnumerator Start()
        {
            Application.logMessageReceived+=OnRuntimeLog;game = GetComponent<PinPopGame>(); savedLevel = PlayerPrefs.GetInt("Level", 0);
            hadTutorial=PlayerPrefs.HasKey(PinPopHUD.TutorialSeenKey);savedTutorial=PlayerPrefs.GetInt(PinPopHUD.TutorialSeenKey,0);
            hadAdventure=PlayerPrefs.HasKey(PinPopAdventure.SaveKey);savedAdventure=PlayerPrefs.GetString(PinPopAdventure.SaveKey,"");
            hadCoins = PlayerPrefs.HasKey("Coins"); hadSwaps = PlayerPrefs.HasKey("SwapCredits");
            savedCoins = PlayerPrefs.GetInt("Coins", 0); savedSwaps = PlayerPrefs.GetInt("SwapCredits", PinPopGame.StarterSwaps);
            hadEconomySave=PlayerPrefs.HasKey(PinPopEconomy.SaveKey);savedEconomySave=PlayerPrefs.GetString(PinPopEconomy.SaveKey,"");walletBackedUp=true;PlayerPrefs.DeleteKey(PinPopEconomy.SaveKey);PlayerPrefs.SetInt("Level",0);PlayerPrefs.SetInt("Coins", 0); PlayerPrefs.SetInt("SwapCredits", 3); game.ReloadEconomy();
            folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../../Documentation"));
            // The build is Project/Builds/PinPop.app/Contents/Resources/Data.
            string overridePath = Environment.GetEnvironmentVariable("PINPOP_QA_DIR");
            if (!string.IsNullOrEmpty(overridePath)) folder = overridePath;
            Directory.CreateDirectory(folder);
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-forty-level-test")>=0){yield return FortyLevelChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"forty-level-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-launch-position-test")>=0){yield return LaunchPositionChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"launch-position-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-grass-lifetime-test")>=0){yield return GrassLifetimeChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"grass-lifetime-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-splash-test")>=0){yield return SplashChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"splash-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-power-match-polish-test")>=0){yield return PowerMatchPolishChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"power-match-polish-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-map-polish-test")>=0){yield return CarnivalMapChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"carnival-map-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-animated-help-test")>=0){yield return AnimatedHelpChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"animated-help-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-adventure-test")>=0){yield return AdventureChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"adventure-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-eight-color-test")>=0){yield return EightColorChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"eight-color-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-fov-layout-test")>=0){yield return FovLayoutChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"fov-layout-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-grass-layout-test")>=0){yield return GrassLayoutChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"grass-layout-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-physics-review-test")>=0){yield return PhysicsReviewChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"physics-review-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-pin-behavior-test")>=0){yield return PinBehaviorChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"pin-behavior-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-tray-polish-test")>=0){yield return TrayPolishChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"tray-polish-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-smash-match-test")>=0){yield return SmashMatchChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"smash-match-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-recovery-test")>=0){yield return RecoveryChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"recovery-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-economy-test")>=0){yield return EconomyChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"economy-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-journey-test")>=0){yield return JourneyChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"journey-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;}
            if(Array.IndexOf(Environment.GetCommandLineArgs(),"-rolling-levels-test")>=0||Array.IndexOf(Environment.GetCommandLineArgs(),"-all-layouts-test")>=0||Array.IndexOf(Environment.GetCommandLineArgs(),"-economy-progression-test")>=0){
                yield return RollingLevelChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,Array.IndexOf(Environment.GetCommandLineArgs(),"-economy-progression-test")>=0?"economy-progression-unity-verification.txt":Array.IndexOf(Environment.GetCommandLineArgs(),"-all-layouts-test")>=0?"all-layouts-unity-verification.txt":"rolling-levels-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-video-test") >= 0)
            {
                yield return VideoReviewChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"video-review-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-sync-test") >= 0)
            {
                yield return SynchronizationChecks();RestoreWallet();File.WriteAllText(Path.Combine(folder,"html-sync-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-transition-test") >= 0)
            {
                int motion=PlayerPrefs.GetInt("ReducedMotion",0);PlayerPrefs.SetInt("ReducedMotion",0);
                Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.4f);
                game.SetMenu(true);game.Hud.ShowHome();int coins=game.Coins,swaps=game.SwapCredits;
                float normalStarted=Time.realtimeSinceStartup;game.Hud.PlayLevel(1);game.Hud.PlayLevel(8);yield return new WaitForSecondsRealtime(.12f);
                var art=FindFirstObjectByType<BowlingTransition>();
                CheckGraphicRaycasts(art);
                Check(game.Transitioning && art!=null && art.gameObject.activeInHierarchy,"Bowling artwork is visible during guarded transition");
                Check(!game.Shoot(Vector3.forward)&&!game.Swap(),"Transition blocks shooting and swapping");
                Check(art.GetComponentsInChildren<UnityEngine.UI.Text>()[2].text==GameLocalization.Format("BÖLÜM {0}",2),"Transition names requested next level before loading");
                // Exercise the same-frame rebuild path: deferred Destroy must not leave stale raycast graphics.
                for(int rebuild=0;rebuild<3;rebuild++){
                    FindFirstObjectByType<HtmlReferenceHUD>().SendMessage("Rebuild");
                    art=FindFirstObjectByType<BowlingTransition>();CheckGraphicRaycasts(art,false);
                }
                yield return null;art=FindFirstObjectByType<BowlingTransition>();CheckGraphicRaycasts(art);
                Capture("transition-unity-start.png");float firstProgress=art.Progress;
                yield return new WaitForSecondsRealtime(.44f);Check(game.Transitioning&&art.GetComponent<CanvasGroup>().alpha>.95f,"Normal loading screen stays fully visible long enough to read");Check(art.Progress>firstProgress,"Bowling artwork animation advances on unscaled time");Capture("transition-unity-impact.png");
                while(game.Transitioning&&Time.realtimeSinceStartup-normalStarted<4)yield return null;Check(Time.realtimeSinceStartup-normalStarted>=1.4f,"Normal transition respects minimum readable duration");Check(!game.Transitioning&&game.Level==1&&!art.gameObject.activeSelf,"Transition enters exactly one requested level and closes");
                Check(game.Coins==coins&&game.SwapCredits==swaps,"Transition preserves coins and swap credits");
                Screen.SetResolution(390,844,false);yield return new WaitForSecondsRealtime(.3f);game.Hud.PlayLevel(2);yield return new WaitForSecondsRealtime(.3f);
                art=FindFirstObjectByType<BowlingTransition>();var bounds=art.rectTransform.rect;
                Check(bounds.width<=Screen.width&&bounds.height<=Screen.height,"Transition artwork fits phone viewport");Capture("transition-unity-phone.png",390,844);
                float beforeResize=art.Progress;Screen.SetResolution(320,740,false);yield return new WaitForSecondsRealtime(.2f);Check(FindFirstObjectByType<BowlingTransition>().Progress>=beforeResize,"Resizing preserves loading animation progress");CheckGraphicRaycasts(FindFirstObjectByType<BowlingTransition>(),false);
                Screen.SetResolution(390,844,false);yield return new WaitForSecondsRealtime(.1f);CheckGraphicRaycasts(FindFirstObjectByType<BowlingTransition>(),false);
                yield return new WaitForSecondsRealtime(1f);Check(game.Level==2&&!game.Transitioning,"Consecutive level transition completes on phone");
                PlayerPrefs.SetInt("ReducedMotion",1);float reducedStarted=Time.realtimeSinceStartup;game.Hud.PlayLevel(3);yield return new WaitForSecondsRealtime(.25f);
                art=FindFirstObjectByType<BowlingTransition>();Check(game.Transitioning&&art!=null&&art.GetComponent<CanvasGroup>().alpha>.99f,"Reduced motion shows a readable static loading screen instead of a flash");Check(!game.Shoot(Vector3.forward)&&!game.Swap(),"Reduced-motion loading keeps gameplay locked while visible");Capture("transition-unity-reduced.png");while(game.Transitioning&&Time.realtimeSinceStartup-reducedStarted<4)yield return null;Check(Time.realtimeSinceStartup-reducedStarted>=1.4f,"Reduced-motion transition respects the same minimum duration");Check(game.Level==3&&!game.Transitioning,"Reduced motion enters the requested level after the loading screen");
                PlayerPrefs.SetInt("ReducedMotion",motion);RestoreWallet();File.WriteAllText(Path.Combine(folder,"transition-unity-verification.txt"),(assertions-failures)+"/"+assertions+" passed.\n"+report);Application.Quit(failures==0?0:1);yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-parity-test") >= 0)
            {
                int motion=PlayerPrefs.GetInt("ReducedMotion",0);PlayerPrefs.SetInt("ReducedMotion",1);
                Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.6f);game.ReloadEconomy();game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.7f);Capture("unity-html-game.png");
                foreach(float power in new[]{.7f,1f,1.3f}){GameObject.Find("HTML sun").GetComponent<Light>().intensity=power;yield return null;Capture("unity-light-"+power.ToString("0.0",System.Globalization.CultureInfo.InvariantCulture)+".png");}GameObject.Find("HTML sun").GetComponent<Light>().intensity=1f;
                game.Camera.GetComponent<HtmlLook>().enabled=false;Capture("unity-no-look.png");game.Camera.GetComponent<HtmlLook>().enabled=true;
                game.SetMenu(true);game.Hud.ShowHome();yield return new WaitForSecondsRealtime(.5f);Capture("unity-html-home.png");
                game.Hud.ShowSettings();yield return new WaitForSecondsRealtime(.2f);Capture("unity-html-settings.png");game.Hud.CloseOverlays();
                game.Hud.ShowShop();yield return new WaitForSecondsRealtime(.2f);Capture("unity-html-shop.png");game.Hud.CloseOverlays();
                PlayerPrefs.SetInt("ReducedMotion",motion);RestoreWallet();Application.Quit(0);yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-collision-test") >= 0)
            {
                yield return CollisionChecks(); RestoreWallet();
                File.WriteAllText(Path.Combine(folder, "collision-unity-verification.txt"), (assertions - failures) + "/" + assertions + " passed.\n" + report);
                Application.Quit(failures == 0 ? 0 : 1); yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-pull-test") >= 0)
            {
                yield return PullChecks(); RestoreWallet();
                File.WriteAllText(Path.Combine(folder, "pull-unity-verification.txt"), (assertions - failures) + "/" + assertions + " passed.\n" + report);
                Application.Quit(failures == 0 ? 0 : 1); yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-polish-test") >= 0)
            {
                yield return PolishChecks(); RestoreWallet();
                File.WriteAllText(Path.Combine(folder, "polish-unity-verification.txt"), (assertions - failures) + "/" + assertions + " passed.\n" + report);
                Application.Quit(failures == 0 ? 0 : 1); yield break;
            }
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-visual-test") >= 0)
            {
                yield return VisualChecks(); RestoreWallet(); Application.Quit(failures == 0 ? 0 : 1); yield break;
            }
            game.LoadLevel(0); yield return new WaitForSecondsRealtime(.5f); Capture("01-ready.png");
            Check(game.Total == 12 && game.Moves == 6, "Intro starts with 12 pins and 6 moves");
            int oldMoves = game.Moves, first = game.Queue[0], second = game.Queue[1];
            Check(game.Coins == 0 && game.SwapCredits == 3 && !game.BuySwap(), "Fresh wallet has 3 starter swaps; insufficient coins cannot buy");
            Check(game.Swap() && game.Queue[0] == second && game.Queue[game.Queue.Count - 1] == first && game.Moves == oldMoves && game.SwapCredits == 2, "Swap rotates the entire queue and consumes one credit, not a move");
            var positions = game.Pins.ConvertAll(p => p.transform.position);
            Check(game.Shoot(new Vector3(-2.8f, .43f, -.2f)), "Wrong-color shot accepted");
            Check(game.Moves == oldMoves - 1, "A shot spends exactly one move");
            Check(!game.Swap() && !game.Shoot(Vector3.forward), "Swap and duplicate shots blocked while physics resolves");
            yield return new WaitForSecondsRealtime(3.2f);
            bool still = game.Cleared == 0;
            for (int i = 0; i < game.Pins.Count; i++) still &= game.Pins[i] != null && Vector3.Distance(game.Pins[i].transform.position, positions[i]) < .001f;
            Check(still, "Wrong-color ball rebounds without moving pins");
            Capture("02-wrong-color.png");
            game.LoadLevel(0);
            Check(game.Shoot(new Vector3(-2.45f, .43f, -.2f)), "Matching-color rolling shot accepted");
            yield return new WaitForSecondsRealtime(.55f); Capture("03-impact.png");
            yield return new WaitForSecondsRealtime(3.5f);
            Check(game.Cleared > 0, "Matching-color impact physically topples and clears pins");
            bool otherColorsIntact = true;
            foreach (var p in game.Pins) if (p != null && p.ColorId != 0) otherColorsIntact &= !p.Cleared && p.Body.linearVelocity.sqrMagnitude < .001f && p.Body.angularVelocity.sqrMagnitude < .001f;
            Check(otherColorsIntact, "Chain reactions do not activate another color");
            int guard = 0;
            while (!game.Finished && game.Moves > 0 && guard < 8)
            {
                if (game.Busy) { yield return new WaitForSecondsRealtime(.2f); continue; }
                guard++;
                PinTarget target = game.Pins.Find(p => p != null && !p.Cleared && p.ColorId == game.Queue[0]);
                if (target == null) { game.Swap(); target = game.Pins.Find(p => p != null && !p.Cleared && p.ColorId == game.Queue[0]); }
                if (target == null) target = game.Pins.Find(p => p != null && !p.Cleared);
                if (target == null) break;
                Debug.Log("QA aimed at " + target.transform.position + " color " + game.Queue[0] + " cleared=" + game.Cleared); game.Shoot(target.transform.position); yield return new WaitForSecondsRealtime(4.9f); Debug.Log("QA after shot: " + game.Cleared + "/" + game.Total);
            }
            yield return new WaitForSecondsRealtime(.8f);
            Check(game.Finished && game.Won && game.Cleared == game.Total, "Intro is winnable within its move budget"); Capture("04-victory.png");
            Check(game.Coins == 30 && game.LastReward == 30, "Level victory awards exactly 30 coins");
            yield return new WaitForSecondsRealtime(.3f); Check(game.Coins == 30, "Reward is not duplicated after the victory");
            game.LoadLevel(1); Check(game.Total == 18 && game.Moves == 8 && game.Cleared == 0 && !game.Finished, "Next level resets state and increases the layout");
            int creditsBefore = game.SwapCredits;
            Check(game.BuySwap() && game.Coins == 20 && game.SwapCredits == creditsBefore + 1, "10 coins purchase exactly one swap credit");
            Check(PlayerPrefs.GetInt("Coins") == 20 && PlayerPrefs.GetInt("SwapCredits") == game.SwapCredits, "Wallet changes persist to PlayerPrefs");
            game.BuySwap(); game.BuySwap(); Check(game.Coins == 0 && !game.BuySwap(), "Purchases cannot overdraw the wallet");
            int credits = game.SwapCredits;
            for (int i = 0; i < credits; i++) game.Swap();
            int next = game.Queue[0];
            Check(game.SwapCredits == 0 && !game.Swap() && game.Queue[0] == next, "No credit means no queue rotation");
            game.LoadLevel(0);
            Check(game.SwapCredits == 0 && game.Coins == 0, "Restart preserves wallet and never refills starter credits");
            for (int i = 0; i < 6; i++) { game.Shoot(new Vector3(4, .43f, -2.6f)); yield return new WaitForSecondsRealtime(3.2f); }
            Check(game.Finished && !game.Won && game.Moves == 0, "Exhausting moves shows failure after physics settles");
            Check(!game.Shoot(Vector3.forward) && !game.Swap(), "Finished state blocks shooting and swapping"); Capture("05-retry.png");
            game.LoadLevel(0); Check(!game.Finished && !game.Busy && game.Moves == 6 && game.Cleared == 0, "Retry restores fresh level state");
            Check(game.Coins == 0, "Failed level grants no coins");
            RestoreWallet();
            string summary = "PinPop runtime verification: " + (assertions - failures) + "/" + assertions + " passed.\nUnity " + Application.unityVersion + "\n";
            File.WriteAllText(Path.Combine(folder, "verification.txt"), summary + report);
            Debug.Log(summary); Application.Quit(failures == 0 ? 0 : 1);
        }
        IEnumerator CollisionChecks()
        {
            game.SetMenu(false); game.Hud.ShowGame(); float[] sideways = new float[3]; float[] offsets = { -.35f, 0, .35f };
            for (int i = 0; i < offsets.Length; i++)
            {
                game.LoadLevel(0); var pin = game.Pins[0];
                foreach (var p in game.Pins) if (p != pin) p.gameObject.SetActive(false);
                pin.transform.position = new Vector3(0, .02f, 0); pin.Body.Sleep(); Physics.SyncTransforms();
                var obj = new GameObject("Collision verification ball"); obj.transform.position = new Vector3(offsets[i], .55f, -2);
                var ball = obj.AddComponent<BowlingBall>(); ball.Setup(game, 0, new Vector3(0, 0, 8), .7f);
                yield return new WaitForSecondsRealtime(.4f);
                Check(ball.ContactCount > 0, "Actual rigidbody contact at offset " + offsets[i]);
                sideways[i] = ball.OutgoingVelocity.x;
                Check(ball.IncomingVelocity.z > 0 && ball.OutgoingVelocity.z < ball.IncomingVelocity.z, "Impact transfers forward momentum to pin at offset " + offsets[i]);
                Check(Mathf.Abs(ball.LastContactNormal.x) > .1f || i == 1, "Glancing hit has a lateral surface normal at offset " + offsets[i]);
                if (i == 2) Capture("unity-collision-glance.png");
                Check(pin.Body.linearVelocity.sqrMagnitude > .01f || pin.Cleared, "Pin receives physical momentum at offset " + offsets[i]);
            }
            Check(sideways[0] < -.3f && sideways[2] > .3f, "Left and right impacts deflect the ball in opposite directions");
            Check(Mathf.Abs(sideways[1]) < .15f, "Centered impact does not invent a lateral direction");
            Check(Mathf.Abs(sideways[0] + sideways[2]) < .5f, "Mirrored contacts produce approximately mirrored deflection");
            float[] blockedSideways = new float[3];
            for (int i = 0; i < offsets.Length; i++)
            {
                game.LoadLevel(0); var target = game.Pins[0]; foreach (var p in game.Pins) if (p != target) p.gameObject.SetActive(false);
                target.transform.position = new Vector3(0, .02f, 0); Physics.SyncTransforms(); var start = target.transform.position;
                var obj = new GameObject("Wrong color rebound verification"); obj.transform.position = new Vector3(offsets[i], .55f, -2);
                var wrong = obj.AddComponent<BowlingBall>(); wrong.Setup(game, 1, new Vector3(0, 0, 8), .7f);
                for (int frame = 0; frame < 40 && wrong.ContactCount == 0; frame++) yield return new WaitForFixedUpdate();
                Check(wrong.ContactCount > 0, "Wrong-color ball contacts pin at offset " + offsets[i]);
                blockedSideways[i] = wrong.OutgoingVelocity.x;
                Check(Vector3.Distance(target.transform.position, start) < .001f && !target.Cleared && target.IsLocked, "Wrong-color pin remains fixed at offset " + offsets[i]);
                Check(wrong.OutgoingVelocity.magnitude < wrong.IncomingVelocity.magnitude, "Wrong-color rebound loses speed at offset " + offsets[i]);
                var block = new MaterialPropertyBlock(); target.SkinRenderer.GetPropertyBlock(block);
                Check(block.GetColor("_EmissionColor").maxColorComponent > .01f, "Wrong-color contact briefly flashes at offset " + offsets[i]);
                if (i == 2) Capture("unity-wrong-color-rebound.png");
                yield return new WaitForSecondsRealtime(.3f); target.SkinRenderer.GetPropertyBlock(block);
                Check(block.GetColor("_EmissionColor").maxColorComponent < .01f, "Block flash returns to original color at offset " + offsets[i]);
                if (i == 1)
                {
                    Check(wrong.OutgoingVelocity.z < 0 && Mathf.Abs(wrong.OutgoingVelocity.x) < .15f, "Centered wrong-color impact rebounds without lateral drift");
                    var matchObject = new GameObject("Matching followup verification"); matchObject.transform.position = new Vector3(0, .55f, -2);
                    matchObject.AddComponent<BowlingBall>().Setup(game, 0, new Vector3(0, 0, 8), .7f);
                    Check(!target.IsLocked, "Matching next ball unlocks the pin"); yield return new WaitForSecondsRealtime(.5f);
                    Check(target.Cleared || target.Body.linearVelocity.sqrMagnitude > .01f, "Matching next ball can move the previously blocked pin");
                }
            }
            Check(blockedSideways[0] < -.3f && blockedSideways[2] > .3f, "Wrong-color left and right contacts deflect in opposite directions");
            Check(Mathf.Abs(blockedSideways[0] + blockedSideways[2]) < .5f, "Wrong-color mirrored contacts have mirrored rebound angles");
            Check(BowlingBall.ContactMaterial.bounciness > 0 && BowlingBall.ContactMaterial.bounciness < 1, "Contacts use dissipative restitution");
        }
        IEnumerator PullChecks()
        {
            game.SetMenu(false);game.Hud.ShowGame();game.LoadLevel(0);yield return new WaitForSecondsRealtime(.3f);
            Check(PinPopGame.Launch.z> -8.4f&&PinPopGame.Launch.z< -6.8f&&Mathf.Abs(PinPopGame.Launch.y-.39f)<.02f,"Ready ball rests on the front of the table");
            float nearest=float.MaxValue;foreach(var pin in game.Pins)nearest=Mathf.Min(nearest,pin.transform.position.z-PinPopGame.Launch.z);
            Check(nearest>=6.9f,"Ready ball has at least 6.9 world units of approach before the pins");
            var floor=GameObject.Find("Table physics").GetComponent<BoxCollider>().bounds;
            Check(floor.min.z<=-8.79f&&floor.max.z>=4.79f,"Extended physical table supports the entire approach and pin deck");
            Check(GameObject.Find("Next bowling ball preview")!=null&&game.Hud.GetPreviewColor(0)==game.Queue[1],"Ready queue skips only the foreground ball");
            var movesProperty=typeof(PinPopGame).GetProperty("Moves");
            foreach(int remaining in new[]{3,2,1,0,6}){
                movesProperty.SetValue(game,remaining);game.Hud.Refresh();int visible=0;
                foreach(var image in Resources.FindObjectsOfTypeAll<UnityEngine.UI.Image>())if(image.gameObject.activeInHierarchy&&image.name.StartsWith("Next bowling ball preview"))visible++;
                Check(visible==Mathf.Min(3,Mathf.Max(0,remaining-1)),"Queue fits remaining moves "+remaining);
            }
            var target=new Vector3(-2.45f,.4f,-.2f);
            var weakVelocity=PinPopGame.ShotVelocity(target,.2f,PinPopGame.Launch);var strongVelocity=PinPopGame.ShotVelocity(target,1,PinPopGame.Launch);
            Check(weakVelocity.y==0&&strongVelocity.y==0&&strongVelocity.magnitude>weakVelocity.magnitude,"Pull power changes horizontal speed without ballistic lift");
            Check(!game.Shoot(target,0)&&!game.Shoot(target,float.NaN)&&game.Moves==6,"Invalid or zero tension spends no move");
            Check(game.Shoot(target,.2f),"Weak rolling shot starts");
            bool queue=true;for(int i=0;i<3;i++)queue &= game.Hud.GetPreviewColor(i)==game.Queue[i];Check(queue,"Rolling shot previews the actual next three balls");
            bool grounded=true,spinning=false;float firstZ=0,lastZ=0;var rolling=FindFirstObjectByType<BowlingBall>();
            for(int frame=0;frame<24;frame++){
                yield return new WaitForFixedUpdate();var body=rolling.GetComponent<Rigidbody>();if(frame==0)firstZ=body.position.z;lastZ=body.position.z;
                if(rolling.ContactCount==0)grounded &= body.position.y>.35f&&body.position.y<.45f;
                spinning |= body.angularVelocity.magnitude>5;
            }
            Check(grounded&&lastZ>firstZ+.3f,"Ball travels forward at table height before contact");Check(spinning,"Ball physically rotates as it rolls");Capture("unity-roll-motion.png");
            yield return new WaitForSecondsRealtime(4.5f);int weak=game.Cleared;Check(weak>0,"Weak rolling impact can topple matching pins");
            Check(game.Pins.TrueForAll(p=>p==null||p.ColorId==0||(!p.Cleared&&p.Body.linearVelocity.sqrMagnitude<.001f)),"Rolling ball leaves other colors fixed");
            game.LoadLevel(0);game.Shoot(target,1);yield return new WaitForSecondsRealtime(4.9f);int strong=game.Cleared;
            Check(strong>weak,"Stronger rolling shot clears more matching pins at the same aim");
            game.LoadLevel(0);yield return new WaitForSecondsRealtime(.2f);Vector2 anchor=game.Camera.WorldToScreenPoint(PinPopGame.Launch);
            game.BeginPullForVerification(anchor);game.PointerUp();Check(game.Moves==6&&!game.Busy,"Tap without pulling cancels for free");
            game.BeginPullForVerification(anchor);game.UpdatePull(anchor+new Vector2(Screen.width*.10f,-Screen.height*.125f));
            Check(game.IsPulling&&game.PullPower>.8f&&game.Aim.x< -2,"Diagonal backward pull sets power and opposite direction");
            var ready=GameObject.Find("Next bowling ball").transform;
            Check(ready.position.y==PinPopGame.Launch.y&&ready.position.z>= -8.3f&&Mathf.Abs(ready.position.x)<=.75f,"Maximum pull keeps the held ball on the table");
            Check(game.AimVisible,"Table aim appears without a falling-point label");
            bool flat=true;foreach(var dot in GetComponentsInChildren<Transform>())if(dot.name.StartsWith("Table aim dot"))flat &= Mathf.Abs(dot.position.y-.035f)<.001f;
            Check(flat,"Every trajectory dot lies on the table surface");
            Check(game.PreviewRoll.Point.z<game.Aim.z&&game.PreviewRoll.Point.y==.035f,"Guide stops at the first pin instead of drawing through it");
            Capture("unity-roll-aim.png");float selected=game.PullPower;game.CancelPull();Check(!game.AimVisible&&game.Moves==6,"Cancel clears the guide without using a move");
            game.BeginPullForVerification(anchor);game.UpdatePull(anchor+new Vector2(Screen.width*.10f,-Screen.height*.125f));game.PointerUp();
            Check(game.Busy&&game.Moves==5&&Mathf.Abs(game.LastShotPower-selected)<.02f,"Release launches exactly once at selected power");yield return null;
            Check(!game.AimVisible,"Release hides the aiming guide and obsolete landing label");
            yield return new WaitForSecondsRealtime(4.9f);Capture("unity-roll-ready.png");
            Check(game.Hud.GetPreviewColor(0)==game.Queue[1],"After rolling the preview skips the new foreground ball once");
            game.LoadLevel(0);movesProperty.SetValue(game,1);game.Shoot(target,1);yield return new WaitForSecondsRealtime(.15f);
            Check(!game.Finished,"Last move is not declared lost before collisions resolve");
            typeof(PinPopGame).GetProperty("Cleared").SetValue(game,game.Total);int beforeReward=game.Coins;
            yield return new WaitForSecondsRealtime(.15f);Check(!game.Finished,"Final clear leaves a short impact beat");
            yield return new WaitForSecondsRealtime(.5f);Check(game.Finished&&game.Won&&game.Coins==beforeReward+PinPopGame.LevelReward,"Last-move win rewards exactly once");
            yield return new WaitForSecondsRealtime(.2f);Check(game.Coins==beforeReward+PinPopGame.LevelReward,"Reward is not duplicated");
        }
        IEnumerator RollingLevelChecks()
        {
            bool noCredits=Array.IndexOf(Environment.GetCommandLineArgs(),"-economy-progression-test")>=0;int[] levels=new[]{1,3,5};if(noCredits||Array.IndexOf(Environment.GetCommandLineArgs(),"-all-layouts-test")>=0){levels=new int[12];for(int i=0;i<levels.Length;i++)levels[i]=i;}
            foreach(int level in levels)
            {
                PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(new EconomySave{coins=0,swaps=noCredits?0:3}));game.ReloadEconomy();
                game.LoadLevel(level);game.SetMenu(false);game.Hud.ShowGame();int attempts=0;
                yield return new WaitForSecondsRealtime(.2f);
                while(!game.Finished&&game.Moves>0&&attempts<12)
                {
                    if(game.Busy){yield return null;continue;}
                    var matching=game.Pins.FindAll(p=>p!=null&&!p.Cleared&&p.ColorId==game.Queue[0]);
                    if(matching.Count==0&&game.SwapCredits>0){game.Swap();continue;}
                    matching.Sort((a,b)=>a.transform.position.z.CompareTo(b.transform.position.z));
                    float z=matching.Count>0?matching[0].transform.position.z:.2f,x=0;int n=0;
                    foreach(var pin in matching)if(pin.transform.position.z<z+.3f){x+=pin.transform.position.x;n++;}
                    x=n>0?x/n:0;attempts++;game.Shoot(new Vector3(x,.4f,z),1);
                    while(game.Busy&&!game.Finished)yield return null;
                    Debug.Log("ROLL LEVEL "+(level+1)+" shot "+attempts+" cleared="+game.Cleared+"/"+game.Total+" moves="+game.Moves);
                }
                Check(game.Won&&game.Cleared==game.Total,"Rolling level "+(level+1)+" is completable within its original move budget");
                if(noCredits)Check(game.Coins==30+2*game.Moves&&game.SwapCredits==0,"Chapter "+(level+1)+" earns exact reward with zero purchased or starter swaps");
                Capture((noCredits?"economy-progress-level-":"unity-roll-level-")+(level+1)+".png");
            }
        }
        IEnumerator JourneyChecks()
        {
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.4f);game.LoadLevel(0);game.SetMenu(true);game.Hud.ShowHome();yield return new WaitForSecondsRealtime(.2f);
            Click("HELP_HOME");Check(game.Hud.HelpVisible&&game.Paused,"Home opens a paused how-to-play page");yield return new WaitForSecondsRealtime(.25f);Capture("journey-unity-help-home.png");
            game.Hud.ShowShop();game.Hud.ShowSettings();Check(game.Hud.HelpVisible&&!game.Hud.ShopVisible&&!game.Hud.SettingsVisible,"Help cannot be replaced by a nested settings or shop modal");
            foreach(var size in new[]{new Vector2Int(390,844),new Vector2Int(320,740),new Vector2Int(1000,600),new Vector2Int(1100,700),new Vector2Int(1200,800)}){
                Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);Canvas.ForceUpdateCanvases();bool fits=true;
                for(int i=0;i<3;i++){var text=ActiveText("Help body "+i);fits &= text!=null&&text.preferredHeight<=text.rectTransform.rect.height+1;var heading=ActiveText("Help step "+i);fits &= heading!=null&&heading.preferredHeight<=heading.rectTransform.rect.height+1;}
                Check(fits&&game.Hud.HelpVisible&&game.Paused,"All help paragraphs fit and pause survives resize "+size.x+"x"+size.y);
                if(size.x==320||size.x==1000)Capture("journey-unity-help-"+size.x+".png",Screen.width,Screen.height);
            }
            var hud=FindFirstObjectByType<HtmlReferenceHUD>();var cache=(System.Collections.Generic.Dictionary<string,Sprite>)typeof(HtmlReferenceHUD).GetField("sprites",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(hud);int gradients=0;foreach(var pair in cache)if(pair.Key.StartsWith("outside")){gradients++;Check(pair.Value.texture.width<=128&&pair.Value.texture.height<=128,"Resize background uses a bounded small texture");}Check(gradients==1,"Window resizing reuses one sidebar texture");
            Click("HELP_CLOSE");Check(!game.Paused&&!game.Hud.HelpVisible,"Help closes and resumes without changing the level");Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.4f);
            Click("PLAY");yield return new WaitForSecondsRealtime(1.65f);Check(!game.InMenu&&!game.Transitioning&&game.Moves==6,"Play enters a fresh level after readable loading");
            typeof(PinPopGame).GetField("keyboardPower",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(game,.8f);typeof(PinPopGame).GetField("keyboardAim",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(game,true);game.Hud.Refresh();game.SendMessage("RefreshAimPreview");
            Check(game.AimVisible&&ActiveText("Power value").text=="%80"&&Mathf.Abs(game.DisplayPower-.8f)<.001f,"Keyboard aim displays selected power instead of zero");Capture("journey-unity-keyboard.png");game.CancelPull();Check(!game.AimVisible&&ActiveText("Power value").text=="%0","Cancel clears keyboard aim and its meter");
            int turns=0;
            while(!game.Finished&&game.Moves>0&&turns<6){
                var matching=game.Pins.FindAll(p=>p!=null&&!p.Cleared&&p.ColorId==game.Queue[0]);if(matching.Count==0&&game.SwapCredits>0){Click("SWAP");continue;}
                matching.Sort((a,b)=>a.transform.position.z.CompareTo(b.transform.position.z));float z=matching.Count>0?matching[0].transform.position.z:.2f,x=0;int n=0;foreach(var pin in matching)if(pin.transform.position.z<z+.3f){x+=pin.transform.position.x;n++;}x=n>0?x/n:0;
                float desired=Mathf.Clamp(-x/.045f,-110,110),dx=desired*140/Mathf.Sqrt(19600-desired*desired);Vector2 anchor=game.Camera.WorldToScreenPoint(PinPopGame.Launch);int before=game.Moves;
                game.BeginPullForVerification(anchor);game.UpdatePull(anchor+new Vector2(dx*game.Camera.pixelWidth/600,-140*game.Camera.pixelHeight/1000));Capture("journey-unity-pull-"+turns+".png");game.PointerUp();turns++;
                Check(game.Busy&&game.Moves==before-1,"Actual pull/release code spends exactly one move on shot "+turns);
                if(turns==1){yield return new WaitForSecondsRealtime(.1f);Click("HELP_GAME");var ball=FindFirstObjectByType<BowlingBall>();var frozen=ball.transform.position;yield return new WaitForSecondsRealtime(.2f);Check(game.Paused&&Vector3.Distance(ball.transform.position,frozen)<.001f,"Opening help freezes a rolling ball without losing the shot");Click("HELP_CLOSE");}
                while(game.Busy&&!game.Finished)yield return null;
                Debug.Log("JOURNEY PULL "+turns+" cleared="+game.Cleared+"/"+game.Total+" moves="+game.Moves);
            }
            Check(game.Won&&game.Cleared==game.Total,"First level is won through the real pull/release control path");Check(game.Coins==30,"Real-control victory grants one 30-coin reward");yield return new WaitForSecondsRealtime(1.1f);Capture("journey-unity-victory.png");
            game.Hud.ShowShop();game.Hud.ShowSettings();game.Hud.ShowHelp();game.Hud.CloseOverlays();Check(game.Hud.ResultVisible&&game.Paused,"Result cannot be replaced or unpaused by another overlay");
            Click("CONTINUE");yield return new WaitForSecondsRealtime(1.65f);Check(game.Level==1&&game.Moves==8,"Continue starts the next chapter");int credits=game.SwapCredits;Click("GAME_SHOP");yield return new WaitForSecondsRealtime(.2f);Click("SHOP_BUY");Check(game.Coins==20&&game.SwapCredits==credits+1,"Earned coins purchase exactly one swap");Click("CLOSE_SHOP");
            game.Shoot(new Vector3(0,.4f,.2f),1);yield return new WaitForSecondsRealtime(.1f);
            Click("SETTINGS");yield return new WaitForSecondsRealtime(.2f);var interruptedBall=FindFirstObjectByType<BowlingBall>();var interruptedAt=interruptedBall.transform.position;
            Click("RESTART");yield return new WaitForSecondsRealtime(.1f);Check(game.Transitioning&&game.Paused&&Vector3.Distance(interruptedBall.transform.position,interruptedAt)<.001f,"Restart freezes the old rolling board immediately while loading");game.Hud.CloseOverlays();Check(game.Transitioning&&game.Paused,"Closing a previous modal cannot unpause the loading board");
            yield return new WaitForSecondsRealtime(1.55f);Check(game.Level==1&&game.Moves==8&&game.Cleared==0&&game.Coins==20,"Settings restart preserves wallet and resets the board");
            Click("SETTINGS");yield return new WaitForSecondsRealtime(.2f);Click("GO_HOME");Check(game.InMenu&&game.Level==1&&game.Hud.HomeVisible,"Settings return home preserves chapter progress");
            game.LoadLevel(1000);Check(game.Level==1000,"High chapter numbers are retained without a 999 cap");
            var lower=((MeshCollider)game.Pins[0].Hitboxes[0]).sharedMesh;var neck=((MeshCollider)game.Pins[0].Hitboxes[1]).sharedMesh;bool reused=true;
            for(int level=0;level<12;level++){game.LoadLevel(level);reused &= ((MeshCollider)game.Pins[0].Hitboxes[0]).sharedMesh==lower&&((MeshCollider)game.Pins[0].Hitboxes[1]).sharedMesh==neck;}
            Check(reused&&lower!=neck,"All twelve layouts reuse two contact meshes without changing their shapes");
            int beforeMaterials=Resources.FindObjectsOfTypeAll<Material>().Length;for(int i=0;i<10;i++)Art.Burst(Vector3.zero,Art.Colors[i%3],1);
            Check(Resources.FindObjectsOfTypeAll<Material>().Length<=beforeMaterials+1,"Repeated impact particles reuse a material instead of allocating per hit");

        }
        IEnumerator FovLayoutChecks(){
            Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.5f);game.MatchTray.Collect(0,new Vector2(.2f,.5f));game.MatchTray.Collect(1,new Vector2(.8f,.5f));yield return new WaitForSecondsRealtime(.9f);
            foreach(var size in new[]{new Vector2Int(600,1000),new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){
                Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);
                foreach(float fov in new[]{35f,45f,60f}){
                    game.Camera.fieldOfView=fov;yield return new WaitForSecondsRealtime(.25f);Check(Mathf.Abs(game.Camera.fieldOfView-fov)<.001f,"Editing Camera.fieldOfView persists over frames at "+size+" / "+fov);
                    var panel=PixelRect("Collection tray");var a=game.Camera.WorldToScreenPoint(new Vector3(-4.275f,0,4.8354167f));var b=game.Camera.WorldToScreenPoint(new Vector3(4.275f,0,4.8354167f));var viewport=FindFirstObjectByType<HtmlReferenceHUD>().AppRect;float left=Mathf.Clamp(a.x,viewport.x,viewport.xMax),right=Mathf.Clamp(b.x,viewport.x,viewport.xMax);Check(Mathf.Abs(panel.xMin-left)<1&&Mathf.Abs(panel.xMax-right)<1,"Collection width equals the visible wooden pin-deck edge at "+size+" / "+fov);
                    var slot=PixelRect("Collection slot 0");var pin=PixelRect("Collected color 1");Check(Vector2.Distance(slot.center,pin.center)<1&&Mathf.Abs(slot.width-slot.height)<1,"Collected pin and square slot stay aligned after zoom at "+size+" / "+fov);yield return new WaitForEndOfFrame();Capture("fov-layout-"+size.x+"-"+fov+".png",Screen.width,Screen.height);
                }
            }
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);game.GameplayFieldOfView=50;yield return null;Check(Mathf.Abs(game.Camera.fieldOfView-50)<.01f,"The exposed gameplay FOV field also applies at runtime");game.Camera.fieldOfView=55;yield return new WaitForSecondsRealtime(.1f);game.SetMenu(true);game.Hud.ShowHome();yield return new WaitForSecondsRealtime(.1f);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.1f);Check(Mathf.Abs(game.Camera.fieldOfView-55)<.01f,"Gameplay Camera FOV survives a home/game round trip");game.Hud.ShowSettings();game.Camera.fieldOfView=48;yield return new WaitForSecondsRealtime(.2f);Check(Mathf.Abs(game.Camera.fieldOfView-48)<.01f,"Camera FOV is editable while gameplay is paused");game.Hud.CloseOverlays();
        }
        Rect PixelRect(string name){var r=GameObject.Find(name).GetComponent<RectTransform>();var c=new Vector3[4];r.GetWorldCorners(c);var a=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,c[0]);var b=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,c[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);}
        IEnumerator LaunchPositionChecks(){
            Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.5f);
            Check(game.LaunchPosition==PinPopGame.Launch,"New level starts at center of front edge");int moves=game.Moves;
            foreach(var size in new[]{new Vector2Int(600,1000),new Vector2Int(320,740),new Vector2Int(1000,600)}){
                Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);game.SetLaunchX(0);
                var anchor=(Vector2)game.Camera.WorldToScreenPoint(game.LaunchPosition);game.BeginPullForVerification(anchor);game.UpdatePull(anchor+new Vector2(Screen.width*.2f,0));
                Check(game.IsPositioning&&game.LaunchPosition.x>1&&game.PullPower==0,"Horizontal drag repositions without charging: "+size);
                game.UpdatePull(anchor+new Vector2(Screen.width*2,Screen.height*.3f));
                Check(game.LaunchPosition.x==PinPopGame.LaunchLimit&&game.LaunchPosition.y==PinPopGame.Launch.y&&game.LaunchPosition.z==PinPopGame.Launch.z,"Positioning remains on front rail and clamps right even with vertical drift: "+size);
                game.PointerUp();Check(!game.Busy&&game.Moves==moves,"Releasing positioning gesture does not shoot: "+size);
                anchor=game.Camera.WorldToScreenPoint(game.LaunchPosition);game.BeginPullForVerification(anchor);game.UpdatePull(anchor-new Vector2(Screen.width*3,0));game.PointerUp();
                Check(game.LaunchPosition.x== -PinPopGame.LaunchLimit,"Horizontal drag reaches left edge: "+size);
                Check(GameObject.Find("Next bowling ball").transform.position==game.LaunchPosition,"Visible ready ball follows selected position: "+size);
                game.Swap();Check(game.LaunchPosition.x== -PinPopGame.LaunchLimit&&GameObject.Find("Next bowling ball").transform.position==game.LaunchPosition,"Swap preserves selected position: "+size);
            }
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);game.SetLaunchX(2);
            var p=(Vector2)game.Camera.WorldToScreenPoint(game.LaunchPosition);game.BeginPullForVerification(p);game.UpdatePull(p+Vector2.down*110);
            Check(!game.IsPositioning&&game.PullPower>.7f&&game.LaunchPosition.x==2,"Pull gesture charges shot without moving selected ball");
            Check(Mathf.Abs(game.PreviewRoll.Point.x-2)<.01f,"Aim preview starts from moved launch position");game.PointerUp();var ball=FindFirstObjectByType<BowlingBall>();
            Check(game.Moves==moves-1&&ball!=null&&Mathf.Abs(ball.transform.position.x-2)<.01f&&Mathf.Abs(ball.transform.position.z-PinPopGame.Launch.z)<.01f,"Released shot uses selected front-edge position and one move");
            Check(!game.SetLaunchX(-2),"Position cannot change while a shot resolves");
            game.LoadLevel(2);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.2f);
            PinTarget target=null;foreach(var pin in game.Pins)if(target==null||pin.transform.position.x<target.transform.position.x)target=pin;
            game.ChangeReadyColor(target.ColorId);game.SetLaunchX(-3.4f);game.Shoot(target.Body.position,1);ball=FindFirstObjectByType<BowlingBall>();float started=Time.time;while(ball!=null&&ball.ContactCount==0&&Time.time-started<3)yield return null;
            Check(ball!=null&&ball.ContactCount>0,"Shifted launch reaches pins past fixed bumpers");Capture("launch-position-unity.png");
            game.LoadLevel(0);yield return null;Check(game.LaunchPosition==PinPopGame.Launch,"Restart resets launch position to center");
            game.Hud.ShowSettings();Check(!game.SetLaunchX(1),"Paused game blocks repositioning");game.Hud.CloseOverlays();
        }
        IEnumerator GrassLifetimeChecks(){
            Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.5f);
            game.Shoot(new Vector3(-24,.4f,0),1);var ball=FindFirstObjectByType<BowlingBall>();var body=ball.GetComponent<Rigidbody>();float start=Time.time;
            while(ball!=null&&!ball.HasReachedGrass&&Time.time-start<5)yield return null;
            Check(ball!=null&&ball.HasReachedGrass,"Full power side shot reaches grass");if(ball==null||!ball.HasReachedGrass)yield break;
            var landed=body.position;float landingSpeed=new Vector2(body.linearVelocity.x,body.linearVelocity.z).magnitude;
            Check(landingSpeed<=3.6f,"Grass contact caps full power ball speed");
            Check(Mathf.Abs(landed.x)<7.5f,"Full power shot lands near table edge");
            yield return new WaitForSeconds(1.5f);
            Check(ball!=null&&Vector3.Distance(body.position,landed)<1.5f,"Grass ball rolls less than 1.5 units after landing");
            Check(body.linearVelocity.sqrMagnitude<.01f,"Grass resistance brings ball to rest");Capture("grass-lifetime-resting.png");
            game.Hud.ShowSettings();float age=ball.GrassAge;var held=body.position;yield return new WaitForSecondsRealtime(.5f);
            Check(ball.GrassAge==age&&Vector3.Distance(held,body.position)<.001f,"Pause freezes grass lifetime and physics");game.Hud.CloseOverlays();
            while(game.Busy)yield return null;Check(game.Shoot(new Vector3(0,.4f,-20),.5f),"Next turn starts normally");yield return null;
            Check(ball!=null,"Next shot preserves grass ball before its expiry");
            while(ball!=null&&ball.GrassAge<9.85f)yield return null;
            Check(ball!=null&&ball.GrassAge<10,"Resting ball remains visible until ten seconds after landing");
            yield return new WaitForSeconds(.2f);Check(ball==null,"Resting grass ball is removed at ten seconds");
            game.LoadLevel(0);yield return null;Check(FindObjectsByType<BowlingBall>(FindObjectsSortMode.None).Length==0,"Restart clears remaining balls");
            Check(Physics.GetIgnoreLayerCollision(PinPopGame.GrassLayer,0),"Pins still fall freely through grass for collection");
        }
        IEnumerator GrassLayoutChecks(){
            Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.5f);
            var anchor=(Vector2)game.Camera.WorldToScreenPoint(PinPopGame.Launch);game.BeginPullForVerification(game.Camera.WorldToScreenPoint(new Vector3(-2.45f,.4f,0)));Check(!game.IsPulling,"Selecting a distant target does not start a timed charge");game.BeginPullForVerification(anchor);game.UpdatePull(anchor);yield return new WaitForSeconds(.25f);game.UpdatePull(anchor);Check(game.IsPulling&&game.PullPower==0,"Holding without pulling does not increase shot power");game.UpdatePull(anchor+Vector2.down*140*game.Camera.pixelHeight/1000);Check(game.PullPower>.99f&&GameObject.Find("Next bowling ball").transform.position==PinPopGame.Launch,"Pull distance controls power while the ball remains fixed");game.CancelPull();
            Check(Physics.GetIgnoreLayerCollision(PinPopGame.GrassLayer,0)&&!Physics.GetIgnoreLayerCollision(PinPopGame.GrassLayer,PinPopGame.BallLayer),"Grass supports bowling balls while collected pins still fall freely");
            game.Shoot(new Vector3(-6,.4f,5),.35f);var ball=FindFirstObjectByType<BowlingBall>();var body=ball.GetComponent<Rigidbody>();float started=Time.time;while(!ball.HasReachedGrass&&Time.time-started<5)yield return null;Check(ball!=null&&ball.HasReachedGrass,"A real shot leaves the table and lands on the grass collider");var landed=body.position;float initialSpeed=new Vector2(body.linearVelocity.x,body.linearVelocity.z).magnitude;yield return new WaitForEndOfFrame();Capture("grass-layout-landing.png");yield return new WaitForSeconds(1);float laterSpeed=new Vector2(body.linearVelocity.x,body.linearVelocity.z).magnitude;Check(Vector3.Distance(body.position,landed)>.5f&&laterSpeed<initialSpeed,"Grass rolling resistance slows a moving ball after a visible roll");
            while(Time.time-started<7)yield return null;Check(ball!=null&&ball.HasReachedGrass&&body.position.y>PinPopGame.GrassHeight+.3f,"Grass ball remains present beyond the former six-second lifetime");float wait=Time.time;while(body.linearVelocity.sqrMagnitude>.01f&&Time.time-wait<10)yield return null;Check(ball!=null&&new Vector2(body.linearVelocity.x,body.linearVelocity.z).magnitude<.1f,"Grass ball naturally comes to rest instead of disappearing");
            while(game.Busy)yield return null;Check(game.Shoot(new Vector3(0,.4f,-20),.5f),"Another shot can begin while the previous ball stays on the grass");yield return new WaitForSeconds(.2f);Check(ball!=null&&FindObjectsByType<BowlingBall>(FindObjectsSortMode.None).Length>=2,"Starting the next turn preserves the previous grass ball");
            game.Hud.ShowSettings();var heldAt=body.position;yield return new WaitForSecondsRealtime(.2f);Check(Vector3.Distance(body.position,heldAt)<.001f,"Pausing also freezes grass physics");game.Hud.CloseOverlays();game.PrepareShotColor(0);var pin=game.Pins.Find(p=>p!=null&&!p.Cleared&&p.ColorId==0);int before=game.Cleared;pin.Body.position=new Vector3(5,1.5f,0);pin.Body.WakeUp();wait=Time.time;while(game.Cleared==before&&Time.time-wait<4)yield return null;Check(game.Cleared>before,"A pin still falls through the meadow into collection");yield return new WaitForSeconds(.8f);
            foreach(var size in new[]{new Vector2Int(600,1000),new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){
                Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.35f);yield return new WaitForEndOfFrame();var tray=PixelRect("Collection tray");var level=PixelRect("level");var swap=PixelRect("SWAP");var title=PixelRect("Next title");var last=PixelRect("Order 2");
                Check(tray.yMax<level.yMin&&tray.yMin>Screen.height*.5f,"Collection board sits below the chapter in the upper half: "+size);
                Check(tray.xMin>=0&&tray.xMax<=Screen.width+1,"All ten collection slots fit: "+size);
                Check(last.yMin>swap.yMax+3&&Mathf.Abs(title.center.x-swap.center.x)<45,"Upcoming balls are ordered directly above swap without overlap: "+size);Capture("grass-layout-"+size.x+".png",Screen.width,Screen.height);
            }
            game.LoadLevel(0);yield return null;Check(FindObjectsByType<BowlingBall>(FindObjectsSortMode.None).Length==0&&game.MatchTray.Empty,"Restart clears grass balls and the moved collection board state");
        }
        [Serializable] sealed class PhysicsReviewShot {public float friction,power,meanTravel,firstImpactSpeed;public int level,fallen,downOnTable;public bool lockedStayed;}
        [Serializable] sealed class PhysicsReviewResults {public System.Collections.Generic.List<PhysicsReviewShot> shots=new System.Collections.Generic.List<PhysicsReviewShot>();}
        IEnumerator PhysicsReviewChecks(){
            Screen.SetResolution(600,1000,false);game.SetMenu(false);game.Hud.ShowGame();var results=new PhysicsReviewResults();float savedFriction=HtmlPhysicsContacts.PinFloorFriction;
            foreach(float friction in new[]{.45f,.24f,.14f})foreach(int level in new[]{0,2,5})foreach(float power in new[]{.6f,1f}){
                HtmlPhysicsContacts.PinFloorFriction=friction;game.LoadLevel(level);yield return null;int color=game.Queue[0];var targets=game.Pins.FindAll(p=>p.ColorId==color);float x=(targets[0].Body.position.x+targets[1].Body.position.x)*.5f;
                var positions=new Vector3[game.Pins.Count];var travel=new float[game.Pins.Count];for(int i=0;i<positions.Length;i++)positions[i]=game.Pins[i].Body.position;
                var shot=new PhysicsReviewShot{friction=friction,power=power,level=level,lockedStayed=true};Check(game.Shoot(new Vector3(x,.4f,targets[0].Body.position.z),power),"Measured shot starts: friction="+friction+" level="+(level+1)+" power="+power);Time.timeScale=4;float start=Time.time;bool impactRecorded=false;
                while(Time.time-start<5){
                    for(int i=0;i<positions.Length;i++){var pin=game.Pins[i];if(pin==null||pin.Cleared)continue;var delta=pin.Body.position-positions[i];delta.y=0;if(pin.ColorId==color)travel[i]=Mathf.Max(travel[i],delta.magnitude);else shot.lockedStayed &= delta.magnitude<.001f;}
                    if(!impactRecorded){var ball=FindFirstObjectByType<BowlingBall>();if(ball!=null&&ball.ContactCount>0){shot.firstImpactSpeed=ball.FirstIncomingVelocity.magnitude;impactRecorded=true;}}
                    yield return null;
                }
                Time.timeScale=1;shot.fallen=game.Cleared;for(int i=0;i<positions.Length;i++){var pin=game.Pins[i];shot.meanTravel+=travel[i]/targets.Count;if(pin!=null&&!pin.Cleared&&pin.ColorId==color&&Vector3.Dot(pin.transform.up,Vector3.up)<.5f)shot.downOnTable++;}results.shots.Add(shot);Debug.Log("PHYSICS REVIEW "+JsonUtility.ToJson(shot));
                if(level==0){yield return new WaitForEndOfFrame();Capture("physics-review-"+friction.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture)+"-"+power.ToString("0.0",System.Globalization.CultureInfo.InvariantCulture)+".png");}
            }
            var baseline=results.shots.FindAll(s=>Mathf.Abs(s.friction-.45f)<.001f);var tuned=results.shots.FindAll(s=>Mathf.Abs(s.friction-.14f)<.001f);int oldFallen=0,newFallen=0;bool noWorse=true,sameIncoming=true;for(int i=0;i<baseline.Count;i++){oldFallen+=baseline[i].fallen;newFallen+=tuned[i].fallen;noWorse &= tuned[i].fallen>=baseline[i].fallen;sameIncoming &= Mathf.Abs(tuned[i].firstImpactSpeed-baseline[i].firstImpactSpeed)<.05f;}
            Check(newFallen>=oldFallen*1.5f&&noWorse,"Tuned floor increases collected targets across six identical shots without worsening any tested shot: "+oldFallen+" -> "+newFallen);Check(sameIncoming,"First-contact ball speed stays the same; improvement comes from pin sliding rather than a stronger ball");
            HtmlPhysicsContacts.PinFloorFriction=savedFriction;Time.timeScale=1;game.LoadLevel(0);yield return null;var stationary=new Vector3[game.Pins.Count];for(int i=0;i<stationary.Length;i++)stationary[i]=game.Pins[i].Body.position;yield return new WaitForSeconds(2);bool idle=true;for(int i=0;i<stationary.Length;i++)idle &= Vector3.Distance(stationary[i],game.Pins[i].Body.position)<.005f;Check(idle&&game.Cleared==0,"Unhit pins stay upright and stationary on the tuned table");
            var blocked=game.Pins.FindAll(p=>p.ColorId==1);var blockedAt=blocked.ConvertAll(p=>p.Body.position);game.Shoot(new Vector3(0,.4f,-.2f),1);yield return new WaitForSeconds(2);bool stayed=true;for(int i=0;i<blocked.Count;i++)stayed &= Vector3.Distance(blocked[i].Body.position,blockedAt[i])<.001f;Check(stayed,"Wrong-color pins remain fixed under a direct full-power ball impact");Check(results.shots.TrueForAll(s=>s.lockedStayed),"Every tested floor setting preserves the different-color fixed-obstacle rule");File.WriteAllText(Path.Combine(folder,"physics-review-comparison.json"),JsonUtility.ToJson(results,true));
        }
        static float PinOverlap(PinTarget a,PinTarget b){float max=0;foreach(var x in a.Hitboxes)foreach(var y in b.Hitboxes)if(Physics.ComputePenetration(x,x.transform.position,x.transform.rotation,y,y.transform.position,y.transform.rotation,out var direction,out float distance))max=Mathf.Max(max,distance);return max;}
        IEnumerator PinBehaviorChecks(){
            Screen.SetResolution(600,1000,false);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.3f);
            for(int level=0;level<12;level++){
                game.LoadLevel(level);Physics.SyncTransforms();bool allSolid=true,allPairs=true;float overlap=0;
                foreach(var pin in game.Pins){allSolid &= pin.Hitboxes.Length==3;foreach(var collider in pin.Hitboxes)allSolid &= collider.enabled&&!collider.isTrigger&&collider.attachedRigidbody==pin.Body&&((MeshCollider)collider).convex;}
                for(int a=0;a<game.Pins.Count;a++)for(int b=a+1;b<game.Pins.Count;b++){foreach(var x in game.Pins[a].Hitboxes)foreach(var y in game.Pins[b].Hitboxes)allPairs &= !Physics.GetIgnoreCollision(x,y);overlap=Mathf.Max(overlap,PinOverlap(game.Pins[a],game.Pins[b]));}
                Check(allSolid&&allPairs&&overlap<.01f,"Chapter "+(level+1)+" starts separated with solid body/neck/head collision for every color pair");
            }
            game.LoadLevel(0);yield return null;var anchor=(Vector2)game.Camera.WorldToScreenPoint(PinPopGame.Launch);int moves=game.Moves;
            foreach(var offset in new[]{new Vector2(-120,-80),new Vector2(120,-80),new Vector2(0,-140),new Vector2(0,140),new Vector2(-140,0),new Vector2(140,0)}){
                game.BeginPullForVerification(anchor);game.UpdatePull(anchor+new Vector2(offset.x*game.Camera.pixelWidth/600,offset.y*game.Camera.pixelHeight/1000));var ready=GameObject.Find("Next bowling ball");Check(game.IsPulling&&game.PullPower>.5f&&Vector3.Distance(ready.transform.position,PinPopGame.Launch)<.0001f,"Pull changes aim/power while the ball stays fixed: "+offset);game.CancelPull();
            }
            game.BeginPullForVerification(anchor);game.UpdatePull(anchor+Vector2.down*140*game.Camera.pixelHeight/1000);Capture("pin-behavior-held.png");game.PointerUp();var launched=FindFirstObjectByType<BowlingBall>();Check(launched!=null&&Vector3.Distance(launched.transform.position,PinPopGame.Launch)<.0001f&&game.Moves==moves-1,"Release launches from the same fixed anchor and consumes one move");
            for(int color=0;color<4;color++){
                game.LoadLevel(0);yield return null;int c=color%3;var a=game.Pins.Find(p=>p.ColorId==c);var b=game.Pins.Find(p=>p!=a&&p.ColorId==(color==3?c:(c+1)%3));game.PrepareShotColor(c);
                foreach(var p in game.Pins)if(p!=a&&p!=b)p.gameObject.SetActive(false);
                a.Body.constraints=RigidbodyConstraints.FreezeRotation|RigidbodyConstraints.FreezePositionY|RigidbodyConstraints.FreezePositionZ;b.Body.constraints=a.Body.constraints;a.Body.position=new Vector3(-1.2f,.02f,-3);b.Body.position=new Vector3(0,.02f,-3);Physics.SyncTransforms();a.Body.linearVelocity=Vector3.right*4;a.Body.WakeUp();float overlap=0;
                for(int step=0;step<65;step++){yield return new WaitForFixedUpdate();overlap=Mathf.Max(overlap,PinOverlap(a,b));}
                Check(overlap<.02f&&b.Body.position.x-a.Body.position.x>.50f,"Actual contact keeps pin surfaces separated: "+c+" against "+b.ColorId+"; max overlap="+overlap);
                Check(color==3?b.Body.position.x>.1f:Mathf.Abs(b.Body.position.x)<.001f,color==3?"Same-color pins transfer physical motion":"Different-color blocker remains fixed after pin impact");
                if(color==0)Capture("pin-behavior-contact.png");
            }
            foreach(int level in new[]{0,2,5}){
                game.LoadLevel(level);yield return null;int color=game.Queue[0];var target=game.Pins.Find(p=>p.ColorId==color);game.Shoot(target.Body.worldCenterOfMass,1);float overlap=0;
                for(int step=0;step<240;step++){yield return new WaitForFixedUpdate();for(int x=0;x<game.Pins.Count;x++)for(int y=x+1;y<game.Pins.Count;y++){var left=game.Pins[x];var right=game.Pins[y];if(left!=null&&right!=null&&!left.Cleared&&!right.Cleared)overlap=Mathf.Max(overlap,PinOverlap(left,right));}}
                Check(overlap<.02f,"Full-power free-rotation shot keeps visible pin bodies separated in chapter "+(level+1)+"; max overlap="+overlap);
            }
            game.LoadLevel(0);yield return null;game.PrepareShotColor(0);var falling=game.Pins.Find(p=>p.ColorId==0);falling.Body.position=new Vector3(-5,1.5f,0);falling.Body.WakeUp();float began=Time.realtimeSinceStartup;while(game.Cleared<1&&Time.realtimeSinceStartup-began<4)yield return null;yield return new WaitForEndOfFrame();Capture("pin-behavior-flight.png");
            var graphic=GameObject.Find("Collected color 1").GetComponent<UnityEngine.UI.Image>();Check(graphic.sprite==Resources.Load<Sprite>("PinIcons/pin-0")&&graphic.type==UnityEngine.UI.Image.Type.Simple&&graphic.preserveAspect,"A physically fallen target flies as the actual printed bowling pin, preserving its silhouette");
            yield return new WaitForSecondsRealtime(.8f);yield return new WaitForEndOfFrame();Capture("pin-behavior-collected.png");Check(game.MatchTray.LiveCount==1,"Collected pin remains in its slot without becoming a cube");
            game.MatchTray.Collect(1,new Vector2(.6f,.6f));game.MatchTray.Collect(0,new Vector2(.3f,.6f));yield return new WaitForSecondsRealtime(.8f);
            foreach(var size in new[]{new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);yield return new WaitForEndOfFrame();Capture("pin-behavior-"+size.x+".png",Screen.width,Screen.height);Check(GameObject.Find("Collected color 1").GetComponent<UnityEngine.UI.Image>().preserveAspect,"Collected pin proportions survive resize to "+size.x+"x"+size.y);}
        }
        IEnumerator TrayPolishChecks(){
            Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.6f);yield return new WaitForEndOfFrame();Capture("tray-polish-empty.png");
            var ui=FindFirstObjectByType<HtmlMatchTrayUI>();bool passThrough=true;foreach(var graphic in ui.GetComponentsInChildren<UnityEngine.UI.Graphic>())passThrough &= !graphic.raycastTarget;Check(passThrough,"Collection panel leaves bowling gestures unobstructed");
            game.MatchTray.Collect(0,new Vector2(.2f,.6f));game.MatchTray.Collect(0,new Vector2(.3f,.6f));yield return new WaitForSecondsRealtime(.8f);yield return new WaitForEndOfFrame();Capture("tray-polish-pair.png");
            Check(game.MatchTray.LiveCount==2&&game.MatchTray.MatchCount==0,"Pair hint waits for a third piece without changing the match rules");
            game.MatchTray.Collect(1,new Vector2(.6f,.5f));yield return new WaitForSecondsRealtime(.8f);yield return new WaitForEndOfFrame();Capture("tray-polish-mixed.png");
            var firstGlow=GameObject.Find("Collected color 1").GetComponent<UnityEngine.UI.Outline>();var otherGlow=GameObject.Find("Collected color 3").GetComponent<UnityEngine.UI.Outline>();Check(firstGlow.effectColor.a>.4f&&otherGlow.effectColor.a==0,"Only matching pairs receive the ready-pair halo");
            game.MatchTray.Collect(0,new Vector2(.8f,.6f));yield return new WaitForSecondsRealtime(.25f);yield return new WaitForEndOfFrame();Capture("tray-polish-flight.png");yield return new WaitForSecondsRealtime(.42f);yield return new WaitForEndOfFrame();Capture("tray-polish-merge.png");yield return new WaitForSecondsRealtime(.5f);Check(game.MatchTray.LiveCount==1&&game.MatchTray.MatchCount==1,"Polished merge still removes exactly three and compacts the surviving piece");
            foreach(var size in new[]{new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.35f);yield return new WaitForEndOfFrame();Capture("tray-polish-"+size.x+".png",Screen.width,Screen.height);ui=FindFirstObjectByType<HtmlMatchTrayUI>();var corners=new Vector3[4];ui.transform.Find("Collection tray").GetComponent<RectTransform>().GetWorldCorners(corners);var left=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[0]);var right=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[2]);Check(left.x>=0&&right.x<=Screen.width,"Polished dock fits "+size.x+"x"+size.y);}
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);game.MatchTray.Clear();for(int i=0;i<8;i++)game.MatchTray.Collect(i/2,new Vector2(.5f,.5f));yield return new WaitForSecondsRealtime(.9f);yield return new WaitForEndOfFrame();Capture("tray-polish-capacity.png");
            game.LoadLevel(0);yield return new WaitForSecondsRealtime(.15f);Check(game.MatchTray.Empty&&GameObject.Find("Collected color 1")==null,"Restart clears all polished tokens and pair hints");
        }
        [System.Serializable] class CampaignAudit {public System.Collections.Generic.List<CampaignLevel> levels=new System.Collections.Generic.List<CampaignLevel>();}
        [System.Serializable] class CampaignLevel {public int level,moves;public System.Collections.Generic.List<CampaignPin> pins=new System.Collections.Generic.List<CampaignPin>();}
        [System.Serializable] class CampaignPin {public float x,z;public int color;}
        IEnumerator FortyLevelChecks(){
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.2f);var audit=new CampaignAudit();var signatures=new System.Collections.Generic.HashSet<string>();var names=new System.Collections.Generic.HashSet<string>();
            Check(AdventureRules.Chapters==40&&AdventureRules.RegionCount==2&&AdventureRules.MapPageCount==8,"Campaign contains forty levels, two regions and eight checkpoints");
            for(int l=0;l<40;l++){var pins=PinPopLayouts.Build(l);var row=new CampaignLevel{level=l,moves=PinPopEconomy.MoveBudget(l)};bool valid=pins.Count==row.moves&&pins.Count%3==0,spacing=true;var signature=new System.Text.StringBuilder();
                for(int c=0;c<PinPopLayouts.ColorCount(l);c++){int count=pins.FindAll(p=>p.Color==c).Count;valid&=count>0&&count%3==0;bool supplied=false;for(int q=0;q<row.moves;q++)supplied|=PinPopLayouts.QueueColor(l,q)==c;valid&=supplied;}
                for(int i=0;i<pins.Count;i++){var pin=pins[i];valid&=Mathf.Abs(pin.Position.x)<3.6f&&pin.Position.z>-3&&pin.Position.z<4;for(int j=0;j<i;j++)spacing&=Vector3.Distance(pin.Position,pins[j].Position)>.59f;row.pins.Add(new CampaignPin{x=pin.Position.x,z=pin.Position.z,color=pin.Color});signature.Append(pin.Position.x.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)).Append(',').Append(pin.Position.z.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)).Append(':').Append(pin.Color).Append(';');}
                Check(valid&&spacing,"Level "+(l+1)+" has complete triples, supplied colors, safe spacing and table bounds");if(l>=20)Check(signatures.Add(signature.ToString()),"New level "+(l+1)+" has a distinct arrangement");Check(names.Add(CarnivalMapCatalog.StopName(l)),"Level "+(l+1)+" has a distinct map name");audit.levels.Add(row);
                if(l>=20){game.LoadLevel(l);yield return null;Check(game.Level==l&&game.Pins.Count==pins.Count&&game.Queue.Count==game.Moves+1,"New level "+(l+1)+" loads its physics pins and queue");if(l==20||l==29||l==39){yield return new WaitForSecondsRealtime(.1f);Capture("forty-level-unity-"+(l+1)+".png");}}
            }
            File.WriteAllText(Path.Combine(folder,"forty-level-unity-layouts.json"),JsonUtility.ToJson(audit,true));
            var save=new EconomySave{coins=176,swaps=4,highestLevel=20,ownedMask=3,selectedBall=1};save.earnings.Add(new LevelEarnings{level=19,bestMoves=7});PlayerPrefs.SetInt("Level",20);PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(save));PlayerPrefs.SetString(PinPopAdventure.SaveKey,JsonUtility.ToJson(new AdventureSave{chests=15,stock=new[]{6,5,4,3}}));game.ReloadEconomy();game.Adventure.Reload();game.LoadLevel(game.HighestLevel);game.SetMenu(true);game.Hud.ShowHome();
            Check(game.Level==20&&game.Coins==176&&game.SwapCredits==4&&game.SelectedBall==1&&game.Adventure.Data.chests==15&&game.Adventure.Data.stock[0]==6,"Twenty-level save resumes at level twenty-one without losing wallet or rewards");
            var ui=game.GetComponent<AdventureHUD>();ui.ShowMap();yield return new WaitForSecondsRealtime(.2f);Check(GameObject.Find("MAP_LEVEL_0")==null&&GameObject.Find("MAP_LEVEL_20")!=null&&GameObject.Find("MAP_LEVEL_39")!=null&&GameObject.Find("MAP_LEVEL_40")==null,"Second region contains only levels twenty-one through forty");Check(GameObject.Find("MAP_LEVEL_20").GetComponent<UnityEngine.UI.Button>().interactable&&!GameObject.Find("MAP_LEVEL_21").GetComponent<UnityEngine.UI.Button>().interactable,"Old campaign completion unlocks only the next new level");Capture("forty-level-unity-map-21.png");MapClick("MAP_REGION_0");yield return new WaitForSecondsRealtime(.15f);Check(GameObject.Find("MAP_LEVEL_0")!=null&&GameObject.Find("MAP_LEVEL_19")!=null&&GameObject.Find("MAP_LEVEL_20")==null&&GameObject.Find("Region progress label").GetComponent<UnityEngine.UI.Text>().text=="20 / 20","Previous region has only its own twenty levels and completed progress");Capture("region-twenty-unity-first.png");MapClick("MAP_REGION_1");yield return new WaitForSecondsRealtime(.15f);Check(GameObject.Find("MAP_LEVEL_20")!=null&&GameObject.Find("MAP_LEVEL_19")==null&&GameObject.Find("Region progress label").GetComponent<UnityEngine.UI.Text>().text=="0 / 20","Switching back opens second region at zero of twenty");Check(game.Coins==176&&game.Adventure.Data.chests==15&&game.Adventure.Data.stock[0]==6,"Region switching preserves wallet, claimed chests and stock");MapClick("MAP_LEVEL_20");while(game.Transitioning)yield return null;Check(game.Level==20&&!game.InMenu,"Map actually launches newly unlocked level twenty-one");
            game.LoadLevel(19);typeof(PinPopGame).GetProperty("Cleared").SetValue(game,game.Total);game.SendMessage("End",true);yield return new WaitForSecondsRealtime(.2f);MapClick("CONTINUE");while(game.Transitioning)yield return null;Check(game.Level==20&&!game.InMenu,"Actual result continue advances twenty to twenty-one");
            var economy=new PinPopEconomy(save);for(int l=20;l<40;l++)economy.Complete(l,2,out _,out _);economy.Save();game.ReloadEconomy();Check(game.HighestLevel==40&&!AdventureRules.FinalLevel(19)&&!AdventureRules.FinalLevel(38)&&AdventureRules.FinalLevel(39),"Campaign ends at forty, with progression continuing after twenty");
            bool rewards=true;for(int c=0;c<8;c++){bool old=c<4;rewards&=game.Adventure.ClaimChest(c)==!old;rewards&=!game.Adventure.ClaimChest(c);}Check(rewards&&game.Adventure.Data.chests==255,"All eight checkpoint rewards are single claim and old rewards remain claimed");
            game.LoadLevel(39);typeof(PinPopGame).GetProperty("Cleared").SetValue(game,game.Total);game.SendMessage("End",true);yield return new WaitForSecondsRealtime(.2f);MapClick("CONTINUE");Check(game.InMenu&&ui.ModalVisible,"Final result action returns to the map after level forty");yield return new WaitForSecondsRealtime(.2f);var map=FindFirstObjectByType<CarnivalMapView>();Check(map.FocusedLevel==39&&Mathf.Abs(PixelRect("MAP_LEVEL_39").center.y-Screen.height/2f)<2,"Completed campaign focuses level forty");Check(GameObject.Find("Region progress label").GetComponent<UnityEngine.UI.Text>().text=="20 / 20","Completed second region shows twenty of twenty");Capture("forty-level-unity-map-complete.png");ui.Close();
        }
        IEnumerator SplashChecks(){
            yield return new WaitForSecondsRealtime(.18f);
            Check(PinPopSplash.Visible&&PinPopSplash.Instance.Ready,"Startup shows branded splash while prepared menu stays behind it");Check(game.InMenu&&game.Hud.HomeVisible,"Startup prepares home without launching a level");
            Check(GameObject.Find("Splash shared title").GetComponent<UnityEngine.UI.Image>().sprite.texture.width>=1600&&GameObject.Find("Splash bowling ball")!=null,"Splash contains PinPop wordmark and bowling artwork");Check(GameObject.Find("Splash pin 0")!=null&&GameObject.Find("Splash pin 1")!=null&&GameObject.Find("Splash pin 2")!=null,"All three character pins are present");
            Check(GameObject.Find("Splash carnival").GetComponent<UnityEngine.UI.Image>().sprite!=null&&GameObject.Find("Splash shared title").GetComponent<UnityEngine.UI.Image>().sprite!=null,"Splash uses full carnival scenery and a large wordmark");Check(GameObject.Find("Splash loading fill").GetComponent<RectTransform>().sizeDelta.x>=239,"Ready loading strip is filled");
            var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=PixelRect("PLAY").center};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointer,hits);Check(hits.Count>0&&hits[0].gameObject.name=="Startup blocker","Splash intercepts taps over the menu play button");var adventure=game.GetComponent<AdventureHUD>();adventure.ShowMap();Check(!adventure.ModalVisible,"Map cannot open while startup splash is active");Capture("splash-unity-opening.png");
            int coins=game.Coins,swaps=game.SwapCredits,level=game.Level;float started=Time.realtimeSinceStartup;bool noLogoOverlap=true;while(PinPopSplash.Visible&&Time.realtimeSinceStartup-started<10){var splash=PinPopSplash.Instance;noLogoOverlap&=splash.CoverOpacity>=.999f||splash.ArtworkOpacity<.001f;yield return null;}Check(noLogoOverlap,"Splash artwork disappears before the menu is revealed");
            Check(!PinPopSplash.Visible&&PinPopSplash.LastReadyDisplaySeconds>=PinPopSplash.MinimumDuration+PinPopSplash.FadeDuration-.04f,"Splash remains readable after setup and first ready frame, then fades out");Check(game.InMenu&&game.Hud.HomeVisible&&!adventure.ModalVisible,"Splash transitions automatically into home");Check(game.Coins==coins&&game.SwapCredits==swaps&&game.Level==level,"Splash preserves wallet, swap inventory and current level");Capture("splash-unity-home.png");
            MapClick("PLAY");yield return new WaitForSecondsRealtime(.25f);Check(adventure.ModalVisible&&!PinPopSplash.Visible,"Menu works after splash and does not replay splash");adventure.Close();game.Hud.ReturnHome();yield return null;Check(!PinPopSplash.Visible,"Returning home never shows startup splash again");
        }
        IEnumerator PowerMatchPolishChecks(){
            game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();
            foreach(var size in new[]{new Vector2Int(600,1000),new Vector2Int(320,740),new Vector2Int(1000,600)}){
                Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);Check(GameObject.Find("Shot power")==null&&GameObject.Find("Power card")==null&&!game.AimVisible,"Idle has no power card or aiming ring: "+size);
                foreach(float power in new[]{.25f,.6f,1f}){
                    yield return null;var anchor=(Vector2)game.Camera.WorldToScreenPoint(game.LaunchPosition);game.BeginPullForVerification(anchor);game.UpdatePull(anchor+Vector2.down*(12+128*power)*Screen.height/1000);yield return new WaitForEndOfFrame();
                    Check(game.AimVisible&&Mathf.Abs(game.PowerRing.Power-power)<.01f,"Endpoint ring tracks power "+power+" at "+size);
                    var expected=power<.5f?Color.Lerp(Art.Hex("43CEC8"),Art.Hex("FFD15C"),power*2):Color.Lerp(Art.Hex("FFD15C"),Art.Hex("F27672"),(power-.5f)*2);Check(Vector4.Distance(expected,game.PowerRing.Tint)<.01f,"Power tint transitions turquoise through yellow to coral: "+power);
                    var line=GameObject.Find("Table aim dot 0").GetComponent<Renderer>().sharedMaterial.GetColor("_Color");var ball=Art.Colors[game.Queue[0]].linear;Check(Vector3.Distance(new Vector3(line.r,line.g,line.b),new Vector3(ball.r,ball.g,ball.b))<.01f,"Trajectory keeps matching ball color: "+power);
                    Check(game.PowerRing.Maximum==(power>=.995f),"Glow belongs only to full power: "+power);
                    var mesh=game.PowerRing.GetComponent<MeshFilter>().sharedMesh;var points=mesh.vertices;if(power==.25f)Check(points[AimPowerRing.Segments*2].x>.55f&&Mathf.Abs(points[AimPowerRing.Segments*2].z)<.01f,"Quarter power draws a real quarter-ring mesh");if(power==1)Check(Vector3.Distance(points[0],points[AimPowerRing.Segments*2])<.001f,"Full power closes the complete ring");
                    Capture("aim-power-ring-unity-"+size.x+"-"+Mathf.RoundToInt(power*100)+".png",Screen.width,Screen.height);game.CancelPull();Check(!game.AimVisible&&game.PowerRing.Power==0,"Cancel clears ring and glow");
                }
                var start=(Vector2)game.Camera.WorldToScreenPoint(game.LaunchPosition);game.BeginPullForVerification(start);game.UpdatePull(start+Vector2.right*100);Check(game.IsPositioning&&!game.AimVisible,"Horizontal positioning never shows a power ring: "+size);game.CancelPull();game.SetLaunchX(0);
            }
            var at=(Vector2)game.Camera.WorldToScreenPoint(game.LaunchPosition);game.BeginPullForVerification(at);game.UpdatePull(at+Vector2.down*140*Screen.height/1000);game.PointerUp();Check(game.Busy&&!game.AimVisible&&game.Moves==game.Total-1,"Releasing shoots once and immediately hides power ring");
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);game.LoadLevel(0);yield return null;
            for(int i=0;i<3;i++)game.MatchTray.Collect(0,new Vector2(.5f,.45f));yield return new WaitForSeconds(.68f);Check(game.MatchTray.Items.Count==3&&game.MatchTray.MatchCount==0,"Real triple remains visible through slower convergence");Capture("power-match-polish-converging.png");float age=game.MatchTray.Items[0].MergeAge;game.Hud.ShowSettings();yield return new WaitForSecondsRealtime(.2f);Check(game.MatchTray.Items[0].MergeAge==age,"Pause freezes slower merge");game.Hud.CloseOverlays();yield return new WaitForSeconds(.2f);Check(game.MatchTray.Items.Count==3&&game.MatchTray.MatchCount==1,"Burst begins while all three icons still exist");Capture("power-match-polish-burst.png");yield return new WaitForSeconds(.45f);Check(game.MatchTray.Empty&&game.MatchTray.MatchCount==1,"Slower burst cleans up exactly once");
        }
        IEnumerator CarnivalMapChecks(){
            bool hadMotion=PlayerPrefs.HasKey("ReducedMotion");int motion=PlayerPrefs.GetInt("ReducedMotion",0);PlayerPrefs.SetInt("ReducedMotion",0);PlayerPrefs.SetInt(PinPopHUD.TutorialSeenKey,1);PlayerPrefs.DeleteKey(PinPopAdventure.SaveKey);game.Adventure.Reload();Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(true);game.Hud.ShowHome();yield return new WaitForSecondsRealtime(.3f);MapClick("PLAY");yield return new WaitForSecondsRealtime(.3f);var ui=game.GetComponent<AdventureHUD>();var map=FindFirstObjectByType<CarnivalMapView>();
            Check(ui.ModalVisible&&Time.timeScale==0,"Home opens paused scrolling map");Check(GameObject.Find("MAP_NEXT")==null&&GameObject.Find("MAP_PREVIOUS")==null,"Page arrows are removed");
            Check(!GameObject.Find("MAP_REGION_1").GetComponent<UnityEngine.UI.Button>().interactable&&GameObject.Find("MAP_LEVEL_20")==null,"Second region stays locked until twenty levels are completed");
            var icons=new System.Collections.Generic.HashSet<string>();for(int i=0;i<20;i++){var node=GameObject.Find("MAP_LEVEL_"+i);Check(node!=null,"Chapter "+(i+1)+" exists in the single scroll content");var icon=GameObject.Find("Map unique icon "+i).GetComponent<UnityEngine.UI.Image>();icons.Add(icon.sprite.name);}Check(icons.Count==20,"All twenty original attraction icons remain");
            Check(map.MapScroll.vertical&&!map.MapScroll.horizontal&&map.MapScroll.content.rect.height>map.MapScroll.viewport.rect.height,"Map has one vertical scrolling viewport");Check(Mathf.Abs(PixelRect("MAP_LEVEL_0").center.y-Screen.height/2f)<2,"First chapter is centered at initial open");
            var scroll=map.MapScroll;scroll.OnScroll(new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){scrollDelta=new Vector2(0,-5)});yield return new WaitForSecondsRealtime(.2f);float offset=map.ScrollOffset;Check(offset>100,"Mouse wheel scrolls route downward");yield return new WaitForSecondsRealtime(.3f);Check(Mathf.Abs(map.ScrollOffset-offset)<.1f,"Manual scrolling is not pulled back to the current chapter");
            map.SetScrollOffset(0);var point=PixelRect("MAP_LEVEL_0").center;var evt=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=point,button=UnityEngine.EventSystems.PointerEventData.InputButton.Left};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();UnityEngine.EventSystems.EventSystem.current.RaycastAll(evt,hits);if(hits.Count>0)evt.pointerPressRaycast=hits[0];scroll.OnInitializePotentialDrag(evt);scroll.OnBeginDrag(evt);evt.position=point+Vector2.up*150;evt.delta=Vector2.up*150;scroll.OnDrag(evt);scroll.OnEndDrag(evt);scroll.StopMovement();yield return null;Check(map.ScrollOffset>100,"Pointer drag scrolls real Unity ScrollRect");
            ui.Close();ui.ShowMap();yield return new WaitForSecondsRealtime(.25f);map=FindFirstObjectByType<CarnivalMapView>();Check(map.ScrollOffset<.1f,"Reopening recenters instead of restoring an arbitrary manual scroll");
            var effects=GameObject.Find("Map theme effects").GetComponent<CarnivalMapGraphic>();var clock=typeof(CarnivalMapGraphic).GetField("clock",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);float tick=(float)clock.GetValue(effects);yield return new WaitForSecondsRealtime(.25f);Check((float)clock.GetValue(effects)>tick,"Decorations animate while map pauses game time");ui.Close();PlayerPrefs.SetInt("ReducedMotion",1);ui.ShowMap();yield return new WaitForSecondsRealtime(.2f);effects=GameObject.Find("Map theme effects").GetComponent<CarnivalMapGraphic>();tick=(float)clock.GetValue(effects);yield return new WaitForSecondsRealtime(.25f);Check((float)clock.GetValue(effects)==tick,"Reduced motion freezes decorative animation");ui.Close();PlayerPrefs.SetInt("ReducedMotion",0);
            foreach(var size in new[]{new Vector2Int(600,1000),new Vector2Int(320,740),new Vector2Int(1000,600)}){
                Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);
                foreach(int progress in new[]{0,19,20,39}){var save=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(PinPopEconomy.SaveKey));save.highestLevel=progress;PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(save));game.ReloadEconomy();ui.ShowMap();yield return new WaitForSecondsRealtime(.3f);map=FindFirstObjectByType<CarnivalMapView>();
                    Check(map.FocusedLevel==progress&&Mathf.Abs(PixelRect("MAP_LEVEL_"+progress).center.y-Screen.height/2f)<2,"Current chapter "+(progress+1)+" centers at "+size);
                    var viewport=PixelRect("Adventure card");Check(viewport.xMin>=-1&&viewport.xMax<=Screen.width+1&&viewport.yMin>=-1&&viewport.yMax<=Screen.height+1,"Scroll map stays in screen at "+size);
                    bool touch=true,readable=true;foreach(var button in map.GetComponentsInChildren<UnityEngine.UI.Button>()){var rect=PixelRect(button.name);touch&=rect.width>=43.8f&&rect.height>=43.8f;}foreach(var text in map.GetComponentsInChildren<UnityEngine.UI.Text>())readable&=text.preferredWidth<=text.rectTransform.rect.width+1&&text.preferredHeight<=text.rectTransform.rect.height+1;
                    Check(touch&&readable,"Touch targets and labels fit at "+size+" progress "+progress);Capture("scroll-map-unity-"+size.x+"-"+progress+".png",Screen.width,Screen.height);ui.Close();
                }
            }
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);var funded=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(PinPopEconomy.SaveKey));funded.highestLevel=5;funded.coins=120;PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(funded));game.ReloadEconomy();ui.ShowMap();yield return new WaitForSecondsRealtime(.3f);map=FindFirstObjectByType<CarnivalMapView>();float before=map.ScrollOffset;Check(GameObject.Find("MAP_CHEST_0").GetComponent<UnityEngine.UI.Button>().interactable&&!GameObject.Find("MAP_CHEST_1").GetComponent<UnityEngine.UI.Button>().interactable,"Intermediate and locked chests coexist on one route");MapClick("MAP_CHEST_0");yield return new WaitForSecondsRealtime(.3f);map=FindFirstObjectByType<CarnivalMapView>();Check(game.Coins==145&&game.Adventure.Data.stock[0]==4&&!game.Adventure.ClaimChest(0),"Chest rewards remain single-use");Check(Mathf.Abs(map.ScrollOffset-before)<.1f,"Claiming a chest preserves manual map position");
            MapClick("MAP_LEVEL_5");while(game.Transitioning)yield return null;Check(game.Level==5&&!game.InMenu,"Tap launches the correct chapter from scrolling map");game.SetMenu(true);game.Hud.ShowHome();funded=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(PinPopEconomy.SaveKey));funded.highestLevel=40;PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(funded));game.ReloadEconomy();ui.ShowMap();yield return new WaitForSecondsRealtime(.3f);Check(Mathf.Abs(PixelRect("MAP_LEVEL_39").center.y-Screen.height/2f)<2&&GameObject.Find("MAP_LEVEL_40")==null,"Completed campaign centers last chapter without adding content");Check(GameObject.Find("Region progress label").GetComponent<UnityEngine.UI.Text>().text=="20 / 20","Completed region progress remains twenty of twenty");Capture("scroll-map-unity-complete.png");ui.Close();if(hadMotion)PlayerPrefs.SetInt("ReducedMotion",motion);else PlayerPrefs.DeleteKey("ReducedMotion");
        }
        void MapClick(string name){Canvas.ForceUpdateCanvases();game.Hud.UICamera.Render();Click(name);}
        IEnumerator AnimatedHelpChecks(){
            PlayerPrefs.DeleteKey(PinPopHUD.TutorialSeenKey);Screen.SetResolution(600,1000,false);game.SetMenu(true);game.Hud.ShowHome();game.Hud.PlayLevel(1);while(game.Transitioning)yield return null;yield return new WaitForSecondsRealtime(.2f);Check(!game.Hud.HelpVisible&&!PlayerPrefs.HasKey(PinPopHUD.TutorialSeenKey),"Other chapters do not trigger or consume first-chapter tutorial");
            game.Hud.PlayLevel(0);while(game.Transitioning)yield return null;yield return new WaitForSecondsRealtime(.3f);var demo=FindFirstObjectByType<AnimatedHowTo>();Check(demo!=null&&demo.Step==0&&game.Paused,"First entry automatically opens tutorial and pauses gameplay");Check(PlayerPrefs.GetInt(PinPopHUD.TutorialSeenKey,0)==1,"First showing persists seen flag immediately");Check(demo.GetComponentsInChildren<UnityEngine.UI.Button>(true).Length==1&&GameObject.Find("HELP_CLOSE")!=null&&GameObject.Find("HELP_HOME")==null&&GameObject.Find("HELP_GAME")==null,"Tutorial has one close cross and no extra navigation buttons");
            int closeMoves=game.Moves,closeCoins=game.Coins,closeSwaps=game.SwapCredits;var closeQueue=game.Queue.ToArray();
            for(int step=0;step<3;step++){demo.Select(step);yield return new WaitForSecondsRealtime(.3f);var closeRect=PixelRect("HELP_CLOSE");var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=closeRect.center,button=UnityEngine.EventSystems.PointerEventData.InputButton.Left};var results=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();Canvas.ForceUpdateCanvases();UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointer,results);var target=results.Count>0?UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<UnityEngine.EventSystems.IPointerClickHandler>(results[0].gameObject):null;Check(target!=null&&target.name=="HELP_CLOSE","Close cross receives the actual pointer raycast in scene "+step);if(target!=null)UnityEngine.EventSystems.ExecuteEvents.Execute(target,pointer,UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);Check(!game.Hud.HelpVisible&&!game.Paused&&Time.timeScale==1,"Close cross immediately resumes play from scene "+step);game.Hud.ShowHelp();yield return new WaitForSecondsRealtime(.3f);demo=FindFirstObjectByType<AnimatedHowTo>();}
            bool closePreserved=game.Moves==closeMoves&&game.Coins==closeCoins&&game.SwapCredits==closeSwaps&&game.Queue.Count==closeQueue.Length;for(int i=0;i<closeQueue.Length;i++)closePreserved&=closeQueue[i]==game.Queue[i];Check(closePreserved,"Closing tutorial preserves queue, moves and wallet");
            int coins=game.Coins,moves=game.Moves,swaps=game.SwapCredits;var queue=game.Queue.ToArray();var positions=game.Pins.ConvertAll(p=>p.transform.position);var hand=GameObject.Find("Demo touch").GetComponent<RectTransform>();var ball=GameObject.Find("Demo bowling ball").GetComponent<RectTransform>();var touchAt=hand.anchoredPosition;var ballAt=ball.anchoredPosition;yield return new WaitForSecondsRealtime(1.4f);Check(Vector2.Distance(hand.anchoredPosition,touchAt)>15&&Vector2.Distance(ball.anchoredPosition,ballAt)<.1f,"Animation runs during pause while held bowling ball stays still");Capture("animated-help-unity-pull.png");
            SwipeHelp(-120,0,17);Check(demo.Step==1,"Finger swipe left advances one scene");SwipeHelp(120,0,17);Check(demo.Step==0,"Finger swipe right returns one scene");SwipeHelp(120,0);Check(demo.Step==0,"Right swipe at first scene does not wrap");SwipeHelp(8,0);SwipeHelp(12,120);Check(demo.Step==0,"Taps and vertical drags do not change scenes");SwipeHelp(-120,0);Check(demo.Step==1,"Mouse drag also advances scene");SwipeHelp(-120,0);Check(demo.Step==2,"Left swipe reaches last scene");SwipeHelp(120,0);Check(demo.Step==1,"Right swipe can revisit previous scene");Screen.SetResolution(390,844,false);yield return new WaitForSecondsRealtime(.35f);demo=FindFirstObjectByType<AnimatedHowTo>();Check(demo.Step==1&&demo.DemoTime>.2f,"Resizing preserves tutorial scene and playback position");Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);demo=FindFirstObjectByType<AnimatedHowTo>();demo.Select(0);
            float start=Time.realtimeSinceStartup;while(demo.Step==0&&Time.realtimeSinceStartup-start<6)yield return null;Check(demo.Step==1,"First video automatically advances after its full duration");yield return new WaitForSecondsRealtime(.7f);Capture("animated-help-unity-color.png");var pin=GameObject.Find("Demo pin 1").GetComponent<RectTransform>();var fixedAt=pin.anchoredPosition;yield return new WaitForSecondsRealtime(.8f);Check(Vector2.Distance(pin.anchoredPosition,fixedAt)<.1f,"Wrong color demonstration keeps targets fixed");yield return new WaitForSecondsRealtime(4.1f);Capture("animated-help-unity-triple.png");yield return new WaitForSecondsRealtime(.8f);Check(GameObject.Find("Help demonstration").GetComponent<UnityEngine.UI.RectMask2D>().isActiveAndEnabled,"Effects stay clipped inside teaching stage");Capture("animated-help-unity-burst.png");start=Time.realtimeSinceStartup;while(demo.Step==1&&Time.realtimeSinceStartup-start<2.5f)yield return null;Check(demo.Step==2,"Second video automatically advances to swap");yield return new WaitForSecondsRealtime(2.9f);Capture("animated-help-unity-swap.png");Check(GameObject.Find("Demo queued ball 0").GetComponent<RectTransform>().anchoredPosition.x>GameObject.Find("Demo queued ball 1").GetComponent<RectTransform>().anchoredPosition.x,"Swap illustration sends first bowling ball to queue end");start=Time.realtimeSinceStartup;while(game.Hud.HelpVisible&&Time.realtimeSinceStartup-start<4)yield return null;Check(!game.Hud.HelpVisible&&!game.Paused,"Last video automatically closes tutorial and resumes gameplay");
            bool untouched=game.Coins==coins&&game.Moves==moves&&game.SwapCredits==swaps&&game.Cleared==0&&game.Queue.Count==queue.Length;for(int i=0;i<queue.Length;i++)untouched&=game.Queue[i]==queue[i];for(int i=0;i<positions.Count;i++)untouched&=Vector3.Distance(game.Pins[i].transform.position,positions[i])<.01f;Check(untouched,"Automatic playback and swipes never mutate board, queue, wallet, moves or swaps");
            game.Hud.PlayLevel(0);while(game.Transitioning)yield return null;yield return new WaitForSecondsRealtime(.3f);Check(!game.Hud.HelpVisible&&!game.Paused,"Returning to chapter one does not repeat tutorial");
            foreach(var size in new[]{new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);game.Hud.ShowHelp();yield return new WaitForSecondsRealtime(.3f);demo=FindFirstObjectByType<AnimatedHowTo>();var r=GameObject.Find("Animated help card").GetComponent<RectTransform>();var corners=new Vector3[4];r.GetWorldCorners(corners);var low=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[0]);var high=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[2]);Check(low.x>=-1&&high.x<=Screen.width+1&&low.y>=-1&&high.y<=Screen.height+1,"Closable tutorial fits "+size.x);var closeRect=PixelRect("HELP_CLOSE");Check(closeRect.width>=44&&closeRect.height>=44,"Tutorial close target stays comfortably tappable at "+size.x);Capture("animated-help-unity-"+size.x+".png",Screen.width,Screen.height);SwipeHelp(-90,0,7);Check(demo.Step==1,"Swipe target works at "+size.x);game.Hud.CloseOverlays();}
            int motion=PlayerPrefs.GetInt("ReducedMotion",0);PlayerPrefs.SetInt("ReducedMotion",1);game.Hud.ShowHelp();yield return new WaitForSecondsRealtime(.2f);demo=FindFirstObjectByType<AnimatedHowTo>();var still=GameObject.Find("Demo touch").GetComponent<RectTransform>().anchoredPosition;yield return new WaitForSecondsRealtime(.3f);Check(Vector2.Distance(GameObject.Find("Demo touch").GetComponent<RectTransform>().anchoredPosition,still)<.01f,"Reduced motion still uses stable teaching pose");typeof(AnimatedHowTo).GetField("started",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(demo,Time.unscaledTime-demo.Duration);yield return null;yield return null;Check(demo.Step==1,"Reduced motion still advances automatically");SwipeHelp(-120,0);SwipeHelp(-120,0);Check(!game.Hud.HelpVisible&&!game.Paused,"Left swipe on final scene exits directly to game");PlayerPrefs.SetInt("ReducedMotion",motion);
        }
        void SwipeHelp(float dx,float dy,int pointer=-1){var r=GameObject.Find("Help demonstration").GetComponent<RectTransform>();var start=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,r.TransformPoint(r.rect.center));var data=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){pointerId=pointer,button=UnityEngine.EventSystems.PointerEventData.InputButton.Left,position=start,pressPosition=start};var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();Canvas.ForceUpdateCanvases();UnityEngine.EventSystems.EventSystem.current.RaycastAll(data,hits);var target=hits.Count>0?UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<UnityEngine.EventSystems.IPointerDownHandler>(hits[0].gameObject):null;Check(target!=null&&target.GetComponent<AnimatedHowTo>()!=null,"Pointer reaches tutorial swipe surface");if(target==null)return;UnityEngine.EventSystems.ExecuteEvents.Execute(target,data,UnityEngine.EventSystems.ExecuteEvents.pointerDownHandler);data.position=start+new Vector2(dx,dy);UnityEngine.EventSystems.ExecuteEvents.Execute(target,data,UnityEngine.EventSystems.ExecuteEvents.beginDragHandler);UnityEngine.EventSystems.ExecuteEvents.Execute(target,data,UnityEngine.EventSystems.ExecuteEvents.dragHandler);UnityEngine.EventSystems.ExecuteEvents.Execute(target,data,UnityEngine.EventSystems.ExecuteEvents.pointerUpHandler);UnityEngine.EventSystems.ExecuteEvents.Execute(target,data,UnityEngine.EventSystems.ExecuteEvents.endDragHandler);}
        IEnumerator AdventureChecks(){
            PlayerPrefs.DeleteKey(PinPopAdventure.SaveKey);game.Adventure.Reload();Screen.SetResolution(600,1000,false);game.SetMenu(false);game.Hud.ShowGame();var ui=game.GetComponent<AdventureHUD>();
            for(int l=0;l<AdventureRules.Chapters;l++){game.LoadLevel(l);yield return null;Check(game.Pins.Count==PinPopLayouts.TargetCount(l),"Adventure chapter "+(l+1)+" has valid board");}
            game.SetMenu(true);game.Hud.ShowHome();ui.ShowMap();yield return new WaitForSecondsRealtime(.25f);Check(GameObject.Find("MAP_LEVEL_0").GetComponent<UnityEngine.UI.Button>().interactable&&!GameObject.Find("MAP_LEVEL_1").GetComponent<UnityEngine.UI.Button>().interactable,"Map opens first chapter and locks future chapters");Capture("adventure-unity-map.png");ui.Close();game.SetMenu(false);game.Hud.ShowGame();
            game.LoadLevel(4);yield return null;var iced=game.Pins.Find(p=>p.Iced);game.PrepareShotColor(iced.ColorId);Check(iced.IsLocked&&!iced.BreakProtection((iced.ColorId+1)%5)&&iced.Iced,"Ice ignores wrong color");Check(iced.BreakProtection(iced.ColorId)&&!iced.Iced&&iced.IsLocked,"Correct impact cracks ice and waits until next shot");game.PrepareShotColor(iced.ColorId);Check(!iced.IsLocked,"Next shot releases thawed pin");
            game.LoadLevel(4);yield return null;iced=game.Pins.Find(p=>p.Iced&&p.ColorId==0);var iceStart=iced.transform.position;game.ChangeReadyColor(0);game.Shoot(iceStart,1);while(game.Busy&&!game.Finished)yield return null;Check(iced!=null&&!iced.Iced&&Vector3.Distance(iced.transform.position,iceStart)<.01f,"Real matching ball cracks ice while protected pin stays fixed");
            game.LoadLevel(6);yield return null;var band=game.Pins.FindAll(p=>p.Banded);game.PrepareShotColor(band[0].ColorId);band[0].BreakProtection(band[0].ColorId);Check(band.Count==3&&band.TrueForAll(p=>!p.Banded&&p.IsLocked),"Band breaks across the three protected targets");game.PrepareShotColor(band[0].ColorId);Check(band.TrueForAll(p=>!p.IsLocked),"Following shot frees entire bound group");
            game.LoadLevel(8);yield return null;var gate=GameObject.Find("Moving festival gate");float x=gate.transform.position.x;yield return new WaitForSeconds(.6f);Check(Mathf.Abs(gate.transform.position.x-x)>.2f,"Moving barrier has actual physical motion");Capture("adventure-unity-obstacles.png");
            game.LoadLevel(0);yield return null;int moves=game.Moves;Check(game.Adventure.Use(0,2)&&game.Queue[0]==2&&game.Moves==moves,"Color chooser changes ready ball without spending move");Check(game.Adventure.Use(3)&&game.Moves==moves+3&&!game.VideoRescueUsed&&game.Queue.Count>=game.Moves+1,"Extra moves preserve video entitlement and fill queue");Check(game.Adventure.Use(1)&&!game.Adventure.Use(1),"Rainbow arms only once");game.Shoot(new Vector3(0,.4f,0));yield return null;Check(FindFirstObjectByType<BowlingBall>().Rainbow&&game.Pins.TrueForAll(p=>!p.IsLocked),"Rainbow shot activates all ordinary colors");while(game.Busy&&!game.Finished)yield return null;
            game.LoadLevel(0);yield return null;var item=game.MatchTray.Collect(2,Vector2.one*.5f);game.MatchTray.Tick(.5f);Check(game.Adventure.Use(2,-1,item.Id)&&game.Adventure.PocketColor==2&&game.MatchTray.LiveCount==0,"Selected arrived token enters pocket");game.MatchTray.Collect(2,Vector2.one*.5f);game.MatchTray.Collect(2,Vector2.one*.5f);Check(game.Adventure.PocketColor==-1&&game.MatchTray.LiveCount==0&&game.MatchTray.Items.Count==3,"Two mates restore pocket and reserve triple");game.MatchTray.Tick(.5f);game.MatchTray.Tick(.14f);game.MatchTray.Tick(.25f);Check(game.MatchTray.Empty&&game.MatchTray.MatchCount==1,"Pocket triple finishes animation");
            game.GrantCoins(100);int before=game.Coins,quantity=game.Adventure.Data.stock[1];Check(game.Adventure.Buy(1)&&game.Coins==before-35&&game.Adventure.Data.stock[1]==quantity+1,"Inventory purchase charges exactly 35 coins");game.Adventure.Reload();Check(game.Adventure.Data.stock[1]==quantity+1,"Booster stock survives save/reload");
            var save=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(PinPopEconomy.SaveKey));save.highestLevel=5;PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(save));game.ReloadEconomy();before=game.Coins;Check(game.Adventure.ClaimChest(0)&&game.Coins==before+25&&!game.Adventure.ClaimChest(0)&&!game.Adventure.ClaimChest(1),"Chest pays once after milestone and locked chest rejects claim");game.Adventure.Reload();Check((game.Adventure.Data.chests&1)!=0,"Claimed chest persists");
            game.LoadLevel(4);yield return null;Check(game.Adventure.Lesson().Length>0,"New color tutorial exists");Check(game.Adventure.Lesson().Contains("BUZLU"),"First obstacle tutorial follows color introduction");Check(game.Adventure.Lesson()=="","Seen tutorial does not repeat");
            foreach(var size in new[]{new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);ui.ShowMap();yield return new WaitForSecondsRealtime(.2f);var rt=GameObject.Find("Adventure card").GetComponent<RectTransform>();var corners=new Vector3[4];rt.GetWorldCorners(corners);var low=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[0]);var high=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[2]);Check(low.x>=-1&&high.x<=Screen.width+1&&low.y>=-1&&high.y<=Screen.height+1,"Map card fits "+size.x);Capture("adventure-unity-map-"+size.x+".png",Screen.width,Screen.height);ui.Close();}
            Screen.SetResolution(600,1000,false);game.LoadLevel(4);yield return new WaitForSecondsRealtime(.2f);ui.ShowBooster(0);yield return new WaitForSecondsRealtime(.2f);Capture("adventure-unity-booster.png");ui.Close();
        }
        IEnumerator EightColorChecks(){
            Screen.SetResolution(600,1000,false);game.SetMenu(false);game.Hud.ShowGame();
            for(int level=0;level<32;level++){
                var layout=PinPopLayouts.Build(level);int count=PinPopLayouts.ColorCount(level);bool valid=count==Mathf.Min(8,3+level/2)&&layout.Count==PinPopEconomy.MoveBudget(level);
                for(int c=0;c<count;c++){int colorTotal=layout.FindAll(p=>p.Color==c).Count;valid&=colorTotal>0&&colorTotal%3==0;}
                for(int i=0;i<layout.Count+4;i++)valid&=PinPopLayouts.QueueColor(level,i)>=0&&PinPopLayouts.QueueColor(level,i)<count;
                Check(valid,"Chapter "+(level+1)+" has unlocked colors, complete triples and valid ammo");
            }
            for(int c=0;c<8;c++){
                Check(Resources.Load<Texture2D>("PinSkins/pin-"+c)!=null&&Resources.Load<Sprite>("PinIcons/pin-"+c)!=null,"Coat and tray sprite exist for "+PinPopPalette.Characters[c]);
                bool icons=true;foreach(int style in new[]{0,1,2,3})icons&=BallLooks.Icon(style,c)!=null;
                Check(icons,"Ball previews support color "+c);
            }
            foreach(int level in new[]{0,2,4,6,8,10,11}){
                game.LoadLevel(level);yield return new WaitForSecondsRealtime(.25f);
                Check(game.Pins.Count==PinPopLayouts.TargetCount(level)&&game.Queue.TrueForAll(c=>c<PinPopLayouts.ColorCount(level)),"Runtime board and queue at chapter "+(level+1));
                yield return new WaitForEndOfFrame();Capture("eight-color-level-"+(level+1)+".png");
            }
            game.LoadLevel(10);yield return null;
            for(int c=0;c<8;c++){
                var pin=game.Pins.Find(p=>!p.Cleared&&p.ColorId==c);while(pin.Iced||pin.Banded)pin.BreakProtection(c);game.PrepareShotColor(c);pin.Body.position=new Vector3(6+c*.7f,1.5f,0);pin.Body.WakeUp();float start=Time.realtimeSinceStartup;while(game.Cleared<c+1&&Time.realtimeSinceStartup-start<3)yield return null;
                Check(game.Cleared==c+1,"Physical falling collection preserves color "+c);
            }
            yield return new WaitForSecondsRealtime(.65f);Check(game.MatchTray.LiveCount==8&&game.MatchTray.Items.Count==8,"Eight distinct figures coexist without false triples");Capture("eight-color-collected.png");
            for(int c=0;c<2;c++){
                var pin=game.Pins.Find(p=>!p.Cleared&&p.ColorId==c);while(pin.Iced||pin.Banded)pin.BreakProtection(c);game.PrepareShotColor(c);pin.Body.position=new Vector3(6+c,1.5f,0);pin.Body.WakeUp();float start=Time.realtimeSinceStartup;while(game.Cleared<9+c&&Time.realtimeSinceStartup-start<3)yield return null;
            }
            yield return new WaitForSecondsRealtime(.7f);Check(game.TrayOverflow&&game.Finished&&!game.Won,"Ten unmatched figures trigger capacity loss");
            game.LoadLevel(10);yield return null;game.PrepareShotColor(7);int n=0;foreach(var pin in game.Pins.FindAll(p=>p.ColorId==7)){if(n>=3)break;pin.Body.position=new Vector3(6+n*.8f,1.5f,0);pin.Body.WakeUp();n++;}
            float began=Time.realtimeSinceStartup;while(game.MatchTray.MatchCount<1&&Time.realtimeSinceStartup-began<4)yield return null;yield return new WaitForSecondsRealtime(.4f);
            Check(game.MatchTray.MatchCount==1&&game.MatchTray.Empty&&game.Cleared==3,"Eighth-color triple merges and removes exactly three");
        }
        IEnumerator SmashMatchChecks(){
            Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.3f);
            Check(Physics.gravity.y==-20&&GameObject.Find("Meadow physics")==null&&GameObject.Find("Rail physics")==null,"Reference gravity and open platform edges have no extra catching floor");
            for(int level=0;level<12;level++){var layout=PinPopLayouts.Build(level);bool triples=true;for(int color=0;color<3;color++)triples &= layout.FindAll(x=>x.Color==color).Count%3==0;Check(triples&&layout.Count==PinPopEconomy.MoveBudget(level),"Chapter "+(level+1)+" has complete color triples and ammo per target");}
            var holder=new GameObject("Inactive tray contract fixture");holder.SetActive(false);var tray=holder.AddComponent<PinPopMatchTray>();tray.Build(game);foreach(int c in new[]{0,1,0,2,0})tray.Collect(c,Vector2.one*.5f);
            Check(tray.Items.ConvertAll(x=>x.Color).ToArray()[2]==0&&tray.Items[3].Color==1&&tray.Items[4].Color==2&&tray.LiveCount==2,"Same colors insert together and reserved triples stop occupying capacity");tray.Tick(.47f);Check(tray.MatchCount==0&&!tray.Items[0].Arrived,"Reference flights wait 480 ms");tray.Tick(.02f);tray.Tick(.28f);Check(tray.MatchCount==0,"Convergence remains visible for 320 ms after arrival");tray.Tick(.03f);Check(tray.MatchCount==1&&tray.Items.Count==5,"Burst precedes removal and starts the combo");tray.Tick(.44f);Check(tray.Items.Count==2&&tray.Items[0].Color==1&&tray.Items[1].Color==2,"Matched triple removes after 440 ms and survivors close the gap");tray.Clear();for(int j=0;j<10;j++)tray.Collect(j/2,Vector2.zero);Check(tray.LiveCount==10,"Ten unmatched objects reach the reference tray limit");Destroy(holder);
            var tilted=game.Pins[0];tilted.Body.rotation=Quaternion.Euler(0,0,90);tilted.Body.position=new Vector3(-2.45f,.35f,0);tilted.Body.Sleep();yield return new WaitForSecondsRealtime(.2f);Check(!tilted.Cleared&&game.Cleared==0,"A target lying on the table does not collect merely because it tilted");
            game.LoadLevel(0);yield return null;game.PrepareShotColor(0);for(int j=0;j<3;j++){var pin=game.Pins[j];pin.Body.position=new Vector3(-5-j*.7f,1.5f,0);pin.Body.WakeUp();}
            yield return new WaitForSecondsRealtime(.12f);Check(game.Cleared==0,"Passing a horizontal table edge is not enough before the vertical fall");float began=Time.realtimeSinceStartup;while(game.Cleared<3&&Time.realtimeSinceStartup-began<3)yield return null;Check(game.Cleared==3&&game.MatchTray.Items.Count==3&&game.MatchTray.LiveCount==0,"Real gravity crossing -0.8 collects exactly three and reserves their match");
            float age=game.MatchTray.Items[0].Age;Time.timeScale=0;yield return new WaitForSecondsRealtime(.2f);Check(Mathf.Abs(game.MatchTray.Items[0].Age-age)<.001f,"Pause also freezes collection flights");Time.timeScale=1;yield return new WaitForEndOfFrame();Capture("smash-match-unity-flights.png");
            began=Time.realtimeSinceStartup;while(game.MatchTray.MatchCount==0&&Time.realtimeSinceStartup-began<3)yield return null;Check(game.MatchTray.MatchCount==1,"Real fallen targets trigger the triple-match burst");yield return new WaitForEndOfFrame();Capture("smash-match-unity-merge.png");yield return new WaitForSecondsRealtime(PinPopMatchTray.BlastDuration+.06f);Check(game.MatchTray.Empty&&game.Cleared==3&&!game.Finished,"Collection slots empty after merging while the remaining board persists");Capture("smash-match-unity-after-match.png");
            for(int color=1;color<3;color++){game.PrepareShotColor(color);var pin=game.Pins.Find(x=>x!=null&&!x.Cleared&&x.ColorId==color);pin.Body.position=new Vector3(5,1.5f,color);pin.Body.WakeUp();began=Time.realtimeSinceStartup;while(game.Cleared<3+color&&Time.realtimeSinceStartup-began<3)yield return null;yield return new WaitForSecondsRealtime(.55f);}
            Check(game.MatchTray.LiveCount==2,"Two differently colored fallen pieces remain waiting in the tray");var ids=game.MatchTray.Items.ConvertAll(x=>x.Id).ToArray();typeof(PinPopGame).GetProperty("Moves").SetValue(game,0);game.Queue.Clear();game.Queue.Add(0);typeof(PinPopGame).GetMethod("End",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(game,new object[]{false});yield return new WaitForSecondsRealtime(.3f);Click("WATCH_VIDEO");began=Time.realtimeSinceStartup;while(game.RewardedVideo.Busy&&Time.realtimeSinceStartup-began<16)yield return null;
            Check(game.Moves==3&&!game.Finished&&game.MatchTray.Items.Count==2&&game.MatchTray.Items[0].Id==ids[0]&&game.MatchTray.Items[1].Id==ids[1],"A real completed video preserves collected pieces and grants three more shots");int dropIndex=0;foreach(var remaining in game.Pins)if(remaining!=null&&!remaining.Cleared){remaining.Body.isKinematic=false;remaining.Body.position=new Vector3(5+(dropIndex++)*.8f,1.5f,remaining.ColorId);remaining.Body.linearVelocity=remaining.Body.angularVelocity=Vector3.zero;remaining.Body.WakeUp();}began=Time.realtimeSinceStartup;while(game.Cleared<game.Total&&Time.realtimeSinceStartup-began<3)yield return null;Debug.Log("SMASH LAST FALL: "+game.Cleared+"/"+game.Total+" tray="+game.MatchTray.Items.Count+" finished="+game.Finished);Check(game.Cleared==game.Total&&!game.Finished&&!game.MatchTray.Empty,"Last physical fall does not win before the final merge finishes");began=Time.realtimeSinceStartup;while(!game.Finished&&Time.realtimeSinceStartup-began<5)yield return null;Check(game.Won&&game.MatchTray.Empty&&game.MatchTray.MatchCount==4&&game.LastReward==30&&game.LastMoveReward==0,"Final triple completes victory and preserves rescued-win coin rules");yield return new WaitForSecondsRealtime(.3f);Capture("smash-match-unity-victory.png");game.LoadLevel(1);Check(game.MatchTray.Empty&&game.MatchTray.MatchCount==0,"Restart/new level cancels every old flight, match and collected tile");
            game.LoadLevel(0);yield return null;var pink=game.Pins.Find(x=>x.ColorId==0);Vector3 before=pink.transform.position;game.Shoot(new Vector3(-2.45f,.4f,0));while(game.Busy&&!game.Finished)yield return null;Check(game.Cleared>0,"A real bowling impact physically pushes targets off the table");Check(!game.MatchTray.Pending&&game.MatchTray.LiveCount+game.MatchTray.MatchCount*3==game.Cleared,"Every naturally fallen target is represented by a waiting tile or a completed triple");
            foreach(var size in new[]{new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);var cell=GameObject.Find("Collection slot 9").GetComponent<RectTransform>();var corners=new Vector3[4];cell.GetWorldCorners(corners);Vector2 min=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[0]),max=RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,corners[2]);Check(min.x>=0&&max.x<=Screen.width+1&&min.y>=0&&max.y<=Screen.height+1,"All ten collection cells fit at "+size.x+"x"+size.y);Check(Mathf.Abs(cell.rect.width-cell.rect.height)<.1f,"Collection tiles stay square at "+size.x+"x"+size.y);var trayCorners=new Vector3[4];var titleCorners=new Vector3[4];GameObject.Find("Collection tray").GetComponent<RectTransform>().GetWorldCorners(trayCorners);GameObject.Find("Next title").GetComponent<RectTransform>().GetWorldCorners(titleCorners);Check(trayCorners[0].y>=titleCorners[1].y-1,"Collection dock leaves upcoming balls visible at "+size.x+"x"+size.y);Capture("smash-match-unity-"+size.x+".png",Screen.width,Screen.height);}
        }
        IEnumerator RecoveryChecks()
        {
            Screen.SetResolution(600,1000,false);game.LoadLevel(0);game.SetMenu(false);game.Hud.ShowGame();yield return new WaitForSecondsRealtime(.4f);
            Check(GameObject.Find("SWAP")!=null&&GameObject.Find("SWAP_PLUS")==null,"Swap replenishment shares the compact main button");
            game.Hud.ShowSettings();Check(GameObject.Find("HELP_SETTINGS")!=null,"How-to replay is available in settings");game.Hud.CloseOverlays();Check(!game.ClaimVideoMoves(game.AttemptId),"Video cannot grant moves to an unfinished game");
            game.Shoot(new Vector3(-2.45f,.4f,-.2f),1);while(game.Busy)yield return null;Check(game.Cleared>0&&game.Cleared<game.Total,"A real matching shot partially clears the table");
            // Fixture reaches the final-move boundary without five redundant misses.
            typeof(PinPopGame).GetProperty("Moves").SetValue(game,1);if(game.Queue.Count>2)game.Queue.RemoveRange(2,game.Queue.Count-2);game.Shoot(new Vector3(4,.4f,-2.6f),1);while(!game.Finished)yield return null;
            Check(!game.Won&&game.Moves==0&&game.CanWatchForMoves&&game.Paused,"Exhaustion offers video rescue on the frozen failed board");yield return new WaitForSecondsRealtime(.3f);Capture("recovery-unity-offer.png");int cleared=game.Cleared,coins=game.Coins,credits=game.SwapCredits,attempt=game.AttemptId;var positions=game.Pins.ConvertAll(pin=>pin==null?Vector3.zero:pin.transform.position);var queue=game.Queue.ToArray();
            Click("WATCH_VIDEO");yield return new WaitForSecondsRealtime(1.5f);Check(game.RewardedVideo.Busy&&game.Moves==0,"Starting video does not award early");var videoTexture=GameObject.Find("Video picture").GetComponent<UnityEngine.UI.RawImage>().texture;Check(videoTexture!=null&&videoTexture.width>=360&&HasVisiblePixels(videoTexture),"Video decoder produces visible pixels in the actual fullscreen UI");yield return new WaitForEndOfFrame();var screen=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,"recovery-unity-video.png"),screen.EncodeToPNG());Destroy(screen);Screen.SetResolution(320,740,false);yield return new WaitForSecondsRealtime(.35f);Canvas.ForceUpdateCanvases();var cancelButton=GameObject.Find("Cancel rewarded video").GetComponent<RectTransform>();var videoCanvas=cancelButton.GetComponentInParent<Canvas>();bool readable=true;foreach(var text in videoCanvas.GetComponentsInChildren<UnityEngine.UI.Text>())readable&=text.fontSize*videoCanvas.scaleFactor>=11.9f;Check(readable&&cancelButton.rect.height*videoCanvas.scaleFactor>=43.9f,"Narrow video UI preserves twelve-pixel text and a 44-pixel cancel target");yield return new WaitForEndOfFrame();screen=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,"recovery-unity-video-320.png"),screen.EncodeToPNG());Destroy(screen);Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.25f);Click("Cancel rewarded video");yield return null;Check(game.Finished&&game.Moves==0&&!game.VideoRescueUsed&&game.CanWatchForMoves,"Cancelling the video grants nothing and preserves retry eligibility");
            Click("WATCH_VIDEO");UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(GameObject.Find("Cancel rewarded video"));float started=Time.realtimeSinceStartup;while(game.RewardedVideo.Busy&&Time.realtimeSinceStartup-started<16)yield return null;
            Check(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject==null,"Video completion returns keyboard focus to gameplay");Check(!game.RewardedVideo.Busy&&!game.Finished&&game.Moves==3&&game.VideoRescueUsed,"Actual video completion grants exactly three moves once");Check(Time.realtimeSinceStartup-started>=7.2f,"Reward waits for the full eight-second media playback");bool unchanged=game.Cleared==cleared;for(int i=0;i<game.Pins.Count;i++)if(game.Pins[i]!=null)unchanged&=Vector3.Distance(positions[i],game.Pins[i].transform.position)<.001f;Check(unchanged,"Video recovery preserves pin poses and all previous clears");Check(game.Coins==coins&&game.SwapCredits==credits&&game.Queue.Count==4&&game.Queue[0]==queue[0],"Recovery preserves wallet and queue prefix while adding the extra shots");Check(!game.ClaimVideoMoves(attempt),"Duplicate reward cannot grant a second set");Capture("recovery-unity-resumed.png");
            int guard=0;while(!game.Finished&&game.Moves>0&&guard++<4){var matching=game.Pins.FindAll(pin=>pin!=null&&!pin.Cleared&&pin.ColorId==game.Queue[0]);if(matching.Count==0&&game.SwapCredits>0){game.Swap();guard--;continue;}matching.Sort((a,b)=>a.transform.position.z.CompareTo(b.transform.position.z));float x=0,z=matching.Count>0?matching[0].transform.position.z:0;int n=0;foreach(var pin in matching)if(pin.transform.position.z<z+.3f){x+=pin.transform.position.x;n++;}game.Shoot(new Vector3(n>0?x/n:4,.4f,z),1);while(game.Busy&&!game.Finished)yield return null;}
            Check(game.Won&&game.LastReward==30&&game.LastMoveReward==0,"A rescued win pays its base reward without farming extra-move bonus");yield return new WaitForSecondsRealtime(.5f);Capture("recovery-unity-rescued-win.png");
            game.LoadLevel(1);Check(!game.VideoRescueUsed&&!game.ClaimVideoMoves(attempt),"New attempt resets eligibility but rejects stale callback");Check(GameObject.Find("HELP_GAME")==null,"Help remains absent after chapter one");typeof(PinPopGame).GetProperty("Moves").SetValue(game,1);if(game.Queue.Count>2)game.Queue.RemoveRange(2,game.Queue.Count-2);game.Shoot(new Vector3(4,.4f,-2.6f),1);while(!game.Finished)yield return null;
            foreach(var size in new[]{new Vector2Int(320,740),new Vector2Int(390,844),new Vector2Int(1000,600)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);Canvas.ForceUpdateCanvases();var body=ActiveText("resultBody");Check(body.preferredHeight<=body.rectTransform.rect.height+1&&GameObject.Find("WATCH_VIDEO").activeInHierarchy,"Recovery offer remains readable at "+size.x+"x"+size.y);Capture("recovery-unity-offer-"+size.x+".png",Screen.width,Screen.height);}
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);Click("WATCH_VIDEO");started=Time.realtimeSinceStartup;while(game.RewardedVideo.Busy&&Time.realtimeSinceStartup-started<16)yield return null;Check(game.Moves==3&&game.VideoRescueUsed,"New attempt earns a fresh one-time video recovery");
            for(int i=0;i<3;i++){game.Shoot(new Vector3(4,.4f,-2.6f),1);while(game.Busy&&!game.Finished)yield return null;}
            Check(game.Finished&&!game.Won&&!game.CanWatchForMoves&&GameObject.Find("WATCH_VIDEO")==null,"Exhausting extra moves hides a second video offer");yield return new WaitForSecondsRealtime(.3f);Capture("recovery-unity-used-offer.png");Click("CONTINUE");yield return new WaitForSecondsRealtime(1.65f);Check(game.Moves==PinPopEconomy.MoveBudget(game.Level)&&!game.VideoRescueUsed&&!game.Finished,"Retry returns to a fresh chapter with normal moves");
        }
        IEnumerator EconomyChecks()
        {
            var e=new PinPopEconomy(new EconomySave{coins=0,swaps=3});
            Check(e.Data.coins==0&&e.Data.swaps==3&&e.Owns(0),"Fresh wallet includes classic and three credits");
            Check(e.Complete(0,3,out int basePaid,out int bonusPaid)==36&&basePaid==30&&bonusPaid==6&&e.Data.coins==36,"First clear with three moves pays exactly 36");
            Check(e.Complete(0,3,out _,out _)==0&&e.Data.coins==36,"Same result cannot pay twice");Check(e.Complete(0,5,out basePaid,out bonusPaid)==4&&basePaid==0&&bonusPaid==4&&e.Data.coins==40,"Improved result pays only its four-coin difference");Check(e.Complete(0,2,out _,out _)==0&&e.Data.coins==40,"Worse replay never pays again");
            Check(e.Complete(1,2,out _,out _)==34&&e.Data.coins==74,"New chapter pays 30 plus four bonus coins");Check(e.BuySwap()&&e.Data.coins==64&&e.Data.swaps==4,"One credit costs exactly ten coins");Check(e.UseSwap()&&e.Data.swaps==3&&e.Data.coins==64,"Using a credit does not spend coins again");Check(!e.BuyBall(1)&&e.Data.coins==64,"Unaffordable ball is not purchased");
            Check(e.Complete(2,8,out _,out _)==46&&e.Data.coins==110,"Eight saved moves pay 46 coins");Check(e.BuyBall(1)&&e.Data.coins==20&&e.Owns(1)&&e.Data.selectedBall==1,"Marble costs ninety and equips permanently");Check(!e.BuyBall(1)&&e.Data.coins==20,"Owned ball cannot be charged again");Check(e.Equip(0)&&!e.Equip(2)&&e.Data.coins==20,"Only owned balls equip and equipment is free");
            var reloaded=new PinPopEconomy(JsonUtility.FromJson<EconomySave>(JsonUtility.ToJson(e.Data)));Check(reloaded.Owns(1)&&reloaded.Data.coins==20&&reloaded.Complete(0,5,out _,out _)==0,"Serialized ownership and claim history survive reload");
            e=new PinPopEconomy(new EconomySave{coins=1000});Check(e.BuyBall(1)&&e.BuyBall(2)&&e.BuyBall(3)&&e.Data.coins==430&&e.Data.ownedMask==15,"Full paid collection costs 570 coins");foreach(int id in new[]{-1,4,int.MaxValue})Check(!e.BuyBall(id)&&e.Data.coins==430,"Invalid product cannot alter the wallet: "+id);
            e=new PinPopEconomy(new EconomySave{coins=999999994,swaps=999});Check(e.Complete(0,3,out _,out _)==5&&e.Data.coins==999999999,"Coin cap credits only available capacity");Check(!e.BuySwap()&&e.Data.coins==999999999&&e.Data.swaps==999,"Credit cap rejects before charging");e.Data.swaps=998;Check(e.BuySwap()&&e.Data.coins==999999989&&e.Data.swaps==999,"Last available credit purchases exactly");Check(e.Complete(0,3,out _,out _)==0,"Spending after capped reward never recreates a claim");Check(PinPopEconomy.Compact(999999999)=="999,9M","Large coin display rounds down without inflating the balance");
            e=new PinPopEconomy(new EconomySave{coins=-5,swaps=-1,ownedMask=-1,selectedBall=3});Check(e.Data.coins==0&&e.Data.swaps==0&&e.Data.ownedMask==1&&e.Data.selectedBall==0,"Invalid negative save cannot grant currency or locked balls");
            PlayerPrefs.DeleteKey(PinPopEconomy.SaveKey);PlayerPrefs.SetInt("Coins",47);PlayerPrefs.SetInt("SwapCredits",2);PlayerPrefs.SetInt("Level",3);e=PinPopEconomy.Load();Check(e.Data.coins==47&&e.Data.swaps==2&&e.Data.highestLevel==3,"Legacy migration preserves 47 coins and two credits");Check(e.Complete(0,3,out _,out _)==0&&e.Data.coins==47,"Legacy paid chapters cannot be farmed again");Check(e.Complete(3,0,out _,out _)==30&&e.Data.coins==77,"First new migrated chapter earns normally");
            PlayerPrefs.DeleteKey(PinPopEconomy.SaveKey);PlayerPrefs.SetInt("Level",0);PlayerPrefs.SetInt("Coins",0);PlayerPrefs.SetInt("SwapCredits",3);game.ReloadEconomy();game.LoadLevel(0);game.SetMenu(true);game.Hud.ShowHome();Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.4f);
            Check(GameObject.Find("BUY_SWAP")==null,"Old upper buy-swap banner is absent");Click("HOME_SHOP");yield return new WaitForSecondsRealtime(.25f);Check(game.Hud.ShopVisible&&ActiveText("Catalog title").text=="TOP MAĞAZASI","Home store opens the bowling ball catalog");Check(!GameObject.Find("BALL_BUY_1").GetComponent<UnityEngine.UI.Button>().interactable,"Insufficient balance disables paid balls");Capture("economy-unity-empty-catalog.png");Click("CLOSE_SHOP");
            Click("PLAY");yield return new WaitForSecondsRealtime(1.65f);int turns=0;
            while(!game.Finished&&game.Moves>0&&turns<6){var matching=game.Pins.FindAll(p=>p!=null&&!p.Cleared&&p.ColorId==game.Queue[0]);if(matching.Count==0&&game.SwapCredits>0){Click("SWAP");continue;}matching.Sort((a,b)=>a.transform.position.z.CompareTo(b.transform.position.z));float z=matching.Count>0?matching[0].transform.position.z:.2f,x=0;int n=0;foreach(var pin in matching)if(pin.transform.position.z<z+.3f){x+=pin.transform.position.x;n++;}x=n>0?x/n:0;float desired=Mathf.Clamp(-x/.045f,-110,110),dx=desired*140/Mathf.Sqrt(19600-desired*desired);Vector2 anchor=game.Camera.WorldToScreenPoint(PinPopGame.Launch);game.BeginPullForVerification(anchor);game.UpdatePull(anchor+new Vector2(dx*game.Camera.pixelWidth/600,-140*game.Camera.pixelHeight/1000));game.PointerUp();turns++;while(game.Busy&&!game.Finished)yield return null;}
            int earned=30+2*game.Moves;Check(game.Won&&game.Coins==earned&&game.LastBaseReward==30&&game.LastMoveReward==2*game.Moves,"Real win credits the displayed base and saved-move bonus");int afterWin=game.Coins;typeof(PinPopGame).GetMethod("End",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(game,new object[]{true});Check(game.Coins==afterWin,"Duplicate completion callback never pays twice");yield return new WaitForSecondsRealtime(2f);Capture("economy-unity-reward.png");Click("CONTINUE");yield return new WaitForSecondsRealtime(1.65f);
            var depleted=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(PinPopEconomy.SaveKey));depleted.swaps=0;PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(depleted));game.ReloadEconomy();game.Hud.Refresh();int moves=game.Moves,first=game.Queue[0],credits=game.SwapCredits;Click("SWAP");yield return new WaitForSecondsRealtime(.2f);Check(game.Hud.SwapShopVisible&&game.Paused&&game.Moves==moves&&game.Queue[0]==first,"Plus opens a paused purchase sheet without using swap");Capture("economy-unity-swap-sheet.png");Click("SWAP_BUY_CONFIRM");Check(!game.Hud.SwapShopVisible&&!game.Paused&&game.Coins==afterWin-10&&game.SwapCredits==credits+1&&game.Moves==moves&&game.Queue[0]==first,"Confirmed purchase adds one credit, deducts ten, and consumes no move");Click("SWAP");Check(game.SwapCredits==credits&&game.Coins==afterWin-10&&game.Moves==moves&&game.Queue[game.Queue.Count-1]==first,"Main swap button still rotates separately and consumes only a credit");
            var funded=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(PinPopEconomy.SaveKey));funded.coins=600;PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(funded));game.ReloadEconomy();game.Hud.Refresh();Click("GAME_SHOP");yield return new WaitForSecondsRealtime(.25f);
            Click("BALL_BUY_1");Check(game.Coins==510&&game.SelectedBall==1&&game.OwnsBall(1),"Live catalog buys and equips marble for 90");Click("BALL_BUY_3");Check(game.Coins==210&&game.SelectedBall==3&&game.OwnsBall(3),"Live catalog buys galaxy for 300");Click("BALL_BUY_2");Check(game.Coins==30&&game.SelectedBall==2&&game.OwnsBall(2),"Live catalog buys orbit for 180");Check(!game.BuyBall(2)&&game.Coins==30,"Duplicate UI purchase is rejected by the model");Click("BALL_BUY_0");Check(game.SelectedBall==0&&game.Coins==30,"Owned classic equips for free");Click("BALL_BUY_3");Check(game.SelectedBall==3&&game.Coins==30,"Owned galaxy equips without a second charge");Capture("economy-unity-owned-catalog.png");
            foreach(var size in new[]{new Vector2Int(390,844),new Vector2Int(320,740),new Vector2Int(1000,600)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.3f);Canvas.ForceUpdateCanvases();bool fits=true;for(int i=0;i<4;i++){var name=ActiveText("Ball name "+i);var price=ActiveText("Ball price "+i);fits &= name.preferredHeight<=name.rectTransform.rect.height+1&&price.preferredHeight<=price.rectTransform.rect.height+1;}Check(fits&&game.Hud.ShopVisible,"All four catalog cards remain readable at "+size.x+"x"+size.y);Capture("economy-unity-catalog-"+size.x+".png",Screen.width,Screen.height);}
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.3f);Click("CLOSE_SHOP");game.ReloadEconomy();game.LoadLevel(1);Check(game.SelectedBall==3&&game.Coins==30&&game.OwnsBall(1)&&game.OwnsBall(2),"Restart/reload preserves equipped and owned balls");var ready=GameObject.Find("Next bowling ball").GetComponentInChildren<MeshRenderer>();Check(ready.sharedMaterial.name.StartsWith("Bowling look 3")&&ready.sharedMaterial.mainTexture!=null,"Equipped galaxy reaches the actual ready 3D material");Check(GameObject.Find("Next bowling ball preview").GetComponent<UnityEngine.UI.Image>().sprite.name.StartsWith("look-3"),"Next-ball previews show the equipped appearance");
            int color=game.Queue[0];var velocity=PinPopGame.ShotVelocity(new Vector3(-2.45f,.4f,.2f),.7f,PinPopGame.Launch);game.Shoot(new Vector3(-2.45f,.4f,.2f),.7f);var ball=FindFirstObjectByType<BowlingBall>();Check(ball.ColorId==color&&ball.GetComponent<Rigidbody>().mass==6&&Vector3.Distance(ball.GetComponent<Rigidbody>().linearVelocity,velocity)<.001f&&ball.GetComponentInChildren<MeshRenderer>().sharedMaterial.name.StartsWith("Bowling look 3"),"Purchased appearance reaches a real shot without changing physics or color");yield return new WaitForSecondsRealtime(.25f);Capture("economy-unity-styled-shot.png");while(game.Busy&&!game.Finished)yield return null;
            var noMoney=JsonUtility.FromJson<EconomySave>(PlayerPrefs.GetString(PinPopEconomy.SaveKey));noMoney.coins=0;noMoney.swaps=0;PlayerPrefs.SetString(PinPopEconomy.SaveKey,JsonUtility.ToJson(noMoney));game.ReloadEconomy();game.Hud.Refresh();Click("SWAP");yield return new WaitForSecondsRealtime(.2f);Check(!GameObject.Find("SWAP_BUY_CONFIRM").GetComponent<UnityEngine.UI.Button>().interactable&&!game.BuySwap()&&game.Coins==0,"Empty wallet cannot buy a credit or become negative");Click("SWAP_BUY_CANCEL");Check(game.Coins==0&&game.SelectedBall==3,"Cancelling credit purchase preserves wallet and selected ball");
        }
        void OnRuntimeLog(string condition,string stack,LogType type){if(type!=LogType.Exception||!walletBackedUp)return;RestoreWallet();if(!string.IsNullOrEmpty(folder))File.WriteAllText(Path.Combine(folder,"runtime-exception.txt"),condition+"\n"+stack);Application.Quit(1);}
        void OnApplicationQuit(){if(walletBackedUp)RestoreWallet();}
        void RestoreWallet()
        {
            if(!walletBackedUp)return;walletBackedUp=false;
            if(hadTutorial)PlayerPrefs.SetInt(PinPopHUD.TutorialSeenKey,savedTutorial);else PlayerPrefs.DeleteKey(PinPopHUD.TutorialSeenKey);
            if(hadAdventure)PlayerPrefs.SetString(PinPopAdventure.SaveKey,savedAdventure);else PlayerPrefs.DeleteKey(PinPopAdventure.SaveKey);
            if(hadEconomySave)PlayerPrefs.SetString(PinPopEconomy.SaveKey,savedEconomySave);else PlayerPrefs.DeleteKey(PinPopEconomy.SaveKey);
            if (hadCoins) PlayerPrefs.SetInt("Coins", savedCoins); else PlayerPrefs.DeleteKey("Coins");
            if (hadSwaps) PlayerPrefs.SetInt("SwapCredits", savedSwaps); else PlayerPrefs.DeleteKey("SwapCredits");
            PlayerPrefs.SetInt("Level", savedLevel); PlayerPrefs.Save();
        }
        string report = "";
        void Click(string name)
        {
            var button = Array.Find(FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None), b => b.name == name && b.gameObject.activeInHierarchy);
            Check(button != null && button.interactable, "UI button is available: " + name);
            if (button == null || !button.interactable) return;
            Canvas.ForceUpdateCanvases();
            var targetCanvas=button.GetComponentInParent<Canvas>();var eventCamera=targetCanvas.renderMode==RenderMode.ScreenSpaceOverlay?null:targetCanvas.worldCamera;var evt = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current) { position = RectTransformUtility.WorldToScreenPoint(eventCamera, button.GetComponent<RectTransform>().TransformPoint(button.GetComponent<RectTransform>().rect.center)) };
            var hits = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(evt, hits);
            Check(hits.Exists(h => h.gameObject == button.gameObject), "Pointer reaches button: " + name);
            var hitPosition=button.transform.position;
            UnityEngine.EventSystems.ExecuteEvents.Execute(button.gameObject, evt, UnityEngine.EventSystems.ExecuteEvents.pointerDownHandler);
            var feedback=button.GetComponent<HtmlButtonFeedback>();if(feedback!=null)Check(Vector3.Distance(hitPosition,button.transform.position)<.001f&&feedback.Artwork!=null,"Press keeps the button hit target fixed: "+name);
            UnityEngine.EventSystems.ExecuteEvents.Execute(button.gameObject, evt, UnityEngine.EventSystems.ExecuteEvents.pointerUpHandler);
            UnityEngine.EventSystems.ExecuteEvents.Execute(button.gameObject, evt, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
        }
        IEnumerator VideoReviewChecks()
        {
            int motion=PlayerPrefs.GetInt("ReducedMotion",0);PlayerPrefs.SetInt("ReducedMotion",0);PlayerPrefs.SetInt("Coins",60);PlayerPrefs.SetInt("SwapCredits",3);game.ReloadEconomy();
            foreach(var size in new[]{new Vector2Int(540,960),new Vector2Int(1080,1920),new Vector2Int(1440,2560),new Vector2Int(2160,3840)}){var r=HtmlReferenceHUD.FitViewport(size.x,size.y);Check(Mathf.Abs(r.x)<.1f&&r.y==0&&Mathf.Abs(r.width-size.x)<.1f&&Mathf.Abs(r.height-size.y)<.1f,"9:16 viewport has no empty margins at "+size.x+"x"+size.y);}
            Screen.SetResolution(864,1536,false);yield return new WaitForSecondsRealtime(.8f);yield return new WaitForEndOfFrame();game.LoadLevel(12);game.SetMenu(true);game.Hud.ShowHome();yield return new WaitForSecondsRealtime(.3f);
            var viewport=game.Camera.pixelRect;Check(Screen.width==864&&Screen.height==1536&&Mathf.Abs(viewport.height-Screen.height)<1&&Mathf.Abs(viewport.width-Screen.width)<1,"Native 9:16 game view uses full screen height and width");var plaque=GameObject.Find("Plaque").transform;float trimFront=0;foreach(Transform sibling in plaque.parent){var filter=sibling.GetComponent<MeshFilter>();if(filter==null)continue;var bounds=filter.sharedMesh.bounds;if(bounds.size.y<.03f&&bounds.size.x>4)trimFront=Mathf.Min(trimFront,sibling.localPosition.z+bounds.min.z*sibling.localScale.z);}Check(plaque.localPosition.z<trimFront-.001f,"Chapter lettering stays in front of decorative cart boards");Capture("video-review-home.png",Screen.width,Screen.height);
            Click("HOME_SHOP");yield return new WaitForSecondsRealtime(.25f);Check(game.Hud.ShopVisible&&game.Paused,"Shop opens and pauses the game at recording resolution");Click("SHOP_BUY");yield return new WaitForSecondsRealtime(.08f);Capture("video-review-buy.png",Screen.width,Screen.height);Check(game.Coins==50&&game.SwapCredits==4,"Shop purchase still spends exactly 10 coins and grants one swap");Click("CLOSE_SHOP");
            Click("PLAY");yield return new WaitForSecondsRealtime(.15f);CheckGraphicRaycasts(FindFirstObjectByType<BowlingTransition>());Capture("video-review-transition.png",Screen.width,Screen.height);yield return new WaitForSecondsRealtime(1.6f);
            Check(game.Level==12&&game.Total==24&&game.Moves==9,"Recorded level 13 starts with its original 24 pins and 9 moves");Capture("video-review-game.png",Screen.width,Screen.height);
            Vector2 ball=game.Camera.WorldToScreenPoint(PinPopGame.Launch);game.BeginPullForVerification(ball);game.UpdatePull(ball+new Vector2(-Screen.width*.04f,-Screen.height*.11f));
            Check(GameObject.Find("Landing label")==null,"Rolling interaction has no falling-point label");
            var marker=GameObject.Find("Aim target");var screen=game.Camera.WorldToScreenPoint(marker.transform.position);var withMarker=WorldPixels();marker.SetActive(false);var withoutMarker=WorldPixels();marker.SetActive(true);int changed=0;for(int y=Mathf.Max(0,(int)screen.y-30);y<Mathf.Min(Screen.height,(int)screen.y+31);y++)for(int x=Mathf.Max(0,(int)screen.x-75);x<Mathf.Min(Screen.width,(int)screen.x+76);x++){int index=y*Screen.width+x;var a=withMarker[index];var b=withoutMarker[index];if(Mathf.Abs(a.r-b.r)+Mathf.Abs(a.g-b.g)+Mathf.Abs(a.b-b.b)>12)changed++;}Check(changed>25,"Aim marker renders visible pixels on the table");Capture("video-review-open-landing.png",Screen.width,Screen.height);
            game.CancelPull();game.BeginPullForVerification(ball);game.UpdatePull(ball+new Vector2(Screen.width*.10f,-Screen.height*.125f));Check(game.IsPulling&&game.PullPower>.9f&&game.AimVisible,"Full viewport pull keeps power and table guide aligned");Capture("video-review-aim.png",Screen.width,Screen.height);game.PointerUp();Check(game.Busy&&game.Moves==8,"Release at recording resolution launches exactly one shot");yield return new WaitForSecondsRealtime(.25f);
            game.Hud.ShowSettings();var flying=FindFirstObjectByType<BowlingBall>();var frozen=flying.transform.position;yield return new WaitForSecondsRealtime(.15f);Check(game.Paused&&Vector3.Distance(flying.transform.position,frozen)<.001f,"Settings pause the in-flight ball without losing the shot");Click("CLOSE_SETTINGS");yield return new WaitForSecondsRealtime(4.9f);
            Check(!game.Busy&&game.Cleared>0&&game.Pins.TrueForAll(p=>p==null||p.ColorId==1||!p.Cleared),"High-power rolling shot resolves and only clears matching cyan pins");
            Click("GAME_SHOP");yield return new WaitForSecondsRealtime(.2f);Click("SHOP_BUY");Check(game.Coins==40&&game.SwapCredits==5,"In-game shop remains clickable and preserves economy");Click("CLOSE_SHOP");int first=game.Queue[0],moves=game.Moves;Click("SWAP");Check(game.Queue[game.Queue.Count-1]==first&&game.SwapCredits==4&&game.Moves==moves,"Swap still rotates queue and spends a credit without a move");
            Click("SETTINGS");yield return new WaitForSecondsRealtime(.2f);Click("RESTART");yield return new WaitForSecondsRealtime(1.8f);Check(game.Level==12&&game.Moves==9&&game.Cleared==0&&game.Coins==40&&game.SwapCredits==4,"Restart from recorded settings resets the board and preserves purchases");
            Screen.SetResolution(972,1728,false);yield return new WaitForSecondsRealtime(.7f);yield return new WaitForEndOfFrame();viewport=game.Camera.pixelRect;Check(Mathf.Abs(viewport.height-Screen.height)<1&&Mathf.Abs(viewport.width-Screen.width)<1,"Resizing to a larger native 9:16 viewport preserves full framing");Click("SETTINGS");yield return new WaitForSecondsRealtime(.25f);Capture("video-review-settings.png",Screen.width,Screen.height);Click("CLOSE_SETTINGS");
            Screen.SetResolution(1000,600,false);yield return new WaitForSecondsRealtime(.4f);yield return new WaitForEndOfFrame();viewport=game.Camera.pixelRect;Check(Mathf.Abs(viewport.width-360)<1&&Mathf.Abs(viewport.height-600)<1&&Mathf.Abs(viewport.x-320)<1,"Landscape view keeps intentional centered portrait framing");Click("SETTINGS");yield return new WaitForSecondsRealtime(.2f);Click("CLOSE_SETTINGS");PlayerPrefs.SetInt("ReducedMotion",motion);
        }
        UnityEngine.UI.Text ActiveText(string name){return Array.Find(FindObjectsByType<UnityEngine.UI.Text>(FindObjectsSortMode.None),t=>t.name==name&&t.gameObject.activeInHierarchy);}
        IEnumerator SynchronizationChecks()
        {
            bool hadMuted=PlayerPrefs.HasKey("Muted");int muted=PlayerPrefs.GetInt("Muted",1),motion=PlayerPrefs.GetInt("ReducedMotion",0);
            PlayerPrefs.DeleteKey("Muted");PlayerPrefs.SetInt("ReducedMotion",0);game.ReloadSettings();
            Check(game.Muted,"Fresh sound preference matches HTML: muted by default");
            Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.4f);game.LoadLevel(0);game.SetMenu(true);game.Hud.ShowHome();yield return new WaitForSecondsRealtime(.2f);
            Click("HOME_SETTINGS");yield return new WaitForSecondsRealtime(.2f);Click("SOUND");yield return new WaitForSecondsRealtime(.03f);
            var soundButton=Array.Find(FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None),b=>b.name=="SOUND"&&b.gameObject.activeInHierarchy);
            Check(!game.Muted&&PlayerPrefs.GetInt("Muted")==0&&((UnityEngine.UI.Image)soundButton.targetGraphic).sprite.name.EndsWith("sound-on"),"Sound enabled state, sprite and saved preference agree");
            Check(game.GetComponent<AudioSource>().isPlaying,"Enabling sound plays HTML confirmation tone");Click("SOUND");
            Check(game.Muted&&PlayerPrefs.GetInt("Muted")==1&&!((UnityEngine.UI.Image)soundButton.targetGraphic).sprite.name.EndsWith("sound-on"),"Sound disabled state, sprite and saved preference agree");
            Click("MOTION");Check(game.ReducedMotion&&PlayerPrefs.GetInt("ReducedMotion")==1,"Reduced motion toggle is saved");Capture("sync-unity-settings.png");Click("CLOSE_SETTINGS");game.ReloadSettings();Check(game.Muted&&game.ReducedMotion,"Saved settings reload without resetting economy");
            game.SetMenu(false);game.Hud.ShowGame();game.LoadLevel(0);yield return new WaitForSecondsRealtime(.2f);
            PlayerPrefs.SetInt("ReducedMotion",0);game.AnimateEntrance();yield return new WaitForSecondsRealtime(.12f);Check(game.Pins.TrueForAll(p=>p.transform.localScale==Vector3.one)&&game.Pins[game.Pins.Count-1].Visual.localScale.x<.99f,"Staggered entrance animates visuals without changing physics collider scale");PlayerPrefs.SetInt("ReducedMotion",1);yield return new WaitForSecondsRealtime(.05f);Check(game.Pins.TrueForAll(p=>p.Visual.localScale==Vector3.one),"Reduced motion immediately finishes an in-progress pin entrance");
            int moves=game.Moves;Check(!game.Shoot(new Vector3(float.NaN,0,0))&&!game.ShootFrom(Vector3.forward,1,new Vector3(float.PositiveInfinity,0,0))&&game.Moves==moves,"Invalid shot input cannot consume a move or corrupt physics");
            var movesProperty=typeof(PinPopGame).GetProperty("Moves");movesProperty.SetValue(game,2);game.Hud.Refresh();Check(ActiveText("moves").color==Art.Hex("FFDB61"),"Last two moves use HTML warning color");movesProperty.SetValue(game,6);game.Hud.Refresh();
            Vector2 ball=game.Camera.WorldToScreenPoint(PinPopGame.Launch);game.BeginPullForVerification(ball);game.UpdatePull(ball+Vector2.down*110);Check(game.IsPulling,"Pull can start before focus loss");game.SendMessage("OnApplicationFocus",false);Check(!game.IsPulling&&game.Moves==6,"Losing application focus cancels pull without spending a move");
            Check(GameObject.Find("Meadow physics").GetComponent<Collider>()!=null,"HTML meadow surface participates in physics");
            Check(game.Pins.TrueForAll(p=>p.Hitboxes.Length==3&&p.Hitboxes[0] is MeshCollider&&p.Hitboxes[2] is SphereCollider),"Pin contact shapes match HTML compound body, neck and head");
            var pin=game.Pins[0];var wrong=game.Pins.Find(p=>p.ColorId!=pin.ColorId);bool ignored=true;foreach(var a in pin.Hitboxes)foreach(var b in wrong.Hitboxes)ignored&=Physics.GetIgnoreCollision(a,b);Check(ignored,"All compound collider pairs enforce pin color isolation");
            Capture("sync-unity-game.png");
            var end=typeof(PinPopGame).GetMethod("End",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);var frozenPin=game.Pins[0];frozenPin.Body.linearVelocity=Vector3.right;var frozenAt=frozenPin.transform.position;end.Invoke(game,new object[]{false});yield return new WaitForSecondsRealtime(.2f);Check(game.Paused&&Vector3.Distance(frozenPin.transform.position,frozenAt)<.001f,"Result dialog freezes game physics while UI effects continue");Check(ActiveText("resultBody").text=="12 duba kaldı.\nDoğru renk için sıranı değiştir."&&ActiveText("resultReward").text=="Coin ve swap hakların korunuyor.","Failure copy matches HTML and preserves wallet message");Capture("sync-unity-fail.png");
            game.Hud.ShowSettings();Check(game.Hud.ResultVisible&&!game.Hud.SettingsVisible,"Result modal cannot be replaced by settings");
            game.Hud.ReturnHome();game.SetMenu(false);game.Hud.ShowGame();game.LoadLevel(0);PlayerPrefs.SetInt("ReducedMotion",0);int before=game.Coins;end.Invoke(game,new object[]{true});yield return new WaitForSecondsRealtime(.8f);
            Check(ActiveText("resultBody").text=="12 duba devrildi.\n6 hamle cebinde kaldı!"&&ActiveText("resultReward").text=="+30 COIN · Bakiye: "+game.Coins+" coin","Victory copy and live reward balance match HTML");
            Check(game.Coins==before+30&&ActiveText("win star 0").text=="★"&&ActiveText("win star 2").text=="★","Victory reward and earned stars are live");
            Check(GameObject.Find("Victory fireworks")!=null&&GameObject.Find("Flying reward coins")!=null,"HTML victory fireworks and flying coin layers are present");Capture("sync-unity-win.png");
            foreach(var size in new[]{new Vector2Int(800,1000),new Vector2Int(1440,900),new Vector2Int(600,700),new Vector2Int(800,1200)}){Screen.SetResolution(size.x,size.y,false);yield return new WaitForSecondsRealtime(.35f);var r=game.Camera.pixelRect;float expectedWidth=Mathf.Min(Screen.width,Screen.height*.6f);Check(Mathf.Abs(r.width-expectedWidth)<1&&Mathf.Abs(r.height-Screen.height)<1,"DPI-independent app bounds match at "+Screen.width+"x"+Screen.height);Check(ActiveText("resultBody").text.Contains("12 duba devrildi"),"Result stays populated after resize "+Screen.width+"x"+Screen.height);if(size.x==1440)Capture("sync-unity-wide.png",Screen.width,Screen.height);}
            game.Hud.ReturnHome();Screen.SetResolution(600,1000,false);yield return new WaitForSecondsRealtime(.4f);PlayerPrefs.SetInt("Coins",12345);PlayerPrefs.SetInt("SwapCredits",123);game.ReloadEconomy();game.Hud.Refresh();yield return new WaitForSecondsRealtime(.6f);
            Check(ActiveText("homeCoins").text=="12345"&&ActiveText("homeSwaps").text=="123 swap","Home wallet displays multi-digit coin and swap values");Capture("sync-unity-funded-home.png");
            Click("HOME_SHOP");yield return new WaitForSecondsRealtime(.3f);Check(ActiveText("Shop coin total").text=="Coin: <b>12345</b>"&&ActiveText("Shop swap total").text=="Swap: <b>123</b>","Shop totals reflow around complete labels");Click("SHOP_BUY");yield return new WaitForSecondsRealtime(.6f);Check(game.Coins==12335&&game.SwapCredits==124&&ActiveText("Shop coin total").text.Contains("12335"),"Funded shop updates live totals after purchase");Capture("sync-unity-funded-shop.png");Click("CLOSE_SHOP");
            game.SetMenu(false);game.Hud.ShowGame();game.LoadLevel(0);yield return new WaitForSecondsRealtime(.6f);var wallet=GameObject.Find("wallet").GetComponent<UnityEngine.UI.Image>();var live=ActiveText("Live coins");Check(live.text=="12335 coin"&&wallet.rectTransform.rect.width>110,"Game wallet expands for multiple digits without baked coin text");Capture("sync-unity-funded-game.png");
            if(hadMuted)PlayerPrefs.SetInt("Muted",muted);else PlayerPrefs.DeleteKey("Muted");PlayerPrefs.SetInt("ReducedMotion",motion);game.ReloadSettings();
        }
        IEnumerator PolishChecks()
        {
            game.LoadLevel(0); game.SetMenu(true); game.Hud.ShowHome();
            yield return new WaitForSecondsRealtime(.5f);
            Check(game.InMenu && game.Hud.HomeVisible && !game.Shoot(Vector3.forward), "Home screen blocks gameplay input"); Capture("unity-polish-home.png");
            Click("HOME_SHOP"); yield return new WaitForSecondsRealtime(.4f);
            Check(game.Hud.ShopVisible && game.Paused && !game.BuySwap(), "Home shop pauses gameplay and protects insufficient balance"); Capture("unity-polish-shop.png"); Click("CLOSE_SHOP");
            Click("HOME_SETTINGS"); yield return new WaitForSecondsRealtime(.4f);
            Check(game.Hud.SettingsVisible && game.Paused, "Settings panel pauses the game"); Capture("unity-polish-settings.png"); Click("CLOSE_SETTINGS");
            Click("PLAY"); yield return new WaitForSecondsRealtime(.1f);
            Check(game.Transitioning && !game.Shoot(Vector3.forward), "Loading transition blocks shots"); Capture("unity-polish-loading.png");
            yield return new WaitForSecondsRealtime(1.5f);
            Check(!game.InMenu && !game.Transitioning && !game.Paused, "Play enters active gameplay after entrance animation"); Capture("unity-polish-game.png");
            int before = game.Queue[0]; Click("SWAP");
            Check(game.Queue[game.Queue.Count - 1] == before && game.SwapCredits == 2, "Animated queue preserves rotation and credit cost");
            Click("SETTINGS"); yield return new WaitForSecondsRealtime(.4f);
            Check(!game.Swap() && !game.Shoot(Vector3.forward), "Paused game blocks shooting and swapping"); Click("CLOSE_SETTINGS");
            Check(!game.Paused && !game.Hud.SettingsVisible, "Resume restores simulation");
            Click("SETTINGS"); yield return new WaitForSecondsRealtime(.1f); Click("GO_HOME"); yield return new WaitForSecondsRealtime(.1f);
            Check(game.InMenu && game.Hud.HomeVisible && game.SwapCredits == 2, "Return home preserves wallet and resets board");
            Click("PLAY"); yield return new WaitForSecondsRealtime(1.7f); Click("SETTINGS"); yield return new WaitForSecondsRealtime(.1f); Click("RESTART"); yield return new WaitForSecondsRealtime(1.7f);
            Check(!game.Transitioning && game.Moves == 6 && game.Cleared == 0 && game.SwapCredits == 2, "Retry transition resets only level state");
        }
        IEnumerator VisualChecks()
        {
            int[,] sizes = { {600,1000}, {390,844}, {800,1000}, {1000,600} };
            string[] names = { "01-ready.png", "06-phone.png", "07-tablet.png", "08-landscape.png" };
            game.LoadLevel(0);
            for (int i = 0; i < names.Length; i++)
            {
                PlayerPrefs.SetInt("SwapCredits", 3); game.ReloadEconomy(); game.Hud.Refresh();
                Screen.SetResolution(sizes[i,0], sizes[i,1], false); yield return new WaitForSecondsRealtime(.6f);
                Canvas.ForceUpdateCanvases(); Capture(names[i], Screen.width, Screen.height);
                var swapButton = Array.Find(FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None), b => b.name.StartsWith("SWAP"));
                Vector2 at = RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera, swapButton.transform.position);
                Canvas.ForceUpdateCanvases();
            var evt = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current) { position = at };
                var hits = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(evt, hits);
                Check(hits.Exists(h => h.gameObject == swapButton.gameObject), "Swap button receives pointer at " + Screen.width + "x" + Screen.height);
                int prior = game.Queue[0];
                UnityEngine.EventSystems.ExecuteEvents.Execute(swapButton.gameObject, evt, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
                Check(game.Queue[game.Queue.Count - 1] == prior && game.SwapCredits == 2, "UI click swaps queue at " + Screen.width + "x" + Screen.height);
            }
            File.WriteAllText(Path.Combine(folder, "visual-verification.txt"), (assertions - failures) + "/" + assertions + " passed.\n" + report);
        }
        void CheckGraphicRaycasts(BowlingTransition art,bool requireCurtainHit=true)
        {
            Canvas.ForceUpdateCanvases();
            var canvas=GameObject.Find("HTML pixel layout").GetComponent<Canvas>();
            bool renderers=true;foreach(var graphic in canvas.GetComponentsInChildren<UnityEngine.UI.Graphic>(true))renderers &= graphic.GetComponent<CanvasRenderer>()!=null;
            Check(renderers,"All UI graphics, including inactive custom effects, have CanvasRenderer");
            var position=art!=null?RectTransformUtility.WorldToScreenPoint(game.Hud.UICamera,art.rectTransform.position):new Vector2(Screen.width/2f,Screen.height/2f);
            var data=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){position=position};
            var hits=new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();bool safe=true;
            try{UnityEngine.EventSystems.EventSystem.current.RaycastAll(data,hits);}catch(MissingComponentException){safe=false;}
            Check(safe,"RaycastAll completes without MissingComponentException during transition/resize");
            if(requireCurtainHit)Check(art!=null&&hits.Exists(hit=>hit.gameObject==art.gameObject),"Visible loading curtain receives pointer raycast");
        }
        void Check(bool condition, string message)
        {
            assertions++; if (!condition) failures++;
            string line = (condition ? "PASS: " : "FAIL: ") + message; report += line + "\n"; Debug.Log(line);
        }
        bool HasVisiblePixels(Texture texture)
        {
            var previous=RenderTexture.active;var target=RenderTexture.GetTemporary(texture.width,texture.height,0,RenderTextureFormat.ARGB32);Graphics.Blit(texture,target);RenderTexture.active=target;var copy=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);copy.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);copy.Apply();int visible=0;foreach(var pixel in copy.GetPixels32())if(pixel.a>25)visible++;RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);Destroy(copy);return visible>100;
        }
        Color32[] WorldPixels()
        {
            int w=Screen.width,h=Screen.height;var rt=new RenderTexture(w,h,24);game.Camera.targetTexture=rt;game.Camera.Render();var previous=RenderTexture.active;RenderTexture.active=rt;var copy=new Texture2D(w,h,TextureFormat.RGB24,false);copy.ReadPixels(new Rect(0,0,w,h),0,0);copy.Apply();var data=copy.GetPixels32();game.Camera.targetTexture=null;RenderTexture.active=previous;rt.Release();Destroy(rt);Destroy(copy);return data;
        }
        void Capture(string name, int width = 600, int height = 1000)
        {
            Canvas.ForceUpdateCanvases();
            var rt = new RenderTexture(width, height, 24); game.Camera.targetTexture = rt; game.Camera.Render(); game.Hud.UICamera.targetTexture = rt; game.Hud.UICamera.Render();
            if(PinPopSplash.Visible){var splash=PinPopSplash.Instance.SplashCamera;splash.targetTexture=rt;splash.Render();splash.targetTexture=null;}
            var prev = RenderTexture.active; RenderTexture.active = rt;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false); tex.ReadPixels(new Rect(0, 0, width, height), 0, 0); tex.Apply();
            File.WriteAllBytes(Path.Combine(folder, name), tex.EncodeToPNG());
            game.Camera.targetTexture = null; game.Hud.UICamera.targetTexture = null; RenderTexture.active = prev; rt.Release(); Destroy(rt); Destroy(tex);
        }
    }
}

#endif
