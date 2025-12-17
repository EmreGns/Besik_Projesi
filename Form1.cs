using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace akillibesik3
{
    public partial class Form1 : Form
    {
        // --- RENK PALETİ ---
        private Color arkaPlanRengi = Color.FromArgb(236, 240, 241);
        private Color kartRengi = Color.White;
        private Color vurguRengi = Color.FromArgb(52, 152, 219);
        private Color butonRengi = Color.FromArgb(41, 128, 185);
        private Color yaziRengi = Color.FromArgb(44, 62, 80);
        private Color yesilRenk = Color.FromArgb(39, 174, 96);
        private Color kirmiziRenk = Color.FromArgb(231, 76, 60);
        private Color turuncuRenk = Color.FromArgb(243, 156, 18);

        // --- ARAYÜZ KONTROLLERİ ---
        private Label lblSicaklikDeger = null!;
        private Label lblNemDeger = null!;
        private Label lblParlaklikDeger = null!;
        private Label lblSesDurumu = null!;
        private Label lblHareketDurumu = null!;
        private Label lblDurumBildirim = null!;

        private TextBox txtIpAdresi = null!;
        private Label lblBaglantiDurumu = null!;
        private Button btnBaglan = null!;
        private Button btnOtomatik = null!;

        // --- AĞ VE SİSTEM DEĞİŞKENLERİ ---
        private TcpClient? client;
        private NetworkStream? stream;
        private Thread? dinlemeThread;
        private bool baglantiAcik = false;
        private bool bagliMi = false;

        private bool otomatikModAcik = false;
        private bool videoCekiliyorMu = false;
        private System.Windows.Forms.Timer videoZamanlayici;

        public Form1()
        {
            InitializeComponent();

            videoZamanlayici = new System.Windows.Forms.Timer();
            videoZamanlayici.Interval = 5000;
            videoZamanlayici.Tick += VideoIslemiBitti;

            ModernArayuzuKur();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void ModernArayuzuKur()
        {
            this.Text = "Akıllı Beşik - TÜBİTAK Projesi";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = arkaPlanRengi;
            this.Controls.Clear();

            // 1. BAĞLANTI PANELİ
            Panel pnlBaglanti = new Panel();
            pnlBaglanti.Location = new Point(20, 20);
            pnlBaglanti.Size = new Size(940, 70);
            pnlBaglanti.BackColor = Color.White;
            this.Controls.Add(pnlBaglanti);

            Label lblIp = new Label();
            lblIp.Text = "Raspberry Pi IP:";
            lblIp.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblIp.Location = new Point(20, 25);
            lblIp.AutoSize = true;
            pnlBaglanti.Controls.Add(lblIp);

            txtIpAdresi = new TextBox();
            txtIpAdresi.Text = "192.168.1.XX";
            txtIpAdresi.Font = new Font("Segoe UI", 11);
            txtIpAdresi.Location = new Point(210, 22);
            txtIpAdresi.Size = new Size(150, 30);
            pnlBaglanti.Controls.Add(txtIpAdresi);

            btnBaglan = new Button();
            btnBaglan.Text = "Bağlan";
            btnBaglan.Location = new Point(370, 20);
            btnBaglan.Size = new Size(100, 35);
            btnBaglan.FlatStyle = FlatStyle.Flat;
            btnBaglan.BackColor = butonRengi;
            btnBaglan.ForeColor = Color.White;
            btnBaglan.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnBaglan.Cursor = Cursors.Hand;
            btnBaglan.Click += BtnBaglan_Click;
            pnlBaglanti.Controls.Add(btnBaglan);

            lblBaglantiDurumu = new Label();
            lblBaglantiDurumu.Text = "• Bağlantı Yok";
            lblBaglantiDurumu.ForeColor = kirmiziRenk;
            lblBaglantiDurumu.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblBaglantiDurumu.Location = new Point(490, 25);
            lblBaglantiDurumu.AutoSize = true;
            pnlBaglanti.Controls.Add(lblBaglantiDurumu);

            // 2. DASHBOARD
            int kartY_Satir1 = 110;
            int kartY_Satir2 = 210;
            int kartGenislik = 180;
            int bosluk = 20;
            int solBaslangic = 180;

            lblSicaklikDeger = VeriKartiOlustur("Sıcaklık", "--°C", solBaslangic, kartY_Satir1, kartGenislik);
            lblNemDeger = VeriKartiOlustur("Nem", "--%", solBaslangic + (kartGenislik + bosluk), kartY_Satir1, kartGenislik);
            lblParlaklikDeger = VeriKartiOlustur("Işık Seviyesi", "--%", solBaslangic + (kartGenislik + bosluk) * 2, kartY_Satir1, kartGenislik);

            lblSesDurumu = VeriKartiOlustur("Ses Algılayıcı", "Sessiz", solBaslangic, kartY_Satir2, kartGenislik);
            lblHareketDurumu = VeriKartiOlustur("Hareket Sensörü", "Hareketsiz", solBaslangic + (kartGenislik + bosluk), kartY_Satir2, kartGenislik);

            // 3. OTOMATİK MOD BUTONU
            btnOtomatik = new Button();
            btnOtomatik.Text = "Otomatik Mod: KAPALI";
            btnOtomatik.Location = new Point(solBaslangic + (kartGenislik + bosluk) * 2, kartY_Satir2 + 20);
            btnOtomatik.Size = new Size(180, 50);
            btnOtomatik.FlatStyle = FlatStyle.Flat;
            btnOtomatik.FlatAppearance.BorderSize = 0;
            btnOtomatik.BackColor = Color.Gray;
            btnOtomatik.ForeColor = Color.White;
            btnOtomatik.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnOtomatik.Cursor = Cursors.Hand;
            btnOtomatik.Click += BtnOtomatik_Click;
            this.Controls.Add(btnOtomatik);

            // 4. KONTROL GRUPLARI
            int satir3Y = 340;
            int grupAralik = 300;
            // Burası artık hem başlat hem durdur butonu içeriyor
            GrupOlustur("Beşik Hızı", "Hız Seçiniz...", 50, satir3Y, "BESIK");
            GrupOlustur("Fan Hızı", "Fan Hızı Seçiniz...", 50 + grupAralik, satir3Y, "FAN");
            GrupOlustur("Parlaklık Ayarı", "Parlaklık Seçiniz...", 50 + (grupAralik * 2), satir3Y, "ISIK");

            // 5. BİLDİRİM ALANI
            lblDurumBildirim = new Label();
            lblDurumBildirim.Text = "Sistem Hazır. Otomatik Mod Bekleniyor...";
            lblDurumBildirim.Location = new Point(50, 510);
            lblDurumBildirim.Size = new Size(900, 40);
            lblDurumBildirim.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblDurumBildirim.ForeColor = yaziRengi;
            lblDurumBildirim.TextAlign = ContentAlignment.MiddleCenter;
            lblDurumBildirim.BackColor = Color.FromArgb(220, 230, 240);
            this.Controls.Add(lblDurumBildirim);

            // 6. KAMERA BUTONLARI
            int satir4Y = 600;
            int sagAltBaslangicX = 720;
            ButonOlustur("Fotoğraf Çek", sagAltBaslangicX - 20, satir4Y, 130, 45, "FOTO_CEK");
            ButonOlustur("Video Çek", sagAltBaslangicX + 120, satir4Y, 110, 45, "VIDEO_CEK");
        }

        // --- İLETİŞİM FONKSİYONLARI ---

        private void BtnBaglan_Click(object? sender, EventArgs e)
        {
            if (!bagliMi)
            {
                try
                {
                    string ip = txtIpAdresi.Text;
                    int port = 12345;

                    btnBaglan.Text = "...";
                    Application.DoEvents();

                    client = new TcpClient();
                    var result = client.BeginConnect(ip, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));

                    if (!success)
                    {
                        throw new Exception("Zaman aşımı! Raspberry Pi'ye ulaşılamadı.");
                    }
                    client.EndConnect(result);
                    stream = client.GetStream();

                    bagliMi = true;
                    baglantiAcik = true;

                    btnBaglan.Text = "Kes";
                    btnBaglan.BackColor = kirmiziRenk;
                    lblBaglantiDurumu.Text = "• Bağlantı Kuruldu";
                    lblBaglantiDurumu.ForeColor = yesilRenk;

                    dinlemeThread = new Thread(VeriDinle);
                    dinlemeThread.IsBackground = true;
                    dinlemeThread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlanamadı!\n" + ex.Message);
                    btnBaglan.Text = "Bağlan";
                    if (client != null) client.Close();
                }
            }
            else
            {
                BaglantiyiKes();
            }
        }

        private void BaglantiyiKes()
        {
            baglantiAcik = false;
            if (client != null) client.Close();
            bagliMi = false;

            if (InvokeRequired) { Invoke(new Action(BaglantiyiKes)); return; }

            btnBaglan.Text = "Bağlan";
            btnBaglan.BackColor = butonRengi;
            lblBaglantiDurumu.Text = "• Bağlantı Yok";
            lblBaglantiDurumu.ForeColor = kirmiziRenk;
            VerileriGuncelle("--", "--", "--", false, false);
        }

        private void VeriDinle()
        {
            if (stream == null) return;

            try
            {
                using (StreamReader okuyucu = new StreamReader(stream, Encoding.UTF8))
                {
                    while (baglantiAcik && bagliMi)
                    {
                        try
                        {
                            string? gelenSatir = okuyucu.ReadLine();

                            if (gelenSatir != null)
                            {
                                VeriyiAyiklaVeBas(gelenSatir);
                            }
                            else
                            {
                                throw new Exception("Sunucu bağlantıyı kesti.");
                            }
                        }
                        catch (IOException)
                        {
                            break;
                        }
                        catch (ObjectDisposedException)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (baglantiAcik) Invoke(new Action(BaglantiyiKes));
            }
        }

        private void VeriyiAyiklaVeBas(string hamVeri)
        {
            string[] parcalar = hamVeri.Trim().Split('|');

            if (parcalar.Length >= 5)
            {
                string t = parcalar[0];
                string h = parcalar[1];
                string l = parcalar[2];
                bool s = (parcalar[3].Trim() == "1");
                bool m = (parcalar[4].Trim() == "1");

                if (InvokeRequired)
                {
                    Invoke(new Action(() => VerileriGuncelle(t, h, l, s, m)));
                }
                else
                {
                    VerileriGuncelle(t, h, l, s, m);
                }
            }
        }

        private void KomutGonder(string komut)
        {
            if (bagliMi && stream != null)
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(komut);
                    stream.Write(data, 0, data.Length);
                }
                catch { }
            }
        }

        // --- GÖRSEL MANTIK ---

        public void VerileriGuncelle(string sicaklik, string nem, string isik, bool sesVarMi, bool hareketVarMi)
        {
            lblSicaklikDeger.Text = sicaklik + (sicaklik == "--" ? "" : "°C");
            lblNemDeger.Text = (nem == "--" ? "" : "%") + nem;
            lblParlaklikDeger.Text = (isik == "--" ? "" : "%") + isik;

            if (sesVarMi) { lblSesDurumu.Text = "Ağlıyor"; lblSesDurumu.ForeColor = kirmiziRenk; }
            else { lblSesDurumu.Text = "Sessiz"; lblSesDurumu.ForeColor = yesilRenk; }

            if (hareketVarMi) { lblHareketDurumu.Text = "Hareketli"; lblHareketDurumu.ForeColor = turuncuRenk; }
            else { lblHareketDurumu.Text = "Hareketsiz"; lblHareketDurumu.ForeColor = yaziRengi; }

            if (otomatikModAcik && !videoCekiliyorMu)
            {
                if (sesVarMi) BaslatOtomatikIslem("Bebek ağlıyor!");
                else if (hareketVarMi) BaslatOtomatikIslem("Bebek hareketli!");
            }
        }

        private void BaslatOtomatikIslem(string sebep)
        {
            videoCekiliyorMu = true;
            lblDurumBildirim.Text = $"⚠️ {sebep} Ebeveyne fotoğraf gönderildi. Video çekiliyor...";
            lblDurumBildirim.ForeColor = sebep.Contains("ağlıyor") ? kirmiziRenk : turuncuRenk;
            videoZamanlayici.Start();
        }

        private void VideoIslemiBitti(object? sender, EventArgs e)
        {
            videoZamanlayici.Stop();
            videoCekiliyorMu = false;
            lblDurumBildirim.Text = "✅ Ebeveyn fotoğraf ve video ile bilgilendirildi.";
            lblDurumBildirim.ForeColor = yesilRenk;
        }

        private void BtnOtomatik_Click(object? sender, EventArgs e)
        {
            otomatikModAcik = !otomatikModAcik;
            if (otomatikModAcik)
            {
                btnOtomatik.Text = "Otomatik Mod: AÇIK";
                btnOtomatik.BackColor = yesilRenk;
                lblDurumBildirim.Text = "Otomatik Mod Aktif. Sensörler izleniyor...";
                lblDurumBildirim.ForeColor = yesilRenk;
                KomutGonder("AUTO_ON");
            }
            else
            {
                btnOtomatik.Text = "Otomatik Mod: KAPALI";
                btnOtomatik.BackColor = Color.Gray;
                lblDurumBildirim.Text = "Otomatik Mod Kapalı.";
                lblDurumBildirim.ForeColor = yaziRengi;
                KomutGonder("AUTO_OFF");
            }
        }

        // --- YARDIMCI METODLAR VE DİNAMİK BUTON OLUŞTURMA (BURASI GÜNCELLENDİ) ---

        private void GrupOlustur(string baslik, string placeholder, int x, int y, string komutTipi)
        {
            Panel pnl = new Panel();
            pnl.Location = new Point(x, y);
            pnl.Size = new Size(250, 150);
            pnl.BackColor = Color.Transparent;
            this.Controls.Add(pnl);

            Label lbl = new Label();
            lbl.Text = baslik;
            lbl.Location = new Point(0, 0);
            lbl.Size = new Size(250, 30);
            lbl.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lbl.ForeColor = yaziRengi;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            pnl.Controls.Add(lbl);

            ComboBox cmb = new ComboBox();
            cmb.Text = placeholder;
            cmb.Location = new Point(25, 40);
            cmb.Size = new Size(200, 30);
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.Font = new Font("Segoe UI", 10);
            cmb.Items.AddRange(new string[] { "Düşük", "Orta", "Yüksek" });
            pnl.Controls.Add(cmb);

            // 1. BUTON: BAŞLAT/SEÇ (Mavi)
            Button btnSec = new Button();
            btnSec.Text = "Başlat";
            btnSec.Location = new Point(25, 80);
            btnSec.Size = new Size(95, 35);
            btnSec.FlatStyle = FlatStyle.Flat;
            btnSec.BackColor = butonRengi;
            btnSec.ForeColor = Color.White;
            btnSec.Font = new Font("Segoe UI", 10);
            btnSec.Cursor = Cursors.Hand;
            pnl.Controls.Add(btnSec);

            btnSec.Click += (s, e) =>
            {
                if (cmb.SelectedIndex != -1)
                {
                    // Düşük=1, Orta=2, Yüksek=3 gönderir
                    KomutGonder($"{komutTipi}_{cmb.SelectedIndex + 1}");
                }
            };

            // 2. BUTON: DURDUR (Kırmızı)
            Button btnDurdur = new Button();
            btnDurdur.Text = "Durdur";
            btnDurdur.Location = new Point(130, 80);
            btnDurdur.Size = new Size(95, 35);
            btnDurdur.FlatStyle = FlatStyle.Flat;
            btnDurdur.BackColor = kirmiziRenk;
            btnDurdur.ForeColor = Color.White;
            btnDurdur.Font = new Font("Segoe UI", 10);
            btnDurdur.Cursor = Cursors.Hand;
            pnl.Controls.Add(btnDurdur);

            // Bu buton tıklandığında _0 gönderir (BESIK_0, FAN_0 vb.)
            btnDurdur.Click += (s, e) =>
            {
                KomutGonder($"{komutTipi}_0");
            };
        }

        private Label VeriKartiOlustur(string baslik, string varsayilanDeger, int x, int y, int genislik)
        {
            Panel pnl = new Panel();
            pnl.Location = new Point(x, y);
            pnl.Size = new Size(genislik, 90);
            pnl.BackColor = kartRengi;

            Label lblBaslik = new Label();
            lblBaslik.Text = baslik;
            lblBaslik.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblBaslik.ForeColor = Color.Gray;
            lblBaslik.Location = new Point(15, 10);
            lblBaslik.AutoSize = true;
            pnl.Controls.Add(lblBaslik);

            Label lblDeger = new Label();
            lblDeger.Text = varsayilanDeger;
            lblDeger.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblDeger.ForeColor = vurguRengi;
            lblDeger.Location = new Point(15, 35);
            lblDeger.AutoSize = true;
            pnl.Controls.Add(lblDeger);

            this.Controls.Add(pnl);
            return lblDeger;
        }

        private Button ButonOlustur(string yazi, int x, int y, int genislik, int yukseklik, string komutKodu)
        {
            Button btn = new Button();
            btn.Text = yazi;
            btn.Location = new Point(x, y);
            btn.Size = new Size(genislik, yukseklik);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = butonRengi;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            this.Controls.Add(btn);
            btn.Click += (s, e) => KomutGonder(komutKodu);
            return btn;
        }

        // Tasarımcı (Designer) hatalarını önlemek için boş event'ler
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void button5_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
    }
}