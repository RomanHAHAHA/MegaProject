using CartsService.Domain.Interfaces;
using MediatR;

namespace CartsService.Application.Features.CartItems.CleanCart;

public class CleanCartCommandHandler(
    ICartsRepository cartsRepository) : IRequestHandler<CleanCartCommand>
{
    public async Task Handle(
        CleanCartCommand request, 
        CancellationToken cancellationToken)
    {
        await cartsRepository.DeleteItemsFromUserCartAsync(
            request.UserId,
            cancellationToken);
    }
}