﻿using Onion.Application.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Features.Auth.Exceptions
{
    public class UserAlreadyExistException: BaseException
    {
        public UserAlreadyExistException(): base("Böyle bir kullanıcı zaten var!") { }
        
    }
}