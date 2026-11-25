# iOS uygulama ikonu

Seçilen tasarım: yazısız, turkuaz arka planda pembe bowling topu ve üç karakterli duba. Oyun içindeki duba mesh'i, kaplamaları ve top görünümü doğrudan Unity'de render edilir. Kaynak render 2048×2048 alınır, ana PNG 1024×1024 üretilir. Köşe maskesi görsele işlenmez; arka plan tam opaktır.

Ana dosya: `Assets/AppIcons/iOS/PinPop-iOS-1024.png`.

Unity menüsü: **PinPop → iOS → Prepare app icons**. Tasarımı yeniden render eder, gerekli boyutları üretir ve yalnızca iOS ikon alanlarına atar. **Validate saved app icons** kayıtlı alanları, boyutları, PNG'nin alfa içermemesini ve renderda top/dubaların bulunmasını doğrular. `ProjectSetup.CreateScene()` ana görsel varsa mevcut iOS atamalarını korur/tamamlar.

Unity 6000.3.24f1 için 19 alan ve 13 farklı boyut hazırlanmıştır: 20, 29, 40, 58, 60, 76, 80, 87, 120, 152, 167, 180 ve 1024 piksel. Application, Spotlight, Settings, Notifications ve Marketing alanları kapsanır. Tüm PNG'ler RGB'dir; alfa kanalı yoktur. Görseller mipmap ve sıkıştırma olmadan, kaynak boyutları korunarak içe aktarılır.

Doğrulama: `ios-icon-verification.txt` ve `ios-icon-audit.json`. 1024, 180 ve 60 piksel çıktılar görsel olarak incelenmiştir. 120 piksel çıktı ayrıca kontrol edilir. Başka platform ikonları veya uygulama kimlikleri değiştirilmez. Bu çalışma Xcode arşivi, imzalama veya App Store yüklemesi oluşturmaz.
