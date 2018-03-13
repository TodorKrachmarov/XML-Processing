namespace CarDealer.App
{
    using System;

    using Data;
    using Models;
    using System.Xml.Linq;
    using System.Linq;
    using System.Collections.Generic;

    public class Application
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            Console.WriteLine("1 :ImportData(!!!! Do This Only The First Time You Start The Program !!!!)");
            Console.WriteLine("2 :QueryOneCars");
            Console.WriteLine("3 :QueryTwoCarsFromMakeFerrari");
            Console.WriteLine("4 :QueryThreeLocalSuppliers");
            Console.WriteLine("5 :QueryFourCarsWithTheirListOfParts");
            Console.WriteLine("6 :QueryFiveTotalSalesByCustomer");
            Console.WriteLine("7 :QuerySixSalesWithAppliedDiscount");
            Console.WriteLine("8 :Exit");
            Console.Write("Enter Number: ");
            int num = int.Parse(Console.ReadLine());
            while (num != 8)
            {
                switch (num)
                {
                    case 1:
                        ImportSuppliers(context);
                        ImportParts(context);
                        ImportCars(context);
                        ImportCustomers(context);
                        ImportSales(context);
                        break;
                    case 2:
                        QueryOneCars(context);
                        break;
                    case 3:
                        QueryTwoCarsFromMakeFerrari(context);
                        break;
                    case 4:
                        QueryThreeLocalSuppliers(context);
                        break;
                    case 5:
                        QueryFourCarsWithTheirListOfParts(context);
                        break;
                    case 6:
                        QueryFiveTotalSalesByCustomer(context);
                        break;
                    case 7:
                        QuerySixSalesWithAppliedDiscount(context);
                        break;
                }
                Console.Clear();
                Console.WriteLine("Success");
                Console.WriteLine("1 :ImportData(!!!! Do This Only The First Time You Start The Program !!!!)");
                Console.WriteLine("2 :QueryOneCars");
                Console.WriteLine("3 :QueryTwoCarsFromMakeFerrari");
                Console.WriteLine("4 :QueryThreeLocalSuppliers");
                Console.WriteLine("5 :QueryFourCarsWithTheirListOfParts");
                Console.WriteLine("6 :QueryFiveTotalSalesByCustomer");
                Console.WriteLine("7 :QuerySixSalesWithAppliedDiscount");
                Console.WriteLine("8 :Exit");
                Console.Write("Enter Number: ");
                num = int.Parse(Console.ReadLine());
            }
        }

        private static void QuerySixSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales.Select(s => new
            {
                make = s.Car.Make,
                model = s.Car.Model,
                travel = s.Car.TravelledDistance,
                name = s.Customer.Name,
                discount =s.Discount,
                price = s.Car.Parts.Sum(p => p.Price),
                pricewithDis = s.Car.Parts.Sum(p => p.Price) / (1 + s.Discount)
            });

            XDocument salesDoc = new XDocument();
            XElement salesXml = new XElement("sales");

            foreach (var s in sales)
            {
                XElement sale = new XElement("sale");
                XElement car = new XElement("car");
                car.SetAttributeValue("make", s.make);
                car.SetAttributeValue("model", s.model);
                car.SetAttributeValue("travelled-distance", s.travel);
                XElement cusName = new XElement("customer-name");
                cusName.Value = s.name;
                XElement discount = new XElement("discount");
                discount.Value = s.discount.ToString();
                XElement price = new XElement("price");
                price.Value = s.price.ToString();
                XElement priceDis = new XElement("price-with-discount");
                priceDis.Value = s.pricewithDis.ToString();

                sale.Add(car);
                sale.Add(cusName);
                sale.Add(discount);
                sale.Add(price);
                sale.Add(priceDis);
                salesXml.Add(sale);
            }

            salesDoc.Add(salesXml);

            salesDoc.Save("../../sales-discounts.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryFiveTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers.Where(c => c.Sales.Count >= 1)
                .Select(c => new
                {
                    name = c.Name,
                    boughtCars = c.Sales.Count,
                    money = c.Sales.Sum(s => s.Car.Parts.Sum(p => p.Price))
                });

            XDocument cusDoc = new XDocument();
            XElement cusXml = new XElement("customers");

            foreach (var c in customers)
            {
                XElement cus = new XElement("customer");
                cus.SetAttributeValue("full-name", c.name);
                cus.SetAttributeValue("bought-cars", c.boughtCars);
                cus.SetAttributeValue("spent-money", c.money);

                cusXml.Add(cus);
            }

            cusDoc.Add(cusXml);

            cusDoc.Save("../../customers-total-sales.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryFourCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars.Select(c => new
            {
                make =c.Make,
                model = c.Model,
                travel = c.TravelledDistance,
                parts = c.Parts.Select(p => new
                {
                    name = p.Name,
                    price = p.Price
                })
            });

            XDocument carsDoc = new XDocument();
            XElement carsXml = new XElement("cars");

            foreach (var c in cars)
            {
                XElement car = new XElement("car");
                car.SetAttributeValue("make", c.make);
                car.SetAttributeValue("model", c.model);
                car.SetAttributeValue("travelled-distance", c.travel);
                XElement parts = new XElement("parts");
                foreach (var p in c.parts)
                {
                    XElement part = new XElement("part");
                    part.SetAttributeValue("name", p.name);
                    part.SetAttributeValue("price", p.price);
                    parts.Add(part);
                }

                car.Add(parts);
                carsXml.Add(car);
            }

            carsDoc.Add(carsXml);

            carsDoc.Save("../../cars-and-parts.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryThreeLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers.Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    partsCount = s.Parts.Count
                });

            XDocument supDoc = new XDocument();
            XElement supXml = new XElement("suppliers");

            foreach (var s in suppliers)
            {
                XElement sup = new XElement("supplier");
                sup.SetAttributeValue("id", s.id);
                sup.SetAttributeValue("name", s.name);
                sup.SetAttributeValue("parts-count", s.partsCount);

                supXml.Add(sup);
            }

            supDoc.Add(supXml);

            supDoc.Save("../../local-suppliers.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryTwoCarsFromMakeFerrari(CarDealerContext context)
        {
            var cars = context.Cars.Where(c => c.Make == "Ferrari").OrderBy(c => c.Model).ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    id = c.Id,
                    model = c.Model,
                    travel = c.TravelledDistance
                });

            XDocument carsDoc = new XDocument();
            XElement carsXml = new XElement("cars");

            foreach (var c in cars)
            {
                XElement car = new XElement("car");
                car.SetAttributeValue("id", c.id);
                car.SetAttributeValue("model", c.model);
                car.SetAttributeValue("travelled-distance", c.travel);

                carsXml.Add(car);
            }

            carsDoc.Add(carsXml);
            carsDoc.Save("../../ferrari-cars.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryOneCars(CarDealerContext context)
        {
            var cars = context.Cars.Where(c => c.TravelledDistance > 2000000).OrderBy(c => c.Make).ThenBy(c => c.Model)
                .Select(c => new
                {
                    make = c.Make,
                    model = c.Model,
                    travel = c.TravelledDistance
                });

            XDocument carsDoc = new XDocument();
            XElement carsXml = new XElement("cars");

            foreach (var c in cars)
            {
                XElement car = new XElement("car");
                XElement make = new XElement("make");
                make.Value = c.make;
                XElement model = new XElement("model");
                model.Value = c.model;
                XElement travel = new XElement("travelled-distance");
                travel.Value = c.travel.ToString();

                car.Add(make);
                car.Add(model);
                car.Add(travel);
                carsXml.Add(car);
            }

            carsDoc.Add(carsXml);

            carsDoc.Save("../../cars.xml", SaveOptions.DisableFormatting);
        }

        private static void ImportSales(CarDealerContext context)
        {
            int num = 0;
            int customersCount = context.Customers.Count();
            int carsCaunt = context.Cars.Count();
            for (int i = 1; i <= 100; i++)
            {
                Sale sale = new Sale();
                sale.CarId = (num % carsCaunt) + 1;
                sale.CustomerId = (num % customersCount) + 1;
                if (i % 2 == 0)
                {
                    sale.Discount = 0.2m;
                }
                else
                {
                    sale.Discount = 0.3m;
                }
                num++;

                context.Sales.Add(sale);
            }

            context.SaveChanges();
        }

        private static void ImportCustomers(CarDealerContext context)
        {
            XDocument customersXml = XDocument.Load(@"F:\SoftUni\Databases Advanced - Entity Framework\11.XML Processing\CarDealer.App\Import\customers.xml");

            XElement customers = customersXml.Root;

            //< customer name = "Emmitt Benally" >
            //< birth - date > 1993 - 11 - 20T00: 00:00 </ birth - date >
            //<is- young - driver > true </is- young - driver >
            //</ customer >

            foreach (var c in customers.Elements())
            {
                string name = c.Attribute("name").Value;
                DateTime date = DateTime.Parse(c.Element("birth-date").Value);
                bool isYoungDriver = bool.Parse(c.Element("is-young-driver").Value);

                Customer cus = new Customer();
                cus.Name = name;
                cus.BirthDate = date;
                cus.IsYoungDriver = isYoungDriver;

                context.Customers.Add(cus);
            }

            context.SaveChanges();
        }

        private static void ImportCars(CarDealerContext context)
        {
            XDocument carsXml = XDocument.Load(@"F:\SoftUni\Databases Advanced - Entity Framework\11.XML Processing\CarDealer.App\Import\cars.xml");

            XElement cars = carsXml.Root;

            //< car >
            //< make > Opel </ make >
            //< model > Omega </ model >
            //< travelled - distance > 2147483647 </ travelled - distance >
            //</ car >

            int num = 0;
            int partsCount = context.Parts.Count();
            foreach (var c in cars.Elements())
            {
                string make = c.Element("make").Value;
                string model = c.Element("model").Value;
                long travel = long.Parse(c.Element("travelled-distance").Value);

                List<Part> parts = new List<Part>();
                for (int i = 0; i < 20; i++)
                {
                    Part part = context.Parts.Find((num % partsCount) + 1);
                    parts.Add(part);
                    num++;
                }

                Car car = new Car();
                car.Make = make;
                car.Model = model;
                car.TravelledDistance = travel;
                car.Parts = parts;

                context.Cars.Add(car);
            }

            context.SaveChanges();
        }

        private static void ImportParts(CarDealerContext context)
        {
            XDocument partsXml = XDocument.Load(@"F:\SoftUni\Databases Advanced - Entity Framework\11.XML Processing\CarDealer.App\Import\parts.xml");

            XElement parts = partsXml.Root;

            //<part name="Bonnet/hood" price="1001.34" quantity="10" />
            
            int num = 0;
            int suppliersCount = context.Suppliers.Count();
            foreach (var p in parts.Elements())
            {
                string name = p.Attribute("name").Value;
                decimal price = decimal.Parse(p.Attribute("price").Value);
                int quantity = int.Parse(p.Attribute("quantity").Value);

                Part part = new Part();
                part.Name = name;
                part.Price = price;
                part.Quantity = quantity;
                part.SupplierId = (num % suppliersCount) + 1;
                num++;

                context.Parts.Add(part);
            }

            context.SaveChanges();
        }

        private static void ImportSuppliers(CarDealerContext context)
        {
            XDocument suppliersXml = XDocument.Load(@"F:\SoftUni\Databases Advanced - Entity Framework\11.XML Processing\CarDealer.App\Import\suppliers.xml");

            XElement suppliers = suppliersXml.Root;

            //<supplier name="3M Company" is-importer="true"/>
            foreach (var s in suppliers.Elements())
            {
                string name = s.Attribute("name").Value;
                bool isImporter = bool.Parse(s.Attribute("is-importer").Value);

                Supplier sup = new Supplier();
                sup.Name = name;
                sup.IsImporter = isImporter;

                context.Suppliers.Add(sup);
            }

            context.SaveChanges();
        }
    }
}
