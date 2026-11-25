# Açılır dil listesi

Ayarlarda Dil / Language etiketiyle aynı satırda açılır liste bulunur. Kapalı alanda yalnızca seçili dilin kendi adı, listede Türkçe ve English görünür; Cihaz dili seçeneği gösterilmez. İlk açılıştaki otomatik cihaz dili davranışı korunur. Kullanıcı varsayılan olarak görünen dili yeniden seçse bile bu seçim açık bir tercih olarak kaydedilir.

Liste tanımları Assets/Resources/Localization/languages.json içindedir. Menü seçenekleri bu tanımlardan üretilir; yeni dil ve çeviri desteği eklendiğinde yeni bir ayarlar düğmesi yazmak gerekmez. Liste en fazla dört satırlık bir alan kullanır ve fazlası kaydırılır. iOS dil bildirimi de aynı tanımları kullanır. İngilizce varsayılan / geri dönüş dili olmaya devam eder.

Seçili dil işaretlidir. Dışarı dokunma listeyi kapatır; ayarları kapatmaz. Seçim oyunu yeniden başlatmadan uygulanır ve sonraki açılışta korunur. Dil değiştirme sırasında bölüm, top sırası, hamleler, bakiye ve sandık ödülleri korunur.

## Doğrulama

- Açılır liste, kalıcılık ve sonuç yerleşimi incelemesi: 233 kontrol.
- Ayrı uygulama işlemiyle kayıtlı dil kontrolü: 3 kontrol.
- İngilizce genel arayüz: 347 kontrol.
- Türkçe genel arayüz: 314 kontrol.
- Toplam 897 kontrol, sıfır hata.

Gerçek pointer raycast’iyle açma ve seçim; başlangıçta gösterilen dili seçerek kaydetme; seçili dili tekrar seçme; dışarı dokunarak kapatma; daha fazla seçenek eklenmiş test listesinde son satıra kaydırma; popup ve giriş engelleyicisinin temizlenmesi kontrol edildi. Test verileri geri yüklenir.

[Açık liste](settings/language-list.png) · [Türkçe ayarlar](settings/settings-home-tr.png) · [İngilizce ayarlar](settings/settings-home-en.png) · [İnceleme raporu](settings/verification.txt) · [Kalıcılık raporu](settings/persistence-verification.txt)

Release derlemesi ve imzası, açılır liste bileşeninin dahil edilmesi, inceleme kodlarının çıkarılması doğrulandı. Canlı reklam ayarları değiştirilmedi. Cihaz incelemesi ayrıca kaydedilir.

Gerçek iPhone 13 incelemesi 27 aşama, 1.783 kare ile tamamlandı; kayıtlı hata yok. Açılır liste UI raycast’iyle açıldı ve iki dil seçildi. Bakiye ve inceleme öncesindeki dil tercihi korundu. [Cihaz raporu](iPhone13/frame-health.json) · [iPhone açık liste](iPhone13/settings-language-list.png).

Son Release uygulama kaldırılmadan iPhone’a kuruldu, açıldı ve yeni kurulum yolundaki çalışan süreç doğrulandı.
