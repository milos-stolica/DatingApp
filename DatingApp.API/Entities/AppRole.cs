﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatingApp.API.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> Users { get; set; }
    }
}
