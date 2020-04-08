﻿using KPZ_Restaurant_REST_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPZ_Restaurant_REST_API.Repositories
{
    public interface IUsersRepository : IRestaurantGeneric<User>
    {
        bool CheckIfPresent(User user);

        List<User> GetAllByRights(int rights);
    }
}
