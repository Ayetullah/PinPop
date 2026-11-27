> Güncel tüm ekran teması: bowling rozeti, krem-altın paneller ve mor bowling arka planı. Unity ve HTML doğrulaması: [BowlingPopupTheme](../../Documentation/BowlingPopupTheme/README.md).

# PinPop · Lunapark ekranları

İki referans görselden PinPop'a uyarlanmış, yerel HTML/CSS/SVG prototipleri. Bu teslim yalnız HTML tasarımlarını kapsar. Unity ekranları, oyun kayıtları ve canlı reklam ayarları değiştirilmedi.

## Açılış

`index.html` dosyasını tarayıcıda açın. İnternet veya paket kurulumu gerektirmez; fontlar ve PinPop duba görselleri pakettedir. Galeride altı ekran birlikte görünür. **Tek ekran** bağlantıları bağımsız HTML dosyalarını açar.

Yerel sunucu alternatifi: bu klasörde `python3 -m http.server 8767 --bind 127.0.0.1` çalıştırıp `http://127.0.0.1:8767/` adresini açın.

| Dosya | Ekran |
| --- | --- |
| bolum-tamamlandi.html | Başarı; yıldızlar, +40 coin ve sonraki bölüm |
| tekrar-dene.html | Başarısızlık; tekrar dene ve video ile +3 hamle |
| yeniden-baslat.html | Bölümü yeniden başlatma onayı |
| ayarlar.html | Ses, azaltılmış hareket, dil ve nasıl oynanır |
| ozellik-acildi.html | Bölüm 7'de açılan Bağlı Üçlü |
| ozellik-ilerlemesi.html | %60 → %80 → özellik açılışı |

## Görsel uyarlama

- Referanstaki kırmızı/krem/mavi çadır, altın süsler, ışıklı mor çerçeve ve parlak butonlar ortak bileşenlerdir.
- Başarıda yıldız açılışı, coin vurgusu ve konfeti; başarısızlıkta kırık kalp; yeniden başlatmada dönüş oku kullanılır.
- Referanstaki bağlı kutular, oyundaki altın bağla bağlı üç pembe dubaya uyarlandı. Açıklama mevcut oyun kuralını kullanır: aynı renkteki top bağı koparır; üç duba sonraki atışta serbest kalır.
- Ayarlarda mevcut oyundaki ses ve Türkçe/English seçenekleri kullanılır. Uygulamada olmayan müzik/haptik ayarları eklenmedi.
- Önceki tasarım tercihi korunarak ekranlarda × kapatma bulunur; ayarlar için ek bir alt kapatma butonu yoktur.

## Etkileşimler

Ses anahtarı ve dil seçimi yalnız bu prototipin `pinpop.carnival-prototype.settings.v1` kaydına yazılır. Oyunun kayıt alanına dokunulmaz. Popup kapatma yalnız × ile yapılır. Hareket azaltma oyun ayarı kaldırılmıştır; yalnız tarayıcının sistem erişilebilirlik tercihi gözetilir.

Sonraki bölüm, özellik ilerlemesine gider. İlerleme ekranındaki Devam et, %60'tan %80'e; tekrar tıklamada Bölüm 7 özellik açılışına geçer. Ayarlardan yeniden başlatmaya gidilir; Yeniden başlatma onayındaki × ayarlara geri döner. Nasıl oynanır ayrı bir açıklama katmanı açar.

Tekrar dene, yeniden başlat, video ve Hadi oynayalım butonları görsel geri bildirim ve `pinpop-ui-action` olayı üretir. **Bu HTML'ler gerçek oyun turu başlatmaz, ödül vermez ve reklam oynatmaz.** Olayın `detail` alanında `screen`, `action`, `level`, `progress` bulunur; Unity entegrasyonu sonraki aşamadır.

## Yapı

`assets/ui.css`: ortak ekran görünümleri, responsive düzen ve animasyonlar.

`assets/ui.js`: ortak SVG çizimleri, altı ekranın içeriği, dil sözlüğü ve etkileşimler.

`assets/gallery.js`: galeri filtreleri ve yeniden oynatma.

`verification/`: tarayıcı ölçümleri ve önizleme görüntüleri.

## Bowling temalı arka plan önizlemesi

`bowling-tema.html`, ayarlar ekranını aynı çadır ve krem panelle gösterir. Dönme dolabı yerine üç mevcut karakterli duba görseli, üç delikli bowling topu ve kavisli yuvarlanma izi kullanır. Arka plan mor geçişlidir; kareli doku kaldırıldı ve küçük sabit parıltılar eklendi. Görseller düşük belirginliktedir ve dekor animasyonu yoktur. Ses ve dil kontrolleri mevcut HTML önizlemesindeki gibi çalışır. Galerinin üstündeki “Yeni bowling teması” bağlantısından açılabilir. Bu çalışma yerel HTML önizlemesidir.
