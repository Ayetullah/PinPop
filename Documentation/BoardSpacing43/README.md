# Tahta–alt panel mesafesi ve FOV 43

Referans görseldeki boşluk azaltıldı. Tahtanın ön sınırı -6.8 yerine -13; arka sınırı 3.2 olarak kaldı. Top ön kenara bağlı konumunu koruyor. Fizik yüzeyi, nişan ve çimen sınırları ortak tahta ölçülerini kullanıyor.

Oyun kamerası FOV değeri hem sahnede hem kod varsayılanında 43. Oyun sırasında en/boy oranına göre FOV büyütülmüyor; gerçek Camera.fieldOfView=43. Kamera (0,26,-23) konumundan (0,.2,-4) noktasına bakıyor. Runtime Camera FOV düzenlemesi desteklenmeye devam ediyor.

Arayüz kontrolleri: 362 başarılı, 0 başarısız. 390×844 ekranda fizik kenarı ile alt panel arasındaki boşluk 46.88 piksel; 1170×2532 ekranda 74.63 piksel. Test edilen altı ekran boyutunda panelle çakışma yok ve gerçek FOV 43. Ekran görüntüleri incelendi. Fiziksel iPhone testi yapılmadı.

40 bölüm yerleşimi, uzatılmış fizik sınırları ve yeni ön kenardan gerçek atış: 196/196 passed.
