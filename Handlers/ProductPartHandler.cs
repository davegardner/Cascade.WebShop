using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Cascade.WebShop.Models;
using System.Linq;

namespace Cascade.WebShop.Handlers
{
    public class ProductPartHandler : ContentHandler
    {
        private readonly IRepository<ProductRecord> _repository;

        public ProductPartHandler(IRepository<ProductRecord> repository)
        {
            _repository = repository;
            Filters.Add(StorageFilter.For(repository));
        }
    
    }
}