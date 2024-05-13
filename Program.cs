using System.Globalization;

namespace GiderTakip
{
    internal class Program
    {
        class Gider()
        {
            public string Ad { get; set; }
            public double Fiyat { get; set; }
            public DateTime Tarih { get; set; }
        }

        static List<Gider> giderler = new List<Gider>();

        static void TxtGiderKaydet()
        {
            using StreamWriter writer = new StreamWriter("gider.txt");

            string[] hesaplarTxt = new string[giderler.Count];

            for (int i = 0; i < giderler.Count; i++)
            {
                var hesapTxt = $"{giderler[i].Ad}|{giderler[i].Fiyat}|{giderler[i].Tarih}";
                hesaplarTxt[i] = hesapTxt;
            }

            writer.Write(string.Join('\n', hesaplarTxt));
        }

        static void GiderVerileriniOku()
        {
            using StreamReader reader = new StreamReader("gider.txt");

            string giderTxt = reader.ReadToEnd();

            if (giderTxt == "")
            {
                return;
            }
            string[] giderIcerikleri = giderTxt.Split('\n');
            Console.WriteLine(giderIcerikleri.Length);

            foreach (var giderIcerik in giderIcerikleri)
            {
                if (string.IsNullOrEmpty(giderIcerik))
                {
                    continue;
                }

                string[] expenseItemArr = giderIcerik.Split('|');
                Gider item = new Gider();
                item.Ad = expenseItemArr[0];
                item.Fiyat = int.Parse(expenseItemArr[1]);
                item.Tarih = DateTime.Parse(expenseItemArr[2]);
                giderler.Add(item);
            }
        }

        static void MenuGoster(bool ilkAcilisMi = false)
        {
            Console.Clear();

            if (ilkAcilisMi)
            {
                Console.WriteLine("Hoş Geldiniz!");
            }

            Console.WriteLine("Yapılacak İşlemi Seçin");
            Console.WriteLine("1. Gider ekle");
            Console.WriteLine("2. Detaylı Raporlar");
            //Console.WriteLine("3. Eski Tarihli Gider");
            //Console.WriteLine("4. Kategori Yönetimi");
            Console.WriteLine("3. Çıkış");
            Console.Write("Seçiminiz: ");
            char inputSecim = Console.ReadKey().KeyChar;

            switch (inputSecim)
            {
                case '1':
                    GiderEkle();
                    break;
                case '2':
                    GiderGoster();
                    break;
                case '3':
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Çıkış yapılıyor...");
                    Console.ResetColor();
                    Environment.Exit(0);
                    break;
                case '4':
                    
                    break;
                //case '5':
                //    Console.Clear();
                //    Console.ForegroundColor = ConsoleColor.DarkRed;
                //    Console.WriteLine("Çıkış yapılıyor...");
                //    Console.ResetColor();
                //    Environment.Exit(0);
                    //break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nHatalı Seçim Yaptınız!");
                    Console.ResetColor();
                    MenuyeDon();
                    break;
            }
        }

        static void MenuyeDon()
        {
            Console.WriteLine("\nMenüye dönmek için herhangi bir tuşa basınız.");
            Console.ReadKey(true);
            MenuGoster();
        }

        static void GiderEkle()
        {
            Console.Clear();
            Console.WriteLine("Eklemek istediğiniz giderin adını yazınız: ");
            string inputGiderAdı = Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"{inputGiderAdı} - maliyetini yazınız: ");
            double inputGiderMaliyeti = Convert.ToDouble(Console.ReadLine());
            Console.Clear();
            Console.WriteLine("Eklemek istediğiniz tarihi giriniz: (Gün/Ay/Yıl)");
            DateTime inputTarih = DateTime.Parse(Console.ReadLine());
            string degisenTarih = inputTarih.ToString("dd-MM-yyyy");
            giderler.Add(new Gider
            {
                Ad = inputGiderAdı,
                Fiyat = Convert.ToDouble(inputGiderMaliyeti),
                Tarih = DateTime.Parse(degisenTarih)
            });
            TxtGiderKaydet();
            MenuGoster();
        }

        static void GiderGoster()
        {
            double toplam = 0;
            Console.Clear();

            if (giderler.Count == 0)
            {
                Console.WriteLine("Listelenecek gider bulunamadı.");
            }

            //for (int i = 0; i < giderler.Count; i++)
            //{
            //    Console.WriteLine($"{i + 1} - {giderler[i].Ad} : {giderler[i].Fiyat} TL.");
            //    toplam += giderler[i].Fiyat;
            //}
            //Console.WriteLine($"\nToplam Gideriniz: {toplam} TL.");

            double aylikToplam = 0;
            var tümHesaplar = giderler.GroupBy(grup => grup.Tarih.Month);
            foreach (var grup in tümHesaplar)
            {
                CultureInfo turkceCeviri = new CultureInfo("tr-TR");
                double toplamHesap = grup.Sum(grup => grup.Fiyat);
                string ayIsmi = turkceCeviri.DateTimeFormat.GetMonthName(grup.Key);
                Console.WriteLine($"Ay: {ayIsmi}, Toplam tutar: {toplamHesap}");
                aylikToplam += toplamHesap;
            }

            Console.WriteLine($"Toplam Gideriniz: {aylikToplam} TL.");
            Console.WriteLine("Ayları filtrelemek için 1'e basınız");
            var inputSecim = Console.ReadLine();

            if (inputSecim == "1")
            {
                double total = 0;
                Console.Write("\nBakmak istediğiniz ayı sayıyla giriniz: ");
                int inputSecilenAy = int.Parse(Console.ReadLine());
                Console.Clear();
                CultureInfo turkceyeCevir = new CultureInfo("tr-TR");
                string ayAdi = turkceyeCevir.DateTimeFormat.GetMonthName(inputSecilenAy);
                Console.WriteLine(ayAdi);

                foreach (var hesap in giderler)
                {
                    if (hesap.Tarih.Month == inputSecilenAy)
                    {
                        Console.WriteLine($"{hesap.Tarih.ToString("dd.MM.yyyy")} - {hesap.Ad}: {hesap.Fiyat} TL.");
                        total += hesap.Fiyat;
                    }
                }
                if (total > 0)
                {
                    Console.WriteLine($"Toplam: {total} TL gideriniz bulunmaktadır.");
                }
                else if (total == 0)
                {
                    Console.WriteLine("Bu ayda bir gider bulunmamaktadır.");
                }

                MenuyeDon();
            }

            //static void EskiTarihliGiderGoster()
            //{

            //}

            //static void KategoriYonetimi()
            //{

            //}
        }

        static void Main(string[] args)
        {
            GiderVerileriniOku();
            MenuGoster(true);
        }
    }
}
