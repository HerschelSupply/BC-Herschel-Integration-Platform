using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BC.Integration.Canonical.Tigers;

namespace BC.Integration.AppService.EpOffRampServiceTigersBC
{
    public static class Mapper
    {
        /// <summary>
        /// Converts Canonical shipping confiramtion into a EP shipping confirmation message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>EP shipping confirmation</returns>
        public static string ConvertShipmentConfirm(CanonicalShippingConfirmation message)
        {
            EPShippingConfirmation epShipmentConfirmation = new EPShippingConfirmation();

            epShipmentConfirmation.OrderNumber = message.OrderNbr;

            //Loop through ShipmentConfirmationLines and populate ep.lineItems
            epShipmentConfirmation.ShipmentNotificationLines = new List<LineItems>();


            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            for (int i = 0; i < message.LineItems.Count; i++)
            {
                LineItems epItem = new LineItems();
                epItem.ItemNumber = message.LineItems[i].ItemNbr;
                epItem.QuantityShipped = message.LineItems[i].QtyShipped;
                epItem.QuantityOrdered = message.LineItems[i].QtyOrdered;
                epItem.LineNumber = message.LineItems[i].LineNbr;
                epItem.OrderNbr = message.OrderNbr;
                epItem.NriOrderNumber = message.NriOrderNbr;
                epItem.ShippingDate = Convert.ToInt64((message.ShippingDate - epoch).TotalMilliseconds);
                epItem.Bol = message.Bol;
                epItem.ShipmentType = message.ShipmentType;
                epItem.CarrierCode = message.CarrierCode;
                epItem.Freight = message.Freight;
                epItem.TotalWeight = message.TotalWeight.ToString();
                epItem.TotalQuantityShipped = message.TotalQtyShipped;
                epItem.TotalCartons = message.TotalCartons;
                epItem.Customer = message.Customer;
                epItem.OrderDate = Convert.ToInt64((message.OrderDate - epoch).TotalMilliseconds); ;
                epItem.FulfillType = message.FulfillType;
                epItem.Site = message.Site;

                epShipmentConfirmation.ShipmentNotificationLines.Add(epItem);
            }

            return SerializeEpShippingConfirmation.ToJson(epShipmentConfirmation);
        }
    }
}