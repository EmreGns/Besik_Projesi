# 🍼 IoT Tabanlı Akıllı Beşik Sistemi

Bu proje, bebek bakımında ebeveynlerin iş yükünü azaltmayı ve bebek konforunu otomatik olarak sağlamayı amaçlayan **IoT tabanlı akıllı bir beşik sistemi** geliştirmeyi hedeflemektedir.

---

## 📌 Proje Hakkında

Günümüzde bebek bakımı, ebeveynler için fiziksel ve zihinsel açıdan oldukça yorucu bir süreçtir. Mevcut ticari akıllı beşik sistemlerinin yüksek maliyetli olması ve sınırlı özelleştirme imkânı sunması, literatürdeki önemli eksikliklerden biridir.

Bu proje; **çoklu sensör entegrasyonu**, **gerçek zamanlı çift yönlü veri iletişimi** ve **hibrit kontrol arayüzleri (Masaüstü & Web)** yetenekleriyle literatürdeki benzer çalışmalardan ayrışmaktadır.

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

### Yazılım & Hibrit İletişim Altyapısı

- **Raspberry Pi 4:** Sensör verilerini anlık olarak işler; bebek ağladığında veya hareket ettiğinde beşik otomatik olarak sallanır. Kritik durumlarda **Raspberry Pi kamerası** ile otomatik video kaydı başlatılır.
- **C# Masaüstü Arayüzü:** Raspberry Pi ile **TCP/IP socket programlama** kullanılarak çift yönlü, düşük gecikmeli ve gerçek zamanlı veri iletişimi sağlar.
- **Web Arayüzü Entegrasyonu:** Sistemin web tabanlı izleme ve kontrol süreçleri için geliştirilen modern web arayüzü sayesinde, ebeveynler beşik durumunu herhangi bir tarayıcı üzerinden tamamen uzaktan ve platform bağımsız olarak izleyip kontrol edebilirler.
- **Telegram API:** Ağlama veya hareket algılandığında ebeveynlere anlık olarak görsel/metinsel push bildirimleri gönderilir.

---

## 🗓️ Proje Yönetimi

Proje, **4 aylık** süreçte beş ana aşamada tamamlanmıştır:

1. Literatür taraması ve sistem tasarımı
2. 3D beşik üretimi ve mekanik montaj
3. Donanım entegrasyonu ve testleri
4. Raspberry Pi, C# ve Web yazılımlarının geliştirilmesi
5. Son sistem entegrasyon testleri ve raporlama

Her aşama için ölçülebilir başarı kriterleri belirlenmiş olup düzenli danışman toplantılarıyla ilerleme takibi yapılmıştır.

---

## 🌍 Yaygın Etki

- Düşük maliyetli ve özelleştirilebilir yapısıyla bebek bakımında ebeveynlerin iş yükünü azaltarak sosyal fayda sağlayacaktır.
- **Açık kaynak** yaklaşımı benimsenerek eğitim amaçlı kullanıma sunulabilecektir.
- Akıllı ev sistemleri ve dijital sağlık uygulamaları alanlarında **yerli teknoloji geliştirme** çabalarına katkı sunacaktır.

---

## 📸 Proje Görselleri

### 🛠️ Donanım ve Prototip
Geliştirilen 3D mekanik gövde ve sensör optimizasyonları:
<img width="2048" height="1536" alt="final" src="https://github.com/user-attachments/assets/75f30468-fa15-4152-926a-053453b6262c" />

### 💻 C# Masaüstü Kontrol Paneli
TCP/IP üzerinden anlık veri takibi ve manuel kontrol arayüzü:
<img width="814" height="598" alt="c#" src="https://github.com/user-attachments/assets/ab3bee90-5f1e-4555-9c6f-9a4777d25368" />

### 🌐 Web Yönetim Paneli
Uzaktan erişim ve platform bağımsız izleme arayüzü:
<img width="1409" height="684" alt="Web Dashboard" src="https://github.com/user-attachments/assets/fdf5e062-a5e0-478a-b1f3-5706fc464454" />
