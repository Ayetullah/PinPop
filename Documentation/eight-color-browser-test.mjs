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
await send('Runtime.discardConsoleEntries');await send('Runtime.enable');await send('Page.enable');await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await send('Page.navigate',{url:pathToFileURL(path.resolve(out,'../index.html')).href});if(!await until('typeof PinPop!=="undefined"',30000))throw Error('Boot failed');await click('#play');await until('PinPop.getState().screen==="game"&&!PinPop.getState().transitioning');await evaluate(`(()=>{const step=PinPop.Game.prototype.step;PinPop.Game.prototype.step=function(dt){window.eightFixture=this;return step.call(this,dt)}})()`);await wait(100);
for(const level of [0,2,4,6,8,10,11]){await evaluate('eightFixture.reset('+level+');eightFixture.onChange?.()');await wait(250);check(await evaluate('PinPop.getState().colorCount')===Math.min(8,3+Math.floor(level/2)),'Chapter '+(level+1)+' unlocked palette');await screenshot('eight-color-browser-level-'+(level+1)+'.png');}
await evaluate(`(()=>{eightFixture.reset(10);for(let c=0;c<8;c++){const item=eightFixture.tray.collect(c,{x:0,y:0,z:0});eightFixture.onCollect?.(item)}})()`);await wait(1000);check(await evaluate('PinPop.getState().tray.live===8&&document.querySelectorAll(".collection-token").length===8'),'Eight distinct rendered tokens');check(await evaluate('[...document.querySelectorAll(".collection-token")].every(x=>getComputedStyle(x).backgroundImage.includes("data:image/png"))'),'All eight use real pin icons');await screenshot('eight-color-browser-collected.png');
for(const [width,height] of [[320,740],[390,844],[1000,600]]){await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await wait(250);check(await evaluate('(()=>{const r=document.querySelector("#collectionDock").getBoundingClientRect();return r.left>=-1&&r.right<=innerWidth+1})()'),'Eight color HUD fits '+width);await screenshot('eight-color-browser-'+width+'.png');}
check(errors.length===0,'No browser exceptions');await fs.writeFile(path.join(out,'eight-color-browser-verification.json'),JSON.stringify({results,errors},null,2));ws.close();process.exit(results.some(r=>!r.pass)?1:0);
