namespace AspireCleanArch.Shared.Common
{
    public record ShippingAddressDto : AddressDto
    {
        public string RecipientName { get; set; }
        public string PhoneNumber { get; set; }
        public string? DeliveryInstructions { get; set; }
    }

}
