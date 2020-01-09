using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.Services
{
    public interface IProductService
    {
        public List<SelectListItem> GetProductsIEnumerable(Stations station);
        public List<ProductsTransactions> GetProductsTransactions(Stations station, Products product, DateTime from, DateTime to);
    }    
}
