# Mobil UI netliği ve polish

- Ana menü, oyun HUD'ı, ayarlar ve kazanma ekranı canlı metin ve kenar yumuşatmalı UI geometrisiyle çizilir. Düşük çözünürlüklü HTML ekran görselleri bu ekranlarda gizlidir.
- Tek alt panel: dört yardımcı solda; sıradaki üç top, SWAP ve hak satın alma sağda. Güvenli alan hesaba katılır. Yardımcı sembolleri font karakteri kullanmaz.
- Nişan noktaları ekran boyutuna göre ölçeklenir ve aralıkları korunur. Güç halkası turkuaz/sarı/mercan geçişlidir; maksimumda MAKS etiketi ve mevcut azaltılmış hareket tercihini izleyen vurgu gösterir.
- Kazanma ekranı üç ayrı yıldız, ödül detayı ve yuvarlatılmış dikdörtgen butonlar kullanır. Ödül efektleri kartın metin alanını kapatmaz.
- UI metinleri gömülü PinPop fontlarını kullanır. Top ikonları 128/256/512 piksel kademelerinde önbelleklenir. iOS ikon dokuları mipmap ve kayıplı sıkıştırma kullanmaz. Oynanabilir sahnenin alan derinliği bulanıklığı kapatılmıştır; menü/overlay arka plan efekti korunur.

## Doğrulama

`CrispUIReviewBuild.Export` izole macOS development uygulamasını üretir. `-crisp-ui-review` yalnızca development/editor derlemesinde çalışır, oyun kameralarıyla görüntü üretir; ilerleme veya ekonomi kaydını değiştirmez. Çıktı klasörü `PINPOP_QA_DIR` veya uygulama veri klasöründeki `PolishReview` dizinidir.

`verification.txt`: 34/34 kontrol geçti. 320/390/430 genişlikte yardımcı başlıkları ve dokunma genişlikleri; %25/%60/%100 güç; 1/2/3 yıldız; 512 px top ikonu doğrulandı. PNG'ler sıkıştırılmamış uygulama render çıktılarıdır; iPhone ekran görüntüsü değildir.

Unity iOS dışa aktarımı ve imzalı ARM64 Debug derlemesi başarılı. Son sürümün gerçek cihaz kurulumu ve açılış doğrulaması cihazın yeniden bağlanmasını bekliyor. Yerel kayıtlar sıfırlanmadı.
