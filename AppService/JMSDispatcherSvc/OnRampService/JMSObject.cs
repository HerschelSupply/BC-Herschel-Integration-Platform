/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace BC.Integration.AppService.JMSDispatcherSvc
{
    [XmlRoot(ElementName = "Header", Namespace = "http://com.elasticpath/repo/order")]
    public class Header
    {
        [XmlElement(ElementName = "OrderStatus", Namespace = "http://com.elasticpath/repo/order")]
        public string OrderStatus { get; set; }
        [XmlElement(ElementName = "StoreCode", Namespace = "http://com.elasticpath/repo/order")]
        public string StoreCode { get; set; }
        [XmlElement(ElementName = "CreatedDate", Namespace = "http://com.elasticpath/repo/order")]
        public string CreatedDate { get; set; }
        [XmlElement(ElementName = "Locale", Namespace = "http://com.elasticpath/repo/order")]
        public string Locale { get; set; }
        [XmlElement(ElementName = "Currency", Namespace = "http://com.elasticpath/repo/order")]
        public string Currency { get; set; }
        [XmlElement(ElementName = "OrderNumber", Namespace = "http://com.elasticpath/repo/order")]
        public string OrderNumber { get; set; }
        [XmlElement(ElementName = "EmailFrom", Namespace = "http://com.elasticpath/repo/order")]
        public string EmailFrom { get; set; }
        [XmlElement(ElementName = "TotalItemCostBeforeTax", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalItemCostBeforeTax { get; set; }
        [XmlElement(ElementName = "OriginalShippingCost", Namespace = "http://com.elasticpath/repo/order")]
        public string OriginalShippingCost { get; set; }
        [XmlElement(ElementName = "TotalShippingCostBeforeTax", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalShippingCostBeforeTax { get; set; }
        [XmlElement(ElementName = "ShippingDiscount", Namespace = "http://com.elasticpath/repo/order")]
        public string ShippingDiscount { get; set; }
        [XmlElement(ElementName = "TotalItemTaxes", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalItemTaxes { get; set; }
        [XmlElement(ElementName = "TotalShippingTaxes", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalShippingTaxes { get; set; }
        [XmlElement(ElementName = "TotalTaxes", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalTaxes { get; set; }
        [XmlElement(ElementName = "TotalItemCostIncludingTax", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalItemCostIncludingTax { get; set; }
        [XmlElement(ElementName = "TotalShippingCostIncludingTax", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalShippingCostIncludingTax { get; set; }
        [XmlElement(ElementName = "TotalLineItemDiscounts", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalLineItemDiscounts { get; set; }
        [XmlElement(ElementName = "TotalSubtotalDiscount", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalSubtotalDiscount { get; set; }
        [XmlElement(ElementName = "TotalDiscountAmount", Namespace = "http://com.elasticpath/repo/order")]
        public string TotalDiscountAmount { get; set; }
        [XmlElement(ElementName = "GrandTotal", Namespace = "http://com.elasticpath/repo/order")]
        public string GrandTotal { get; set; }
    }

    [XmlRoot(ElementName = "Customer", Namespace = "http://com.elasticpath/repo/order")]
    public class Customer
    {
        [XmlElement(ElementName = "CustomerId", Namespace = "http://com.elasticpath/repo/common")]
        public string CustomerId { get; set; }
        [XmlElement(ElementName = "CustomerUserName", Namespace = "http://com.elasticpath/repo/common")]
        public string CustomerUserName { get; set; }
        [XmlElement(ElementName = "FirstName", Namespace = "http://com.elasticpath/repo/common")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "LastName", Namespace = "http://com.elasticpath/repo/common")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "Email", Namespace = "http://com.elasticpath/repo/common")]
        public string Email { get; set; }
        [XmlElement(ElementName = "DateOfBirth", Namespace = "http://com.elasticpath/repo/common")]
        public string DateOfBirth { get; set; }
        [XmlElement(ElementName = "PcccNumber", Namespace = "http://com.elasticpath/repo/common")]
        public string PcccNumber { get; set; }
    }

    [XmlRoot(ElementName = "ShippingAddress", Namespace = "http://com.elasticpath/repo/order")]
    public class ShippingAddress
    {
        [XmlElement(ElementName = "AddressId", Namespace = "http://com.elasticpath/repo/common")]
        public string AddressId { get; set; }
        [XmlElement(ElementName = "AddressType", Namespace = "http://com.elasticpath/repo/common")]
        public string AddressType { get; set; }
        [XmlElement(ElementName = "FirstName", Namespace = "http://com.elasticpath/repo/common")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "LastName", Namespace = "http://com.elasticpath/repo/common")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "Street1", Namespace = "http://com.elasticpath/repo/common")]
        public string Street1 { get; set; }
        [XmlElement(ElementName = "Street2", Namespace = "http://com.elasticpath/repo/common")]
        public string Street2 { get; set; }
        [XmlElement(ElementName = "City", Namespace = "http://com.elasticpath/repo/common")]
        public string City { get; set; }
        [XmlElement(ElementName = "Region", Namespace = "http://com.elasticpath/repo/common")]
        public string Region { get; set; }
        [XmlElement(ElementName = "Country", Namespace = "http://com.elasticpath/repo/common")]
        public string Country { get; set; }
        [XmlElement(ElementName = "ZipPostalCode", Namespace = "http://com.elasticpath/repo/common")]
        public string ZipPostalCode { get; set; }
        [XmlElement(ElementName = "PhoneNumber", Namespace = "http://com.elasticpath/repo/common")]
        public string PhoneNumber { get; set; }
    }

    [XmlRoot(ElementName = "Option", Namespace = "http://com.elasticpath/repo/common")]
    public class Option
    {
        [XmlElement(ElementName = "Key", Namespace = "http://com.elasticpath/repo/common")]
        public string Key { get; set; }
        [XmlElement(ElementName = "Value", Namespace = "http://com.elasticpath/repo/common")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Options", Namespace = "http://com.elasticpath/repo/order")]
    public class Options
    {
        [XmlElement(ElementName = "Option", Namespace = "http://com.elasticpath/repo/common")]
        public List<Option> Option { get; set; }
    }

    [XmlRoot(ElementName = "Field", Namespace = "http://com.elasticpath/repo/common")]
    public class Field
    {
        [XmlElement(ElementName = "Key", Namespace = "http://com.elasticpath/repo/common")]
        public string Key { get; set; }
        [XmlElement(ElementName = "Value", Namespace = "http://com.elasticpath/repo/common")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Fields", Namespace = "http://com.elasticpath/repo/order")]
    public class Fields
    {
        [XmlElement(ElementName = "Field", Namespace = "http://com.elasticpath/repo/common")]
        public List<Field> Field { get; set; }
    }

    [XmlRoot(ElementName = "TaxLine", Namespace = "http://com.elasticpath/repo/common")]
    public class TaxLine
    {
        [XmlElement(ElementName = "JurisdictionId", Namespace = "http://com.elasticpath/repo/common")]
        public string JurisdictionId { get; set; }
        [XmlElement(ElementName = "TaxRegionId", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxRegionId { get; set; }
        [XmlElement(ElementName = "TaxIsInclusive", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxIsInclusive { get; set; }
        [XmlElement(ElementName = "TaxName", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxName { get; set; }
        [XmlElement(ElementName = "TaxCode", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxCode { get; set; }
        [XmlElement(ElementName = "TaxCodeGP", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxCodeGP { get; set; }
        [XmlElement(ElementName = "TaxAmount", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxAmount { get; set; }
        [XmlElement(ElementName = "TaxRate", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxRate { get; set; }
        [XmlElement(ElementName = "TaxCalculationDate", Namespace = "http://com.elasticpath/repo/common")]
        public string TaxCalculationDate { get; set; }
    }

    [XmlRoot(ElementName = "TaxLines", Namespace = "http://com.elasticpath/repo/order")]
    public class TaxLines
    {
        [XmlElement(ElementName = "TaxLine", Namespace = "http://com.elasticpath/repo/common")]
        public List<TaxLine> TaxLine { get; set; }
    }

    [XmlRoot(ElementName = "LineItem", Namespace = "http://com.elasticpath/repo/order")]
    public class LineItem
    {
        [XmlElement(ElementName = "LineItemId", Namespace = "http://com.elasticpath/repo/order")]
        public string LineItemId { get; set; }
        [XmlElement(ElementName = "ProductCode", Namespace = "http://com.elasticpath/repo/order")]
        public string ProductCode { get; set; }
        [XmlElement(ElementName = "ItemCode", Namespace = "http://com.elasticpath/repo/order")]
        public string ItemCode { get; set; }
        [XmlElement(ElementName = "Quantity", Namespace = "http://com.elasticpath/repo/order")]
        public string Quantity { get; set; }
        [XmlElement(ElementName = "ListUnitPrice", Namespace = "http://com.elasticpath/repo/order")]
        public string ListUnitPrice { get; set; }
        [XmlElement(ElementName = "UnitPrice", Namespace = "http://com.elasticpath/repo/order")]
        public string UnitPrice { get; set; }
        [XmlElement(ElementName = "ItemSubtotalPrice", Namespace = "http://com.elasticpath/repo/order")]
        public string ItemSubtotalPrice { get; set; }
        [XmlElement(ElementName = "AmountBeforeTax", Namespace = "http://com.elasticpath/repo/order")]
        public string AmountBeforeTax { get; set; }
        [XmlElement(ElementName = "ItemTaxes", Namespace = "http://com.elasticpath/repo/order")]
        public string ItemTaxes { get; set; }
        [XmlElement(ElementName = "AmountIncludingTax", Namespace = "http://com.elasticpath/repo/order")]
        public string AmountIncludingTax { get; set; }
        [XmlElement(ElementName = "DisplayName", Namespace = "http://com.elasticpath/repo/order")]
        public string DisplayName { get; set; }
        [XmlElement(ElementName = "Options", Namespace = "http://com.elasticpath/repo/order")]
        public Options Options { get; set; }
        [XmlElement(ElementName = "Fields", Namespace = "http://com.elasticpath/repo/order")]
        public Fields Fields { get; set; }
        [XmlElement(ElementName = "TaxLines", Namespace = "http://com.elasticpath/repo/order")]
        public TaxLines TaxLines { get; set; }
    }

    [XmlRoot(ElementName = "LineItems", Namespace = "http://com.elasticpath/repo/order")]
    public class LineItems
    {
        [XmlElement(ElementName = "LineItem", Namespace = "http://com.elasticpath/repo/order")]
        public List<LineItem> LineItem { get; set; }
    }

    [XmlRoot(ElementName = "ShippingTaxLines", Namespace = "http://com.elasticpath/repo/order")]
    public class ShippingTaxLines
    {
        [XmlElement(ElementName = "TaxLine", Namespace = "http://com.elasticpath/repo/common")]
        public List<TaxLine> TaxLine { get; set; }
    }

    [XmlRoot(ElementName = "Shipment", Namespace = "http://com.elasticpath/repo/order")]
    public class Shipment
    {
        [XmlElement(ElementName = "ShipmentId", Namespace = "http://com.elasticpath/repo/order")]
        public string ShipmentId { get; set; }
        [XmlElement(ElementName = "ShipmentStatus", Namespace = "http://com.elasticpath/repo/order")]
        public string ShipmentStatus { get; set; }
        [XmlElement(ElementName = "ShippingCost", Namespace = "http://com.elasticpath/repo/order")]
        public string ShippingCost { get; set; }
        [XmlElement(ElementName = "DiscountAmount", Namespace = "http://com.elasticpath/repo/order")]
        public string DiscountAmount { get; set; }
        [XmlElement(ElementName = "ServiceLevel", Namespace = "http://com.elasticpath/repo/order")]
        public string ServiceLevel { get; set; }
        [XmlElement(ElementName = "ShippingAddress", Namespace = "http://com.elasticpath/repo/order")]
        public ShippingAddress ShippingAddress { get; set; }
        [XmlElement(ElementName = "ShipmentType", Namespace = "http://com.elasticpath/repo/order")]
        public string ShipmentType { get; set; }
        [XmlElement(ElementName = "ShipmentCompleteDate", Namespace = "http://com.elasticpath/repo/order")]
        public string ShipmentCompleteDate { get; set; }
        [XmlElement(ElementName = "ShipmentCarrier", Namespace = "http://com.elasticpath/repo/order")]
        public string ShipmentCarrier { get; set; }
        [XmlElement(ElementName = "TrackingReference", Namespace = "http://com.elasticpath/repo/order")]
        public string TrackingReference { get; set; }
        [XmlElement(ElementName = "LineItems", Namespace = "http://com.elasticpath/repo/order")]
        public LineItems LineItems { get; set; }
        [XmlElement(ElementName = "ShippingTaxLines", Namespace = "http://com.elasticpath/repo/order")]
        public ShippingTaxLines ShippingTaxLines { get; set; }
        [XmlElement(ElementName = "DistributionCenterCode", Namespace = "http://com.elasticpath/repo/order")]
        public string DistributionCenterCode { get; set; }
    }

    [XmlRoot(ElementName = "Shipments", Namespace = "http://com.elasticpath/repo/order")]
    public class Shipments
    {
        [XmlElement(ElementName = "Shipment", Namespace = "http://com.elasticpath/repo/order")]
        public List<Shipment> Shipment { get; set; }
    }

    [XmlRoot(ElementName = "Address", Namespace = "http://com.elasticpath/repo/common")]
    public class Address
    {
        [XmlElement(ElementName = "AddressId", Namespace = "http://com.elasticpath/repo/common")]
        public string AddressId { get; set; }
        [XmlElement(ElementName = "AddressType", Namespace = "http://com.elasticpath/repo/common")]
        public string AddressType { get; set; }
        [XmlElement(ElementName = "FirstName", Namespace = "http://com.elasticpath/repo/common")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "LastName", Namespace = "http://com.elasticpath/repo/common")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "Street1", Namespace = "http://com.elasticpath/repo/common")]
        public string Street1 { get; set; }
        [XmlElement(ElementName = "Street2", Namespace = "http://com.elasticpath/repo/common")]
        public string Street2 { get; set; }
        [XmlElement(ElementName = "City", Namespace = "http://com.elasticpath/repo/common")]
        public string City { get; set; }
        [XmlElement(ElementName = "Region", Namespace = "http://com.elasticpath/repo/common")]
        public string Region { get; set; }
        [XmlElement(ElementName = "Country", Namespace = "http://com.elasticpath/repo/common")]
        public string Country { get; set; }
        [XmlElement(ElementName = "ZipPostalCode", Namespace = "http://com.elasticpath/repo/common")]
        public string ZipPostalCode { get; set; }
        [XmlElement(ElementName = "PhoneNumber", Namespace = "http://com.elasticpath/repo/common")]
        public string PhoneNumber { get; set; }
    }

    [XmlRoot(ElementName = "Addresses", Namespace = "http://com.elasticpath/repo/order")]
    public class Addresses
    {
        [XmlElement(ElementName = "Address", Namespace = "http://com.elasticpath/repo/common")]
        public List<Address> Address { get; set; }
    }

    [XmlRoot(ElementName = "PaymentMethod", Namespace = "http://com.elasticpath/repo/order")]
    public class PaymentMethod
    {
        [XmlElement(ElementName = "PaymentType", Namespace = "http://com.elasticpath/repo/order")]
        public string PaymentType { get; set; }
        [XmlElement(ElementName = "Provider", Namespace = "http://com.elasticpath/repo/order")]
        public string Provider { get; set; }
        [XmlElement(ElementName = "Gateway", Namespace = "http://com.elasticpath/repo/order")]
        public string Gateway { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "Payment", Namespace = "http://com.elasticpath/repo/order")]
    public class Payment
    {
        [XmlElement(ElementName = "PaymentId", Namespace = "http://com.elasticpath/repo/order")]
        public string PaymentId { get; set; }
        [XmlElement(ElementName = "Amount", Namespace = "http://com.elasticpath/repo/order")]
        public string Amount { get; set; }
        [XmlElement(ElementName = "TransactionType", Namespace = "http://com.elasticpath/repo/order")]
        public string TransactionType { get; set; }
        [XmlElement(ElementName = "TransactionDate", Namespace = "http://com.elasticpath/repo/order")]
        public string TransactionDate { get; set; }
        [XmlElement(ElementName = "PaymentStatus", Namespace = "http://com.elasticpath/repo/order")]
        public string PaymentStatus { get; set; }
        [XmlElement(ElementName = "ReferenceId", Namespace = "http://com.elasticpath/repo/order")]
        public string ReferenceId { get; set; }
        [XmlElement(ElementName = "RequestToken", Namespace = "http://com.elasticpath/repo/order")]
        public string RequestToken { get; set; }
        [XmlElement(ElementName = "PaymentMethod", Namespace = "http://com.elasticpath/repo/order")]
        public PaymentMethod PaymentMethod { get; set; }
        [XmlElement(ElementName = "ShipmentId", Namespace = "http://com.elasticpath/repo/order")]
        public string ShipmentId { get; set; }
    }

    [XmlRoot(ElementName = "Payments", Namespace = "http://com.elasticpath/repo/order")]
    public class Payments
    {
        [XmlElement(ElementName = "Payment", Namespace = "http://com.elasticpath/repo/order")]
        public List<Payment> Payment { get; set; }
    }

    [XmlRoot(ElementName = "ReturnSku", Namespace = "http://com.elasticpath/repo/order")]
    public class ReturnSku
    {
        [XmlElement(ElementName = "ReturnSkuId", Namespace = "http://com.elasticpath/repo/order")]
        public string ReturnSkuId { get; set; }
        [XmlElement(ElementName = "ReturnItemCode", Namespace = "http://com.elasticpath/repo/order")]
        public string ReturnItemCode { get; set; }
        [XmlElement(ElementName = "LineItemId", Namespace = "http://com.elasticpath/repo/order")]
        public string LineItemId { get; set; }
        [XmlElement(ElementName = "Quantity", Namespace = "http://com.elasticpath/repo/order")]
        public string Quantity { get; set; }
        [XmlElement(ElementName = "ReceivedQuantity", Namespace = "http://com.elasticpath/repo/order")]
        public string ReceivedQuantity { get; set; }
        [XmlElement(ElementName = "ReturnAmount", Namespace = "http://com.elasticpath/repo/order")]
        public string ReturnAmount { get; set; }
        [XmlElement(ElementName = "UnitPrice", Namespace = "http://com.elasticpath/repo/order")]
        public string UnitPrice { get; set; }
        [XmlElement(ElementName = "Tax", Namespace = "http://com.elasticpath/repo/order")]
        public string Tax { get; set; }
        [XmlElement(ElementName = "ItemSubtotalPrice", Namespace = "http://com.elasticpath/repo/order")]
        public string ItemSubtotalPrice { get; set; }
        [XmlElement(ElementName = "AmountBeforeTax", Namespace = "http://com.elasticpath/repo/order")]
        public string AmountBeforeTax { get; set; }
        [XmlElement(ElementName = "AmountIncludingTax", Namespace = "http://com.elasticpath/repo/order")]
        public string AmountIncludingTax { get; set; }
        [XmlElement(ElementName = "TaxLines", Namespace = "http://com.elasticpath/repo/order")]
        public TaxLines TaxLines { get; set; }
        [XmlElement(ElementName = "ReturnReason", Namespace = "http://com.elasticpath/repo/order")]
        public string ReturnReason { get; set; }
    }

    [XmlRoot(ElementName = "ReturnSkus", Namespace = "http://com.elasticpath/repo/order")]
    public class ReturnSkus
    {
        [XmlElement(ElementName = "ReturnSku", Namespace = "http://com.elasticpath/repo/order")]
        public ReturnSku ReturnSku { get; set; }
    }

    [XmlRoot(ElementName = "ReturnAddress", Namespace = "http://com.elasticpath/repo/order")]
    public class ReturnAddress
    {
        [XmlElement(ElementName = "AddressId", Namespace = "http://com.elasticpath/repo/common")]
        public string AddressId { get; set; }
        [XmlElement(ElementName = "AddressType", Namespace = "http://com.elasticpath/repo/common")]
        public string AddressType { get; set; }
        [XmlElement(ElementName = "FirstName", Namespace = "http://com.elasticpath/repo/common")]
        public string FirstName { get; set; }
        [XmlElement(ElementName = "LastName", Namespace = "http://com.elasticpath/repo/common")]
        public string LastName { get; set; }
        [XmlElement(ElementName = "Street1", Namespace = "http://com.elasticpath/repo/common")]
        public string Street1 { get; set; }
        [XmlElement(ElementName = "Street2", Namespace = "http://com.elasticpath/repo/common")]
        public string Street2 { get; set; }
        [XmlElement(ElementName = "City", Namespace = "http://com.elasticpath/repo/common")]
        public string City { get; set; }
        [XmlElement(ElementName = "Region", Namespace = "http://com.elasticpath/repo/common")]
        public string Region { get; set; }
        [XmlElement(ElementName = "Country", Namespace = "http://com.elasticpath/repo/common")]
        public string Country { get; set; }
        [XmlElement(ElementName = "ZipPostalCode", Namespace = "http://com.elasticpath/repo/common")]
        public string ZipPostalCode { get; set; }
        [XmlElement(ElementName = "PhoneNumber", Namespace = "http://com.elasticpath/repo/common")]
        public string PhoneNumber { get; set; }
    }

    [XmlRoot(ElementName = "Return", Namespace = "http://com.elasticpath/repo/order")]
    public class Return
    {
        [XmlElement(ElementName = "ReturnId", Namespace = "http://com.elasticpath/repo/order")]
        public string ReturnId { get; set; }
        [XmlElement(ElementName = "CreatedDate", Namespace = "http://com.elasticpath/repo/order")]
        public string CreatedDate { get; set; }
        [XmlElement(ElementName = "CreatedBy", Namespace = "http://com.elasticpath/repo/order")]
        public string CreatedBy { get; set; }
        [XmlElement(ElementName = "RmaCode", Namespace = "http://com.elasticpath/repo/order")]
        public string RmaCode { get; set; }
        [XmlElement(ElementName = "Status", Namespace = "http://com.elasticpath/repo/order")]
        public string Status { get; set; }
        [XmlElement(ElementName = "ReturnType", Namespace = "http://com.elasticpath/repo/order")]
        public string ReturnType { get; set; }
        [XmlElement(ElementName = "PhysicalReturn", Namespace = "http://com.elasticpath/repo/order")]
        public string PhysicalReturn { get; set; }
        [XmlElement(ElementName = "ExchangeOrderId", Namespace = "http://com.elasticpath/repo/order")]
        public string ExchangeOrderId { get; set; }
        [XmlElement(ElementName = "LessRestockAmount", Namespace = "http://com.elasticpath/repo/order")]
        public string LessRestockAmount { get; set; }
        [XmlElement(ElementName = "ShippingCost", Namespace = "http://com.elasticpath/repo/order")]
        public string ShippingCost { get; set; }
        [XmlElement(ElementName = "ShippingDiscount", Namespace = "http://com.elasticpath/repo/order")]
        public string ShippingDiscount { get; set; }
        [XmlElement(ElementName = "ReturnSkus", Namespace = "http://com.elasticpath/repo/order")]
        public ReturnSkus ReturnSkus { get; set; }
        [XmlElement(ElementName = "ReturnAddress", Namespace = "http://com.elasticpath/repo/order")]
        public ReturnAddress ReturnAddress { get; set; }
        [XmlElement(ElementName = "ShippingTaxLines", Namespace = "http://com.elasticpath/repo/order")]
        public ShippingTaxLines ShippingTaxLines { get; set; }
    }

    [XmlRoot(ElementName = "Returns", Namespace = "http://com.elasticpath/repo/order")]
    public class Returns
    {
        [XmlElement(ElementName = "Return", Namespace = "http://com.elasticpath/repo/order")]
        public Return Return { get; set; }
    }

    [XmlRoot(ElementName = "Promotion", Namespace = "http://com.elasticpath/repo/order")]
    public class Promotion
    {
        [XmlElement(ElementName = "PromoName", Namespace = "http://com.elasticpath/repo/order")]
        public string PromoName { get; set; }
        [XmlElement(ElementName = "DisplayName", Namespace = "http://com.elasticpath/repo/order")]
        public string DisplayName { get; set; }
        [XmlElement(ElementName = "DisplayDescription", Namespace = "http://com.elasticpath/repo/order")]
        public string DisplayDescription { get; set; }
        [XmlElement(ElementName = "Coupons", Namespace = "http://com.elasticpath/repo/order")]
        public Coupons Coupons { get; set; }
    }

    [XmlRoot(ElementName = "Coupons", Namespace = "http://com.elasticpath/repo/order")]
    public class Coupons
    {
        [XmlElement(ElementName = "Code", Namespace = "http://com.elasticpath/repo/order")]
        public string Code { get; set; }
    }

    [XmlRoot(ElementName = "AppliedPromotions", Namespace = "http://com.elasticpath/repo/order")]
    public class AppliedPromotions
    {
        [XmlElement(ElementName = "Promotion", Namespace = "http://com.elasticpath/repo/order")]
        public List<Promotion> Promotion { get; set; }
    }

    [XmlRoot(ElementName = "Order", Namespace = "http://com.elasticpath/repo/order")]
    public class Order
    {
        [XmlElement(ElementName = "Header", Namespace = "http://com.elasticpath/repo/order")]
        public Header Header { get; set; }
        [XmlElement(ElementName = "Customer", Namespace = "http://com.elasticpath/repo/order")]
        public Customer Customer { get; set; }
        [XmlElement(ElementName = "Shipments", Namespace = "http://com.elasticpath/repo/order")]
        public Shipments Shipments { get; set; }
        [XmlElement(ElementName = "Addresses", Namespace = "http://com.elasticpath/repo/order")]
        public Addresses Addresses { get; set; }
        [XmlElement(ElementName = "Payments", Namespace = "http://com.elasticpath/repo/order")]
        public Payments Payments { get; set; }
        [XmlElement(ElementName = "Returns", Namespace = "http://com.elasticpath/repo/order")]
        public Returns Returns { get; set; }
        [XmlElement(ElementName = "AppliedPromotions", Namespace = "http://com.elasticpath/repo/order")]
        public AppliedPromotions AppliedPromotions { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "ns2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns2 { get; set; }
    }

}
