# PinPop · ortak popup polish

Güncel kural: vazgeçme/kapatma × ile yapılır; başarılı yardımcı kullanımı popup’ı kapatıp oyunu sürdürür. [Yardımcı akışı](../HelperUseClose/README.md). [Kapatma politikası ve kontroller](../CrossOnlyPopups/README.md).

Son dil seçici ve lunapark popup çalışmasının görsel dili bütün uygulama içi pencere ailelerine yayıldı. Çadır çatısı, krem/altın yüzeyler, ışıklı çerçeve, kahverengi metinler, dairesel × kapatma, sabit dokunma alanı ve basma geri bildirimi ortak bileşenlerden üretilir. Başarı/başarısızlık ekranlarının mor ve mercan vurguları korunur. Haritanın bölge çizimleri korunarak çerçevesi ve kontrolleri uyarlanır.

| Pencere ailesi | Uyarlama ve örnek |
| --- | --- |
| Ana menü/oyun ayarları | Altın kenarlı ses satırı, dil seçimi ve yardımcı bağlantılar. [Ana menü](settings-home-tr-390.png), [oyun](settings-tr-390.png) |
| Dil listesi | Tam satır dokunma, dönen ok ve seçili dil onayı. [Liste](language-list-tr-390.png) |
| Top mağazası | İki sütunlu kartlar, net top görselleri, fiyat/sahiplik durumu, seçili kenarlık ve pasif eylem. [Mağaza](shop-tr-390.png) |
| Swap satın alma | Büyük swap rozeti, belirgin fiyat teklifi, ana satın alma butonu ve yetersiz bakiye/limit bilgisi. [Swap](swap-tr-390.png) |
| Renk seç | Sekiz rengin tamamını destekleyen görselli seçim alanı. [Renk](helper-0-tr-390.png) |
| Gökkuşağı | Envanter rozeti, simge, ayrı kullan/satın al eylemleri. [Gökkuşağı](helper-1-tr-390.png) |
| Tepsi cebi | Dokuz bekleyen figüre kadar seçim, boş/dolu cep bilgisi. [Seçim](helper-2-tr-390.png), [boş cep](pocket-empty-tr-390.png) |
| +3 hamle | Kullanma ve satın alma eylemlerinin açık ayrımı. [Hamle](helper-3-tr-390.png) |
| Mekanik tanıtımları | Sekme, buz, bağlı üçlü ve hareketli bariyer; mevcut işleyişi gösteren animasyonlar ortak çerçevede. [Sekme](lesson-0-tr-390.png), [buz](lesson-1-tr-390.png), [bağ](lesson-2-tr-390.png), [bariyer](lesson-3-tr-390.png) |
| Renk/karakter açılışı | Bölümün açtığı gerçek duba görseliyle tanıtım. [Açılış](lesson-4-tr-390.png) |
| Nasıl oynanır | Üç animasyonlu adım, kaydırma, sayfa göstergesi ve güvenli kapatma. [Nişan](howto-0-tr-390.png), [eşleşme](howto-1-tr-390.png), [swap](howto-2-tr-390.png) |
| Bölge haritası | Altın kenarlık, yuvarlak kapatma, bölge ve sandık eylemlerinde ortak yüzey. [Harita](map-tr-390.png) |
| Video hazırlığı/yerel önizleme | Ortak çerçeve, yuvarlatılmış video alanı, ilerleme/durum bilgisi, istek sürerken doğru iptal durumu. [Video](video-preparing-tr-390.png) |
| Sonuç, yeniden başlatma, özellik ilerlemesi | Mevcut lunapark ekranlarıyla aynı kontrol/pasif durum sistemi. [Başarı](success-tr-390.png), [hata](failure-tr-390.png), [yeniden başlat](restart-tr-390.png), [özellik](progress-tr-390.png) |

Coin bilgisi üstte ortak simge ve kısaltılmış değerle verilir. Alt çerçeveye sıkışan yinelenen mağaza/yardımcı bakiye satırları kaldırıldı. Satın alma fiyatları, gerçek sahiplik, envanter ve ödül kuralları mevcut oyun modelinden gelir.

**Sonuç: 4.432 kontrol geçti, 0 hata.** Hareket azaltma oyun ayarının ve eski kaydının kaldırılması da doğrulandı.

## Davranış ve doğrulama

Yerel Unity UI kullanılır; WebView eklenmedi. Metinler `CrispText` ile ekran yoğunluğunda çizilir. Tüm yeni eylemler en az 61 tasarım birimi dokunma yüksekliğine sahiptir; 320×568'de en az 44 piksel karşılığı korunur. Kapatma hedefleri görünür simgeden büyük ve sabittir. Kullanılamayan butonların metinleri ve ikonları da yüzeyleriyle birlikte pasifleşir.

Giriş animasyonu, ışıklar, öğretici ve mekanik demolar ölçeklenmemiş zaman kullanır. Oyunda azaltılmış hareket seçeneği ve buna ait kayıt bulunmaz. Sistem erişilebilirlik tercihi otomatik olarak gözetilir. Gerçek oyun duraklatılırken öğretici animasyonu ilerler; canlı hamle/coin değerlerine dokunmaz.

`CarnivalUIReview` ve `AllPopupReview`, Türkçe/English olarak 320×568, 390×844 ve iPhone 13 güvenli alanını temsil eden 1170×2532 boyutlarını inceler. EventSystem raycast ile gerçek butonlara basılır. Satın alma tutarı, ücretsiz stil seçimi, swap'ın × beklemesi, yardımcı kullanımı/satın alma, boş/yetersiz envanter, ana menü öğreticisinden ayarlara dönüş, ders kapatma, harita, video iptali, gerçek ödül ve sonraki bölüm akışı kontrol edilir. Denetim sonunda oyuncunun önceki kayıt ve tercihleri geri yüklenir.

[Doğrulama raporu](verification.json) · [Derleme kaydı](build.log) · [Kaynak eşleşmesi](source-sha256.json) · [Popup envanteri](popup-inventory.json)

Bu kontroller macOS Unity oynatıcısında yapılır. Fiziksel iPhone testi ve canlı reklam yayını testi değildir. AdMob'un kendi reklam/izin formları SDK tarafından çizilir; uygulamanın video hazırlık penceresi ve reklam tercihine giriş kontrolü uyarlanmıştır. Reklam kimlikleri veya ödül şartları değiştirilmedi.

## Tekrar çalıştırma

Unity'de `CarnivalUIReviewBuild.Export` çalıştırın; ardından Development oynatıcıyı `-carnival-ui-test -all-popups-test` parametreleriyle başlatın. `PINPOP_QA_DIR` çıktı klasörünü seçer. İnceleme bileşenleri üretim derlemesinde devreye girmez.

Temel uygulama dosyaları: `CarnivalPopup.cs`, `PopupControlState.cs`, `AdventurePopups.cs`, `HtmlReferenceHUD.cs`, `HtmlCrispHUD.cs`, `AnimatedHowTo.cs`, `PinPopRewardedVideo.cs`, `CarnivalMapView.cs`.
