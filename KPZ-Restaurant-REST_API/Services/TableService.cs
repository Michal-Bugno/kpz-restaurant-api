using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KPZ_Restaurant_REST_API.Models;
using KPZ_Restaurant_REST_API.Repositories;

namespace KPZ_Restaurant_REST_API.Services
{
    public class TableService : ITableService
    {
        private IOrdersRepository _ordersRepository;
        private ITablesRepository _tablesRepository;

        public TableService(ITablesRepository tablesRepository, IOrdersRepository ordersRepository)
        {
            _tablesRepository = tablesRepository;
            _ordersRepository = ordersRepository;
        }

        public async Task<IEnumerable<Table>> AddManyTables(List<Table> tables, int restaurantId)
        {
            var addedTables = new List<Table>();
            foreach (var table in tables)
            {
                var tableCorrect = await _tablesRepository.TableCorrect(table, restaurantId);
                if (tableCorrect)
                {
                    addedTables.Add(table);
                }
                else
                    return null;
            }

            await _tablesRepository.AddRange(addedTables);
            await _tablesRepository.SaveAsync();
            return addedTables;
        }

        public async Task<Table> AddNewTable(Table table, int restaurantId)
        {
            var tableCorrect = _tablesRepository.TableCorrect(table, restaurantId);
            var tableNotPresent = _tablesRepository.CheckIfTablePresent(table, restaurantId);

            if (!tableNotPresent.Result && tableCorrect.Result && table.Number > 0 && table.Seats > 0 && table.X >= 0 && table.Y >= 0)
            {
                await _tablesRepository.Add(table);
                await _tablesRepository.SaveAsync();
                return table;
            }
            else
                return null;

        }

        public async Task<IEnumerable<Table>> DeleteManyTables(List<Table> tables, int restaurantId)
        {
            var deletedTables = new List<Table>();
            foreach (var table in tables)
            {
                var tableInDatabase = await _tablesRepository.GetTableById(table.Id, restaurantId);
                if (tableInDatabase != null)
                {
                    await _tablesRepository.DeleteTableById(tableInDatabase.Id, restaurantId);
                    deletedTables.Add(tableInDatabase);
                }
                else
                    return null;
            }

            await _tablesRepository.SaveAsync();
            return deletedTables;
        }

        public async Task<IEnumerable<Table>> GetAllTablesByRoomId(int roomId, int restaurantId)
        {
            return await _tablesRepository.GetTablesByRoomId(roomId, restaurantId);
        }

        public async Task<Table> GetTableById(int id, int restaurantId)
        {
            return await _tablesRepository.GetTableById(id, restaurantId);
        }

        public async Task<IEnumerable<Table>> GetTablesFilterd(int restaurantId)
        {
            return await _tablesRepository.GetAllTablesFilteredBySeatsCount(restaurantId);
        }

        public async Task<Table> RemoveTableById(int id, int restaurantId)
        {
            var removedTable = await _tablesRepository.DeleteTableById(id, restaurantId);
            if (removedTable != null)
            {
                await _tablesRepository.SaveAsync();
                return removedTable;
            }
            else
                return null;
        }

        public async Task<IEnumerable<Table>> GetTablesWithReadyProducts(int restaurantId)
        {
            var ordersInProgress = await _ordersRepository.GetOrdersInProgress(restaurantId);
            var tables = new List<Table>();

            foreach(var order in ordersInProgress)
            {
                if (order.OrderedProducts.Any(o => o.Status.ToUpper() == "READY"))
                {
                    if(!tables.Any(t => t.Id == order.TableId))
                        tables.Add(await _tablesRepository.GetTableById(order.TableId, restaurantId));
                }
            }

            return tables;
        }

        public async Task<IEnumerable<Table>> UpdateManyTables(List<Table> tables, int restaurantId)
        {
            var addedTables = new List<Table>();
            foreach (var table in tables)
            {
                var tableCorrect = await _tablesRepository.TableCorrect(table, restaurantId);
                if (tableCorrect)
                {
                    var tableInDatabase = await _tablesRepository.GetTableById(table.Id, restaurantId);
                    if (tableInDatabase != null)
                    {
                        tableInDatabase.Number = table.Number;
                        tableInDatabase.RoomId = table.RoomId;
                        tableInDatabase.Seats = table.Seats;
                        tableInDatabase.Status = table.Status;
                        tableInDatabase.X = table.X;
                        tableInDatabase.Y = table.Y;

                        _tablesRepository.Update(tableInDatabase);
                        addedTables.Add(tableInDatabase);
                    }
                    else
                        return null;
                }
                else
                    return null;
            }

            await _tablesRepository.SaveAsync();
            return addedTables;
        }

        public async Task<Table> UpdateTable(int id, Table table, int restaurantId)
        {
            var entity = await _tablesRepository.GetTableById(id, restaurantId);

            if (entity != null)
            {
                entity.Number = table.Number;
                entity.RoomId = table.RoomId;
                entity.Seats = table.Seats;
                entity.Status = table.Status;
                entity.X = table.X;
                entity.Y = table.Y;

                _tablesRepository.Update(entity);
                await _tablesRepository.SaveAsync();
                return entity;
            }
            else
                return null;

        }

    }
}