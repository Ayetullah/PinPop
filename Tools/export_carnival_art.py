#!/usr/bin/env python3
"""Export the approved HTML's decorative SVGs. Text and controls stay native Unity UI.
Requires resvg-py (authoring only), never needed by the game/player.
"""
import re
from pathlib import Path
import resvg_py
ROOT=Path(__file__).resolve().parents[1]
source=(ROOT/'Design/CarnivalUI/assets/ui.js').read_text()
svg=source.split('const symbols=`',1)[1].split('`;',1)[0]
defs=svg.split('<defs>',1)[1].split('</defs>',1)[0]
gradients=defs.split('<symbol',1)[0]
out=ROOT/'Assets/Resources/CarnivalUI';out.mkdir(parents=True,exist_ok=True)
def render(name,body,w,h):
 text=f'<svg xmlns="http://www.w3.org/2000/svg" width="{w*3}" height="{h*3}" viewBox="0 0 {w} {h}"><defs>{gradients}</defs>{body}</svg>'
 (out/(name+'.png')).write_bytes(resvg_py.svg_to_bytes(svg_string=text))
for name in ['canopy','heart','restart','star','coin','wheel','volume','motion','video','lock']:
 match=re.search(r'<symbol id="'+name+r'" viewBox="([^"]+)">(.*?)</symbol>',defs,re.S)
 vb,body=match.groups();_,_,w,h=map(int,vb.split());render(name,body.replace('currentColor','#d0bad8'),w,h)
for name,cream,fail in [('panel',False,False),('panel-fail',False,True),('panel-settings',True,False)]:
 border='#fff0c0' if cream else '#e1a2cb' if fail else '#d79cf5'
 outer=['#f4d598','#d3ac72'] if cream else ['#a36a98','#81466f'] if fail else ['#9856c7','#67328f']
 inner=['#fff7df','#f1e2bf'] if cream else ['#825273','#41263f'] if fail else ['#775198','#352344']
 body=f'''<defs><linearGradient id="outer" x2=".7" y2="1"><stop stop-color="{outer[0]}"/><stop offset="1" stop-color="{outer[1]}"/></linearGradient><radialGradient id="inner" cy=".3" r=".9"><stop stop-color="{inner[0]}"/><stop offset="1" stop-color="{inner[1]}"/></radialGradient></defs>
 <rect x="2" y="8" width="318" height="358" rx="25" fill="#170c2c"/>
 <rect x="2" y="2" width="318" height="358" rx="25" fill="url(#outer)" stroke="{border}" stroke-width="3"/>
 <rect x="7" y="7" width="308" height="348" rx="20" fill="none" stroke="{border}" stroke-opacity=".35" stroke-width="3"/>
 <rect x="12" y="12" width="298" height="338" rx="16" fill="url(#inner)" stroke="{border}" stroke-opacity=".35"/>
 <rect x="16" y="16" width="290" height="330" rx="13" fill="none" stroke="{border}" stroke-opacity=".1" stroke-width="4"/>'''
 render(name,body,322,370)
for name,colors,shadow in [('button-green',['#b6f35b','#83d824','#64c300','#4ca900'],'#274909'),('button-coral',['#ffb2a0','#ff7a87','#f55377','#e53d64'],'#7c1b45')]:
 body=f'''<defs><linearGradient id="btn" x2="0" y2="1"><stop stop-color="{colors[0]}"/><stop offset=".42" stop-color="{colors[1]}"/><stop offset=".44" stop-color="{colors[2]}"/><stop offset="1" stop-color="{colors[3]}"/></linearGradient></defs><rect x="1" y="6" width="230" height="60" rx="19" fill="{shadow}"/><rect x="1" y="1" width="230" height="60" rx="19" fill="url(#btn)" stroke="#f8df89" stroke-width="2"/><path d="M15 5Q116 0 217 5" fill="none" stroke="#fff" stroke-opacity=".6" stroke-width="2"/>'''
 render(name,body,232,68)
print('Exported',len(list(out.glob('*.png'))),'decorative sprites to',out)

render('binding', '<rect x="1" y="9" width="176" height="20" rx="7" fill="#79502d"/><rect x="1" y="6" width="176" height="20" rx="7" fill="url(#gold)" stroke="#f8d879" stroke-width="2"/><path d="M5 11h169" stroke="#ffefb4" stroke-width="3"/><rect x="72" y="1" width="34" height="31" rx="5" fill="url(#gold)" stroke="#ffdb69" stroke-width="2"/><rect x="78" y="6" width="22" height="21" rx="2" fill="#9b5f26"/>',178,34)
render('plaque', '<rect x="1" y="4" width="242" height="46" rx="10" fill="#9a6c66"/><rect x="1" y="1" width="242" height="46" rx="10" fill="url(#white)" stroke="#fff0bd" stroke-width="2"/>',244,51)
render('ribbon', '<defs><linearGradient id="ribbon" x2="0" y2="1"><stop stop-color="#c179e9"/><stop offset=".65" stop-color="#803ab6"/><stop offset="1" stop-color="#ab5dd0"/></linearGradient></defs><rect x="1" y="5" width="223" height="50" rx="13" fill="#3b164f"/><rect x="1" y="1" width="223" height="50" rx="13" fill="url(#ribbon)" stroke="#e7b8ff" stroke-width="2"/><path d="M12 5h201" stroke="#edb3fb" stroke-opacity=".55" stroke-width="2"/>',225,57)
# Background art has no text: its low-contrast grid and radial color are fixed.
body='<defs><radialGradient id="scene" cx=".5" cy=".45" r=".7"><stop stop-color="#50325c"/><stop offset=".6" stop-color="#30203f"/><stop offset="1" stop-color="#21192e"/></radialGradient><pattern id="grid" width="25" height="25" patternUnits="userSpaceOnUse"><path d="M25 0H0V25" fill="none" stroke="#b6a4d5" stroke-opacity=".032" stroke-width="1"/></pattern></defs><rect width="390" height="780" fill="url(#scene)"/><rect width="390" height="780" fill="url(#grid)"/>'
render('backdrop',body,390,780)
