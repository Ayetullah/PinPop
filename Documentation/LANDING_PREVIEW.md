> Bu belge önceki havada atış sürümünün tarihsel kaydıdır. Güncel oyun masada yuvarlanır; düşüş alanı kaldırılmıştır. Güncel tasarım ve testler: [ROLLING_SHOT.md](ROLLING_SHOT.md).

# Belirgin tahmini düşüş alanı

Önceki küçük hedef noktası yerine beyaz dış çember, yarı saydam renkli dolgu, topun yarıçapını gösteren iç çember, merkez noktası ve dört yön işareti kullanılır. Dış alan yarıçapı 0.66, top izi yarıçapı 0.39 birimdir. Ekrandaki “TAHMİNİ DÜŞÜŞ” etiketi alanın ne anlama geldiğini açıklar. Hesaplanan nokta masanın dışındaysa “MASA DIŞI” yazılır ve masaya oturmayan bir halka gösterilmez.

Gösterge sabit nişan noktasına konmaz: ilk hız, yer çekimi, hava sönümü ve top yarıçapı kullanılarak, aşağı doğru inerken masa seviyesine ilk temas hesaplanır. Kesikli uçuş çizgisi bu noktada biter. Gösterge çekişte yön/güçle güncellenir ve atışın ilk hava bölümünde kalır. Duba teması uçuşu değiştirdiğinde tahmini alan gizlenir. Çarpışma sonrası yeni bir garanti hedef üretilmez.

Tahmin çarpışmasız ilk uçuş içindir; duba ve masa kenarı çarpmaları gerçek fizik motorunca çözülür. İzole düşük/yüksek güç ve gerilmiş başlangıç denemelerinde tahmin, gerçek fizik motorunun ilk masa temasıyla karşılaştırılmıştır.

Raporlar: `collision-core-verification.json`, `landing-browser-verification.json`, `pull-unity-verification.txt`. Görseller: `landing-area-aim.png`, `landing-area-390.png`, `landing-area-320.png`, `landing-area-1000.png`, `cartoon-pins-ready.png`.
