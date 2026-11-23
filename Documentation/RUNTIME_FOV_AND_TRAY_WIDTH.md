# Runtime FOV ve tahta genişliğine hizalanan toplama alanı

Play Mode'da **Game camera → Camera → Field of View** değeri doğrudan değiştirilebilir. Değer bir sonraki karede 45'e geri yazılmaz. `PinPopGame` üzerinde `Gameplay Field Of View` ve `Home Field Of View` alanları da vardır. Oyun ve ana menü kadrajları ayrı tutulur; oyun kadrajı menüye gidip dönüldüğünde korunur. Duraklatılmış ekranda kamera FOV düzenlemesi de çalışır. Bunlar çalışma anındaki ayarlardır; Play Mode bittikten sonra kalıcı kullanıcı ayarı olarak kaydedilmez.

Dar ekranlardaki otomatik görüş açısı uyarlaması korunur. Kamera bileşenindeki gerçek dikey FOV değişikliği mevcut ekran oranına göre temel kadraja çevrilir. Böylece doğrudan düzenlenen değer o ekranda sabit kalırken, ekran boyutu değiştiğinde kadraj uyarlanabilir. Kamera projeksiyonunun mevcut dikey kayması korunur.

Toplama paneli artık ekranın %90'ını sabit olarak kaplamaz. Dubaların bulunduğu arka tahta kenarının iki ucu kamerayla ekrana yansıtılır; panel bu kenarların arasına hizalanır. Ölçüm, tahta yüzeyinin 8,55 birimlik dış genişliğini ve arka kenar derinliğini kullanır. Tahta uçları ekran dışına çıkarsa görünür aralık kullanılır.

On yuva, boşluklar, panel yüksekliği ve yuvarlatılmış köşeler bu genişlikle orantılı ölçeklenir. Toplanan dubalar ve birleşme hedefleri aynı yuva koordinatlarını kullanır. Kapasite yazısı okunaklı boyutunu korur. FOV değiştirilince panel ve mevcut dubalar aynı karede yeniden hizalanır.

HTML sürümünde aynı davranış bulunur. Geliştirici konsolundan `PinPop.setFieldOfView(55)` ile FOV düzenlenebilir. `PinPop.getRendererInfo().fieldOfView` mevcut değeri, `PinPop.getTableEdges()` yansıtılan tahta kenarlarını verir.

`-fov-layout-test` gerçek Unity Camera bileşenini 35°, 45° ve 60° olarak değiştirir; kareler boyunca kalıcılığı, dört ekran boyutunda panelin tahtaya eşit genişliğini, yuva/duba hizalamasını, oyun/menü geçişini ve duraklatma durumunu doğrular. `fov-layout-browser-test.mjs` HTML karşılığını aynı ekran ve FOV değerleriyle kontrol eder. Ekran kayıtları bu klasörde `fov-layout-*.png` adlarıyladır.

Son doğrulama: Unity FOV/hizalama **39/39**, HTML **37/37**, mevcut atış/çim/yerleşim kontrolleri **25/25** geçti. Son derleme ve kaynak eşliği `fov-layout-final-audit.json` dosyasındadır.
