// Run with Node 20+: node --experimental-websocket Documentation/browser-test.mjs
// Requires a dedicated Chrome launched with --remote-debugging-port=9224.
import fs from 'node:fs/promises';
import path from 'node:path';
import {fileURLToPath} from 'node:url';
const pages=await(await fetch('http://127.0.0.1:9224/json/list')).json();
const ws=new WebSocket(pages.find(p=>p.type==='page').webSocketDebuggerUrl);
await new Promise((res,rej)=>{ws.addEventListener('open',res,{once:true});ws.addEventListener('error',rej,{once:true})});
let id=0;const pending=new Map(),errors=[];
ws.addEventListener('message',e=>{const m=JSON.parse(e.data);if(m.method==='Runtime.consoleAPICalled'&&m.params.type==='error')errors.push(m.params.args.map(a=>a.value||a.description||'').join(' '));if(m.method==='Runtime.exceptionThrown')errors.push(m.params.exceptionDetails.text+': '+m.params.exceptionDetails.exception?.description);if(m.id){const p=pending.get(m.id);pending.delete(m.id);m.error?p.reject(m.error):p.resolve(m.result)}});
const send=(method,params={})=>new Promise((resolve,reject)=>{const key=++id;pending.set(key,{resolve,reject});ws.send(JSON.stringify({id:key,method,params}));});
const evaluate=async expression=>{const r=await send('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true});if(r.exceptionDetails)throw Error(r.exceptionDetails.exception?.description||r.exceptionDetails.text);return r.result.value};
const wait=ms=>new Promise(r=>setTimeout(r,ms));
const results=[];function check(value,label){results.push({pass:!!value,label});console.log((value?'PASS':'FAIL')+': '+label)}
const out='/Users/ayetullah/Desktop/Projeler/Unity Projects/PinPop/Documentation';
async function screenshot(name){const shot=await send('Page.captureScreenshot',{format:'png',captureBeyondViewport:false});await fs.writeFile(path.join(out,name),Buffer.from(shot.data,'base64'));}
async function click(selector){const p=await evaluate(`(()=>{const r=document.querySelector(${JSON.stringify(selector)}).getBoundingClientRect();return{x:r.x+r.width/2,y:r.y+r.height/2}})()`);await send('Input.dispatchMouseEvent',{type:'mousePressed',...p,button:'left',clickCount:1});await send('Input.dispatchMouseEvent',{type:'mouseReleased',...p,button:'left',clickCount:1});}

await send('Runtime.enable');await send('Page.enable');
await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});
await send('Page.navigate',{url:'file:///Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/index.html'});await wait(1800);await evaluate("localStorage.clear();location.reload();true");await wait(1200);
check(await evaluate('!!window.PinPop'),'Standalone rolling game boots');
const core=await evaluate(`(()=>{const checks=[],test=(pass,label)=>checks.push({pass:!!pass,label}),run=(g,n=600)=>{for(let i=0;i<n;i++)g.step(1/120)};
const weak=new PinPop.Game(),strong=new PinPop.Game();weak.shoot({x:-2.45,z:-.2},.2);strong.shoot({x:-2.45,z:-.2},1);
test(weak.balls[0].body.mass===strong.balls[0].body.mass&&strong.balls[0].body.mass===6,'Bowling ball mass is constant at every power');
test(weak.balls[0].vy===0&&strong.balls[0].vy===0&&strong.balls[0].vx<weak.balls[0].vx,'Power sets horizontal speed without upward launch');
let onTable=true,spin=false;for(let i=0;i<30;i++){weak.step(1/120);const b=weak.balls[0];onTable&&=b.y>.35&&b.y<.45;spin||=b.body.angularVelocity.length()>5}test(onTable&&spin,'Ball rolls and rotates on table before contact');run(weak);run(strong);test(weak.cleared>0&&strong.cleared>weak.cleared,'Strong roll topples more pins than weak roll at the same aim');
test(strong.pins.filter(p=>p.color!==0).every(p=>!p.cleared&&p.body.velocity.length()<.001),'Other-color pins stay fixed');
const blocked=new PinPop.Game();blocked.queue[0]=1;const original=blocked.pins.filter(p=>p.color===0).map(p=>[p.x,p.z]);blocked.shoot({x:-2.45,z:-.2},1);run(blocked);test(blocked.pins.filter(p=>p.color===0).every((p,i)=>!p.cleared&&Math.hypot(p.x-original[i][0],p.z-original[i][1])<.001),'Wrong-colored roll cannot move the blocking cluster');
for(let level=0;level<6;level++){const g=new PinPop.Game();g.reset(level);let guard=0;while(!g.finished&&g.moves>0&&guard++<15){const matching=g.pins.filter(p=>!p.cleared&&p.color===g.queue[0]).sort((a,b)=>a.z-b.z);if(!matching.length&&g.swaps>0){g.swap();continue}const front=matching.filter(p=>p.z<(matching[0]?.z??0)+.3),x=front.length?front.reduce((a,p)=>a+p.x,0)/front.length:0;g.shoot({x,z:matching[0]?.z??0},1);run(g)}test(g.won&&g.cleared===g.total&&g.moves>=0,'Level '+(level+1)+' is winnable within authored moves with rolling physics')}
return checks})()`);for(const r of core)check(r.pass,r.label);
await click('#play');await wait(1600);
check(await evaluate('Math.abs(PinPop.getState().launch.y-.39)<.02&&PinPop.getState().launch.z>-8.4'),'Ready ball starts on the table');
await screenshot('rolling-html-ready.png');
async function anchor(){return evaluate(`(()=>{const p=PinPop.getLaunchPoint(),r=document.querySelector('#board').getBoundingClientRect();return{x:r.x+p.x*r.width/600,y:r.y+p.y*r.height/1000,w:r.width,h:r.height}})()`)}
async function down(){const p=await anchor();await send('Input.dispatchMouseEvent',{type:'mousePressed',x:p.x,y:p.y,button:'left',clickCount:1});return p}
async function move(p,dx,dy){await send('Input.dispatchMouseEvent',{type:'mouseMoved',x:p.x+dx*p.w/600,y:p.y+dy*p.h/1000,buttons:1});await wait(50)}
async function up(p,dx,dy){await send('Input.dispatchMouseEvent',{type:'mouseReleased',x:p.x+dx*p.w/600,y:p.y+dy*p.h/1000,button:'left',clickCount:1})}
let p=await down();await up(p,0,0);check(await evaluate('!PinPop.getState().busy&&PinPop.getState().moves===6'),'Tap without pull spends no move');
p=await down();await move(p,60,125);
check(await evaluate(`(()=>{const s=PinPop.getState();return s.pulling&&s.power>.8&&s.aim.x< -2&&s.origin.z>=-8.3&&s.origin.y===s.launch.y&&Math.abs(s.origin.x)<=.75})()`),'Backward diagonal drag aims opposite and keeps held ball on table');
check(await evaluate(`(()=>{const s=PinPop.getState();return s.aimVisible&&!s.landingVisible&&document.querySelector('#landingLabel')===null&&s.aimPreview.y===.035&&s.aimPreview.z<s.aim.z})()`),'Surface aim stops at first pin and has no drop label');
check(await evaluate(`document.querySelector('#pullInstructions').textContent.includes('PEMBE')`),'Aim instruction names the matching pin color');
await screenshot('rolling-html-aim.png');await up(p,60,125);await wait(80);
check(await evaluate('PinPop.getState().busy&&PinPop.getState().moves===5&&!PinPop.getState().aimVisible'),'Release rolls once and hides the guide');
check(await evaluate(`(()=>{const s=PinPop.getState();return Number(document.querySelector('#nextBall').dataset.color)===s.queue[0]})()`),'Rolling preview shows actual next ball');
await click('#settings');let frozen=await evaluate('PinPop.getState().balls[0]');await wait(180);check(await evaluate(`(()=>{const b=PinPop.getState().balls[0];return b&&Math.hypot(b.x-(${frozen.x}),b.y-(${frozen.y}),b.z-(${frozen.z}))<.001})()`),'Settings pause rolling physics');await click('#resume');for(let n=0;n<100&&await evaluate('PinPop.getState().busy');n++)await wait(100);
for(const [width,height]of[[390,844],[320,740],[1000,600]]){await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await wait(250);p=await down();await move(p,-60,125);check(await evaluate('PinPop.getState().pulling&&PinPop.getState().aimVisible&&PinPop.getState().origin.z>=-8.3'),'Touch-sized aim works at '+width+'x'+height);await screenshot('rolling-html-'+width+'.png');await evaluate("window.dispatchEvent(new Event('blur'));true");await up(p,-60,125);check(await evaluate('!PinPop.getState().pulling&&!PinPop.getState().busy'),'Focus loss cancels without firing at '+width+'x'+height)}
await send('Emulation.setDeviceMetricsOverride',{width:390,height:844,deviceScaleFactor:1,mobile:true});await send('Emulation.setTouchEmulationEnabled',{enabled:true,maxTouchPoints:2});await wait(200);p=await anchor();
const beforeTouch=await evaluate('PinPop.getState().moves');
await send('Input.dispatchTouchEvent',{type:'touchStart',touchPoints:[{x:p.x,y:p.y,id:1}]});await send('Input.dispatchTouchEvent',{type:'touchMove',touchPoints:[{x:p.x+35,y:p.y+85,id:1}]});await wait(100);
check(await evaluate('PinPop.getState().pulling&&PinPop.getState().power>.5&&PinPop.getState().aimVisible'),'Real touch drag powers the table roll on a phone');await screenshot('rolling-html-touch.png');
await send('Input.dispatchTouchEvent',{type:'touchCancel',touchPoints:[]});check(await evaluate(`!PinPop.getState().pulling&&!PinPop.getState().busy&&PinPop.getState().moves===${beforeTouch}`),'Touch cancellation never rolls or spends a move');await send('Emulation.setTouchEmulationEnabled',{enabled:false});
check(errors.length===0,'No browser runtime exceptions');await fs.writeFile(path.join(out,'rolling-browser-verification.json'),JSON.stringify({results,errors},null,2));ws.close();if(results.some(r=>!r.pass))process.exitCode=1;
