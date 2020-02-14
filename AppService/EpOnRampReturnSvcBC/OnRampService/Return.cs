using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Web;


// Class for eComm New Order object
namespace BC.Integration.AppService.EpReturnOnRampServiceBC
{
    public class JMS
    {
        [XmlRoot(ElementName = "Header", Namespace = "http://com.elasticpath/repo/order")]
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
            public decimal TotalSubtotalDiscount { get; set; }
            [XmlElement(ElementName = "TotalDiscountAmount")]
            public decimal TotalDiscountAmount { get; set; }
            [XmlElement(ElementName = "GrandTotal")]
            public decimal GrandTotal { get; set; }
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
            [XmlElement(ElementName = "LineItemId")]
            public string LineItemId { get; set; }
            [XmlElement(ElementName = "ProductCode")]
            public string ProductCode { get; set; }
            [XmlElement(ElementName = "ItemCode")]
            public string ItemCode { get; set; }
            [XmlElement(ElementName = "Quantity")]
            public decimal Quantity { get; set; }
            [XmlElement(ElementName = "ListUnitPrice")]
            public decimal ListUnitPrice { get; set; }
            [XmlElement(ElementName = "UnitPrice")]
            public decimal UnitPrice { get; set; }
            [XmlElement(ElementName = "ItemSubtotalPrice")]
            public decimal ItemSubtotalPrice { get; set; }
            [XmlElement(ElementName = "AmountBeforeTax")]
            public decimal AmountBeforeTax { get; set; }
            [XmlElement(ElementName = "ItemTaxes")]
            public string ItemTaxes { get; set; }
            [XmlElement(ElementName = "AmountIncludingTax")]
            public decimal AmountIncludingTax { get; set; }
            [XmlElement(ElementName = "DisplayName")]
            public string DisplayName { get; set; }
            [XmlElement(ElementName = "Options")]
            public Options Options { get; set; }
            [XmlElement(ElementName = "Fields")]
            public Fields Fields { get; set; }
            [XmlElement(ElementName = "TaxLines")]
            public TaxLines TaxLines { get; set; }
        }

        [XmlRoot(ElementName = "LineItems", Namespace = "http://com.elasticpath/repo/order")]
        public class LineItems
        {
            [XmlElement(ElementName = "LineItem")]
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
            [XmlElement(ElementName = "ShipmentCarrier")]
            public string ShipmentCarrier { get; set; }
            [XmlElement(ElementName = "LineItems")]
            public LineItems LineItems { get; set; }
            [XmlElement(ElementName = "ShippingTaxLines")]
            public ShippingTaxLines ShippingTaxLines { get; set; }
        }

        [XmlRoot(ElementName = "Shipments", Namespace = "http://com.elasticpath/repo/order")]
        public class Shipments
        {
            [XmlElement(ElementName = "Shipment")]
            public Shipment Shipment { get; set; }
        }

        [XmlRoot(ElementName = "Address", Namespace = "http://com.elasticpath/repo/common")]
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

        [XmlRoot(ElementName = "Addresses", Namespace = "http://com.elasticpath/repo/order")]
        public class Addresses
        {
            [XmlElement(ElementName = "Address", Namespace = "http://com.elasticpath/repo/common")]
            public List<Address> Address { get; set; }
        }

        /* [XmlRoot(ElementName = "PaymentMethod")]
         [XmlInclude(typeof(TokenPaymentMethod))]
         public class PaymentMethod
         {
             [XmlElement(ElementName = "PaymentType")]
             public string PaymentType { get; set; }
             [XmlElement(ElementName = "Provider")]
             public string Provider { get; set; }
             [XmlElement(ElementName = "Gateway")]
             public string Gateway { get; set; }
            //[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            // public string Xsi { get; set; }
            // [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            // public string Type { get; set; }
         }*/

