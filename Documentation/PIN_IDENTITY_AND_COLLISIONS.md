# Duba görünümü, sabit çekiş ve katı çarpışmalar

## Toplanan nesne de duba

Toplama alanındaki kare renk parçaları kaldırıldı. Uçuş, yerleşme ve üçlü birleşme boyunca sahnedeki `Art.PinMesh()` ve `Art.PinCoat()` ile oluşturulmuş gerçek duba görselleri kullanılır. Beyaz kafa, renkli gövde ve baskı korunur. Görseller `Assets/Resources/PinIcons` içindedir; editörde `PinPop/Bake collection pin icons` komutuyla eksik dosyalar üretilebilir. Unity UI oranı korur ve üçlü ipucu duba siluetinin çevresinde görünür. HTML sürümünde aynı görseller dosyanın içine gömülüdür.

## Top çekiş boyunca yerinde kalır

Topun hazırlık, çekiş ve bırakma başlangıcı tek bir `Launch` noktasıdır. Parmak/fare konumu yalnızca yönü ve gücü hesaplar. Çekişte topu masanın arkasına/yanına taşıyan ışın-düzlem hesabı kaldırıldı. İptal ve yeniden başlatma aynı noktayı kullanır. Atış oluşturulurken de başlangıç noktası sabitlenir; farklı bir başlangıç gönderen çağrı topu taşıyamaz.

## Her dubanın katı çarpışması

Farklı renkteki duba çiftlerinde `Physics.IgnoreCollision` kullanımı kaldırıldı. Cannon sürümündeki yalnızca kendi rengine çarpma maskesi de kaldırıldı. Renk, topun hangi dubaları hareket ettirebileceğini belirler; dubalar her renkte fiziksel engel olarak kalır.

Her duba üç katı, trigger olmayan konveks gövdeden oluşur: alt gövde, boyun ve tam kafa. Bu gövdeler görsel silueti tabandan 1,53 birim yüksekliğe kadar kapsar. Önceki küçük/eksik silindir-küre birleşimi yerine duba profili kullanılır. Unity'de dinamik dubalarda açısal hareketi de hesaba katan Continuous Speculative çarpışma algılama, 120 Hz fizik ve gövde başına 24 konum/12 hız çözücü adımı kullanılır. Kilitli renkler kinematik ve katı kalır. HTML karşılığı aynı profillerden oluşturulmuş konveks gövdeler ve tüm renk çiftlerini içeren çarpışma maskeleridir.

## Kontrol kapsamı

- `-pin-behavior-test`: 12 bölümün başlangıç ayrıklığı ve her renk çiftinin etkin collider'ları; altı yönde çekiş sırasında top konumu; sabit noktadan bırakma; üç farklı renk çifti ve aynı renk gerçek temas; tam güçte serbest dönen dubalar; fiziksel düşüşten sonra duba görseli; üç ekran boyutu.
- `pin-behavior-browser-test.mjs`: gerçek fare girdileriyle sabit çekiş, üç renk çiftiyle katı engel, gerçek duba PNG'si, üçlü birleşme ve dört ekran boyutunda yerleşim.
- `-smash-match-test`: düşme, üçlü zamanlamaları, duraklama, video ile devam, son birleşmeden sonra zafer ve tek ödül kontrolleri.
- `-tray-polish-test`: mevcut panel ve ikili duba silueti vurgusu.

Fizik ölçümleri yalnızca görsel izlenime dayanmaz: Unity'de `Physics.ComputePenetration` ile gerçek collider çiftleri kontrol edilir. Sonuç ve ekran görüntüleri bu klasörde `pin-behavior-*` adlarıyla bulunur. Eski raporlar eski başlangıç konumu ve collider biçimini yansıtabilir; bu değişiklik için güncel rapor kullanılmalıdır.

Dönen gövdelerde mod seçiminin gerekçesi: [Unity Continuous Speculative belgesi](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/collisiondetectionmode/continuousspeculative). Ölçüm sonuçları uygulamanın kendi collider testlerinden alınır.

Son doğrulama: yeni Unity kontrolleri **35/35**, düşme/birleşme kontrolleri **44/44**, panel kontrolleri **9/9**, tarayıcı **21/21**, model **33/33** geçti. Tam güçlü üç serbest dönüşlü atışta ölçülen en büyük geçici collider örtüşmeleri 0,003965 / 0 / 0,002756 dünya birimidir; renkler arası yönlendirilmiş temaslarda örtüşme sıfırdır. Son uygulama ve kaynak eşliği `pin-behavior-final-audit.json` içinde kayıtlıdır.
