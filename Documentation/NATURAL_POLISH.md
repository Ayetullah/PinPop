# Görsel tutarlılık düzenlemesi

Kullanıcının gönderdiği ekran görüntüsünde düz lunapark çizimleri, güçlü plastik parlamalar, sonradan eklenmiş rampanın perspektifi ve kalın altın çerçeveler aynı görsel dilde görünmüyordu.

## HTML

- Düz 2B sahne yerine gerçek 3B WebGL sahnesi; tek perspektif kamera ve tek ışık düzeni.
- Yuvarlatılmış ahşap masa, ahşap dokusu, ince metal detaylar ve gerçek masa ayakları.
- İnceltilmiş bowling duba profili, kısıtlı yüzey parlaması, aynı geometride renk işaretleri.
- Işığı ve gölgeyi paylaşan 3B çevre; uzak geometride hafif alan derinliği.
- Sonradan eklenmiş gibi görünen rampa kaldırıldı. Top masanın önünden doğal olarak yuvarlanır.
- Mat yüzeyli, daha az çerçeveli ve daha kontrollü kontrastlı arayüz.
- Perspektif nişan dönüşümü ve dar ekranlarda masayı kadraja sığdıran kamera uzaklığı.
- Coin, hak, kuyruk, fiyat ve bölüm ilerleme kuralları korunur.

Three.js gömülüdür ve çalışma anında dış kaynak istenmez. Lisans HTML içine ve üçüncü taraf bildirimine eklenmiştir. Prosedürel çevre ve modeller özgündür.

## Unity

- Aynı duba oranları ve sakinleştirilmiş renk/malzeme paleti.
- Düz gökyüzü katmanı ve üstüne eklenen 2B dönme dolap kaldırıldı; mevcut 3B ortam ve ortak ışık kullanılır.
- Arayüz çerçeveleri ve parlak renkler hafifletildi.

## Doğrulama

`natural-browser-verification.json`: 49/49 geçti. Gerçek atışlar, pause/resume, kazanma/kaybetme, ödül, alışveriş, kalıcı kayıt, GPU/Shader hataları, internet olmadan file:// açılışı ve farklı pencere oranları kontrol edildi. Nişan projeksiyonu/ters projeksiyon testi ayrıca çalıştırıldı. Test ortamında ölçülen kare hızı yaklaşık 30 FPS; bu bir cihazlar arası performans garantisi değildir.

Güncel HTML görüntüleri `natural-*.png` dosyalarındadır. Önceki `polish-*.png` dosyaları önceki sürüme aittir. Unity raporları: `verification.txt`, `polish-unity-verification.txt`, `visual-verification.txt`.
