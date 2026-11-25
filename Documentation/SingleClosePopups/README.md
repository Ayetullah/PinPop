# Popuplarda tek kapatma kontrolü

Çarpısı olan ayar ve öğretici popuplarındaki ikinci kapatma düğmeleri kaldırıldı. Ana menü ve oyun ayarlarındaki TAMAM/DONE artık oluşturulmuyor. Öğretici ve yeni karakter popuplarındaki ANLADIM/GOT IT kaldırıldı. Bu kartlar içeriğe göre küçültülüp ortalandı. Öğretici çarpısı geniş, sabit bir dokunma alanı ve vektör ikon kullanır.

Öğretici çarpısı eski Anladım akışını korur: sıradaki bekleyen açıklama varsa gösterilir; son açıklama kapanınca oyun devam eder. Escape de aynı akışı kullanır. Programatik Close işlemi sıradaki öğreticiyi açmaz; yardımcı/harita işlemleri korunur.

| Popup | Kapatma | Korunan işlemler |
| --- | --- | --- |
| Ana menü ayarları | Çarpı | Ses, hareket, dil |
| Oyun ayarları | Çarpı | Ses, hareket, dil, tekrar, ana menü |
| Mekanik / karakter öğreticisi | Çarpı | Animasyon, sıradaki açıklama |
| Nasıl oynanır | Çarpı | Kaydırma ve otomatik anlatım |
| Yardımcılar | Çarpı | Satın alma, kullanma, renk/figür seçimi |
| Harita | Çarpı | Bölüm seçme ve sandık |
| Mağaza / SWAP satın alma | Mevcut tek Kapat / Vazgeç | Çarpı olmadığı için mevcut kontrol korunur |
| Sonuç ekranları | Çarpı yok | Bölüme devam ve ana menü korunur |

## Doğrulama

- İngilizce arayüz: 372 başarılı kontrol.
- Türkçe arayüz: 337 başarılı kontrol.
- Dil listesi / sonuç yerleşimi: 233 başarılı kontrol.
- Ayrı işlemle dil kalıcılığı: 3 başarılı kontrol.
- Toplam 945 kontrol, sıfır hata.

Yeni kontroller iki ayar ekranında Done bulunmadığını, gerçek pointer raycast’iyle çarpıların oyunu devam ettirdiğini ve öğreticide yalnızca bir kapatma düğmesi olduğunu doğrular. Birden fazla bekleyen gerçek açıklama çarpıyla sırayla kapatılır; oyun en sonunda devam eder. Hamleler, sıra ve coin korunur; inceleme sırasında değişen öğretici kayıtları geri yüklenir.

[Ana menü ayarları](en-ui/settings.png) · [Oyun ayarları](en-ui/settings-game.png) · [Öğretici](en-ui/lesson-0.png) · [Yeni karakter](en-ui/unlocked-character.png) · [İngilizce rapor](en-ui/verification.txt) · [Türkçe rapor](tr-ui/verification.txt)

Release derlemesi ve imzası doğrulandı; inceleme bileşenleri ve test dili zorlaması pakette bulunmuyor. Canlı reklam ayarları değiştirilmedi. Gerçek iPhone incelemesi ayrıca kaydedilir.

Gerçek iPhone 13 incelemesi tamamlandı: hata listesi boş, coin ve dil tercihi korundu. Ayar ve öğretici görselleri kontrol edildi; gereksiz ikinci kapatma düğmesi yok. [Cihaz raporu](iPhone13/frame-health.json) · [Ayarlar](iPhone13/settings.png) · [Öğretici](iPhone13/lesson-1.png). Son Release sürümü mevcut kayıtlar korunarak cihaza yüklendi; açılışı ve yeni kurulumun çalışan işlemi doğrulandı.
