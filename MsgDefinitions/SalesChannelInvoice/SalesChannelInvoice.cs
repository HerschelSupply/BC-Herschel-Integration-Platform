
namespace BC.Integration.Canonical.RetailChannel
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;


    ///Class Decoratives <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://BC.Integration.Schema.ECommerce.RetailChannelInvoice")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BC.Integration.Schema.ECommerce.RetailChannelInvoice", IsNullable=false)]
    public partial class Invoice {
        
        private string processField;
        
        private InvoiceHeader headerField;
        
        private List<InvoiceLineItem> lineItemsField;
        
        private InvoiceCustomer customerField;

        private List<InvoicePayment> paymentItemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Process {
            get {
                return this.processField;
            }
            set {
                this.processField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public InvoiceHeader Header {
            get {
                return this.headerField;
            }
            set {
                this.headerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("LineItem", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public List<InvoiceLineItem> LineItems {
            get {
                return this.lineItemsField;
            }
            set {
                this.lineItemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public InvoiceCustomer Customer {
            get {
                return this.customerField;
            }
            set {
                this.customerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("LineItem", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<InvoicePayment> Payments
        {
            get
            {
                return this.paymentItemsField;
            }
            set
            {
                this.paymentItemsField = value;
            }
        }

        /// <summary>
        /// This method serializes an object model to an xml string.
        /// </summary>
        /// <param name="doc">The invoice document object to be serialized</param>
        /// <returns>XML string representation of the invoice document</returns>
        public string ConvertInvoiceToString(Invoice doc)
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
                throw new Exception("Error occured serializing the Corp.Integration.Invoice Document object model.  The error occured in the ConvertInvoiceToString method.", ex);
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
        public Invoice ConvertToObjectModel(string doc)
        {
            using (TextReader sr = new StringReader(doc))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Invoice));
                Invoice obj = (Invoice)serializer.Deserialize(sr);
                return obj;
            }

        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://Corp.Integration.Schema.ECommerce.RetailChannelInvoice")]
    public partial class InvoiceHeader {
        
        private string invoiceNumberField;
        
        private System.DateTime invoiceDateField;

        private System.DateTime invoiceTimeField;

        private string invoiceTypeField;
        
        private bool exchangeField;

        private bool giftCardField;

        private string customerPoField;
        
        private string currencyField;
        
        private string siteIdField;

        private string shopIdField;
        
        private System.DateTime shipmentDateField;
        
        private decimal freightCostField;
        
        private decimal freightTaxField;
        
        private decimal freightTotalField;
        
        private decimal totalLineItemAmountBeforeTaxField;
        
        private decimal totalTaxField;
        
        private decimal totalAmountField;

        private string slipCodeField;

        public InvoiceHeader() {
            this.invoiceTypeField = "SalesInvoice";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string InvoiceNumber {
            get {
                return this.invoiceNumberField;
            }
            set {
                this.invoiceNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime InvoiceDate {
            get {
                return this.invoiceDateField;
            }
            set {
                this.invoiceDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "time")]
        public System.DateTime InvoiceTime
        {
            get
            {
                return this.invoiceTimeField;
            }
            set
            {
                this.invoiceTimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string InvoiceType {
            get {
                return this.invoiceTypeField;
            }
            set {
                this.invoiceTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool Exchange {
            get {
                return this.exchangeField;
            }
            set {
                this.exchangeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool GiftCard
        {
            get
            {
                return this.giftCardField;
            }
            set
            {
                this.giftCardField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CustomerPo {
            get {
                return this.customerPoField;
            }
            set {
                this.customerPoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Currency {
            get {
                return this.currencyField;
            }
            set {
                this.currencyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SiteId {
            get {
                return this.siteIdField;
            }
            set {
                this.siteIdField = value;
            }
        }
        
        /// <summary>
        /// This field is used to store the Lightspeed POS shop ID.  The value is used in the on-ramp to resolve the site ID.
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ShopId {
            get {
                return this.shopIdField;
            }
            set {
                this.shopIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime ShipmentDate {
            get {
                return this.shipmentDateField;
            }
            set {
                this.shipmentDateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal FreightCost
        {
            get {
                return this.freightCostField;
            }
            set {
                this.freightCostField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal FreightTax
        {
            get {
                return this.freightTaxField;
            }
            set {
                this.freightTaxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal FreightTotal
        {
            get {
                return this.freightTotalField;
            }
            set {
                this.freightTotalField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal TotalLineItemAmountBeforeTax {
            get {
                return this.totalLineItemAmountBeforeTaxField;
            }
            set {
                this.totalLineItemAmountBeforeTaxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal TotalTax {
            get {
                return this.totalTaxField;
            }
            set {
                this.totalTaxField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal TotalAmount {
            get {
                return this.totalAmountField;
            }
            set {
                this.totalAmountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SlipCode
        {
            get
            {
                return this.slipCodeField;
            }
            set
            {
                this.slipCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://Corp.Integration.Schema.ECommerce.RetailChannelInvoice")]
    public partial class InvoiceLineItem {
        
        private string itemNumberField;

        private string itemDescription;

        private decimal unitPriceField;

        private decimal actualPriceField;
        
        private int quantityField;
        
        private decimal discountField;
        
        private decimal extendedPriceField;
        
        private decimal lineItemTaxesField;
        
        private decimal lineItemTotalAmountField;
        
        private int returnedInPerfectConditionField;

        private int returnedDamagedField;
        
        private int returnedInUnknownConditionField;

        private List<InvoiceLineItemTax> taxesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ItemNumber
        {
            get {
                return this.itemNumberField;
            }
            set {
                this.itemNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ItemDescription
        {
            get
            {
                return this.itemDescription;
            }
            set
            {
                this.itemDescription = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal UnitPrice {
            get {
                return this.unitPriceField;
            }
            set {
                this.unitPriceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ActualPrice
        {
            get {
                return this.actualPriceField;
            }
            set {
                this.actualPriceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int Quantity {
            get {
                return this.quantityField;
            }
            set {
                this.quantityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Discount {
            get {
                return this.discountField;
            }
            set {
                this.discountField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal ExtendedPrice
        {
            get {
                return this.extendedPriceField;
            }
            set {
                this.extendedPriceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal LineItemTaxes {
            get {
                return this.lineItemTaxesField;
            }
            set {
                this.lineItemTaxesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal LineItemTotalAmount {
            get {
                return this.lineItemTotalAmountField;
            }
            set {
                this.lineItemTotalAmountField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int ReturnedInPerfectCondition {
            get {
                return this.returnedInPerfectConditionField;
            }
            set {
                this.returnedInPerfectConditionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int ReturnedDamaged
        {
            get
            {
                return this.returnedDamagedField;
            }
            set
            {
                this.returnedDamagedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int ReturnedInUnknownCondition
        {
            get
            {
                return this.returnedInUnknownConditionField;
            }
            set
            {
                this.returnedInUnknownConditionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Tax", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public List<InvoiceLineItemTax> Taxes {
            get {
                return this.taxesField;
            }
            set {
                this.taxesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://Corp.Integration.Schema.ECommerce.RetailChannelInvoice")]
    public partial class InvoiceLineItemTax {
        
        private string taxCodeField;
        
        private decimal taxAmountField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TaxCode {
            get {
                return this.taxCodeField;
            }
            set {
                this.taxCodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal TaxAmount {
            get {
                return this.taxAmountField;
            }
            set {
                this.taxAmountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://Corp.Integration.Schema.ECommerce.RetailChannelInvoice")]
    public partial class InvoiceCustomer {
        
        private string customerIdField;
        
        private string nameField;
        
        private string emailAddressField;
        
        private string telephoneNumberField;
        
        private string paymentTermsField;
        
        private InvoiceCustomerShipToAddress shipToAddressField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CustomerId
        {
            get {
                return this.customerIdField;
            }
            set {
                this.customerIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EmailAddress {
            get {
                return this.emailAddressField;
            }
            set {
                this.emailAddressField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TelephoneNumber {
            get {
                return this.telephoneNumberField;
            }
            set {
                this.telephoneNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PaymentTerms {
            get {
                return this.paymentTermsField;
            }
            set {
                this.paymentTermsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public InvoiceCustomerShipToAddress ShipToAddress {
            get {
                return this.shipToAddressField;
            }
            set {
                this.shipToAddressField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://Corp.Integration.Schema.ECommerce.RetailChannelInvoice")]
    public partial class InvoiceCustomerShipToAddress {
        
        private string addressIdField;
        
        private string nameField;
        
        private string street1Field;
        
        private string street2Field;
        
        private string cityField;
        
        private string stateField;
        
        private string zipcodeField;
        
        private string countryField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddressId
        {
            get {
                return this.addressIdField;
            }
            set {
                this.addressIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Street1 {
            get {
                return this.street1Field;
            }
            set {
                this.street1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Street2 {
            get {
                return this.street2Field;
            }
            set {
                this.street2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string State {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Zipcode {
            get {
                return this.zipcodeField;
            }
            set {
                this.zipcodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Country {
            get {
                return this.countryField;
            }
            set {
                this.countryField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://Corp.Integration.Schema.ECommerce.RetailChannelInvoice")]
    public partial class InvoicePayment
    {

        private string paymentIdField;
        /// <summary>
        ///   1. CASH
        ///        - Cash
        ///        - Debit Card
        ///   2. CHECK
        ///        - Check
        ///   3. CREDIT
        ///        - Credit Card
        ///        - Debit Card
        ///        - Discover Card
        ///        - Master Card
        ///        - Visa
        /// </summary>
        private string methodTypeField;

        /// <summary>
        /// Visa, Mastercard, cash ...etc
        /// </summary>
        private string methodField;

        private decimal amountField;
        /// <summary>
        /// Reference number from payment institution
        /// </summary>
        private string confirmationNumberField;
        /// <summary>
        /// Timestamp of the payment transaction
        /// </summary>
        private DateTime transactionDateField;
        /// <summary>
        /// Completed, Failed, Authorized
        /// </summary>
        private string transactionStatusField;
        /// <summary>
        /// Maybe needed for some forms of token based payment processes
        /// </summary>
        private string requestTokenField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PaymentId
        {
            get
            {
                return this.paymentIdField;
            }
            set
            {
                this.paymentIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PaymentMethodType
        {
            get
            {
                return this.methodTypeField;
            }
            set
            {
                this.methodTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PaymentMethod
        {
            get
            {
                return this.methodField;
            }
            set
            {
                this.methodField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public decimal Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ConfirmationNumber
        {
            get
            {
                return this.confirmationNumberField;
            }
            set
            {
                this.confirmationNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime TransactionDate
        {
            get
            {
                return this.transactionDateField;
            }
            set
            {
                this.transactionDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TransactionStatus
        {
            get
            {
                return this.transactionStatusField;
            }
            set
            {
                this.transactionStatusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RequestToken
        {
            get
            {
                return this.requestTokenField;
            }
            set
            {
                this.requestTokenField = value;
            }
        }

    }

}


