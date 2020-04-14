using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Corp.Integration.AppService.Gp
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("eConnect", Namespace = "", IsNullable = false)]
    public partial class EConnect
    {
        private EConnectSOPTransactionType itemField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SOPTransactionType", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public EConnectSOPTransactionType SopTransaction
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        public string ConvertToString(EConnect doc)
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
                throw new Exception("Error occured serializing the EConnect object model.  The error occured in the ConvertToString method of the Corp.Integration.AppService.GP.EConnect class.", ex);
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

        public EConnect ConvertToObjectModel(string doc)
        {
            using (TextReader sr = new StringReader(doc))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(EConnect));
                EConnect msgEnv = (EConnect)serializer.Deserialize(sr);
                return msgEnv;
            }

        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionType
    {

        private EConnectSOPTransactionTypeTaSopHdrIvcInsert taSopHdrIvcInsertField;

        private EConnectSOPTransactionTypeTaSopUserDefined taSopUserDefinedField;

        private EConnectSOPTransactionTypeTaCreateCustomerAddress_ItemsTaCreateCustomerAddress taCreateCustomerAddress_ItemsField;

        private EConnectSOPTransactionTypeTaCreateInternetAddresses[] taCreateInternetAddressesField;

        private EConnectSOPTransactionTypeTaSopLineIvcInsert_ItemsTaSopLineIvcInsert[] taSopLineIvcInsert_ItemsField;

        private EConnectSOPTransactionTypeTaSopLineIvcTaxInsert_ItemsTaSopLineIvcTaxInsert[] taSopLineIvcTaxInsert_ItemsField;

        private EConnectSOPTransactionTypeTaSopVoidDocument taSopVoidDocumentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("taSopHdrIvcInsert", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public EConnectSOPTransactionTypeTaSopHdrIvcInsert taSopHdrIvcInsert
        {
            get
            {
                return this.taSopHdrIvcInsertField;
            }
            set
            {
                this.taSopHdrIvcInsertField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("taSopUserDefined", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public EConnectSOPTransactionTypeTaSopUserDefined taSopUserDefined
        {
            get
            {
                return this.taSopUserDefinedField;
            }
            set
            {
                this.taSopUserDefinedField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("taCreateCustomerAddress", typeof(EConnectSOPTransactionTypeTaCreateCustomerAddress_ItemsTaCreateCustomerAddress), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public EConnectSOPTransactionTypeTaCreateCustomerAddress_ItemsTaCreateCustomerAddress taCreateCustomerAddress_Items
        {
            get
            {
                return this.taCreateCustomerAddress_ItemsField;
            }
            set
            {
                this.taCreateCustomerAddress_ItemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("taCreateInternetAddresses", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public EConnectSOPTransactionTypeTaCreateInternetAddresses[] taCreateInternetAddresses
        {
            get
            {
                return this.taCreateInternetAddressesField;
            }
            set
            {
                this.taCreateInternetAddressesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("taSopLineIvcInsert", typeof(EConnectSOPTransactionTypeTaSopLineIvcInsert_ItemsTaSopLineIvcInsert), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public EConnectSOPTransactionTypeTaSopLineIvcInsert_ItemsTaSopLineIvcInsert[] taSopLineIvcInsert_Items
        {
            get
            {
                return this.taSopLineIvcInsert_ItemsField;
            }
            set
            {
                this.taSopLineIvcInsert_ItemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("taSopLineIvcTaxInsert", typeof(EConnectSOPTransactionTypeTaSopLineIvcTaxInsert_ItemsTaSopLineIvcTaxInsert), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public EConnectSOPTransactionTypeTaSopLineIvcTaxInsert_ItemsTaSopLineIvcTaxInsert[] taSopLineIvcTaxInsert_Items
        {
            get
            {
                return this.taSopLineIvcTaxInsert_ItemsField;
            }
            set
            {
                this.taSopLineIvcTaxInsert_ItemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("taSopVoidDocument", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public EConnectSOPTransactionTypeTaSopVoidDocument taSopVoidDocument
        {
            get
            {
                return this.taSopVoidDocumentField;
            }
            set
            {
                this.taSopVoidDocumentField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionTypeTaSopHdrIvcInsert
    {

        private string sOPTYPEField;

        private string dOCIDField;

        private string sOPNUMBEField;

        private string tAXSCHIDField;

        private string sHIPMTHDField;

        private string tAXAMNTField;

        private string lOCNCODEField;

        private string dOCDATEField;

        private string fREIGHTField;

        private string cUSTNMBRField;

        private string cUSTNAMEField;

        private string cSTPONBRField;

        private string shipToNameField;

        private string bACHNUMBField;

        private string aDDRESS1Field;

        private string aDDRESS2Field;

        private string cNTCPRSNField;

        private string cITYField;

        private string sTATEField;

        private string zIPCODEField;

        private string cOUNTRYField;

        private string print_Phone_NumberGBField;

        private string pHNUMBR1Field;

        private string sUBTOTALField;

        private string pRBTADCDField;

        private string pRSTADCDField;

        private string oRDRDATEField;

        private string dUEDATEField;

        private string uSINGHEADERLEVELTAXESField;

        private string cREATETAXESField;

        private string dEFTAXSCHDSField;

        private string cURNCYIDField;

        private string cOMMENT_1Field;

        private string reqShipDateField;

        private string updateExistingField;

        private string pRCLEVELField;

        private string dEFPRICINGField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPTYPE
        {
            get
            {
                return this.sOPTYPEField;
            }
            set
            {
                this.sOPTYPEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DOCID
        {
            get
            {
                return this.dOCIDField;
            }
            set
            {
                this.dOCIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPNUMBE
        {
            get
            {
                return this.sOPNUMBEField;
            }
            set
            {
                this.sOPNUMBEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TAXSCHID
        {
            get
            {
                return this.tAXSCHIDField;
            }
            set
            {
                this.tAXSCHIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SHIPMTHD
        {
            get
            {
                return this.sHIPMTHDField;
            }
            set
            {
                this.sHIPMTHDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TAXAMNT
        {
            get
            {
                return this.tAXAMNTField;
            }
            set
            {
                this.tAXAMNTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LOCNCODE
        {
            get
            {
                return this.lOCNCODEField;
            }
            set
            {
                this.lOCNCODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DOCDATE
        {
            get
            {
                return this.dOCDATEField;
            }
            set
            {
                this.dOCDATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string FREIGHT
        {
            get
            {
                return this.fREIGHTField;
            }
            set
            {
                this.fREIGHTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CUSTNMBR
        {
            get
            {
                return this.cUSTNMBRField;
            }
            set
            {
                this.cUSTNMBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CUSTNAME
        {
            get
            {
                return this.cUSTNAMEField;
            }
            set
            {
                this.cUSTNAMEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CSTPONBR
        {
            get
            {
                return this.cSTPONBRField;
            }
            set
            {
                this.cSTPONBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ShipToName
        {
            get
            {
                return this.shipToNameField;
            }
            set
            {
                this.shipToNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BACHNUMB
        {
            get
            {
                return this.bACHNUMBField;
            }
            set
            {
                this.bACHNUMBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADDRESS1
        {
            get
            {
                return this.aDDRESS1Field;
            }
            set
            {
                this.aDDRESS1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADDRESS2
        {
            get
            {
                return this.aDDRESS2Field;
            }
            set
            {
                this.aDDRESS2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CNTCPRSN
        {
            get
            {
                return this.cNTCPRSNField;
            }
            set
            {
                this.cNTCPRSNField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CITY
        {
            get
            {
                return this.cITYField;
            }
            set
            {
                this.cITYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STATE
        {
            get
            {
                return this.sTATEField;
            }
            set
            {
                this.sTATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ZIPCODE
        {
            get
            {
                return this.zIPCODEField;
            }
            set
            {
                this.zIPCODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COUNTRY
        {
            get
            {
                return this.cOUNTRYField;
            }
            set
            {
                this.cOUNTRYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Print_Phone_NumberGB
        {
            get
            {
                return this.print_Phone_NumberGBField;
            }
            set
            {
                this.print_Phone_NumberGBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PHNUMBR1
        {
            get
            {
                return this.pHNUMBR1Field;
            }
            set
            {
                this.pHNUMBR1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SUBTOTAL
        {
            get
            {
                return this.sUBTOTALField;
            }
            set
            {
                this.sUBTOTALField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PRBTADCD
        {
            get
            {
                return this.pRBTADCDField;
            }
            set
            {
                this.pRBTADCDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PRSTADCD
        {
            get
            {
                return this.pRSTADCDField;
            }
            set
            {
                this.pRSTADCDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ORDRDATE
        {
            get
            {
                return this.oRDRDATEField;
            }
            set
            {
                this.oRDRDATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DUEDATE
        {
            get
            {
                return this.dUEDATEField;
            }
            set
            {
                this.dUEDATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string USINGHEADERLEVELTAXES
        {
            get
            {
                return this.uSINGHEADERLEVELTAXESField;
            }
            set
            {
                this.uSINGHEADERLEVELTAXESField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CREATETAXES
        {
            get
            {
                return this.cREATETAXESField;
            }
            set
            {
                this.cREATETAXESField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DEFTAXSCHDS
        {
            get
            {
                return this.dEFTAXSCHDSField;
            }
            set
            {
                this.dEFTAXSCHDSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CURNCYID
        {
            get
            {
                return this.cURNCYIDField;
            }
            set
            {
                this.cURNCYIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COMMENT_1
        {
            get
            {
                return this.cOMMENT_1Field;
            }
            set
            {
                this.cOMMENT_1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ReqShipDate
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
        public string UpdateExisting
        {
            get
            {
                return this.updateExistingField;
            }
            set
            {
                this.updateExistingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PRCLEVEL
        {
            get
            {
                return this.pRCLEVELField;
            }
            set
            {
                this.pRCLEVELField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DEFPRICING
        {
            get
            {
                return this.dEFPRICINGField;
            }
            set
            {
                this.dEFPRICINGField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionTypeTaSopUserDefined
    {

        private string updateIfExistsField;

        private string uSRDAT01Field;

        private string sOPTYPEField;

        private string sOPNUMBEField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UpdateIfExists
        {
            get
            {
                return this.updateIfExistsField;
            }
            set
            {
                this.updateIfExistsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string USRDAT01
        {
            get
            {
                return this.uSRDAT01Field;
            }
            set
            {
                this.uSRDAT01Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPTYPE
        {
            get
            {
                return this.sOPTYPEField;
            }
            set
            {
                this.sOPTYPEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPNUMBE
        {
            get
            {
                return this.sOPNUMBEField;
            }
            set
            {
                this.sOPNUMBEField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionTypeTaCreateCustomerAddress_ItemsTaCreateCustomerAddress
    {

        private string aDRSCODEField;

        private string cNTCPRSNField;

        private string updateIfExistsField;

        private string aDDRESS1Field;

        private string print_Phone_NumberGBField;

        private string aDDRESS2Field;

        private string cITYField;

        private string sTATEField;

        private string cOUNTRYField;

        private string cCodeField;

        private string zIPCODEField;

        private string cUSTNMBRField;

        private string lOCNCODEField;

        private string pHNUMBR1Field;

        private string shipToNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADRSCODE
        {
            get
            {
                return this.aDRSCODEField;
            }
            set
            {
                this.aDRSCODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CNTCPRSN
        {
            get
            {
                return this.cNTCPRSNField;
            }
            set
            {
                this.cNTCPRSNField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UpdateIfExists
        {
            get
            {
                return this.updateIfExistsField;
            }
            set
            {
                this.updateIfExistsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADDRESS1
        {
            get
            {
                return this.aDDRESS1Field;
            }
            set
            {
                this.aDDRESS1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Print_Phone_NumberGB
        {
            get
            {
                return this.print_Phone_NumberGBField;
            }
            set
            {
                this.print_Phone_NumberGBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADDRESS2
        {
            get
            {
                return this.aDDRESS2Field;
            }
            set
            {
                this.aDDRESS2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CITY
        {
            get
            {
                return this.cITYField;
            }
            set
            {
                this.cITYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STATE
        {
            get
            {
                return this.sTATEField;
            }
            set
            {
                this.sTATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COUNTRY
        {
            get
            {
                return this.cOUNTRYField;
            }
            set
            {
                this.cOUNTRYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CCode
        {
            get
            {
                return this.cCodeField;
            }
            set
            {
                this.cCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ZIPCODE
        {
            get
            {
                return this.zIPCODEField;
            }
            set
            {
                this.zIPCODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CUSTNMBR
        {
            get
            {
                return this.cUSTNMBRField;
            }
            set
            {
                this.cUSTNMBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LOCNCODE
        {
            get
            {
                return this.lOCNCODEField;
            }
            set
            {
                this.lOCNCODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PHNUMBR1
        {
            get
            {
                return this.pHNUMBR1Field;
            }
            set
            {
                this.pHNUMBR1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ShipToName
        {
            get
            {
                return this.shipToNameField;
            }
            set
            {
                this.shipToNameField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionTypeTaCreateInternetAddresses
    {

        private string iNET1Field;

        private string master_TypeField;

        private string master_IDField;

        private string aDRSCODEField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string INET1
        {
            get
            {
                return this.iNET1Field;
            }
            set
            {
                this.iNET1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Master_Type
        {
            get
            {
                return this.master_TypeField;
            }
            set
            {
                this.master_TypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Master_ID
        {
            get
            {
                return this.master_IDField;
            }
            set
            {
                this.master_IDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADRSCODE
        {
            get
            {
                return this.aDRSCODEField;
            }
            set
            {
                this.aDRSCODEField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionTypeTaSopLineIvcInsert_ItemsTaSopLineIvcInsert
    {

        private string iTEMNMBRField;

        private string cOMMENT_3Field;

        private string updateIfExistsField;

        private string cOMMENT_1Field;

        private string qTYFULFIField;

        private string print_Phone_NumberGBField;

        private string pRCLEVELField;

        private string cURNCYIDField;

        private string pRSTADCDField;

        private string shipToNameField;

        private string xTNDPRCEField;

        private string pHONE1Field;

        private string cNTCPRSNField;

        private string sOPTYPEField;

        private string tAXSCHIDField;

        private string cOMMENT_2Field;

        private string lOCNCODEField;

        private string uNITPRCEField;

        private string mRKDNAMTField;

        private string cUSTNMBRField;

        private string dOCIDField;

        private string sOPNUMBEField;

        private string quantityField;

        private string iVITMTXBField;

        private string sHIPMTHDField;

        private string dEFEXTPRICEField;

        private string dEFPRICINGField;

        private string dOCDATEField;

        private string iTEMDESCField;

        private string aDDRESS1Field;

        private string aDDRESS2Field;

        private string cITYField;

        private string sTATEField;

        private string zIPCODEField;

        private string cOUNTRYField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ITEMNMBR
        {
            get
            {
                return this.iTEMNMBRField;
            }
            set
            {
                this.iTEMNMBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COMMENT_3
        {
            get
            {
                return this.cOMMENT_3Field;
            }
            set
            {
                this.cOMMENT_3Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UpdateIfExists
        {
            get
            {
                return this.updateIfExistsField;
            }
            set
            {
                this.updateIfExistsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COMMENT_1
        {
            get
            {
                return this.cOMMENT_1Field;
            }
            set
            {
                this.cOMMENT_1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string QTYFULFI
        {
            get
            {
                return this.qTYFULFIField;
            }
            set
            {
                this.qTYFULFIField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Print_Phone_NumberGB
        {
            get
            {
                return this.print_Phone_NumberGBField;
            }
            set
            {
                this.print_Phone_NumberGBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PRCLEVEL
        {
            get
            {
                return this.pRCLEVELField;
            }
            set
            {
                this.pRCLEVELField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CURNCYID
        {
            get
            {
                return this.cURNCYIDField;
            }
            set
            {
                this.cURNCYIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PRSTADCD
        {
            get
            {
                return this.pRSTADCDField;
            }
            set
            {
                this.pRSTADCDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ShipToName
        {
            get
            {
                return this.shipToNameField;
            }
            set
            {
                this.shipToNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string XTNDPRCE
        {
            get
            {
                return this.xTNDPRCEField;
            }
            set
            {
                this.xTNDPRCEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PHONE1
        {
            get
            {
                return this.pHONE1Field;
            }
            set
            {
                this.pHONE1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CNTCPRSN
        {
            get
            {
                return this.cNTCPRSNField;
            }
            set
            {
                this.cNTCPRSNField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPTYPE
        {
            get
            {
                return this.sOPTYPEField;
            }
            set
            {
                this.sOPTYPEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TAXSCHID
        {
            get
            {
                return this.tAXSCHIDField;
            }
            set
            {
                this.tAXSCHIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COMMENT_2
        {
            get
            {
                return this.cOMMENT_2Field;
            }
            set
            {
                this.cOMMENT_2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LOCNCODE
        {
            get
            {
                return this.lOCNCODEField;
            }
            set
            {
                this.lOCNCODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UNITPRCE
        {
            get
            {
                return this.uNITPRCEField;
            }
            set
            {
                this.uNITPRCEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MRKDNAMT
        {
            get
            {
                return this.mRKDNAMTField;
            }
            set
            {
                this.mRKDNAMTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CUSTNMBR
        {
            get
            {
                return this.cUSTNMBRField;
            }
            set
            {
                this.cUSTNMBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DOCID
        {
            get
            {
                return this.dOCIDField;
            }
            set
            {
                this.dOCIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPNUMBE
        {
            get
            {
                return this.sOPNUMBEField;
            }
            set
            {
                this.sOPNUMBEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IVITMTXB
        {
            get
            {
                return this.iVITMTXBField;
            }
            set
            {
                this.iVITMTXBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SHIPMTHD
        {
            get
            {
                return this.sHIPMTHDField;
            }
            set
            {
                this.sHIPMTHDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DEFEXTPRICE
        {
            get
            {
                return this.dEFEXTPRICEField;
            }
            set
            {
                this.dEFEXTPRICEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DEFPRICING
        {
            get
            {
                return this.dEFPRICINGField;
            }
            set
            {
                this.dEFPRICINGField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DOCDATE
        {
            get
            {
                return this.dOCDATEField;
            }
            set
            {
                this.dOCDATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ITEMDESC
        {
            get
            {
                return this.iTEMDESCField;
            }
            set
            {
                this.iTEMDESCField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADDRESS1
        {
            get
            {
                return this.aDDRESS1Field;
            }
            set
            {
                this.aDDRESS1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ADDRESS2
        {
            get
            {
                return this.aDDRESS2Field;
            }
            set
            {
                this.aDDRESS2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CITY
        {
            get
            {
                return this.cITYField;
            }
            set
            {
                this.cITYField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STATE
        {
            get
            {
                return this.sTATEField;
            }
            set
            {
                this.sTATEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ZIPCODE
        {
            get
            {
                return this.zIPCODEField;
            }
            set
            {
                this.zIPCODEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string COUNTRY
        {
            get
            {
                return this.cOUNTRYField;
            }
            set
            {
                this.cOUNTRYField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionTypeTaSopLineIvcTaxInsert_ItemsTaSopLineIvcTaxInsert
    {

        private string sTAXAMNTField;

        private string lNITMSEQField;

        private string sOPNUMBEField;

        private string tAXDTLIDField;

        private string cUSTNMBRField;

        private string sOPTYPEField;

        private string tDTTXSLSField;

        private string sALESAMTField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string STAXAMNT
        {
            get
            {
                return this.sTAXAMNTField;
            }
            set
            {
                this.sTAXAMNTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LNITMSEQ
        {
            get
            {
                return this.lNITMSEQField;
            }
            set
            {
                this.lNITMSEQField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPNUMBE
        {
            get
            {
                return this.sOPNUMBEField;
            }
            set
            {
                this.sOPNUMBEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TAXDTLID
        {
            get
            {
                return this.tAXDTLIDField;
            }
            set
            {
                this.tAXDTLIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CUSTNMBR
        {
            get
            {
                return this.cUSTNMBRField;
            }
            set
            {
                this.cUSTNMBRField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPTYPE
        {
            get
            {
                return this.sOPTYPEField;
            }
            set
            {
                this.sOPTYPEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TDTTXSLS
        {
            get
            {
                return this.tDTTXSLSField;
            }
            set
            {
                this.tDTTXSLSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SALESAMT
        {
            get
            {
                return this.sALESAMTField;
            }
            set
            {
                this.sALESAMTField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EConnectSOPTransactionTypeTaSopVoidDocument
    {

        private string sOPTYPEField;

        private string sOPNUMBEField;

        private string bACHNUMBField;

        private string removePaymentsField;

        private string vOIDDATEField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPTYPE
        {
            get
            {
                return this.sOPTYPEField;
            }
            set
            {
                this.sOPTYPEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SOPNUMBE
        {
            get
            {
                return this.sOPNUMBEField;
            }
            set
            {
                this.sOPNUMBEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BACHNUMB
        {
            get
            {
                return this.bACHNUMBField;
            }
            set
            {
                this.bACHNUMBField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string RemovePayments
        {
            get
            {
                return this.removePaymentsField;
            }
            set
            {
                this.removePaymentsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string VOIDDATE
        {
            get
            {
                return this.vOIDDATEField;
            }
            set
            {
                this.vOIDDATEField = value;
            }
        }
    }
}
