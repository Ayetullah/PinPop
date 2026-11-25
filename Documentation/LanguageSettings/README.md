# Sonuç ekranı ve kalıcı dil seçimi

Kullanıcı görselindeki çakışmanın kaynağı, AdventureHUD üzerindeki ayrı Milestone reward summary katmanıydı. Bu katman kaldırıldı. Sandık ödülü artık sonuç kartının içinde ikonlu, bağımsız bir satırda gösterilir. Sandık varsa kart ve düğme konumları genişler; normal sonuç kartı önceki boyutunu korur. Ödül efektlerinin korunan alanı da karta uyar.

Ana menü ve oyun içi ayarlarda Cihaz dili / Türkçe / English seçenekleri bulunur. Seçilen seçenek yeşil gösterilir. Değişiklik aynı açık ayarlar ekranına uygulanır; bölüm, hamleler, top sırası ve bakiyeler korunur. Seçim PinPop.Language anahtarına kaydedilir. Cihaz dili seçeneğinde Türkçe cihaz Türkçe, diğer diller İngilizce kullanır. Menü arabasındaki BÖLÜM/LEVEL tabelası da anında yenilenir. Reklam gizliliği düğmesi yoksa ayarlar kartında boş alan ayrılmaz.

## Yerel doğrulama

- Yeni davranış ve çakışma incelemesi: 202 başarılı kontrol.
- Gerçek ikinci uygulama işlemiyle dil kalıcılığı: 3 başarılı kontrol. İngilizce cihaz benzetiminde kayıtlı Türkçe seçimi korunur.
- İngilizce genel arayüz: 347 başarılı kontrol.
- Türkçe genel arayüz: 314 başarılı kontrol.
- Toplam 866 kontrol, sıfır hata.

Yeni inceleme gerçek bölüm tamamlamasını çalıştırır; 5, 10, 15 ve 20. bölüm sandıklarını ödüllendirir. Her sandık iki dilde, 320×568 / 390×844 / 1170×2532 boyutlarında kontrol edilir. Ödül satırının düğme yazısıyla ve dokunma alanıyla çakışmadığı, metnin sığdığı, kartın güvenli alanda kaldığı doğrulanır. Dil değiştirme ve yeniden boyutlandırmanın ikinci kez ödül vermediği kontrol edilir. İnceleme oyuncunun kayıtlarını yedekleyip geri yükler.

[Düzeltilmiş sonuç](settings/milestone-5-tr-390.png) · [İngilizce sonuç](settings/milestone-5-en-390.png) · [Dil seçimi](settings/settings-home-tr.png) · [İnceleme raporu](settings/verification.txt) · [Yeniden açılış raporu](settings/persistence-verification.txt)

Release derlemesi ve imzası doğrulandı; dil tercihi kodu dahil, inceleme bileşenleri ve test dili zorlaması hariç. Canlı reklam kimlikleri ve ödül kuralları değiştirilmedi. Gerçek iPhone incelemesi ayrıca kaydedilir.

Gerçek iPhone 13 incelemesi 28 aşama ve 1.787 kare ile tamamlandı; kayıtlı hata yok. İngilizce/Türkçe/Cihaz dili seçeneklerine UI raycast’iyle basıldı. Dil tercihi ve bakiyeler inceleme öncesindeki değerlerine geri döndürüldü. Sandıklı sonuç cihazda ödül verilmeden önizlendi; gerçek ödül davranışı yerel tamamlanma testinde doğrulandı. [Cihaz raporu](iPhone13/frame-health.json) · [iPhone ayarlar](iPhone13/settings-tr.png) · [iPhone sonuç](iPhone13/milestone-result.png).

Güncel Release uygulama kaldırılmadan iPhone’a kuruldu, açıldı ve yeni kurulum yolundaki çalışan süreç doğrulandı.
