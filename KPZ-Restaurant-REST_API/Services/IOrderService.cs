using System.Collections.Generic;
using System.Threading.Tasks;
using KPZ_Restaurant_REST_API.Models;

namespace KPZ_Restaurant_REST_API.Services
{
    public interface IOrderService
    {
        Task<Order> CreateNewOrder(Order newOrder, int restaurantId);
        Task<IEnumerable<OrderedProducts>> AddOrderedProducts(List<OrderedProducts> orderedProducts, int restaurantId);
        Task<IEnumerable<Order>> GetAllOrders(int restaurantId);
        Task<IList<OrderedProducts>> GetOrderedProducts(int orderId, int restaurantId);
        Task<OrderedProducts> UpdateOrderedProduct(OrderedProducts orderedProduct, int restaurantId);
        Task<Order> UpdateOrder(Order order, int restaurantId);
        Task<IEnumerable<Order>> GetOrdersForTable(int tableId, int restaurantId);
        Task<Order> GetOrderById(int orderId, int restaurantId);
        Task<OrderedProducts> DeleteOrderedProduct(int orderedProductId, int restaurantId);
        Task<IEnumerable<OrderedProducts>> UpdateManyOrderedProducts(List<OrderedProducts> orderedProducts, int restaurantId);
        Task<IEnumerable<OrderedProducts>> GetServedProducts(int orderId, int restaurantId);
        Task<IEnumerable<Order>> GetOrdersByOrderDate(int year, int month, int day, int restaurantId);
        Task<IEnumerable<Order>> GetOrdersInProgress(int restaurantId);
        Task<Order> UpdateOrderStatus(int orderId, string status, int restaurantId);
        Task<IEnumerable<Order>> GetOrdersByOrderDateRange(DateRange dateRange, int restaurantId);
        Task<IEnumerable<Order>>  GetOrdersFromLastMonth(int restaurantId);
        Task<IEnumerable<Order>>  GetOrdersFromLastWeek(int restaurantId);

    }
}