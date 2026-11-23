# Video ile +3 hamle ve polish

Hamleler bittiğinde, henüz devrilmeyen duba varsa sonuç ekranı videoyu isteğe bağlı sunar. Tamamlanan video aynı masaya tam üç hamle ekler; duba pozları, devrilen duba sayısı, top sırasının mevcut başı, seçili görünüm, coin ve swap bakiyesi korunur. Mevcut kuyruğa üç ek top hazırlanır. Videoyu erken kapatmak, yükleme hatası veya eski denemenin geç gelen bildirimi ödül vermez.

Her denemede bir tamamlanmış video desteği kullanılabilir. Ek hamleler de tükenirse yeniden video yerine tekrar deneme sunulur. Yeni deneme normal hamle sayısı ve yeni video hakkıyla başlar. Videoyla kazanılan bölüm 30 coin taban ödülü kazanır; ek hamleler tasarruf bonusu veya fazladan yıldız üretmez. Önceden alınan bölüm ödülü ve iyileştirme geçmişi korunur.

## AdMob

Google Mobile Ads Unity SDK **11.7.0**, resmi Google dağıtımından `Packages/com.google.ads.mobile` içine sabitlendi; lisansı paketle birlikte bulunur. Unity External Dependency Manager 2.1.1 ve Video modülü paket kilidinde çözülüdür.

Android ve iOS, UMP izin akışından sonra AdMob'u başlatır, ödüllü reklamı hazırlar ve sadece SDK ödül bildirimi gelmişse reklam kapanınca oyuna devam eder. Unity nesnelerine dokunan SDK bildirimleri `MobileAdsEventExecutor.ExecuteInUpdate` ile ana iş parçacığına taşınır. Yükleme/gösterme hatası coin veya hamle üretmez. İstekler sonsuz hata döngüsüne girmez; kullanıcı tekrar deneyebilir. İzin tercihi bağlantısı UMP gerektiğini bildirdiğinde ayarlarda görünür. Oyunun sessiz ayarı AdMob'a da aktarılır.

Geliştirme ayarı: **Google test reklamları**. Canlı yayın için:

1. `Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset` dosyasındaki Android ve iOS uygulama kimliklerini kendi AdMob uygulama kimlikleriyle değiştir.
2. `Assets/Resources/AdMobConfig.json` içindeki platform ödüllü reklam birimi kimliklerini doldur; canlıya geçerken `useTestAds` değerini `false` yap.
3. Android/iOS cihazında reklam yükleme, erken kapatma, ödül ve izin tercihlerinin gerçek SDK akışını doğrula. Bu makinede mobil Build Support bulunmadığı için cihaz reklamı bu çalışmada çalıştırılamadı.

Mac, Unity Editor ve bağımsız HTML sürümü gerçek **8 saniyelik PinPop tanıtım MP4 videosunu** oynatır. Görüntüler kendi oyunumuzdan kaydedildi. Bu, canlı AdMob reklamı olarak gösterilmez; başlık açıkça “PINPOP TANITIMI”dir. Ödül gerçek medya bitişine bağlıdır. Erken çıkış, hız değiştirme veya sona sarma ödül vermez. Uygulama arka plana geçince yerel video duraklar. HTML videosu dosyanın içine gömülüdür; sunucu gerektirmez.

## Polish

- SWAP üzerindeki görünür `+` diski 44 pikselden **30 piksele** küçültüldü; rahat dokunma için 44 piksel giriş alanı korundu.
- Hamle bitiş ekranında masayı koruyarak devam etme seçeneği, tekrar deneme ve ana menü ayrı ve okunaklı sunulur.
- Video ekranı gerçek oynatma ilerlemesini ve kalan süreyi gösterir; vazgeçme seçeneği ödül vermeden geri döner.
- +3 hamle verildiğinde hamle sayacı/top sırası canlanır ve kısa devam bildirimi gösterilir. Hareket azaltma tercihi korunur.
- Native mağaza ve yeni sonuç düğmeleri basılma geri bildirimi alır; tıklama alanı yerinde kalır.
- Video ve bölüm geçişinden sonra klavye odağı oyuna döner. Gizlenmiş UI düğmesi klavye girdisini yutmaz.
- Nasıl oynanır butonu yalnızca ilk bölümde görünmeye devam eder.

## Doğrulama

Güncel sonuçlar `recovery-model-verification.json`, `recovery-browser-verification.json`, `recovery-unity-verification.txt` ve `playthrough-browser-verification.json` dosyalarındadır. Unity için `-recovery-test`, temel ekonomi için `economy-core-test.cjs` çalıştırılır. Baştan sona tarayıcı oynayışı `playthrough-browser-test.mjs` ile gerçek çek-bırak girişleri kullanır; doğal bir başarısızlıkta video desteğiyle devam eder. Oyunda 12 farklı bölüm düzeni bulunur, sonraki bölümlerde bunlar tekrar eder.

Resmi kaynaklar: [AdMob Unity kurulumu](https://developers.google.com/admob/unity/quick-start), [ödüllü reklam akışı](https://developers.google.com/admob/unity/rewarded), [Google test reklam birimleri](https://developers.google.com/admob/unity/test-ads), [ana iş parçacığında bildirimler](https://developers.google.com/admob/unity/global-settings), [SDK 11.7.0 dağıtımı](https://github.com/googleads/googleads-mobile-unity/releases/tag/v11.7.0).

Son doğrulama: yeni native video/sonuç akışı **38/38**, native mağaza/coin/basılma geri bildirimi **96/96**, native 12 bölüm ilerlemesi **24/24**, tarayıcı video sınır durumları **19/19**, kurtarma modeli **10/10**, temel ekonomi **37/37**. Tarayıcı kampanyası 12 düzenin tümünü bitirdi; 12. bölümde ilk denemede kalan bir duba, sonraki gerçek denemede tamamlandı. Native ve tarayıcı görüntüleri incelendi. Ayrıntılı gereksinim eşleştirmesi: `rewarded-polish-completion-audit.json`.
