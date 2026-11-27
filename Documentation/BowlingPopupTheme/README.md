# Tüm oyun popup’larında bowling teması

Onaylanan tasarım ortak CarnivalPopup bileşenine uygulandı: pembe tavşanlı duba, üç delikli altın bowling topu, turkuaz kuşlu duba ve ince altın kemer. Çadır ve dönme dolabı çizimi yerine bu rozet ve düşük belirginlikte bowling figürlü mor arka plan kullanılır. Tüm standart popup kartları krem-altın, başlık ve açıklamalar koyu mürekkep rengindedir. Butonlar, metinler ve etkileşimler native UI olarak kalır.

Ayarlar (menü/oyun), top mağazası, swap, dört yardımcı, başarı/başarısızlık, yeniden başlatma, özellik açılışı/ilerlemesi, mekanik dersleri, nasıl oynanır, sandık ve ödüllü video pencereleri aynı ortak görselleri kullanır. Harita bölge sayfası ve oyun tahtası kendi sahne tasarımlarını kullanır.

Rozet 290×106 referans birim boyutundadır, panelin 98 birim üzerinde başlar ve yalnız altın kemer panel kenarına 8 birim oturur. Kapatma simgesiyle örtüşmez ve raycast almaz. Safe-area kesişimi, ölçek, dokunma hedefleri ve mevcut × kapatma davranışı korunur. Kartlar dokuz parçalı sprite ile çizilir; uzun panellerde çerçeve esnemez.

## Doğrulama

Unity 6000.3.24f1 macOS Development derlemesi başarılı. Safe-area/purchase çalıştırmasında 9563, geniş popup akışında 6868 kontrol geçti; hata yok, test tercihleri geri yüklendi. Türkçe ve İngilizce, küçük telefon, iPhone 13 gerçek piksel ölçüsü, çentik ve yatay kenar boşlukları sınandı. Satın alma, kullanım sonrası kapatma, sandık, ders, yardım animasyonu, video tamamlama/iptal ve bölüm geçişi kontrolleri geçti. Yeni rozetin kapanmayı engellememesi, her popup’ın yeni arka plan/krem panel kullanması ve eski figürlerin çizilmemesi ayrıca kontrol edilir.

HTML galerisindeki altı ekran tarayıcıda kontrol edildi: yeni rozet yüklendi, eski çadır/dönme dolabı yok, ortak arka plan ve krem panel aktif. Design/CarnivalUI/index.html günceldir.

Görseller image_gen ile referans tasarımdan üretildi. Kaynak PNG’ler Assets/Resources/CarnivalUI ve HTML assets klasörüne kaydedildi. asset-provenance.json üretim yöntemi ve prompt özetlerini, source-sha256.json derlenen kaynak eşleşmelerini içerir.

## Önizlemeler

[Ayarlar](settings.png) · [Mağaza](shop.png) · [Swap](swap.png) · [Yardımcı](helper.png) · [Başarı](success.png) · [Başarısızlık](failure.png) · [Yeniden başlatma](restart.png) · [Özellik](feature.png) · [Nasıl oynanır](help.png) · [Video](video.png)

Bu turda iPhone erişilemez durumdaydı; fiziksel kurulum ve dokunma kontrolü yapılmadı.

## Hazır iOS sürümü

iOS Release çıktısı ve Xcode derlemesi tamamlandı; imza doğrulandı. Uygulama yolu ios-build-verification.json içinde kayıtlıdır. Bu bir cihaz test sürümüdür; test reklamları korunur. Telefon erişilemediği için kurulmadı.

## Son cihaz kurulumu

Bowling temalı güncel Release sürümü iPhone 13’e mevcut uygulamanın üzerine kuruldu, açıldı ve başlatılan işlemin çalışmaya devam ettiği doğrulandı. ios-install.json ve ios-launch.json başarılı cihaz sonuçlarını içerir. Fiziksel popup görüntüsü bu kurulum sırasında otomatik kontrol edilmedi.