        [XmlRoot(ElementName = "Payment", Namespace = "http://com.elasticpath/repo/order")]
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
            [XmlElement(ElementName = "Reason")]
            public string Reason { get; set; }
            /* [XmlElement(ElementName = "PaymentMethod")]
             public PaymentMethod PaymentMethod { get; set; }*/
            [XmlElement(ElementName = "ShipmentId")]
            public string ShipmentId { get; set; }
        }

        [XmlRoot(ElementName = "Payments", Namespace = "http://com.elasticpath/repo/order")]
        public class Payments
        {
            [XmlElement(ElementName = "Payment")]
            public List<Payment> Payment { get; set; }
        }

        /* public class TokenPaymentMethod : PaymentMethod
         {

             // [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
             // public string Type { get; set; }
         }*/

        [XmlRoot(ElementName = "Promotion", Namespace = "http://com.elasticpath/repo/order")]
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

        [XmlRoot(ElementName = "AppliedPromotions", Namespace = "http://com.elasticpath/repo/order")]
        public class AppliedPromotions
        {
            [XmlElement(ElementName = "Promotion")]
            public List<Promotion> Promotion { get; set; }
        }

        [XmlRoot(ElementName = "ReturnSku", Namespace = "http://com.elasticpath/repo/order")]
        public partial class ReturnSku
        {
            [XmlElement(ElementName = "ReturnSkuId")]
            public string ReturnSkuId { get; set; }
            [XmlElement(ElementName = "ReturnItemCode")]
            public string ReturnItemCode { get; set; }
            [XmlElement(ElementName = "LineItemId")]
            public string LineItemId { get; set; }
            [XmlElement(ElementName = "Quantity")]
            public int Quantity { get; set; }
            [XmlElement(ElementName = "ReceivedQuantity")]
            public int ReceivedQuantity { get; set; }
            [XmlElement(ElementName = "ReturnAmount")]
            public decimal ReturnAmount { get; set; }
            [XmlElement(ElementName = "UnitPrice")]
            public decimal UnitPrice { get; set; }
            [XmlElement(ElementName = "Tax")]
            public decimal Tax { get; set; }
            [XmlElement(ElementName = "ItemSubtotalPrice")]
            public decimal ItemSubtotalPrice { get; set; }
            [XmlElement(ElementName = "AmountBeforeTax")]
            public decimal AmountBeforeTax { get; set; }
            [XmlElement(ElementName = "AmountIncludingTax")]
            public decimal AmountIncludingTax { get; set; }
            [XmlElement(ElementName = "TaxLines")]
            public TaxLines TaxLines { get; set; }
            [XmlElement(ElementName = "ReturnReason")]
            public string ReturnReason { get; set; }
        }

        [XmlRoot(ElementName = "ReturnSkus", Namespace = "http://com.elasticpath/repo/order")]
        public partial class ReturnSkus
        {
            [XmlElement(ElementName = "ReturnSku")]
            public List <ReturnSku> ReturnSku { get; set; }
        }

        [XmlRoot(ElementName = "ReturnAddress", Namespace = "http://com.elasticpath/repo/order")]
        public partial class ReturnAddress
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
        public partial class Return
        {
            [XmlElement(ElementName = "ReturnId")]
            public string ReturnId { get; set; }

            [XmlElement(ElementName = "CreatedDate")]
            public string CreatedDate { get; set; }

            [XmlElement(ElementName = "CreatedBy")]
            public string CreatedBy { get; set; }

            [XmlElement(ElementName = "ReceivedBy")]
            public string ReceivedBy { get; set; }

            [XmlElement(ElementName = "RmaCode")]
            public string RmaCode { get; set; }

            [XmlElement(ElementName = "Status")]
            public string Status { get; set; }

            [XmlElement(ElementName = "ReturnType")]
            public string ReturnType { get; set; }

            [XmlElement(ElementName = "PhysicalReturn")]
            public string PhysicalReturn { get; set; }

            [XmlElement(ElementName = "ExchangeOrderId")]
            public string ExchangeOrderId { get; set; }

            [XmlElement(ElementName = "LessRestockAmount")]
            public decimal LessRestockAmount { get; set; }

            [XmlElement(ElementName = "ShippingCost")]
            public decimal ShippingCost { get; set; }

            [XmlElement(ElementName = "ShippingDiscount")]
            public decimal ShippingDiscount { get; set; }

            [XmlElement(ElementName = "ReturnSkus")]
            public ReturnSkus ReturnSkus { get; set; }

            [XmlElement(ElementName = "ReturnAddress")]
            public ReturnAddress ReturnAddress { get; set; }

            [XmlElement(ElementName = "ShippingTaxLines")]
            public ShippingTaxLines ShippingTaxLines { get; set; }
        }

        [XmlRoot(ElementName = "Returns", Namespace = "http://com.elasticpath/repo/order")]
        public partial class Returns
        {
            [XmlElement(ElementName = "Return")]
            public List<Return> Return { get; set; }
        }

        [XmlRoot(ElementName = "Order", Namespace = "http://com.elasticpath/repo/order")]
        public class ReturnJMS
        {
            [XmlElement(ElementName = "Header")]
            public Header Header { get; set; }

            [XmlElement(ElementName = "Customer")]
            public Customer Customer { get; set; }

            [XmlElement(ElementName = "Shipments")]
            public Shipments Shipments { get; set; }

            [XmlElement(ElementName = "Addresses", Namespace = "http://com.elasticpath/repo/order")]
            public Addresses Addresses { get; set; }

            [XmlElement(ElementName = "Payments")]
            public Payments Payments { get; set; }

            [XmlElement(ElementName = "Returns")]
            public Returns Returns { get; set; }

            [XmlElement(ElementName = "AppliedPromotions")]
            public AppliedPromotions Promotions { get; set; }

            public string ConvertOrderToString(ReturnJMS jms)
            {

                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(stringwriter, this);
                    return stringwriter.ToString();
                }
            }

        }

    }
}
