﻿using KPZ_Restaurant_REST_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPZ_Restaurant_REST_API.Repositories
{
    public interface IRoomsRepository : IRestaurantGeneric<Room>
    {
        Task<bool> RoomCorrect(Room room);
        Task<IEnumerable<Room>> GetAllRooms(int restaurantId);
        Task<Room> DeleteRoomById(int roomId, int restaurantId);
    }
}
