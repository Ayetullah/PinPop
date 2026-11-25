# Yayın öncesi arayüz düzenlemesi

Yardımcılar solda dikey sıralanır. Alt alanda sıradaki üç top ve SWAP bulunur. Kamera, tahta ile yardımcıların üst üste gelmemesi için ayrılmış oyun alanını kullanır; runtime FOV düzenlemesi korunur.

Metin üretimi gerçek çizim yoğunluğunu dikkate alır; ölçeklenen panellerde font atlası büyütülür ve atlas yenilemelerinden sonra yazılar tekrar çizilir. İkon kenarları yumuşatılır, küçük resimler iOS'ta sıkıştırılmadan kullanılır. Renk seçeneklerinde yazı kontrastı zemine göre seçilir.

Butonlar sabit dokunma alanını korurken basma animasyonu oynatır. Yardım ekranlarında engel, buz, bağ ve hareketli bariyer için animasyonlu örnekler bulunur. Azaltılmış hareket tercihi desteklenir. Çekme yüzdesi ve MAX metni yoktur; güç halka rengi ve yoğunluğuyla gösterilir. Sonuç ekranı, menü, harita, mağaza, ayarlar ve öğreticiler aynı net metin yolunu kullanır. iOS için markalı native açılış ekranı eklenmiştir.

## Doğrulama

`verification.txt`: 163 başarılı, 0 başarısız otomatik kontrol. 320×568, 390×693, 390×844, 430×932, 768×1024 ve 1170×2532 boyutları kontrol edildi. Son boyut gerçek offscreen render hedefidir; fiziksel Mac penceresi boyutuyla sınırlı değildir. iPhone 13 için simüle edilen güvenli alan: x=0, y=102, genişlik=1170, yükseklik=2289. PNG dosyaları bu çalışmanın görsel kanıtıdır.

Kontroller yerleşim, güvenli alan, dokunma boyutu, metin taşması, eksik glif, güç geri bildirimi, yardımcı panelleri, sonuç yıldızları, öğretici türleri ve buton davranışını kapsar. Bu çalışma fiziksel cihaz testi veya performans ölçümü değildir. Kullanıcı telefonu şu anda bağlayamadığı için iPhone üzerindeki son dokunma, geçiş, performans ve netlik kontrolü beklemektedir.

## iOS Release adayı

Export: `Builds/iOSReleaseCandidate/Unity-iPhone.xcworkspace`

Uygulama: `Builds/iOSReleaseDerivedData/Build/Products/Release-iphoneos/PinPop.app`

Development Build ve Script Debugging kapalıdır. Test/review bileşenleri Release kodundan çıkarılır. Kullanıcının isteğiyle test reklamları korunur; canlı reklam açılmaz.

```sh
python3 Tools/ios_device_test.py export --release
python3 Tools/ios_device_test.py build --release --generic
python3 Tools/ios_device_test.py store-check --release
```

Telefon bağlandığında kayıtları silmeden `install --release`, ardından `launch --release` kullanılabilir. Unity export her yenilendiğinde native build tekrar yapılmalıdır.

## Mağaza yüklemesi için kalan ortam koşulları

Mevcut Mac macOS 15.5, Xcode 16.4 ve iOS 18.5 SDK kullanıyor. Apple, 28 Nisan 2026 itibarıyla App Store yüklemelerinde Xcode 26+ ve iOS 26 SDK+ istiyor. Bu nedenle mevcut imzalı paket cihaz testi adayıdır; mağazaya gönderilebilir dağıtım paketi değildir. Xcode 26 için en az macOS 15.6 gerekir. Sistem yükseltmesi yapılmamıştır. App Store dağıtım profili/Archive işlemi ayrıca gereklidir.

Kaynaklar: [Apple SDK gereksinimi](https://developer.apple.com/news/upcoming-requirements/?id=04282026a), [Xcode sistem gereksinimleri](https://developer.apple.com/xcode/system-requirements/).

`store-check --release` SDK ve dağıtım profili uygun olmadığında exit 2 döndürür; bu kontrolün başarısız olması bu ortamda beklenen sonuçtur.
