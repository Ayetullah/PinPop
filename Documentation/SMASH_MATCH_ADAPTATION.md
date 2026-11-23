# Smash Match düşme ve üçlü toplama uyarlaması

Kaynak: kullanıcının verdiği `Smash_Match.html`; değiştirilmeyen kopyası `References/Smash_Match.html` içindedir. Bowling dubaları, renkli top kuralı, top görünümleri, swap, coin kaydı ve video ile devam özelliği PinPop'ta korunur.

## Düşme

Duba yana yatınca veya yalnızca masa sınırını yatayda geçince toplanmaz. Yerçekimiyle masadan düşüp fiziksel gövde merkezi **y < -0,8** olduğunda bir kez toplanır. Gövde ve çarpışmaları o anda sahneden çıkar; aynı renkte bir parça toplama alanına uçar. Bu eşik JS'de Cannon gövde merkezinden, Unity'de Rigidbody dünya kütle merkezinden ölçülür.

Kaynakla aynı yerçekimi **-20**, temas sürtünmesi **0,45**, sekme **0,05** kullanılır. Kenarlardaki görünmez duvarlar ve düşen dubayı yakalayan ikinci fizik zemini kaldırıldı. Çevre görselleri yerinde kalır. PinPop bowling topunun kütlesi 6 olarak korunur; en güçlü yuvarlama hızı kaynak oyununkine uygun 24'e çıkarıldı. Dağılan dubalara erişebilmek için çekişin tersine her yöne nişan alınabilir; klavyedeki sol/sağ oklar yönü döndürür.

## Toplama ve birleşme

| Kaynaktaki kural | Uygulama |
| --- | --- |
| Toplama kapasitesi | 10 yer; eşleşmiş üçlüler canlı kapasiteden hemen düşer |
| Yerleştirme | Son eşleşmemiş aynı renk parçanın arkasına eklenir |
| Uçuş | 0,48 sn; cubic ease-in-out, 70–120 birim kavis, ±270° başlangıç dönüşü ve %25 boyut kabarması |
| Yerleşme | 0,30 sn esneme; sıra hareketi 0,22 sn `cubic-bezier(.3,1.4,.5,1)` |
| Üçlü | Üç aynı renk rezerve edilir; hepsi alana gelmeden birleşmez |
| Birleşme | Ortadaki parçanın yerine 0,32 sn içinde kayar; ardından patlama başlar |
| Silme | Patlamadan 0,44 sn sonra üç parça kaldırılır; kalanlar boşluğu kapatır |
| Geri bildirim | Halka, kıvılcımlar, +3 / kombo metni, iniş notaları ve üçlü akoru |
| Kazanma | Tüm dubalar düşmüş ve alan tamamen boşalmış olmalı; son animasyondan sonra 0,7 sn beklenir |

Duraklatma toplama uçuşlarını da durdurur. Yeniden başlatma eski uçuş/grupları temizler. Video ile +3 hamle alındığında bekleyen parçalar ve masa korunur. Alan eşleşmeden dolarsa kaybedilir; bu durumda fazladan hamle kapasite sorununu çözmeyeceği için video önerilmez.

Renk adetleri bölümlerde üçün katıdır: ilk bölüm 6/3/3, sonraki iki bölüm 6/6/6, devamında 9/9/6 ve renk dönüşümleri. Toplam duba sayıları 12/18/24 olarak korunur. Fiziksel düşürme gerektiğinden kaynak oyundaki hedef başına atış yaklaşımıyla normal hamle bütçeleri de **12/18/24** oldu. Coin formülü hâlâ 30 + kalan hamle × 2; video hamleleri tasarruf bonusu üretmez.

Toplama alanına yer açmak için oyun kadrajı yukarı alındı. Alt alan sıradaki topları kapatmaz; parçalar dar ve yatay ekranlarda da kare kalır. Hareket azaltma tercihi uçuş dönüşünü ve kıvılcımları azaltır.

## Doğrulama

- `smash-match-model-test.cjs`: kaynak sabitleri, gruplama/kapasite/zamanlama, 12 düzenin üçlü adetleri, gerçek Cannon düşüşü, video devamı, son eşleşme sonrası zafer ve tek ödeme.
- `smash-match-browser-test.mjs`: gerçek çek-bırak ile ilk bölümün 12 dubası ve dört üçlüsü tamamlandı; kazanma ödülü, animasyon kareleri ve ekran ölçüleri kaydedildi.
- Unity `-smash-match-test`: gerçek PhysX düşüşleri, masada yatan hedefin silinmemesi, uçuş ve birleşme, duraklatma, gerçek video sonrası alanın korunması, son fiziksel düşüşten sonra birleşmeyi bekleyen zafer ve kare/ekrana sığan parçalar.
- `smash-match-layout-test.mjs`: dört ekran boyutunda kare parçalar, toplama alanı/top sırası ve top sırası/swap çakışma kontrolleri.
- Sonuç dosyaları: `smash-match-model-verification.json`, `smash-match-browser-verification.json`, `smash-match-layout-verification.json`, `smash-match-unity-verification.txt`.

Önceki sabit 6/8/9 hamle, 12 dubanın renkte dörder olması ve yatınca silinmesi varsayımlarını kullanan eski fizik raporları bu yeni mekanik için geçerli değildir. Yeni davranış yukarıdaki kontrollerle doğrulanır.

Son doğrulama: model **33/33**, gerçek tarayıcı oynanışı **24/24**, ekran yerleşimi **12/12**, son macOS Unity derlemesi **44/44** geçti. Son Unity günlüğünde Exception/FAIL yok. Kaynak dosya özeti ve son derleme eşliği `smash-match-final-audit.json` dosyasındadır.

Güncel fizik ayarı: duba–masa sürtünmesi, masada yatıp duran hedefleri daha rahat düşürmek için **0,14** olarak ayrıldı; diğer temaslar **0,45** kalır. Ölçüm ve karşılaştırma: [PHYSICS_REVIEW.md](PHYSICS_REVIEW.md).


## Daha okunaklı birleşme temposu

Kullanıcının yavaşlatma isteğiyle birleşme + kaybolma süresi 0,37 saniyeden **0,76 saniyeye** çıkarıldı. 0,48 saniyelik uçuş korunur; aynı anda toplanan üçlünün bütün akışı yaklaşık 1,24 saniyedir. Görsel yakınlaşma, büyüme ve saydamlaşma Unity/HTML modelindeki ortak aşama sürelerini kullanır; ikonlar animasyon bitmeden silinmez. Üçlü rezervasyonu, kapasite, kombo ve kazanmanın alan boşalmasını beklemesi korunur.

`power-match-polish-browser-test.mjs` ve Unity `-power-match-polish-test` görünür birleşmeyi, kaybolma sırasını ve güç göstergesini kontrol eder. `smash-match-model-test.cjs` daha yavaş zamanlamayla düşme, üçlü rezervasyonu ve son animasyon sonrasında tek ödül/zafer akışını doğrular.
