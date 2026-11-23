# Top mağazası ve coin hesabı

Unity ve bağımsız HTML sürümleri aynı ürünleri ve hesap kurallarını kullanır.

| Ürün | Fiyat | Davranış |
| --- | ---: | --- |
| Klasik | Ücretsiz | Başlangıçta açık |
| Mermer | 90 coin | Kalıcı top görünümü |
| Yörünge | 180 coin | Kalıcı top görünümü |
| Galaksi | 300 coin | Kalıcı top görünümü |
| 1 swap hakkı | 10 coin | Oyun ekranındaki SWAP üzerindeki `+` penceresinden alınır |

Top mağazası ana menüden ve oyun ekranındaki mağaza simgesinden açılır. Satın alma görünümü hemen seçer; sahip olunan topları yeniden seçmek ücretsizdir. Aynı ürün ikinci kez ücretlendirilemez. Görünüm, aktif 3D topa ve sıradaki top simgelerine uygulanır. Pembe/turkuaz/mor renk kuralı, topun kütlesi, atış hızı ve çarpışma davranışı korunur.

Ana SWAP düğmesi ilk topu bütün kuyruğun sonuna taşır ve bir hak tüketir. `+` ayrı bir satın alma penceresi açar; kuyruğu döndürmez ve hamle harcamaz. Onayda tam 10 coin düşülür, tam bir hak eklenir. Vazgeçmek harcama yapmaz. Eski üst `+1 Swap · 10 coin` düğmesi ve ekran görüntüsünde işaretlenen üst açıklama kaldırıldı. Fiyat yalnızca satın alma penceresinde gösterilir.

## Hesap kuralları

- Başlangıç: 0 coin, 3 swap hakkı, Klasik top.
- İlk bölüm zaferi: **30 + kalan hamle × 2 coin**. Örneğin 3 hamle kalırsa 36 coin.
- Sonuç ekranı bölüm ödülünü, hamle bonusunu, toplam kazancı ve bakiyeyi gösterir.
- Aynı bölümde aynı veya daha kötü sonuç yeniden para üretmez. Daha iyi sonuç yalnızca bonus farkını öder: 3 hamleden 5 hamleye iyileştirme +4 coin.
- Yenilgi, iptal, yeniden başlatma, mağazayı açıp kapama ve görünüm seçimi para üretmez. Yeniden başlatmak harcanmış hakları geri getirmez.
- Tüm işlemler tam sayı ile hesaplanır. Yetersiz bakiye, geçersiz ürün ve sahip olunan ürün satın alımı harcama yapmaz.
- Coin üst sınırı 999.999.999, hak üst sınırı 999. Sınıra ulaşan bir ödül yalnızca mevcut kapasiteyi krediler; limit nedeniyle alınamayan kısım sonradan tekrar oluşturulmaz. Hak limiti kontrolü ücret düşülmeden yapılır.
- Dar HUD bakiyeyi aşağı yuvarlanan K/M biçiminde gösterir. Mağaza ve satın alma pencereleri tam bakiyeyi gösterir.

Unity'de `PinPop.Economy.v2`, HTML'de `pinpop-html-economy-v2` tek cüzdan kaydı; bakiye, hak, sahiplik, seçili görünüm ve bölüm ödül geçmişini birlikte saklar. HTML her işlemden önce son kaydı tekrar okur ve diğer sekmelerdeki kayıt değişimini dinler. Bu yerel bir oyun kaydıdır; sunucu tabanlı ekonomi değildir. İki motorun kayıtları birbirinden ayrıdır.

Eski coin/hak ve bölüm ilerlemesi yeni kayda taşınır. Önceden tamamlanan eski bölümlerin taban ödülü tekrar verilmez. Sahiplik, bakiye ve ödül geçmişi bölüm değişiminden ve uygulamayı yeniden açmaktan etkilenmez.

## Güncel doğrulama

- `economy-core-test.cjs`: 37/37 hesap, sahiplik, kayıt, eski kayıt geçişi ve limit kontrolü.
- `economy-browser-test.mjs`: 32/32 gerçek tarayıcı kontrolü. Çek-bırak ile kazanma, bonus metni, mağaza alımı/seçimi, `+` ve ayrı SWAP işlemi, kayıt yükleme, yetersiz/stale bakiye, hak limiti, 320/390 piksel ve yatay görünüm.
- Unity `-economy-test`: 82/82 kontrol. Gerçek çekiş/atış, zafer ödemesi, aynı bitiş çağrısının korunması, gerçek UI raycast'leri, tüm ürünleri alma/seçme, yeniden yükleme ve 3D top malzemesi; top rengi, kütlesi ve hızı değişmiyor.
- `economy-progression-test.cjs`: 12 bölüm düzeni de sıfır coin/sıfır hak ile tamamlandı; her zafer tam ödül verdi. Ücretli swap ilerlemek için zorunlu değil.
- Unity `-economy-progression-test`: 24/24 kontrol; 12 bölüm düzeni de sıfır coin/sıfır hak ile kendi hamle bütçesinde tamamlandı ve 30 + kalan hamle × 2 ödülünü aldı. Sonuç: `economy-progression-unity-verification.txt`.

Bu özellik için eski sabit 30 coin ve mağazadan swap alma varsayımlarını içeren tarihsel raporlar yerine yukarıdaki ekonomi testleri kullanılır. Test cüzdanları kullanıcının Unity kaydından önce yedeklenir ve test bitiminde geri yüklenir. Tarayıcı testleri ayrı bir Chrome profili gerektirir ve o profilin yerel kaydını sıfırlar.

Proje kökünden çalıştırma:

```sh
node Documentation/economy-core-test.cjs
node Documentation/economy-progression-test.cjs
# Ayrı test Chrome profili, remote debugging port 9224 ile açıkken:
node --experimental-websocket Documentation/economy-browser-test.mjs
# Derlenmiş Unity uygulaması:
PINPOP_QA_DIR="$PWD/Documentation" Builds/PinPop.app/Contents/MacOS/PinPop -economy-test -logFile /tmp/pinpop-economy.log
PINPOP_QA_DIR="$PWD/Documentation" Builds/PinPop.app/Contents/MacOS/PinPop -economy-progression-test -logFile /tmp/pinpop-progression.log
```
