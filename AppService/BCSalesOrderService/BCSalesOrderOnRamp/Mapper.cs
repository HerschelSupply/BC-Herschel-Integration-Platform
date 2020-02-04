using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Diagnostics;
using Corp.Integration.AppService.Tigers.SalesOrder;

namespace Corp.Integration.AppService.GPSalesOrderSvc
{
    public class Mapper
    {
        //Sid - Delete this if not used.
        //private static Invoice ConvertToCanonical(string processName, SalesOrder trans)
        //{
        //    Invoice inv = new Invoice();
        //    try
        //    {
        //        inv.Process = processName;

        //        inv.Header = new InvoiceHeader();
        //        inv.Header.InvoiceNumber = trans.ticketNumber.Value;
        //        inv.Header.InvoiceDate = trans.completeTime;
        //        inv.Header.InvoiceTime = trans.completeTime;
        //        if (exchange)
        //        {
        //            inv.Header.InvoiceType = "Sales";
        //        }
        //        else if (trans.total.Value < 0)
        //        {
        //            inv.Header.InvoiceType = "Return";
        //        }
        //        else
        //        {
        //            inv.Header.InvoiceType = "Sales";
        //        }
        //        inv.Header.Exchange = exchange;
        //        inv.Header.ShopId = trans.shopID;
        //        inv.Header.SiteId = MapShopIdToSiteId(trans.shopID, shopIdToSiteMapping);
        //        inv.Header.Currency = trans.total.currency;
        //        inv.Header.TotalLineItemAmountBeforeTax = Math.Abs(trans.calcTaxable.Value);
        //        inv.Header.TotalTax = trans.taxTotal;
        //        inv.Header.TotalTax = trans.taxTotal;
        //        inv.Header.TotalAmount = trans.total.Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when mapping the Lightspeed Sale transaction to the CanonicalSaleChannel" +
        //            " schema (Header).  The exception occurred in Corp.Integration.AppService.LightspeedPosOnRamp.Mapper.ConvertToCanonical method. " +
        //            " Exception message: " + ex.Message);
        //        throw new Exception("An exception occurred mapping the header elements for transaction: " + trans.ticketNumber.Value + ".", ex);
        //    }
        //}
    }
}