# 🍼 IoT Tabanlı Akıllı Beşik Sistemi

Bu proje, bebek bakımında ebeveynlerin iş yükünü azaltmayı ve bebek konforunu otomatik olarak sağlamayı amaçlayan **IoT tabanlı akıllı bir beşik sistemi** geliştirmeyi hedeflemektedir.

---

## 📌 Proje Hakkında

Günümüzde bebek bakımı, ebeveynler için fiziksel ve zihinsel açıdan oldukça yorucu bir süreçtir. Mevcut ticari akıllı beşik sistemlerinin yüksek maliyetli olması ve sınırlı özelleştirme imkânı sunması, literatürdeki önemli eksikliklerden biridir.

Bu proje; **çoklu sensör entegrasyonu** ve **gerçek zamanlı çift yönlü veri iletişimi** yetenekleriyle literatürdeki benzer çalışmalardan ayrışmaktadır.

---

## ⚙️ Sistem Mimarisi

### Donanım

| Bileşen | Açıklama |
|--------|----------|
| Beşik Gövdesi | 3D baskı teknolojisiyle üretilmiştir |
| Sallama Mekanizması | Servo motor tabanlı |
| PIR Sensörü | Hareket algılama |
| Ses Sensörü | Ağlama tespiti |
| LDR Sensörü | Işık seviyesi ölçümü |
| Sıcaklık & Nem Sensörü | Ortam konforu izleme |
| Kırmızı LED | Gece aydınlatması |
| Havalandırma Fanı | Sıcaklık regülasyonu |
| Raspberry Pi 4 | Ana işlem birimi (SBC) |

### Yazılım & İletişim

- **Raspberry Pi 4** sensör verilerini işler; bebek ağladığında veya hareket ettiğinde beşik otomatik olarak sallanır.
- **C# tabanlı masaüstü arayüzü** ile Raspberry Pi arasında **TCP/IP socket programlama** kullanılarak çift yönlü gerçek zamanlı veri iletişimi sağlanır.
- Ağlama veya hareket algılandığında **Raspberry Pi kamerası** ile video kaydı alınır.
- **Telegram API** aracılığıyla ebeveynlere otomatik bildirim gönderilir.

---

## 🗓️ Proje Yönetimi

Proje, **4 aylık** süreçte beş ana aşamada tamamlanacaktır:

1. Literatür taraması ve sistem tasarımı
2. 3D beşik üretimi ve mekanik montaj
3. Donanım entegrasyonu ve testleri
4. Raspberry Pi ve C# yazılım geliştirme
5. Son sistem testleri ve raporlama

Her aşama için ölçülebilir başarı kriterleri belirlenmiş olup düzenli danışman toplantılarıyla ilerleme takibi yapılacaktır.

---

## 🌍 Yaygın Etki

- Proje sonunda **çalışır bir prototip** üretilecek ve **faydalı model başvurusu** yapılacaktır.
- Düşük maliyetli ve özelleştirilebilir yapısıyla bebek bakımında ebeveynlerin iş yükünü azaltarak sosyal fayda sağlayacaktır.
- **Açık kaynak** yaklaşımı benimsenerek eğitim amaçlı kullanıma sunulabilecektir.
- Akıllı ev sistemleri ve dijital sağlık uygulamaları alanlarında **yerli teknoloji geliştirme** çabalarına katkı sunacaktır.
