# Tüm oyun içi popuplarda sağ üst kapatma çarpısı

Kapatılabilir oyun pencereleri tarandı. Yalnızca kapatma yapan alt düğmeler kaldırıldı; her modal kendi sağ üst çarpısıyla kapanıyor. Kapatma ikonları vektör geometrisi kullanır ve en az 44 mantıksal piksel dokunma alanıyla test edildi.

## İncelenen pencereler

| Pencere / durum | Kapatma | Korunan işlemler ve dönüş |
| --- | --- | --- |
| Ana menü ayarları | Sağ üst çarpı | Ses, hareket, dil, nasıl oynanır; ana menüye dönüş |
| Oyun ayarları | Sağ üst çarpı | Ayarlar, tekrar, ana menü; çarpı oyuna devam eder |
| Ana menü ve oyun içi top mağazası | Sağ üst çarpı | Satın alma / kuşanma; eski Kapat düğmesi yok |
| SWAP satın alma | Sağ üst çarpı | Satın Al; eski Vazgeç düğmesi yok, çarpı para/hamle harcamaz |
| Dört yardımcı penceresi | Sağ üst çarpı | Satın alma, kullanma, renk ve cep figürü seçimi |
| Yeni karakter ve dört mekanik öğreticisi | Sağ üst çarpı | Bekleyen öğreticilere geçiş; ekstra Anladım düğmesi yok |
| Animasyonlu nasıl oynanır | Sağ üst çarpı | Adım geçişleri; ayarlardan açıldıysa ayarlara dönüş |
| Her iki bölge haritası | Sağ üst vektör çarpı | Bölüm seçimi, bölge geçişi ve sandıklar |
| Zafer, tekrar oynama, sandık ödülü, son bölüm sonucu | Sağ üst çarpı | Sonraki Bölüm / Haritaya Dön; eski alt Ana Menü kısayolu çarpıya taşındı |
| Hamle bitmesi, video kullanılmış durum, tepsi dolması, video hatası | Sağ üst çarpı | Tekrar Dene ve uygun durumdaki Video İzle; eski alt Ana Menü kısayolu yok |
| Oyunun kendi video hazırlama / yerel video ekranı | Sağ üst çarpı | Eski alt Vazgeç düğmesi yok; iptal ödül kazandırmaz |

Sonuç ekranlarının çarpısı ana menüye döndürür; kapanan sonuçtan sonra oynanamaz bir bitmiş masa bırakılmaz. Kazanılmış ödül kapanışta tekrar verilmez. Sonuçtan kapatma, video isteği sürerken devre dışıdır. SDK reklamı gösterilirken uygulamanın iptal kontrolünü kilitleyen mevcut kural korunur.

Mağaza, SWAP ve sonuç kartları alt düğmenin kaldırılmasına göre kısaltılıp ortalandı. Başlıklar çarpı için yer bırakır. Harita çarpısı fonttaki × karakteri yerine vektör ikon kullanır; dokunma alanı genişletildi.

Satın Al, Kullan, Sonraki Bölüm, Tekrar Dene, Video İzle gibi işlem düğmeleri korunur. Oyun ayarlarındaki Ana Menü de ayrı bir gezinme işlemidir; ayar çarpısından farklı olarak oyundan çıkar.

Dil açılır listesi seçim menüsüdür; dışarı dokunma/seçim davranışı korunur. Splash ve bölüm geçişi kapatılabilir popup değildir. AdMob’un kendi reklam/onay/mahremiyet formları SDK tarafından çizilir; bu formların kontrolleri değiştirilmez.

## Doğrulama

- Türkçe tüm popup durumları: **457/457**.
- İngilizce tüm popup durumları: **457/457**.
- Genel Türkçe arayüz: **337/337**.
- Genel İngilizce arayüz: **372/372**.
- Gerçek yerel video ile devam/iptal: **39/39**.
- Önceki altı hata düzeltmesi: **41/41**.
- Kampanya, harita ve ödüller: **148/148**.
- **Toplam 1851 başarılı kontrol, sıfır başarısızlık.**

Popup durumları 320×568, 390×844 ve 1170×2532 boyutlarında çalıştırıldı. Kontroller yalnızca fonksiyon çağırmıyor: her çarpının merkezine gerçek EventSystem raycast’i yapılıyor. Tek kapatma çarpısı, sağ üst konum, dokunma boyutu, başlıkların sığması ve görünür kapatma metni bulunmaması kontrol ediliyor. Mağaza, SWAP, yardımcılar, harita ve öğretici kapanışlarında hamle, sıra, deneme kimliği, coin ve envanterin korunması doğrulanıyor.

Video hazırlama testi, cihazda SDK başlatılmadan uygulamanın kendi penceresini iptal eder. Ayrı yerel video testi, oynatma/iptal/tamamlanma ve tek ödül kuralını sınar.

[Koşular](runs.json) · [Özet](verification-summary.json) · [Kaynak hash’leri](source-manifest.json)

## Görseller

[Mağaza](popups-tr/shop-game-320.png) · [SWAP](popups-tr/swap-320.png) · [Zafer](popups-tr/victory-4-320.png) · [Tepsi dolması](popups-tr/failure-overflow-320.png) · [Video hazırlama](popups-tr/video-preparation.png) · [İlk bölge](popups-tr/map-0-1170.png) · [İkinci bölge](popups-tr/map-20-1170.png)

## iOS paketi

İnceleme ve normal Release iOS derlemeleri başarılı. Normal paketin imzası doğrulandı; popup test sınıflarının Release IL2CPP çıktısında bulunmadığı kontrol edildi.

İlk denemede telefon bağlantısı yoktu. Yeniden bağlandıktan sonra gerçek iPhone 13 üzerinde **457/457 popup kontrolü** geçti; test verileri ve oyuncu tercihleri geri yüklendi. Mağaza, SWAP, yardımcılar, her iki harita, öğreticiler, sonuçlar ve video hazırlama pencereleri üç boyutta test edildi. Son normal Release sürümü mevcut uygulamanın üzerine yüklendi, açılışı ve yeni kurulumun çalışan işlemi doğrulandı.

[Gerçek cihaz raporu](iPhone13/verification.json) · [iPhone mağaza ekranı](iPhone13/shop-game-1170.png) · [iPhone video hazırlama ekranı](iPhone13/video-preparation.png)
