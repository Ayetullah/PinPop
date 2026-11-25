# PinPop ayrıntılı hata ve polish incelemesi

> Durum: Bu rapordaki altı bulgu uygulandı ve yeniden doğrulandı. [Düzeltmeler ve son testler](../DeepAuditFixes/README.md). Aşağıdaki kayıt, düzeltme öncesindeki inceleme kanıtlarını korur.

İnceleme, dil seçimi polish çalışmasından sonraki üretim kodu üzerinde yapıldı. Çalışan Unity oyuncusu, gerçek fizik, Türkçe/İngilizce arayüzler ve bağlı iPhone 13 kullanıldı. Üretim kodu değiştirilmedi; tespitler aşağıda düzeltme önerileriyle kaydedildi. Kullanılan üretim dosyalarının hash’leri `production-source-manifest.json` içindedir.

## Öncelikli, yeniden üretilmiş bulgular

### 1. P2 — Buz materyalleri bölüm tekrarlarında birikiyor

**Tekrar:** Buz bulunan 5. bölümü yükle, ardından aynı bölümü 20 kez yeniden başlat. Eski dubalar ve görseller silinse de oluşturulan buz materyalleri silinmiyor.

**Ölçüm:** İlk yüklemede 4 materyal; 20 yeniden başlatmadan sonra 84 materyal. Sahnede hâlâ yalnızca 4 buz kabuğu var. Ölçüm çöp toplama sonrasında yapıldı. Bu, uzun oturumlarda gereksiz yerel grafik belleği birikimi yaratır; bu test bir çökme veya belirli bir bellek limiti göstermemiştir.

