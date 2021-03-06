﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace xml
{
    class MainClass
    {

        public static XDocument Betoltes()
        {
            // XDocument nem felismerése esetén:
            // using system . xml . linq
            // mac esetén project > edit references > hozzáadás"

            // local megadás /bin/debug mappában található fájlra
            //return XDocument.Load("workers.xml");

            // egyből egy url is magadható neki
            // FF >> java szerver végpont
            return XDocument.Load("http://users.nik.uni-obuda.hu/siposm/db/workers.xml");
        }

        public static void Feldolgoz <T> (IEnumerable<T> bemenet)
        {
            Console.WriteLine();
            foreach (var item in bemenet)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }

        public static void Feldolgoz(XDocument doc)
        {
            var persons = from x in doc.Root.Descendants("person")
                          select x;

            foreach (var item in persons)
                Console.WriteLine(item);
        }

        public static void Main(string[] args)
        {

            XDocument allomany = Betoltes();

            // Feldolgoz(allomany);

            var dolgozokNevei = from x in allomany.Root.Descendants("person")
                           select x.Element("name").Value;

            Feldolgoz(dolgozokNevei);




            // =================================================================




            // 1. feladat:
            // kérdezzük le a tamásokat (figyelve kis és nagybetűkre)

            var tamasok = from x in allomany.Root.Descendants("person")
                          where x.Element("name").Value.ToUpper().Contains("tamás".ToUpper())
                          select x.Element("name").Value;

            Feldolgoz(tamasok);



            // =================================================================



            // 2. feladat:
            // kérjük le a polihisztorokat (email és név)

            var polihisztorok = from x in allomany.Root.Descendants("person")
                                where x.Element("rank").Value.Equals("polihisztor")
                                select new
                                {
                                    Nev = x.Element("name").Value,
                                    Mail = x.Element("email").Value
                                };

            Feldolgoz(polihisztorok);


            // =================================================================



            // 3. feladat:
            // számoljuk meg, hányan dolgoznak az egyes intézetekben

            var intezetek = from x in allomany.Root.Descendants("person")
                      group x by x.Element("dept").Value into g
                      select new
                      {
                          Letszam = g.Count(),
                          Intezet = g.Key
                      };

            Feldolgoz(intezetek);


            // =================================================================



            // 4. feladat
            // számoljuk meg (egy teljesen új lekéréssel) hogy csak az AII-ben hányan
            // HF: csináljuk meg ugyan ezt, de az eredmény a 3. feladat "intezetek" változójából nyerjük ki!

            var aii = from x in allomany.Root.Descendants("person")
                      where x.Element("dept").Value.Equals("Alkalmazott Informatikai Intézet")
                      group x by x.Element("dept").Value into g
                      select new
                      {
                          Letszam = g.Count(),
                          Intezet = g.Key
                      };

            Feldolgoz(aii);




            // =================================================================



            // 5. feladat:
            // adjunk hozzá az egész állományhoz egy új alkalmazottat


            XElement uj = new XElement("person",
                new XElement("name", "Ultron"),
                new XElement("email", "ultron@gmail.com"),
                new XElement("dept", "Alkalmazott Informatikai Intézet"),
                new XElement("rank", "pusztító"),
                new XElement("phone", "1234567890"),
                new XElement("room", "BA.6.66")
            );

            allomany.Root.Add(uj);

            // Feldolgoz(allomany);

            // mentés, ha akarjuk
            //allomany.Save("ujTetszolegesNev.xml");

        }
    }
}
