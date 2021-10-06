using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Database
{
    public class AuthorisedUser : IDatabaseObject
    {

        public string UserName { get; set; }

        public string Table => "Steam_Accounts";

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
