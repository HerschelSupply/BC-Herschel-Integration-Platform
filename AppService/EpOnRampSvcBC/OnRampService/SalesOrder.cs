using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.Web;

// Class for eConnect object
namespace BC.Integration.AppService.EpOnRampServiceBC
{
        [XmlRoot(ElementName = "taCreateCustomerAddress")]
        public class TaCreateCustomerAddress
        {
            [XmlElement(ElementName = "ADRSCODE")]
            public string ADRSCODE { get; set; }
            [XmlElement(ElementName = "CNTCPRSN")]
            public string CNTCPRSN { get; set; }
            [XmlElement(ElementName = "UpdateIfExists")]
            public string UpdateIfExists { get; set; }
            [XmlElement(ElementName = "ADDRESS1")]
            public string ADDRESS1 { get; set; }
            [XmlElement(ElementName = "Print_Phone_NumberGB")]
            public string Print_Phone_NumberGB { get; set; }
            [XmlElement(ElementName = "ADDRESS2")]
            public string ADDRESS2 { get; set; }
            [XmlElement(ElementName = "CITY")]
            public string CITY { get; set; }
            [XmlElement(ElementName = "STATE")]
            public string STATE { get; set; }
            [XmlElement(ElementName = "COUNTRY")]
            public string COUNTRY { get; set; }
            [XmlElement(ElementName = "CCode")]
            public string CCode { get; set; }
            [XmlElement(ElementName = "ZIPCODE")]
            public string ZIPCODE { get; set; }
            [XmlElement(ElementName = "CUSTNMBR")]
            public string CUSTNMBR { get; set; }
            [XmlElement(ElementName = "LOCNCODE")]
            public string LOCNCODE { get; set; }
            [XmlElement(ElementName = "PHNUMBR1")]
            public string PHNUMBR1 { get; set; }
            [XmlElement(ElementName = "ShipToName")]
            public string ShipToName { get; set; }
        }

        [XmlRoot(ElementName = "taCreateInternetAddresses")]
        public class TaCreateInternetAddresses
        {
            [XmlElement(ElementName = "INET1")]
            public string INET1 { get; set; }
            [XmlElement(ElementName = "Master_Type")]
            public string Master_Type { get; set; }
            [XmlElement(ElementName = "Master_ID")]
            public string Master_ID { get; set; }
            [XmlElement(ElementName = "ADRSCODE")]
            public string ADRSCODE { get; set; }
        }

