# Çimde yuvarlanma ve HUD yerleşimi

## İstenen değişiklikler

- Basılı tutarak otomatik güç doldurma taslağı geri alındı. Topun üstünden çekiş başlar; çekiş uzunluğu gücü, çekişin tersi atış yönünü belirler. Beklemek güç artırmaz. Bırakmak bir atış yapar. Top çekiş sırasında başlangıç noktasında sabittir. Klavyede yön okları/güç okları ve Enter atışı geri getirildi.
- Toplama alanı, bölüm başlığı ve üst kontrollerin altına taşındı. On yuva, kapasite rozeti, duba uçuşu ve birleşme hedefleri aynı konumu kullanır. Birleşme yazısı üst başlığı örtmemek için alanın altında görünür.
- Sonraki üç top sağda swap düğmesinin üstünde, 1→2→3 sırasıyla gösterilir. Yerleşim düğmenin gerçek konumuna bağlıdır. Yardım düğmesi üst alana çakışmaması için alt sola taşındı.

## Çim fiziği

Çim yüzeyi görünür zeminle aynı yükseklikte, y=-1,25'tedir. Yalnızca bowling topları bu zemine çarpar; dubalar mevcut y < -0,8 toplama eşiğine serbestçe düşmeye devam eder. Çime inmiş topların duba collider'larıyla teması da kapatılır; bu toplar düşen dubalara ikinci bir zemin oluşturamaz.

Çim–top temasında sürtünme 0,72 ve sekme 0,02'dir. Masadan ayrılan topların yatay hızı 4,5 birim/sn ile sınırlanır; çime ilk temasta hız en fazla 3,5 birim/sn olur. Dönüş hızı da yuvarlanma hızına uyarlandığından eski yüksek spin topu yeniden hızlandırmaz. Temas sırasında 5 birim/sn² yuvarlanma direnci uygulanır. Çime inişten sonra top yaklaşık 1,2 birim daha yuvarlanıp durur; yüksek güçlü atışlar da masanın yakınında kalır.

Top, çime ilk temasından **10 oyun saniyesi sonra** silinir; sonraki temaslar süreyi sıfırlamaz. Duran/uyuyan toplar da silinir. Sonraki atış bu süreyi kısaltmaz. Duraklatma hem fiziği hem süreyi durdurur; bölüm yeniden yüklendiğinde toplar hemen temizlenir. Masa üzerinde kalan eski top, yeni atışta önceki temizleme davranışını sürdürür. Aynı davranış Unity ve HTML sürümlerinde uygulanır.

Masa üzerindeki duba–masa sürtünmesi 0,14; diğer masa temasları, top kütlesi, atış hızları, renk kuralı ve duba collider'ları korunur.

## Doğrulama

`-grass-layout-test` gerçek Unity atışıyla çime düşmeyi, yavaşlamayı, eski altı saniyelik sürenin ötesinde kalmayı, durmayı, sonraki atışta korunmayı, duraklatmayı, dubaların yine toplanmasını ve 600×1000 / 320×740 / 390×844 / 1000×600 ekran yerleşimlerini kontrol eder. Ekran görüntüleri `grass-layout-*.png` adlarıyla kaydedilir.

`grass-layout-browser-test.mjs` HTML sürümünde gerçek fare basma/çekme/bırakma, otomatik güç dolmaması, Cannon çim fiziği ve dört ekran boyutunda yerleşimi kontrol eder. Otomasyondaki mouseMoved olayında basılı sol düğme açıkça belirtilir; gerçek sürüklemenin pointer capture'ı korunur.

Mevcut `-pin-behavior-test`, `-smash-match-test` ve model kontrolleri çarpışma, üçlü eşleşme, video ile devam ve kazanma akışını ayrıca doğrular.

Son doğrulama: yeni Unity kontrolleri **25/25**, duba/atış kontrolleri **35/35**, düşme ve birleşme kontrolleri **44/44**, HTML kontrolleri **18/18**, model kontrolleri **33/33** geçti. Son kaynak ve uygulama eşliği `grass-layout-final-audit.json` dosyasındadır.


## Yakın mesafe ve 10 saniye doğrulaması

