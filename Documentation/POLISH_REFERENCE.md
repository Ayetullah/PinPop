# Smash Fest referans incelemesi ve PinPop uyarlaması

19 Kasım 2026. Kullanıcının verdiği [YouTube oynatma listesi](https://www.youtube.com/watch?v=mlSaTsozktY&list=PLZ3sjV8uAsXs), Bamsi Gaming 2.0 kanalındaki **Smash Fest All Levels Full Gameplay Walkthrough - iOS** listesini açıyor. YouTube paneli 638 video gösterdi.

## İncelenen görüntüler

- **Levels 1–30** (`mlSaTsozktY`, 12:04): videonun 0, 4, 10, 20 ve yaklaşık 35–40. saniyelerindeki ana menü, oyun, atış ve zafer görüntüleri doğrudan tarayıcıda incelendi. Videonun tamamını kapsayan 5 saniye aralıklı zaman çizelgesi kareleriyle yükleme/oyun/zafer döngüsü karşılaştırıldı.
- **Levels 31–40** (`ndIq_7UaAoA`): zaman çizelgesinde aynı ana menü ve oyun sunumu, farklı hedef düzenleri.
- **Level 99** (`m4D0A_mp_ck`) ve **Level 200** (`O1y9t8yLtN4`): zaman çizelgesinde daha yoğun hedef düzenleriyle aynı çimenli oyun alanı ve atış arayüzü.
- 60. bölüm örneğinin zaman çizelgesi erişimi 404 döndürdü; bu video incelenmiş sayılmadı. İlk videoda 80. saniye sonrasına doğrudan atlama bazı karelerde buffer'da kaldı; bu donmuş kareler sonraki zamanlara ait kanıt olarak kullanılmadı.

638 videonun tamamı tek tek izlenmedi. Görsel dil ve ekran döngüsü, yukarıdaki erken/ileri bölüm örnekleri üzerinden çıkarıldı. Referans görüntüleri yalnızca inceleme içindir; oyuna eklenmedi.

## Gözlem → uygulama

| Referansta görülen | PinPop uygulaması |
| --- | --- |
| Açık mavi gökyüzü, çimen, çadır, dönme dolap | Özgün kodla üretilen lunapark çevresi; dönen dönme dolap, balon, bulutlar, çiçekler |
| Ödül arabasında bölüm numarası ve büyük yeşil Play düğmesi | Bowling ödüllü araba, bölüm göstergesi, OYNA ve alt menü |
| Mavi-altın çerçeveli sayaçlar ve düğmeler | Hamle rozeti, ilerleme çubuğu, coin alanı, altın çerçeveler |
| Mavi kısa yükleme ekranı | Çift tıklamayı engelleyen bölüm geçişi ve kısa yükleme perdesi |
| Ateşleme parlaması/geri tepme, fiziksel dağılma | Bowling rampasında geri tepme, top izi, darbe halkaları, renk parçacıkları |
| Hedeflerin belirgin temizlenmesi | Girişte sekerek gelen dubalar, fiziksel devrilme, küçük küçülme/parçacık kapanışı |
| Büyük zafer logosu, havai fişek, konfeti, uçan coin | Özgün PINPOP başlığı, kademeli yıldızlar, havai fişek, coin uçuşu, zafer sesi |
| Dişli ile ayarlar | Gerçek fizik duraklatma, ses, hareketi azalt, tekrar başlat, ana menü |

## Korunan oyun kuralları

Bowling topları ve dubaları; yalnızca aynı renkte fiziksel etki; sınırlı hamle; her zaferde 30 coin; 10 coin ile 1 swap hakkı; başlangıçta 3 hak; swap ile **ilk topun tüm kuyruğun sonuna taşınması**; bakiye/hakların kalıcı kaydı. Referanstaki can, reklam ve güçlendirici ekonomisi eklenmedi.

## Uygulama

- **HTML:** `index.html`, internetsiz ve bağımsız Canvas oyunu. Ana menü, mağaza, ayarlar, yükleme, oyun, kazanma ve kaybetme akışları. Efektler ve ortam çizimleri kodla üretilir.
- **Unity:** aynı ekran akışı UGUI ile; HTML ortam çiziminden alınan özgün gökyüzü dokusu ve hareketli 2B/3B dönme dolap; çadır, bulut, dönme dolap ve ödül arabası prosedürel 3B modellerdir. Dünya fizikleri gerçek Rigidbody kullanır.
- Her iki sürümde ses ve hareket azaltma ayarları yerel kaydedilir; mağaza ve ayarlar ekranları oyun akışını duraklatır.

PinPop'un adı, bowling modelleri ve grafikleri korunur/yeniden üretilir; referans oyunun logosu, görüntüleri veya model dosyaları kopyalanmadı.
