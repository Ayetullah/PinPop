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
async function click(selector){const p=await evaluate(`(()=>{const r=document.querySelector(${JSON.stringify(selector)}).getBoundingClientRect();return{x:r.x+r.width/2,y:r.y+r.height/2}})()`);await send('Input.dispatchMouseEvent',{type:'mousePressed',...p,button:'left',clickCount:1});await send('Input.dispatchMouseEvent',{type:'mouseReleased',...p,button:'left',clickCount:1});await wait(100)}


async function until(expression,limit=15000){const start=Date.now();while(Date.now()-start<limit){if(await evaluate(expression))return true;await wait(100)}return false}



await send('Runtime.enable');await send('Page.enable');await send('Emulation.setFocusEmulationEnabled',{enabled:true});await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await send('Page.addScriptToEvaluateOnNewDocument',{source:"localStorage.clear();localStorage.setItem('pinpop-html-howto-seen-v1','1')"});await send('Page.navigate',{url:pathToFileURL(path.resolve(out,'../index.html')).href});if(!await until('typeof PinPop!=="undefined"',30000))throw Error('Boot failed');await click('#play');await click('#mapLevel0');if(!await until('PinPop.getState().screen==="game"&&!PinPop.getState().transitioning'))throw Error('Game failed');
async function anchor(){return evaluate('(()=>{const p=PinPop.getLaunchPoint(),r=document.querySelector("#board").getBoundingClientRect();return{x:r.left+p.x*r.width/600,y:r.top+p.y*r.height/1000}})()')}
async function mouse(type,x,y){await send('Input.dispatchMouseEvent',{type,x,y,button:'left',buttons:type==='mouseReleased'?0:1,clickCount:1})}
for(const [width,height]of[[600,1000],[320,740],[1000,600]]){
 await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await wait(300);let a=await anchor();const before=await evaluate('PinPop.getState().moves'),beforeX=await evaluate('PinPop.getState().launch.x');await mouse('mousePressed',a.x,a.y);await mouse('mouseMoved',a.x+width*.15,a.y);await wait(50);
 check(await evaluate('PinPop.getState().positioning&&PinPop.getState().launch.x>('+beforeX+')&&PinPop.getState().power===0'),'Horizontal drag moves right without charge '+width);
 await mouse('mouseMoved',width-1,a.y+30);await mouse('mouseReleased',width-1,a.y+30);check(await evaluate('PinPop.getState().launch.x===3.65&&PinPop.getState().launch.z===-7.2&&PinPop.getState().launch.y===.4&&!PinPop.getState().busy&&PinPop.getState().moves==='+before),'Clamp and release preserve front rail and moves '+width);
 a=await anchor();await mouse('mousePressed',a.x,a.y);await mouse('mouseMoved',1,a.y);await mouse('mouseReleased',1,a.y);check(await evaluate('PinPop.getState().launch.x===-3.65&&!PinPop.getState().busy'),'Horizontal drag clamps left '+width);
 await screenshot('launch-position-browser-'+width+'.png');
}
await send('Emulation.setDeviceMetricsOverride',{width:600,height:1000,deviceScaleFactor:1,mobile:false});await wait(300);await click('#swap');check(await evaluate('PinPop.getState().launch.x===-3.65'),'Swap preserves selected position');
// Move away from the edge with real touch input; releasing must not shoot.
let a=await anchor();await send('Input.dispatchTouchEvent',{type:'touchStart',touchPoints:[{x:a.x,y:a.y}]});await send('Input.dispatchTouchEvent',{type:'touchMove',touchPoints:[{x:a.x+150,y:a.y}]});await wait(100);await send('Input.dispatchTouchEvent',{type:'touchEnd',touchPoints:[]});check(await evaluate('PinPop.getState().launch.x>-2&&!PinPop.getState().busy&&PinPop.getState().moves===12'),'Touch positioning moves only horizontally without spending moves');
a=await anchor();const origin=await evaluate('PinPop.getState().launch');await mouse('mousePressed',a.x,a.y);await mouse('mouseMoved',a.x,a.y+110);await wait(100);check(await evaluate('PinPop.getState().power>.7&&!PinPop.getState().positioning&&Math.abs(PinPop.getState().launch.x-('+origin.x+'))<.001'),'New vertical gesture charges at selected position');await mouse('mouseReleased',a.x,a.y+110);await wait(20);check(await evaluate('PinPop.getState().moves===11&&PinPop.getState().busy&&Math.abs(PinPop.getState().balls[0].x-('+origin.x+'))<.03'),'Release shoots once from moved position');check(errors.length===0,'No browser runtime errors');await fs.writeFile(path.join(out,'launch-position-browser-verification.json'),JSON.stringify({results,errors},null,2));ws.close();process.exit(results.some(x=>!x.pass)?1:0);
