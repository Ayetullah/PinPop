# Hafif oyun arayüzü

Bu revizyon oyun ekranındaki üst bilgi alanını ve alt kontrolleri sadeleştirir.

- Alt panel 200 yerine 150 tasarım birimi yüksekliğinde; krem zemin, ince altın kenar ve yumuşak gölge kullanır.
- Dört büyük yardımcı kartı, en az 44 ekran birimi dokunma alanı korunan yuvarlak ikonlarla değiştirildi. Stok adetleri küçük rozetlerdedir. Tam yardımcı adı ve açıklaması dokunulduğunda açılan pencerede görünür.
- SWAP tek belirgin alt eylemdir; hak sayısı butonun içindedir. Satın alma küçük `+` kontrolünden açılır. Sıradaki topların numaraları küçük rozetlerde, ilk top altın çerçeveyle gösterilir.
- Üst bölüm başlığının büyük arka planı kaldırıldı; hamle sayacı küçültüldü, ayarlar/mağaza kontrolleri krem yuvarlak düğmelere dönüştürüldü.
- Çekme sırasında alt açıklama boş kalır. Yüzde göstergesi ve nişan ucundaki MAKS etiketi tamamen kaldırıldı. Halkanın doluluğu, renk geçişi ve maksimum güç parlaması korunur.
- Atış hesabı, fizik, bölüm ve ödül kuralları değiştirilmedi.

## Kanıt

`verification.txt`, oyunun kendi kameralarıyla alınan PNG'ler ve `audit.json` bu klasördedir. 320×568, 390×693, 390×844, 430×932 oranları incelendi. Masa ön kenarı ile panel arasında en kısa testte 41 ekran pikseli boşluk ölçüldü. Dokunma alanları, %25/%60/%100 güçte yazıların yokluğu, güç halkası ve 1/2/3 yıldız durumları doğrulandı.

Bunlar masaüstünde oluşturulan uygulama render çıktılarıdır; fiziksel iPhone ekran görüntüsü değildir. iOS native derleme sonucu audit dosyasına kaydedilir. Cihaz bağlantısız olduğunda yeni sürümün telefona yüklendiği varsayılmaz.
