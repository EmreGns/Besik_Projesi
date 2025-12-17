import socket
import threading
import RPi.GPIO as GPIO
import time
import board            # Pin yönetimi
import adafruit_dht     # DHT Sensör kütüphanesi
import os
import telepot          # Telegram kütüphanesi

# ==========================================
# 1. AYARLAR (Telegram Bilgilerin)
# ==========================================
HOST = '0.0.0.0'
PORT = 12345
TELEGRAM_TOKEN = '8265010120:AAGLJke-0xEvEkNuy23vX1UOfsSRJWLBWNc'
CHAT_ID = '1133253550'

# ==========================================
# 2. PIN TANIMLAMALARI (BCM Numaraları)
# ==========================================
# DHT22 -> GPIO 4 (board.D4)
PIN_SERVO = 18    # Servo
PIN_PIR = 17      # Hareket Sensörü
PIN_SES = 26      # Ses Sensörü
PIN_FAN = 5       # FAN (MOSFET)
PIN_LED = 6       # LED (MOSFET veya Transistör)
PIN_LDR = 13      # LDR (Dijital Okuma Yapacak)

# ==========================================
# 3. GLOBAL DEĞİŞKENLER
# ==========================================
try:
    dht_device = adafruit_dht.DHT22(board.D4)
except:
    dht_device = None

bot = telepot.Bot(TELEGRAM_TOKEN)
istemci_socket = None

otomatik_mod = False
besik_hizi = 0
fan_hizi = 0
medya_mesgul = False
son_bildirim_zamani = 0

# ==========================================
# 4. GPIO KURULUMU
# ==========================================
GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)

GPIO.setup(PIN_SERVO, GPIO.OUT)
GPIO.setup(PIN_LED, GPIO.OUT)
GPIO.setup(PIN_FAN, GPIO.OUT)
GPIO.setup(PIN_PIR, GPIO.IN)
GPIO.setup(PIN_SES, GPIO.IN)
GPIO.setup(PIN_LDR, GPIO.IN) # LDR Dijital Giriş

# PWM BAŞLATMA
servo = GPIO.PWM(PIN_SERVO, 50)  # 50Hz Servo
servo.start(0)

fan_pwm = GPIO.PWM(PIN_FAN, 100) # 100Hz Fan
fan_pwm.start(0)

# LED için de PWM yapalım ki parlaklık ayarı (Düşük/Orta/Yüksek) çalışsın
led_pwm = GPIO.PWM(PIN_LED, 100) 
led_pwm.start(0)

# ==========================================
# 5. FONKSİYONLAR
# ==========================================

def besik_salla():
    global besik_hizi
    while True:
        if otomatik_mod and besik_hizi > 0:
            duty = 2 + (besik_hizi * 2)
            servo.ChangeDutyCycle(duty)
            time.sleep(0.5)
            servo.ChangeDutyCycle(2)
            time.sleep(0.5)
        else:
            servo.ChangeDutyCycle(0)
            time.sleep(0.5)

def fan_kontrol(seviye):
    # Normal Mantık (Logic Level Converter ile çalışır)
    # Seviye 0: Dur
    # Seviye 1: %40 (Kalkış yapabilmesi için)
    # Seviye 2: %70
    # Seviye 3: %100
    
    duty = 0
    if seviye == 1: 
        duty = 40
    elif seviye == 2: 
        duty = 70
    elif seviye == 3: 
        duty = 100
    
    fan_pwm.ChangeDutyCycle(duty)
    print(f"-> Fan Seviyesi: {seviye} (Duty: {duty})")

def led_kontrol(seviye):
    """LED parlaklığını PWM ile kontrol eder"""
    duty = 0
    if seviye == 1: duty = 30
    elif seviye == 2: duty = 60
    elif seviye == 3: duty = 100
    led_pwm.ChangeDutyCycle(duty)

def foto_cek_ve_gonder(sebep):
    global medya_mesgul
    if medya_mesgul: return
    medya_mesgul = True 
    dosya_adi = "anlik_foto.jpg"
    print(f"KAMERA: {sebep} için FOTO çekiliyor...")
    try:
        # rpicam-still (Bookworm)
        komut = f"rpicam-still -o {dosya_adi} --width 640 --height 480 --immediate --nopreview --timeout 1"
        os.system(komut)
        bot.sendMessage(CHAT_ID, f"📸 {sebep}")
        with open(dosya_adi, 'rb') as f:
            bot.sendPhoto(CHAT_ID, f)
    except Exception as e:
        print(f"Hata: {e}")
    finally:
        if os.path.exists(dosya_adi): os.remove(dosya_adi)
        medya_mesgul = False 

