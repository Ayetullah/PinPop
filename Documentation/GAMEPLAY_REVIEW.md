# Oynanış karşılaştırması ve düzeltmeler

20 Kasım 2026. Karşılaştırma, aşağıdaki oyunların resmi ürün açıklamaları ile PinPop'un çalışan HTML ve Unity sürümlerinin incelenmesine dayanır. Rakip oyunların cihazda oynanarak ölçüldüğü iddia edilmez.

| Referans | Resmi üründe belirtilen özellik | PinPop için değerlendirme |
| --- | --- | --- |
| [Knock Balls — Voodoo](https://voodoo.io/games/knock-balls) | Kolay öğrenilen tek dokunuş kontrolü, yapıları toplarla devirme, görsel darbe efektleri | PinPop'un çekip bırakma kontrolünde oyuncuya mevcut topun hangi rengi devirebildiğini nişan anında anlatmak gerekir. |
| [Hit & Knock down — mobirix](https://play.google.com/store/apps/details?id=com.mobirix.throwball&hl=en_US) | Basit kontrol, sınırlı top modu, hedefleri devirerek bölüm tamamlama | Kalan hamle ve gelecekteki top bilgisinin tutarlı olması, son atışın fiziksel sonucu görülmeden kayıp ilan edilmemesi gerekir. |

Rakiplerden çıkarılan tasarım kararları, resmi açıklamaların birebir sunduğu özellikler değil, PinPop'a yönelik değerlendirmelerdir.

## Uygulanan değişiklikler

1. **Uçuş sırasında doğru top sırası.** Atılan top kuyruktan hemen çıkarılıyordu; önizleme ise hâlâ ikinci elemandan başladığı için sıradaki kullanılabilir topu atlıyordu. Top uçarken önizleme kuyruk başından, yeni top hazır olduğunda onun arkasından başlıyor. Gösterilen top sayısı kalan kullanılabilir hamleyle sınırlı. Bir hamle kalan uçuşta sıradaki tek top görünür; son atışta hayali ek top görünmez.
2. **Nişan anında renk yönlendirmesi.** Hazır durumda “Topu geriye çek · Aynı rengi devir”, yeterince çekildiğinde mevcut topun rengiyle “PEMBE dubaları hedefle · Bırak” gibi bir yönerge gösteriliyor. Zayıf çekişte mevcut güç uyarısı korunuyor. Renk kuralı yalnızca görsel renk farkına bırakılmıyor.
3. **Çözülmüş bölümde gereksiz bekleme kaldırıldı.** Bütün dubalar temizlendiğinde son darbeye 0,45 saniye bırakılarak zafer açılıyor. Çözülmemiş bölümlerde en az 2,8 saniyelik uçuş ve en fazla 4,8 saniyelik zincirleme fizik penceresi korunuyor. Son hamlede de başarı kontrolü kayıp kontrolünden önce; ödül bir kez veriliyor. Duraklatma bu süreleri de durduruyor.
4. **Yükleme UI yaşam döngüsü sağlamlaştırıldı.** Yükleme perdesi ve sonuç grafikleri, RectTransform ve CanvasRenderer ile etkisiz durumda kurulup hazırlandıktan sonra etkinleştiriliyor. UI yeniden kurulurken eskileri devre dışı bırakılıyor. Aynı karede birden fazla yeniden kurulumdan sonra raycast ve sonraki karede perdenin tıklama engellemesi test ediliyor.

İlk topu tüm kuyruğun sonuna taşıyan SWAP davranışı, kredi maliyeti, bölüm ödülü, renk dışı dubaların fiziksel engel olması ve okunabilir yükleme süresi korunur. Oynanış değişiklikleri hem Unity hem bağımsız HTML sürümüne uygulanmıştır.

## Doğrulama

- `browser-comparison-test.mjs`: gerçek tarayıcıda açılış, gerçek klavye atışıyla hazır/uçuş/yeniden hazır önizlemeleri, son hamlede erken kayıp olmaması, kısa son darbe süresi, erken zafer ve tek ödül. Sonuç: `comparison-browser-verification.json`.
- Unity `-pull-test`: gerçek uçuş, kuvvet ve renk filtresi kontrollerine uçuş sırası, tek kalan top, yeniden hazır sıra ve kontrollü son hamle/ödül senaryoları eklendi. Sonuç: `pull-unity-verification.txt`.
- Unity `-self-test`, `-collision-test`, `-polish-test`: gerçek bölüm oynanışı, çarpışmalar, mağaza/SWAP, sonuç ve tekrar ekranları.
- Unity `-transition-test`: yükleme perdesi, aynı karede tekrar UI kurma, ekran boyutu değişimi ve azaltılmış hareket.
- Görsel inceleme: `unity-pull-aim.png` renk yönergesini; `comparison-html-flight.png` uçuş sırasında gerçek sıradaki topları gösterir.

Unity derlemesi açık kullanıcı projesine müdahale etmeyen geçici proje kopyasında alınır; başarılı çıktı `Builds/PinPop.app` konumuna aktarılır. Testler oyuncunun kayıtlı cüzdan ve bölüm ilerlemesini geri yükler.

Son doğrulama: Unity çekiş/oynanış 41/41, gerçek bölüm 23/23, çarpışma 36/36, ekran akışı 36/36, yükleme geçişi 31/31; tarayıcı 10/10. Toplam 177/177 kontrol geçti. Test günlüklerinde MissingComponentException, ArgumentException veya NullReferenceException yok.
