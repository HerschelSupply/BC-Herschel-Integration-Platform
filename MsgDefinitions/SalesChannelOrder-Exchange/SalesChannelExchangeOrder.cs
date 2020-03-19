﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BC.Integration.Canonical.SaleChannelOrder.Exchange
{


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://BC.Integration.Schema.ECommerce.SalesChannelOrder.Exchange")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://BC.Integration.Schema.ECommerce.SalesChannelOrder.Exchange", IsNullable = false)]
    public partial class Order
    {

        private string processField;

        private OrderHeader headerField;

        private List<OrderLineItem> lineItemsField;

        private List<OrderAddress> addressesField;

        private List<OrderDiscounts> discountsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Process
        {
            get
            {
                return this.processField;
            }
            set
            {
                this.processField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public OrderHeader Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("LineItem", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<OrderLineItem> LineItems
        {
            get
            {
                return this.lineItemsField;
            }
            set
            {
                this.lineItemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Address", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<OrderAddress> Addresses
        {
            get
            {
                return this.addressesField;
            }
            set
            {
                this.addressesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Address", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<OrderDiscounts> Discounts
        {
            get
            {
                return this.discountsField;
            }
            set
            {
                this.discountsField = value;
            }
        }

        /// <summary>
        /// This method serializes an object model to an xml string.
        /// </summary>
        /// <param name="doc">The invoice document object to be serialized</param>
        /// <returns>XML string representation of the invoice document</returns>
        public string ConvertOrderToString(Order doc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(doc.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, doc);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured serializing the BC.Integration.Invoice Document object model.  The error occured in the ConvertInvoiceToString method.", ex);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }

        /// <summary>
        /// This method is used to deserialize a XML representation of a product to the object model.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public Order ConvertToObjectModel(string doc)
        {
            using (TextReader sr = new StringReader(doc))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Order));
                Order obj = (Order)serializer.Deserialize(sr);
                return obj;
            }

        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://BC.Integration.Schema.ECommerce.SalesChannelOrder.Exchange")]
    public partial class OrderHeader
    {

        private string orderNumberField;

        private int siteIdField;

        private string currencyIdField;

        private string taxRegistrationNumberField;

        private string carrierCodeField;

        private string paymentTypeField;

        private string customerIdField;

        private string customerPONumField;

        //private string shipAddIdField;

        //private string shipCustNameField;

        //private string shipCustAdd1Field;

        //private string shipCustAdd2Field;

        //private string shipCustAdd3Field;

        //private string shipCityField;

        //private string shipPostalCodeField;

        //private string shipCountryField;

        //private string shipPhoneField;

        //private string billAddIdField;

        //private string billCustNameField;

        //private string billCustAdd1Field;

        //private string billCustAdd2Field;

        //private string billCustAdd3Field;

        //private string billCityField;

        //private string billPostalCodeField;

        //private string billCountryField;

        //private string billPhoneField;

        private string custEmailField;

        private System.DateTime orderDateField;

        private System.DateTime quoteExpirationDateField;

        private System.DateTime cancelDateField;

        private System.DateTime reqShipDateField;

        private string paymentMethodField;

        private string commentField;

        private string incoTermsField;

        private decimal discountField;

        private string discountCodeField;

        private string priceCodeField;

        private decimal freightField;

        private decimal taxAmountField;

        private bool isStaffOrderField;

        private bool itHasGiftCardField;

        private bool itHasDiscountField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string OrderNumber
        {
            get
            {
                return this.orderNumberField;
            }
            set
            {
                this.orderNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int SiteId
        {
            get
            {
                return this.siteIdField;
            }
            set
            {
                this.siteIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CurrencyId
        {
            get
            {
                return this.currencyIdField;
            }
            set
            {
                this.currencyIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TaxRegistrationNumber
        {
            get
            {
                return this.taxRegistrationNumberField;
            }
            set
            {
                this.taxRegistrationNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CarrierCode
        {
            get
            {
                return this.carrierCodeField;
            }
            set
            {
                this.carrierCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PaymentType
        {
            get
            {
                return this.paymentTypeField;
            }
            set
            {
                this.paymentTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CustomerId
        {
            get
            {
                return this.customerIdField;
            }
            set
            {
                this.customerIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CustomerPONum
        {
            get
            {
                return this.customerPONumField;
            }
            set
            {
                this.customerPONumField = value;
            }
        }

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipAddId
        //{
        //    get
        //    {
        //        return this.shipAddIdField;
        //    }
        //    set
        //    {
        //        this.shipAddIdField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipCustName
        //{
        //    get
        //    {
        //        return this.shipCustNameField;
        //    }
        //    set
        //    {
        //        this.shipCustNameField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipCustAdd1
        //{
        //    get
        //    {
        //        return this.shipCustAdd1Field;
        //    }
        //    set
        //    {
        //        this.shipCustAdd1Field = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipCustAdd2
        //{
        //    get
        //    {
        //        return this.shipCustAdd2Field;
        //    }
        //    set
        //    {
        //        this.shipCustAdd2Field = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipCustAdd3
        //{
        //    get
        //    {
        //        return this.shipCustAdd3Field;
        //    }
        //    set
        //    {
        //        this.shipCustAdd3Field = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipCity
        //{
        //    get
        //    {
        //        return this.shipCityField;
        //    }
        //    set
        //    {
        //        this.shipCityField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipPostalCode
        //{
        //    get
        //    {
        //        return this.shipPostalCodeField;
        //    }
        //    set
        //    {
        //        this.shipPostalCodeField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipCountry
        //{
        //    get
        //    {
        //        return this.shipCountryField;
        //    }
        //    set
        //    {
        //        this.shipCountryField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string ShipPhone
        //{
        //    get
        //    {
        //        return this.shipPhoneField;
        //    }
        //    set
        //    {
        //        this.shipPhoneField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillAddId
        //{
        //    get
        //    {
        //        return this.billAddIdField;
        //    }
        //    set
        //    {
        //        this.billAddIdField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillCustName
        //{
        //    get
        //    {
        //        return this.billCustNameField;
        //    }
        //    set
        //    {
        //        this.billCustNameField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillCustAdd1
        //{
        //    get
        //    {
        //        return this.billCustAdd1Field;
        //    }
        //    set
        //    {
        //        this.billCustAdd1Field = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillCustAdd2
        //{
        //    get
        //    {
        //        return this.billCustAdd2Field;
        //    }
        //    set
        //    {
        //        this.billCustAdd2Field = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillCustAdd3
        //{
        //    get
        //    {
        //        return this.billCustAdd3Field;
        //    }
        //    set
        //    {
        //        this.billCustAdd3Field = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillCity
        //{
        //    get
        //    {
        //        return this.billCityField;
        //    }
        //    set
        //    {
        //        this.billCityField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillPostalCode
        //{
        //    get
        //    {
        //        return this.billPostalCodeField;
        //    }
        //    set
        //    {
        //        this.billPostalCodeField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillCountry
        //{
        //    get
        //    {
        //        return this.billCountryField;
        //    }
        //    set
        //    {
        //        this.billCountryField = value;
        //    }
        //}

        ///// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //public string BillPhone
        //{
        //    get
        //    {
        //        return this.billPhoneField;
        //    }
        //    set
        //    {
        //        this.billPhoneField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CustEmail
        {
            get
            {
                return this.custEmailField;
            }
            set
            {
                this.custEmailField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime OrderDate
        {
            get
            {
                return this.orderDateField;
            }
            set
            {
                this.orderDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime QuoteExpirationDate
        {
            get
            {
                return this.quoteExpirationDateField;
            }
            set
            {
                this.quoteExpirationDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime CancelDate
        {
            get
            {
                return this.cancelDateField;
            }
            set
            {
                this.cancelDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime ReqShipDate
        {
            get
            {
                return this.reqShipDateField;
            }
            set
            {
                this.reqShipDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PaymentMethod
        {
            get
            {
                return this.paymentMethodField;
            }
            set
            {
                this.paymentMethodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IncoTerms
        {
            get
            {
                return this.incoTermsField;
            }
            set
            {
                this.incoTermsField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Discount
        {
            get
            {
                return this.discountField;
            }
            set
            {
                this.discountField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DiscountCode
        {
            get
            {
                return this.discountCodeField;
            }
            set
            {
                this.discountCodeField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PriceCode
        {
            get
            {
                return this.priceCodeField;
            }
            set
            {
                this.priceCodeField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Freight
        {
            get
            {
                return this.freightField;
            }
            set
            {
                this.freightField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal TaxAmount
        {
            get
            {
                return this.taxAmountField;
            }
            set
            {
                this.taxAmountField = value;
            }
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool isStaffOrder
        {
            get
            {
                return this.isStaffOrderField;
            }
            set
            {
                this.isStaffOrderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool itHasGiftCard
        {
            get
            {
                return this.itHasGiftCardField;
            }
            set
            {
                this.itHasGiftCardField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool itHasDiscount
        {
            get
            {
                return this.itHasDiscountField;
            }
            set
            {
                this.itHasDiscountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://BC.Integration.Schema.ECommerce.SalesChannelOrder.Exchange")]
    public partial class OrderLineItem
    {

        private string lineSeqNumField;

        private string itemNumberField;

        private string unitOfMeasureField;

        private int unitQuantityField;

        //private bool unitQuantityFieldSpecified;

        private decimal unitPriceField;

        //private bool unitPriceFieldSpecified;

        private string commentField;

        private System.DateTime shipDateField;

        private bool shipDateFieldSpecified;

        private string categoryField;

        private string productNameField;

        private string sizeField;

        private string colourField;

        private string fabricField;

        private int siteIdField;

        private string discountCodeField;

        private string taxExemptField;

        private string upcField;

        private bool isGiftCardField;

        private bool isDiscountField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LineSeqNum
        {
            get
            {
                return this.lineSeqNumField;
            }
            set
            {
                this.lineSeqNumField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ItemNumber
        {
            get
            {
                return this.itemNumberField;
            }
            set
            {
                this.itemNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UnitOfMeasure
        {
            get
            {
                return this.unitOfMeasureField;
            }
            set
            {
                this.unitOfMeasureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int UnitQuantity
        {
            get
            {
                return this.unitQuantityField;
            }
            set
            {
                this.unitQuantityField = value;
            }
        }

        ///// <remarks/>
        //[System.Xml.Serialization.XmlIgnoreAttribute()]
        //public bool UnitQuantitySpecified
        //{
        //    get
        //    {
        //        return this.unitQuantityFieldSpecified;
        //    }
        //    set
        //    {
        //        this.unitQuantityFieldSpecified = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal UnitPrice
        {
            get
            {
                return this.unitPriceField;
            }
            set
            {
                this.unitPriceField = value;
            }
        }

        ///// <remarks/>
        //[System.Xml.Serialization.XmlIgnoreAttribute()]
        //public bool UnitPriceSpecified
        //{
        //    get
        //    {
        //        return this.unitPriceFieldSpecified;
        //    }
        //    set
        //    {
        //        this.unitPriceFieldSpecified = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
        public System.DateTime ShipDate
        {
            get
            {
                return this.shipDateField;
            }
            set
            {
                this.shipDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ShipDateSpecified
        {
            get
            {
                return this.shipDateFieldSpecified;
            }
            set
            {
                this.shipDateFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ProductName
        {
            get
            {
                return this.productNameField;
            }
            set
            {
                this.productNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Colour
        {
            get
            {
                return this.colourField;
            }
            set
            {
                this.colourField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Fabric
        {
            get
            {
                return this.fabricField;
            }
            set
            {
                this.fabricField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int SiteId
        {
            get
            {
                return this.siteIdField;
            }
            set
            {
                this.siteIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DiscountCode
        {
            get
            {
                return this.discountCodeField;
            }
            set
            {
                this.discountCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TaxExempt
        {
            get
            {
                return this.taxExemptField;
            }
            set
            {
                this.taxExemptField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UPC
        {
            get
            {
                return this.upcField;
            }
            set
            {
                this.upcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool isGiftCard
        {
            get
            {
                return this.isGiftCardField;
            }
            set
            {
                this.isGiftCardField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool isDiscount
        {
            get
            {
                return this.isDiscountField;
            }
            set
            {
                this.isDiscountField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://BC.Integration.Schema.ECommerce.SalesChannelOrder.Exchange")]
    public partial class OrderAddress
    {

        private string addressIdField;

        private string customerNameField;

        private string add1Field;

        private string add2Field;

        private string add3Field;

        private string addCityField;

        private string addPostalCodeField;

        private string countryField;

        private string phoneField;

        private string emailField;

        private string addressType;

        private string stateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddressId
        {
            get
            {
                return this.addressIdField;
            }
            set
            {
                this.addressIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CustomerName
        {
            get
            {
                return this.customerNameField;
            }
            set
            {
                this.customerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Add1
        {
            get
            {
                return this.add1Field;
            }
            set
            {
                this.add1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Add2
        {
            get
            {
                return this.add2Field;
            }
            set
            {
                this.add2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Add3
        {
            get
            {
                return this.add3Field;
            }
            set
            {
                this.add3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddCity
        {
            get
            {
                return this.addCityField;
            }
            set
            {
                this.addCityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddPostalCode
        {
            get
            {
                return this.addPostalCodeField;
            }
            set
            {
                this.addPostalCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddressType
        {
            get
            {
                return this.addressType;
            }
            set
            {
                this.addressType = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://BC.Integration.Schema.ECommerce.SalesChannelOrder.Exchange")]
    public partial class OrderDiscounts
    {
        private string discountCodeField;
        private string discountItemNumberField;
        private string displayNameField;
        private string promoNameField;


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DiscountCode
        {
            get
            {
                return this.discountCodeField;
            }
            set
            {
                this.discountCodeField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DiscountItemNumber
        {
            get
            {
                return this.discountItemNumberField;
            }
            set
            {
                this.discountItemNumberField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PromoName
        {
            get
            {
                return this.promoNameField;
            }
            set
            {
                this.promoNameField = value;
            }
        }
    }
    }
