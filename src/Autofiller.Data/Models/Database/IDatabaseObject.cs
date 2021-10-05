using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Models.Database
{
    public interface IDatabaseObject
    {
        public string Table { get; }

        public void Update();
    }
}
