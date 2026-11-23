# Çizgi film karakter baskıları

Güncelleme: Set sekiz figüre genişletildi. Güncel renk/bölüm eşlemesi, yeni beş PNG ve üretim promptları [EIGHT_COLOR_PROGRESSION.md](EIGHT_COLOR_PROGRESSION.md) dosyasında. Aşağıdaki kayıt ilk üç figürün üretimini belgeler.

Üretim modu: yerleşik **image_gen** aracı, imagegen becerisi. CLI/API anahtarı kullanılmadı. Üç ayrı özgün karakter üretildi; alfa kanalı korundu. Kaynak PNG dosyaları 1254×1254, şeffaf köşe alfa değeri 0'dır.

## Projeye kaydedilen görseller

- [Pembe tavşan](/Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/Assets/Art/Characters/rabbit.png)
- [Turkuaz penguen](/Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/Assets/Art/Characters/penguin.png)
- [Mor kedi](/Users/ayetullah/Desktop/Projeler/Unity%20Projects/PinPop/Assets/Art/Characters/cat.png)

HTML dosyası üç görseli içinde taşır ve yükledikten sonra duba dokusuna çizer. Unity aynı kompozisyonun `Assets/Resources/PinSkins/pin-0.png`, `pin-1.png`, `pin-2.png` çıktısını kullanır. Ham karakterler proje içinde saklanır. Şeffaf karakter çevresine yeni arka plan eklenmez; baskı doğrudan duba gövdesinin üzerine gelir.

## Nihai prompt seti

Tam ve birebir kullanılan üç prompt, şeffaflık ayarı ve dosya eşlemesi [character-generation.json](character-generation.json) dosyasındadır. Ortak stil: önden görünen büyük başlı 2B çizgi film büstü, koyu lacivert kontur, canlı cel shading, şeffaf fon; yazı, logo ve gölge yok. Karakterler pembe/kremsi tavşan, turkuaz/lacivert penguen ve lavanta/kremsi kedidir.
