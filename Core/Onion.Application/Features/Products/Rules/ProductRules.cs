using Onion.Application.Bases;
using Onion.Application.Features.Products.Exceptions;
using Onion.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Features.Products.Rules
{
    public class ProductRules: BaseRules
    {
        public Task ProductTitleMustNotBeSame(IList<Product> products, string requestTitle)
        {
            if (products.Any(x => x.Title == requestTitle)) throw new ProductTitleMustNotBeSameException();
            return Task.CompletedTask;
        }
    }
}
