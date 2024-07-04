using System;
using System.Collections.Generic;

namespace onlineStore
{
    // Абстрактный класс для доставки
    public abstract class Delivery
    {
        public string Address;
    }

    // Класс для домашней доставки
    public class HomeDelivery : Delivery
    {
        public DateTime EstimatedDeliveryTime { get; set; }
        public string CourierName { get; set; }

        // Метод для расчета времени доставки
        public void CalculateDeliveryTime()
        {
            EstimatedDeliveryTime = DateTime.Now.AddHours(2); // Предположим, доставка занимает 2 часа
            Console.WriteLine($"{CourierName} доставит ваш заказ к {EstimatedDeliveryTime}.");
        }
    }

    // Класс для доставки в пункт выдачи
    public class PickPointDelivery : Delivery
    {
        public string PickPointId { get; set; }
        public string PickPointName { get; set; }
        public DateTime ClosingTime { get; set; }

        // Метод для расчета времени до закрытия пункта выдачи
        public void CalculateTimeUntilClosing()
        {
            TimeSpan timeUntilClosing = ClosingTime - DateTime.Now;
            if (timeUntilClosing.TotalMinutes > 0)
            {
                Console.WriteLine($"Пункт выдачи {PickPointName} (ID: {PickPointId}) закроется через {timeUntilClosing.Hours} часов и {timeUntilClosing.Minutes} минут.");
            }
            else
            {
                Console.WriteLine($"Пункт выдачи {PickPointName} (ID: {PickPointId}) уже закрыт.");
            }
        }
    }


    // Класс для доставки в магазин
    public class ShopDelivery : Delivery
    {
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public Dictionary<DayOfWeek, (TimeSpan open, TimeSpan close)> ShopWorkingHours { get; set; }

        // Метод для проверки, открыт ли магазин в данный момент
        public bool IsShopOpen()
        {
            DayOfWeek today = DateTime.Now.DayOfWeek;
            TimeSpan now = DateTime.Now.TimeOfDay;

            if (ShopWorkingHours.ContainsKey(today))
            {
                var hours = ShopWorkingHours[today];
                if (now >= hours.open && now <= hours.close)
                {
                    Console.WriteLine($"Магазин {ShopName} (ID: {ShopId}) открыт.");
                    return true;
                }
            }

            Console.WriteLine($"Магазин {ShopName} (ID: {ShopId}) закрыт.");
            return false;
        }
    }

    // Обобщенный класс для заказа
    public class Order<TDelivery> where TDelivery : Delivery
    {
        public TDelivery Delivery;

        public int Number;
        public string Description;

        public string CustomerName;
        public string CustomerPhone;

        public decimal TotalAmount;

        // Метод для вывода информации о заказе
        public void DisplayOrderDetails()
        {
            Console.WriteLine($"Order Number: {Number}");
            Console.WriteLine($"Description: {Description}");
            Console.WriteLine($"Customer: {CustomerName}, {CustomerPhone}");
            Console.WriteLine($"Total Amount: {TotalAmount}");

            // Дополнительная информация о доставке
            if (typeof(TDelivery) == typeof(HomeDelivery))
            {
                HomeDelivery homeDelivery = Delivery as HomeDelivery;
                Console.WriteLine($"Delivery Address: {homeDelivery.Address}");
                Console.WriteLine($"Estimated Delivery Time: {homeDelivery.EstimatedDeliveryTime}");
                Console.WriteLine($"Courier Name: {homeDelivery.CourierName}");
            }
            else if (typeof(TDelivery) == typeof(PickPointDelivery))
            {
                PickPointDelivery pickPointDelivery = Delivery as PickPointDelivery;
                Console.WriteLine($"Pick-up Address: {pickPointDelivery.Address}");
                Console.WriteLine($"Pick-up Point ID: {pickPointDelivery.PickPointId}");
                Console.WriteLine($"Pick-up Point Name: {pickPointDelivery.PickPointName}");
                Console.WriteLine($"Closing Time: {pickPointDelivery.ClosingTime}");
            }
            else if (typeof(TDelivery) == typeof(ShopDelivery))
            {
                ShopDelivery shopDelivery = Delivery as ShopDelivery;
                Console.WriteLine($"Shop ID: {shopDelivery.ShopId}");
                Console.WriteLine($"Shop Name: {shopDelivery.ShopName}");

                Console.WriteLine("Shop Working Hours:");
                foreach (var kvp in shopDelivery.ShopWorkingHours)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value.open} - {kvp.Value.close}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Home Delivery Example
            HomeDelivery homeDelivery = new HomeDelivery
            {
                Address = "123 Main St",
                EstimatedDeliveryTime = DateTime.Now.AddHours(3),
                CourierName = "John Doe"
            };

            Order<HomeDelivery> homeOrder = new Order<HomeDelivery>
            {
                Delivery = homeDelivery,
                Number = 1001,
                Description = "Home delivery order",
                CustomerName = "Alice",
                CustomerPhone = "555-1234",
                TotalAmount = 50.00m
            };

            homeOrder.DisplayOrderDetails();
            homeDelivery.CalculateDeliveryTime();

               PickPointDelivery pickPointDelivery = new PickPointDelivery
            {
                Address = "456 Elm St",
                PickPointId = "PP001",
                PickPointName = "Convenient Pickup",
                ClosingTime = DateTime.Today.AddHours(18).AddMinutes(30) 
            };

            Order<PickPointDelivery> pickPointOrder = new Order<PickPointDelivery>
            {
                Delivery = pickPointDelivery,
                Number = 2001,
                Description = "Pick-up point order",
                CustomerName = "Bob",
                CustomerPhone = "555-5678",
                TotalAmount = 70.00m
            };

            pickPointOrder.DisplayOrderDetails();
            pickPointDelivery.CalculateTimeUntilClosing();

            ShopDelivery shopDelivery = new ShopDelivery
            {
                Address = "789 Oak St",
                ShopId = "S001",
                ShopName = "Online Store",
                ShopWorkingHours = new Dictionary<DayOfWeek, (TimeSpan open, TimeSpan close)>
            {
                { DayOfWeek.Monday, (new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0)) },
                { DayOfWeek.Tuesday, (new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0)) },
                          }
            };

            Order<ShopDelivery> shopOrder = new Order<ShopDelivery>
            {
                Delivery = shopDelivery,
                Number = 3001,
                Description = "Shop delivery order",
                CustomerName = "Charlie",
                CustomerPhone = "555-9012",
                TotalAmount = 100.00m
            };

            shopOrder.DisplayOrderDetails();
            shopDelivery.IsShopOpen();

                 }
    }
}
