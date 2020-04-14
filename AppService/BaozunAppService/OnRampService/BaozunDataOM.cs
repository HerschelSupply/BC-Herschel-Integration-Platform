using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Diagnostics;
using BC.Integration.Utility;
using System.Configuration;


namespace BC.Integration.AppService.BaozunOnRamp
{
    public class Orders
    {
        //CR7: Tracing prefix
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        private List<OrderLineItem> lineItems = new List<OrderLineItem>();

        public List<OrderLineItem> LineItems { get => lineItems; set => lineItems = value; }

        public void AddLine(string csvLine)
        {
            OrderLineItem item = new OrderLineItem();
            int n;
            decimal d;
            //"TBHE1709000020,20170912,200302,TB_HE,12345678901234567,S,2,1234567888,80,80,3,0,240,TEST01,,张三,,13612341234,10,,,"
            //"R9000020,20170912,200302,TB_HE,12345678901234567,R,2,1234567888,80,80,3,0,240,TEST01,,张三,,13612341234,10,1,1,1"
            string[] colfields = csvLine.Split(',');
            if (colfields.Count() != 22)
                throw new Exception("The AddLine method of the BC.Integration.AppService.Baozun.Orders class was given a data line with too many" +
                    "elements.  The dataset should have 20 elements.  Please ensure the data does not contain any extra commas or missing trailing commas.");

            item.TransactionNumber = colfields[0];
            try
            {
                item.DeliveryDate = new DateTime(Convert.ToInt16(colfields[1].Substring(0, 4)),
                                                 Convert.ToInt16(colfields[1].Substring(4, 2)),
                                                 Convert.ToInt16(colfields[1].Substring(6, 2)));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create Delivery Date field for transaction number: " + item.TransactionNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
            }
            item.DeliveryTime = colfields[2];
            item.StoreCode = colfields[3];
            item.SlipCode = colfields[4];
            if (colfields[5] == "S" || colfields[5] == "R" || colfields[5] == "E")
            {
                item.TransactionType = colfields[5];
            }
            else
            {
                throw new Exception("Failed to set Transaction Type field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[5] + ").");
            }
            item.LineNumber = colfields[6];
            item.ProductCode = colfields[7];
            if (Utilities.ProcessDecimalString(colfields[8], out d))
            {
                item.UnitPrice = d;
            }
            else
            {
                throw new Exception("Failed to set the Unit Price field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[8] + ").");
            }
            if (Utilities.ProcessDecimalString(colfields[9], out d))
            {
                item.ActualPrice = d;
            }
            else
            {
                throw new Exception("Failed to set the Actual Price field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[9] + ").");
            }
            if (Utilities.ProcessIntString(colfields[10], out n))
            {
                item.Quantity = n;
            }
            else
            {
                throw new Exception("Failed to set the Quantity field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[10] + ").");
            }
            //item.Discount = Convert.ToDecimal(colfields[11]);
            if (Utilities.ProcessDecimalString(colfields[11], out d))
            {
                item.Discount = d;
            }
            else
            {
                throw new Exception("Failed to set the Discount field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[11] + ").");
            }
            //item.LineSumAmount = Convert.ToDecimal(colfields[12]);
            if (Utilities.ProcessDecimalString(colfields[12], out d))
            {
                item.LineSumAmount = d;
            }
            else
            {
                throw new Exception("Failed to set the Line Sum Amount field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[12] + ").");
            }
            try
            {
                item.CustId = colfields[13];
                item.CustEmail = colfields[14];
                item.CustName = colfields[15];
                item.CustTelNumber = colfields[16];
                item.CustMobile = colfields[17];
            }
            catch (Exception ex)
            {
                //Supress an exception mapping this data as it is not used and may contain Chinese charators that GP can not handle.
                Trace.WriteLine(tracingExceptionPrefix + " Customer data resulted in a mapping exception.  Ecxeption was supressed as data not currently used.  " +
                    "Exception message: " + ex.Message);
            }
            if(Utilities.ProcessIntString(colfields[18], out n))
            {
                item.LogisticCost = n;
            }
            else
            {
                throw new Exception("Failed to set the Logistic Cost field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[18] + ").");
            }
            if (Utilities.ProcessIntString(colfields[19], out n))
            {
                item.ReturnGood = n;
            }
            else
            {
                throw new Exception("Failed to set the Returned Goods in a Good Condition field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[19] + ").");
            }
            if (Utilities.ProcessIntString(colfields[20], out n))
            {
                item.ReturnDamaged = n;
            }
            else
            {
                throw new Exception("Failed to set the Returned Goods in a Damaged Condition field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[20] + ").");
            }
            if (Utilities.ProcessIntString(colfields[21], out n))
            {
                item.ReturnUnknown = n;
            }
            else
            {
                throw new Exception("Failed to set the Returned Goods in an Unknown Condition field for transaction number: " + item.TransactionNumber + ".  The value was not recognised as a valid value (" + colfields[21] + ").");
            }

            LineItems.Add(item);
        }

