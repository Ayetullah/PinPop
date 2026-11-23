# Resimli lunapark haritası

Toplam **40 bölüm**, her biri **20 bölümlük iki ayrı bölge haritasında** sunulur. Lunapark Kapısı 1–20, Dönme Dolap 21–40 bölümlerini gösterir. Her harita kendi başlığını, arka planını, dört sandığını ve /20 ilerlemesini kullanır. Üstteki bölge sekmeleri açılmış bölgelere geçer; ikinci bölge 20. bölüm tamamlanmadan açılamaz. İlk 20 özgün ikon ve tüm 40 durak adı korunur. Bölüm kilitleri, yıldızlar, cüzdan ve sandık hakları mevcut kayıtta tutulur.

| Bölge | Görsel kimlik | Hareket | Duraklar |
| --- | --- | --- | --- |
| Lunapark Kapısı | Gün batımı, ışıklı giriş kapısı, bilet gişeleri ve çiçekler | Sallanan balonlar ve parıldayan ampuller | Bilet Gişesi, Balon Sokağı, İlk Tampon, Şeker Durağı, Buzlu Geçit |
| Dönme Dolap | Turkuaz kabinli büyük dönme dolap, pembe/mor gökyüzü | Dönen çark; kabinler dik durur | Gökyüzü Turu, Altın Kabin, Rüzgâr Gülü, Bulut Yolu, Gökkuşağı |
| Oyun Çadırı | Kırmızı/krem çadır, oyun tezgâhları ve ödül rafları | Rüzgârda hareket eden bayraklar | Büyük Çadır, Halka Atışı, Ördek Avı, Güç Kulesi, Ödül Rafı |
| Yıldız Festivali | İndigo gece, altın ışıklar, yıldızlı festival sahnesi | Havai fişekler ve yıldız parıltıları | Yıldız Yolu, Roket Rampası, Ay Işığı, Işık Şöleni, Büyük Kupa |

## Arayüz

- Kavisli yol seçili bölgenin 20 durağını kesintisiz bağlar. Açılan güzergâh altın renkle belirginleşir. İlk ve son durak dahil mevcut bölüm harita açılırken ekranın ortasına getirilir. Fare tekerleği, sürükleme ve doğal dokunmatik kaydırma desteklenir. Kaydırma bir durak üzerinden başlasa da bölüm açılmaz. Sabit başlık ve sayaç, hareketli yolun üstünde kalır; başlık arkasındaki duraklar gizlenir. Manuel kaydırma ve ekran boyutu değişiminde konum korunur. Harita kapatılıp yeniden açıldığında mevcut bölüme dönülür.
- Mevcut bölüm titreşimsiz, hafif bir nefes animasyonu ve BURADASIN etiketiyle gösterilir. Hit alanı sabit kalır.
- Her durakta tema ikonu, bölüm numarası ve kısa gerçek mekanik özeti vardır. Kilitli ikonların kimliği renkleri hafifçe azaltılarak korunur; kilit simgesi eklenir.
- Tamamlanan duraklar mevcut en iyi sonuçtan gelen yıldızları gösterir. Eski tamamlanmış kayıtlarda sonuç ayrıntısı yoksa bir yıldız kullanılır.
- Cüzdan ve bölge ilerlemesi küçük etiketlerde yer alır. Büyük bakiyeler mevcut kompakt sayı biçimini kullanır.
- 5, 10, 15, 25, 30 ve 35. bölümlerdeki ara sandıklar ile 20 ve 40. bölümlerdeki bölge sandıkları, eski tek seferlik coin ve güçlendirici ödüllerini verir. Alınmış sandık tekrar ödemez. Son bölümden sonra haritaya dönülür ve tamamlanma mesajı gösterilir; 41. bölüm eklenmez.
- Küçük ekranlarda metinler kısaltılır; düğme çizimleri büyütülmeden dokunma alanları en az 44 piksel tutulur. 320, 390 ve yatay ekranlar kontrol edilir.
- Hareket azaltma açıkken dekoratif hareket, parıltı ve nabız animasyonları durur. Normal modda oyun duraklatılsa da harita unscaled saat ile canlı kalır. Kapatılan haritanın HTML animasyon döngüsü ve ölçüm gözlemcisi temizlenir.

## Varlıklar ve kaynaklar

Dört arka plan yerleşik **image_gen** aracıyla üretildi. Seçilen PNG çıktıları değiştirilmeden projeye kopyalandı:

