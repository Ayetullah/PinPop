# iPhone popup üst taşması

Sebep: Screen.safeArea alt-sol koordinat sistemindeyken popup viewport’u üst-sol koordinatlarla yerleştiriliyordu. Üst çentik payı pozitif Y ile ekleniyor, bütün 390×780 sahne aşağı yerine yukarı kayıyordu. Bölüm/coin etiketleri ekran üstünde kesiliyor; mağazanın çadır tepesi kenara taşıyor, alt tarafta fazla boşluk kalıyordu.

Düzeltme: güvenli alan ile oyun viewport’unun hem yatay hem dikey kesişimi alınır. Üst boşluk negatif yerel Y ile uygulanır; bütün popup sahnesi bu dikdörtgen içinde oranı korunarak ortalanır. Panel/çadır/buton boyutları değiştirilmedi.

Önceki kontroller buton sınırlarını ölçüyor, sahnenin tamamını ve üstteki etkileşimsiz başlıkları ölçmüyordu. Ortak Inspect kontrolüne sahnenin, bölüm/coin etiketlerinin, coin simgesinin, çadırın, markanın, panelin ve kapatma hedefinin görünür sınırları eklendi.

## Doğrulama

Unity 6000.3.24f1 macOS Development derlemesi başarılı. Native Unity çalıştırmasında 8525 kontrol geçti, 0 hata; geçici test tercihleri geri yüklendi. Türkçe/İngilizce; 320×568 çentiksiz, 390×844 iPhone 13 mantıksal güvenli alanı, 1170×2532 iPhone 13 fiziksel güvenli alanı, 393×852 üst boşluğu yüksek çentik ve yatay kenar boşluklu beş geometri sınandı. Mağaza, oyun/menü ayarları, swap, dört yardımcı, başarısızlık/başarı, yeniden başlatma, özellik ilerlemesi/açılışı ve açılış animasyonu kontrol edildi. Dil listesi, satın alma ve yardımcı kullanım kontrolleri de tekrar geçti.

Önce/sonra native mağaza görüntüleri ve diğer temsilî görüntüler buradadır. verification.json tüm kontrol sonuçlarını, source-sha256.json derlenen kaynak eşleşmesini içerir. Fiziksel iPhone kurulumu ayrı kurulum kaydına yazılır.

## iPhone kurulumu

Düzeltme güncel Release iOS çıktısına alındı; Xcode derlemesi ve imza doğrulaması başarılı. Mevcut uygulama kaldırılmadan iPhone 13’e yüklendi, başlatıldı ve işlem cihazdan sorgulanarak çalıştığı doğrulandı. Fiziksel cihazdaki popup görüntüsü otomatik yakalanmadı; yukarıdaki görüntüler gerçek Unity çalıştırmasında iPhone güvenli alanıyla üretilmiştir. ios-install-verification.json, ios-install.json ve ios-launch.json kurulum kanıtlarını içerir.