def video_cek_ve_gonder(sebep):
    global medya_mesgul, son_bildirim_zamani
    if medya_mesgul: return
    medya_mesgul = True
    zaman = int(time.time())
    ham_dosya = f"vid_{zaman}.h264"
    mp4_dosya = f"vid_{zaman}.mp4"
    print(f"KAMERA: {sebep} için VIDEO çekiliyor...")
    try:
        bot.sendMessage(CHAT_ID, f"🎥 {sebep} (Video hazırlanıyor...)")
        # rpicam-vid (Bookworm)
        os.system(f"rpicam-vid -t 5000 -o {ham_dosya} --width 640 --height 480 --nopreview")
        os.system(f"ffmpeg -y -i {ham_dosya} -c:v copy {mp4_dosya} -loglevel quiet")
        with open(mp4_dosya, 'rb') as f:
            bot.sendVideo(CHAT_ID, f)
        son_bildirim_zamani = time.time()
    except Exception as e:
        print(f"Hata: {e}")
    finally:
        if os.path.exists(ham_dosya): os.remove(ham_dosya)
        if os.path.exists(mp4_dosya): os.remove(mp4_dosya)
        medya_mesgul = False

def veri_gonderimi():
    global istemci_socket, otomatik_mod, son_bildirim_zamani
    sayac = 0
    while True:
        try:
            ses = GPIO.input(PIN_SES)
            hareket = GPIO.input(PIN_PIR)
            
            # LDR OKUMA (Analog Pin Olmadığı için Dijital Okuyoruz)
            # Voltaj > 1.8V ise 1 (Aydınlık), değilse 0 (Karanlık)
            ldr_val = GPIO.input(PIN_LDR)
            
            # C# Arayüzü için sahte analog değer
            # Bağlantıya göre değişir: LDR VCC'de ise 1=Aydınlık
            if ldr_val == 1: str_isik = "90"
            else: str_isik = "10"

        except: 
            ses=0; hareket=0; str_isik="--"

        # Alarm Kontrolü (Otomatik Mod)
        if otomatik_mod and not medya_mesgul and (time.time() - son_bildirim_zamani > 30):
            if ses == 1:
                threading.Thread(target=video_cek_ve_gonder, args=("BEBEK AĞLIYOR!",)).start()
            elif hareket == 1:
                threading.Thread(target=video_cek_ve_gonder, args=("Hareket Algılandı",)).start()

        # Veri Gönderimi
        if sayac % 10 == 0:
            str_t = "--"; str_h = "--"
            if dht_device:
                try:
                    t = dht_device.temperature; h = dht_device.humidity
                    if t: str_t = f"{t:.1f}"; str_h = f"{h:.1f}"
                except: pass
            
            # Sicaklik|Nem|Isik|Ses|Hareket
            veri = f"{str_t}|{str_h}|{str_isik}|{ses}|{hareket}\n"
            
            if istemci_socket:
                try: istemci_socket.send(veri.encode('utf-8'))
                except: istemci_socket = None
        
        sayac += 1
        time.sleep(0.1)

def komut_dinle():
    global istemci_socket, otomatik_mod, besik_hizi
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server.bind((HOST, PORT))
    server.listen(1)
    print(f"Sunucu {PORT} portunda hazır.")

    while True:
        try:
            client, addr = server.accept()
            print(f"Bağlandı: {addr}")
            istemci_socket = client
            
            while True:
                try:
                    data = client.recv(1024).decode('utf-8').strip()
                    if not data: break
                    print(f"Komut: {data}")
                    
                    if data == "FOTO_CEK":
                        threading.Thread(target=foto_cek_ve_gonder, args=("Kullanıcı",)).start()
                    elif data == "VIDEO_CEK":
                        threading.Thread(target=video_cek_ve_gonder, args=("Kullanıcı",)).start()
                    elif data == "AUTO_ON": otomatik_mod = True
                    elif data == "AUTO_OFF": 
                        otomatik_mod = False
                        besik_hizi = 0
                    
                    # FAN KONTROL
                    elif "FAN_" in data:
                        try: fan_kontrol(int(data.split('_')[1]))
                        except: pass
                    
                    # LED/ISIK KONTROL
                    elif "ISIK_" in data:
                        try: led_kontrol(int(data.split('_')[1]))
                        except: pass

                    # BESIK KONTROL
                    elif "BESIK_" in data:
                        try: besik_hizi = int(data.split('_')[1])
                        except: pass
                
                except ConnectionResetError: break
            
            istemci_socket = None
            client.close()
        except Exception as e:
            print(f"Sunucu Hatası: {e}")

if __name__ == '__main__':
    try:
        t1 = threading.Thread(target=komut_dinle)
        t2 = threading.Thread(target=veri_gonderimi)
        t3 = threading.Thread(target=besik_salla)
        t1.start(); t2.start(); t3.start()
        t1.join(); t2.join(); t3.join()
    except KeyboardInterrupt:
        print("Kapatılıyor...")
        fan_pwm.stop()
        servo.stop()
        led_pwm.stop()
        GPIO.cleanup()
        if dht_device: dht_device.exit()
