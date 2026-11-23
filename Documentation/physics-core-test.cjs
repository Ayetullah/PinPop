// Run: node Documentation/physics-core-test.cjs — reads the actual offline game bundle.
const fs=require('node:fs'),path=require('node:path'),vm=require('node:vm');
const html=fs.readFileSync(path.join(__dirname,'../index.html'),'utf8');
const library=html.slice(html.indexOf('const CANNON=(()=>{'),html.indexOf('const LEVEL_REWARD='));
const game=html.slice(html.indexOf('class Game{'),html.indexOf('\nlet wallet={}'));
const constants=html.slice(html.indexOf('const LEVEL_REWARD='),html.indexOf('const $=id=>'));
const context={};vm.createContext(context);vm.runInContext(library+'\n'+constants+'\n'+game+'\nthis.Game=Game;',context);
const checks=[],angles=[];function check(pass,label){checks.push({pass:!!pass,label});console.log((pass?'PASS: ':'FAIL: ')+label)}
function energy(b){const w=b.quaternion.conjugate().vmult(b.angularVelocity);return .5*b.mass*b.velocity.lengthSquared()+.5*(b.inertia.x*w.x*w.x+b.inertia.y*w.y*w.y+b.inertia.z*w.z*w.z)}
for(const x of[-.35,0,.35]){
 const g=new context.Game(),pin=g.pins[0];for(const b of [...g.world.bodies])if(b!==pin.body)g.world.removeBody(b);g.pins=[pin];g.world.gravity.set(0,0,0);pin.body.position.set(0,.44,0);pin.body.linearDamping=pin.body.angularDamping=0;pin.body.sleep();
 g.shoot({x:0,z:0},.7,{x,y:.55,z:-2});const b=g.balls[0].body;b.position.set(x,.55,-2);b.velocity.set(0,0,8);b.angularVelocity.set(0,0,0);b.linearDamping=b.angularDamping=0;
 const before=energy(b),momentum=b.mass*8;for(let i=0;i<60;i++)g.step(1/240);const c=g.contacts[0],after=energy(b)+energy(pin.body);
 check(!!c,'Contact occurs at offset '+x);check(Math.abs(b.mass*b.velocity.x+pin.body.mass*pin.body.velocity.x)<.001,'Horizontal x momentum conserved at offset '+x);check(Math.abs(b.mass*b.velocity.z+pin.body.mass*pin.body.velocity.z-momentum)<.001,'Horizontal z momentum conserved at offset '+x);check(after<before&&after>0,'Impact dissipates energy without creating energy at offset '+x);
 angles.push({offset:x,normal:c?.normal,outgoing:c?.outgoing,kineticBefore:before,kineticAfter:after});
}
check(angles[0].outgoing.x<-.3&&angles[2].outgoing.x>.3,'Contact angle controls left/right deflection');check(Math.abs(angles[1].outgoing.x)<.01,'Centered contact has no artificial lateral kick');check(Math.abs(angles[0].outgoing.x+angles[2].outgoing.x)<.01,'Mirrored angles produce mirrored deflection');
const blockedAngles=[];
for(const x of[-.35,0,.35]){
 const g=new context.Game(),pin=g.pins[0];for(const b of [...g.world.bodies])if(b!==pin.body)g.world.removeBody(b);g.pins=[pin];g.world.gravity.set(0,0,0);pin.body.position.set(0,.44,0);pin.body.sleep();g.queue[0]=1;let blocked=0;g.onBlocked=()=>blocked++;
 g.shoot({x:0,z:0},.7,{x,y:.55,z:-2});const b=g.balls[0].body;b.position.set(x,.55,-2);b.velocity.set(0,0,8);b.angularVelocity.set(0,0,0);b.linearDamping=0;
 for(let i=0;i<60;i++)g.step(1/240);const c=g.contacts[0];
 check(!!c,'Wrong-color pin physically contacts ball at offset '+x);check(Math.abs(pin.body.position.x)<1e-8&&Math.abs(pin.body.position.y-.44)<1e-8&&Math.abs(pin.body.position.z)<1e-8&&!pin.cleared,'Wrong-color pin stays fixed and uncleared at offset '+x);check(b.velocity.length()<8,'Wrong-color collision dissipates ball speed at offset '+x);check(blocked===1&&pin.flash>0,'Wrong-color contact triggers one brief block feedback at offset '+x);check(g.cleared===0&&g.moves===5,'Wrong-color rebound costs one move without clearing pins at offset '+x);
 blockedAngles.push({offset:x,outgoing:c?.outgoing});
 for(let i=0;i<60;i++)g.step(1/120);check(pin.flash===0,'Block flash fades away at offset '+x);
 if(x===0){check(c.outgoing.z<-.5&&Math.abs(c.outgoing.x)<.05,'Centered wrong-color collision rebounds without lateral drift');g.busy=false;g.queue[0]=0;g.shoot({x:0,z:0},1,{x:0,y:.55,z:-2});g.balls[0].body.velocity.set(0,0,8);check(pin.body.mass===1&&!pin.locked,'Matching next ball unlocks the formerly fixed pin');for(let i=0;i<240;i++)g.step(1/240);check(pin.cleared||pin.body.velocity.length()>.1,'Matching next ball can move the previously blocked pin');}
}
check(blockedAngles[0].outgoing.x<-.3&&blockedAngles[2].outgoing.x>.3,'Wrong-color glancing contacts deflect in opposite directions');check(Math.abs(blockedAngles[0].outgoing.x+blockedAngles[2].outgoing.x)<.05,'Wrong-color mirrored contacts have consistent rebound angles');
{
 const g=new context.Game();g.shoot({x:0,z:0},.7,{x:0,y:1.5,z:-2});const b=g.balls[0].body;b.velocity.set(0,0,0);b.angularVelocity.set(0,0,0);for(let i=0;i<360;i++)g.step(1/120);check(Math.abs(b.position.y-.39)<.03,'All ball colors still collide with the table after filter changes');
}
for(const power of [.2,.8,1]){
 const origin={x:0,y:.4,z:-7.2},target={x:-2.45,z:.2};
 const guide=context.Game.preview(target,power,origin),path=context.Game.trajectory(target,power,origin),velocity=context.Game.shot(target,power,origin);
 check(velocity.vy===0&&guide.time>0,'Rolling launch is horizontal at power '+power);
 check(path.every(p=>Math.abs(p.y-.035)<1e-6),'Every guide point stays on table surface at power '+power);
 check(Math.hypot(path.at(-1).x-guide.x,path.at(-1).z-guide.z)<1e-5,'Guide ends at surface target at power '+power);
 const g=new context.Game();g.shoot(target,power);const body=g.balls[0].body;let grounded=true;for(let n=0;n<24;n++){g.step(1/120);grounded&&=Math.abs(body.position.y-.39)<.05}
 check(grounded&&body.angularVelocity.length()>1,'Real body rolls on table before pin contact at power '+power);
}
const previewGame=new context.Game(),preview=context.Game.preview({x:0,z:.2},1,{x:0,y:.4,z:-7.2},previewGame.pins);
check(preview.z<-.2,'Guide terminates at first pin obstacle');
const layouts=[];for(let level=0;level<12;level++){const g=new context.Game();g.reset(level);let shots=0,guard=0;while(!g.finished&&g.moves>0&&guard++<20){const matching=g.pins.filter(p=>!p.cleared&&p.color===g.queue[0]).sort((a,b)=>a.z-b.z);if(!matching.length&&g.swaps>0){g.swap();continue}const front=matching.filter(p=>p.z<(matching[0]?.z??0)+.3),x=front.length?front.reduce((a,p)=>a+p.x,0)/front.length:0;g.shoot({x,z:matching[0]?.z??0},1);shots++;for(let i=0;i<600;i++)g.step(1/120)}layouts.push({level:level+1,won:g.won,cleared:g.cleared,total:g.total,movesLeft:g.moves,shots});check(g.won&&g.cleared===g.total,'Distinct layout '+(level+1)+' completes within its move budget')}
fs.writeFileSync(path.join(__dirname,'all-layouts-verification.json'),JSON.stringify({layouts},null,2));
fs.writeFileSync(path.join(__dirname,'collision-core-verification.json'),JSON.stringify({date:new Date().toISOString(),passed:checks.filter(c=>c.pass).length,total:checks.length,angles,blockedAngles,checks},null,2));process.exitCode=checks.every(c=>c.pass)?0:1;