- `Assets/Resources/MapArt/region-gate.png`
- `Assets/Resources/MapArt/region-wheel.png`
- `Assets/Resources/MapArt/region-tent.png`
- `Assets/Resources/MapArt/region-festival.png`

Tam üretim promptları ve kaynak dosya eşlemeleri [carnival-map-generation.json](carnival-map-generation.json) içinde kayıtlıdır.

20 bölüm ikonu ve iki ortak kilit/sandık ikonu, projeye özel SVG çizimleridir: `Assets/ArtSource/carnival-map-art.js`. Unity için 192×192 şeffaf PNG çıktıları `Assets/Resources/MapArt/Icons/` altındadır. `MapArtImporter` bunları Sprite olarak, okunabilir CPU dokusu gerektirmeden içe aktarır. Görselden `GetPixels32` ile okuma yapılmaz.

Bölge adları, paletler ve ikon eşlemelerinin kaynağı `Assets/Resources/MapArt/catalog.json` dosyasıdır. Unity `CarnivalMapView` ve `CarnivalMapGraphic`, HTML `carnival-map-view.js` aynı katalog ve durak koordinatlarını kullanır. HTML illüstrasyonları ve SVG kaynakları `index.html` içine gömülür; dosya çevrimdışı açılabilir.

## Bakım ve doğrulama

- `Documentation/sync-carnival-map.py`: katalog, SVG kaynakları, arayüz kodu ve arka planları HTML içine günceller.
- `Documentation/export-carnival-icons.mjs`: ayrı test Chrome'unda SVG'leri Unity PNG ikonlarına aktarır.
- `Documentation/carnival-map-browser-test.mjs`: tek haritada 20 benzersiz ikon, ilk/orta/son bölüm ortalaması, gerçek fare ve dokunmatik kaydırma, /20 ilerleme, animasyon/hareket azaltma, kilitler, küçük ekranlar, sandık ve bölüm seçimi.
- Unity `-map-polish-test`: gerçek kaynak yükleme, maskeli düğme raycastleri, 20 ikon, yıldız/kilit/ödül, 44 piksel hedefler ve taşmayan metinler.
- Mevcut güçlendirici/ödül tarayıcı testleri ve otomatik ilk-bölüm rehber testi regresyon olarak çalıştırılır.

Son ekran görüntüleri `carnival-map-unity-region-*.png`, `carnival-map-browser-region-*.png` ve küçük ekran varyantlarıdır. Testler kullanıcı cüzdanını, macera ve rehber kaydını yedekleyip geri yükler.

Güncel kampanya doğrulaması Unity `-forty-level-test` ve `forty-level-browser-test.mjs` ile yapılır. Eski yirmi bölümlük raporlar tarihsel kayıttır. Yeni testler bütün 40 dizilimin Unity–HTML eşitliğini, eski ödülleri, kayıt geçişini ve son bölüm davranışını kontrol eder.

## Tek parça kaydırılabilir harita

Unity `CarnivalMapView.MapScroll` standart dikey `ScrollRect` kullanır. HTML `#mapScroll` doğal dokunmatik kaydırma ve tekerlek kaydırmasına ek olarak fareyle sürüklemeyi destekler. Sürükleme sonundaki tıklama bastırılır. Dış HTML dialog `overflow:clip` kullanır; odak hareketi yalnızca iç haritayı etkileyebilir. Yeniden açılışta mevcut bölüm bir kez ortalanır; animasyon veya ölçüm döngüsü konumu sürekli sıfırlamaz.

Yol konumları Unity `CarnivalMapCatalog.ScrollStop` ve HTML `mapStop` ile eşlenir. İlk ve son durakta 410 mantıksal piksel üst/alt boşluk bulunur; 820 piksel yüksekliğindeki görünür alanın ortası bütün ilerleme seviyelerinde kullanılabilir. Her beş bölümde bir sandık için ilave boşluk bırakılır. Sandık hakları ve ödüller mevcut kayıt kimliklerini kullanır.

Unity `-map-polish-test` ve tarayıcı `carnival-map-browser-test.mjs`, 600×1000, 320×740 ve 1000×600 boyutlarında 1, 20, 21 ve 40. bölümün ortalanmasını, bölge başına 20 durağı, dört sandığı ve bölge sekmelerini, kaydırmayı, tekrar açılışı ve bölüm seçimini sınar. Güncel ekran görüntüleri `scroll-map-*.png` dosyalarıdır.
