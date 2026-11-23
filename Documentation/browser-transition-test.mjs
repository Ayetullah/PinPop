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
const out=path.dirname(fileURLToPath(import.meta.url));
async function screenshot(name){const shot=await send('Page.captureScreenshot',{format:'png',captureBeyondViewport:false});await fs.writeFile(path.join(out,name),Buffer.from(shot.data,'base64'));}
async function click(selector){const p=await evaluate(`(()=>{const r=document.querySelector(${JSON.stringify(selector)}).getBoundingClientRect();return{x:r.x+r.width/2,y:r.y+r.height/2}})()`);await send('Input.dispatchMouseEvent',{type:'mousePressed',...p,button:'left',clickCount:1});await send('Input.dispatchMouseEvent',{type:'mouseReleased',...p,button:'left',clickCount:1});}
await send('Runtime.enable');await send('Page.enable');
const base='file:///Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/index.html';
async function boot(w,h,reduced=false,level=0){await send('Emulation.setDeviceMetricsOverride',{width:w,height:h,deviceScaleFactor:1,mobile:false});await send('Page.navigate',{url:base});await wait(700);await evaluate(`localStorage.setItem('pinpop-html-motion','${reduced}');localStorage.setItem('pinpop-html-level','${level}');location.reload();true`);await wait(700);}
await boot(600,1000);let original=await evaluate('PinPop.getState()');await click('#play');await wait(140);
check(await evaluate('PinPop.getState().transitioning&&document.querySelector("#curtain").classList.contains("active")'),'Transition is visible and locks the level');
check(await evaluate('document.querySelector("#transitionLevel").textContent==="BÖLÜM 1"&&!!document.querySelector("#transitionPin")'),'Transition has bowling artwork and live chapter title');
await screenshot('transition-html-start.png');let first=await evaluate('getComputedStyle(document.querySelector(".transition-ball")).transform');await wait(250);await screenshot('transition-html-impact.png');
check(first!==await evaluate('getComputedStyle(document.querySelector(".transition-ball")).transform'),'Bowling ball animation advances');await wait(1100);
check(await evaluate('!PinPop.getState().transitioning&&PinPop.getState().screen==="game"'),'Transition hands off to playable board');
check(await evaluate(`PinPop.getState().coins===${original.coins}&&PinPop.getState().swaps===${original.swaps}&&PinPop.getState().moves===6`),'Transition preserves economy and moves');
for(const [w,h]of[[390,844],[320,740],[1000,600]]){await boot(w,h,false,4);await click('#play');await wait(300);check(await evaluate('(()=>{const a=document.querySelector("#app").getBoundingClientRect(),c=document.querySelector("#curtain").getBoundingClientRect();return c.width===a.width&&c.height===a.height&&document.querySelector("#transitionLevel").textContent==="BÖLÜM 5"&&document.documentElement.scrollWidth<=innerWidth})()'),'Chapter artwork fits '+w+'×'+h);await screenshot('transition-html-'+w+'.png');await wait(800)}
await boot(600,1000,true,2);await click('#play');await wait(30);check(await evaluate('document.querySelector("#curtain").dataset.reduced==="true"&&getComputedStyle(document.querySelector(".transition-ball")).animationName==="none"'),'Reduced motion disables bowling animation');await wait(350);check(await evaluate('PinPop.getState().transitioning'),'Reduced motion preserves readable loading duration');await wait(1200);check(await evaluate('!PinPop.getState().transitioning&&PinPop.getState().level===2'),'Reduced motion enters the requested chapter after readable loading');
check(errors.length===0,'No browser runtime exceptions');await fs.writeFile(path.join(out,'transition-browser-verification.json'),JSON.stringify({results,errors},null,2));ws.close();if(results.some(r=>!r.pass))process.exitCode=1;
