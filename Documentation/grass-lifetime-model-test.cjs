const fs=require('fs'),path=require('path'),vm=require('vm');
const root=path.resolve(__dirname,'..'),html=fs.readFileSync(path.join(root,'index.html'),'utf8'),ref=fs.readFileSync(path.join(__dirname,'References/Smash_Match.html'),'utf8'),ctx={};vm.createContext(ctx);vm.runInContext(html.slice(html.indexOf('const CANNON=(()=>{'),html.indexOf('const LEVEL_REWARD='))+'\n'+html.slice(html.indexOf('const LEVEL_REWARD='),html.indexOf('const $=id=>'))+'\n'+html.slice(html.indexOf('class Game{'),html.indexOf('\nlet wallet={}'))+'\nthis.Game=Game;this.Tray=MatchTray;this.C=CANNON',ctx);
const results=[];function check(pass,label,data){results.push({pass:!!pass,label,data});console.log((pass?'PASS: ':'FAIL: ')+label+(data?' '+JSON.stringify(data):''))}
for(const [name,dir] of [['left',{x:-100,z:-7.2}],['right',{x:100,z:-7.2}],['back',{x:0,z:-100}],['front',{x:3.8,z:100}]]){
 const g=new ctx.Game();for(const p of g.pins)g.world.removeBody(p.body);g.shoot(dir,1);const ball=g.balls[0];let steps=0;while(!ball.onGrass&&steps++<1200)g.step(1/120);
 check(ball.onGrass,name+' strongest shot reaches grass');if(!ball.onGrass)continue;
 const land={x:ball.x,z:ball.z},edgeDistance=Math.hypot(Math.max(0,Math.abs(land.x)-4.2),Math.max(0,-8.8-land.z,land.z-4.8));
 check(edgeDistance<3,name+' lands close to table',{edgeDistance});
 check(Math.hypot(ball.vx,ball.vz)<=3.51,name+' grass impact caps speed');
 for(let i=0;i<180;i++)g.step(1/120);
 const distance=Math.hypot(ball.x-land.x,ball.z-land.z);
 check(distance<1.5&&distance>.01,name+' rolls a short distance',{distance});
 check(Math.hypot(ball.vx,ball.vz)<.02,name+' comes to rest');
 while(ball.grassAge<9.85)g.step(1/120);
 check(g.balls.includes(ball),name+' sleeping ball survives until ten seconds');
 for(let i=0;i<24;i++)g.step(1/120);
 check(!g.balls.includes(ball)&&!ball.body.world,name+' removed from renderer list and physics after ten seconds');
}
const g=new ctx.Game();g.shoot({x:-100,z:-7.2},1);const ball=g.balls[0];while(!ball.onGrass)g.step(1/120);g.busy=false;g.shoot({x:0,z:-100},.5);check(g.balls.includes(ball),'Next shot retains grass ball before expiry');
fs.writeFileSync(path.join(__dirname,'grass-lifetime-model-verification.json'),JSON.stringify({results},null,2));process.exit(results.some(x=>!x.pass)?1:0);
