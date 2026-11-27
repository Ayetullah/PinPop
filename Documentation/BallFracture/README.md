> Güncel çim süresi **2 saniyedir**: [güncel ayar ve doğrulama](../Grass2Seconds/README.md). Aşağıdaki 10 saniyelik kayıt önceki sürüme aittir.

# Bowling topunun parçalanması

Önceki toz/küçülme efekti kaldırıldı. Küre doğrudan kendi hacmini oluşturan 20 kapalı mesh parçaya ayrılır. Her parçada kavisli dış yüzey ve daha koyu, opak kırık yüzeyler vardır. Topun seçili kaplaması dış yüzeylerde korunur; gökkuşağı topu çok renkli parçalanır.

Parçalar dışarı ve yukarı savrulup döner; tahtada veya çimde yerçekimiyle düşüp kısa sekmeler yapar. İlk 0,72 saniye tam boy kalır, son bölümde küçülerek temizlenir; toplam süre 1,1 saniyedir. Bunlar kozmetik parçalardır: collider, Rigidbody veya gameplay etkisi eklemezler. Kaynak meshler ve malzemeler önbelleğe alınır.

## Tetikleme

- Tahtaya temas eden top durduğunda dört fizik adımlık (0,08 sn) titreşim filtresinin ardından parçalanır. Oyuncuya geri sayım gösterilmez; çimdeki 10 saniye veya sonraki atış beklenmez.
- Atış tamamlandığında hâlâ tahta üzerinde desteklenen top aynı anda parçalanır. Böylece çok yavaş yuvarlanan bir top da sıra bitince ortada kalmaz. Havada veya çimdeki top bu temizliğe dahil edilmez.
- Çimde sürtünme, yuvarlanma ve yere ilk temasından itibaren 10 saniyelik ömür korunur. Süresi dolunca aynı parçalanma oynar. Yeni atış çimdeki önceki topun ömrünü kısaltmaz.
- Bölüm yükleme eski top ve parçaları perde arkasında doğrudan temizler. Duraklatma ömür sayacını durdurur; başlamış kozmetik parçalanma duraklatılsa da tamamlanır.
- Azaltılmış hareket ayarı eklenmedi.

## Doğrulama

Unity 6000.3.24f1 macOS Development derlemesi başarılı. Native Unity çalıştırmasında 34 kontrol başarılı, 0 hata; test tercihleri geri yüklendi. Tahtada gerçek atışın bitişi, fiziksel durma, yavaş yuvarlanma, havada kalma, çime gerçek atış ve gerçek 10 saniyelik ömür, renkler, sekmeler, temizleme ve bölüm sıfırlama sınandı. Fiziksel iPhone testi bu turda yapılmadı.

verification.json sonuçları, build.log derleme kaydı ve source-hashes.json derlenen kaynak eşleşmelerini içerir. table-*.png normal oyun kamerasındaki evrelerdir. grass-*.png kırık yüzeyleri ve çim sekmesini incelemek için QA sırasında kameranın yaklaştırıldığı detay görüntüleridir; oyunun kamerası değişmedi.
