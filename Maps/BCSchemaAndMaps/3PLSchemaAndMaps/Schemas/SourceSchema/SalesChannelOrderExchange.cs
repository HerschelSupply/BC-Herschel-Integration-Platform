namespace Corp.Integration.Maps.ECommerce {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://Corp.Integration.Schema.ECommerce.SalesChannelOrder",@"Order")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Order"})]
    public sealed class SalesChannelOrder : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://Corp.Integration.Schema.ECommerce.SalesChannelOrder"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" targetNamespace=""http://Corp.Integration.Schema.ECommerce.SalesChannelOrder"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:annotation>
    <xs:appinfo>
      <b:schemaInfo version=""1.0"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" />
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""Order"">
    <xs:complexType>
      <xs:sequence>
        <xs:element minOccurs=""1"" maxOccurs=""1"" name=""Process"" type=""xs:string"" />
        <xs:element name=""Header"">
          <xs:complexType>
            <xs:sequence>
              <xs:element minOccurs=""0"" maxOccurs=""1"" name=""OrderNumber"" type=""xs:string"" />
              <xs:element name=""SiteId"" type=""xs:int"" />
              <xs:element name=""CurrencyId"" type=""xs:string"" />
              <xs:element name=""TaxRegistrationNumber"" type=""xs:string"" />
              <xs:element name=""CarrierCode"" type=""xs:string"" />
              <xs:element name=""PaymentType"" type=""xs:string"" />
              <xs:element name=""CustomerId"" type=""xs:string"" />
              <xs:element name=""CustomerPONum"" type=""xs:string"" />
              <xs:element name=""ShipAddId"" type=""xs:string"" />
              <xs:element name=""ShipCustName"" type=""xs:string"" />
              <xs:element name=""ShipCustAdd1"" type=""xs:string"" />
              <xs:element name=""ShipCustAdd2"" type=""xs:string"" />
              <xs:element name=""ShipCustAdd3"" type=""xs:string"" />
              <xs:element name=""ShipCity"" type=""xs:string"" />
              <xs:element name=""ShipPostalCode"" type=""xs:string"" />
              <xs:element name=""ShipCountry"" type=""xs:string"" />
              <xs:element name=""ShipPhone"" type=""xs:string"" />
              <xs:element name=""BillAddId"" type=""xs:string"" />
              <xs:element name=""BillCustName"" type=""xs:string"" />
              <xs:element name=""BillCustAdd1"" type=""xs:string"" />
              <xs:element name=""BillCustAdd2"" type=""xs:string"" />
              <xs:element name=""BillCustAdd3"" type=""xs:string"" />
              <xs:element name=""BillCity"" type=""xs:string"" />
              <xs:element name=""BillPostalCode"" type=""xs:string"" />
              <xs:element name=""BillCountry"" type=""xs:string"" />
              <xs:element name=""BillPhone"" type=""xs:string"" />
              <xs:element name=""CustEmail"" type=""xs:string"" />
              <xs:element minOccurs=""1"" maxOccurs=""1"" name=""OrderDate"" type=""xs:date"" />
              <xs:element name=""QuoteExpirationDate"" type=""xs:date"" />
              <xs:element name=""CancelDate"" type=""xs:date"" />
              <xs:element name=""ReqShipDate"" type=""xs:date"" />
              <xs:element name=""PaymentMethod"" type=""xs:string"" />
              <xs:element name=""Comment"" type=""xs:string"" />
              <xs:element name=""IncoTerms"" type=""xs:string"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
        <xs:element minOccurs=""1"" maxOccurs=""1"" name=""LineItems"">
          <xs:complexType>
            <xs:sequence minOccurs=""0"" maxOccurs=""unbounded"">
              <xs:element minOccurs=""1"" maxOccurs=""unbounded"" name=""LineItem"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""LineSeqNum"" type=""xs:string"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""ItemNumber"" type=""xs:string"" />
                    <xs:element name=""UnitOfMeasure"" type=""xs:string"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""UnitQuantity"" type=""xs:int"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""UnitPrice"" type=""xs:decimal"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""Comment"" type=""xs:string"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""ShipDate"" type=""xs:date"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""Category"" type=""xs:string"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""ProductName"" type=""xs:string"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""Size"" type=""xs:string"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""Colour"" type=""xs:string"" />
                    <xs:element minOccurs=""0"" maxOccurs=""1"" name=""Fabric"" type=""xs:string"" />
                    <xs:element minOccurs=""1"" maxOccurs=""1"" name=""SiteId"" type=""xs:int"" />
                  </xs:sequence>
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
        <xs:element minOccurs=""1"" maxOccurs=""1"" name=""Addresses"">
          <xs:complexType>
            <xs:sequence minOccurs=""0"" maxOccurs=""unbounded"">
              <xs:element minOccurs=""1"" maxOccurs=""unbounded"" name=""Address"">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name=""AddressId"" type=""xs:string"" />
                    <xs:element name=""CustomerName"" type=""xs:string"" />
                    <xs:element name=""Add1"" type=""xs:string"" />
                    <xs:element name=""Add2"" type=""xs:string"" />
                    <xs:element name=""Add3"" type=""xs:string"" />
                    <xs:element name=""AddCity"" type=""xs:string"" />
                    <xs:element name=""AddPostalCode"" type=""xs:string"" />
                    <xs:element name=""Country"" type=""xs:string"" />
                    <xs:element name=""Phone"" type=""xs:string"" />
                    <xs:element name=""Email"" type=""xs:string"" />
                    <xs:element name=""AddressType"" type=""xs:string"" />
                  </xs:sequence>
                </xs:complexType>
              </xs:element>
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public SalesChannelOrder() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "Order";
                return _RootElements;
            }
        }
        
        protected override object RawSchema {
            get {
                return _rawSchema;
            }
            set {
                _rawSchema = value;
            }
        }
    }
}
