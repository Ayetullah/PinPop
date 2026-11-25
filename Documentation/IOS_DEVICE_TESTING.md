# Gerçek iPhone / iPad testi

## Hazırlananlar

- Unity 6000.3.24f1 iOS Build Support zaten kurulu; gerçek cihaz hedefiyle C# derlemesi ve IL2CPP/Xcode dışa aktarımı başarıyla tamamlandı.
- Bundle ID: `com.pinpop.colorbowling`, build number: `1`, minimum iOS: `15.0`.
- Device SDK, ARM64, IL2CPP, Metal ve otomatik imzalama etkin. Apple Team ID bilinmediği için boş bırakıldı.
- iPhone/iPad/App Store ikonları dahil 19 iOS ikon alanı atanmış durumda.
- Reklamlar test modunda. Google örnek iOS App ID, SKAdNetwork girdileri ve yerel erişilebilirlik eklentisi dışa aktarılan projede doğrulandı.
- CocoaPods 1.17.0 kullanıcıya özel kuruldu: `~/Library/PinPop/iOS-tools/bin/pod`. Ortak Homebrew dizininin izinleri değiştirilmedi.
- Mevcut reklam paketi varsayılan olarak Swift Package Manager kullanıyor. Xcode projesinde Google Mobile Ads 13.12+ ve Google UMP 3.1.0 referansları bulunuyor. Bağımlılıklar çözüldü; Xcode 16.4 ile imzasız ARM64 Debug derlemesi başarıyla tamamlandı.

Xcode çıktısı: `Builds/iOSDevice/Unity-iPhone.xcworkspace`. Workspace ile projeyi aç; şema **Unity-iPhone**.

## Mevcut kurulum ve kalan adımlar

Xcode 16.4 (16F6), resmi Apple arşivinden `/Applications/Xcode-16.4.app` konumuna kuruldu. Arşiv ve uygulamanın Apple imzası, Gatekeeper kabulü ve ilk açılış hazırlığı doğrulandı. Sistem `xcode-select` ayarı değiştirilmedi.

iPhone 13 / iOS 27.2.1 Xcode tarafından eşleşmiş ve kullanılabilir olarak görüldü. Son başarılı sorguda Geliştirici Modu kapalıydı; sonraki sorgu CoreDeviceService başlangıç zaman aşımı verdi. Native ARM64 derleme `BUILD SUCCEEDED` ile tamamlandı; oluşturulan uygulama imzasızdır ve henüz cihaza kurulmadı.

1. `/Applications/Xcode-16.4.app` uygulamasını aç. **Settings → Accounts → Apple Accounts** altında hesabınla giriş yap. Apple web sitesindeki oturum Xcode oturumunu oluşturmaz.
2. `Builds/iOSDevice/Unity-iPhone.xcworkspace` dosyasını aç. **Unity-iPhone → Signing & Capabilities → Team** alanında kendi takımını seç; **Automatically manage signing** açık kalsın. Şu an takım alanı boş ve geçerli kod imzalama kimliği yok.
3. iPhone'da **Ayarlar → Gizlilik ve Güvenlik → Geliştirici Modu** seçeneğini aç; yeniden başlatma ve cihaz onaylarını tamamla. [Apple Developer Mode rehberi](https://developer.apple.com/documentation/xcode/enabling-developer-mode-on-a-device)
4. İmzalı build, kurulum ve başlatma için aşağıdaki komutları çalıştır veya Xcode'da cihazı seçip **Run** kullan. [Apple cihazda çalıştırma rehberi](https://developer.apple.com/documentation/Xcode/running-your-app-on-simulated-or-physical-devices)

Native Xcode arayüzünü otomasyonla kontrol etme izni bulunmadığından hesap ve Team seçimini kullanıcı tamamlar. İşletim sistemi yükseltmesi yapılmadı.

## Tekrarlanabilir komutlar

Proje klasöründen:

