# Lunapark macerası: harita, engeller, güçlendiriciler ve sunum

## Bölüm haritası

Toplam **40 bölüm** vardır. İlk 20 bölümün dizilimleri korunur; 21–40 için 20 farklı duba düzeni ve yeni durak adları eklenmiştir. Her yeni bölümde sekiz renkten altışar duba ve 48 hamle bulunur. Beş dizilim ailesi dört farklı aralık varyasyonuyla kullanılır; sabit tampon, buz, bağ ve hareketli bariyer kuralları devam eder.

Her bölgenin haritası kendi **20 bölümünü** tek bir dikey yolda gösterir. Lunapark Kapısı 1–20, Dönme Dolap 21–40 bölümlerini içerir. Üstteki iki bölge sekmesi açılmış bölgeler arasında geçiş sağlar; ikinci bölge ilk 20 bölüm tamamlanana kadar kilitlidir. Sayaç, seçili bölgenin ilerlemesini /20 olarak gösterir. İlk bölge tamamlanmışsa 20/20, ikinci bölgeye yeni geçilmişse 0/20 görünür.

Fare tekerleği, sürükleme ve dokunmatik kaydırma desteklenir. Harita açıldığında mevcut ilerlemenin bölgesi ve bölümü seçilir. Önceki bölgeye geçildiğinde son açılmış bölüm ortalanır. Sandık alınırken hem seçili bölge hem kaydırma konumu korunur. Sekme değiştirmek ilerleme veya ödül vermez.

Toplam içerik Unity `AdventureRules.Chapters=40`, HTML `ADVENTURE.chapters=40` ile belirlenir. `LevelsPerRegion=20`, `MapPageSize=5` korunur. Sekiz sandık eşiği 5/10/15/20/25/30/35/40'tır. 20 ve 40 bölge sandığı, diğerleri ara sandıktır. Ödül döngüsü 25/40/60/100 coin ve sırayla renk seçici/gökkuşağı/cep/ek hamledir. Eski dört sandığın bitleri değişmez; sandıklar tekrar ödemez.

20 bölümü bitirmiş kayıtlar 21. bölümden devam eder; coin, swap, görünümler, yıldızlar ve yardımcı envanteri korunur. 20. bölümün sonuç düğmesi sonraki bölümü açar. 40. bölümün sonuç düğmesi tamamlanan haritaya döner; 41. bölüm oluşturulmaz.

## Duba ve masa engelleri

- 3. bölüm: iki sabit tampon; merkezde ve yanlarda atış koridorları bırakır.
- 5. bölüm: dört renkten birer buzlu duba. Yanlış renk etkilemez; uygun renk veya gökkuşağı ilk çarpmada buzu kırar. Duba sonraki atışta hareket eder.
- 7. bölüm: aynı renkte üç duba bağlıdır. Uygun ilk temas bağı tüm grupta koparır; sonraki atışta serbestleşir.
- 9. bölüm: sinüs hareketli fiziksel geçit. Bekleme/çekiş sırasında hareket eder; menü ve duraklatmada donar.
- Sonraki bölümlerde mekanikler karışır. Korunmuş dubalar da normal renk ve üçlü toplama kurallarına uyar. Buz/bağ kırılması duba toplamaz veya hamle dışında otomatik hasar vermez.

## Güçlendiriciler

Oyun ekranının sol altındaki araçlar açıklama/seçim ekranı açar. Mağazaları aynı açıklama ekranında bulunur.

| Araç | Başlangıç | Coin | Davranış |
| --- | --- | --- | --- |
| Renk seçici | 3 | 20 | Sıradaki topu masada kalan bir renge çevirir; hamle harcamaz. |
| Gökkuşağı | 2 | 35 | Tek atışta bütün renkleri hareket ettirir; dubalar kendi renklerinde tepsiye gelir. |
| Tepsi cebi | 2 | 25 | Oyuncunun seçtiği, ulaşmış ve eşleşmemiş tek dubayı saklar. Aynı renkten iki eşleşmemiş duba geldiğinde geri dönüp üçlüyü tamamlar. |
| +3 hamle | 2 | 30 | Devam eden bölümde üç hamle ve uygun top sırası ekler; ödüllü video kullanımından bağımsızdır. |

