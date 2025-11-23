using System;
using System.Collections.Generic;
using System.Text;

namespace AspireCleanArch.Domain.Entities.ValueObjects
{
    /// <summary>
    /// Shipping address value object extending base Address
    /// </summary>
    public record ShippingAddress : Address
    {
        public string RecipientName { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string? DeliveryInstructions { get; init; }

        // Constructor for creating shipping address
        public ShippingAddress(
            string street,
            string city,
            string state,
            string zipCode,
            string country,
            string recipientName,
            string phoneNumber,
            string? deliveryInstructions = null)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            RecipientName = recipientName;
            PhoneNumber = phoneNumber;
            DeliveryInstructions = deliveryInstructions;
        }

        // Parameterless constructor for EF Core
        public ShippingAddress() { }
    }
}
