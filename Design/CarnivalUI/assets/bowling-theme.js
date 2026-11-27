(()=>{'use strict';
function fit(){
 const view=document.querySelector('.viewport'),stage=document.querySelector('.stage');if(!view||!stage)return;
 const css=getComputedStyle(view),left=parseFloat(css.paddingLeft)||0,right=parseFloat(css.paddingRight)||0,top=parseFloat(css.paddingTop)||0,bottom=parseFloat(css.paddingBottom)||0;
 stage.style.setProperty('--fit',Math.min((view.clientWidth-left-right)/390,(view.clientHeight-top-bottom)/780));
 stage.style.left=`calc(50% + ${(left-right)/2}px)`;stage.style.top=`calc(50% + ${(top-bottom)/2}px)`;
}
function apply(){
 document.body.classList.add('bowling-preview');
 const art=document.querySelector('.festival-scene');if(art)art.innerHTML='';
 const canopy=document.querySelector('svg.canopy');if(canopy){const emblem=document.createElement('img');emblem.className='canopy bowling-emblem';emblem.src='assets/bowling-emblem.png';emblem.alt='';emblem.setAttribute('aria-hidden','true');emblem.draggable=false;canopy.replaceWith(emblem);}
 if(location.pathname.endsWith('/bowling-tema.html')){document.title='PinPop · '+(document.documentElement.lang==='en'?'Bowling theme':'Bowling teması');}
 fit();
}
new MutationObserver(apply).observe(document.body,{childList:true});addEventListener('resize',fit);apply();
})();
