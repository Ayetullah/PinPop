# Merkezde tahta ve canlı bölüm geçişi

24 Kasım 2026 — iPhone 13.

Tahta ekranın yatay merkezine yerleştirildi. Yardımcılar 24×24 çizimle, arka plan plakası olmadan gösteriliyor; görünmez 54×54 dokunma alanları korunuyor. Adet metni 999 değerine kadar sığıyor. Alt top sırası ve SWAP alanı ortalandı. SWAP görseli 80×40 boyutuna küçültüldü; hak varken adet, hak bitince aynı düğmenin üzerinde altın renkli + gösteriliyor. + düğmeye dahil ve hak mağazasını açıyor; hamle harcamıyor.

Bölüm geçişi daha canlı turkuaz, pembe, mor ve altın renklerle yeniden çizildi. Büyük bölüm başlığı, oyunun karakterli dubaları, yuvarlanan top, devrilme ve yıldız animasyonu kullanılıyor. 1,4 saniye süre, giriş kilidi ve azaltılmış hareket desteği korunuyor. Güç yüzdesi ve MAX yazısı eklenmedi. Canlı reklam ayarları değiştirilmedi.

## Kontroller

- Splash 17/17, geçiş 31/31, arayüz 241/241: toplam 289 başarılı kontrol.
- İki gerçek iPhone çalışması: 22 aşama, her ikisinde sıfır kayıtlı hata ve bakiye koruması.
- İkinci çalışma: 1.609 kare; splash görünür animasyonu p95 17,69 ms, en uzun 20,70 ms. Bölüm geçişi p95 16,83 ms, en uzun 34,91 ms. Nişan en uzun 16,90 ms.
- İlk kurulumdan sonraki çalışmada geçişte 214,51 ms ve nişanda 133,44 ms tekil gecikmeler görüldü; tekrarda görülmedi. İlk hazırlık sabit splash altında 3,79 saniye, tekrarda 0,59 saniye sürdü. Bu ölçümler development tanılamasından gelir; uzun süreli performans testi değildir. PNG kayıtlarının ek yükü ölçümden çıkarılır.
- Release derlemesi ve imza kontrolü başarılı. Tanılama tipleri ve debug bayrakları pakette bulunmuyor; native açılış storyboard’u derlenmiş, Google test reklam kimliği korunmuş.

[İlk cihaz raporu](iPhone13-first-run/frame-health.json) · [Tekrar cihaz raporu](iPhone13-repeat/frame-health.json) · [Release kontrolü](release-verification.json) · [Oyun ekranı](iPhone13-repeat/game.png) · [Bölüm geçişi](iPhone13-repeat/level-transition.png)

App Store yüklemesi bu işlemde yapılmadı. Yerel Xcode 16.4/iOS 18.5 SDK ve cihaz geliştirme profili kullanılıyor; mağaza gönderimi için uygun güncel SDK ve dağıtım imzası ayrıca gerekli. Canlı reklam entegrasyonu kullanıcı isteğiyle kapsam dışında.

Güncel Release paketi iPhone’a uygulama kaldırılmadan kuruldu, başarıyla açıldı ve yeni kurulum yolundaki çalışan işlem doğrulandı.
