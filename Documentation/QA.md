# PinPop doğrulama

Unity 6000.3.24f1, Apple M3 / macOS 15.5; HTML sürümü Chrome'da kontrol edildi.

## Yükleme ekranı görünürlük süresi

Unity'de hareket azaltma açıkken 0,12 saniyelik hızlı geçiş kaldırıldı. Her iki modda da ilk çizilen kareden sonra en az 1,4 saniye beklenir; hareket azaltma açıkken ekran sabit kalır. Animasyon ve akış aynı süre sabitini kullanır. Yeniden boyutlandırmada başlangıç zamanı korunur. Son derlemede normal/statik mod, süre, giriş kilidi, raycast, yeniden boyutlandırma ve ekran akışı kontrolleri **58/58 geçti** (geçiş 22, akış 36). İlgili ekran: `transition-unity-reduced.png`.

## Yerel oynanış videosu incelemesi

86,17 saniyelik `Screen Recording 2026-11-20 at 16.40.02.mov` kaydı incelendi. 9:16 Game View'deki boş alt alan, düğme görsellerindeki opak dikdörtgenler ve bölüm yazısının tahta çıtaları arkasında kalması düzeltildi. Unity görünümü artık fiziksel piksel boyutundan bağımsız olarak tam yüksekliği kullanır; geniş ekranlarda yalnızca yan çerçeve korunur. 540×960 için 2x görsel profil eklendi; dilimlenmiş cüzdanlar da görsel yoğunluğunu dikkate alır.

**127/127 çalışma zamanı kontrolü geçti:** kayıttaki 13. bölüm ve akışlar 54, ayar/UI 49, geçiş/raycast 16, responsive 8. Boş kaydedilmiş tahmini düşüş/masa dışı etiketleri de yeniden üretildi; gerçek GPU piksel okuması ve açık/kapalı halka görüntüsü farkıyla çizildikleri doğrulandı. Ayrıca 32/32 mağaza düğme görselinin köşelerinde şeffaflık doğrulandı. Koşu günlüklerinde script exception bulunmadı. Masaüstü pencere yöneticisinin yükseklik sınırına takılmayan 864×1536 ve 972×1728 boyutlarında gerçek oynanış; 1080×1920, 1440×2560 ve 2160×3840 için de tam 9:16 yerleşim hesabı doğrulandı.

Önce/sonra kareler `video-review-*.png`, ayrıntılı bulgu ve sonuçlar `video-review-verification.json` içindedir. Kullanıcının kaydı değiştirilmedi; coin, SWAP ve ilerleme kayıtları testlerden sonra geri yüklendi.

## CanvasRenderer düzeltmesi

Özel yükleme ve sonuç grafiklerine zorunlu `CanvasRenderer` eklendi; etkinleştirmede eksik bileşen onarılır. Yeniden boyutlandırmada eski arayüz düğümleri silinmeden önce devre dışı bırakılarak raycast kayıtları kaldırılır. Son derlemede **52/52 kontrol geçti**: yükleme/raycast/yeniden boyutlandırma 16, ekran akışı 36. Koşu günlüklerinde MissingComponentException bulunmadı.

## Güncel HTML ayar ve davranış aktarımı

**195/195 geçti:** ayar/UI eşleştirmesi 49, fizik/ekonomi 23, çekiş 34, temas/sekme 36, ekran akışı 36, responsive 8, bölüm geçişi 9. Bu koşunun toplu raporu `html-unity-sync-verification.json` içindedir. Aynı durumun HTML ve Unity görüntüleri `sync-html-*.png` ve `sync-unity-*.png` dosyalarıdır.

## HTML → Unity görünüm aktarımı (20 Kasım 2026)

**136/136 geçti:** fizik/ekonomi 23, çekiş 33, temas/sekme 36, ekran akışı 36, responsive 8. `HTML_UNITY_PARITY.md` aktarım kapsamını ve görsel karşılaştırma sınırlarını açıklar. Güncel kareler `unity-html-*.png`; macOS uygulaması yeniden derlendi.

## Güncel düşüş alanı ve çizgi film baskıları

