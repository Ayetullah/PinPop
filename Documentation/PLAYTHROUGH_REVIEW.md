# Baştan sona oynanış incelemesi

## Kapsam ve yöntem

Ana menü → yardım → ayarlar → mağaza → yükleme → gerçek atışlar → zafer → sonraki bölüm → satın alma → yeniden başlatma → hamleleri bitirme → kaybetme → tekrar deneme → ana menü → kayıtları yeniden yükleme akışı incelendi.

Tarayıcıda fare ve klavye olayları gerçekten gönderilerek ilk bölüm kazanıldı, sonraki bölümde hamleler tüketilerek kaybetme ekranına ulaşıldı. Atışlar değerlendirilirken ayrı fizik örneklerinden yararlanıldı; oynanan oyunda duba silme, sonucu zorlama veya cüzdanı değiştirme yapılmadı. Unity yolculuğu da gerçek tutma/çekiş/bırakış kodundan ve raycast ile erişilen UI düğmelerinden geçer. Diğer regresyon testlerinde kontrollü durum fikstürleri ayrıca kullanılır.

Oyun devam eden bölüm yapısına sahiptir. İlk üç giriş düzeni ve ardından tekrarlanan dokuz düzen olmak üzere **12 farklı düzen** vardır. Hepsi gerçek fizik motorlarında, özgün hamle bütçeleriyle kontrol edilir; sonsuz sayıdaki bölümün tek tek elle oynandığı iddia edilmez.

## Düzeltilenler

| Bulgu | Değişiklik |
| --- | --- |
| Klavyeyle seçilen güç `%0` görünüyordu; HTML'de nişan rehberi de açılmıyordu. | Seçili klavye gücü hem göstergede hem rehberde kullanılıyor. Enter bu güçle atıyor. İptal ve odak kaybı klavye nişanını da temizliyor. |
| HTML'de Esc ile açılan ayarlar, aynı tuşun varsayılan dialog davranışı yüzünden hemen kapanabiliyordu. | Açma/iptal olayının varsayılan davranışı engellendi. Esc önce nişanı iptal eder, sonraki basış duraklatır. |
| Kuralların tekrar okunabildiği bir ekran yoktu. | Ana menüden ve oyun içinden `?` düğmesiyle açılan **Nasıl oynanır?** sayfası eklendi. Açıkken fizik durur; kapatınca hamle/cüzdan değişmeden devam eder. |
| Yardım metninin ilk Unity yerleşimi küçük ekranlarda kesiliyordu. | Yazı ölçeği seçilen raster profilinden bağımsız olarak gerçek görüntü genişliğine bağlandı; kısa, açık metin ve uyarlanmış kart yüksekliği kullanıldı. Başlık ve paragrafların sığması kontrol edilir. |
| Hareket azaltılmış HTML yüklemesi yaklaşık 0,12 saniyede geçiyordu. | Normal ve azaltılmış harekette en az 1,4 saniye okunabilir yükleme süresi sağlandı. Animasyon tercihi süreyi kısaltmaz. |
| Bölüm/kombo yazısı yeni uzun masada duba başlarının üstüne gelebiliyordu. | Yazılar oyun alanının üstündeki boş bölgeye taşındı. |
| Yeniden başlatmanın ilk yükleme anında eski atış devam edebiliyordu. | Eski bölümün çözümleme işlemleri ve fiziği hemen durdurulur. Eski ayarlar penceresi yüklemeyi açamaz; tarayıcıda önceki pencere yükleme başlamadan kapanır. |
| Sonuç ekranını başka bir pencereyle değiştirmek veya yanlışlıkla oyunu açmak mümkündü. | Pencere geçişleri sonuç/duraklatma durumunu korur; iç içe pencere açılması engellenir. Pencere kapanınca klavye odağı oyuna döner. |
| Pencerenin her farklı boyutu için tam ekran arka plan dokusu saklanıyordu. | Yan alanlar tek, küçük, yeniden kullanılan gradyanla çizilir; boyut değişimleri yeni büyük doku biriktirmez. |
| Her duba ve bazı darbe/iz efektleri aynı geometri veya materyali yeniden oluşturuyordu. | Duba temas şekilleri, gölge ve halka geometrileri; parçacık ve top izi materyali paylaşılır. Temas şekilleri ve fizik parametreleri korunur. |
| Kaydedilmiş bölüm 1000 ve sonrasında başlangıçta 999'a kırpılıyordu. | Kaydedilmiş ilerleme üstten kırpılmaz; yüksek bölüm numaraları korunur. |
| İptal edilmiş çekişten kalan işaretçi yakalaması sonraki hareketi etkileyebilirdi. | Yakalama iptalinde bırakılır; kayıp/iptal olayları yalnızca ilgili aktif hareketi temizler. Hızlı iptal/yeniden tutma da test edilir. |

