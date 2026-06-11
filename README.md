# 🍼 IoT Tabanlı Akıllı Beşik Sistemi

[![TÜBİTAK 2209-A](https://img.shields.io/badge/T%C3%9CB%C4%B0TAK-2209--A%20%C4%B0ndikat%C3%B6r%C3%BC-blue)](https://www.tubitak.gov.tr/)
> **Bu proje, TÜBİTAK 2209-A Üniversite Öğrencileri Araştırma Projeleri Destekleme Programı kapsamında başvurusu gerçekleştirilmiş bir çalışmadır.**

Bu proje; bebek bakımında ebeveynlerin iş yükünü azaltmayı, bebek konforunu ve güvenliğini otomasyon sistemleriyle optimize etmeyi amaçlayan **IoT tabanlı akıllı bir beşik sistemidir**.

---

## 📌 Proje Hakkında ve Mantığı

Mevcut ticari akıllı beşik sistemlerinin yüksek maliyetli olması ve sınırlı özelleştirme imkânı sunması bu projenin çıkış noktasıdır. Sistem, çoklu sensör entegrasyonu sayesinde bebeğin durumunu anlık takip eder ve **hibrit kontrol arayüzleri (Masaüstü & Web)** üzerinden ebeveynlere çift yönlü gerçek zamanlı erişim sunar.

Sistem şu senaryolara göre otomatik çalışır:
- 🔊 **Ağlama ve Hareket Algılama:** Ses ve PIR sensörleri bebekte bir huzursuzluk (ağlama veya aşırı hareket) algıladığı an servo motor tabanlı sallama mekanizması otomatik devreye girer.
- 🌡️ **Sıcaklık ve Işık Regülasyonu:** Ortam sıcaklığı veya nemi konfor sınırından çıktığında havalandırma fanı; ortam karardığında ise bebeği ürkütmeyecek kırmızı gece LED'i otomatik tetiklenir.
- 📱 **Anlık Bildirim & Canlı Yayın:** Kritik durumlarda Raspberry Pi kamerası otomatik video kaydına başlar ve **Telegram API** üzerinden ebeveynin telefonuna anlık push bildirimi gönderilir.

---

## ⚙️ Sistem Mimarisi

### Donanım Bileşenleri

| Bileşen | Açıklama / Görevi |
|--------|----------|
| **Raspberry Pi 4** | Ana işlem birimi (SBC), tüm sensör ve motor lojiğini yönetir. |
| **Beşik Gövdesi** | Tamamen 3D baskı teknolojisiyle modüler üretilmiştir. |
| **Sallama Mekanizması** | Servo motor tabanlı mekanik tasarım. |
| **Sensör Grubu** | PIR (Hareket), Ses (Ağlama), LDR (Işık), DHT (Sıcaklık & Nem). |
| **Aktüatörler** | Havalandırma Fanı ve Gece Aydınlatma LED'i. |

### Yazılım & Hibrit İletişim Altyapısı

- **C# Masaüstü Arayüzü:** Raspberry Pi ile **TCP/IP socket programlama** altyapısını kullanır. Çok düşük gecikmeli, çift yönlü ve gerçek zamanlı lokal veri iletişimi ve manuel kontrol sağlar.
- **Web Yönetim Paneli:** Sistemin web mimarisine taşınmasıyla geliştirilen modern dashboard sayesinde, ebeveynler evin dışındayken de beşiğin durumunu herhangi bir internet tarayıcısı üzerinden platform bağımsız olarak izleyebilir ve kontrol edebilirler.

---

## 🌍 Yaygın Etki

- **Düşük Maliyet:** Ticari muadillerine kıyasla yüksek oranda ekonomik ve geliştirilebilir mimari.
- **Yerli ve Açık Kaynak:** Akıllı ev ve dijital sağlık teknolojilerinde yerli kod geliştirme vizyonu.

---

## 📸 Proje Görselleri

### 💻 Arayüz Tasarımları (Masaüstü & Web)
Sistemin hem lokal masaüstü (C#) hem de uzaktan izleme (Web Dashboard) ekranları:

| C# Masaüstü Kontrol Paneli | Web Yönetim Paneli |
|----------------------------|--------------------|
| <img width="400" alt="C# Masaüstü Arayüzü" src="https://github.com/user-attachments/assets/ab3bee90-5f1e-4555-9c6f-9a4777d25368" /> | <img width="400" alt="Web Dashboard" src="https://github.com/user-attachments/assets/fdf5e062-a5e0-478a-b1f3-5706fc464454" /> |

---

### 🛠️ Donanım Kurulumu ve Fiziksel Prototip
Geliştirilen 3D mekanik gövde, Raspberry Pi entegrasyonu ve sensör optimizasyonları:

<p align="center">
  <img width="820" alt="3D Beşik Donanım Prototipi" src="https://github.com/user-attachments/assets/75f30468-fa15-4152-926a-053453b6262c" />
</p>
