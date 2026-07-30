using AiCommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        var categoryNames = new Dictionary<string, string>
        {
            ["Telefon"] = "Akıllı telefonlar",
            ["Bilgisayar"] = "Dizüstü ve masaüstü bilgisayarlar",
            ["Kitap"] = "Roman, bilim kurgu ve diğer kitaplar",
            ["Giyim"] = "Giyim ürünleri",
            ["Spor"] = "Spor ürünleri ve ekipmanları"
        };

        var existingCategories =
            await context.Categories.ToListAsync();

        foreach (var categoryDefinition in categoryNames)
        {
            bool categoryExists = existingCategories.Any(category =>
                category.Name.Equals(
                    categoryDefinition.Key,
                    StringComparison.OrdinalIgnoreCase));

            if (!categoryExists)
            {
                var category = new Category
                {
                    Name = categoryDefinition.Key,
                    Description = categoryDefinition.Value
                };

                context.Categories.Add(category);
                existingCategories.Add(category);
            }
        }

        await context.SaveChangesAsync();

        int CategoryId(string categoryName)
        {
            return existingCategories
                .First(category =>
                    category.Name.Equals(
                        categoryName,
                        StringComparison.OrdinalIgnoreCase))
                .Id;
        }

        var products = new List<Product>
        {
            // Telefon

            CreateProduct(
                "Samsung Galaxy S25",
                "256 GB Android akıllı telefon",
                "Samsung",
                42999,
                20,
                CategoryId("Telefon")),

            CreateProduct(
                "Samsung Galaxy A56",
                "128 GB Android akıllı telefon",
                "Samsung",
                18999,
                35,
                CategoryId("Telefon")),

            CreateProduct(
                "Apple iPhone 16",
                "128 GB iOS akıllı telefon",
                "Apple",
                62999,
                12,
                CategoryId("Telefon")),

            CreateProduct(
                "Apple iPhone 15",
                "128 GB iOS akıllı telefon",
                "Apple",
                51999,
                0,
                CategoryId("Telefon")),

            CreateProduct(
                "Xiaomi Redmi Note 14",
                "256 GB Android akıllı telefon",
                "Xiaomi",
                14999,
                40,
                CategoryId("Telefon")),

            CreateProduct(
                "Google Pixel 9",
                "256 GB Android akıllı telefon",
                "Google",
                47999,
                8,
                CategoryId("Telefon")),

            // Bilgisayar

            CreateProduct(
                "MacBook Air M4",
                "16 GB RAM ve 256 GB SSD",
                "Apple",
                54999,
                10,
                CategoryId("Bilgisayar")),

            CreateProduct(
                "Lenovo ThinkPad E14",
                "16 GB RAM ve 512 GB SSD",
                "Lenovo",
                38999,
                15,
                CategoryId("Bilgisayar")),

            CreateProduct(
                "Asus ROG Strix G16",
                "Oyuncu dizüstü bilgisayarı",
                "Asus",
                79999,
                6,
                CategoryId("Bilgisayar")),

            CreateProduct(
                "HP Victus 15",
                "16 GB RAM oyuncu bilgisayarı",
                "HP",
                45999,
                11,
                CategoryId("Bilgisayar")),

            CreateProduct(
                "Dell Inspiron 15",
                "Günlük kullanım dizüstü bilgisayarı",
                "Dell",
                28999,
                22,
                CategoryId("Bilgisayar")),

            CreateProduct(
                "Acer Aspire 5",
                "16 GB RAM ve 512 GB SSD",
                "Acer",
                26999,
                0,
                CategoryId("Bilgisayar")),

            // Kitap

            CreateProduct(
                "Vakıf",
                "Isaac Asimov bilim kurgu romanı",
                "İthaki Yayınları",
                260,
                40,
                CategoryId("Kitap")),

            CreateProduct(
                "Ben Robot",
                "Isaac Asimov robot öyküleri",
                "İthaki Yayınları",
                220,
                30,
                CategoryId("Kitap")),

            CreateProduct(
                "Dune",
                "Frank Herbert bilim kurgu romanı",
                "İthaki Yayınları",
                350,
                25,
                CategoryId("Kitap")),

            CreateProduct(
                "1984",
                "George Orwell distopya romanı",
                "Can Yayınları",
                180,
                50,
                CategoryId("Kitap")),

            CreateProduct(
                "Simyacı",
                "Paulo Coelho romanı",
                "Can Yayınları",
                160,
                0,
                CategoryId("Kitap")),

            CreateProduct(
                "Suç ve Ceza",
                "Fyodor Dostoyevski romanı",
                "Türkiye İş Bankası Kültür Yayınları",
                210,
                18,
                CategoryId("Kitap")),

            // Giyim

            CreateProduct(
                "Nike Dri-FIT Tişört",
                "Erkek spor tişörtü",
                "Nike",
                1299,
                30,
                CategoryId("Giyim")),

            CreateProduct(
                "Adidas Essentials Sweatshirt",
                "Unisex günlük sweatshirt",
                "Adidas",
                2199,
                20,
                CategoryId("Giyim")),

            CreateProduct(
                "Levi's 501 Jean",
                "Klasik kesim jean pantolon",
                "Levi's",
                3299,
                14,
                CategoryId("Giyim")),

            CreateProduct(
                "Puma Spor Şort",
                "Erkek spor şortu",
                "Puma",
                999,
                32,
                CategoryId("Giyim")),

            CreateProduct(
                "Mavi Basic Tişört",
                "Pamuklu günlük tişört",
                "Mavi",
                699,
                45,
                CategoryId("Giyim")),

            CreateProduct(
                "Columbia Yağmurluk",
                "Su geçirmez outdoor yağmurluk",
                "Columbia",
                4999,
                0,
                CategoryId("Giyim")),

            // Spor

            CreateProduct(
                "Adidas Koşu Ayakkabısı",
                "Günlük koşu ayakkabısı",
                "Adidas",
                3499,
                18,
                CategoryId("Spor")),

            CreateProduct(
                "Nike Revolution Koşu Ayakkabısı",
                "Hafif koşu ayakkabısı",
                "Nike",
                3999,
                16,
                CategoryId("Spor")),

            CreateProduct(
                "Decathlon Yoga Matı",
                "Kaymaz egzersiz matı",
                "Decathlon",
                799,
                28,
                CategoryId("Spor")),

            CreateProduct(
                "Voit Dambıl Seti",
                "Ayarlanabilir dambıl seti",
                "Voit",
                2499,
                12,
                CategoryId("Spor")),

            CreateProduct(
                "Wilson Basketbol Topu",
                "Standart boy basketbol topu",
                "Wilson",
                1499,
                21,
                CategoryId("Spor")),

            CreateProduct(
                "Speedo Yüzücü Gözlüğü",
                "Buğu önleyici yüzücü gözlüğü",
                "Speedo",
                899,
                0,
                CategoryId("Spor"))
        };

        var existingProductNames = await context.Products
            .Select(product => product.Name)
            .ToListAsync();

        var existingProductNameSet = new HashSet<string>(
            existingProductNames,
            StringComparer.OrdinalIgnoreCase);

        var missingProducts = products
            .Where(product =>
                !existingProductNameSet.Contains(product.Name))
            .ToList();

        if (missingProducts.Count == 0)
        {
            return;
        }

        await context.Products.AddRangeAsync(missingProducts);
        await context.SaveChangesAsync();
    }

    private static Product CreateProduct(
        string name,
        string description,
        string brand,
        decimal price,
        int stock,
        int categoryId)
    {
        return new Product
        {
            Name = name,
            Description = description,
            Brand = brand,
            Price = price,
            Stock = stock,
            CategoryId = categoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}