// Original wraparound print artwork. Canvas authoring source shared by HTML and Unity PNGs.
function createPinSkinCanvas(index, characterImages) {
 const canvas=document.createElement('canvas');canvas.width=512;canvas.height=1024;
 const c=canvas.getContext('2d'),colors=['#df5f84','#32a6ba','#8c6ac4','#e8bc32','#e98a3d','#55aa66','#397ac7','#d84f48'],color=colors[index];
 c.fillStyle='#f3f1e8';c.fillRect(0,0,512,1024);
 const coat=c.createLinearGradient(0,550,0,1000);coat.addColorStop(0,color);coat.addColorStop(1,['#c74670','#258d9f','#7354af','#bc8f1d','#c36326','#38844c','#24579a','#b53331'][index]);c.fillStyle=coat;c.fillRect(0,553,512,471);
 // Subtle finish variation, not a baked lighting highlight.
 for(let x=0;x<512;x++){c.fillStyle=`rgba(255,255,255,${.004+.004*(1+Math.sin(x*17.3))})`;c.fillRect(x,0,1,1024)}
 c.fillStyle=color;c.fillRect(0,278,512,21);c.fillRect(0,333,512,21);
 c.fillStyle='#f0dfbb';c.fillRect(0,550,512,4);c.fillStyle='#46505b';c.fillRect(0,1000,512,24);c.fillStyle='#a9a495';c.fillRect(0,998,512,2);
 // Character art is preloaded and printed directly onto the curved lower body.
 c.drawImage(characterImages[index],181,520,150,380);
 c.textAlign='center';c.fillStyle='#fff5de';c.font='bold 20px Arial';c.fillText('PINPOP',256,940);c.font='bold 16px Arial';c.fillText('0'+(index+1),256,971);
 return canvas;
}
