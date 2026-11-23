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

await send('Runtime.enable');await send('Page.enable');await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await send('Page.navigate',{url:'file:///Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/index.html'});await wait(1800);await click('#play');await wait(1400);
for(const [width,height]of[[600,1000],[390,844],[1000,600]]){
 await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await wait(250);
 const state=await evaluate(`(()=>{const s=PinPop.getState(),nearest=Math.min(...s.pins.map(p=>p.z))-s.launch.z,launch=PinPop.getLaunchPoint(),pin=PinPop.project(0,-.2,0);const ballHeight=PinPop.project(0,s.launch.z,s.launch.y-.39).y-PinPop.project(0,s.launch.z,s.launch.y+.39).y;return{nearest,pixels:launch.y-pin.y,ballHeight,launch}})()`);
 check(state.nearest>=6.9,'At least 6.9 units of rolling approach at '+width+'x'+height);check(state.pixels-state.ballHeight/2>=state.ballHeight*3,'At least three ball heights of visible empty approach at '+width+'x'+height);await screenshot('spacing-html-'+width+'.png');
}
check(errors.length===0,'No browser runtime exceptions');await fs.writeFile(path.join(out,'spacing-browser-verification.json'),JSON.stringify({results,errors},null,2));ws.close();if(results.some(r=>!r.pass))process.exitCode=1;
