using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalCore.Interfaces;
using TechnicalCore.Context;
using TechnicalCore.Models;

namespace TechnicalCore.Managers
{
    public class ItemManager : IItemManager
    {
        DbLeonContext _context;
        public ItemManager(DbLeonContext dbcontext)
        {
            _context = dbcontext;
        }
        public List<ItemBucketVM> GetItems()
        {
            try
            {
                var itemList = _context.Items.ToList();
                var itemBucketList = new List<ItemBucketVM>();
                foreach (var item in itemList)
                {
                    var obj = new ItemBucketVM
                    {
                        Id = item.ItemId,
                        Description = item.Description,
                        Title = item.ItemTitle,
                        Type = item.Type,
                        Priority = Convert.ToDouble(item.Priority),
                        Version = Convert.ToDouble(item.Version),
                        MockupLink = item.MockUpLink,
                        OrderInLane = Convert.ToDouble(item.OrderInLane)
                    };
                    if (item.Bucket != null)
                    {
                        obj.BucketDescription = item.Bucket.Description;
                        obj.OrderInVertical = Convert.ToDouble(item.Bucket.OrderInVertical);
                    }

                    itemBucketList.Add(obj);
                }

                return itemBucketList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}