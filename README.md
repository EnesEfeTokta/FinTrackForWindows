# 💰 FinTrack - Kişisel Finansal Takip Uygulaması

> Modern ve kullanıcı dostu kişisel finans yönetimi uygulaması. Gelir, gider ve yatırımlarınızı takip ederek finansal hedeflerinize ulaşmanıza yardımcı olur.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Status](https://img.shields.io/badge/status-Active-brightgreen.svg)

## 📋 İçindekiler

- [Genel Bakış](#genel-bakış)
- [Özellikler](#özellikler)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [Teknoloji Yığını](#teknoloji-yığını)
- [Ekran Görüntüleri](#ekran-görüntüleri)
- [API Dokümantasyonu](#api-dokümantasyonu)
- [Katkıda Bulunma](#katkıda-bulunma)
- [Lisans](#lisans)
- [İletişim](#iletişim)

## 🎯 Genel Bakış

FinTrack, günlük finansal işlemlerinizi kolayca takip edebileceğiniz, bütçe planlaması yapabileceğiniz ve finansal raporlar oluşturabileceğiniz kapsamlı bir masaüstü uygulamasıdır. C# ve .NET Framework kullanılarak geliştirilmiş olup, SQLite veritabanı ile yerel veri depolama sağlar.

### 🎨 Tasarım Felsefesi

- **Basitlik**: Karmaşık olmayan, sezgisel kullanıcı arayüzü
- **Güvenilirlik**: Verilerinizin güvenliği ve bütünlüğü
- **Performans**: Hızlı ve responsive kullanıcı deneyimi
- **Esneklik**: Farklı kullanım senaryolarına uyum sağlama

## ⚡ Özellikler

### 🏆 Ana Özellikler

- 💸 **Gelir/Gider Takibi**: Tüm finansal işlemlerinizi kategorilere ayırarak detaylı takip
- 📊 **Akıllı Raporlama**: Aylık, yıllık ve özel dönem raporları ile trend analizi
- 🎯 **Bütçe Yönetimi**: Kategoriler için bütçe limitleri belirleme ve otomatik uyarılar
- 📈 **Görsel Analiz**: Interactive grafikler ve dashboard ile harcama görselleştirme
- 🔒 **Güvenli Veri Saklama**: AES-256 şifreleme ile yerel SQLite veritabanı
- 🌙 **Tema Desteği**: Light/Dark mode ve özelleştirilebilir renkler
- 📱 **Responsive Tasarım**: Farklı ekran boyutlarına uyumlu arayüz

### 🛠️ Teknik Özellikler

- **.NET 6+** ile geliştirilmiş modern mimari
- **Entity Framework Core** ile güçlü ORM desteği
- **MVVM Pattern** ile temiz kod yapısı
- **WPF** ile zengin kullanıcı arayüzü
- **SQLite** ile hafif ve hızlı veritabanı
- **AutoMapper** ile nesne eşleme
- **FluentValidation** ile veri doğrulama
- **Serilog** ile kapsamlı loglama

### 👥 Hedef Kitle

- Kişisel finanslarını takip etmek isteyen bireyler
- Aile bütçesi yönetimi yapan kullanıcılar
- Freelancer ve küçük işletme sahipleri
- Finansal okuryazarlık geliştirmek isteyen öğrenciler

## 🚀 Kurulum

### Sistem Gereksinimleri

- **İşletim Sistemi**: Windows 10 veya üzeri (64-bit)
- **Framework**: .NET 6.0 Runtime veya üzeri
- **RAM**: Minimum 2GB (4GB önerilen)
- **Disk Alanı**: 100MB boş alan
- **Ekran Çözünürlüğü**: Minimum 1024x768

### Kurulum Adımları

#### 📦 Hazır Kurulum Dosyası

1. [Releases](https://github.com/username/FinTrack/releases) sayfasından en son sürümü indirin
2. `FinTrack-Setup.msi` dosyasını çalıştırın
3. Kurulum sihirbazını takip edin
4. İlk açılışta kullanıcı hesabı oluşturun

#### 🔧 Kaynak Koddan Derleme

```bash
# Projeyi klonlayın
git clone https://github.com/username/FinTrack.git
cd FinTrack

# Bağımlılıkları yükleyin
dotnet restore

# Projeyi derleyin
dotnet build --configuration Release

# Uygulamayı çalıştırın
dotnet run --project FinTrack.WPF
```

### 🔑 İlk Kurulum

1. **Master Password** oluşturun (en az 8 karakter, güçlü şifre)
2. **Varsayılan kategorileri** import edin veya kendiniz oluşturun
3. **Para birimi** ve **dil** ayarlarını yapın
4. **Otomatik yedekleme** ayarlarını aktifleştirin

## 📱 Kullanım

### Hızlı Başlangıç

#### 1️⃣ İlk İşlem Ekleme

```
Ana Ekran → Yeni İşlem → Gelir/Gider Seç → Tutar Girin → Kategori Seç → Kaydet
```

#### 2️⃣ Kategori Yönetimi

```
Ayarlar → Kategoriler → Yeni Kategori → İsim & Renk Seç → Kaydet
```

#### 3️⃣ Bütçe Belirleme

```
Bütçe → Yeni Bütçe → Kategori Seç → Limit Belirle → Dönem Seç → Kaydet
```

### Gelişmiş Özellikler

#### 📊 Rapor Oluşturma

- **Özet Raporu**: Genel finansal durum
- **Kategori Analizi**: Harcama dağılımı
- **Trend Raporu**: Zaman içindeki değişim
- **Karşılaştırma**: Dönemler arası analiz

#### 📈 Dashboard Özellikleri

- **Finansal Özet Kartları**: Toplam gelir, gider ve bakiye
- **Hızlı İşlem Butonları**: One-click işlem ekleme
- **Son İşlemler**: Chronological işlem listesi
- **Bütçe İndikątörleri**: Progress bar ile görsel takip
- **Grafik Alanı**: Interactive charts ve trendler

## 🏗️ Teknoloji Yığını

### Backend

- **Framework**: .NET 6.0
- **ORM**: Entity Framework Core 6
- **Veritabanı**: SQLite 3
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Mapping**: AutoMapper
- **Testing**: xUnit, Moq

### Frontend

- **UI Framework**: WPF (Windows Presentation Foundation)
- **MVVM**: CommunityToolkit.Mvvm
- **Charts**: LiveCharts2
- **Icons**: Material Design Icons
- **Styling**: Material Design in XAML

### DevOps & Tools

- **CI/CD**: GitHub Actions
- **Code Quality**: SonarQube
- **Package Manager**: NuGet
- **Documentation**: DocFX

## 📸 Ekran Görüntüleri

### Ana Dashboard
![Dashboard](docs/images/dashboard.png)
*Finansal özet ve hızlı erişim butonları*

### İşlem Yönetimi
![Transactions](docs/images/transactions.png)
*Detaylı işlem listesi ve filtreleme*

### Raporlama
![Reports](docs/images/reports.png)
*Interactive grafikler ve analiz*

### Bütçe Planlama
![Budget](docs/images/budget.png)
*Kategori bazlı bütçe takibi*

## 📚 API Dokümantasyonu

### Veritabanı Şeması

```sql
-- Kullanıcılar
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Kategoriler
CREATE TABLE Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Type TEXT NOT NULL CHECK(Type IN ('Income', 'Expense')),
    Color TEXT DEFAULT '#0078D4',
    Icon TEXT DEFAULT 'Money',
    UserId INTEGER,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- İşlemler
CREATE TABLE Transactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Amount DECIMAL(18,2) NOT NULL,
    Description TEXT,
    Date DATETIME NOT NULL,
    CategoryId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

### Service Layer

```csharp
public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllAsync(int userId);
    Task<Transaction> GetByIdAsync(int id);
    Task<Transaction> CreateAsync(CreateTransactionDto dto);
    Task<Transaction> UpdateAsync(UpdateTransactionDto dto);
    Task DeleteAsync(int id);
    Task<TransactionSummaryDto> GetSummaryAsync(int userId, DateTime? from, DateTime? to);
}
```

## 🛡️ Güvenlik

### Veri Koruması

- **Şifreleme**: AES-256 ile sensitive data encryption
- **Hashing**: BCrypt ile password hashing
- **Validation**: Input validation ve SQL injection koruması
- **Access Control**: Role-based yetkilendirme

### Gizlilik

- **Local Storage**: Tüm veriler yerel olarak saklanır
- **No Telemetry**: Kullanıcı verisi toplanmaz
- **GDPR Compliance**: Avrupa veri koruma uyumluluğu
- **Data Export**: Tam veri export/import imkanı

## 🧪 Test ve Kalite

### Test Coverage

- **Unit Tests**: %85+ kod coverage
- **Integration Tests**: Database ve service layer
- **UI Tests**: Automated UI testing
- **Performance Tests**: Load ve stress testing

### Code Quality

- **Static Analysis**: SonarQube ile otomatik analiz
- **Code Reviews**: Pull request review process
- **Coding Standards**: .NET coding conventions
- **Documentation**: XML comments ve wiki

## 📈 Performans

### Benchmarks

- **Startup Time**: < 3 saniye (SSD)
- **Transaction Insert**: < 100ms
- **Report Generation**: < 2 saniye (10K kayıt)
- **Memory Usage**: < 150MB
- **Database Size**: 1MB per 10K transactions

### Optimizasyon

- **Lazy Loading**: On-demand data loading
- **Caching**: Frequently used data caching
- **Pagination**: Large dataset handling
- **Background Tasks**: Non-blocking operations

## 🤝 Katkıda Bulunma

### Geliştirme Süreci

1. **Fork** edin repository'yi
2. **Feature branch** oluşturun (`git checkout -b feature/AmazingFeature`)
3. **Commit** edin değişikliklerinizi (`git commit -m 'Add some AmazingFeature'`)
4. **Push** edin branch'inizi (`git push origin feature/AmazingFeature`)
5. **Pull Request** açın

### Katkı Kuralları

- **Code Style**: .NET coding conventions
- **Testing**: Yeni kodlar için unit test yazın
- **Documentation**: Public API'ler için XML documentation
- **Commit Messages**: Conventional commits format

### Proje Yapısı

```
FinTrack/
├── src/
│   ├── FinTrack.Core/          # Domain models
│   ├── FinTrack.Infrastructure/ # Data access
│   ├── FinTrack.Application/   # Business logic
│   └── FinTrack.WPF/          # UI layer
├── tests/
│   ├── FinTrack.Core.Tests/
│   ├── FinTrack.Application.Tests/
│   └── FinTrack.Integration.Tests/
├── docs/                       # Documentation
└── scripts/                   # Build scripts
```

## 🆘 Destek ve SSS

### Sıkça Sorulan Sorular

**S: Verilerim nerede saklanıyor?**
A: Tüm veriler yerel SQLite veritabanında (`%APPDATA%\FinTrack\`) güvenle saklanır.

**S: Şifremi unuttum, ne yapmalıyım?**
A: Güvenlik nedeniyle şifre kurtarma yoktur. Yedekleme dosyanızdan geri yükleme yapabilirsiniz.

**S: Mobil uygulama var mı?**
A: Şu anda sadece Windows masaüstü versiyonu mevcut. Mobil versiyon roadmap'te.

### Sorun Bildirimi

Hata bulduğunuzda veya öneriniz olduğunda:

1. [GitHub Issues](https://github.com/username/FinTrack/issues) sayfasını ziyaret edin
2. Mevcut issue'ları kontrol edin
3. Yeni issue oluştururken template'i kullanın
4. Detaylı açıklama ve ekran görüntüsü ekleyin

## 🗺️ Roadmap

### v2.1 (Q3 2025)
- [ ] Export/Import functionality (CSV, Excel)
- [ ] Multi-currency support
- [ ] Recurring transactions
- [ ] Advanced filtering

### v2.2 (Q4 2025)
- [ ] Cloud sync (OneDrive, Google Drive)
- [ ] Mobile companion app
- [ ] API for third-party integrations
- [ ] Machine learning insights

### v3.0 (2026)
- [ ] Web version
- [ ] Real-time collaboration
- [ ] Banking integration
- [ ] Investment tracking

## 📄 Lisans

Bu proje [MIT License](LICENSE) altında lisanslanmıştır. Detaylar için LICENSE dosyasını inceleyiniz.

```
MIT License

Copyright (c) 2025 FinTrack Development Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```

## 👥 Geliştirici Ekibi

- **Project Lead**: [İsim](https://github.com/username)
- **Backend Developer**: [İsim](https://github.com/username)
- **UI/UX Designer**: [İsim](https://github.com/username)
- **QA Engineer**: [İsim](https://github.com/username)

## 📞 İletişim

- **GitHub**: [FinTrack Repository](https://github.com/username/FinTrack)
- **Email**: fintrack.support@example.com
- **Website**: [fintrack.app](https://fintrack.app)
- **Discord**: [FinTrack Community](https://discord.gg/fintrack)

---

<div align="center">

**⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın! ⭐**

Made with ❤️ by FinTrack Team

</div>
