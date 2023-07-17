using System;
using System.Collections.Generic;

namespace Earmark.Data.Navigation
{
    public class AccountGroup
    {
        public string Name { get; }

        public IEnumerable<int> AccountIds { get; }

        public AccountGroup(string name, IEnumerable<int> accountIds)
        {
            Name = name;
            AccountIds = accountIds;
        }
    }
}
