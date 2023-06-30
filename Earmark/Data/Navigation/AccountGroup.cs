using System;
using System.Collections.Generic;

namespace Earmark.Data.Navigation
{
    public class AccountGroup
    {
        public string Name { get; }

        public IEnumerable<Guid> AccountIds { get; }

        public AccountGroup(string name, IEnumerable<Guid> accountIds)
        {
            Name = name;
            AccountIds = accountIds;
        }
    }
}
