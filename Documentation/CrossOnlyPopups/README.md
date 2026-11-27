# Popup kapatma politikası

Uygulama içi popup'ların vazgeçme/kapatma eylemi yalnız × kontrolüdür.

- `Oyuna devam et`, `Hadi oynayalım` ve HTML öğreticisindeki `Anladım` gibi yalnız kapatan düğmeler kaldırıldı.
- Dışarı dokunma ve Escape/geri yolu popup'ı kapatmaz. Dil listesinin de ayrı × kontrolü vardır; dışarı dokunma ve iptal olayı yalnız girdiyi engeller.
- Öğretici son sahnede döngüye devam eder. Son sahnede kaydırmak veya süreyi doldurmak kapatmaz.
- Swap ve yardımcı satın alma sonrası pencere açık kalır. Başarılı yardımcı kullanımı ise pencereyi kapatır ve oyunu sürdürür. [Güncel yardımcı akışı](../HelperUseClose/README.md).
- Video tamamlanma ve hata bilgileri × basılana kadar görünür. Kazanılmış video ödülü × ile çıkarken bir kez uygulanır; yarım kalan video ödül vermez.
- Sonraki bölüm, yeniden başlatma ve haritadan bölüm seçimi oyun akışını değiştiren işlevlerdir; korunmuştur. Dil seçimi ayarı uygular, ana ayarlar penceresini kapatmaz.

[Çarpılı dil listesi](language-list-tr-390.png) · [Yeniden başlatma](restart-tr-390.png) · [Özellik açılışı](unlock-tr-390.png) · [Öğretici](howto-2-tr-390.png) · [Video tamamlandı](video-completed-cross-only.png)

Sonuç: **4.432 geçti, 0 hata.** Türkçe/English, 320×568, 390×844 ve 1170×2532 güvenli alan düzenleri Unity macOS oynatıcısında kontrol edildi. EventSystem raycast ve cancel olaylarıyla kapatma yolları; yardımcı/swap işlemleri; öğreticinin son sayfası ve video ödülünün × beklemesi doğrulandı. Video tamamlanma kontrolü sağlayıcı callback'ini simüle eder; canlı reklam testi değildir. Önceki oyuncu kayıtları geri yüklendi.

[Kontrol raporu](verification.json) · [Derleme kaydı](build.log) · [Kaynak eşleşmesi](source-sha256.json)