- **Düşüş alanı tarayıcı 15/15**: `landing-browser-verification.json`. Yön ve güçle hesaplanan nokta, masadaki büyük alan, uçuş boyunca görünürlük, çarpışmada gizlenme, iptal ve üç ekran oranı doğrulandı.
- **Fizik çekirdeği 49/49**: `collision-core-verification.json`. Düşük/yüksek güç ve gerilmiş başlangıç için tahmin, gerçek motorun ilk masa temasıyla karşılaştırılır. Uçuş yolu top yarıçapı yüksekliğinde biter; masa dışı durum saptanır.
- **Unity sıra/çekiş/düşüş 33/33**, **temas 36/36**, **ekran 8/8**. Büyük alan, ekran etiketi, doğru tahmin yönü, iptalde gizlenme ve ilk hava bölümünde devam etme kontrol edildi.
- Üç karakter yerleşik image_gen ile üretildi. Ham PNG'ler 1254×1254 ve şeffaftır. HTML gömülü veriyi, Unity aynı kompozisyondan üretilen duba dokusunu kullanır. Ayrıntı/promptlar: `CHARACTER_ASSETS.md`.
- Görseller: `cartoon-pins-ready.png`, `landing-area-aim.png`, `landing-area-390.png`, `unity-pull-aim.png`. Eski şimşek/dalga/taç baskısı ekranları tarihsel görsellerdir.

## Önceki üç top sırası ve baskılı duba görünümü

- **Tarayıcı görünüm 7/7**: `pin-presentation-verification.json`. Üç baskı ve yumuşak geometri, ilk sıra, swap sonrası üç rengin güncellenmesi, 390×844 / 320×740 / 1000×600 oranları ve GPU/JavaScript sağlığı kontrol edildi.
- **Tarayıcı oyun 66/66 + sekme 7/7**: üçlü sıra, atış/swap sonrası renkler ve azalan hamlelerde önizleme sayısı mevcut fizik/ekonomi akışlarıyla birlikte doğrulandı.
- **Unity sıra/çekiş 27/27**, **temas 36/36**, **ekran 8/8**: üç resim, sıra renkleri, kalan hamle 3/2/1/0/6 için önizleme sınırları, UV koordinatları, üç PNG'nin runtime yüklenmesi, yeni yüzeyde emisyon parıltısı ve ekran raycast'leri kontrol edildi.
- Baskılar Canvas kodundan üretildi; HTML CanvasTexture ile, Unity aynı çıktının PNG dosyalarıyla çalışır. İnternet gerekmez.
- Görseller: `printed-pins-ready.png`, `printed-pins-detail.png`, `printed-pins-390.png`. Kaynak ve tasarım: `PIN_PRESENTATION.md`.

## Temas açısı ve fizik altyapısı

- **Fizik çekirdeği 39/39**: `collision-core-verification.json`. İzole sağ/orta/sol çarpışma, yatay momentum korunumu, enerji kaybı, simetri ve merkez vuruş kontrol edildi. Gerçek HTML içindeki Cannon-es ve Game kodu kullanılır. Farklı renk için sabit duba, açılı ve merkez sekmesi, hız kaybı, parıltı, hamle/puan koruması ve doğru renkle tekrar açılma da kontrol edilir.
- **Tarayıcı 60/60**: `collision-browser-verification.json`. Temas normaline göre sapma, momentum aktarımı, renk filtresi, gerçek mouse/touch çekişleri, iptal, pause/resume, hamle bütçesi içinde kazanma, coin/swap ve kayıt doğrulandı.
- **Gerçek ekran sekmesi 7/7**: `bumper-browser-verification.json`. Fareyle yapılan farklı renk atışında fiziksel temas, parıltı, sabit konum, sıfır temizlenen duba, tek hamle ve parıltının sönmesi doğrulandı.
- **Unity açı/temas 36/36**: `collision-unity-verification.txt`. Üç farklı ofsetle gerçek Rigidbody çarpışması; aynı renk momentum aktarımı, farklı renkte sabit engelden sekme, hız kaybı, temas parıltısı ve doğru renkle yeniden vurma doğrulandı.
- **Unity mevcut oyun akışları 85/85**: çekiş 18, fizik/ekonomi 23, ekran akışı 36 ve responsive 8. Tümü yeni dinamik dubalarla çalıştırıldı.
- `browser-test.mjs` güncel `browser-collision-test.mjs`, `browser-bumper-test.mjs` ve `browser-presentation-test.mjs` ve `browser-landing-test.mjs` dosyalarını sırayla çalıştırır. Eski `browser-pull-test.mjs` ve raporu, önceki sabit etki modelinin tarihsel kayıtlarıdır.
- Masa sahnesi, yay çizerek atış, SWAP yanındaki sonraki top ve ekonomi korunur. Fizik artık temas noktasından hesaplanır; kuvvet yüzdesi sabit bir duba sayısına çevrilmez.
- Uygulama ve HTML çevrimdışı çalışır. WebGL context/shader hatası bulunmadı. Geçerli kütüphane lisansları `third-party/` klasöründedir.