```sh
python3 Tools/ios_device_test.py doctor
python3 Tools/ios_device_test.py export
python3 Tools/ios_device_test.py build --team APPLE_TEAM_ID
python3 Tools/ios_device_test.py install
python3 Tools/ios_device_test.py launch
```

Birden fazla cihaz varsa `--device DEVICE_IDENTIFIER` kullan. Araç Apple şifresi veya doğrulama kodu istemez ve saklamaz. `doctor` sonucu `UserSettings/ios-device-host.json` içine yazılır; bu dizin sürüm kontrolüne dahil değildir. Eksik Xcode/cihaz/imzalama durumunda hazır raporlamaz.

`export`, ana Unity projesinin açık olmasıyla çakışmamak için `Builds/.ios-export-cache` içinde izole bir proje kullanır. Yerel Unity sürümü `ProjectVersion.txt` içinden bulunur. Apple Team ID isteğe bağlı `PINPOP_APPLE_TEAM_ID` ortam değişkeniyle verilebilir. Xcode kurulmuşsa araç kendi süreçlerinde `DEVELOPER_DIR` kullanır; sistemdeki `xcode-select` seçimini değiştirmez. Xcode üzerinden Run, CLI'nin cihazı desteklemediği durumlarda da kullanılabilir.

Unity menüleri: **PinPop → iOS → Configure real device testing** ve **Export development Xcode project**.

## İlk cihaz testi

- Ana ekran ikonu, splash → ana menü, açılmış bölge ve mevcut kayıt.
- Parmağı geri çekerek atış, yana konumlandırma, güç halkası, aynı/farklı renk teması.
- Topların çimende yuvarlanması, üçlü tepsi animasyonu, swap ve dört yardımcı.
- 20. bölüm sonrası 21. bölüm; iki bölgede ayrı 20 bölüm ve kayıtların korunması.
- Test ödüllü reklamını aç/kapat, ödülün bir kez uygulanması; arka plana alıp geri dönme.
- Cihazda görsel, ses, dokunma, performans ve çökme kontrolü. Bu maddeler henüz fiziksel cihazda doğrulanmadı.

Kanıt dosyaları: `ios-device-settings-verification.json`, `ios-device-export-verification.json`, `ios-device-preparation-audit.json`. Bunlar imzalı native build veya gerçek cihaz testi tamamlandı anlamına gelmez.


## Son kurulum sonucu

Apple Team seçildi ve Unity ayarına kaydedildi. Geliştirici Modu etkin. İmzalı native Debug derlemesi başarılı ve PinPop iPhone 13 cihazına kuruldu. İlk açılış iOS güvenlik denetimi tarafından reddedildi; cihazdaki geliştirici profiline güven onayı bekleniyor. Ayarlar → Genel → VPN ve Aygıt Yönetimi altında kendi geliştirici hesabına güven verildikten sonra launch komutu yeniden denenmeli. Oynanış henüz cihazda doğrulanmadı.

Güven onayı sonrası `devicectl` ile açılış başarılı oldu. Ardından cihaz süreçleri sorgusunda PinPop çalışır durumda doğrulandı. Kurulum ve açılış tamamlandı; görsel, dokunma ve oynanış maddeleri kullanıcıyla cihazda test edilmeyi bekliyor.

## Açılış hatası düzeltmesi

Cihaz konsolunda önce eksik runtime shader nedeniyle ArgumentNullException, ardından CreatePrimitive için çıkarılmış CapsuleCollider nedeniyle NullReferenceException yakalandı. Shader bağımlılıkları GraphicsSettings ve build öncesi doğrulamayla korunuyor; native oluşturulan collider/renderer türleri Assets/link.xml ile korunuyor. Yeni imzalı sürüm cihaza yüklendi. Konsolda `PINPOP: Startup complete; home ready, level=0` görüldü ve açılış boyunca managed exception oluşmadı. Tam oynanış testi bundan ayrıdır.