        [XmlRoot(ElementName = "taSopLineIvcInsert")]
        public class TaSopLineIvcInsert
        {
            [XmlElement(ElementName = "ITEMNMBR")]
            public string ITEMNMBR { get; set; }
            [XmlElement(ElementName = "COMMENT_3")]
            public string COMMENT_3 { get; set; }
            [XmlElement(ElementName = "UpdateIfExists")]
            public string UpdateIfExists { get; set; }
            [XmlElement(ElementName = "COMMENT_1")]
            public string COMMENT_1 { get; set; }
            [XmlElement(ElementName = "QTYFULFI")]
            public string QTYFULFI { get; set; }
            [XmlElement(ElementName = "Print_Phone_NumberGB")]
            public string Print_Phone_NumberGB { get; set; }
            [XmlElement(ElementName = "PRCLEVEL")]
            public string PRCLEVEL { get; set; }
            [XmlElement(ElementName = "CURNCYID")]
            public string CURNCYID { get; set; }
            [XmlElement(ElementName = "PRSTADCD")]
            public string PRSTADCD { get; set; }
            [XmlElement(ElementName = "ShipToName")]
            public string ShipToName { get; set; }
            [XmlElement(ElementName = "XTNDPRCE")]
            public string XTNDPRCE { get; set; }
            [XmlElement(ElementName = "PHONE1")]
            public string PHONE1 { get; set; }
            [XmlElement(ElementName = "CNTCPRSN")]
            public string CNTCPRSN { get; set; }
            [XmlElement(ElementName = "SOPTYPE")]
            public string SOPTYPE { get; set; }
            [XmlElement(ElementName = "TAXSCHID")]
            public string TAXSCHID { get; set; }
            [XmlElement(ElementName = "COMMENT_2")]
            public string COMMENT_2 { get; set; }
            [XmlElement(ElementName = "LOCNCODE")]
            public string LOCNCODE { get; set; }
            [XmlElement(ElementName = "UNITPRCE")]
            public string UNITPRCE { get; set; }
            [XmlElement(ElementName = "MRKDNAMT")]
            public string MRKDNAMT { get; set; }
            [XmlElement(ElementName = "CUSTNMBR")]
            public string CUSTNMBR { get; set; }
            [XmlElement(ElementName = "DOCID")]
            public string DOCID { get; set; }
            [XmlElement(ElementName = "SOPNUMBE")]
            public string SOPNUMBE { get; set; }
            [XmlElement(ElementName = "Quantity")]
            public string Quantity { get; set; }
            [XmlElement(ElementName = "IVITMTXB")]
            public string IVITMTXB { get; set; }
            [XmlElement(ElementName = "SHIPMTHD")]
            public string SHIPMTHD { get; set; }
            [XmlElement(ElementName = "DEFEXTPRICE")]
            public string DEFEXTPRICE { get; set; }
            [XmlElement(ElementName = "DEFPRICING")]
            public string DEFPRICING { get; set; }
            [XmlElement(ElementName = "DOCDATE")]
            public string DOCDATE { get; set; }
            [XmlElement(ElementName = "ITEMDESC")]
            public string ITEMDESC { get; set; }
            [XmlElement(ElementName = "ADDRESS1")]
            public string ADDRESS1 { get; set; }
            [XmlElement(ElementName = "ADDRESS2")]
            public string ADDRESS2 { get; set; }
            [XmlElement(ElementName = "CITY")]
            public string CITY { get; set; }
            [XmlElement(ElementName = "STATE")]
            public string STATE { get; set; }
            [XmlElement(ElementName = "ZIPCODE")]
            public string ZIPCODE { get; set; }
            [XmlElement(ElementName = "COUNTRY")]
            public string COUNTRY { get; set; }
        }

        [XmlRoot(ElementName = "taSopUserDefined")]
        public class TaSopUserDefined
        {
            [XmlElement(ElementName = "UpdateIfExists")]
            public string UpdateIfExists { get; set; }
            [XmlElement(ElementName = "USRDAT01")]
            public string USRDAT01 { get; set; }
            [XmlElement(ElementName = "SOPTYPE")]
            public string SOPTYPE { get; set; }
            [XmlElement(ElementName = "SOPNUMBE")]
            public string SOPNUMBE { get; set; }
        }

