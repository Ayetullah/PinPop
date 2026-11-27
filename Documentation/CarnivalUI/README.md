# PinPop · Unity lunapark ekranları

Güncel tüm-popup çalışması ve ayarlar: [AllPopups](../AllPopups/README.md). Hareket azaltma oyun ayarı kaldırılmıştır.

`Design/CarnivalUI` içindeki altı onaylı HTML ekranının yerel Unity UI uyarlaması. Oyun içi ekranlar bir WebView kullanmaz; metin, butonlar, anahtarlar ve ilerleme canlı Unity bileşenleridir.

| HTML | Unity akışı |
| --- | --- |
| Bölüm tamamlandı | Gerçek zafer, kazanılan yıldızlar, gerçek bölüm/hamle ödülü ve varsa bölge sandığı |
| Tekrar dene | Gerçek başarısızlık; yeniden deneme ve mevcut ödüllü video sağlayıcısı üzerinden +3 hamle |
| Yeniden başlat | Ayarlardan açılan onay; iptal ayarlara döner, onay aynı bölümü bir kez yükler |
| Ayarlar | Kalıcı ses ve Türkçe/English; mevcut animasyonlu nasıl oynanır ekranı; gerekli olduğunda reklam tercihleri |
| Özellik açıldı | Bölüm 7'de mevcut Bağlı Üçlü dersini sunar; kapanınca ders zinciri/oyun devam eder |
| Özellik ilerlemesi | İlk altı bölümün zaferinden sonra, Bölüm 7'ye gerçek ilerleme; tamamlanan veya öğretilen özellik için tekrar gösterilmez |

Başarı, hata ve özellik ekranlarının çadır çatısı, altın süsleri, ışıklı mor çerçevesi, yeşil/mercan butonları ve ikonları HTML çizimlerinden alınmıştır. Girişler, yıldızlar, ışıklar, dönen yeniden başlatma oku, duba hareketi ve aşağıdan yukarı renklenen özellik göstergesi ölçeklenmemiş zamanla çalışır. Sistem erişilebilirlik tercihinde sabit görünürler; oyunda hareket azaltma ayarı bulunmaz. Başarıda mevcut havai fişek ve coin efektleri korunur.

Metinler `CrispText` ile son ekran yoğunluğunda çizilir. Dekoratif sprite'lar 3× çözünürlükte, sıkıştırmasız, mipmap olmadan içe alınır. Yerleşim güvenli alan içinde 390×780 tasarım oranını korur. Küçük ekranlarda da etkileşim alanları en az 44 piksel; Retina boyutunda ölçekli karşılığı kadar büyüktür.

## Önizlemeler

| Ekran | Türkçe | English |
| --- | --- | --- |
| Başarı | [Görsel](success-tr-390.png) | [Görsel](success-en-390.png) |
| Başarısızlık | [Görsel](failure-tr-390.png) | [Görsel](failure-en-390.png) |
| Yeniden başlat | [Görsel](restart-tr-390.png) | [Görsel](restart-en-390.png) |
| Ayarlar | [Görsel](settings-tr-390.png) | [Görsel](settings-en-390.png) |
| Özellik açılışı | [Görsel](unlock-tr-390.png) | [Görsel](unlock-en-390.png) |
| Özellik ilerlemesi | [Görsel](progress-tr-390.png) | [Görsel](progress-en-390.png) |

[Gerçek bölüm 7 açılışı](unlock-real.png) · [Gerçek ödül ekranı](success-real.png) · [Doğrulama raporu](verification.json)

Son doğrulama: **1.223 geçti, 0 hata**. Oyuncu kayıtları geri yüklendi. Derlenen kaynaklar `source-sha256.json` ile eşleşir.

## Doğrulama ve tekrar çalıştırma

`CarnivalUIReviewBuild.Export` Development macOS inceleme derlemesini `/tmp/PinPopCarnivalReview.app` konumuna üretir. `-carnival-ui-test` parametresi yalnız geliştirme derlemesinde `CarnivalUIReview` denetimlerini etkinleştirir. `PINPOP_QA_DIR` çıktı klasörünü belirler. Bu denetim, eski HTML raster ekranlarının kopya/metin yerleşim denetimlerinin yerine yeni altı ekranın referansıdır.

Denetim 320×568, 390×844 ve iPhone 13'ün 1170×2532 piksel/güvenli alan düzenini Türkçe ve İngilizce inceler. Aktif ekranlar gerçek Unity kameralarından yakalanır; görsellerdeki düğmelere EventSystem raycast ile basılır. Yalnız tercih yüksekliği değil, metinlerin gerçekten glyph mesh'i üretmesi de kontrol edilir. Gerçek zafer/ödül, özellik ekranından sonraki bölüme geçiş, onaylı yeniden başlatma, gerçek başarısızlık, video isteği iptali ve yeniden deneme ayrıca çalıştırılır. Denetim sonunda önceki oyuncu tercihleri ve kayıtları geri yüklenir.

Bu doğrulama macOS Unity oynatıcısındadır; fiziksel iPhone veya canlı AdMob yayını testi değildir. Canlı reklam kimlikleri ve mevcut reklam modu değiştirilmedi.

## Dosyalar

- `Assets/Scripts/CarnivalPopup.cs`: ortak görünüm, güvenli alan, animasyon ve yerel kontroller.
- `Assets/Scripts/HtmlCarnivalFlow.cs`: yeniden başlatma ve özellik ilerlemesi bağlantıları.
- `HtmlCrispHUD.cs`, `HtmlReferenceHUD.cs`, `PinPopHUD.cs`, `AdventureHUD.cs`: mevcut akışlara entegrasyon.
- `Assets/Resources/CarnivalUI`: HTML'den türetilen dekoratif sprite'lar.
- `Assets/Editor/CarnivalArtImporter.cs`: mobilde sıkıştırma bulanıklığını önleyen içe alma ayarları.
- `Tools/export_carnival_art.py`: HTML SVG çizimlerinden yeniden üretim; yalnız tasarım aracı olarak `resvg-py` gerektirir. Oyun bu Python aracına ihtiyaç duymaz.
