# PinPop — Color Bowling Club

Unity 6.3 (6000.3.24f1) ile hazırlanmış, renk eşleştirmeli 3B fizik bulmacası.

## Oynanış ve ekran incelemesi

Ana menüden ve oyun içinden **?** ile Nasıl oynanır ekranı açılır. Yardım ve ayarlar oyunu duraklatır. Klavye nişanı seçilen gücü gösterir; Esc nişanı iptal eder veya duraklatır. Tam yolculuk, küçük ekranlar ve 12 farklı bölüm düzeni için inceleme: `Documentation/PLAYTHROUGH_REVIEW.md`.

## HTML / tarayıcı sürümü

`index.html` dosyasını çift tıklayarak Chrome, Safari veya Edge'de aç. Ana menüde **OYNA** ile bölümü başlat. İnternet, kurulum veya sunucu gerekmez. Öndeki topu tutup geriye çek; çekme mesafesi gücü, yana çekme yönü belirler. Bırakınca top masanın önünden dubalara doğru yüzeyde yuvarlanır. Space ile swap, R ile tekrar başlat; sol/sağ oklarla yön, yukarı/aşağı oklarla güç, Enter ile atış ayarlanır.

Bu bağımsız WebGL/Three.js sürümü, aynı renk kurallarını ve bölüm düzenlerini tarayıcıda çalıştırır. Top sırası, coin ile satın alınan swap hakları, hamle sınırı, aynı renkte zincirleme devrilme, kazanma/kaybetme, ses ve ilerleme kaydı dahildir. Unity sürümünün kaynakları ve macOS uygulaması da bu klasördedir.

Güncel mekanik ve testler: `Documentation/ROLLING_SHOT.md`. Ekran görüntüleri: `Documentation/rolling-html-ready.png`, `Documentation/rolling-html-aim.png` ve `Documentation/unity-roll-aim.png`.

## Temas açısına göre fizik

HTML sürümünde Cannon-es 0.20.0, Unity sürümünde PhysX kullanılır. Top ve aynı renkteki dubalar dinamik cisimlerdir; farklı renk dubalar o atış boyunca sabit sekme engelidir. Temas noktası, yüzey normali, kütle, sürtünme ve esneklik sekmeyi ve dönüşü belirler. Çarpışma sonrasında topu dubanın merkezine doğru yönlendiren kod ve hazır duba devrilme animasyonu kaldırılmıştır. Top başka bir top gibi hareket eden dubaya da yeniden çarpabilir.

Güç daha yüksek ilk hız ve momentum sağlar; devrilen duba sayısı isabet açısına, temas noktasına ve zincirleme çarpışmalara bağlıdır. Yüzde güce bağlı sabit bir duba sayısı yoktur. Farklı renkteki duba düşmez; top temas açısına göre sekip hız kaybeder. Kısa parıltı ve tok bir ses bu teması bildirir.

## Masa üzerinde yuvarlanma

Top masanın ön tarafında, ilk duba sırasından 7 oyun birimi uzakta başlar. Masa, aradaki yuvarlanma alanını gösterecek şekilde uzatılmıştır. Çekiş gücü yatay hızı belirler; topun ağırlığı sabittir. İlk hızın yukarı bileşeni yoktur. Masa üzerinde dönerek ilerleyen topun dubalara, kenarlara ve zemine tepkisini fizik motoru hesaplar. Nişan çizgisi yüzey üzerindedir ve ilk duba ya da masa kenarında biter. Düşüş noktası etiketi kaldırılmıştır.

Geriye çekme alanı topu masanın üzerinde tutacak şekilde sınırlıdır. Çekişin yana bileşeni ters yönde nişan alır. Yeni kamera açısı masa yüzeyini ve ön taraftaki topu daha büyük gösterir. Ayrıntı ve doğrulama: `Documentation/ROLLING_SHOT.md`.

## Baskılı bowling dubaları

