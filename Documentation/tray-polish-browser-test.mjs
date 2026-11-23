// Run with Node 20+: node --experimental-websocket Documentation/browser-test.mjs
// Requires a dedicated Chrome launched with --remote-debugging-port=9224.
import fs from 'node:fs/promises';
import path from 'node:path';
import {fileURLToPath,pathToFileURL} from 'node:url';
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


async function until(expression,limit=15000){const start=Date.now();while(Date.now()-start<limit){if(await evaluate(expression))return true;await wait(100)}return false}
await send('Runtime.enable');await send('Page.enable');await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await send('Page.navigate',{url:pathToFileURL(path.resolve(out,'../index.html')).href});await wait(1100);await click('#play');if(!await until('PinPop.getState().screen==="game"&&!PinPop.getState().transitioning'))throw Error('Game not entered');
await evaluate(`(()=>{const step=PinPop.Game.prototype.step;PinPop.Game.prototype.step=function(dt){window.trayPolishFixture=this;return step.call(this,dt)}})()`);await wait(100);
async function collect(colors){await evaluate(`(()=>{for(const c of ${JSON.stringify(colors)}){const item=trayPolishFixture.tray.collect(c,{x:0,y:0,z:0});trayPolishFixture.onCollect?.(item)}})()`)}
await screenshot('tray-polish-browser-empty.png');await collect([0,0]);await wait(850);
check(await evaluate('document.querySelectorAll(".collection-token.pair").length===2'),'Two waiting same-color pieces receive pair halos');await screenshot('tray-polish-browser-pair.png');
await collect([1]);await wait(850);check(await evaluate('document.querySelectorAll(".collection-token.pair").length===2&&document.querySelectorAll(".collection-token").length===3'),'Unrelated piece stays distinct from the ready pair');await screenshot('tray-polish-browser-mixed.png');
await collect([0]);await wait(660);await screenshot('tray-polish-browser-merge.png');await wait(500);check(await evaluate('PinPop.getState().tray.live===1&&PinPop.getState().tray.matches===1'),'Polished merge preserves exact three-piece removal');
for(const [width,height] of [[600,1000],[320,740],[390,844],[1000,600]]){await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await wait(250);check(await evaluate('(()=>{const cells=[...document.querySelectorAll(".collection-slots i")];return cells.length===10&&cells.every(c=>{const r=c.getBoundingClientRect();return r.width>10&&Math.abs(r.width-r.height)<1})})()'),'Ten square inset wells at '+width+'x'+height);check(await evaluate('(()=>{const d=document.querySelector("#collectionDock").getBoundingClientRect(),n=document.querySelector("#nextPreview").getBoundingClientRect();return d.bottom<=n.top+1})()'),'Dock leaves upcoming balls clear at '+width+'x'+height);await screenshot('tray-polish-browser-'+width+'.png')}
await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await evaluate('trayPolishFixture.reset(0)');await collect([0,0,1,1,2,2,3,3]);await wait(900);check(await evaluate('document.querySelector("#collectionCount").textContent==="8 / 10"&&document.querySelector("#collectionTray").classList.contains("danger")'),'Capacity warning communicates eight unmatched pieces');await screenshot('tray-polish-browser-capacity.png');
check(errors.length===0,'No browser runtime errors');await fs.writeFile(path.join(out,'tray-polish-browser-verification.json'),JSON.stringify({results,errors},null,2));ws.close();process.exit(results.some(r=>!r.pass)?1:0);
