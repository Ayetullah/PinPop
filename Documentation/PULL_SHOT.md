# Çekiş gücüyle uzak hedefe atış

- Öndeki tek aktif top tutulup aşağı/geriye çekilir. Yana çekişin ters tarafına nişan alınır.
- Çekiş mesafesi yüzde güç göstergesine, ilk hıza ve topun momentumuna dönüşür.
- Top zemine temas eden öndeki atış noktasından yer çekimiyle yay çizerek uzaktaki dubalara uçar. Tahmin çizgisi aynı başlangıç hızı, hava sönümü ve yer çekimiyle hesaplanan düşüş alanında biter. Alan belirgin beyaz çember, renkli dolgu ve “TAHMİNİ DÜŞÜŞ” etiketiyle gösterilir.
- Top ve dubalar artık dinamik 3B fizik cisimleridir. Normal, sürtünme, esneklik ve kütle çarpışma yönünü belirler; dubalara temas noktasından tork aktarılır. Yapay hız yönlendirmesi ve sabit duba devrilme oranı kaldırılmıştır.
- Daha yüksek güç daha fazla ilk momentum verir; aynı hedefte bile farklı yükseklik ve temas açısı farklı sonuç verebilir. Güce göre önceden belirlenmiş 1/2/3/4 duba sayısı yoktur.
- Farklı renkteki duba sabit kalır; topu açıya göre sektirir ve kısa bir parıltıyla temas bildirir. Atış başına tek hamle; iptal/kısa dokunma ücretsiz; coin ve swap fiyatları/kalıcı kayıt aynıdır.
- Büyük alt top tepsisi kaldırılmıştır. Aktif top sahnenin önünde zemine oturur; sonraki üç top SWAP düğmesinin üstünde 1–2–3 sırasıyla üç parmak delikli bowling toplarıdır. Önizleme kuyruğun 1, 2 ve 3 indislerindeki renkleri kullanır; atış/swap sonrasında yenilenir. Yeterli hamle kalmadığında kullanılmayacak toplar gizlenir.
- Fare ve dokunma desteklenir. Esc / pointercancel / touchcancel / odak kaybı ile tutuş iptal edilir. Klavyede sol/sağ yönü, yukarı/aşağı gücü değiştirir; Enter fırlatır.

## Doğrulama

Güncel fizik ve oyun raporları: `collision-core-verification.json` (49/49), `collision-browser-verification.json` (66/66) ve `collision-unity-verification.txt` (36/36). Güncel Unity sıra/çekiş 33/33, temas 36/36 ve ekran 8/8 kontrolleri geçmiştir. Ayrıntılar `CONTACT_PHYSICS.md` dosyasındadır.

Zeminden başlayan atış masanın ön kenarını aşacak şekilde hesaplanır. HTML’de başlangıç merkezi zemin üstünde 0.40 birimdir (yarıçap 0.39); Unity’de aynı temas, çayır yüzeyinin 0.40 birim üstündedir. Tutuş sırasında da aynı yatay düzlem korunur.

Unity gerçek fizik sürümünde ilk darbede hareket edip ayakta kalan dubalar da sonraki toplardan yeniden darbe alabilir. Her top aynı dubaya enerjisini en fazla bir kez aktarır; renk filtresi korunur.
