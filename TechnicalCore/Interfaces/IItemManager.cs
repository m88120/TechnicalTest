using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
    public interface IItemManager
    {
        List<ItemBucketVM> GetItems();
    }
}
