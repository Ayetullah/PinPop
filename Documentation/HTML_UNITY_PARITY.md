> Güncel atış değişikliği: top masada yuvarlanır; kamera, kütle ve nişan rehberi için [ROLLING_SHOT.md](ROLLING_SHOT.md). Aşağıdaki aktarım notları önceki incelemelerin kaydını da içerir.

# HTML → Unity görünüm aktarımı

20 Kasım 2026. Referans: projenin `index.html` sürümü. HTML dosyası bu aktarım sırasında değiştirilmedi.

## Aktarılanlar

- HTML sahnesinin masa, ödül arabası, çimen, tepeler, ağaçlar, çadırlar ve dönme dolabı aynı geometri, konum, renk ve UV bilgileriyle Unity Mesh nesnelerine aktarıldı.
- Perspektif, kadraj, ana menü kamera kaydırması, uzaklık sisi, alan derinliği ve ACES ton eşleme uyarlandı. Güneş şiddeti iki motorun farklı ışık hesaplarına göre ekran örnekleriyle ayarlandı.
- Ana menü, oyun, mağaza, iki ayarlar durumu, zafer ve kaybetme ekranları için HTML/CSS görselleri yerel Unity varlıklarına dönüştürüldü. Düğmeler gerçek Unity Button ve raycast kullanır; sayaçlar, renk kuyruğu, güç, ilerleme ve ekonomi canlı oyun modeline bağlıdır.
- 600×1000, 390×844, 320×740, 1000×600, 800×1000, 1440×900 ve 600×700 arayüz profilleri; yükleme perdesi, üç top önizlemesi, düşüş etiketi, canlı bölüm tabelası ve ayar düğmesi durumları eklendi.
- Oyun Unity Rigidbody/PhysX üzerinde çalışmaya devam eder. Arayüz tarayıcı veya WebView içermez.

## Görsel karşılaştırma

`html-parity-home.png` ↔ `unity-html-home.png`; `html-parity-game.png` ↔ `unity-html-game.png`.

Aynı 600×1000 kadrajda gökyüzü örneği HTML/Unity RGB **199,213,216**, aydınlık çimen **166,195,131**, hamle rozeti zemini **88,123,150** olarak ölçüldü. Bu örnekler tüm görüntünün piksel düzeyinde aynı olduğu anlamına gelmez. Dinamik font rasterizasyonu, gölge kenarları ve malzemelerin yansımalarında Three.js ile Unity arasında küçük farklar kalabilir. Fizik motorları da aynı temas sonucunu piksel düzeyinde üretmez.

**Güncel Unity kontrolleri: 195/195 geçti.** Ayar ve HTML eşleştirmesi 49, fizik/ekonomi 23, çekiş ve düşüş önizlemesi 34, temas/sekme 36, ekran akışı 36, pencere oranları 8, bowling geçişi 9. Son derleme başarılı; koşu günlüklerinde script exception veya shader derleme hatası bulunmadı.

Son ekran kontrolleri `unity-html-*.png`, oynanış/akış raporları `verification.txt`, `pull-unity-verification.txt`, `collision-unity-verification.txt`, `polish-unity-verification.txt` ve `visual-verification.txt` içindedir.

Hazır uygulama: `Builds/PinPop.app`. Unity sürümü: 6000.3.24f1. Android/iOS paket üretimi ve fiziksel cihaz testi bu aktarımın parçası değildir.


## HTML ayar ve davranış eşleştirmesi

Güncel HTML dosyası bu çalışmada değiştirilmedi. Referans SHA-256: `0114f150bde407e3995eafe7b4ad2a7d20cf808b3d8fe98d1dd1f792842f6582`.

