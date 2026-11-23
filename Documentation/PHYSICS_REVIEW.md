# Duba ve top fiziği incelemesi

## Bulgu ve değişiklik

Topun ilk darbe hızı yeterliydi. Devrilen dubalarda yüksek masa sürtünmesi hareketi kısa sürede kesiyor; dubalar masada yatık kalıyordu. Tüm temaslar için kullanılan 0,45 sürtünme yerine duba–masa temasına özel **0,14** uygulanır. Top–masa, top–duba ve duba–duba sürtünmeleri 0,45; sekme 0,05 kalır. Top kütlesi 6, duba kütlesi 1, atış hızı 4–24, duba ağırlık merkezi, collider şekilleri ve sürekli çarpışma algılama ayarları korunur. Aynı duba–masa katsayısı bağımsız HTML sürümüne de uygulanır.

Dubalar yatınca silinmez; masadan düşüp mevcut y < -0,8 eşiğini geçtiğinde toplanır. Farklı renkler sabit fiziksel engeldir. Sabit top başlangıcı, duba görselleri, hamle bütçeleri ve ödüller bu çalışmada değişmez.

## Gerçek PhysX karşılaştırması

Üç bölümde (1, 3, 6), ön sıradaki iki aynı renk dubanın ortasına aynı nişanla %60 ve %100 güçte atış yapılır. Her atış için beş saniyelik fizik sonucu ölçülür. Her katsayı için altı, toplamda 18 gerçek atış çalışır. Ölçüm sırasında oyun saati dört kat hızlanır; fizik adımı 120 Hz olarak kalır. Son üretim oyun hızı normaldir.

| Duba–masa sürtünmesi | Altı atışta masadan düşen | Masada yatık kalan | Ortalama yatay hareket |
| --- | ---: | ---: | ---: |
| 0.45 | 8 | 17 | 1.18 birim |
| 0.24 | 11 | 13 | 2.01 birim |
| 0.14 | 17 | 7 | 2.79 birim |

Son karşılaştırmanın atış bazındaki sonuçları:

| Bölüm | Güç | Önce (0,45) | Sonra (0,14) |
| --- | --- | ---: | ---: |
| 1 | %60 | 2 | 4 |
| 1 | %100 | 2 | 6 |
| 3 | %60 | 1 | 1 |
| 3 | %100 | 1 | 1 |
| 6 | %60 | 1 | 2 |
| 6 | %100 | 1 | 3 |

Seçilen ayar, ölçülen altı atışın hiçbirini kötüleştirmedi; ilk bölümde tam güçlü atışın düşürdüğü duba sayısı 2’den 6’ya çıktı. Fiziksel temasların küçük zaman farkları sonucu sayıları az miktarda değişebilir; bu sonuçlar belirtilen atış setine aittir. Her bölümde aynı oranda artış iddia edilmez.

`BowlingBall.FirstIncomingVelocity`, ilk duba temasından hemen önceki hızı ayrı kaydeder. Böylece sonraki çarpışmaların hızını ilk darbe sanma hatası önlenir. Son ölçümde aynı atışların ilk darbe hızları iki sürtünme ayarında eşittir: %60 güçte yaklaşık 15,77; tam güçte 23,77 birim/sn. İyileşme topu güçlendirmekten değil, dubanın masa üzerindeki hareketinin erken kesilmemesinden gelir.

## Doğrulama

- `-physics-review-test`: 18 karşılaştırmalı atış, sonuç iyileşmesi, eşit ilk darbe hızı, vuruş almayan dubaların sabit kalması ve doğrudan yanlış renk top temasında duba kilidinin korunması.
- `-pin-behavior-test`: 12 bölümün collider düzeni, altı yönde sabit çekiş, renkler arası katı temas, güçlü atışlarda gövde ayrıklığı ve duba görselleri.
- `-smash-match-test`: düşme, üçlü eşleşme, video ile devam, son animasyondan sonra zafer ve ödül kontrolleri.
- `physics-review-html-test.cjs`: HTML’de aynı sürtünme ayrımı ve eşit top kütlesi/hızıyla bir örnek atışta iyileşme.
- `physics-review-comparison.json`: ölçümün ham sonuçları.

Önce/sonra görüntüleri `physics-review-0.45-1.0.png` ve `physics-review-0.14-1.0.png` dosyalarındadır.

Son sürüm: karşılaştırma **23/23**, sabit top/collider kontrolleri **35/35**, düşme ve eşleşme kontrolleri **44/44**, model **33/33** ve HTML fizik karşılaştırması geçti. Kaynak ve son derleme eşliği `physics-review-final-audit.json` dosyasındadır.
