# Dil seçimi polish çalışması

Dil satırı tek, tam genişlikte krem yüzey olarak yenilendi. Sol tarafta vektör dünya ikonu ve başlık, sağda büyük yerel dil adı ile açılır ok bulunur. Alan diğer ayarlarla aynı hizadadır; 56 birimlik dokunma yüksekliği kullanır.

Liste satırın genişliğinde açılır. Koyu zemin, krem çerçeve, seçili satırda mavi vurgu ve altın renkli onay rozeti kullanılır. Ok açılış/kapanışta kendi merkezinde döner; hareketi azalt ayarında animasyon atlanır. Seçili satırın vurgusu ve rozeti birlikte görünür/gizlenir.

Türkçe ve İngilizce tercihi, mevcut dili tekrar seçme, dışarı dokunarak listeyi kapatma, ileriye dönük kaydırma, oyun duraklatma ve tercih kalıcılığı korunur. Yeni Tamam düğmesi eklenmedi.

## Doğrulama

- Çalışan Unity uygulamasında 320×568, 390×844 ve 1170×2532 boyutlarında iki dilin kapalı/açık görünümleri üretildi ve incelendi.
- Satırların tamamen görünmesi, ok merkezi, metin sığması, liste genişliği, gerçek pointer raycast, dil değişimi ve oyuncu durumunun korunması doğrulandı.
- 284 arayüz kontrolü ve 3 ayrı işlemle kalıcılık kontrolü geçti; sıfır hata.
- Lokalizasyon kataloğu kontrolü geçti: 281 kayıt.

[Görünüm](settings-home-tr.png) · [Açık liste](language-menu-tr-320.png) · [Kontrol raporu](verification.txt) · [Kalıcılık](persistence-verification.txt)

iOS Release derlemesi ve imzası doğrulandı. Sürüm mevcut uygulamanın üzerine iPhone 13’e yüklendi; açılışı ve yeni kurulumun çalışan işlemi doğrulandı. Görsel inceleme yukarıdaki Unity uygulaması çıktılarında yapıldı.
