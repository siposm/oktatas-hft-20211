﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dbase
{
    class Program
    {
        static void InsertToDb(EDDatabaseEntities db, EMP ember)
        {
            db.EMP.Add(ember);
            db.SaveChanges();
            Console.WriteLine("insert ok");
        }

        static void UpdateInDb(EDDatabaseEntities db, int empnumber)
        {
            EMP ember = db.EMP.Single(x => x.EMPNO == empnumber);
            ember.ENAME = "Hulk";
            db.SaveChanges();
            Console.WriteLine("update ok");
        }

        static void ListEmployees(EDDatabaseEntities db)
        {
            Console.WriteLine("\nEMPLOYEES:");
            foreach (var item in db.EMP)
            {
                Console.WriteLine("> "+item.ENAME + item.EMPNO);
            }
        }

        static void DeleteFromDb(EDDatabaseEntities db, int empnumber)
        {
            EMP ember = db.EMP.Single(x => x.EMPNO == empnumber);
            db.EMP.Remove(ember);
            db.SaveChanges();
            Console.WriteLine("remove ok");
        }

        static void SaveToXML(EDDatabaseEntities db)
        {
            XDocument kimenet = new XDocument();
            kimenet.Add(new XElement("emberek", new XAttribute("darabszam", db.EMP.Count())));

            foreach (var item in db.EMP)
            {
                kimenet.Root.Add(new XElement("nev", item.ENAME));
            }

            kimenet.Save("mentett_embernevek.xml");
        }

        static void Main(string[] args)
        {
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //
            // DB LÉTREHOZÁSÁNAK LÉPÉSEI
            //
            // 0.lépés: ha hibát dob a Service Based Database létrehozásakor, akkor fel kell telepíteni a VS_telepítőből a 'Data Storage and Processing' csoportot is
            //      ehhez a vs telepítőt futtasd le megint, klikk a 'Modify' gombra, és ott válaszd ki a fentebbi csomagot pluszban >> install
            //      restart vs és jó lesz, mehet a többi lépés:
            //
            // 1.lépés: jobb klikk projekt >> add new item >> service based database (ha kéri a VS, hogy legyen telepítve SQL valami, akkor telepítsük)
            //
            // 2.lépés: view felső menü >> server explorer >> megjelent a database >> jobb klikk new query >> futtassuk le az sql fájl tartalmát(kiinduló állapot létrehozása)
            //
            // 3.lépés: jobb klikk projekt >> add new item >> ado.net entity data model >> generate from database (első) >> _.mdf fájl kiválasztása
            //
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~






            EDDatabaseEntities db = new EDDatabaseEntities();
            Console.WriteLine("* connection OK *");

            DEPT reszl = db.DEPT.First();
            Console.WriteLine(reszl.DNAME);

            // ------------------------------------------------------------------

            Console.WriteLine("\nDEPARTMENTS:");
            foreach (var item in db.DEPT)
                Console.WriteLine("> " + item.DNAME);

            // ------------------------------------------------------------------

            // INSERT

            EMP lajos = new EMP()
            {
                ENAME = "Lomha Lajos", // 11 karakter >> validation error
                DEPTNO = 20,
                MGR = null,
                EMPNO = 1000,
                SAL = 9500
            };

            try
            {
                InsertToDb(db, lajos);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Console.WriteLine(ex.Message);
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }

            lajos.ENAME = "Lajos";
            InsertToDb(db, lajos);
            
            // UPDATE

            UpdateInDb(db, 1000);

            // LIST

            ListEmployees(db);

            // DELETE

            DeleteFromDb(db, 1000);

            // SELECT

            Console.WriteLine("\n\n\n");

            foreach (EMP dolgozo in db.EMP.Where(dolgozo => dolgozo.SAL >= 3000))
                Console.WriteLine("> " + dolgozo.ENAME);

            var dolgozok = from x in db.EMP
                           where x.SAL >= 3000
                           select x;

            Console.WriteLine();

            foreach (var item in dolgozok)
                Console.WriteLine("-->" + item.ENAME);


            Console.WriteLine("\n\n");

            // JOIN

            // linq összekötés

            var dolgozok2 = from dolgozo in db.EMP
                            join reszleg in db.DEPT
                            on dolgozo.DEPTNO equals reszleg.DEPTNO
                            select new
                            {
                                dolgozo.ENAME,
                                dolgozo.SAL,
                                dolgozo.COMM,
                                reszleg.DNAME // lásd megjegyzés lentebb
                            };
            
            // megjegyzés!
            // órán néztük: dolgozo.DEPT.DNAME volt a reszleg.DNAME helyére írva >> ez alapból is működik, nem kell join hozzá!!! >> LAZY LOADING
            //
            // pl.: a db.EMP.First().DEPT >> a teljes DEPT entitást visszaadja nekünk, amelyet ugye az EMP entitáson keresztül kérünk le
            //EMP emberEntitas = db.EMP.First();
            //Console.WriteLine(emberEntitas.DEPT.DNAME);


            foreach (var item in dolgozok2)
                Console.WriteLine(item.ENAME + "\t" + item.SAL + "\t" + item.COMM + "\t" + item.DNAME);


            Console.WriteLine("\n\n");

            // lazy loading

            var dolgozok3 = from dolgozo in db.EMP
                            orderby dolgozo.DEPTNO
                            select new
                            {
                                dolgozo.ENAME,
                                dolgozo.SAL,
                                dolgozo.COMM,
                                dolgozo.DEPT.DNAME // itt már lazy loading elvet használunk egyből
                            };

            foreach (var item in dolgozok3)
                Console.WriteLine(item.ENAME + "\t" + item.SAL + "\t" + item.COMM + "\t" + item.DNAME);




            // =================================================================================

            Console.WriteLine(); Console.WriteLine(); Console.WriteLine();

            // F1 - részlegenként a dolgozók átlagos jövedelme

            var f1 = from x in dolgozok2
                     group x by x.DNAME into g
                     select new
                     {
                         csoport = g.Key,
                         darab = g.Count(),
                         atlagKereset = g.Average(a => a.SAL),
                         atlagJuttatas = g.Average(a => a.COMM != null ? a.COMM : 0 ),
                         atlagJovdelem = g.Average(a => a.SAL) + g.Average(a => (a.COMM != null) ? a.COMM : 0 )
                     };

            foreach (var item in f1)
                Console.WriteLine(item);

            // F2 - legtöbb főt foglalkoztató cég minden adata

            // F3 - legkisebb összfizetésű munkakör (minden adata)

            // F4 - dolgozók akik az elnök felvétele utáni 30 napban érkeztek

            // F5 - módosítsunk minden dolgozót az adatbázisban akik a SALES osztályon dolgoznak
            //      emeljük meg a béren kívüli juttatásukat +500-zal és mentsük el a módosításokat

            // F6 - a RESEARCH osztályon dolgozó, a nevükben 'S' betűvel rendelkező dolgozókat
            //      fokozzuk le, azaz fizetésük felét kapják innentől kezdve csak

            // F7 - mentsük el .xml fájlba a neveket
            SaveToXML(db);

            // használjunk metódus helyett (megfelelő) delegáltat és adjunk hozzá pár extrát még:

            Func<decimal?, int> ValutaValto = x => (int) x * 334;

            Func<EDDatabaseEntities, XDocument> XML_Saver = (x =>
            {
                XDocument kimenet = new XDocument();
                kimenet.Add(new XElement("emberek", new XAttribute("darabszam", x.EMP.Count())));

                foreach (var item in x.EMP)
                {
                    kimenet.Root.Add(new XElement("ember",
                        new XAttribute("ID", item.EMPNO),
                        new XElement("nev", item.ENAME),
                        new XElement("munkakor", item.JOB),
                        new XElement("eltoltottMunkaido", DateTime.Now.Year - item.HIREDATE.Value.Year),

                        (item.COMM == null)
                            ?
                            new XElement("juttatas1", item.SAL, new XAttribute("valuta", "USD"))
                            :
                            new XElement("juttatas1", item.SAL + item.COMM, new XAttribute("valuta", "USD")),

                        (item.COMM == null)
                            ? 
                            new XElement("juttatas2", ValutaValto(item.SAL), new XAttribute("valuta","HUF"))
                            :
                            new XElement("juttatas2", ValutaValto(item.SAL + item.COMM), new XAttribute("valuta", "HUF"))
                        
                        ));
                }

                return kimenet;
            });

            XDocument file_to_save = XML_Saver(db);

            file_to_save.Save("delegalt_mentes.xml");

        }
    }
}
