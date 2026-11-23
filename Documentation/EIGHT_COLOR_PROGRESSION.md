# Sekiz renk ve figür

Bölüm 1–2: 3 renk; 3–4: 4; 5–6: 5; 7–8: 6; 9–10: 7; 11 ve sonrası: 8.

| Renk | Figür |
| --- | --- |
| Pembe | Tavşan |
| Turkuaz | Penguen |
| Mor | Kedi |
| Sarı | Aslan |
| Turuncu | Ayı |
| Yeşil | Kurbağa |
| Mavi | Fil |
| Kırmızı | Tilki |

İlk iki bölüm korunur. Sonraki bölümlerde her aktif renkten altı duba bulunur; her renk iki üçlü eşleşme oluşturur. Tek numaralı dahili seviyelerde komşu gruplar üçlüler halinde karışır. Hamle bütçesi duba sayısına eşittir. 10 kapasiteli toplama alanı ve mevcut swap/video/coin akışı korunur. Kayıt şeması değişmedi; mevcut üç renk kimliği, satın alımlar ve cüzdan korunur.

Unity ve HTML aynı renk paletini, bölüm dağılımını ve top sırasını kullanır. Fizik katmanları renklerden bağımsızdır; yalnızca atış rengiyle eşleşen dubalar dinamikleşir. Sekiz renk için top malzemeleri, üç sıradaki topun önizlemesi, güç çubuğu, konfeti ve duba ikonları desteklenir.

Yeni figürler yerleşik image generation aracıyla ayrı şeffaf PNG olarak üretildi. İlk üç figür korundu. Kaynaklar `Assets/Art/Characters/`, kaplamalar `Assets/Resources/PinSkins/`, gerçek Unity duba meshinden üretilen toplama ikonları `Assets/Resources/PinIcons/` altındadır. Kaplama tuvali `Assets/ArtSource/pin-skins.js` içinde tutulur; HTML bunların gömülü kopyalarını kullanır.

## Kullanılan üretim promptları

Her figüre aşağıdaki ortak prompt uygulandı; `${subject}` yerine tabloda belirtilen cümle konuldu:

```
Use case: stylized-concept. Create one production game decal asset: ${subject}, original friendly cartoon animal mascot bust, smiling open mouth, very large expressive white eyes with dark navy pupils and highlights, thick clean dark navy outlines, glossy soft cel shading with subtle gradients, cream muzzle, one small raised waving paw/hand visible. Straight-on centered face and upper torso, large head, compact narrow portrait silhouette, fully contained with a little clear margin. Polished cheerful mobile bowling game print art matching a pink rabbit and turquoise penguin mascot set. No accessories, no text, no letters, no logo, no watermark, no extra characters, no floor or shadow. Genuinely transparent background, clean alpha, single mascot only.
```

| Dosya | Subject |
| --- | --- |
| lion.png | a golden yellow lion cub with a fluffy round amber mane |
| bear.png | a warm orange honey-brown bear cub with round ears |
| frog.png | a bright leafy green frog with prominent round eyes |
| elephant.png | a cornflower blue elephant calf with big rounded ears and a short curved trunk |
| fox.png | a red-orange fox with large triangular ears and cream cheek tufts |

## Doğrulama

- `eight-color-model-test.cjs`: 40 bölüm, üçlü adetleri, top sırası, renk bağımsız fizik katmanları ve sekizinci renk düşme/eşleşme testi.
- `eight-color-browser-test.mjs`: 7 bölüm eşiği, sekiz farklı gerçek ikon, 320/390/1000 genişlikleri, çalışma zamanı hata kontrolü.
- Unity `-eight-color-test`: 32 bölüm, 8 kaynak kaplama/ikon, 32 top önizlemesi, gerçek düşme, 8 farklı figürün birlikte tutulması, 10 doluluk yenilgisi ve sekizinci renkte üçlü.

Son çalıştırma: Unity sekiz renk testi 66/66, çimen/HUD regresyonu 25/25; HTML model testi 85/85 ve tarayıcı testi 13/13 geçti. Bölüm 1, 3, 5, 7, 9, 11 ve 12 ekranları `eight-color-level-*.png` ve `eight-color-browser-level-*.png` dosyalarındadır.
Unity atış/eşleşme/kazanma regresyonu 44/44 ve mevcut HTML eşleşme regresyonu 33/33 geçti.
