namespace ProductsShop
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    using Data;

    using Model;

    public class Application
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            Console.WriteLine("1 :ImportData(!!!! Do This Only The First Time You Start The Program !!!!)");
            Console.WriteLine("2 :QueryOneProductsInRange");
            Console.WriteLine("3 :QueryTwoSoldProducts");
            Console.WriteLine("4 :QueryThreeCategoriesByProductsCount");
            Console.WriteLine("5 :QueryFourUsersAndProducts");
            Console.WriteLine("6 :Exit");
            Console.Write("Enter Number: ");
            int num = int.Parse(Console.ReadLine());
            while (num != 6)
            {
                switch (num)
                {
                    case 1:
                        ImportUsers(context);
                        ImportProducts(context);
                        ImportCategories(context);
                        break;
                    case 2:
                        QueryOneProductsInRange(context);
                        break;
                    case 3:
                        QueryTwoSoldProducts(context);
                        break;
                    case 4:
                        QueryThreeCategoriesByProductsCount(context);
                        break;
                    case 5:
                        QueryFourUsersAndProducts(context);
                        break;
                }
                Console.Clear();
                Console.WriteLine("Success");
                Console.WriteLine("1 :ImportData(!!!! Do This Only The First Time You Start The Program !!!!)");
                Console.WriteLine("2 :QueryOneProductsInRange");
                Console.WriteLine("3 :QueryTwoSoldProducts");
                Console.WriteLine("4 :QueryThreeCategoriesByProductsCount");
                Console.WriteLine("5 :QueryFourUsersAndProducts");
                Console.WriteLine("6 :Exit");
                Console.Write("Enter Number: ");
                num = int.Parse(Console.ReadLine());
            }
        }

        private static void QueryFourUsersAndProducts(ProductShopContext context)
        {
            var users = context.Users.Where(u => u.ProductsSold.Count >= 1).OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProductsCount = u.ProductsSold.Count,
                    soldProducts = u.ProductsSold.Select(p => new
                    {
                        name = p.Name,
                        price = p.Price
                    })
                });

            XDocument usersDoc = new XDocument();
            XElement usersXml = new XElement("users");
            usersXml.SetAttributeValue("count", users.Count());

            foreach (var u in users)
            {
                XElement user = new XElement("user");
                user.SetAttributeValue("first-name", u.firstName);
                user.SetAttributeValue("last-name", u.lastName);
                user.SetAttributeValue("age", u.age);
                XElement soldProducts = new XElement("sold-products");
                soldProducts.SetAttributeValue("count", u.soldProductsCount);

                foreach (var p in u.soldProducts)
                {
                    XElement product = new XElement("product");
                    product.SetAttributeValue("name", p.name);
                    product.SetAttributeValue("price", p.price);

                    soldProducts.Add(product);
                }

                user.Add(soldProducts);
                usersXml.Add(user);
            }

            usersDoc.Add(usersXml);

            usersDoc.Save("../../users-and-products.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryThreeCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories.OrderBy(c => c.Products.Count)
                .Select(c => new
                {
                    name = c.Name,
                    productsCount = c.Products.Count,
                    averagePrice = c.Products.Average(p => p.Price),
                    totalRevenue = c.Products.Sum(p => p.Price)
                });

            XDocument categoriesDoc = new XDocument();
            XElement categoriesXml = new XElement("categories");

            foreach (var c in categories)
            {
                XElement category = new XElement("category");
                category.SetAttributeValue("name", c.name);
                XElement productsCount = new XElement("products-count");
                productsCount.Value = c.productsCount.ToString();
                XElement averagePrice = new XElement("average-price");
                averagePrice.Value = c.averagePrice.ToString();
                XElement totalPrice = new XElement("total-revenue");
                totalPrice.Value = c.totalRevenue.ToString();

                category.Add(productsCount);
                category.Add(averagePrice);
                category.Add(totalPrice);
                categoriesXml.Add(category);
            }

            categoriesDoc.Add(categoriesXml);

            categoriesDoc.Save("../../categories-by-products.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryTwoSoldProducts(ProductShopContext context)
        {
            var users = context.Users.Where(u => u.ProductsSold.Count() >= 1).OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToList()
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    productsSold = u.ProductsSold.Select(p => new
                    {
                        name = p.Name,
                        price = p.Price
                    })
                });

            XDocument usersDoc = new XDocument();
            XElement usersXml = new XElement("users");

            foreach (var u in users)
            {
                XElement user = new XElement("user");
                user.SetAttributeValue("first-name", u.firstName);
                user.SetAttributeValue("last-name", u.lastName);
                XElement soldProducts = new XElement("sold-products");
                foreach (var p in u.productsSold)
                {
                    XElement product = new XElement("product");
                    XElement name = new XElement("name");
                    name.Value = p.name;
                    XElement price = new XElement("price");
                    price.Value = p.price.ToString();

                    product.Add(name);
                    product.Add(price);
                    soldProducts.Add(product);
                }

                user.Add(soldProducts);
                usersXml.Add(user);
            }

            usersDoc.Add(usersXml);

            usersDoc.Save("../../users-sold-products.xml", SaveOptions.DisableFormatting);
        }

        private static void QueryOneProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Where(p => p.Price >= 1000 && p.Price <= 2000 && p.BuyerId != null)
                .OrderBy(p => p.Price).Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                });

            XDocument productsDoc = new XDocument();
            XElement productsXml = new XElement("products");

            foreach (var p in products)
            {
                XElement product = new XElement("product");
                product.SetAttributeValue("name", p.name);
                product.SetAttributeValue("price", p.price);
                product.SetAttributeValue("buyer", p.buyer);

                productsXml.Add(product);
            }

            productsDoc.Add(productsXml);

            productsDoc.Save("../../products-in-range.xml", SaveOptions.DisableFormatting);
        }

        private static void ImportCategories(ProductShopContext context)
        {
            XDocument categoriesXml = XDocument.Load(@"F:\SoftUni\Databases Advanced - Entity Framework\11.XML Processing\ProductsShop\Import\categories.xml");

            XElement categories = categoriesXml.Root;

            //< category >
            //< name > Weapons </ name >
            //</ category >

            int num1 = 1;
            int num2 = 18;
            foreach (var category in categories.Elements())
            {
                string name = category.Element("name").Value;

                var products = context.Products.Where(p => p.Id >= num1 && p.Id <= num2);
                Category c = new Category();
                c.Name = name;
                foreach (var p in products)
                {
                    c.Products.Add(p);
                }

                num1 += 18;
                num2 += 18;

                context.Categories.Add(c);
            }

            context.SaveChanges();
        }

        private static void ImportProducts(ProductShopContext context)
        {
            XDocument productsXml = XDocument.Load(@"F:\SoftUni\Databases Advanced - Entity Framework\11.XML Processing\ProductsShop\Import\products.xml");

            XElement products = productsXml.Root;
            //< product >
            //< name > Care One Hemorrhoidal</ name >
            //< price > 932.18 </ price >
            //</ product >

            int num = 0;
            int userCount = context.Users.Count();
            foreach (var product in products.Elements())
            {
                string name = product.Element("name").Value;
                decimal price = decimal.Parse(product.Element("price").Value);

                Product p = new Product();
                p.Name = name;
                p.Price = price;
                p.SelledId = (num % userCount) + 1;
                num++;
                if (num % 3 == 0)
                {
                    p.BuyerId = (num % userCount) + 1;
                }
                

                context.Products.Add(p);
            }

            context.SaveChanges();
        }

        private static void ImportUsers(ProductShopContext context)
        {
            XDocument usersXml = XDocument.Load(@"F:\SoftUni\Databases Advanced - Entity Framework\11.XML Processing\ProductsShop\Import\users.xml");

            XElement users = usersXml.Root;

            foreach (var user in users.Elements())
            {
                string firstName = user.Attribute("first-name")?.Value;
                string lastName = user.Attribute("last-name").Value;
                int age = 0;
                try
                {
                    age = int.Parse(user.Attribute("age").Value);

                }
                catch
                {
                    age = 0;
                }

                User u = new User();
                u.FirstName = firstName;
                u.LastName = lastName;
                if (age != 0)
                {
                    u.Age = age;
                }

                context.Users.Add(u);
            }

            context.SaveChanges();
        }
    }
}
