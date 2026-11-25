# Splash ve yayın öncesi ekran düzenlemesi

24 Kasım 2026 — PinPop, iPhone 13.

## Sonuç

Native iOS açılış ekranı ile Unity splash aynı başlığı, karakter yerleşimini, arka planı ve gölge katmanını kullanır. Motor hazırlığı sırasında karakterler sabit kalır ve gerçek yükleme ilerlemesi gösterilir. Kaynaklar hazır olduğunda kısa animasyon başlar; önce splash yazıları/karakterleri, sonra örtü kaybolur. Böylece ana menünün logosuyla üst üste binmez. Ana menüye dönüşte splash yeniden oynatılmaz.

Sahne geometrisi, top kaplamaları/ikonları, duba geometrisi ve öğretici görselleri artık Editor'da hazırlanır. Telefonda 7 MB sahne JSON'u işlenmez veya top görselleri piksel piksel üretilmez. Kaynaklar düşük yükleme önceliğiyle birlikte hazırlanır; UI kurulum adımları ve bölüm dubaları karelere bölünür. Kullanılmayan eski HTML kontrolleri kurulup gizlenmez.

Öğretici kartlarının kenarları vektör olarak çizilir; mağaza ve sonuç metinleri büyütülür. Harita navigasyonu ve coin göstergesi daha açık görseller kullanır. UI ikonlarının yayları daha az geometriyle çizilir; tekrarlanan geçici koleksiyonlar kaldırılır. Oyun sırasında gereksiz bulanıklık örneklemesi ve derinlik geçişi çalışmaz. iOS gölge ayarları masaüstüyle tutarlı hâle getirilmiştir. Runtime kamera FOV ayarı korunur.

Canlı reklam kimliği veya ödül kuralları değiştirilmemiştir. Reklam SDK'sının otomatik başlangıç yükü splash/menü geçişinden çıkarılmıştır; video kullanıcı tarafından istendiğinde hazırlanır. Reklam gösterimi bu görsel doğrulamanın kapsamı değildir.

## Doğrulama

- Splash: **17/17** — görünürlük, raycast engeli, logo çakışması, geçiş ve kayıt koruma.
- Bölüm geçişleri: **31/31** — CanvasRenderer bütünlüğü, normal/azaltılmış hareket, yeniden boyutlandırma ve giriş kilidi.
- Arayüz: **163/163** — altı ekran boyutu, güvenli alanlar, yardımcılar, fontlar, yıldızlar ve animasyonlu öğreticiler.
- Gerçek iPhone: **22 aşama, 1.616 kare**, kaydedilen uygulama hatası yok; coin ve SWAP bakiyesi korunmuş.

Gerçek cihaz tanılama çalışmasında Unity hazırlığı 0,61 saniyede, Unity splash'ten menüye geçiş 1,40 saniyede tamamlandı. Animasyonlu splash geçişinde p95 17,37 ms, en uzun kare 17,56 ms ölçüldü. Menü ve diğer ekranlarda çoğu kare yaklaşık 16,8 ms (60 FPS); bölüm geçişinin en uzun karesi 34,33 ms oldu. Ham sonuçlar [frame-health.json](iPhone13/frame-health.json) içindedir.

İlk motor/ekran hazırlığı **sabit görsel aşamasıdır** ve kısa yükleme sıçramaları içerir; bunlar animasyonlu geçiş başlamadan tamamlanır. Bu süre ölçümü işletim sisteminin uygulama başlatma süresini kapsamaz. Kare süreleri development tanılama paketinde ölçülmüştür; ekran PNG kaydı sırasında oluşan ek yük izleyen üç kareyle birlikte ölçümden çıkarılır. Release paketi ayrıca derlenir ve telefona kurulur; tanılama komutları ve kayıt bileşenleri bu pakette bulunmaz. Bu kısa çalışma uzun süreli pil/ısınma testi değildir.

## Görsel kayıtlar

[iPhone ana menü](iPhone13/home.png) · [Oyun](iPhone13/game.png) · [Harita](iPhone13/map.png) · [Mağaza](iPhone13/shop.png) · [Öğretici](iPhone13/how-to.png) · [Sonuç](iPhone13/result.png) · [Kaybetme](iPhone13/failure.png) · [Splash](splash/splash-unity-opening.png)

Sonuç ve kaybetme ekranları tanılama içinde ödül verilmeden önizlenmiştir. Oyuncunun ilerlemesi ve bakiyesi değiştirilmemiştir.

## Tekrar hazırlama

Unity menüsü: **PinPop → Prepare production assets**. Aynı hazırlık build öncesinde otomatik çalışır. Üretilen kaynaklar `Assets/Resources/Prepared` ve `Assets/PreparedNative` altındadır.

Release export: **PinPop → iOS → Export release candidate (test ads)**.

```sh
python3 Tools/ios_device_test.py build --release --generic
python3 Tools/ios_device_test.py install --release
python3 Tools/ios_device_test.py launch --release
```

## Mağaza yüklemesi

Bu çalışma arayüzü ve cihazda çalışmayı doğrular. Yerel makine hâlâ Xcode 16.4 / iOS 18.5 SDK kullanıyor. Mevcut imza cihaz testi içindir. App Store gönderimi için Xcode/iOS SDK güncellemesi ve dağıtım imzası gereklidir; `Tools/ios_device_test.py store-check --release` bu koşulları ayrıca kontrol eder. Sistem yükseltmesi veya mağaza gönderimi yapılmamıştır. [Apple SDK gereksinimi](https://developer.apple.com/news/upcoming-requirements/?id=04282026a).