| HTML özelliği | Unity karşılığı ve doğrulama |
| --- | --- |
| Ses KAPALI varsayılanı, açık/kapalı düğmesi ve kayıt | `HtmlGameAudio`, `PinPopGame.ReloadSettings`; düğme görseli, kaydı ve ses kanalı testi |
| Sistem hareket azaltma tercihi ve kalıcı oyun içi seçim | `HtmlPlatformPreferences`; macOS, Windows, iOS ve Android okuma köprüleri; açık oyun içi tercih önceliklidir. macOS çalışma zamanı doğrulandı; diğer platform paketleri bu koşuda üretilmedi. |
| Hareket azaltırken sade geçiş, sabit lunapark, animasyonsuz sayaç/yıldızlar | `BowlingTransition`, `HtmlReferenceWorld`, `HtmlReferenceHUD`; devam eden giriş/pencere animasyonunun hemen bitmesi testi |
| Ana menü, oyun, mağaza, ayarlar, kazanma ve kaybetme | Gerçek Unity Button/raycast; duraklat/devam/tekrar/ana menü ve sonuç üstüne ayar açılmaması testleri |
| Ayar/mağaza/sonuç penceresi açıkken fizik durması | `Time.timeScale`; sonuç sırasında fiziksel konumun sabit kaldığı test edildi. UI ve kutlama efektleri ölçeklenmemiş zaman kullanır. |
| 30 coin ödülü, 10 coinlik SWAP, 3 başlangıç hakkı, kuyruk döndürme, kalıcı bakiye | Oynanış/ekonomi testleri; yeniden başlatma, yetersiz bakiye ve ödülün tekrarlanmaması |
| Büyüyen cüzdanlar, mağaza toplamları ve sayı animasyonu | Esnek cüzdan çerçeveleri, canlı `Coin`/`Swap` satırları; 12345 coin ve 123 hakla görüntü ve alım testleri |
| Bölüm düzenleri, renkler, hamle sayıları ve ilerleme | Aynı başlangıç düzeni ve deterministik varyasyon formülleri; güncel HTML yapılandırma örnekleri `html-settings-reference.json` içinde |
| Çek-bırak, klavye yön/güç, iptal, odak kaybı | Aynı eşikler; odak kaybında hamle harcamadan iptal, geçersiz sayıları reddetme, UI odağında çift klavye işlemini önleme |
| Nişan noktaları, güç, düşüş alanı ve etiket sınırları | 27 renkli nokta, .39 top yarıçapı, HTML etiket sınırları ve uçuştaki gösterim; 34 çekiş kontrolü |
| Fizik parametreleri ve renk kuralı | 1/120 adım, yerçekimi -9.8, 16 çözücü adımı; gövde/boyun/baş bileşik temas şekilleri, tüm farklı renk çiftlerini ayırma; `HtmlPhysicsContacts` ile dört malzeme çiftinin sürtünme/sekme değerleri |
| Çimen zemini ve masanın yan kenarları | HTML ile aynı gizli fizik yüzeyleri; yuvarlanan topun fiziksel dönüşü Unity'nin varsayılan açısal hız sınırına takılmaz. |
| Duba giriş sırası, devrilme, darbe halkası ve kaybolma | Görsel katman fizik gövdesinden ayrıdır; .014 gecikmeli giriş, .85 + .25 kaybolma, blok parlaması, temas halkaları ve parçacıklar |
| Az hamle uyarısı, sonuç metinleri, yıldızlar, coin uçuşu, havai fişek | Canlı metinler ve bakiye, son iki hamlede sarı renk, sırayla açılan yıldızlar ve UI kutlama katmanları |
| Farklı ekranlar ve pencere boyutunu değiştirme | HTML uygulama genişliği/yüksekliği sınırları, yedi görsel profil; sonuç içeriğinin yeniden boyutlandırmada korunması |
| Unity editörü ve derlemede aynı doku ayarları | `HtmlReferenceTextureImporter`; tabela/rakam okuma izinleri, özgün doku boyutları, sıkıştırmasız UI ve tutarlı import metadatası |

Güncel ekranlar: `sync-unity-*.png` ve aynı durumdaki `sync-html-*.png`. Genel rapor: `html-unity-sync-verification.json`. Fiziksel temaslar PhysX ile hesaplanır; Cannon-es ile aynı malzeme değerleri kullanılması iki motorun her karede aynı sayısal sonucu üretmesini garanti etmez.