## Önceki doğal görünüm düzenlemesi

- **HTML: 49/49 geçti** (`natural-browser-verification.json`). Gerçek WebGL renderer, gölge haritası, GPU/Shader sağlığı, perspektif nişan dönüşümü, dar ekranlarda masanın kadraja sığması ve mevcut oyun/ekonomi/ekran akışları kontrol edildi.
- Güncel görseller `natural-*.png` dosyalarıdır. Three.js HTML içine gömülüdür; çalışma anında dış dosya/CDN gerekmemesi ve doğrudan `file://` açılışı doğrulandı.
- Unity sürümünde de malzeme/renkler ve duba oranları düzenlendi; düz gökyüzü/dönme dolap katmanı kaldırıldı. Güncel Unity kontrolleri aşağıdaki aynı rapor dosyalarına yazılır.
- Görsel değişikliklerin gerekçesi: `NATURAL_POLISH.md`.

## Önceki lunapark sürümünün doğrulamaları

- **Tarayıcı: 42/42 kontrol geçti** (`polish-browser-verification.json`). Ana menü, mağaza, ses/hareket azaltma kaydı, korunan yükleme geçişi, atış, atış sırasında gerçek duraklatma, kazanma, coin ödülü, alım, ana menüye dönüş ve kaybetme/tekrar deneme gerçek UI etkileşimleriyle doğrulanır.
- Responsive boyutlar: 390×844, 320×740, 1000×600. Yatay taşma ve temel düğmelerin görünürlüğü kontrol edildi. `file://` üzerinden doğrudan, internetsiz açılış doğrulandı.
- **Unity fizik/ekonomi: 23/23 geçti**, `verification.txt`: gerçek Rigidbody renk kuralı, aynı renkte zincir, kuyruk döndürme, hak tüketimi, coin ödülü, satın alma, yetersiz bakiye/hak, tekrar deneme ve PlayerPrefs.
- **Unity ekran akışı: 36/36 geçti**, `polish-unity-verification.txt`: gerçek UI raycast/tıklama, ana menü, mağaza, ayarlar, yükleme kilidi, oyuna giriş, pause/resume, ana menü ve retry.
- **Unity ekran oranları: 8/8 geçti**, `visual-verification.txt`: dört pencere boyutunda swap raycast/tıklaması.

Son Unity derlemesinde üç doğrulama koşusu da başarıyla tamamlandı; raporlar yukarıdaki dosyalardadır. macOS uygulaması açık Unity editörünü kapatmadan aynı kaynakların geçici proje kopyasında derlenip `Builds/PinPop.app` konumuna aktarıldı.

## Görseller

- `polish-home.png`, `polish-game.png`, `polish-shop.png`, `polish-settings.png`, `polish-loading.png`, `polish-impact.png`, `polish-victory.png`, `polish-failure.png`: HTML ekranları.
- `polish-game-390.png`, `polish-game-320.png`, `polish-game-1000.png`: responsive HTML oyunu.
- `unity-polish-*.png`: son Unity ekran akışı koşusundan menü, mağaza, ayarlar, yükleme ve oyun.
- `01-ready.png`–`05-retry.png`: Unity fizik testinden oyun ve sonuç durumları.
- `06-phone.png`, `07-tablet.png`, `08-landscape.png`: Unity pencere oranları.

Önceki sürümlerin `html-*.png` ve `html-verification.json` dosyaları önceki görünümün kayıtlarıdır. Güncel tarayıcı raporu `natural-browser-verification.json` dosyasıdır. `browser-test.mjs`, güncel `browser-natural-test.mjs` betiğine yönlendirir. `polish-*` raporları önceki sunumun kayıtlarıdır.

Fiziksel Android/iOS cihaz testi ve mobil paket üretimi yapılmadı. Referans incelemesinin kapsamı ve sınırları `POLISH_REFERENCE.md` dosyasındadır.
