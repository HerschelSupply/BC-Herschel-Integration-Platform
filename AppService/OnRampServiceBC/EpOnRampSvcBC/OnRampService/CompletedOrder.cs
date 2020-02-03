using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Web;


namespace BC.Integration.AppService.EpOnRampServiceBC
{
    public class CompletedOrder
    {
        [XmlRoot(ElementName = "Header")]
        public class Header
        {
            [XmlElement(ElementName = "OrderStatus")]
            public string OrderStatus { get; set; }
            [XmlElement(ElementName = "StoreCode")]
            public string StoreCode { get; set; }
            [XmlElement(ElementName = "CreatedDate")]
            public string CreatedDate { get; set; }
            [XmlElement(ElementName = "Locale")]
            public string Locale { get; set; }
            [XmlElement(ElementName = "Currency")]
            public string Currency { get; set; }
            [XmlElement(ElementName = "OrderNumber")]
            public string OrderNumber { get; set; }
            [XmlElement(ElementName = "EmailFrom")]
            public string EmailFrom { get; set; }
            [XmlElement(ElementName = "TotalItemCostBeforeTax")]
            public string TotalItemCostBeforeTax { get; set; }
            [XmlElement(ElementName = "OriginalShippingCost")]
            public string OriginalShippingCost { get; set; }
            [XmlElement(ElementName = "TotalShippingCostBeforeTax")]
            public string TotalShippingCostBeforeTax { get; set; }
            [XmlElement(ElementName = "ShippingDiscount")]
            public string ShippingDiscount { get; set; }
            [XmlElement(ElementName = "TotalItemTaxes")]
            public string TotalItemTaxes { get; set; }
            [XmlElement(ElementName = "TotalShippingTaxes")]
            public string TotalShippingTaxes { get; set; }
            [XmlElement(ElementName = "TotalTaxes")]
            public string TotalTaxes { get; set; }
            [XmlElement(ElementName = "TotalItemCostIncludingTax")]
            public string TotalItemCostIncludingTax { get; set; }
            [XmlElement(ElementName = "TotalShippingCostIncludingTax")]
            public string TotalShippingCostIncludingTax { get; set; }
            [XmlElement(ElementName = "TotalLineItemDiscounts")]
            public string TotalLineItemDiscounts { get; set; }
            [XmlElement(ElementName = "TotalSubtotalDiscount")]
            public string TotalSubtotalDiscount { get; set; }
            [XmlElement(ElementName = "TotalDiscountAmount")]
            public string TotalDiscountAmount { get; set; }
            [XmlElement(ElementName = "GrandTotal")]
            public string GrandTotal { get; set; }
        }

        [XmlRoot(ElementName = "Customer")]
        public class Customer
        {
            [XmlElement(ElementName = "CustomerId")]
            public string CustomerId { get; set; }
            [XmlElement(ElementName = "CustomerUserName")]
            public string CustomerUserName { get; set; }
            [XmlElement(ElementName = "FirstName")]
            public string FirstName { get; set; }
            [XmlElement(ElementName = "LastName")]
            public string LastName { get; set; }
            [XmlElement(ElementName = "Email")]
            public string Email { get; set; }
            [XmlElement(ElementName = "DateOfBirth")]
            public string DateOfBirth { get; set; }
            [XmlElement(ElementName = "PcccNumber")]
            public string PcccNumber { get; set; }
        }

        [XmlRoot(ElementName = "ShippingAddress")]
        public class ShippingAddress
        {
            [XmlElement(ElementName = "AddressId")]
            public string AddressId { get; set; }
            [XmlElement(ElementName = "AddressType")]
            public string AddressType { get; set; }
            [XmlElement(ElementName = "FirstName")]
            public string FirstName { get; set; }
            [XmlElement(ElementName = "LastName")]
            public string LastName { get; set; }
            [XmlElement(ElementName = "Street1")]
            public string Street1 { get; set; }
            [XmlElement(ElementName = "Street2")]
            public string Street2 { get; set; }
            [XmlElement(ElementName = "City")]
            public string City { get; set; }
            [XmlElement(ElementName = "Region")]
            public string Region { get; set; }
            [XmlElement(ElementName = "Country")]
            public string Country { get; set; }
            [XmlElement(ElementName = "ZipPostalCode")]
            public string ZipPostalCode { get; set; }
            [XmlElement(ElementName = "PhoneNumber")]
            public string PhoneNumber { get; set; }
        }

        [XmlRoot(ElementName = "Option")]
        public class Option
        {
            [XmlElement(ElementName = "Key")]
            public string Key { get; set; }
            [XmlElement(ElementName = "Value")]
            public string Value { get; set; }
        }

