from pathlib import Path
import json,base64
root=Path(__file__).resolve().parents[1];p=root/'index.html';h=p.read_text();a=h.index('// CARNIVAL_MAP_SOURCE_BEGIN') if '// CARNIVAL_MAP_SOURCE_BEGIN' in h else h.index('// Code-native icons: one original attraction');b=h.index('fillCatalog();resetLevel(',a)
backgrounds={id:'data:image/png;base64,'+base64.b64encode((root/('Assets/Resources/MapArt/region-'+id+'.png')).read_bytes()).decode() for id in ['gate','wheel','tent','festival']}
source='// CARNIVAL_MAP_SOURCE_BEGIN\nconst CARNIVAL_BACKGROUNDS='+json.dumps(backgrounds)+';\n'+(root/'Assets/ArtSource/carnival-map-art.js').read_text()+'\nconst CARNIVAL_CATALOG='+json.dumps(json.loads((root/'Assets/Resources/MapArt/catalog.json').read_text()),ensure_ascii=False)+';\n'+(root/'Assets/ArtSource/adventure-ui.js').read_text()+'\n'+(root/'Assets/ArtSource/carnival-map-view.js').read_text()+'\n// CARNIVAL_MAP_SOURCE_END\n'
p.write_text(h[:a]+source+h[b:])