Dubalar 96 halkalı, 64 dilimli yumuşak bir siluete, dar tabana, dolgun gövdeye ve yuvarlak başa sahiptir. Kırık beyaz cilalı üst yüzey, renkli alt kaplama, çift boyun şeridi ve koyu taban kullanılır. Pembe tavşan, turkuaz penguen ve mor kedi çizgi film karakterleri PINPOP yazısıyla birlikte gövdeye sarılan büyük baskılardır. Karakter PNG dosyaları `Assets/Art/Characters/` klasöründedir; üretim promptları `Documentation/CHARACTER_ASSETS.md` dosyasındadır.

HTML ve Unity aynı özgün baskıları kullanır. Kaynak `Assets/ArtSource/pin-skins.js`, Unity PNG dosyaları `Assets/Resources/PinSkins/` klasöründedir. Yeni dokular dış bağlantı gerektirmez. Temas parıltısı cilalı yüzeyde kısa bir emisyonla gösterilir. Ayrıntılar: `Documentation/PIN_PRESENTATION.md`.

## Daha doğal 3B görünüm

HTML önizlemesi artık perspektif kamera, ortak 3B ışık ve gölgelerle çizilir. Ahşap masa, gerçek duba/top geometrileri, temas gölgeleri, uzak plana alan derinliği ve daha sakin bir arayüz kullanılır. Sonradan eklenmiş gibi duran rampa ve kalın neon/altın çerçeveler kaldırıldı. Tutuş noktası ekrandan 3B atış düzlemine çevrilir; nişan çekiş vektöründen hesaplanır.

Three.js 0.160.1 dosyanın içine gömülüdür; internet/CDN bağlantısı gerekmez. HTML için WebGL destekli modern tarayıcı kullanılır. Three.js ve Cannon-es MIT lisansları HTML içinde ve `Documentation/third-party/` klasöründe yer alır. Her iki kütüphane de dosyaya gömülüdür; fizik için de internet gerekmez. Unity sürümünde de renkler/malzemeler düzenlendi ve düz gökyüzü görseli yerine 3B çevre kullanıldı.

## Lunapark sunumu ve ekranlar

- Ana menü: ödül arabası, bölüm numarası, OYNA, mağaza ve ayarlar.
- Oyun: mavi/altın HUD, coin alanı, renkli top sırası ve swap sayacı.
- Bölüm geçişi: kısa yükleme perdesi; girişte sekerek gelen dubalar.
- Atış: fiziksel top dönüşü, aynı renk etkisi, ölçülü parçacıklar, darbe halkaları ve combo yazıları.
- Zafer: PINPOP başlığı, yıldızlar, konfeti, havai fişek ve uçan coinler.
- Kaybetme: cüzdanı koruyan tekrar deneme ve ana menü akışı.
- Ayarlar: sesi aç/kapat, hareketi azalt, oyuna devam et, bölümü tekrar başlat, ana menü. Oyun ayarlarda gerçekten duraklatılır.

Referans incelemesi ve uyarlama ayrıntıları: `Documentation/POLISH_REFERENCE.md`. Erken ve ileri bölüm örnekleri incelendi; oynatma listesindeki 638 videonun tamamı tek tek izlenmedi. Görseller kodla üretildi.

## Yerel oynanış kaydı düzeltmeleri

9:16 Unity Game View tam yüksekliği kullanır; yüksek çözünürlükte alt boşluk oluşmaz. Yeni 9:16 profil, 2x şeffaf düğmeler, doğru yoğunlukta cüzdan çerçeveleri ve çıtalar önünde kalan bölüm yazısı eklendi. Kayıttaki 13. bölüm ve diğer UI/akış kontrolleri **127/127**, düğme şeffaflığı kontrolleri **32/32** geçti. Kanıtlar: `Documentation/video-review-verification.json` ve `video-review-*.png`.

## Bowling temalı bölüm geçişi

