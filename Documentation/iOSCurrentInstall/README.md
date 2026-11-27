# Güncel iPhone kurulumu

Güncel kaynaklardan Release iOS çıktısı alındı; CocoaPods çalışma alanı hazırlandı, Xcode derlemesi başarılı oldu. İmza ve bağlı iPhone’un provizyon profiline dahil olduğu doğrulandı. Güncelleme devicectl ile mevcut uygulamanın üzerine kuruldu; kaldırma veya kayıt silme yapılmadı.

Cihaz: iPhone 13, iOS 27.2.1. Uygulama: com.pinpop.colorbowling, 1.0 (1). Başlatma başarılı; başlatılan işlem cihazda tekrar sorgulanarak çalıştığı doğrulandı. Son ortak satın alma arayüzü ve çime temasından 2 saniye sonra parçalanma kaynakları iOS derlemesine dahil. Fiziksel dokunma/oynanış kontrolü bu adımda otomatik yapılmadı.

verification.json kaynak ve imzalı uygulama hashlerini, install.json ve launch.json cihaz kurulum/başlatma sonuçlarını içerir. Ayrıntılı derleme kayıtları /tmp/pinpop-ios-current-export.log, /tmp/pinpop-ios-current-pods.log ve /tmp/pinpop-ios-current-xcode.log içindedir.
