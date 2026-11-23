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

await send('Runtime.enable');await send('Page.enable');const url='file:///Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/index.html';await send('Page.navigate',{url});await wait(1300);
for(const [width,height]of[[600,1000],[390,844],[320,740],[1000,600]]){await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await wait(180);await click('#homeHelp');await wait(400);check(await evaluate(`(()=>{const d=document.querySelector('#helpModal'),b=document.querySelector('#closeHelp').getBoundingClientRect();return d.open&&d.scrollHeight<=d.clientHeight+1&&b.bottom<innerHeight&&b.top>0})()`),'Help text and action fit without clipping at '+width+'x'+height);await screenshot('journey-help-'+width+'.png');await click('#closeHelp');await click('#homeSettings');await wait(400);await screenshot('journey-settings-'+width+'.png');await click('#resume');await click('#homeShop');await wait(400);await screenshot('journey-shop-'+width+'.png');await click('#closeShop')}
await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await click('#play');await wait(1550);check(await evaluate(`document.querySelector('#startBadge').getBoundingClientRect().bottom<PinPop.project(0,2.44,1.45).y`),'Chapter badge stays above pin heads');await screenshot('journey-entry-clear.png');check(errors.length===0,'No runtime errors during page inspection');await fs.writeFile(path.join(out,'journey-gallery-verification.json'),JSON.stringify({results,errors},null,2));ws.close();if(results.some(r=>!r.pass))process.exitCode=1;
