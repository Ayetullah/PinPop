# Açılı temas ve fizik

Topun hızını çarpışma sonrasında dubanın merkezine doğru zorla çeviren kod kaldırıldı. Dubalar da hazır bir tilt animasyonuyla devrilmez.

HTML sürümü gömülü Cannon-es 0.20.0 ile, Unity sürümü PhysX ile gerçek dinamik 3B cisimler kullanır. Yer çekimi, doğrusal/açısal hız, kütle, eylemsizlik, temas noktası, sürtünme ve esneklik fizik motoruna aittir. Görseller cisim konumu ve quaternion dönüşünden çizilir. Sağ/sol kenardan vuruş topu farklı tarafa saptırır; merkezden vuruş yan yön uydurmaz. Ağır top her çarpmada geriye dönmez; momentumunu dubalara aktarırken ilerlemeye devam edebilir.

Top kütlesi sabit 6, duba kütlesi 1 birimdir. Çekiş gücü kütleyi değiştirmez; yatay ilk hızı 4–12 aralığında belirler. Temas esnekliği 0.38 ve sürtünme kullanılır. Top aynı dubaya tekrar çarpabilir; ses/parçacık bildirimi aynı top-duba çifti için bir kez üretilir. Farklı renk duba, atış boyunca sabit bir engeldir: topu temas normaline göre sektirir; kendisi hareket etmez, devrilmez ve puana sayılmaz. Sonraki top onun rengindeyse yeniden dinamik olur. Duba/duba arasında farklı renk çarpışması kapalıdır. Masa ve zemin bütün renklere etki eder.

HTML dubasının çarpışma şekli alt gövde, boyun ve baştan oluşan bileşik cisimdir. Unity de alt gövde ve boyun için convex mesh, baş için küre içeren bileşik collider kullanır. Bu nedenle iki motor aynı isabette birebir aynı sayıda duba devirmek zorunda değildir. Her ikisinde fiziksel çarpışma sonucu kullanılır; sabit duba sayısı veya hedefe otomatik yönlendirme yoktur.

## Kanıt ve tekrar çalıştırma

- `node Documentation/physics-core-test.cjs`: 49/49. Kod doğrudan mevcut index.html içinden okunur; sağ/orta/sol açılarda momentum ve enerji davranışı ölçülür.
- `node --experimental-websocket Documentation/browser-test.mjs`: ayrı Chrome CDP oturumu 9224 ve yerel HTTP sunucusu 8765 ile 60/60 ana akış + 7/7 gerçek ekran sekmesi. Fizik, gerçek sürükleme, dokunma, kazanma ve ekonomi kontrol edilir.
- macOS uygulaması `-collision-test`: 36/36; `-self-test`, `-pull-test`, `-polish-test`, `-visual-test`: toplam 85/85. Testler kullanıcının cüzdanını başlangıçta kaydedip çıkışta geri yükler.

Raporlar: `collision-core-verification.json`, `collision-browser-verification.json`, `collision-unity-verification.txt`. Uygulama açık Unity editörüne dokunmadan geçici proje kopyasında derlenmiştir.

Motor başvuruları: [Cannon-es](https://github.com/pmndrs/cannon-es), [Cannon-es collision filters](https://github.com/pmndrs/cannon-es/blob/master/examples/collision_filter.html), [Unity PhysicsMaterial](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/physicsmaterial).

## Sabit farklı renk kuralı

Atış başında yalnızca aktif topun rengindeki dubalar hareketlidir. Diğer dubalar mevcut konumlarında sabitlenir. Fizik motoru bu dubaları sabit yüzey olarak çözer; sekme sırasında hız kaybedilir. Temas için hedefe yönlendirme veya özel bir sekme yönü atanmaz. Temas normalini ve esnekliği motor hesaplar. Her atışta önceki top kaldırılarak iki farklı rengin aynı anda kilit durumunu değiştirmesi önlenir.

Farklı renk temasında yaklaşık 0.18 saniyelik sıcak beyaz parıltı ve 105 Hz kısa tok ses vardır. Hasar parçacığı veya puan verilmez. Doğru renkle vurulduğunda normal devrilme ve puan davranışı sürer.
