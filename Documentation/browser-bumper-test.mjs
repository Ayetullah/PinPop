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
await send('Runtime.enable');await send('Page.enable');await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await send('Page.navigate',{url:'http://127.0.0.1:8765/index.html?v=bumpers'});await wait(700);errors.length=0;await evaluate("localStorage.clear();location.reload();true");await wait(700);await click('#play');await wait(1700);await click('#swap');
const before=await evaluate('PinPop.getState()');const p=await evaluate(`(()=>{const p=PinPop.getLaunchPoint(),r=document.querySelector('#board').getBoundingClientRect();return{x:r.x+p.x*r.width/600,y:r.y+p.y*r.height/1000}})()`);
await send('Input.dispatchMouseEvent',{type:'mousePressed',...p,button:'left',clickCount:1});await send('Input.dispatchMouseEvent',{type:'mouseMoved',x:p.x+60,y:p.y+125,buttons:1,button:'left'});await send('Input.dispatchMouseEvent',{type:'mouseReleased',x:p.x+60,y:p.y+125,button:'left',clickCount:1});
let impact=null;for(let i=0;i<120;i++){impact=await evaluate('(()=>{const s=PinPop.getState();return s.contacts.some(c=>c.pinColor!==c.ballColor)?s:null})()');if(impact)break;await wait(16)}
check(!!impact,'Real wrong-color UI shot makes physical contact');check(impact?.pins.some(p=>p.flash>0&&p.locked),'Wrong-color UI contact flashes the fixed pin');check(impact?.cleared===0,'Wrong-color UI shot awards no cleared pins');await screenshot('wrong-color-rebound.png');await wait(3500);
const after=await evaluate('PinPop.getState()');check(after.pins.filter(p=>p.color===0).every((p,i)=>Math.hypot(p.x-before.pins[i].x,p.z-before.pins[i].z)<.001&&!p.cleared),'Wrong-color group remains at original positions');check(after.moves===5&&after.swaps===2,'Rebound spends one move and preserves remaining swap credits');check(after.pins.every(p=>p.flash===0),'Temporary block flash fades');check(errors.length===0,'Block feedback raises no renderer or JavaScript errors');
await fs.writeFile(path.join(out,'bumper-browser-verification.json'),JSON.stringify({date:new Date().toISOString(),passed:results.filter(r=>r.pass).length,total:results.length,checks:results,contact:impact?.contacts[0],errors},null,2));console.log('Results',results.filter(r=>r.pass).length,results.length);ws.close();process.exitCode=results.every(r=>r.pass)?0:1;
