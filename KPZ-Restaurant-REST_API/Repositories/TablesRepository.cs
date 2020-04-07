﻿using KPZ_Restaurant_REST_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KPZ_Restaurant_REST_API.Repositories
{
    public class TablesRepository: RestaurantGeneric<Table>, ITablesRepository
    {
        private RestaurantContext _context;

        public TablesRepository(RestaurantContext context): base(context) 
        {
            _context = context;
        }
    }
}