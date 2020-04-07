using BC.Integration.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace BC.Integration.AppService.BCSalesOrderSvc
{


    public class SalesOrder
    {
        //private static string tracingPrefix1 = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        //private static string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";

        public string orderNumber { get; set; }
        public int siteId { get; set; }
        public string currencyId { get; set; }
        public string taxRegistrationNum { get; set; }
        public string carrierCode { get; set; }
        public string paymentType { get; set; }
        public string customerId { get; set; }
        public string customerPONum { get; set; }
        public string orderType { get; set; }
       
        public DateTime orderDate { get; set; }
        public DateTime? quoteExpirationDate { get; set; }
        public DateTime cancelDate { get; set; }
        public DateTime reqShipDate { get; set; }
        public string paymentMethod { get; set; }
        public string sopHeaderComment { get; set; }

        public string shippingAddressId { get; set; }
        public string shippingName { get; set; }
        public string shippingAddress1 { get; set; }
        public string shippingAddress2 { get; set; }
        public string shippingAddress3 { get; set; }
        public string shippingcity { get; set; }
        public string shippingpostalCode { get; set; }
        public string shippingcountry { get; set; }
        public string shippingphone { get; set; }
        public string customerEmail { get; set; }

        public string billingaddressId { get; set; }
        public string billingcustomerName { get; set; }
        public string billingcustomerAddress1 { get; set; }
        public string billingcustomerAddress2 { get; set; }
        public string billingcustomerAddress3 { get; set; }
        public string billingcity { get; set; }
        public string billingpostalCode { get; set; }
        public string billingcountry { get; set; }
        public string billingphone { get; set; }
        //public string customerEmail { get; set; }
        public string incoTerms { get; set; }

        public List<SalesOrderLineItem> salesOrderLineItem { get; set; }

        public List<SalesOrderAddress> salesOrderAddress { get; set; }


        //private SalesOrders AddOrders(string[] colLines)
        //{
        //    SalesOrder salesOrder = new SalesOrder();
        //    SalesOrderLineItem item = new SalesOrderLineItem();
        //    SalesOrderAddress address = new SalesOrderAddress();
        //    SalesOrders orderList = new SalesOrders();
        //    string[] colfields;
        //    decimal d;
        //    double dbl;
        //    long l;
        //    int n;
        //    List<string> uniqueOrders = new List<string>();

        //    foreach (string csvLine in colLines)
        //    {
        //        colfields = csvLine.Split(',').Select(s => s.Trim('"')).ToArray();

        //        if (colfields.Count() < 80)
        //        {
        //            throw new Exception("The AddLine method of the BC.Integration.AppService.Tigers.Orders class was given a data line with too many" +
        //                "elements.  The dataset should have 20 elements.  Please ensure the data does not contain any extra commas or missing trailing commas.");
        //        }

        //        #region AddItem

        //        item.LineSeqNum = colfields[0];
        //        item.ItemNum = colfields[1];
        //        item.UnitOfMeasure = colfields[2];

        //        if (double.TryParse(colfields[3], out dbl))
        //        {
        //            item.UnitQuantity = Convert.ToInt32(dbl);
        //        }
        //        else
        //        {
        //            throw new Exception("Failed to set the Returned Goods in a Good Condition field for transaction number: " + item.OrderNumber + ".  The value was not recognised as a valid value (" + colfields[19] + ").");
        //        }

        //        if (Utilities.ProcessDecimalString(colfields[4], out d))
        //        {
        //            item.UnitPrice = d;
        //        }
        //        else
        //        {
        //            throw new Exception("Failed to set the Actual Price field for transaction number: " + item.OrderNumber + ".  The value was not recognised as a valid value (" + colfields[9] + ").");
        //        }

        //        item.ItemLineComment = colfields[5];
        //        item.ItemCategory = colfields[6];
        //        item.ItemProductName = colfields[7];
        //        item.ItemSize = colfields[8];
        //        item.ItemColour = colfields[9];
        //        item.ItemFabric = colfields[10];
        //        item.OrderNumber = colfields[14];

        //        if (Utilities.ProcessIntString(colfields[15], out n))
        //        {
        //            item.SiteId = n;
        //        }
        //        else
        //        {
        //            throw new Exception("Failed to set the Actual Price field for transaction number: " + item.OrderNumber + ".  The value was not recognised as a valid value (" + colfields[9] + ").");
        //        }

        //        if (salesOrder.salesOrderLineItem == null)
        //        {
        //            salesOrder.salesOrderLineItem = new List<SalesOrderLineItem>();
        //        }

        //        salesOrder.salesOrderLineItem.Add(item);

        //        #endregion

        //        #region AddAddress

        //        address.customerName = colfields[21];
        //        address.customerAddress1 = colfields[22];
        //        address.customerAddress2 = colfields[23];
        //        address.customerAddress3 = colfields[24];
        //        address.city = colfields[25];
        //        address.postalCode = colfields[27];
        //        address.country = colfields[28];
        //        address.phone = colfields[29];
        //        address.addressId = colfields[30];
        //        address.customerEmail = colfields[74];

        //        if (salesOrder.salesOrderAddress == null)
        //        {
        //            salesOrder.salesOrderAddress = new List<SalesOrderAddress>();
        //        }

        //        salesOrder.salesOrderAddress.Add(address);

        //        address.customerName = colfields[31];
        //        address.customerAddress1 = colfields[32];
        //        address.customerAddress2 = colfields[33];
        //        address.customerAddress3 = colfields[34];
        //        address.city = colfields[35];
        //        address.postalCode = colfields[37];
        //        address.country = colfields[38];
        //        address.phone = colfields[39];
        //        address.customerEmail = colfields[74];


        //        salesOrder.salesOrderAddress.Add(address);


        //        #endregion

        //        if (salesOrder.orderNumber == null) // new order
        //        {
        //            #region AddOrder

        //            salesOrder.orderNumber = colfields[16];
        //            salesOrder.customerId = colfields[20];

                   
        //            //salesOrder.billingAddressId =  colfields[0];

        //            salesOrder.customerPONum = colfields[40];
        //            salesOrder.orderType = colfields[41];

        //            try
        //            {
        //                //salesOrder.orderDate = FromExcelSerialDate(colfields[43]);

        //                salesOrder.orderDate = DateTime.Parse(colfields[43]);
        //                //salesOrder.orderDate = new DateTime(Convert.ToInt16((colfields[43]).Substring(0, 4)),
        //                //                                 Convert.ToInt16(colfields[43].Substring(4, 2)),
        //                //                                 Convert.ToInt16(colfields[43].Substring(6, 2)));
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Failed to create Delivery Date field for transaction number: " + salesOrder.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
        //            }

        //            try
        //            {
        //                salesOrder.quoteExpirationDate = DateTime.Parse(colfields[44]);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Failed to create Delivery Date field for transaction number: " + salesOrder.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
        //            }

        //            try
        //            {
        //                salesOrder.cancelDate = DateTime.Parse(colfields[45]);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Failed to create Delivery Date field for transaction number: " + salesOrder.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
        //            }

        //            try
        //            {
        //                salesOrder.reqShipDate = DateTime.Parse(colfields[48]);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Failed to create Delivery Date field for transaction number: " + salesOrder.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
        //            }

        //            salesOrder.paymentMethod = colfields[50];
        //            salesOrder.carrierCode = colfields[54];
        //            salesOrder.sopHeaderComment = colfields[65];
        //            if (Utilities.ProcessIntString(colfields[67], out n))
        //            {
        //                salesOrder.siteId = n;
        //            }
        //            else
        //            {
        //                throw new Exception("Failed to set the Actual Price field for transaction number: " + salesOrder.orderNumber + ".  The value was not recognised as a valid value (" + colfields[9] + ").");
        //            }
                
        //            salesOrder.currencyId = colfields[76];
        //            salesOrder.taxRegistrationNum = colfields[77];

        //            #endregion

        //        }
        //        else if (salesOrder.orderNumber != item.OrderNumber) //addition of another order
        //        {
        //            orderList.salesOrders.Add(salesOrder);
        //        }
        //        else if (salesOrder.orderNumber == item.OrderNumber) // same order continue
        //        {
        //            orderList.salesOrders.Add(salesOrder);

        //            continue;
        //        }
        //    }
        //    return orderList;
        //}


        public static DateTime FromExcelSerialDate(string SerialDate)
        {
            int serialDate = Convert.ToInt32(SerialDate);
            if (serialDate > 59) serialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(serialDate);
        }

        public void Add(SalesOrderLineItem item)
        {
            salesOrderLineItem.Add(item);
        }

        public string ConvertOrdersToString(SalesOrder orders)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(orders.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, orders);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured serializing the Tigers Sales Orders object model.  The error occured in the BC.Integration.AppService.Tigers.Orders.ConvertOrdersToString method.", ex);
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

    }


    public class SalesOrderLineItem
    {
        public string LineSeqNum { get; set; } = "";
        public string ItemNum { get; set; } = "";
        public string UnitOfMeasure { get; set; } = "";
        public int UnitQuantity { get; set; } = 0;
        public decimal UnitPrice { get; set; } = 0;
        public string ItemLineComment { get; set; } = "";
        public DateTime ItemShipDate { get; set; } = new DateTime();
        public string ItemCategory { get; set; } = "";
        public string ItemProductName { get; set; } = "";
        public string ItemSize { get; set; } = "";
        public string ItemColour { get; set; } = "";
        public string ItemFabric { get; set; } = "";
        public string OrderNumber { get; set; } = "";
        public int SiteId { get; set; } = 0;


        public string ConvertToCsv()
        {
            return LineSeqNum + "," + ItemNum + "," + UnitOfMeasure + "," + UnitQuantity + "," + UnitPrice + "," + ItemLineComment + "," +
                ItemShipDate.ToString("yyymmdd") + "," + ItemCategory + "," + ItemProductName + "," + ItemSize + "," + ItemColour + "," + ItemFabric + "," + OrderNumber + "," +
                SiteId;
        }
    }


    public class SalesOrderAddress
    {
        public string addressId { get; set; }
        public string customerName { get; set; }
        public string customerAddress1 { get; set; }
        public string customerAddress2 { get; set; }
        public string customerAddress3 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string customerEmail { get; set; }


        //public string ConvertToCsv()
        //{
        //    return LineSeqNum + "," + ItemNum + "," + UnitOfMeasure + "," + UnitQuantity + "," + UnitPrice + "," + ItemLineComment + "," +
        //        ItemShipDate.ToString("yyymmdd") + "," + ItemCategory + "," + ItemProductName + "," + ItemSize + "," + ItemColour + "," + ItemFabric + "," + OrderNumber + "," +
        //        SiteId;
        //}
    }

    public class SalesOrders
    {
        public List<SalesOrder> salesOrders { get; set; }
    }

 
}