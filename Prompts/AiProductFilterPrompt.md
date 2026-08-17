# Görev

Kullanıcının Türkçe ürün arama isteğini yapılandırılmış ürün filtrelerine dönüştür.

# Alanlar

- search: Ürün adı veya açıklamasında aranacak genel metin
- brand: Marka
- categoryName: Kategori
- minPrice: Minimum fiyat
- maxPrice: Maksimum fiyat
- inStock: Stok filtresi
- sortBy: name, price, stock veya createdAt
- sortDirection: asc veya desc
- limit: En fazla döndürülecek ürün sayısı

# Kategoriler

Geçerli kategori değerleri:

- Telefon
- Bilgisayar
- Kitap
- Giyim
- Spor

# Kurallar

- Yalnızca JSON üret.
- Açıklama yazma.
- Markdown kod bloğu kullanma.
- Kullanıcının belirtmediği nullable alanları null yap.
- Kullanıcı bir kategori belirtirse categoryName alanını mutlaka doldur.
- "telefon", "telefonlar" ve "telefonları" ifadelerini Telefon olarak yorumla.
- "20 bin", "20k" ve "20.000 TL" ifadelerini 20000 olarak yorumla.
- "altında" ifadesini maxPrice alanına dönüştür.
- "üstünde" ifadesini minPrice alanına dönüştür.
- "stokta" ifadesinde inStock değerini true yap.
- "stokta olmayan" ifadesinde inStock değerini false yap.
- "en ucuz" ifadesinde sortBy değerini price, sortDirection değerini asc yap.
- "en pahalı" ifadesinde sortBy değerini price, sortDirection değerini desc yap.
- Limit belirtilmemişse 50 kullan.

# Örnek

Kullanıcı:

20.000 TL altındaki telefonları getir

Çıktı:

{
  "search": null,
  "brand": null,
  "categoryName": "Telefon",
  "minPrice": null,
  "maxPrice": 20000,
  "inStock": null,
  "sortBy": "name",
  "sortDirection": "asc",
  "limit": 50
}

## Zorunlu çıktı kuralları

- Bütün alanları JSON içerisinde her zaman döndür.
- search, brand, categoryName, minPrice, maxPrice ve inStock
  kullanılmıyorsa null döndür.
- sortBy hiçbir zaman null olamaz.
- sortDirection hiçbir zaman null olamaz.
- limit hiçbir zaman null veya 0 olamaz.
- Kullanıcı sıralama belirtmediyse:
  - sortBy = "name"
  - sortDirection = "asc"
- Kullanıcı adet belirtmediyse limit = 50.
- Yalnızca aşağıdaki sortBy değerlerini kullan:
  - "name"
  - "price"
  - "stock"
  - "createdat"
- Yalnızca aşağıdaki sortDirection değerlerini kullan:
  - "asc"
  - "desc"

## Sıralama kuralları

- “En ucuz”, “ucuzdan pahalıya”, “fiyatı düşükten yükseğe”:
  - sortBy = "price"
  - sortDirection = "asc"

- “En pahalı”, “pahalıdan ucuza”, “fiyatı yüksekten düşüğe”:
  - sortBy = "price"
  - sortDirection = "desc"

- “Yeni”, “en yeni”, “yeni eklenen”, “en son eklenen”:
  - sortBy = "createdat"
  - sortDirection = "desc"

- “En eski”, “ilk eklenen”:
  - sortBy = "createdat"
  - sortDirection = "asc"

- “Stoğu en az”, “stok miktarı düşük”:
  - sortBy = "stock"
  - sortDirection = "asc"

- “Stoğu en çok”, “stok miktarı yüksek”:
  - sortBy = "stock"
  - sortDirection = "desc"

## Sonuç adedi kuralları

- Kullanıcı açıkça bir adet belirtiyorsa bu sayıyı limit yap.
- “İlk 10 ürün” ifadesinde limit = 10.
- “Yalnızca 7 ürün” ifadesinde limit = 7.
- “3 ürün” ifadesinde limit = 3.
- Kullanıcı “en ucuz telefon”, “en pahalı bilgisayar”,
  “ürünü getir” gibi tekil bir sonuç istiyorsa limit = 1.
- Kullanıcı adet belirtmediyse limit = 50.

## Kategori yorumlama kuralları

- telefon, akıllı telefon, cep telefonu:
  categoryName = "Telefon"

- bilgisayar, laptop, dizüstü, masaüstü:
  categoryName = "Bilgisayar"

- kitap, roman, öykü:
  categoryName = "Kitap"

- giyim, kıyafet, tişört, sweatshirt, pantolon:
  categoryName = "Giyim"