        [XmlRoot(ElementName = "Options")]
        public class Options
        {
            [XmlElement(ElementName = "Option")]
            public List<Option> Option { get; set; }
        }

        [XmlRoot(ElementName = "Field")]
        public class Field
        {
            [XmlElement(ElementName = "Key")]
            public string Key { get; set; }
            [XmlElement(ElementName = "Value")]
            public string Value { get; set; }
        }

        [XmlRoot(ElementName = "Fields")]
        public class Fields
        {
            [XmlElement(ElementName = "Field")]
            public List<Field> Field { get; set; }
        }

        [XmlRoot(ElementName = "TaxLine")]
        public class TaxLine
        {
            [XmlElement(ElementName = "JurisdictionId")]
            public string JurisdictionId { get; set; }
            [XmlElement(ElementName = "TaxRegionId")]
            public string TaxRegionId { get; set; }
            [XmlElement(ElementName = "TaxIsInclusive")]
            public string TaxIsInclusive { get; set; }
            [XmlElement(ElementName = "TaxName")]
            public string TaxName { get; set; }
            [XmlElement(ElementName = "TaxCode")]
            public string TaxCode { get; set; }
            [XmlElement(ElementName = "TaxCodeGP")]
            public string TaxCodeGP { get; set; }
            [XmlElement(ElementName = "TaxAmount")]
            public string TaxAmount { get; set; }
            [XmlElement(ElementName = "TaxRate")]
            public string TaxRate { get; set; }
            [XmlElement(ElementName = "TaxCalculationDate")]
            public string TaxCalculationDate { get; set; }
        }

        [XmlRoot(ElementName = "TaxLines")]
        public class TaxLines
        {
            [XmlElement(ElementName = "TaxLine")]
            public TaxLine TaxLine { get; set; }
        }

        [XmlRoot(ElementName = "LineItem")]
        public class LineItem
        {
            [XmlElement(ElementName = "LineItemId")]
            public string LineItemId { get; set; }
            [XmlElement(ElementName = "ProductCode")]
            public string ProductCode { get; set; }
            [XmlElement(ElementName = "ItemCode")]
            public string ItemCode { get; set; }
            [XmlElement(ElementName = "Quantity")]
            public string Quantity { get; set; }
            [XmlElement(ElementName = "ListUnitPrice")]
            public string ListUnitPrice { get; set; }
            [XmlElement(ElementName = "UnitPrice")]
            public string UnitPrice { get; set; }
            [XmlElement(ElementName = "ItemSubtotalPrice")]
            public string ItemSubtotalPrice { get; set; }
            [XmlElement(ElementName = "AmountBeforeTax")]
            public string AmountBeforeTax { get; set; }
            [XmlElement(ElementName = "ItemTaxes")]
            public string ItemTaxes { get; set; }
            [XmlElement(ElementName = "AmountIncludingTax")]
            public string AmountIncludingTax { get; set; }
            [XmlElement(ElementName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlElement(ElementName = "Options")]
            public Options Options { get; set; }
            [XmlElement(ElementName = "Fields")]
            public Fields Fields { get; set; }
            [XmlElement(ElementName = "TaxLines")]
            public TaxLines TaxLines { get; set; }
        }

        [XmlRoot(ElementName = "LineItems")]
        public class LineItems
        {
            [XmlElement(ElementName = "LineItem")]
            public LineItem LineItem { get; set; }
        }

        [XmlRoot(ElementName = "ShippingTaxLines")]
        public class ShippingTaxLines
        {
            [XmlElement(ElementName = "TaxLine")]
            public TaxLine TaxLine { get; set; }
        }

        [XmlRoot(ElementName = "Shipment")]
        public class Shipment
        {
            [XmlElement(ElementName = "ShipmentId")]
            public string ShipmentId { get; set; }
            [XmlElement(ElementName = "ShipmentStatus")]
            public string ShipmentStatus { get; set; }
            [XmlElement(ElementName = "ShippingCost")]
            public string ShippingCost { get; set; }
            [XmlElement(ElementName = "DiscountAmount")]
            public string DiscountAmount { get; set; }
            [XmlElement(ElementName = "ServiceLevel")]
            public string ServiceLevel { get; set; }
            [XmlElement(ElementName = "ShippingAddress")]
            public ShippingAddress ShippingAddress { get; set; }
            [XmlElement(ElementName = "ShipmentType")]
            public string ShipmentType { get; set; }
            [XmlElement(ElementName = "ShipmentCompleteDate")]
            public string ShipmentCompleteDate { get; set; }
            [XmlElement(ElementName = "ShipmentCarrier")]
            public string ShipmentCarrier { get; set; }
            [XmlElement(ElementName = "TrackingReference")]
            public string TrackingReference { get; set; }
            [XmlElement(ElementName = "LineItems")]
            public LineItems LineItems { get; set; }
            [XmlElement(ElementName = "ShippingTaxLines")]
            public ShippingTaxLines ShippingTaxLines { get; set; }
            [XmlElement(ElementName = "DistributionCenterCode")]
            public string DistributionCenterCode { get; set; }
        }

