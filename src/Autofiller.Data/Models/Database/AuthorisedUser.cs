﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Models.Database
{
    public class AuthorisedUser : IDatabaseObject
    {

        public string UserName { get; set; }

        public string Table => "Steam_Accounts";

        public void Update(string value)
        {
            throw new NotImplementedException();
        }
    }
}