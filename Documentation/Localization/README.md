# Cihaz diline göre İngilizce / Türkçe

Cihaz dili Türkçe olduğunda oyun Türkçe açılır. İngilizce, bilinmeyen ve desteklenmeyen tüm diğer cihaz dillerinde İngilizce kullanılır. Dil seçimi açılışta Unity Application.systemLanguage üzerinden yapılır; dil tercihi kayıt dosyasına yazılmaz. iOS paketinde desteklenen diller en/tr, geliştirme ve varsayılan bölge en olarak bildirilir.

276 metinlik katalog Assets/Resources/Localization/strings.json içindedir. Mevcut Türkçe metinler sabit anahtardır; İngilizce çeviriler en alanında, gerekli tekil biçimler enOne alanındadır. Ortak CrispText bileşeni statik metinleri çevirir. Sayı ve ad içeren cümleler GameLocalization.Format ile tam cümle şablonundan oluşturulur. Rakam ayırıcıları dile uyar; bölüm, yardımcı ve kayıt kimlikleri çevrilmez. Menü, ayarlar, harita/bölge/durak adları, öğreticiler, geçişler, top mağazası, SWAP, yardımcılar, sonuçlar, toparlanma ekranı ve video durum mesajları kapsamdadır.

Splash logosu ve native açılış görseli dilden bağımsız hazırlanır; alt başlık çalışma anında çevrilir. Menü arabasındaki görsele gömülü BÖLÜM yazısı İngilizcede hazırlanmış LEVEL şeridiyle değiştirilir. Runtime doku okuma ayarı korunur. Daha uzun İngilizce bildirimler için mesaj alanı iki satıra genişletilmiştir.

## Doğrulama

- İngilizce arayüz: 347/347 başarılı kontrol.
- Türkçe arayüz: 314/314 başarılı kontrol.
- İngilizce animasyonlu öğretici: 53/53; çarpıyla üç adımda kapanma, kaydırma, otomatik anlatım, hamle/sıra/bakiye koruması.
- İki dilde splash: menüye ulaşma, kamera boşluğu ve logo çakışması olmaması, bakiye koruması doğrulandı.
- Altı ekran boyutu, iki oynanabilir bölge haritası ve dört tema kataloğu, gerçek bölüm geçişi, yardımcılar, mağaza, sonuçlar ve video kullanılamama metinleri kontrol edildi. Kullanılan arayüz metinlerinde katalog eksiği yok.
- Cihaz dili eşlemesi tüm Unity SystemLanguage değerlerinde kontrol edildi; Türkçe dışındaki tüm değerler İngilizceye düşer.
- Katalog kontrolü: benzersiz anahtarlar, eşleşen yer tutucular, bütün harita temaları ve Türkçe runtime metinlerinin kaydı.

[İngilizce menü](en-ui/home.png) · [Türkçe menü](tr-ui/home.png) · [İngilizce öğretici](en-ui/how-to-1.png) · [İngilizce geçiş](en-ui/transition.png) · [Arayüz EN raporu](en-ui/verification.txt) · [Arayüz TR raporu](tr-ui/verification.txt)

Yeniden kontrol: python3 Tools/verify_localization.py. Unity PinPop/Prepare production assets menüsü gerekli tabela ve splash kaynaklarını hazırlar. Development incelemesinde PINPOP_REVIEW_LANGUAGE=en veya tr kullanılabilir; bu geçersiz kılma kodu Release paketine dahil edilmez. Üretim paketi cihaz dilini kullanır.

Bu çalışmada henüz gerçek iPhone üzerinde iki dil çalıştırılmadı; bağlı cihaz bulunamadı. Canlı reklam ayarları ve ödül kuralları değiştirilmedi.

İmzalı iOS Release derlemesi tamamlandı. Paketin Info.plist dil tanımları, resources.assets içindeki çeviri kataloğu, yerelleştirme kodu ve test dil zorlamasının çıkarılmış olması doğrulandı. [Release doğrulaması](release-verification.json).