`grass-lifetime-model-test.cjs`, HTML oyununun gerçek Cannon fiziğiyle dört masa kenarında tam güçlü atışları kontrol eder: masaya yakın iniş, kısa yuvarlanma, durma, on saniyeye kadar görünürlük, fizik dünyasından ve render listesinden silinme, sonraki atışta korunma. Kenar testi dubaları kaldırarak her kenara ulaşmayı sağlar; gerçek duba/toplama regresyonu `smash-match-model-test.cjs` ile ayrıca kontrol edilir.

Unity `-grass-lifetime-test`, gerçek tam güçlü yan atışla yakın inişi, durmayı, duraklatmayı, sonraki atışta korunmayı ve 10 saniye sonunda uyuyan topun silinmesini doğrular. Güncel sonuçlar `grass-lifetime-model-verification.json` ve `grass-lifetime-unity-verification.txt` dosyalarına yazılır. Yukarıdaki eski doğrulama sayıları önceki HUD değişikliğine aittir.


## Ön kenarda top konumlandırma

Atış öncesi topa dokunup yatay sürüklemek yalnızca x konumunu değiştirir. Yükseklik 0,40 ve ön kenar derinliği z=-7,2 sabittir; yatay sınır ±3,65'tir. Top kenardan taşmaz. Yatay harekete başladıktan sonra parmağın dikey kayması modu değiştirmez. Bırakmak atış yapmaz ve hamle harcamaz. Yeni bir dokunuşla geriye çekip bırakmak, seçilen konumdan atış yapar; nişan önizlemesi de aynı konumu kullanır.

Hareketin ilk 12 tasarım pikseli içinde mod seçilmez. Eşik aşıldığında yatay mesafe dikey mesafenin 1,3 katından fazlaysa konumlandırma, diğer durumda mevcut çekiş/nişan modu seçilir. Fare ve dokunma aynı kuralı kullanır. İptal, odak kaybı ve duraklatma atış başlatmaz. Swap ve sonraki atışlar seçili konumu korur; yeniden başlatma ve yeni bölüm ortaya döndürür.

Klavyede sağ/sol oklar topu 0,35 birim kaydırır. Shift+sağ/sol nişan açısını, yukarı/aşağı gücü ayarlar; Enter atış yapar. Atış çözülürken ve oyun duraklatılmışken konum değiştirilemez.

Unity `-launch-position-test`, model `launch-position-model-test.cjs` ve tarayıcı `launch-position-browser-test.mjs` kontrolleri, üç ekran boyutunda yatay hareketi, sınırları, hamle harcamamayı, swap'ı, seçilen konumdan atışı ve engellerin yanından dubaya ulaşmayı doğrular. Tarayıcı testi ayrıca gerçek dokunmatik konumlandırma yapar. Sonuç dosyaları `launch-position-*-verification` adlarını kullanır.


## Nişan ucunda güç halkası

Ayrı güç kartı kaldırıldı. Çekiş gücü, nişan çizgisinin ucundaki halkada 0–360 derece dolumla gösterilir. Dolan yay açık kremden altın rengine geçer. Tam güçte hafif bir parıltı belirir; azaltılmış harekette parıltı sabit kalır. Nişan çizgisi topun eşleşme rengini korur; top ve dubaların renkleri değişmez.

Halka kameraya dönüktür; perspektiften basık bir elipse dönüşmez. Küçük ekranlarda en az 36 piksel çapla çizilir ve duba arkasında gizlenmez. Parmakla çekiş alanında yer almaz; yeni bir dokunma alanı veya collider eklemez. Çekiş iptal edildiğinde, atış bırakıldığında, duraklatmada ve yatay top konumlandırmada gizlenir. Klavyeyle nişan alma aynı halkayı kullanır.

Unity `AimPowerRing`, tekrar kullanılan 96 segmentli bir mesh kullanır. HTML aynı sayıda segmentle yayı günceller; boyut ve yön kameraya uyarlanır. Üstteki kartın nesneleri ve HTML DOM öğeleri oluşturulmaz. Yavaşlatılmış birleşme süreleri (0,32 + 0,44 saniye), atış fiziği ve ilerleme kayıtları korunur.

Unity `-power-match-polish-test` ve HTML `power-match-polish-browser-test.mjs` çeyrek/orta/tam dolumu, renk ayrımını, tam güç parıltısını, iptali, yatay konumlandırmayı ve üç ekran boyutunu doğrular. HTML testi gerçek dokunmatik bırakmayı ve klavye nişanını da kontrol eder. Görüntüler `aim-power-ring-*.png`, sonuçlar `power-match-polish-*-verification` dosyalarındadır.
