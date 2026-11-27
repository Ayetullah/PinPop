#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.Reflection;
using UnityEngine;
namespace PinPop {
 public sealed partial class CarnivalUIReview {
  BowlingBall ReviewBall(Vector3 at,int color=0,Vector3? velocity=null){var obj=new GameObject("Fracture review ball");obj.transform.position=at;var ball=obj.AddComponent<BowlingBall>();ball.Setup(game,color,velocity??Vector3.zero,.5f);return ball;}
  IEnumerator BallDespawnChecks(){
   CrispUI.ReviewReducedMotion=null;PlayerPrefs.SetInt("ReducedMotion",1);game.ReloadSettings();Check(!game.ReducedMotion&&!PlayerPrefs.HasKey("ReducedMotion"),"Removed motion setting cannot disable normal animations");
   GameLocalization.SetPreference("tr");Size(new Vector2Int(1170,2532));yield return new WaitForSecondsRealtime(.3f);Board(0);yield return null;Time.timeScale=0;
   int moves=game.Moves,coins=game.Coins,cleared=game.Cleared,queue=game.Queue.Count;
   var ball=ReviewBall(new Vector3(0,BowlingBall.Radius,-4));var visual=ball.transform.Find("Polished bowling ball");var shared=visual.GetComponent<Renderer>().sharedMaterial;Color originalColor=shared.color;
   Capture("table-before");Check(ball.Despawn(),"First retirement starts a fracture");Check(!ball.Despawn()&&ball.GetComponents<BallDespawnFX>().Length==1,"Repeated retirement cannot duplicate debris");
   Check(!ball.GetComponent<Collider>().enabled&&ball.GetComponent<Rigidbody>().isKinematic&&!ball.GetComponent<TrailRenderer>().emitting,"Breaking ball stops physics and trail emission");
   var fx=ball.GetComponent<BallDespawnFX>();Check(!visual.gameObject.activeSelf&&visual.localScale==Vector3.one*BowlingBall.Radius*2,"Whole sphere is replaced immediately without shrinking or dissolving");
   Check(fx.Fragments.Length==20&&ball.GetComponentsInChildren<Collider>().Length==1&&ball.GetComponentsInChildren<ParticleSystem>().Length==0,"Exactly twenty solid mesh fragments; no extra physics bodies or dust particles");
   bool closed=true;foreach(var piece in fx.Fragments){var mesh=piece.GetComponent<MeshFilter>().sharedMesh;closed&=mesh.subMeshCount==2&&mesh.GetTriangles(0).Length>0&&mesh.GetTriangles(1).Length>0;}
   Check(closed,"All fragments have curved outer shells and opaque cut faces");Capture("table-fracture-start");
   Vector3 initial=fx.Fragments[0].position;Quaternion rotation=fx.Fragments[0].rotation;
   yield return new WaitForSecondsRealtime(.15f);Check(fx.Fragments[0].position!=initial&&Quaternion.Angle(rotation,fx.Fragments[0].rotation)>1,"Fragments fly apart and tumble even while gameplay is paused");
   Check(Mathf.Abs(fx.Fragments[0].localScale.x-BowlingBall.Radius)<.001f,"Fragments keep their full readable size during breakup");Capture("table-fracture-flight");
   yield return new WaitForSecondsRealtime(.42f);Check(fx.BounceCount>0,"Solid fragments visibly bounce on the wooden surface");Capture("table-fracture-bounce");
   yield return new WaitForSecondsRealtime(BallDespawnFX.Duration+.1f);Check(ball==null,"All debris and the retired ball are cleaned up");Check(shared.color==originalColor,"Fracture never mutates a shared ball material");Capture("table-after");
   Check(game.Moves==moves&&game.Coins==coins&&game.Cleared==cleared&&game.Queue.Count==queue,"Fracture spends no moves, coins, pins or queue entries");
   Board(0);
   var settled=ReviewBall(new Vector3(0,BowlingBall.Radius+.01f,-4));float deadline=Time.realtimeSinceStartup+1;
   while(settled!=null&&!settled.IsDespawning&&Time.realtimeSinceStartup<deadline)yield return null;
   Check(settled!=null&&settled.IsDespawning&&!settled.HasReachedGrass&&settled.GrassAge==0,"A supported table ball fractures automatically on settling, without the grass countdown or another shot");
   game.LoadLevel(0);yield return null;Check(FindObjectsByType<BallDespawnFX>(FindObjectsSortMode.None).Length==0,"Level reset clears every active fragment");
   var moving=ReviewBall(new Vector3(0,BowlingBall.Radius+.01f,-4),0,new Vector3(.8f,0,0));yield return new WaitForSeconds(.25f);Check(moving!=null&&!moving.IsDespawning,"A slowly rolling table ball is not fractured prematurely");
   moving.GetComponent<Rigidbody>().linearVelocity=Vector3.zero;moving.GetComponent<Rigidbody>().angularVelocity=Vector3.zero;moving.GetComponent<Rigidbody>().Sleep();yield return new WaitForSeconds(.16f);Check(moving!=null&&moving.IsDespawning,"The same table ball fractures as soon as it rests");
   game.LoadLevel(0);yield return null;var airborne=ReviewBall(new Vector3(0,1.5f,-4));yield return new WaitForSeconds(.1f);Check(airborne!=null&&!airborne.IsDespawning,"An airborne ball with zero horizontal speed does not fracture");game.LoadLevel(0);yield return null;
   Check(game.Shoot(new Vector3(0,.4f,0),.2f),"Table end-of-turn test uses the real shot resolver");var turnBall=FindFirstObjectByType<BowlingBall>();var turnBody=turnBall.GetComponent<Rigidbody>();turnBody.position=new Vector3(-2,BowlingBall.Radius+.01f,-6);turnBody.linearVelocity=new Vector3(.35f,0,0);turnBody.angularVelocity=new Vector3(0,0,-.35f/BowlingBall.Radius);
   deadline=Time.realtimeSinceStartup+4;while(game.Busy&&!turnBall.IsDespawning&&Time.realtimeSinceStartup<deadline)yield return null;
   Check(turnBall!=null&&turnBall.IsDespawning&&!game.Busy&&!turnBall.HasReachedGrass,"A ball remaining on the table fractures at shot completion without waiting for grass expiry or a new shot");
   game.LoadLevel(0);yield return null;
   game.Hud.ShowGame();game.SetMenu(false);Time.timeScale=1;Check(game.Shoot(new Vector3(-20,.4f,-3),1),"Grass test launches a real side shot");var grass=FindFirstObjectByType<BowlingBall>();deadline=Time.realtimeSinceStartup+5;
   while(grass!=null&&!grass.HasReachedGrass&&Time.realtimeSinceStartup<deadline)yield return null;
   Check(grass!=null&&grass.HasReachedGrass,"Shot lands on the real grass collider");
   if(grass!=null&&grass.HasReachedGrass){
    Check(!grass.IsDespawning&&BowlingBall.GrassLifetime==2,"Grass landing starts the requested two-second lifetime");
    var body=grass.GetComponent<Rigidbody>();float speed=new Vector2(body.linearVelocity.x,body.linearVelocity.z).magnitude;yield return new WaitForSeconds(.3f);Check(new Vector2(body.linearVelocity.x,body.linearVelocity.z).magnitude<speed,"Grass friction still slows the rolling ball");
    Time.timeScale=0;float age=grass.GrassAge;yield return new WaitForSecondsRealtime(.2f);Check(grass.GrassAge==age&&!grass.IsDespawning,"Pause freezes grass lifetime before fracture");Time.timeScale=1;
    var second=ReviewBall(new Vector3(0,BowlingBall.Radius+.01f,-4),1);Check(!grass.IsDespawning,"The next ball does not retire the previous grass ball");second.Despawn();
    // Observe the actual timer from first grass contact, without accelerating it.
    while(grass!=null&&grass.GrassAge<BowlingBall.GrassLifetime-.3f&&!grass.IsDespawning)yield return null;
    Check(grass!=null&&!grass.IsDespawning&&grass.GrassAge>=BowlingBall.GrassLifetime-.3f,"Grass ball remains intact before the two-second threshold");
    Vector3 cameraPosition=game.Camera.transform.position;Quaternion cameraRotation=game.Camera.transform.rotation;
    game.enabled=false;game.Camera.transform.position=grass.transform.position+new Vector3(0,3.2f,-4);game.Camera.transform.LookAt(grass.transform.position);Capture("grass-before");
    while(grass!=null&&!grass.IsDespawning)yield return null;
    Check(grass!=null&&grass.GrassAge>=2&&grass.GrassAge<2.1f&&grass.GetComponent<BallDespawnFX>().Fragments.Length==20,"At two seconds after grass contact the ball starts its twenty-piece fracture");
    yield return new WaitForSecondsRealtime(.16f);Capture("grass-fracture-flight");yield return new WaitForSecondsRealtime(.45f);Check(grass!=null&&grass.GetComponent<BallDespawnFX>().BounceCount>0,"Fragments bounce on grass rather than an invisible tabletop");Capture("grass-fracture-bounce");
    yield return new WaitForSecondsRealtime(BallDespawnFX.Duration);Check(grass==null,"Grass fracture cleans up every fragment");game.Camera.transform.SetPositionAndRotation(cameraPosition,cameraRotation);game.enabled=true;
   }
   Board(0);Time.timeScale=0;typeof(PinPopAdventure).GetProperty("RainbowArmed").SetValue(game.Adventure,true);game.Adventure.BeginShot();var rainbow=ReviewBall(new Vector3(0,BowlingBall.Radius,-4));Check(rainbow.Rainbow,"Rainbow fixture uses real shot configuration");rainbow.Despawn();var rainbowFx=rainbow.GetComponent<BallDespawnFX>();
   Check(!rainbow.GetComponent<RainbowBallFX>().enabled&&rainbowFx.Fragments[0].GetComponent<Renderer>().sharedMaterials[0].color!=rainbowFx.Fragments[1].GetComponent<Renderer>().sharedMaterials[0].color,"Rainbow ball breaks into differently colored solid pieces");yield return new WaitForSecondsRealtime(.16f);Capture("rainbow-fracture");game.LoadLevel(0);yield return null;Check(FindObjectsByType<BallDespawnFX>(FindObjectsSortMode.None).Length==0,"Level change also removes rainbow debris");
  }
 }
}
#endif
