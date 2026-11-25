# Öğretici ve yardımcı kontrolleri

24 Kasım 2026.

Animasyonlu nasıl oynanır ekranına sağ üstte vektör çarpı eklendi. Görselin çevresindeki geniş dokunma alanı sabit kalır; çarpı üç anlatım adımından da ekranı kapatır ve oyunu devam ettirir. Hamle, sıra ve bakiyeyi değiştirmez. Masaüstünde Escape de kapatır. Öğretici kartı güvenli alana göre ortalanır; başlık ve üst metin çarpıya yer açacak şekilde düzenlendi. Kaydırma ve otomatik anlatım korunur.

Yardımcıların her biri küçük, yarı saydam krem bir yüzey ve yumuşak kenar üzerinde gösterilir. İkonlar 24 birim, dokunma alanları 54 birimdir. Adetler ikon altında ortalanır; 999 değeri sığar. Dokunulduğunda ikon, adet ve yüzey birlikte tepki verir; raycast alanı yer değiştirmez. Tahta yatay merkezini korur; SWAP düzeni ve reklam ayarları değişmez.

## Yerel doğrulama

- Arayüz: 265/265 başarılı kontrol; altı ekran boyutu, saydam yüzeyler, güvenli alanlar, merkezde tahta, SWAP ve yazı netliği.
- Öğretici: 53/53 başarılı kontrol; gerçek UI raycast’iyle üç adımda çarpıya basma, oyunun devam etmesi, kayıt ve sıra koruma, kaydırma, otomatik ilerleme, küçük/dikey/yatay ekranlarda yerleşim ve dokunma alanı.

[Arayüz raporu](ui/verification.txt) · [Öğretici raporu](tutorial/animated-help-unity-verification.txt) · [Oyun](ui/game-1170x2532.png) · [Öğretici](tutorial/animated-help-unity-pull.png)

Release derlemesi, imza ve tanılama kodlarının çıkarılması doğrulandı. Gerçek iPhone 13 incelemesi 24 aşama / 1.669 kare ile tamamlandı. Kayıtlı hata yok; bakiyeler korundu. Üç öğretici adımında da gerçek UI raycast’i üzerinden çarpıya basıldı; güvenli alan ve oyuna dönüş kontrol edildi. Öğretici aşamalarında en uzun kare 16,93 ms; bölüm geçişinde 34,89 ms. Bu kısa development ölçümü uzun süreli performans testi değildir; görüntü kaydı yükü ölçümden çıkarılır.

[Gerçek cihaz raporu](iPhone13/frame-health.json) · [iPhone öğretici](iPhone13/how-to.png) · [iPhone oyun](iPhone13/game.png)

Güncel Release paketi iPhone’a uygulama kaldırılmadan kuruldu, başarıyla açıldı ve yeni kurulum yolundaki çalışan süreç doğrulandı. Canlı reklam ayarları değiştirilmedi.