Boş festival hazırlık ekranı yerine bölüm numarası, bowling masası, yuvarlanan pembe top, yalnızca aynı renkte devrilen dubalar, renkli festival bayrakları ve kısa bir oyun ipucu gösterilir. HTML ve Unity aynı görsel düzeni kullanır. Unity geçişi her iki ayarda da en az 1,4 saniyedir. Hareket azaltma yalnızca animasyonu sabitler; ekranın okunabilir süresini kısaltmaz. Geçiş boyunca atış, SWAP ve ikinci bölüm isteği kilitlidir. Unity geçiş 9/9, ekran akışı 36/36; tarayıcı geçiş 11/11 kontrolleri geçti. Görseller `Documentation/transition-*.png`, raporlar `transition-unity-verification.txt` ve `transition-browser-verification.json` içinde.

## Güncel HTML ayarlarının Unity eşleştirmesi

Ses ve hareket azaltma ayarları, kayıtlar, pencere duraklatması, sonuç ekranları, yıldız/coin efektleri, esnek cüzdanlar, klavye/çekiş iptali, nişan noktaları, duba animasyonları ve fizik malzemeleri güncel HTML davranışına uyarlandı. Yedi ekran profili ve pencere değişiminde içerik korunması eklendi. **195/195 Unity kontrolü geçti.** Ayrıntılı eşleştirme: `Documentation/HTML_UNITY_PARITY.md`; toplu rapor: `Documentation/html-unity-sync-verification.json`.

## HTML görünümünün Unity aktarımı

Masa ve lunapark geometrisi, menüler, düğmeler, renk kuyruğu, yükleme ekranı ve kamera HTML referansından yerel Unity nesnelerine aktarıldı. Karşılaştırmalar ve kapsam: `Documentation/HTML_UNITY_PARITY.md`. Hazır macOS uygulaması bu kaynaklarla yeniden derlendi.

## Çalıştırma

- Hazır macOS oyun: `Builds/PinPop.app` dosyasını aç.
- Unity Hub → Add → bu `PinPop` klasörünü seç → `Assets/Scenes/PinPop.unity` → Play → oyun içi OYNA.
- Sahne, modeller, materyaller ve arayüz çalışma anında oluşturulur. Sahnedeki `PinPop · Game` nesnesi giriş noktasıdır.
- Editör menüsü: **PinPop → Create or reset playable scene** sahneyi yeniden üretir.

## Kontroller ve kurallar

Masanın önündeki aktif topu tutup kendine doğru çek. Yana çekişin ters yönüne nişan alınır; gerilme miktarı yatay atış hızını belirler. Masa üzerindeki kesikli çizgi ilk engelde biter. Bırakınca top aynı yüzeyde dönerek ilerler. Topa yalnızca dokunmak, çok kısa çekiş ve iptal hamle harcamaz.

