using Cascade.WebShop.Models;
using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cascade.WebShop.Services
{
    public interface IProductService : IDependency
    {
        string GetNextSku();
    }

    public class ProductService : IProductService
    {
        private readonly IRepository<ProductRecord> _repository;
        public ProductService(IRepository<ProductRecord> repo)
        {
            _repository = repo;
        }

        public string GetNextSku()
        {
            var product = _repository.Table.Where(p => p.Sku.ToUpper().StartsWith("SKU")).OrderByDescending(p => p.Sku).FirstOrDefault();

            if (product == null)
                return "SKU1001";

            var lastSku = product.Sku;
            var numerals = lastSku.Substring(3);
            var number = int.Parse(numerals);

            // search to make sure SKU doesn't already exist; limit number of iterations JIC.
            for (var count = 0; count < 100; ++count)
            {
                ++number;

                // make up a new sku, check that it doesn't already exist and return it
                var provisionalSku = "SKU" + number.ToString("0000");
                if (!_repository.Table.Any(p => p.Sku.ToUpper() == provisionalSku))
                    return provisionalSku;
            }
            return string.Empty;
        }

    }
}