using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Abstract
{
    public interface IMenuTabRepository
    {
        IEnumerable<MenuTab> MenuTabs { get; }
    }
}
