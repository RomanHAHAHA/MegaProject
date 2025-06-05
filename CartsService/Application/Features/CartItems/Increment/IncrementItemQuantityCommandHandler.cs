using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Common.Application.Options;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.CartItem;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace CartsService.Application.Features.CartItems.Increment;

public class IncrementItemQuantityCommandHandler(
    ICartsRepository cartsRepository,
    IPublishEndpoint publisher,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<IncrementItemQuantityCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(IncrementItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await cartsRepository.GetByIdAsync(
            request.UserId,
            request.ProductId,
            cancellationToken);

        if (cartItem is null)
        {
            return BaseResponse.NotFound(nameof(CartItem));
        }
        
        cartItem.Quantity++;
        await OnProductQuantityIncremented(cartItem, cancellationToken);
        
        var updated = await cartsRepository.SaveChangesAsync(cancellationToken);

        return updated ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }

    private async Task OnProductQuantityIncremented(CartItem cartItem, CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new ProductCartQuantityIncrementedEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                ProductId = cartItem.ProductId,
                UserId = cartItem.UserId,
                RequestedQuantity = cartItem.Quantity,
            },
            cancellationToken);
    }
}