- Top **kendi rengindeki** dubaları hareket ettirip devirebilir. **Farklı renk** duba sabit kalır; top temas açısına göre seker ve hız kaybeder. Bu dubaları doğru hedefe ulaşmak için sekme engeli olarak kullanabilirsin.
- Dubalar da yalnızca aynı renkteki dubaları itebilir. Böylece renk kuralı zincirleme çarpışmalarda da geçerlidir.
- Yeterince devrilen veya masadan düşen dubalar sayılır, kısa bir fizik animasyonundan sonra renk parçacıklarıyla kaybolur.
- Aktif top masanın ön tarafında yüzeye oturur. Sıradaki **üç top**, SWAP düğmesinin üstünde 1–2–3 sırasıyla ve parmak delikleriyle gösterilir. Atış, swap ve bölüm değişiminde güncellenir. Hamleler azaldığında yalnızca kullanılabilecek sonraki toplar görünür. **SWAP**, aktif topu bütün kuyruğun en sonuna gönderir. Bir swap hakkı tüketir, hamle harcamaz.
- Her geçerli bırakış bir hamledir. Gererken ve fizik çözülürken swap/yeni atış kilitlidir. Daha yüksek güç daha fazla ilk momentum sağlar; sonuç temas açısına, konumuna ve gerçek çarpışmalara bağlıdır.
- Son hamle sonrasında da devam eden devrilme beklenir. Bütün dubaları temizlersen kazanırsın; hamle biter ve duba kalırsa tekrar denersin.
- **←/→:** yön. **↑/↓:** güç. **Enter:** klavye atışı. **Space:** swap. **R:** bölümü tekrar başlat. **Esc:** çekişi iptal et / duraklat. **Dişli:** duraklatma ve ayarlar. Ses ve hareket azaltma ayarlarını bu panelden değiştirebilirsin.
- Bir bölümün ilk zaferi **30 + kalan hamle × 2 coin** verir. Aynı sonuç tekrar ödül vermez; daha iyi tekrar yalnızca bonus farkını öder.
- **Top Mağazası:** Klasik ücretsiz; Mermer 90, Yörünge 180, Galaksi 300 coin. Satın alınan görünüm kalıcıdır ve ücretsiz seçilir. Renk kuralını veya top fiziğini değiştirmez.
- **SWAP üzerindeki +** satın alma penceresini açar: **1 hak = 10 coin**. Ana SWAP kuyruğu döndürür; satın alma düğmesi ayrı çalışır. Eski üst satın alma yazısı kaldırıldı.
- İlk açılışta **3 swap hakkı** verilir. Hakkın bitince swap kullanamazsın; yeterli coin olmadan satın alamazsın.
- Coin ve haklar yeniden başlatmada sıfırlanmaz; tamamlanan bölüm ve ses tercihiyle birlikte yerel kaydedilir. HTML ve Unity sürümleri kendi yerel kayıtlarını tutar.

İlk bölüm 12 duba / 6 hamle, ikinci ve üçüncü bölüm 18 duba / 8 hamledir. Sonrasında 24 dubalı, renkleri ve arka sıraları değişen deterministik düzenler gelir. Bu sürüm temel oynanışı ve görsel hissi değerlendirmek için hazırlanmıştır.

## Proje yapısı

- `Assets/Scripts/PinPopGame.cs`: bölüm düzenleri, top sırası, hamleler, giriş ve oyun durumları.
- `PinTarget.cs` / `BowlingBall.cs`: renk filtreli fizik ve devrilme algılama.
- `PinPopHUD.cs`: safe area uyumlu Türkçe arayüz, top sırası, sonuç kartları.
- `PinPopVisuals.cs`: prosedürel bowling dubaları, masa, materyaller, parçacıklar.
- `CarnivalWorld.cs`: çadırlar, bulutlar, dönme dolap ve 3B ödül arabası.
- `Assets/Editor/ProjectSetup.cs`: sahne ve macOS build üretimi.
- `Documentation`: referans notları, doğrulama raporu ve oyun içi ekran görüntüleri.

## Tekrar derleme ve doğrulama

```sh
UNITY='/Applications/Unity/Hub/Editor/6000.3.24f1/Unity.app/Contents/MacOS/Unity'
"$UNITY" -batchmode -projectPath "$PWD" -executeMethod ProjectSetup.BuildMac -quit -logFile /tmp/pinpop-build.log
PINPOP_QA_DIR="$PWD/Documentation" ./Builds/PinPop.app/Contents/MacOS/PinPop -economy-test -logFile /tmp/pinpop-qa.log
```

Güncel doğrulama mağaza alımı/seçimi, ayrı swap satın alma akışı, coin hesabı, kayıt, gerçek çekiş/atış ve görünümün fiziği değiştirmemesini kontrol eder. Test bayrağı normal oyunda çalışmaz. Tüm bölüm düzenleri için `-economy-progression-test` kullanılabilir.

Android/iOS dokunma girdisi ve dikey ekran ayarları hazırdır. Bu makinede yalnızca macOS Build Support bulunduğu için mobil cihaz paketi üretilmemiştir.


Güncel mağaza ve coin doğrulaması için `Documentation/ECONOMY_AND_SHOP.md` dosyasına bak. Önceki 30 coin sabit ödül ve mağazadan swap alma varsayımını içeren tarihsel test raporlarının yerine bu özellik için `-economy-test` ve `economy-browser-test.mjs` kullanılır.


