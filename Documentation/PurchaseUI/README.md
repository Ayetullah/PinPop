> Çentikli ekrandaki üst taşma daha sonra düzeltildi. Güncel güvenli alan testi ve görüntüler: [PopupSafeArea](../PopupSafeArea/README.md).

# Ortak satın alma arayüzü

Swap ve dört yardımcı penceresi 322 × 420 referans birimlik aynı krem paneli, başlık/çadır yerleşimini ve panelin altında 272 × 61 boyutlu aynı yeşil satın alma butonunu kullanır. Butonun etiketi yerelleştirilmiş SATIN AL · {fiyat} metnidir; fiyatın ardından canlı coin simgesi gelir. +1 AL etiketi kaldırıldı.

Top mağazasında aynı satın alma bileşeninin 118 × 44 görsel boyutlu sürümü kullanılır; dokunma hedefi 61 birim yüksekliğinde korunur. Sahip olunan toplarda krem SEÇ / SEÇİLİ görünümü kullanılır; bu durumda fiyat ve coin simgesi gizlenir. KULLAN ve renk/duba seçimleri krem seçim stiliyle satın almadan ayrılır.

Ortak giriş noktaları CarnivalPopup.Purchase, PurchaseAction ve PurchaseButton; ortak sunum bileşeni CarnivalPurchaseButton. SetOffer fiyatı veya stok sınırı durumunu, SetSelection sahip olunan ürün durumunu gösterir. Yerelleştirme değiştiğinde etiket ve coin konumu yeniden hesaplanır. Basılma tepkisi ve devre dışı görünüm mevcut ortak geri bildirim bileşenleriyle sağlanır.

Fiyatlar, coin/yardımcı kayıtları ve oyun kuralları değiştirilmedi. Satın alma penceresi açık kalır; başarılı yardımcı kullanımı pencereyi kapatıp oyunu sürdürür. Genel kapatma yalnız × ile yapılır.

## Doğrulama

Unity 6000.3.24f1 macOS Development derlemesi başarılı. Native Unity çalıştırmasında Türkçe/İngilizce, 320 × 568, 390 × 844 ve iPhone 13 oranı/güvenli alanıyla 1170 × 2532 test edildi. 2309 kontrol başarılı, 0 başarısız; test tercihleri geri yüklendi.

Kontroller: altı ekranın ortak görseli, buton/yazı/simge sınırları, güvenli dokunma alanları, sekiz renk ve dokuz dubalı cep ızgarası; yeterli/yetersiz ve tam fiyat kadar bakiye; 998 → 999 stok geçişi, dolu stok engeli, coin ve envanterin bir kez güncellenmesi; top satın alma/seçme; pencerenin satın alma sonrası açık kalması, başarılı yardımcı kullanımında kapanması, başarısız kullanımda açık kalması; yalnız × ile genel kapatma; duraklatma sırasında basılma ve iptal geri bildirimi.

Bu turda fiziksel iPhone testi veya cihaza yükleme yapılmadı. Native ekran görüntüleri bu klasörde, sonuçlar verification.json, derleme kaydı build.log ve derlenen kaynak eşleşmeleri source-sha256.json içindedir.

Örnekler: [Swap](swap-tr-390.png), [Renk seçimi](helper-0-tr-390.png), [Top mağazası](catalog-tr-320.png), [Dolu cep](pocket-full-grid-tr-320.png).
