# Üç top sırası ve baskılı dubalar

Sıradaki üç top SWAP üstünde, soldan sağa 1–2–3 olarak gösterilir. Aktif top önizlemeye dahil değildir: kaynak sıra indisleri 1, 2 ve 3'tür. Swap tüm sırayı döndürür; üç önizleme de güncellenir. Atış ve bölüm geçişinde aynı güncelleme uygulanır. Sonraki top için hamle kalmadığında ilgili yuva gizlenir; 3 hamle kaldığında 2, 2 hamlede 1, son hamlede 0 sonraki top görünür.

Duba silueti 96 profil halkası ve 64 dilimden oluşur. Dar ve düz taban, dolgun alt gövde, dar boyun ve yuvarlatılmış baş klasik bowling görünümü verir. Lastik taban yüzeyi altını kapatır. Cilalı kırık beyaz üst kaplama, renkli alt gövde ve iki boyun şeridi renk kuralının okunmasını korur.

Her renk için özgün bir gövde baskısı vardır: pembe tavşan, turkuaz penguen, mor kedi çizgi film karakterleri. Karakter resmi, PINPOP yazısı ve numara gövdenin eğrisine sarılır; ayrı bir ekran etiketi değildir. Temas parıltısı emisyonla yapılır ve asıl baskıyı değiştirmez.

Baskıların 512×1024 Canvas çizim kaynağı: `Assets/ArtSource/pin-skins.js`. Unity dosyaları: `Assets/Resources/PinSkins/pin-0.png`, `pin-1.png`, `pin-2.png`. HTML karakter PNG verilerini kendi içinde taşır ve aynı kaplama çizimine ekler. Ham resimler `Assets/Art/Characters/` klasöründedir; imagegen promptları `CHARACTER_ASSETS.md` içindedir. Kullanılan fizik motoru, renkli engel kuralı ve coin/swap fiyatları korunur.

Doğrulama: `pin-presentation-verification.json` 7/7; `collision-browser-verification.json` 66/66; `bumper-browser-verification.json` 7/7. Unity `pull-unity-verification.txt` 33/33, `collision-unity-verification.txt` 36/36, `visual-verification.txt` 8/8. Önceki ekonomi ve ekran akışı kontrolleri tarihsel raporlarda bulunur.
