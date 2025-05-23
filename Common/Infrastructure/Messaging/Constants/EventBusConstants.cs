namespace Common.Infrastructure.Messaging.Constants;

public static class EventBusConstants
{
    public static class Exchanges
    {
        public const string Cart = "cart_exchange";
        public const string Log = "log_exchange";
        public const string Email = "email_exchange";
        public const string User = "user_exchange";
        public const string Product = "product_exchange";
        public const string Order = "order_exchange";
    }

    public static class Queues
    {
        public const string CartServiceProductCreated = "cart_queue_product_created";
        public const string CartServiceProductUpdated = "cart_queue_product_updated";
        public const string CartServiceProductImageSet = "cart_queue_product_image_set";
        public const string CartServiceOrderCreated = "cart_queue_order_created";

        public const string OrderServiceUserRegistered = "order_service_user_registered_queue";
        public const string OrderServiceProductCreated = "order_queue_product_created";
        public const string OrderServiceProductUpdated = "order_queue_product_updated";
        public const string OrderServiceProductImageSet = "order_queue_product_image_set";
        
        public const string LogsActionPerformed = "logs_queue_action_performed";
        public const string LogsIncorrectPasswordAttempted = "logs_queue_password_incorrect";
        public const string LogsPasswordUpdated = "logs_queue_password_updated";
        
        public const string EmailServiceUserRegistered = "email_serice_user_registered_queue";
        public const string UserEmailConfirmed = "user_email_confirmed_queue";
        public const string UserAvatarUpdated = "user_avatar_updated_queue";
    }

    public static class RoutingKeys
    {
        public const string ProductCreated = "product.created";
        public const string ProductUpdated = "product.updated";
        public const string ProductMainImageSet = "product.image.set";

        public const string LogActionPerformed = "log.action_performed";

        public const string SecurityIncorrectPassword = "security.password.incorrect";
        public const string SecurityPasswordUpdated = "security.password.updated";
        
        public const string UserRegistered = "user.registered";
        public const string UserAvatarUpdated = "user.avatar.updated";
        public const string EmailConfirmed = "email.confirmed";
        
        public const string OrderCreated = "order.created";
    }
}