using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using TechnicalCore.Interfaces;

namespace TechnicalWeb.Controllers.ApiControllers
{
    [Produces("application/json")]
    [Route("api/Items")]
    public class ItemController : Controller
    {
        private IItemManager _itemManager;
        public ItemController(IItemManager itemManager)
        {
            _itemManager = itemManager;
        }

        [Route("GetItems")]
        [HttpGet]
        public IActionResult GetItems()
        {
            var itemList = _itemManager.GetItems();
            if (itemList != null)
            {
                return Ok(itemList);
            }
            else
            {
                return BadRequest();
            }

        }

    }
}
