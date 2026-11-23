# Masa üzerinde yuvarlanan bowling topu

## Oynanış

Top artık çimen üzerinden masaya kavis çizerek fırlamaz. Masanın ön tarafında yüzeye oturur, geriye çekilip bırakılınca yüzey boyunca dönerek ilerler. Bu davranış hem bağımsız HTML/WebGL hem yerel Unity oyununda uygulanmıştır.

- Başlangıç: `(0, 0.40, -7.2)`; top yarıçapı `0.39`, fiziksel masa yüzeyi `y=0`.
- Çekiş sırasında merkez `x=-0.75…0.75`, `z=-8.3…-7.2` içinde tutulur. Top masa kenarından geriye düşmez; parmak daha uzağa çekilse de güç göstergesi çalışır.
- Geri çekiş gücü belirler. Yana çekişin tersi yönde nişan alınır; yatay nişan aralığı `x=-3.85…3.85`.
- İlk hız yalnızca yataydır: `4…12`. Kütle güçle değişmez: top `6`, duba `1` fizik birimidir. Başlangıç dönmesi hız/yarıçap oranından hesaplanır; ilerleyen hareketi ve temasları fizik motoru çözer.
- Yukarı ilk hız ve düşüş noktasını bulmak için kullanılan parabol kaldırılmıştır. Duba temasından sonraki doğal küçük sıçramalar, sekmeler ve masadan düşme fiziksel olarak mümkündür.
- Her geçerli bırakış tek hamledir. Dokunma, yetersiz çekiş, iptal ve odak kaybı hamle harcamaz.

## Nişan ve görünüm

27 renkli nokta masanın hemen üzerinde (`y=0.035`) düz bir rehber oluşturur. Rehber ilk duba veya masa kenarında biter; dubaların arkasından geçecekmiş gibi bir yol göstermez. Bu, hareket öncesi yön rehberidir; çarpışma sonrası sekmelerin garantili tahmini değildir. Bırakışta rehber kapanır. Havada düşüş alanı ve “TAHMİNİ DÜŞÜŞ” etiketi kullanılmaz.

Kamera masa yüzeyini daha geniş gösterir: Unity konumu `(0,13,-25)`, bakış hedefi `(0,0.2,-2)`; HTML'de aynı kadraj dünya koordinatlarına çevrilmiştir. Arka planın alan derinliği yeni mesafeye göre ayarlanmıştır. Nişan sırasında renk yönergesi “PEMBE dubaları hedefle · Yuvarla” gibi görünür.

Renk filtreleri, temas açısına göre sekmeler, sesler, parçacıklar, darbe halkaları, sonuç ekranları, kayıtlar, SWAP maliyeti ve bölüm hamle bütçeleri korunur. Yükleme perdesinin 1,4 saniyelik okunabilir süresi korunur.

## Kaynaklar

- `Assets/Scripts/PinPopGame.cs`: yatay ilk hız, masa üstü tutuş, yön rehberi, kamera.
- `Assets/Scripts/BowlingBall.cs`: sabit bowling kütlesi, fiziksel dönüş ve temaslar.
- `Assets/Scripts/HtmlReferenceHUD.cs`, `HtmlLook.cs`: canlı yönergeler, eski düşüş etiketinin kaldırılması ve odak mesafesi.
- `index.html`: karşılık gelen Cannon-es / Three.js uygulaması.

## Doğrulama ve kapsam

`node Documentation/physics-core-test.cjs`, gömülü gerçek Cannon-es kodunu ve güncel oyun sınıfını dosyadan okuyarak momentum, enerji kaybı, sağ/sol sekme, renk kilidi, yatay ilk hız ve yüzey üzerinde dönme kontrollerini çalıştırır.

`node --experimental-websocket Documentation/browser-rolling-test.mjs`, ayrı Chrome CDP oturumunda (9224) dosyayı doğrudan açar. Gerçek fare ve dokunma olayları, bırakış, iptal, odak kaybı, duraklatma, top sırası ve 390×844 / 320×740 / 1000×600 ekranları kontrol edilir. İlk altı bölüm gerçek Cannon-es simülasyonuyla özgün hamle sınırları içinde tamamlanır. `browser-pull-test.mjs` ve `browser-landing-test.mjs` yeni teste yönlenir.

Unity uygulama bayrakları:

- `-pull-test`: yüzeye oturma, dönme, güç farkı, renk filtresi, masa üzerinde tutuş, düz rehber, bırakış ve son hamle/ödül.
- `-self-test`: gerçek PhysX çarpışmalarıyla ilk bölümün tamamlanması, hamle sınırı, sonuç, ekonomi ve kayıtlar.
- `-rolling-levels-test`: 18 ve 24 dubalı farklı düzenlerin (bölüm 2, 4 ve 6) gerçek PhysX simülasyonuyla mevcut hamle sınırları içinde tamamlanması. Her düzen için normal başlangıç cüzdanı kullanılır; kullanıcının kayıtları test sonunda geri yüklenir.
- `-collision-test`, `-polish-test`, `-transition-test`, `-sync-test`, `-video-test`: temas, ekran akışı, yükleme UI yaşam döngüsü, kayıtlar ve yüksek çözünürlükte kontrol regresyonları.

Görseller: `unity-roll-aim.png`, `unity-roll-motion.png`, `rolling-html-aim.png`, `rolling-html-touch.png`. Fizik motorlarının aynı nişanda birebir aynı sayıda duba devirmesi beklenmez; her iki sürüm için kendi gerçek simülasyonu doğrulanır.

## İlk yuvarlanma sürümünün sonucu

343/343 kontrol geçti. Ayrıntılı dağılım: `rolling-verification.json`. Test günlüklerinde MissingComponentException, ArgumentException veya NullReferenceException görülmedi. Güncel macOS uygulaması: `Builds/PinPop.app`.

## Top ve dubalar arasındaki mesafe

Yaklaşma alanı dört birim uzatıldı. Masa uzunluğu 9,6 birimden 13,6 birime çıkarıldı; fizik yüzeyi z=-8,8…4,8 aralığını kaplar. Duba düzenleri yerinde kaldı, top z=-7,2 konumuna taşındı. İlk sıraya kadar 7 birim boş yaklaşma alanı vardır (önceki değer 3 birimdi). Kamera masanın tamamını ve belirgin boşluğu gösterecek şekilde geri alındı. Uzun mesafede güçlü atışların ileri sıralara momentum taşıması için en yüksek yatay hız 12 olarak dengelendi.

Mesafe değişikliği doğrulaması: **200/200 kontrol geçti**. Unity başlangıç ve ileri bölümler, masa sınırları, çekiş/nişan, yüksek çözünürlük; tarayıcıda ilk altı bölüm, dokunma ve üç farklı kadrajda görünür boşluk doğrulandı. Ayrıntı: `spacing-verification.json`; görseller: `01-ready.png`, `spacing-html-600.png`, `spacing-html-390.png`.
