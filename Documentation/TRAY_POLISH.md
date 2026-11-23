# Toplama alanı görsel düzenlemesi

İstenen alan, kullanıcının ekran görüntüsündeki 10 yuvalı toplama panelidir. Bu çalışma panelin görünümünü ve geri bildirimlerini günceller.

- Mavi tonlarda yükseltilmiş çerçeve, ince açık kenar ve aşağı düşen gölge.
- İçe gömülü, kare oranını koruyan yuvalar; boş yuvada küçük ışık izi.
- Üç renkli küçük simge, kısa “ÜÇLÜ TOPLA” başlığı ve ayrı kapasite rozeti.
- Parçalarda parlak üst yüzey, koyu alt kenar ve küçük yüzey yansıması.
- Aynı renkten iki parça beklediğinde ince ışık çerçevesi; eşleşmeyen renk vurgulanmaz.
- Yerleşmede yuva rengi kısa süre canlanır. Üçlü tamamlandığında çerçeve kısa süre aydınlanır; mevcut halka, kıvılcım ve kombo yazısının kontrastı güçlendirilir.
- Sekiz eşleşmemiş parçadan itibaren kapasite rozeti ve çerçeve sıcak uyarı tonuna geçer.

Unity görselleri çalışma anında üretilen, yeniden kullanılan küçük yuvarlatılmış yüzey sprite'larıdır. Yeniden boyutlandırmada kenar kalınlığı ekranla ölçeklenir; HUD yeniden kurulurken önceki dokular ve olay abonelikleri temizlenir. Süslemeler pointer/raycast yakalamaz. Hareket azaltma tercihinde ikili vurgusu sabittir. Aynı stil bağımsız HTML sürümüne de uygulanır.

Oyun kuralları, 10 yerlik kapasite, üçlü eşleşmenin zamanlaması, hamleler ve coin hesabı bu çalışmada değiştirilmez.

Görsel durum kayıtları `tray-polish-*.png`, HTML kayıtları `tray-polish-browser-*.png` adlarıyla bu klasördedir. Kapasite uyarısı, bir görsel test düzeneğinde oluşturulur; mevcut üç renkli bölümlerde doğal maksimum eşleşmemiş sayısı daha düşüktür.

Doğrulama: tarayıcıdaki durum ve yerleşim kontrolleri **13/13**, Unity panel kontrolleri **9/9**, mevcut düşme/birleşme fizik kontrolleri **44/44**, model kontrolleri **33/33** geçti. Son değişiklik yalnızca birleşme yazısının başlık üzerindeki konumuydu; bu konum son panel kontrollerinde tekrar görüntülendi.
