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
await send('Page.navigate',{url:'file:///Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/index.html'});await wait(2000);
check(await evaluate('!!window.PinPop'),'Game boots');
await click('#play');await wait(1600);
check(await evaluate(`document.querySelector('#pullInstructions').textContent.includes('Aynı rengi')`),'Ready instruction explains color rule');
check(await evaluate(`(()=>{const s=PinPop.getState();return [...Array(3)].every((_,i)=>Number(document.querySelector(i?'#nextBall'+i:'#nextBall').dataset.color)===s.queue[i+1])})()`),'Ready preview excludes foreground ball');
await send('Input.dispatchKeyEvent',{type:'keyDown',key:'Enter',code:'Enter',windowsVirtualKeyCode:13});await send('Input.dispatchKeyEvent',{type:'keyUp',key:'Enter',code:'Enter',windowsVirtualKeyCode:13});
check(await evaluate(`(()=>{const s=PinPop.getState();return s.busy&&[...Array(3)].every((_,i)=>Number(document.querySelector(i?'#nextBall'+i:'#nextBall').dataset.color)===s.queue[i])})()`),'In-flight UI previews actual next playable balls');
await screenshot('comparison-html-flight.png');
const core=await evaluate(`(()=>{const g=new PinPop.Game({coins:0,swaps:3});g.moves=1;g.shoot({x:0,z:0},1);for(let i=0;i<24;i++)g.step(1/120);const waits=!g.finished;g.cleared=g.total;for(const p of g.pins)p.cleared=true;for(let i=0;i<18;i++)g.step(1/120);const beat=!g.finished;for(let i=0;i<44;i++)g.step(1/120);const wins=g.finished&&g.won&&g.coins===30;for(let i=0;i<120;i++)g.step(1/120);return {waits,beat,wins,once:g.coins===30}})()`);
for(const [k,v]of Object.entries(core))check(v,'Final shot: '+k);
await wait(4400);check(await evaluate(`(()=>{const s=PinPop.getState();return !s.busy&&Number(document.querySelector('#nextBall').dataset.color)===s.queue[1]})()`),'Ready preview restores after flight');
check(errors.length===0,'No browser runtime exceptions');
await fs.writeFile(path.join(out,'comparison-browser-verification.json'),JSON.stringify({results,errors},null,2));ws.close();if(results.some(r=>!r.pass))process.exitCode=1;
