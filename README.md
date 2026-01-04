İmza Tabanlı Antivirüs Simülasyonu

## Proje Hakkında
Bu proje, siber güvenlik dersi kapsamında "Signature Based Detection" (İmza Tabanlı Tespit) mantığını simüle etmek amacıyla geliştirilmiş bir Windows Forms uygulamasıdır. Gerçek bir antivirüs gibi dosya içeriklerini tarar ve MD5 özetlerini (Hash) bilinen zararlı veritabanıyla karşılaştırır.

## Özellikler
* **MD5 Hash Hesaplama:** Seçilen dosyaların benzersiz parmak izini çıkarır.
* **Veritabanı Simülasyonu:** Bilinen zararlı imzaları bellekte tutar.
* **Karantina İşlemi:** Tespit edilen zararlı dosyaların uzantısını değiştirerek (.quarantine) etkisiz hale getirir.
* **Asenkron Tarama:** Arayüz donmadan (Thread/Task yapısı ile) tarama yapar.
* **Test Modu:** EICAR mantığıyla zararsız bir test virüsü oluşturup sistemin çalışmasını kanıtlar.

## Kullanılan Teknolojiler
* C# (.NET Framework)
* Windows Forms
* System.Security.Cryptography (MD5)
* System.IO (Dosya İşlemleri)