        [XmlRoot(ElementName = "Shipments")]
        public class Shipments
        {
            [XmlElement(ElementName = "Shipment")]
            public Shipment Shipment { get; set; }
        }

        [XmlRoot(ElementName = "Address")]
        public class Address
        {
            [XmlElement(ElementName = "AddressId")]
            public string AddressId { get; set; }
            [XmlElement(ElementName = "AddressType")]
            public string AddressType { get; set; }
            [XmlElement(ElementName = "FirstName")]
            public string FirstName { get; set; }
            [XmlElement(ElementName = "LastName")]
            public string LastName { get; set; }
            [XmlElement(ElementName = "Street1")]
            public string Street1 { get; set; }
            [XmlElement(ElementName = "Street2")]
            public string Street2 { get; set; }
            [XmlElement(ElementName = "City")]
            public string City { get; set; }
            [XmlElement(ElementName = "Region")]
            public string Region { get; set; }
            [XmlElement(ElementName = "Country")]
            public string Country { get; set; }
            [XmlElement(ElementName = "ZipPostalCode")]
            public string ZipPostalCode { get; set; }
            [XmlElement(ElementName = "PhoneNumber")]
            public string PhoneNumber { get; set; }
        }

        [XmlRoot(ElementName = "Addresses")]
        public class Addresses
        {
            [XmlElement(ElementName = "Address")]
            public List<Address> Address { get; set; }
        }

        [XmlRoot(ElementName = "PaymentMethod")]
        public class PaymentMethod
        {
            [XmlElement(ElementName = "PaymentType")]
            public string PaymentType { get; set; }
            [XmlElement(ElementName = "Provider")]
            public string Provider { get; set; }
            [XmlElement(ElementName = "Gateway")]
            public string Gateway { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            public string Type { get; set; }
        }

        [XmlRoot(ElementName = "Payment")]
        public class Payment
        {
            [XmlElement(ElementName = "PaymentId")]
            public string PaymentId { get; set; }
            [XmlElement(ElementName = "Amount")]
            public string Amount { get; set; }
            [XmlElement(ElementName = "TransactionType")]
            public string TransactionType { get; set; }
            [XmlElement(ElementName = "TransactionDate")]
            public string TransactionDate { get; set; }
            [XmlElement(ElementName = "PaymentStatus")]
            public string PaymentStatus { get; set; }
            [XmlElement(ElementName = "ReferenceId")]
            public string ReferenceId { get; set; }
            [XmlElement(ElementName = "RequestToken")]
            public string RequestToken { get; set; }
            [XmlElement(ElementName = "PaymentMethod")]
            public PaymentMethod PaymentMethod { get; set; }
            [XmlElement(ElementName = "ShipmentId")]
            public string ShipmentId { get; set; }
        }

        [XmlRoot(ElementName = "Payments")]
        public class Payments
        {
            [XmlElement(ElementName = "Payment")]
            public List<Payment> Payment { get; set; }
        }

        [XmlRoot(ElementName = "Promotion")]
        public class Promotion
        {
            [XmlElement(ElementName = "PromoName")]
            public string PromoName { get; set; }
            [XmlElement(ElementName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlElement(ElementName = "DisplayDescription")]
            public string DisplayDescription { get; set; }
            [XmlElement(ElementName = "Coupons")]
            public string Coupons { get; set; }
        }

        [XmlRoot(ElementName = "AppliedPromotions")]
        public class AppliedPromotions
        {
            [XmlElement(ElementName = "Promotion")]
            public Promotion Promotion { get; set; }
        }

        [XmlRoot(ElementName = "Order")]
        public class Order
        {
            [XmlElement(ElementName = "Header")]
            public Header Header { get; set; }
            [XmlElement(ElementName = "Customer")]
            public Customer Customer { get; set; }
            [XmlElement(ElementName = "Shipments")]
            public Shipments Shipments { get; set; }
            [XmlElement(ElementName = "Addresses")]
            public Addresses Addresses { get; set; }
            [XmlElement(ElementName = "Payments")]
            public Payments Payments { get; set; }
            [XmlElement(ElementName = "Returns")]
            public string Returns { get; set; }
            [XmlElement(ElementName = "AppliedPromotions")]
            public AppliedPromotions AppliedPromotions { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
            [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Ns2 { get; set; }
        }
    }
}