Atış sırasında araç kullanılmaz. Yetersiz stok/bakiye, dolu cep ve geçersiz seçim envanteri tüketmez. Yeniden başlatma cep ve silahlandırılmış gökkuşağı gibi geçici durumu sıfırlar; satın alınan envanter korunur. Kullanılmış araç geri verilmez. Envanter sınırı 999.

## Sunum ve kayıt

Yeni renk/engel ilk gelişinde figürlü kısa tanıtım gösterilir. Kapatılan tanıtımlar kaydedilir. Buz kırılma parçacığı, altın bağ kopma efekti, değişen gökkuşağı rengi, seçilen gerçek figür ikonları ve sandık özeti mevcut ses/ödül akışına eklenir. Hareket azaltma tercihi kart geçişlerini kaldırır; zamanlamaya bağlı fiziksel engeller işlevini korur.

Unity `PinPop.Adventure.v1` anahtarında envanter/sandık/tanıtım kaydını tutar; mevcut ekonomi v2 kaydını değiştirmez. HTML aynı alanları mevcut v2 cüzdanda `adventure` altında tutar; eski kayıtlar başlangıç envanteriyle açılır. Mevcut coin, swap, top görünümleri ve ilerleme korunur.

Tarayıcı model ve UI kaynakları `Assets/ArtSource/adventure-model.js` ve `adventure-ui.js` içinde; çalışma kopyaları tek dosyalık `index.html` içine gömülüdür. Unity kuralları `PinPopAdventure`/`AdventureRules`, sunumu `AdventureHUD` yönetir.

## Doğrulama

Önceki macera testleri `adventure-model-test.cjs`, `adventure-browser-test.mjs` ve Unity `-adventure-test`; korumaların renk davranışı, hareketli bariyer, dört araç, satın alma, kayıt, sandık tekrarı ve farklı ekran genişlikleri. Ayrıntılı sonuçlar aynı klasördeki `adventure-*-verification` dosyalarına yazılır. Unity QA kullanıcının cüzdan ve macera kaydını yedekleyip geri yükler.

Son teslim doğrulaması: Unity yeni özellikler 45/45, çimen/HUD 25/25, eşleşme/kazanma 44/44; HTML model 65/65, arayüz 19/19, mevcut eşleşme modeli 33/33. Toplam 231 kontrol geçti. Kaynak ile izole derlemeye alınan dokuz C# dosyası birebir karşılaştırıldı.


## Açılış splash ekranı

Uygulama açılışında ana menü yerine tam ekran lunapark kapısı sahnesi, büyük ve katmanlı PinPop logosu, üç karakterli duba, bowling topu ve ince yükleme şeridi görünür. Dubalar hafifçe salınır, top yana yuvarlanır; hareket azaltma seçeneğinde bu hareketler durur. Yükleme şeridi hazırlık sırasında belirsiz süreli hareket eder, hazır olduğunda dolar; sahte bir yüzde gösterilmez. İlk uygulama karesi bu ekranla kaplanır; menü arkasında hazırlanır. Splash, hazırlık tamamlandıktan sonraki ilk çizilmiş kareden itibaren en az 1,6 saniye görünür. Ardından çizimler 0,20 saniyede kaybolur; kapak 0,35 saniyede açılarak ana menüyü gösterir. Böylece açılış ve menü logoları üst üste binmez. Kullanıcıdan dokunma beklemez. Başlatma reklam ağı veya internet yanıtını beklemez; mobil reklam/onay başlatma işlemi splash kapandıktan sonra yapılır.

Bu sırada menüye dokunma ve bölüm açma engellenir. Ana menüye geri dönmek veya bölüm değiştirmek splash'i yeniden göstermez; uygulamanın yeni açılışında tekrar görünür. Ses çalmaz. Hareket azaltma açıkken dekor sabittir. Yükleme hatasında menü kilitli kalır ve kayıtları sıfırlamadan yeniden deneme sunulur.

