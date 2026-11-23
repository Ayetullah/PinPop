# Referans incelemesi ve uygulama kararları

İnceleme tarihi: 19 Kasım 2026.

Verilen Google Play arama bağlantısında aynı adı taşıyan birden fazla oyun bulunuyor. Aşağıdaki resmi mağaza açıklamalarının ortak mekanikleri temel alındı; uygulamalar kurulup oynanmadı. Bu notlar, mağazada doğrulanan özelliklerle PinPop için verilen tasarım kararlarını ayırır.

## İncelenen kaynaklar

- [Smash Fest! — FCgames](https://play.google.com/store/apps/details?id=com.FCgames.smashfest): dokunulan hedefe atış, fiziksel devrilme, platformdan düşen hedeflerin sayılması ve destek noktalarından zincirleme yıkım.
- [Smash Fest! — HelixCore Studio](https://play.google.com/store/apps/details?id=com.helixcore.stacksmash.physicspuzzle): sınırlı toplarla fizik bulmacası, farklı bölüm düzenleri, zincirleme etki ve renkli 3B ortamlar.
- [Smash Fest! — Y.G.I.LLC Innovations](https://play.google.com/store/apps/details?id=com.smashfest.game): parlak festival görselleri, darbe efektleri, kısa tekrar oynanabilir bölümler; mağaza güncelleme notlarında nişan önizlemesi ve hızlı tekrar deneme.

## PinPop'a uyarlama

| Referansta gözlenen temel yapı | PinPop uygulaması |
| --- | --- |
| Nişan al → ateşle → fizik sonucunu izle | Basılı tut → nişan çizgisi → bırak → bowling topu yuvarlanır |
| Sınırlı atış | Belirli hamle sayısı; atış anında bir azalır |
| Zincirleme yıkım | Aynı renkteki dubalar birbirini devirir |
| Platformdan düşen nesnelerin sayılması | Düşen veya yeterince eğilen duba temizlenir |
| Hızlı tekrar deneme | Sonuç kartında tek düğme ve R kısayolu |
| Etkili görsel geri bildirim | Parlak malzemeler, yumuşak gölge, parçacıklar, hafif kamera titreşimi, kısa sentez sesleri |

Kullanıcının özel kuralları: üç renk, farklı renkte fiziksel etki yok, aşağıda görünür top sırası ve coin ile alınan swap hakkıyla aktif topu kuyruğun sonuna taşıma. Farklı renkli dubalar arası çarpışmalar da engellenerek dolaylı renk kuralı ihlali önlendi.

PinPop modelleri, masa, UI görselleri ve sesleri kodla üretildi. Referans oyunun adı, görselleri veya modelleri kullanılmadı. Fontlar bu bilgisayardaki Arial fontlarından alınmıştır; harici oyun asset paketi gerekmez.