## Video ile devam ve polish

Hamleler bitince isteğe bağlı video tamamlayarak aynı masada **+3 hamle** ile devam edebilirsin. Her denemede bir kez kullanılabilir; erken kapatma ödül vermez. Ek hamleler coin bonusu üretmez. SWAP üzerindeki `+` görünümü küçültüldü, dokunma alanı korundu; sonuç ekranı ve video sonrası geri bildirimler düzenlendi. Android/iOS için AdMob test entegrasyonu, masaüstü/HTML için yerel PinPop tanıtım videosu hazır. Canlı reklam kimlikleri ve doğrulama kapsamı: [REWARDED_MOVES_AND_POLISH.md](Documentation/REWARDED_MOVES_AND_POLISH.md).


## Smash Match düşme ve üçlü eşleştirme

Duba yalnızca yatınca silinmez: masadan aşağı düştüğünde 10 yerlik alana uçar. Üç aynı renk geldikten sonra ortada birleşip patlayarak silinir. Son üçlü bitmeden bölüm tamamlanmaz. Renk adetleri üçün katına, hamle bütçeleri 12/18/24'e uyarlandı. Atış yönü her yöne çevrilebilir. Ayrıntılar ve güncel doğrulama: [SMASH_MATCH_ADAPTATION.md](Documentation/SMASH_MATCH_ADAPTATION.md).

## Duba görünümü ve çarpışma düzeltmeleri

Düşen hedefler toplama alanında kendi baskılı duba görselleriyle uçar ve birleşir. Çekiş yalnızca yön/güç değiştirir; top başlangıç noktasında sabittir. Farklı renkler dahil tüm duba çiftleri katı çarpışır; gövde, boyun ve kafa duba profiline uygun üç konveks collider ile korunur. [Uygulama ve kontroller](Documentation/PIN_IDENTITY_AND_COLLISIONS.md).

## Duba–masa sürtünmesi

Devrilen dubaların masada fazla çabuk durmasını azaltmak için duba–masa sürtünmesi 0,14 olarak ayrıldı. Topun gücü, kütlesi ve diğer temas katsayıları korunur. Aynı altı atışın son karşılaştırmasında düşen duba sayısı 8’den 17’ye çıktı. [Fizik incelemesi ve doğrulama](Documentation/PHYSICS_REVIEW.md).

## Çimde yuvarlanma ve üst toplama alanı

Çekerek atış sürer; basılı tutarak otomatik güç doldurma taslağı geri alındı. Toplar çime düşünce sürtünmeyle yuvarlanıp durur ve sonraki atışta korunur. Toplama alanı bölüm başlığının altında, sonraki üç top swap düğmesinin üstündedir. [Ayrıntılar ve doğrulama](Documentation/GRASS_AND_HUD_LAYOUT.md).

## Runtime kamera FOV ve toplama genişliği

Play Mode'da `Game camera` üzerindeki Camera / Field of View değeri düzenlenebilir; kare başına sabit değere geri dönmez. Toplama paneli, dubaların bulunduğu arka tahta kenarının ekrandaki genişliğine hizalanır ve FOV değiştikçe yuvalarla birlikte ölçeklenir. [Ayrıntılar](Documentation/RUNTIME_FOV_AND_TRAY_WIDTH.md).

### Lunapark macerası

Her biri 20 bölümlük iki bölge, toplam 40 bölüm ve sekiz tek seferlik sandık, dört engel ve dört güçlendirici eklendi. Kullanım, kayıt uyumluluğu ve test komutları: [ADVENTURE_FEATURES.md](Documentation/ADVENTURE_FEATURES.md).

### Resimli bölge haritası

Dört lunapark teması, 20 özgün bölüm simgesi ve bölgeye özel hareketli süsler: [CARNIVAL_MAP.md](Documentation/CARNIVAL_MAP.md). Arka planlar ve ikonlar `Assets/Resources/MapArt/` altında saklanır; HTML kopyaları dosya içinde gömülüdür.
