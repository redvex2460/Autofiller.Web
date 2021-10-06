using System;
using System.Collections.Generic;
using System.Text;

namespace Autofiller.Data.Commands
{
    internal class InitialiseSteamCommand : ICommand<bool>
    {
        public bool Result { get; set; }

        public ICommand<bool> Execute()
        {
            throw new NotImplementedException();
        }
    }
}