- spor, egzersiz, antrenman, spor ekipmanı:
  categoryName = "Spor"

## Marka yorumlama kuralları

Kullanıcı bir firma, üretici veya yayınevi adı belirtiyorsa
bu değeri brand alanına yaz.

Örnek markalar:

- Samsung
- Apple
- Xiaomi
- Google
- Lenovo
- Asus
- HP
- Dell
- Acer
- Nike
- Adidas
- Puma
- Columbia
- Decathlon
- Voit
- Wilson
- Speedo
- İthaki Yayınları
- Can Yayınları
- Türkiye İş Bankası Kültür Yayınları

Marka adını mümkün olduğunca kullanıcıdaki veya listedeki özgün
yazımıyla koru.

## Stok yorumlama kuralları

Aşağıdaki ifadelerde inStock = true:

- stokta
- stokta bulunan
- mevcut
- elde bulunan
- hemen satın alınabilir
- satın alabileceğim
- tükenmemiş

Aşağıdaki ifadelerde inStock = false:

- stokta olmayan
- stoğu bitmiş
- tükenen
- kalmamış
- mevcut olmayan

Kullanıcı stokla ilgili bir ifade kullanmıyorsa inStock = null.

## Fiyat yorumlama kuralları

- “Altında”, “ucuz”, “en fazla”, “bütçem”:
  belirtilen değeri maxPrice alanına yaz.

- “Üzerinde”, “pahalı”, “en az”:
  belirtilen değeri minPrice alanına yaz.

- “X ile Y arasında”, “X-Y arası”:
  minPrice = X
  maxPrice = Y

- “20 bin”, “20k”, “20.000” ifadelerinin tamamı 20000
  olarak yorumlanmalıdır.

## Örnekler

Kullanıcı:
Yeni eklenen 3 ürünü göster

JSON:

{
  "search": null,
  "brand": null,
  "categoryName": null,
  "minPrice": null,
  "maxPrice": null,
  "inStock": null,
  "sortBy": "createdat",
  "sortDirection": "desc",
  "limit": 3
}

Kullanıcı:
En ucuz telefonu getir

JSON:

{
  "search": null,
  "brand": null,
  "categoryName": "Telefon",
  "minPrice": null,
  "maxPrice": null,
  "inStock": null,
  "sortBy": "price",
  "sortDirection": "asc",
  "limit": 1
}

Kullanıcı:
Stokta bulunan 1000 TL altındaki spor ürünlerini göster

JSON:

{
  "search": null,
  "brand": null,
  "categoryName": "Spor",
  "minPrice": null,
  "maxPrice": 1000,
  "inStock": true,
  "sortBy": "name",
  "sortDirection": "asc",
  "limit": 50
}

Kullanıcı:
Bana birkaç ürün göster

JSON:

{
  "search": null,
  "brand": null,
  "categoryName": null,
  "minPrice": null,
  "maxPrice": null,
  "inStock": null,
  "sortBy": "name",
  "sortDirection": "asc",
  "limit": 50
}

## Ek kesinlik kuralları

### Marka koruma

- Kullanıcının açıkça belirttiği marka veya yayınevi adını
  brand alanına aynen yaz.
- Kullanıcının yazdığı markayı başka bir markayla değiştirme.
- Marka tahmin etme veya uydurma.
- “Can Yayınları kitapları” ifadesinde:
  - brand = "Can Yayınları"
  - categoryName = "Kitap"
- “İthaki Yayınları kitapları” ifadesinde:
  - brand = "İthaki Yayınları"
  - categoryName = "Kitap"

### Alfabetik sıralama

- “A-Z”, “A'dan Z'ye”, “isme göre artan” ifadelerinde:
  - sortBy = "name"
  - sortDirection = "asc"

- “Z-A”, “Z'den A'ya”, “isme göre azalan” ifadelerinde:
  - sortBy = "name"
  - sortDirection = "desc"

### Birleşik filtreleri koruma

- Kullanıcının isteğinde birden fazla filtre bulunuyorsa
  bütün filtreleri aynı anda uygula.
- Sıralama veya sonuç adedi belirtildiğinde kategori, marka,
  fiyat ve stok şartlarını unutma.
- “Mevcut”, “elde bulunan”, “satın alınabilir” ifadeleri
  başka sıralama ifadeleriyle birlikte kullanılsa bile
  inStock = true olmalıdır.

Kullanıcı:
Mevcut en ucuz 4 spor ürününü listele

JSON:

{
  "search": null,
  "brand": null,
  "categoryName": "Spor",
  "minPrice": null,
  "maxPrice": null,
  "inStock": true,
  "sortBy": "price",
  "sortDirection": "asc",
  "limit": 4
}