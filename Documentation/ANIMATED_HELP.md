# Otomatik ve kaydırılabilir nasıl oynanır

Rehberde gezinme, tekrar oynatma, kapatma veya yardım açma düğmesi yoktur. Ana ekrandaki ve oyun ekranındaki soru işaretleri kaldırıldı. Üç adım göstergesi ve kısa kaydırma ipucu korunur. Mini gösterimin içindeki SWAP bir resimdir; canlı oyun düğmesi değildir.

## Akış

1. Bölüm 1'e ilk gerçek girişte, yükleme perdesi kapandıktan sonra rehber kendiliğinden açılır. Ana menüde, uygulama başlatılırken veya diğer bölümlerde açılmaz.
2. Çek/bırak gösterimi 5,4 saniye sonra renk/üçlü gösterimine geçer.
3. Renk/üçlü gösterimi 7,8 saniye sonra swap gösterimine geçer.
4. Swap gösterimi 5,5 saniye sonra rehberi kapatır; aynı canlı bölüm durduğu yerden devam eder.

Sola kaydırma sonraki sahneye, sağa kaydırma önceki sahneye gider. Son sahnede sola kaydırma doğrudan oyuna döner. İlk sahnede sağa kaydırma dönüp son sahneyi açmaz. Kısa dokunmalar ve dikey sürüklemeler sahneyi değiştirmez. Fareyle sürükleme de desteklenir; çoklu dokunmada ilk temas takip edilir. Kullanıcı parmağını basılı tuttuğunda sayaç durur; bırakıldığında devam eder. Ekran yönü değişiminde sahne ve oynatma konumu korunur. Uygulama/sekme arka plandayken sahneler tüketilmez.

İlk gösterim başladığında görülme kaydı hemen saklanır. Bölüm yeniden başlatıldığında veya uygulama/sayfa yeniden açıldığında tekrarlanmaz. Bölüm 1'i önceden geçmiş eski kayıtlar da yeniden gösterime zorlanmaz. Escape/geri sistemiyle kapatma korunur; ekranda bunun için düğme oluşturulmaz.

## Kayıt ve uygulama

Unity anahtarı: `PinPop.HowTo.Seen.v1`. HTML anahtarı: `pinpop-html-howto-seen-v1`. Mevcut ekonomi, kozmetik ve ilerleme kayıtları değişmez.

Unity `AnimatedHowTo` unscaled süreyle oynar ve Unity pointer/drag olaylarını işler. HTML `animated-help.js` canvas ve pointer olaylarını kullanır; kaynak tek dosyalık `index.html` içine gömülür. İki sürüm de gösterim sırasında canlı oyunu durdurur ve duba, tepsi, coin, hamle, swap veya sıraya dokunmaz. Hareket azaltma açıkken sabit öğretici pozlar gösterilir, süreli otomatik geçiş devam eder.

Kart geçişleri, ahşap yüzey, temas gölgeleri, kavisli figür hareketleri ve sınırlandırılmış birleşme efektleri korunur.

## Doğrulama

Unity `-animated-help-test` ve `Documentation/animated-help-browser-test.mjs`: ilk bölümde otomatik açılma, diğer bölümlerde açılmama, düğmesiz görünüm, parmak/fare kaydırma, otomatik üç sahne/bitiş, yeniden giriş ve sayfa yeniden yüklemesinde tekrar etmeme, canlı oyun durumunun korunması, 320/390/yatay görünüm, yön değişiminde kaldığı yerden devam ve hareket azaltma. QA kullanıcının görülme kaydını ve cüzdanını yedekleyip geri yükler.

Son teslim: Unity 43/43, HTML 30/30 kontrol geçti. Üretim derlemesi ile dört ilgili C# kaynağı ve HTML içine gömülen kaynak birebir karşılaştırıldı.
