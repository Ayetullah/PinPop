# Dil seçici polish

Güncel tüm-popup çalışması ve ayarlar: [AllPopups](../AllPopups/README.md). Hareket azaltma oyun ayarı kaldırılmıştır.

Unity ayarlarındaki dil alanı güncellendi. Tüm satır dokunulabilir; globe ikonu altın kenarlı bir rozet içinde, dil adı daha büyük ve kontrastlı bir krem alanda. Ok merkezinden döner. Tam genişlikteki açılır listede seçili dil renkli satır ve onay işaretiyle görünür. Liste kısa bir açılma geçişi kullanır; azaltılmış harekette sabittir. Dışarı dokunma kapatmaz; seçimden vazgeçmek için dil listesindeki × kullanılır. Aynı dili seçmek de kalıcı dil tercihini kaydeder.

[Kapalı görünüm](settings-tr-390.png) · [Türkçe açık liste](language-list-tr-390.png) · [English açık liste](language-list-en-390.png) · [Küçük ekran](language-list-en-320.png)

macOS Unity derlemesi başarılı. 320×568, 390×844 ve iPhone 13 güvenli alanını temsil eden 1170×2532 boyutlarında Türkçe/English kontrol edildi. Gerçek EventSystem raycast ile satırın soluna dokunma, seçeneklerin dokunma boyutu, liste güvenli alanı, seçili onay işareti, dışarı dokunarak kapatma, mevcut dili yeniden seçme ve diğer dile geçme doğrulandı. Altı ekranın mevcut ödül ve geçiş kontrolleri de tekrar geçti.

Sonuç: **1.223 geçti, 0 hata**. Denetim sonunda oyuncunun önceki kayıtları ve tercihleri geri yüklendi. Bu macOS Unity doğrulamasıdır; fiziksel iPhone testi yapılmadı.

[Kontrol raporu](verification.json) · [Kaynak eşleşmesi](source-sha256.json)
