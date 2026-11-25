# Altı hata düzeltmesi

Önceki ayrıntılı incelemede doğrulanan altı sorun uygulandı. Kullanıcının seçimi doğrultusunda 1000+ bölüm altyapısı bu çalışmanın dışında tutuldu; kampanya hâlâ 40 bölüm içeriyor.

## Değişiklikler

| Bulgu | Uygulanan davranış | Kanıt |
| --- | --- | --- |
| Buz materyali birikimi | Tüm buz kabukları ortak materyal kullanır. Buzlu bölümün 20 yeniden başlatılmasında materyal sayısı 1 → 1 kaldı. | [Yerel hedefli rapor](regressions/verification.json), [iPhone raporu](iPhone13-regressions/verification.json) |
| Engelin arkasına uzanan nişan | Tahmin, topun gerçek yarıçapıyla sabit ve hareketli engellere ilk temasta durur. Sabit engelde fizik ve nişan mesafesi aynı: 3,05 birim. | [Gerçek iPhone nişanı](iPhone13-regressions/aim-blocked.png) |
| Tepsi dışına taşan figürler | Tamamlanan üçlüler tek görünür yuvada üç figür olarak birleşir. Görünür yuva sayısını aşan rezervasyonlar sıraya alınır; gizli sıradaki uçuşların saati başlamaz. Birleşmede figür tekrar büyür; yuva boşaldıkça bekleyen rezervasyon görünür. | [12 figür](regressions/multi-collect-12.png), [48 figür](regressions/multi-collect-48.png) |
| Üst üste binen kombo metinleri | Ayrı ayrı metin üretmek yerine tek ortalanmış bildirim son kombo değerine güncellenir. Parçacık ve halka efektleri korunur. | [Tek kombo bildirimi](regressions/combo-12.png) |
| Tepsi dolması sonucu | Oynanış anında kilitlenir; son toplama/birleşme animasyonları gerçek zamanla tamamlanır. Figürler görünür olduktan sonra kaybetme açılır. Kart/düğme konumları ortak durum kullanır; ilgisiz reklam metni kaldırılır. | [Gerçek iPhone sonucu](iPhone13-regressions/overflow-1170.png) |
| Öğreticiye tekrar ulaşamama | Ana menü ve oyun ayarlarına Türkçe/İngilizce “Nasıl oynanır?” eklendi. Çarpıyla kapatılınca ayarlara dönülür, oyun duraklatılmış kalır. Ayarlar kapanınca önceki oyun/menü bağlamı devam eder. | [Yeni ayarlar](iPhone13-polish/settings-tr.png), [Öğretici](iPhone13-regressions/help-game.png) |

Nişan sorgusu önceden ayrılmış temas tamponunu kullanır. Tepsi sunum nesneleri de tekrar kullanılır; her karede yeni rezervasyon listeleri üretilmez. Normal duraklatma toplama uçuşlarını hâlâ durdurur. Gerçek zamanla ilerleme yalnızca tepsi dolması sonucu hazırlanırken kullanılır.

Taşma sonucu hazırlanırken yeni atış, SWAP ve ayarlar engellenir. Yeniden başlatma bekleyen sonucu iptal eder; eski denemenin sonucu yeni bölüme taşınmaz. Öğreticide ekstra Tamam düğmesi yoktur.

## Doğrulama

| Test | Başarılı kontrol |
| --- | ---: |
| Altı düzeltmeye özel yerel regresyon | 41 |
| İngilizce arayüz | 372 |
| Türkçe arayüz | 337 |
| Dil seçimi ve sonuç yerleşimleri | 284 |
| Ayrı işlemde dil kalıcılığı | 3 |
| 40 bölüm, harita, geçiş ve sandıklar | 148 |
| Video ile devam | 38 |
| Yardımcılar ve engeller | 65 |
| Gerçek fizik çarpışmaları | 36 |
| Üçlü eşleşme / tepsi sözleşmesi | 44 |
| **Yerel toplam** | **1368** |
| **Gerçek iPhone 13 hedefli regresyon** | **41** |

Son koşuların tamamında sıfır başarısız kontrol var. [Koşu sonuçları](runs.json) · [Makine özeti](verification-summary.json)

Hedefli senaryolar 12 ve 48 gerçek duba için normal toplama eşiklerini tetikler. Bunlar nişanla 48 dubayı tek atışta düşürebilme testi değildir; toplu toplamanın uçuş/kapasite sunumunu sınar. Tüm figürlerin yalnızca bir kez eşleştiği, kuyruğun ilerlediği, tepsinin dışına çıkılmadığı ve tek kombo metni kaldığı kontrol edildi. Normal duraklatma, taşma sırasında giriş kilidi, taşma ortasında yeniden başlatma ve sonuç düğmelerinin 320×568 / 390×844 / 1170×2532 boyutlarında kart içinde kalması sınandı.

Öğretici ana menüden ve 12. bölümden gerçek pointer raycast ile açılıp kapatıldı. Hamle, top sırası, deneme kimliği ve cüzdan korundu; öğreticide tek kapatma çarpısı kaldığı doğrulandı.

Eski test yardımcıları buton köşesi yerine merkezine gerçek raycast yapacak şekilde güncellendi. Reklam kurtarma testi sabit eski 8 hamle yerine güncel bölüm bütçesini doğrular. Basma animasyonu testi açıkça normal hareket modunda çalışır; hareketi azalt modu ayrı kontrollerde korunur. Dil arayüz testleri istenen dili açıkça seçip önceki tercihi işlem kapanışında geri yükler.

## iPhone ve kurulum

Aynı düzeltmelerle oluşturulan inceleme paketi iPhone 13’e yüklendi. 41 hedefli kontrolün tamamı geçti; sonra normal ekran/perfomans incelemesi de tamamlandı. Hedefli test tercihleri geri yükledi; normal incelemede cüzdan ve dil tercihi korundu, hata/exception listesi boş kaldı.

Kısa ölçümde oyun boşta ve nişan medyanı yaklaşık 16,7 ms/kare; nişanda en uzun kare 17,3 ms. Bu ölçüm uzun termal/pil testi değildir. [Ham performans raporu](iPhone13-polish/frame-health.json)

Normal Release derlemesi ve imzası doğrulandı. İnceleme sınıflarının Release IL2CPP çıktısında bulunmadığı kontrol edildi. Son normal sürüm mevcut uygulamanın üzerine yüklendi, açıldı ve yeni kurulumun çalışan işlemi doğrulandı. Reklam kimlikleri ve yayın reklam ayarı bu değişikliklere dahil değildir.

## Kaynaklar

- Oynanış: `PinPopGame.cs`, `PinTarget.cs`, `PinPopAdventure.cs`.
- Toplama ve efektler: `PinPopMatchTray.cs`, `HtmlMatchTrayUI.cs`.
- Ayarlar ve öğretici: `PinPopHUD.cs`, `HtmlReferenceHUD.cs`, `HtmlCrispHUD.cs`, lokalizasyon kataloğu.
- Geliştirme testleri: `AuditFixesReview.cs`, `CrispUIReview.cs`, `PinPopSelfTest.cs`. İnceleme girişi yalnızca Development oyuncularında vardır.
- [Tüm kaynak hash’leri](source-manifest.json), [üretim kaynak hash’leri](production-source-manifest.json).