Unity `PinPopSplash`, sahne yüklenmeden kurulur; ilk kareden sonra oyun hazırlığına geçilir. Bekleme süresi, Unity'nin yerleşik açılış katmanı bittikten ve oyun hazır olduktan sonraki ilk görünür kareden sayılır. Sahneye kaydedilmiş, yalnızca düzenleme modunda etkin bir kamera ve açılış görseli Game görünümündeki kamerasız boşluğu kapatır; Play başladığında bu önizleme kapanır. HTML splash içeriği büyük oyun betiğinden önce bulunur; başlangıçta menü görünmez ve `inert` durumdadır. `PinPopBoot.complete()` hazır olma, minimum süre ve geçişi birleştirir. Tarayıcı depolamasına erişilememesi splash'i bozmaz.

Unity `-splash-test` ve HTML `splash-browser-test.mjs`; logo/ilk ekran, dokunma engeli, süre, otomatik menü geçişi, kayıt koruma ve yeniden göstermemeyi kontrol eder. HTML testi 600×1000, 320×740 ve 1000×600 ekranları, azaltılmış hareketi ve hata/yeniden deneme akışını da kapsar. Sonuçlar `splash-*-verification`, görüntüler `splash-*.png` dosyalarına kaydedilir. Diğer açık QA başlatma bayrakları kendi oyun kontrollerine doğrudan geçer; normal açılış splash'i atlamaz.

23 Kasım 14.19.36 ekran kaydı incelendi: düzenleme modunda kamerasız Game görünümü, hazırlık sırasında kısalan görünür bekleme ve geçişte çift logo tespit edildi. Düzeltilen sahne kaydedilip yeniden açılarak kamera/görsel doğrulandı. `VideoReview/startup-after-review.mp4` uygulamanın kendi render çıktısından alınan yeni kayıttır; masaüstü kaydı değildir. Unity'nin varlık içe aktarma penceresi editör tarafından yönetilir. Önceki geçiş düzeltmesinde Unity 15/15, HTML 35/35 kontrol geçti; kayıt 34 karede kapak/çizim sırasını, etkin kamerayı, ana menüye ulaşmayı ve kayıt korumayı doğrular.

Splash görsel yenilemesi: Kullanıcının seçtiği “lunapark sahnesi ve büyük PinPop logosu” yönü uygulandı. Haritanın mevcut `region-gate` çizimi kullanılır; çevrimdışı çalışır ve HTML içine gömülüdür. Alt koyu geçiş metni okunur tutar; eski stüdyo dipnotu kaldırılmıştır.

Yeni tasarım ayrıca tam sahne görseli, büyük logo ve yükleme şeridini doğrulayan kontrollerle kapsanır. Son raporlar: `splash-unity-verification.txt`, `splash-browser-verification.json`; animasyon kaydı: `VideoReview/startup-after-review.mp4`.

## 40 bölüm doğrulaması

Unity `-forty-level-test`, tüm dizilimlerin renk üçlülerini, top tedarikini, masa sınırlarını ve duba aralıklarını; yeni 20 bölümün gerçek fizik nesneleriyle yüklenmesini; eski kayıt aktarımını; sekiz sandığı ve sonuç düğmelerini kontrol eder. `forty-level-browser-test.mjs` bütün 40 dizilimi Unity'nin çıkardığı `forty-level-unity-layouts.json` ile karşılaştırır ve gerçek sonuç/sonraki bölüm düğmelerini test eder. Sonuç akışlarını sınamak için çözülmüş masa durumu test içinde simüle edilir; bu, 40 bölümün elle oynanıp bitirildiği anlamına gelmez.

`-map-polish-test` ve `carnival-map-browser-test.mjs` 1, 20, 21 ve 40. bölümün 600×1000, 320×740 ve 1000×600 ekranlarda ortalanmasını, kilitleri, tekerlek/sürükleme/dokunma kaydırmasını ve sandıkları doğrular. Kullanıcı kayıtları Unity testinden sonra geri yüklenir.

Bölge ayrımı doğrulaması: Unity `-forty-level-test` eski/yeni bölgeye geçişi, her haritanın yalnızca 20 bölüm içermesini, 20/20 ve 0/20 sayaçlarını, kayıt korumayı ve son bölüm dönüşünü sınar. `-map-polish-test` kilitli bölgeyi ve farklı ekranlarda metin/dokunma alanlarını kontrol eder. HTML karşılıkları `forty-level-browser-test.mjs` ve `carnival-map-browser-test.mjs` dosyalarıdır.
