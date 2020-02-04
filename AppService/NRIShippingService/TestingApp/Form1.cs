using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BC.Integration.AppService.NriShippingOnRampService;

namespace TestingApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            NriShippingOnRampSvc.OnRampClient client = new NriShippingOnRampSvc.OnRampClient();
            client.InitializeProcess(Guid.NewGuid().ToString());
        }

        private void btnProcessConfirmations_Click(object sender, EventArgs e)
        {
            string data = "[{'OrderID':'636e2e7d-6da7-46c3-b81a-1649f516dba6','ShipmentNumber':139921,'ShipmentDocumentDate':null,'CompletionDate':'2010-07-14T19:02:27','PaymentTerm':'PACK AND HOLD','FreightTerm':'Prepaid','CarrierName':'FedEx Express','ShipmentPin':'863546916716','CodShipment':false,'ShipmentDate':'2010-07-14T00:00:00','ClientFreightCharge':0.00,'ClientInsuranceCharge':2.62,'ClientTotalFreightCharge':2.62,'CustomerFreightCharge':0.00,'ShipmentValue':75.00,'CodValue':0.00,'ShipToCustomerCode':null,'ShipToName':'HERSCHEL ONLINE CUSTOMER','ShipToAddressLine1':'4415 LEMAC','ShipToAddressLine2':'','ShipToCity':'KAMLOOPS','ShipToProvinceCode':'BC','ShipToPostalCode':'V2C6T4','ShipToCountryCode':'CAN','ShipToPhoneNumber':'','BillToCustomerCode':'173','BillToName':'HERSCHEL ONLINE CUSTOMER','BillToAddressLine1':'2107B 4TH ST SW','BillToAddressLine2':'','BillToCity':'CALGARY','BillToProvinceCode':'AB','BillToPostalCode':'T2S1W8','BillToCountryCode':'CAN','ExpressService':true,'CarrierSCAC':'FEDE','RetailerType':0,'ClientCarrierCode':'FEDEXINTEX','BolNumber':'','OrderNumber':2007569,'OrderDate':'2010-07-13T00:00:00','ClientReferenceNumber1':'10092','ClientReferenceNumber2':'','CustomerDepartment':'','ClientDocumentType':'Herschel Supply Company Default Order','PurchaseOrderNumber':'1692883','PurchaseOrderDate':'2010-07-13T00:00:00','OrderValue':75.00,'NetAmount':10.00,'TaxAmount':0.72,'InvoiceAmount':10.72,'LeadOrderOnShipment':true,'Imported':true,'TotalQuantityOrdered':1,'TotalQuantityShipped':1,'WarehouseName':'Herschel Supply Company Ltd.','ClientWarehouseCode':'10','CarrierServiceCode':'01','CarrierServiceName':'FedEx Priority Overnight','MarkFor':'','ShippingInstructions':'','HandlingInstructions':'','PackedDate':'2010-07-14T06:08:05.903','AtsNumber':'','DataSource':8,'ShipmentCartons':[{'CartonNumber':1,'LicensePlate':'0016015089','PinNumber':'','Sscc':'','CrossDock':false,'Length':11.00,'Width':11.00,'Height':8.00,'Weight':2.30,'TrackingUrl':'http://www.fedex.com/Tracking?cntry_code=us&tracknumber_list=&language=english','ShipmentCartonItems':[{'ItemNumber':'H-103-28-BLK-OS','ClientItemNumber':'H-103-28-BLK-OS','Description':'Novel-BLACK','UomDescription':'Each','Gtin':'00793573775641','Quantity':1}]}],'OrderConfirmationLines':[{'LineNumber':1,'ItemNumber':'H-103-28-BLK-OS','ClientItemNumber':'H-103-28-BLK-OS','Description':'Novel-BLACK','UomCode':'EA','UomDescription':'Each','WarehouseName':'Herschel Supply Company Ltd.','QuantityOrdered':1,'QuantityShipped':1,'CustomerItemNumber':'','ClientLineNumber':0,'GrossPrice':75.00,'DiscountAmount':0.00,'DependencyCode':'','NetPrice':75.00,'ExtendedPriceQuantityOrdered':75.00,'ExtendedPriceQuantityShipped':75.00,'WholesaleValue':40.00,'ExtendedValue':40.00,'ServiceCharge':0.450000,'Gtin':'00793573775641'}],'Data':{'FulfillType': ''}}]";

            Process process = new Process();
            //process.ProcessShipmentConfirmData(data);
        }

        private void btnCancellations_Click(object sender, EventArgs e)
        {
            string dataCancelled = "[{'OrderID':'7d6ba7ad-ff74-4afe-8256-0952db44194c','DocumentNumber':4622588,'DocumentDate':'2017-06-28T15:01:06.953','ClientReferenceNumber1':'JOURNALS','ClientReferenceNumber2':'','Status':'Cancelled','CompletionDate':null,'ClientDocumentType':'Herschel Supply Company Default Order','PurchaseOrderNumber':'','FirstShipDate':'2017-06-28T00:00:00','LastShipDate':'2017-06-28T00:00:00','MarkFor':'','PackedDate':null,'ShipToCustomerCode':'HERSCHEAST','ShipToName':'HERSCHEL C/O FARROW LOGISTICS','ShipToAddressLine1':'102-3255 ARGENTIA ROAD','ShipToAddressLine2':'','ShipToAddressLine3':'','ShipToCity':'MISSISSAUGA','ShipToProvinceCode':'ON','ShipToCountryCode':'CAN','ShipToPostalCode':'L5N0G1','BillToCustomerCode':'HERSCHEAST','BillToName':'HERSCHEL C/O FARROW LOGISTICS','BillToAddressLine1':'102-3255 ARGENTIA ROAD','BillToAddressLine2':'','BillToAddressLine3':'','BillToCity':'MISSISSAUGA','BillToProvinceCode':'ON','BillToCountryCode':'CAN','BillToPostalCode':'L5N0G1','OrderLines':[{'LineNumber':1,'ClientLineNumber':1,'ItemNumber':'10000-17S1JRNL','ClientItemNumber':'10000-17S1JRNL','ItemDescription':'HERSCHEL SUPPLY JOURNAL 2017 S1','UomCode':'EA','UomDescription':'Each','WarehouseName':'Herschel EAST','GrossPrice':1.00,'DiscountAmount':0.00,'NetPrice':1.00,'DependencyCode':'','Gtin':'00828432131921','CustomerGtin':'','QuantityOrdered':815,'QuantityAllocated':0,'QuantityPicked':0,'QuantityShipped':0}],'Data':{'WarehouseCode':'21','FulfillType': ''}}]";

            Process process = new Process();
            //process.ProcessCancelledOrdersData(dataCancelled);

        }
    }
}
