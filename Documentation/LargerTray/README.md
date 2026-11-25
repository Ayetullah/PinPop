# Daha büyük toplama boardı ve okunur eşleşmeler

Boardın genişliği artırıldı. Küçük ekranlarda yardımcı butonların sağında güvenli boşluk korunuyor. 1014×1792 referans çözünürlüğünde board 512×85 yerine 653×135 piksel: yaklaşık %28 daha geniş, %59 daha yüksek.

Duba ikonları tam kaynak dokudan, şeffaf kenarları alınmış portre sprite olarak çiziliyor. Tam görsel korunuyor; kafası/gövdesi kırpılmıyor. Portre yüksekliği 92 tasarım birimi; önceki 47.5 birim kareye göre ve board büyümesiyle birlikte dubalar yaklaşık iki kat veya daha büyük görünüyor.

Üçlü grubun görünür ölçeği .44 yerine .58, merkez aralığı .22 yerine .30 slot. Birleşmenin başlangıcında SmoothStep kullanılıyor; üç duba ayrı ayrı seçiliyor, ardından birleşip büyüyerek patlıyor. Oyun kapasitesi ve eşleşme zamanları aynı. +3 yazısı büyütüldü.

399 UI/görünüm kontrolü ve 41 yoğun toplama/taşma kontrolü geçti. Referans ekran, dolu board, üçlü birleşme ve patlama görüntüleri incelendi. Yoğun toplama kontrolü tercih/cüzdan durumunu geri yükledi. Fiziksel iPhone testi yapılmadı.
