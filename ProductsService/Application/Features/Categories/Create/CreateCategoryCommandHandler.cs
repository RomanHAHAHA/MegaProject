using Common.Domain.Interfaces;
using Common.Domain.Models;
using Common.Domain.Models.Results;
using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductsService.Application.Features.Categories.Common;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Categories.Create;

public class CreateCategoryCommandHandler(
    ICategoriesRepository categoriesRepository,
    IValidator<CategoryCreateDto> validator,
    CategoryFactory categoryFactory) : IRequestHandler<CreateCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        CreateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request.CategoryCreateDto);

        if (!validationResult.IsValid)
        {
            return BaseResponse.BadRequest(validationResult);
        }
        
        var category = categoryFactory.ToEntity(request.CategoryCreateDto);

        try
        {
            var created = await categoriesRepository.CreateAsync(category, cancellationToken);

            return created ? 
                BaseResponse.Ok() :
                BaseResponse.InternalServerError("Failed to create category");
        }
        catch (DbUpdateException exception) when 
            (exception.InnerException is SqlException { Number: 2601 })
        {
            return BaseResponse.Conflict("Category with the same name already exists.");
        }
        catch (Exception)
        {
            return BaseResponse.InternalServerError();
        }
    }
}