        [XmlRoot(ElementName = "taSopHdrIvcInsert")]
        public class TaSopHdrIvcInsert
        {
            [XmlElement(ElementName = "SOPTYPE")]
            public string SOPTYPE { get; set; }
            [XmlElement(ElementName = "DOCID")]
            public string DOCID { get; set; }
            [XmlElement(ElementName = "SOPNUMBE")]
            public string SOPNUMBE { get; set; }
            [XmlElement(ElementName = "TAXSCHID")]
            public string TAXSCHID { get; set; }
            [XmlElement(ElementName = "SHIPMTHD")]
            public string SHIPMTHD { get; set; }
            [XmlElement(ElementName = "TAXAMNT")]
            public string TAXAMNT { get; set; }
            [XmlElement(ElementName = "LOCNCODE")]
            public string LOCNCODE { get; set; }
            [XmlElement(ElementName = "DOCDATE")]
            public string DOCDATE { get; set; }
            [XmlElement(ElementName = "FREIGHT")]
            public string FREIGHT { get; set; }
            [XmlElement(ElementName = "CUSTNMBR")]
            public string CUSTNMBR { get; set; }
            [XmlElement(ElementName = "CUSTNAME")]
            public string CUSTNAME { get; set; }
            [XmlElement(ElementName = "CSTPONBR")]
            public string CSTPONBR { get; set; }
            [XmlElement(ElementName = "ShipToName")]
            public string ShipToName { get; set; }
            [XmlElement(ElementName = "BACHNUMB")]
            public string BACHNUMB { get; set; }
            [XmlElement(ElementName = "ADDRESS1")]
            public string ADDRESS1 { get; set; }
            [XmlElement(ElementName = "ADDRESS2")]
            public string ADDRESS2 { get; set; }
            [XmlElement(ElementName = "CNTCPRSN")]
            public string CNTCPRSN { get; set; }
            [XmlElement(ElementName = "CITY")]
            public string CITY { get; set; }
            [XmlElement(ElementName = "STATE")]
            public string STATE { get; set; }
            [XmlElement(ElementName = "ZIPCODE")]
            public string ZIPCODE { get; set; }
            [XmlElement(ElementName = "COUNTRY")]
            public string COUNTRY { get; set; }
            [XmlElement(ElementName = "Print_Phone_NumberGB")]
            public string Print_Phone_NumberGB { get; set; }
            [XmlElement(ElementName = "PHNUMBR1")]
            public string PHNUMBR1 { get; set; }
            [XmlElement(ElementName = "SUBTOTAL")]
            public string SUBTOTAL { get; set; }
            [XmlElement(ElementName = "PRBTADCD")]
            public string PRBTADCD { get; set; }
            [XmlElement(ElementName = "PRSTADCD")]
            public string PRSTADCD { get; set; }
            [XmlElement(ElementName = "ORDRDATE")]
            public string ORDRDATE { get; set; }
            [XmlElement(ElementName = "DUEDATE")]
            public string DUEDATE { get; set; }
            [XmlElement(ElementName = "USINGHEADERLEVELTAXES")]
            public string USINGHEADERLEVELTAXES { get; set; }
            [XmlElement(ElementName = "CREATETAXES")]
            public string CREATETAXES { get; set; }
            [XmlElement(ElementName = "DEFTAXSCHDS")]
            public string DEFTAXSCHDS { get; set; }
            [XmlElement(ElementName = "CURNCYID")]
            public string CURNCYID { get; set; }
            [XmlElement(ElementName = "COMMENT_1")]
            public string COMMENT_1 { get; set; }
            [XmlElement(ElementName = "ReqShipDate")]
            public string ReqShipDate { get; set; }
            [XmlElement(ElementName = "UpdateExisting")]
            public string UpdateExisting { get; set; }
            [XmlElement(ElementName = "PRCLEVEL")]
            public string PRCLEVEL { get; set; }
            [XmlElement(ElementName = "DEFPRICING")]
            public string DEFPRICING { get; set; }
        }

        [XmlRoot(ElementName = "SOPTransactionType")]
        public class SOPTransactionType
        {
            [XmlElement(ElementName = "taCreateCustomerAddress")]
            public TaCreateCustomerAddress TaCreateCustomerAddress { get; set; }
            [XmlElement(ElementName = "taCreateInternetAddresses")]
            public TaCreateInternetAddresses TaCreateInternetAddresses { get; set; }
            [XmlElement(ElementName = "taSopLineIvcInsert")]
            public List<TaSopLineIvcInsert> TaSopLineIvcInsert { get; set; }
            [XmlElement(ElementName = "taSopUserDefined")]
            public TaSopUserDefined TaSopUserDefined { get; set; }
            [XmlElement(ElementName = "taSopHdrIvcInsert")]
            public TaSopHdrIvcInsert TaSopHdrIvcInsert { get; set; }
        }

        [XmlRoot(ElementName = "eConnect")]
        public class EConnect
        {
            [XmlElement(ElementName = "SOPTransactionType")]
            public SOPTransactionType SOPTransactionType { get; set; }
        }
        
}