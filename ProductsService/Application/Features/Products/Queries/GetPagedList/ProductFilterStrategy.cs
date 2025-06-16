﻿using Common.Domain.Extensions;
using Common.Domain.Interfaces;
using ProductsService.Domain.Entities;

namespace ProductsService.Application.Features.Products.Queries.GetPagedList;

public class ProductFilterStrategy : IFilterStrategy<Product, ProductFilter>
{
    public IQueryable<Product> Filter(IQueryable<Product> query, ProductFilter filter)
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), p => 
                p.Name.StartsWith(filter.Name!))

            .WhereIf(filter.Price.HasValue, p => 
                p.Price >= filter.Price)

            .WhereIf(filter.IsAvailable.HasValue,p => 
                filter.IsAvailable!.Value
                    ? p.StockQuantity > 0
                    : p.StockQuantity == 0)

            .WhereIf(filter.Rating.HasValue, p => 
                p.AverageRating >= filter.Rating)

            .WhereIf(filter.Categories.Count != 0, p => 
                p.Categories.Any(c => filter.Categories.Contains(c.Id)))

            .WhereIf(filter.UserId != Guid.Empty && filter.ExcludeMyProducts, p => 
                p.UserId != filter.UserId);
    }
}
