using Common.Domain.Abstractions;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Infrastructure.Persistence.Repositories.Base;

public class CategoriesRepository(ProductsDbContext dbContext) : 
    Repository<ProductsDbContext, Category, Guid>(dbContext),
    ICategoriesRepository
{
}

