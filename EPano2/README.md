# ISUBU Bilgisayar Mühendisliği Dijital Bilgi Panosu

Bu proje, ISUBU Bilgisayar Mühendisliği Bölümü için tasarlanmış tam ekran dijital bilgi panosu sistemidir. ASP.NET MVC framework'ü kullanılarak geliştirilmiştir.

## 🎯 Proje Özellikleri

### Ana Panel Özellikleri
- **Tam Ekran Tasarım**: 1920x1080 (Full HD) çözünürlük için optimize edilmiş
- **ISUBU Kurumsal Tasarım**: Resmi ISUBU renk paleti ve görsel stilini kullanır
- **Responsive Layout**: Farklı ekran boyutlarına uyumlu
- **Otomatik Güncellemeler**: Gerçek zamanlı tarih/saat ve hava durumu
- **Video Rotasyonu**: Otomatik video geçişleri
- **Duyuru Carousel**: Otomatik kaydırmalı duyuru gösterimi
- **Kaydırmalı Metin**: Alt kısımda sürekli kaydırmalı duyuru başlıkları

### Admin Paneli
- **Dashboard**: Genel istatistikler ve sistem durumu
- **Video Yönetimi**: Video ekleme, düzenleme, silme
- **Duyuru Yönetimi**: Duyuru ekleme, düzenleme, silme
- **Modern Arayüz**: ISUBU teması ile tutarlı tasarım

## 🎨 Tasarım Özellikleri

### Renk Paleti
- **Ana Renk**: Navy Blue (#002147)
- **Vurgu Rengi**: Light Blue (#4A90E2)
- **Arka Plan**: Beyaz ve açık gri tonları
- **Metin**: Koyu navy başlıklar, orta gri alt başlıklar

### Tipografi
- **Font**: Poppins (Google Fonts)
- **Temiz ve Modern**: Büyük ekranlar için optimize edilmiş

## 🏗️ Proje Yapısı

```
EPano2/
├── Controllers/
│   ├── DashboardController.cs    # Ana panel kontrolcüsü
│   ├── AdminController.cs         # Admin panel kontrolcüsü
│   └── HomeController.cs          # Ana sayfa yönlendirmesi
├── Models/
│   ├── Video.cs                   # Video modeli
│   ├── Announcement.cs            # Duyuru modeli
│   ├── Weather.cs                 # Hava durumu modeli
│   └── DashboardViewModel.cs      # Ana panel view modeli
├── Views/
│   ├── Dashboard/
│   │   └── Index.cshtml           # Ana dijital panel
│   └── Admin/
│       ├── Dashboard.cshtml       # Admin dashboard
│       ├── Videos.cshtml          # Video yönetimi
│       └── Announcements.cshtml   # Duyuru yönetimi
└── wwwroot/
    ├── css/
    │   └── dashboard.css          # Ana stil dosyası
    └── js/
        └── dashboard.js           # JavaScript fonksiyonları
```

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler
- .NET 8.0 SDK
- Visual Studio 2022 veya VS Code
- Modern web tarayıcısı

### Adımlar
1. Projeyi klonlayın veya indirin
2. Terminal/Command Prompt'ta proje dizinine gidin
3. Bağımlılıkları geri yükleyin:
   ```bash
   dotnet restore
   ```
4. Projeyi derleyin:
   ```bash
   dotnet build
   ```
5. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```
6. Tarayıcınızda `https://localhost:5001` adresine gidin

## 📱 Kullanım

### Ana Panel
- Ana sayfa otomatik olarak dijital panoya yönlendirir
- Panel tam ekran modda çalışır
- Tüm içerik otomatik olarak güncellenir

### Admin Paneli
- `/Admin/Dashboard` - Genel bakış ve istatistikler
- `/Admin/Videos` - Video yönetimi
- `/Admin/Announcements` - Duyuru yönetimi

## 🔧 Teknik Detaylar

### Frontend Teknolojileri
- **HTML5**: Semantik yapı
- **CSS3**: Flexbox ve Grid layout
- **JavaScript**: ES6+ özellikleri
- **jQuery**: DOM manipülasyonu
- **Bootstrap**: Responsive framework

### Backend Teknolojileri
- **ASP.NET Core MVC**: Web framework
- **C# 12**: Programlama dili
- **Razor**: View engine

### Özellikler
- **Otomatik Video Geçişi**: 30 saniyede bir
- **Duyuru Carousel**: 8 saniyede bir
- **Gerçek Zamanlı Saat**: Her saniye güncellenir
- **Kaydırmalı Metin**: Sürekli animasyon
- **Responsive Design**: Mobil uyumlu

## 📊 Mock Data

Proje şu anda statik mock verilerle çalışır:

### Videolar
- Bilgisayar Mühendisliği Tanıtım
- Yazılım Geliştirme Süreçleri
- Veri Yapıları ve Algoritmalar

### Duyurular
- Final sınavları
- Yaz stajı başvuruları
- Bitirme projesi sunumları
- Laboratuvar açılışları

### Hava Durumu
- Isparta için mock veriler
- Otomatik güncelleme simülasyonu

## 🎯 Gelecek Geliştirmeler

- [ ] Veritabanı entegrasyonu (Entity Framework)
- [ ] Gerçek hava durumu API entegrasyonu
- [ ] Dosya yükleme sistemi
- [ ] Kullanıcı yetkilendirme sistemi
- [ ] API endpoints
- [ ] Logging ve monitoring
- [ ] Docker containerization

## 📝 Lisans

Bu proje ISUBU Bilgisayar Mühendisliği Bölümü için geliştirilmiştir.

## 👥 Katkıda Bulunanlar

- Proje geliştirici: AI Assistant
- Tasarım: ISUBU kurumsal kimlik rehberi
- Mock veriler: Bölüm ihtiyaçlarına göre hazırlanmıştır

## 📞 İletişim

Proje hakkında sorularınız için ISUBU Bilgisayar Mühendisliği Bölümü ile iletişime geçebilirsiniz.

---

**Not**: Bu proje şu anda mock verilerle çalışmaktadır. Gerçek veri entegrasyonu için ek geliştirme gereklidir.


