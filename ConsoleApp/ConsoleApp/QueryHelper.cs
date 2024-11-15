using ConsoleApp.Model;
using ConsoleApp.Model.Enum;
using ConsoleApp.OutputTypes;

namespace ConsoleApp;

public class QueryHelper : IQueryHelper
{
    /// <summary>
    /// Get Deliveries that has payed
    /// </summary>
    public IEnumerable<Delivery> Paid(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Where(d => d.IsPaid); // Фільтруємо за умовою оплати
    }

    /// <summary>
    /// Get Deliveries that now processing by system (not Canceled or Done)
    /// </summary>
    public IEnumerable<Delivery> NotFinished(IEnumerable<Delivery> deliveries)
    {
        return deliveries.Where(d => d.Status != DeliveryStatus.Canceled && d.Status != DeliveryStatus.Done);
    }    
    
    /// <summary>
    /// Get DeliveriesShortInfo from deliveries of specified client
    /// </summary>
    public IEnumerable<DeliveryShortInfo> DeliveryInfosByClient(IEnumerable<Delivery> deliveries, string clientId)
    {
        return deliveries
            .Where(d => d.ClientId == clientId) // Фільтруємо за клієнтом
            .Select(d => new DeliveryShortInfo
            {
                Id = d.Id,
                Status = d.Status.ToString(),
                StartCity = d.StartCity,
                EndCity = d.EndCity
            });
    }
    
    /// <summary>
    /// Get first ten Deliveries that starts at specified city and have specified type
    /// </summary>
    public IEnumerable<Delivery> DeliveriesByCityAndType(IEnumerable<Delivery> deliveries, string cityName, DeliveryType type)
    {
        return deliveries
            .Where(d => d.StartCity == cityName && d.Type == type)
            .Take(10); // Повертаємо перші 10 записів
    }

    
    /// <summary>
    /// Order deliveries by status, then by start of loading period
    /// </summary>
    public IEnumerable<Delivery> OrderByStatusThenByStartLoading(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .OrderBy(d => d.Status) // Спочатку по статусу
            .ThenBy(d => d.StartTime); // Потім по часу початку завантаження
    }


    /// <summary>
    /// Count unique cargo types
    /// </summary>
    public int CountUniqCargoTypes(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .Select(d => d.CargoType) // Беремо тільки тип вантажу
            .Distinct() // Залишаємо тільки унікальні значення
            .Count(); // Підраховуємо їх кількість
    }

    
    /// <summary>
    /// Group deliveries by status and count deliveries in each group
    /// </summary>
    public Dictionary<DeliveryStatus, int> CountsByDeliveryStatus(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .GroupBy(d => d.Status) // Групуємо по статусу
            .ToDictionary(g => g.Key, g => g.Count()); // Повертаємо словник з кількістю доставок для кожного статусу
    }

    
    /// <summary>
    /// Group deliveries by start-end city pairs and calculate average gap between end of loading period and start of arrival period (calculate in minutes)
    /// </summary>
    public IEnumerable<AverageGapsInfo> AverageTravelTimePerDirection(IEnumerable<Delivery> deliveries)
    {
        return deliveries
            .GroupBy(d => new { d.StartCity, d.EndCity }) // Групуємо по містах
            .Select(g => new AverageGapsInfo
            {
                StartCity = g.Key.StartCity,
                EndCity = g.Key.EndCity,
                AverageGap = g.Average(d => (d.ArrivalTime - d.EndLoadingTime).TotalMinutes) // Обчислюємо середній проміжок часу
            });
    }


    /// <summary>
    /// Paging helper
    /// </summary>
    public IEnumerable<TElement> Paging<TElement, TOrderingKey>(IEnumerable<TElement> elements,
        Func<TElement, TOrderingKey> ordering,
        Func<TElement, bool>? filter = null,
        int countOnPage = 100,
        int pageNumber = 1)
    {
        var query = elements.AsQueryable();
        
        if (filter != null)
        {
            query = query.Where(filter); // Застосовуємо фільтр, якщо він є
        }
    
        return query
            .OrderBy(ordering) // Сортуємо по вказаному параметру
            .Skip((pageNumber - 1) * countOnPage) // Пропускаємо записи до поточної сторінки
            .Take(countOnPage); // Беремо необхідну кількість записів на сторінці
    }