## Doğrulama dosyaları

- `browser-journey-test.mjs` / `journey-browser-verification.json`: gerçek giriş olaylarıyla tam kullanıcı yolculuğu, kazanma/kaybetme, kayıt ve yüksek bölüm.
- `browser-gallery-test.mjs` / `journey-gallery-verification.json`: yardım, ayarlar ve mağazanın dört boyutta oturmuş animasyon sonrasındaki görüntüleri; bölüm yazısının dubaları kapatmaması.
- `browser-rolling-test.mjs` / `rolling-browser-verification.json`: gerçek fare/dokunma, renkler, yüzeyde yuvarlanma, iptal ve ekran boyutları.
- `browser-transition-test.mjs` / `transition-browser-verification.json`: yükleme ekranı, normal ve azaltılmış hareket, okunabilir süre.
- `physics-core-test.cjs` / `collision-core-verification.json`, `all-layouts-verification.json`: gerçek gömülü fizik kodu, temas/momentum ve bütün farklı düzenler.
- Unity `-journey-test` / `journey-unity-verification.txt`: gerçek çekiş koduyla kazanma, ekran akışları, yardım metni sığması, kaynakların yeniden kullanımı.
- Unity `-all-layouts-test` / `all-layouts-unity-verification.txt`: 12 düzenin PhysX ile mevcut hamle bütçelerinde tamamlanması.
- Unity `-self-test`, `-pull-test`, `-collision-test`, `-polish-test`, `-transition-test`, `-sync-test`, `-video-test`: fizik, ekonomi/kayıtlar, ekranlar, yükleme yaşam döngüsü ve yüksek çözünürlük regresyonları.

Unity testleri kullanıcının cüzdan ve bölüm kayıtlarını geri yükler. Tarayıcı testleri ayrı test profili kullanır. Derleme açık kullanıcı editörüne müdahale etmeyen geçici proje kopyasında alınır; başarılı çıktı `Builds/PinPop.app` konumuna aktarılır.

Görseller: `journey-unity-help-320.png`, `journey-unity-victory.png`, `journey-help-320.png`, `journey-entry-clear.png`, `journey-keyboard-aim.png`, `journey-victory.png`, `journey-failure.png` ve `journey-settings-*.png` / `journey-shop-*.png`.

## Sonuç

**503/503 kontrol geçti.** 12 farklı düzen hem Unity/PhysX hem HTML/Cannon-es ile tamamlandı. Gerçek çekiş yoluyla ilk bölüm Unity’de 3, tarayıcıda 4 atışta kazanıldı. Ana menüden zafer, sonraki bölüm, satın alma, kaybetme, tekrar deneme ve kayıtları yüklemeye kadar akışlar doğrulandı. Son günlük taramasında istisna veya başarısız kontrol yok. Yükleme iptali ve son hizalama için ilgili yolculuk, ekran akışı ve geçiş kontrolleri tekrar çalıştırıldı. Ayrıntılı toplam: `playthrough-verification.json`. Son sürüm: `Builds/PinPop.app` ve `index.html`.

Toplu ekran incelemesi: `playthrough-contact-sheet.png` (üst iki satır Unity, alt iki satır HTML).
