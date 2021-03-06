using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CarShop.Data;     // teljes projektek hozzáadása a referenciákhoz
using CarShop.Logic;

namespace FF_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // add connection string
            // add entity fw

            CarDatabaseEntities db = new CarDatabaseEntities();
            Console.WriteLine(db.CAR.Count());

            //var q = from car in db.CAR
            //        group car by car.BRAND into grp
            //        select new
            //        {
            //            Brand = grp.Key.brand_name,
            //            AvgPrice = grp.Average(car => car.car_baseprice)
            //        };

            //foreach (var item in q) Console.WriteLine(item);


            // ----


            CarLogic cl = new CarLogic();
            Console.WriteLine("----");
            foreach (var item in cl.GetBrandAverages()) Console.WriteLine(item);

            Console.WriteLine("----");
            foreach (var item in cl.GetCarsWithLetter('a',false)) Console.WriteLine(item);

            cl.InsertCar(new CAR()
            {
                car_model = "Ford mustang",
                car_baseprice = 990,
                car_discountprice = 10,
                car_brand = 1,
                car_id = 5 // no identity ~ auto incr.
            });

            cl.DeleteCar(1);

            Console.WriteLine("----");
            foreach (var item in cl.GetAll())
                Console.WriteLine($"{item.car_model} {item.car_baseprice} {item.car_discountprice}");

            Console.WriteLine("----");
            Console.WriteLine(cl.GetCheapestCarModel());
        }
    }
}