**Kaynak:** [PinTarget.Protect](../../Assets/Scripts/PinTarget.cs#L40) her kabuk için `new Material` oluşturuyor. [OnDestroy](../../Assets/Scripts/PinTarget.cs#L73) yalnızca gölgeyi temizliyor; buz kırıldığında da materyalin sahibi temizlenmiyor.

**Düzeltme:** Buz materyalini paylaşmak veya sahipliğini izleyip görsel/duba yok olurken temizlemek. Uzun bölüm tekrar testinde materyal sayısı sabit kalmalı.

### 2. P2 — Nişan göstergesi engelleri hesaba katmıyor

**Tekrar:** 3. bölümde başlangıç topunu sol engelin karşısına kaydırıp düz ileri nişan al.

**Ölçüm:** Top yarıçapıyla yapılan fizik taraması 3,05 birimde sol engele çarpıyor. Oyunun tahmini hedefi ise 6,51 birim ileride kalıyor. Nişan çizgisi engelin içinden geçerek arkasındaki hedefe uzanıyor; oyuncuya mümkün olmayan doğrudan isabet hissi veriyor.

**Kaynak:** [PredictRoll](../../Assets/Scripts/PinPopGame.cs#L222) masa sınırları ve dubaları tarıyor; sabit/hareketli bariyerleri taramıyor.

**Düzeltme:** Top yarıçapını hesaba katan ilk temas sorgusuyla nişanı engelde durdurmak; varsa sekme önizlemesini ayrı ve fizik kurallarıyla tutarlı göstermek.

[Kanıt: engelin içinden geçen nişan](Reproductions/aim-bumper.png)

### 3. P2 — Çoklu toplamada figürler tepsinin dışına taşıyor

**Tekrar:** 12 dubanın aynı karede toplandığı durumu üret. Bu fixture, 12 gerçek dubayı toplama eşiğinin altına indirerek normal `OnPinCleared` akışını kullanır; nişan/atış başarısını ölçmez. Özellikle gökkuşağıyla birden çok grubun düştüğü durumu sınar.

**Ölçüm:** 12 figür animasyonda, eşleşmemiş figür sayısı 0; buna rağmen 3 ikonun sınırları tepsinin dışına çıkıyor. Eşleşmiş üçlüler, birleşme/silinme bitene kadar `Items` içinde kaldığı için görsel indeksler 10 yuvayı aşıyor.

**Kaynak:** [Collect](../../Assets/Scripts/PinPopMatchTray.cs#L28) ve [HtmlMatchTrayUI.LateUpdate](../../Assets/Scripts/HtmlMatchTrayUI.cs#L37). Görsel yuva hesabı canlı kapasite yerine geçici tüm öğeleri de içeriyor.

**Düzeltme:** Bekleyen üçlüleri kontrollü görsel alanlarda birleştirmek; yeni uçuşlar için görünür yuva rezervasyonu yapmak. Yalnızca maske eklemek, taşan eşleşmeleri görünmez yapacağından tek başına yeterli olmaz.

[Kanıt: tepsi dışındaki figür](Reproductions/tray-multi-collect.png)

### 4. P2 — Kombo yazıları birbirinin üstüne biniyor

**Tekrar:** Yukarıdaki çoklu toplamada dört üçlüyü aynı anda birleştir.

**Ölçüm:** Aynı anda 4 kombo etiketi oluşuyor; 3 etiket çifti üst üste geliyor. “GÜZEL”, “HARİKA” ve “SÜPER” metinleri okunamaz hâle geliyor.

**Kaynak:** [HtmlMatchTrayUI.Burst](../../Assets/Scripts/HtmlMatchTrayUI.cs#L59) her üçlüye bağımsız, geniş bir metin oluşturuyor; eşzamanlı metinlerin konumlarını düzenlemiyor.

**Düzeltme:** Tek kombo alanını güncellemek veya metinleri kısa bir sırayla/çakışmayacak katmanlarda göstermek.

[Kanıt: kombo metinleri](Reproductions/tray-multi-collect.png)

### 5. P2 — Tepsi dolunca açılan sonuç akışı eksik düzenlenmiş

**Tekrar:** Beş renkten ikişer duba aynı karede toplansın; 10 eşleşmemiş figürle kaybetme oluşur.

**Gözlenenler:**

- Kart küçülüyor fakat Ana Menü düğmesi eski konumda kalıp alt çerçeveden taşıyor. 1170×2532 çıktıda yaklaşık 7,6 piksel taşma ölçüldü; görselde de doğrulandı.
- Reklam düğmesi yokken “Her denemede 1 kez · Coin harcanmaz” reklam açıklaması kalıyor.
- Oyun hemen durduğu için bu toplu örnekte 10 figürden hiçbiri uçuşunu tamamlamıyor. Üstteki tepsi boş görünürken “Toplama alanı doldu” deniyor. Daha yavaş toplamada da son gelen figür havada donabilir.

**Kaynak:** [OnPinCleared / End](../../Assets/Scripts/PinPopGame.cs#L263) sonucu ve duraklamayı hemen başlatıyor. [RefreshRecoveryUI](../../Assets/Scripts/HtmlReferenceHUD.cs#L70) kart için `VideoRescueUsed || TrayOverflow`, düğme konumları ve durum yazısı için yalnızca `VideoRescueUsed` kullanıyor.

**Düzeltme:** Taşma sonucu için ayrı, tutarlı bir yerleşim ve metin durumu; oynanışı kilitleyip son toplama animasyonunu tamamladıktan sonra sonuç gösterimi.

[Kanıt: kaybetme ekranı](Reproductions/overflow-result.png)

### 6. P3 — Nasıl oynanır kapatıldıktan sonra tekrar açılamıyor

**Tekrar:** İlk öğreticiyi kapat; 1. bölümde oyun ve ayarları aç.

**Gözlem:** Her iki ekranda da görünür yardım girişi sayısı 0. Otomatik ilk açılış çalışıyor; fakat tekrar izlemek isteyen oyuncu için giriş yok. Eski `HELP_GAME` kontrolü yeni arayüz kurulurken gizlenmiş ve yeni karşılığı oluşturulmamış.

**Kaynak:** [BuildCrispInterfaceRoutine / BuildNativeGame](../../Assets/Scripts/HtmlCrispHUD.cs#L20), [TryShowFirstPlayHelp](../../Assets/Scripts/PinPopHUD.cs#L15).

**Düzeltme:** Ayarlara küçük bir “Nasıl oynanır” işlemi eklemek. Kapatma çarpısı yeterli olmaya devam edebilir; yeniden Tamam düğmesi gerekmez.

## Ek polish ve ölçekleme notları

- **Sandık ödül metni:** `+1 +3 hamle` / `+1 +3 moves` biçimi teknik envanter bilgisini iki adetmiş gibi okutturuyor. “1 adet +3 Hamle yardımcısı” veya ikon + adet daha anlaşılır olur. [Gerçek cihazda üretilen metin](iPhone13/milestone-result.png), [NativeResult](../../Assets/Scripts/HtmlCrispHUD.cs).
- **Dil değiştirme:** iPhone’daki bir Türkçe geçişinde 67,1 ms’lik tek kare görüldü. Çoğu kare yaklaşık 16,7 ms. Bu tek ölçüm sürekli takılma kanıtı değildir; bütün arayüzü eşzamanlı yeniden kuran dil değişimi küçük bir optimizasyon adayıdır.
- **1000+ bölüm hedefi:** Güncel kampanya 40 bölümle sınırlı. Bu mevcut içerik sınırıdır. Gelecek ölçekleme için `AdventureRules.Chapters`, tek `int` alanındaki sandık bitleri ve sabit yatay bölge düğmeleri yeniden ele alınmalı. Bu inceleme 1000 bölümün hazır/oynanabilir olduğunu göstermiyor.
- **AdMob ilk onayı:** Kodda SDK/onay hazırlığı ve reklam yüklemesi aynı 12 saniyelik beklemeyi paylaşıyor. İlk onay formunda uzun kalan kullanıcı için erken “hazır değil” sonucu olasılığı var. Bu statik inceleme bulgusudur; gerçek reklam/onay akışında yeniden üretilmiş hata olarak sayılmadı.

## Yapılan doğrulama ve testlerin yorumu

| Kontrol | Sonuç | Yorum |
| --- | --- | --- |
| İngilizce arayüz | 372/372 | Ekranlar, güvenli alan, sonuçlar, yardımcı pencereleri, çarpılar ve metinler |
| Türkçe arayüz | 337/337 | Aynı arayüz taraması |
| Gerçek fizik çarpışmaları | 36/36 | Doğru/yanlış renk, fiziksel itme, sekme ve temas geri bildirimi |
| 40 bölüm kampanya | 146/148 | İki eski köşe koordinatıyla tıklama kontrolü başarısız; ilerleme/kampanya kontrolleri geçti. Yeni fixture gerçek buton merkezine raycast ile 20→21 geçişini doğruladı. |
| Video ile devam | 36/38 | Bir başarısızlık gerçek eksik yardım girişidir (#6). Diğeri eski 8 hamle beklentisidir; mevcut bölüm bütçesi değişmiştir. Video tamamlanması/iptali, tek ödül, durum ve cüzdan korunması kontrolleri geçti. |
| Yardımcılar ve engeller | 65/65 | Renk seçme, gökkuşağı, cep, +3 hamle; buz/bağ açılması, satın alma ve sandık kaydı |
| Hedefli yeniden üretim | 6 bulgu | 20 bölüm tekrarı ve toplama fixture’ları; `Reproductions/observations.json` |
| Gerçek iPhone 13 | Tamamlandı | 1170×2532, hata listesi boş, cüzdan ve dil tercihi korundu |

Eski testlerdeki başarısızlıklar gizlenmedi; raporları `Suites` klasöründedir. Butonun köşesini tıklayan eski kontrolün sonucu, butonun merkezinin oyuncu için çalışmadığı anlamına gelmiyor. Hedefli gerçek pointer kontrolü bunu ayırdı.

İlk hedefli fixture yanlışlıkla buz korumalı bölümde bazı dubaları toplamaya çalıştı ve test kurulumunda null hatası verdi. Korumasız bölümle düzeltildi; final fixture tamamlandı, hata listesi boş ve tercih yedeği geri yüklendi. İlk fixture hatası üretim oyunu hatası olarak sayılmadı.

## iPhone ölçümü

Gerçek iPhone 13 inceleme paketiyle menü, giriş, boşta oyun, nişan, mağaza, ayarlar, iki dil seçimi, dört yardımcı penceresi, harita, dört öğretici animasyonu, nasıl oynanır, sonuçlar ve SWAP penceresi gezildi. Yerel fizik/video testleri ayrıca gerçek Unity oyuncusunda çalıştırıldı.

- Menü, nişan ve çoğu arayüz bölümünde medyan yaklaşık 16,7 ms/kare, yani yaklaşık 60 FPS.
- Splash açılışı sırasında en uzun hazırlık karesi 349,9 ms; görünür splash geçişinin en uzun karesi 20,4 ms.
- Bölüm geçişinde en uzun ölçüm 34,0 ms; oyun boşta ve nişanda 50 ms üzerinde kare yok.
- Dil değişimindeki tek 67,1 ms kare yukarıda ayrıca belirtildi.
- Oyun çalışma zamanı hata/exception kaydı: 0.
- Cüzdan ve dil tercihi korundu. İnceleme bittikten sonra güncel normal Release tekrar yüklendi ve çalıştığı doğrulandı.

[Ham cihaz raporu](iPhone13/frame-health.json) · [Hedefli ölçümler](Reproductions/observations.json) · [Test koşuları](Suites/runs.json)

Bu tarama bütün 40 bölümü oyuncu gibi baştan sona bitiren bir oynanabilirlik kanıtı değildir. Gerçek çarpışmalar, örnek atışlar, kurtarma akışı, düzenler ve ilerleme ayrı ayrı sınandı. Uzun termal/pil oturumu ve canlı reklam kimlikleriyle yayın reklamı testi yapılmadı.

## Hedefli testi tekrar çalıştırma

`DeepAuditReview.cs` yalnızca izole inceleme projesinin Assets/Scripts klasörüne kopyalandı; oyunun üretim Assets klasörüne eklenmedi. Development oyuncusu `-deep-audit-test` argümanı ve `PINPOP_QA_DIR` çıktı klasörüyle çalıştırıldığında ölçümleri ve görüntüleri üretir. Buz materyali ölçümü öncesinde 5. bölüm 20 kez yeniden yüklenir. Geçici fixture kaynak dosyası izole proje klasöründen inceleme sonunda kaldırıldı; bu rapordaki kopya yeniden üretim için korunur.
