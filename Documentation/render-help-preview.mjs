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




await send('Runtime.enable');await send('Page.enable');await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});
const state=await evaluate('PinPop.getState()');if(state.modal)throw Error('Close test modal first');if(state.highestLevel>0)throw Error('Use a fresh dedicated test profile');if(state.screen==='game'){await click('#settings');if(state.reduced)await click('#motion');await click('#goHome')}else if(state.reduced){await click('#homeSettings');await click('#motion');await click('#resume')}
const priorSeen=await evaluate('localStorage.getItem("pinpop-html-howto-seen-v1")');await evaluate('localStorage.removeItem("pinpop-html-howto-seen-v1")');await click('#play');await click('#mapLevel0');if(!await until('PinPop.getState().modal==="helpModal"',5000))throw Error('Automatic tutorial did not open');
const data=await evaluate(`new Promise((resolve,reject)=>{const canvas=document.querySelector('#helpDemo'),stream=canvas.captureStream(30),recorder=new MediaRecorder(stream,{mimeType:'video/webm;codecs=vp9',videoBitsPerSecond:1800000}),chunks=[];recorder.ondataavailable=e=>{if(e.data.size)chunks.push(e.data)};recorder.onstop=()=>{for(const track of stream.getTracks())track.stop();const file=new FileReader();file.onload=()=>resolve(file.result);file.onerror=reject;file.readAsDataURL(new Blob(chunks,{type:'video/webm'}))};recorder.start();setTimeout(()=>recorder.stop(),18300)})`);
await fs.writeFile(path.join(out,'automatic-help-preview.webm'),Buffer.from(data.split(',')[1],'base64'));if(!await until('PinPop.getState().modal!=="helpModal"',3000))throw Error('Tutorial did not finish');await evaluate(priorSeen===null?'localStorage.removeItem("pinpop-html-howto-seen-v1")':'localStorage.setItem("pinpop-html-howto-seen-v1",'+JSON.stringify(priorSeen)+')');console.log('Saved automatic tutorial preview');ws.close();
