﻿using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public interface IApplicationUser : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser applicationUser);
    }
}
