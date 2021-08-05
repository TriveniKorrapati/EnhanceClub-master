using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Abstract
{
    public interface IMenuItemRepository
    {
        IEnumerable<MenuItem> MenuItems { get; }  
    }
}