        public void Add(OrderLineItem item)
        {
            LineItems.Add(item);
        }

        public int TransactionCount()
        {
            var items = from item in lineItems
                      select item.TransactionNumber;
            var distinctItemNumbers = items.Distinct();

            return distinctItemNumbers.Count();
        }

        public List<string> DistinctTransactionNumbers()
        {
            var items = from item in lineItems
                      select item.TransactionNumber;
            var distinctItemNumbers = items.Distinct();

            return distinctItemNumbers.ToList();
        }

        public List<OrderLineItem> OrderLineItems(string transactionNumber)
        {
            var items = from item in lineItems
                        where item.TransactionNumber == transactionNumber
                        select item;
            
            return items.ToList();
        }

        public string ConvertOrdersToString(Orders orders)
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
                throw new Exception("Error occured serializing the Baozun Orders object model.  The error occured in the BC.Integration.AppService.Baozun.Orders.ConvertOrdersToString method.", ex);
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

    public class OrderLineItem
    {
        private string transactionNumber = "";
        private DateTime deliveryDate = new DateTime();
        private string deliveryTime = "";
        private string storeCode = "";
        private string slipCode = "";
        private string transactionType = "";
        private string lineNumber = "";
        private string productCode = "";
        private decimal unitPrice = 0;
        private decimal actualPrice = 0;
        private int quantity = 0;
        private decimal discount = 0;
        private decimal lineSumAmount = 0;
        private string custId = "";
        private string custEmail = "";
        private string custName = "";
        private string custTelNumber = "";
        private string custMobile = "";
        private decimal logisticCost = 0;
        private int returnGood = 0;
        private int returnDamaged = 0;
        private int returnUnknown = 0;
        //ProcessedStatus used to track which messages in the batch file were processed.
        private string processedStatus = "Unprocessed";
        private Nullable<DateTime> processedDateTime = null;

        public string TransactionNumber { get => transactionNumber; set => transactionNumber = value; }
        public DateTime DeliveryDate { get => deliveryDate; set => deliveryDate = value; }
        public string DeliveryTime { get => deliveryTime; set => deliveryTime = value; }
        public string StoreCode { get => storeCode; set => storeCode = value; }
        public string SlipCode { get => slipCode; set => slipCode = value; }
        public string TransactionType { get => transactionType; set => transactionType = value; }
        public string LineNumber { get => lineNumber; set => lineNumber = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public decimal UnitPrice { get => unitPrice; set => unitPrice = value; }
        public decimal ActualPrice { get => actualPrice; set => actualPrice = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public decimal Discount { get => discount; set => discount = value; }
        public decimal LineSumAmount { get => lineSumAmount; set => lineSumAmount = value; }
        public string CustId { get => custId; set => custId = value; }
        public string CustEmail { get => custEmail; set => custEmail = value; }
        public string CustName { get => custName; set => custName = value; }
        public string CustTelNumber { get => custTelNumber; set => custTelNumber = value; }
        public string CustMobile { get => custMobile; set => custMobile = value; }
        public decimal LogisticCost { get => logisticCost; set => logisticCost = value; }
        public int ReturnGood { get => returnGood; set => returnGood = value; }
        public int ReturnDamaged { get => returnDamaged; set => returnDamaged = value; }
        public int ReturnUnknown { get => returnUnknown; set => returnUnknown = value; }
        public string ProcessedStatus { get => processedStatus; set => processedStatus = value; }
        public DateTime? ProcessedDateTime { get => processedDateTime; set => processedDateTime = value; }

        public string ConvertToCsv()
        {
            return transactionNumber + "," + deliveryDate.ToString("yyyyMMdd") + "," + deliveryTime + "," + storeCode + "," + slipCode + "," +
                transactionType + "," + lineNumber + "," + productCode + "," + unitPrice + "," + actualPrice + "," + quantity + "," + discount + "," +
                lineSumAmount + "," + custName + "," + custTelNumber + "," + custMobile + "," + logisticCost + "," + returnGood + "," + 
                returnDamaged + "," + returnUnknown;
        }
    }
}
