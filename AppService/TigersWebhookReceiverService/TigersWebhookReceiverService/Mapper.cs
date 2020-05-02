using BC.Integration.Canonical.Tigers;
using System;
using System.Collections.Generic;
using BC.Integration.APICalls;

namespace BC.Integration.AppService.TigersWebhookReceiverService
{
    public static class Mapper
    {
        /// <summary>
        /// Converts a Tigers shipping confiramtion into a canonical shipping confirmation message.
        /// </summary>
        /// <param name="message">Tigers shipping confiramtion</param>
        /// <returns>canonical shipping confirmation</returns>
        public static string ConvertShipmentConfirm(ShipmentConfirmation message, string siteMapping)
        {
            //Add code to convert message to the canonical format
            CanonicalShippingConfirmation canonical = new CanonicalShippingConfirmation();
            API_Calls APIcalls = new API_Calls();

            canonical.OrderStatus = "Shipped";

            if (message.salesOrderRef == null || message.salesOrderRef == "")
                throw new Exception("Shipping Confirmation Mapping Error: Herschel's Order Number (called 'Client Reference Number 1' in the data) is not populated.  This is a required field " +
                   "for processing the message.  The Tigers Order Number is: " + message.salesOrderRef);
            else
            {
                if (message.salesOrderRef.IndexOf('.') > 0)
                    canonical.OrderNbr = message.salesOrderRef.Substring(0, message.salesOrderRef.IndexOf('.'));
                else
                    canonical.OrderNbr = message.salesOrderRef;
            }

            canonical.ShippingDate = Convert.ToDateTime(message.despatchDate);

            canonical.OrderDate = Convert.ToDateTime(message.receivedDate);

            if (message.despatchItems == null)
            {
                throw new Exception("Shipping Confirmation Mapping Error: The Shipping Confirmation has no line items.  This is a required section " +
                    "for processing the message.  The Herschel Order Number is: " + message.salesOrderRef);

            }

            //Logic for determining warehouse code.  Due to Tigers not seperating west coast warehouses into seperate wholesale and eComm stock, logic is needed to determine the actual
            //warehouse site ID.  This code can be switched out if necessary.
            canonical.Site = message.addresses[0].locationNumber;

            //Sid - find what value goe in this.
            //if (message.FulfillType == null)
            //{
            //    throw new Exception("Shipping Confirmation Mapping Error: Fulfill Type (contained in the Data section) is null.  This is a required field " +
            //        "for processing the message.  The Herschel Order Number is: " + message.salesOrderRef);
            //}
            //else
            //{
            //    canonical.FulfillType = message.Data.FulfillType;
            //}


            //Verify if it's a cancellation 
            int despatchItems = 0;

            foreach (var item in message.despatchItems)
            {
                if (item.qtyDespatched > 0)
                {
                    ++despatchItems;
                }
            }  
            

            if (message.trackingRef == null || message.trackingRef == "")
            {
                
                if (despatchItems > 0) // if it's not a cancellation and the tracking ref is empty, throw the Exception
                {
                    throw new Exception("Shipping Confirmation Mapping Error: Shipment Pin (aka tracking number) is not populated.  This is a required field " +
                   "for processing the message.  The Herschel Order Number is: " + message.salesOrderRef);
                }
                
            }
            else
            {
                canonical.Bol = message.trackingRef;
            }

            //canonical.Customer = message.consigneeRef;
            canonical.Customer = APIcalls.GetCustomerCountryFromSite(canonical.Site);
            canonical.ShipmentType = message.deliveryCompany;

            if  (string.IsNullOrEmpty(message.deliveryService))
            {
                if (despatchItems > 0) // if it's not a cancellation and the deliveryService is empty, throw the Exception
                {
                    throw new Exception("Shipping Confirmation Mapping Error: Client Carrier Code is not populated.  This is a required field " +
                    "for processing the message.  The Herschel Order Number is: " + message.salesOrderRef);
                }
            }
            else
            {
                canonical.CarrierCode = message.deliveryService; // or is it scac code ?
            }

            canonical.Freight = message.deliveryCharge;

            canonical.TotalCartons = message.despatchItems.Count;
            canonical.TotalWeight = message.shipmentWeight; //shipmentWeight or orderweight ?

             //Loop through OrderConfirmationLines and populate canonical.lineItems
            canonical.LineItems = new List<LineItem>();

            foreach (DespatchItems item in message.despatchItems)
            {
                //foreach (Items line in item.items)
                //{
                    LineItem canonicalItem = new LineItem();
                    canonicalItem.LineNbr = item.orderLineNumber;

                    if (item.stockCode == null || item.stockCode == "")
                    {
                        throw new Exception("Shipping Confirmation Mapping Error: Item Number is not populated.  This is a required field " +
                            "for processing the message.  The Herschel Order Number is: " + message.salesOrderRef);
                    }
                    else
                    {
                        canonicalItem.ItemNbr = item.stockCode;
                    }

                    canonicalItem.QtyOrdered = item.qtyRequested;
                    canonicalItem.QtyShipped = item.qtyDespatched;
                    canonical.TotalQtyShipped += item.qtyDespatched;
                    canonical.LineItems.Add(canonicalItem);
                //}
            }

            return SerializeCanonicalShippingConfirmation.ToJson(canonical);
        }


        /// <summary>
        /// Converts the Tigers cancelled shipping orders into a zero shipped, shipping confirmation canonical message.
        /// </summary>
        /// <param name="message">Cancelled shipping order</param>
        /// <returns>zero shipped canonical shipping confirmation.</returns>
        public static string ConvertCancelled(Cancellation message, string locationCode)
        {
            //Add code to convert message to the canonical format
            CanonicalShippingConfirmation canonical = new CanonicalShippingConfirmation();

            canonical.OrderStatus = "CANCELLED";
            canonical.NriOrderNbr = message.documentRef;
            if (message.documentRef.IndexOf('.') > 0)
            {
                canonical.OrderNbr = message.documentRef.Substring(0, message.documentRef.IndexOf('.'));
            }
            else
            {
                canonical.OrderNbr = message.documentRef;
            }
            canonical.ShippingDate = new DateTime(1900, 1, 1);
            canonical.OrderDate = new DateTime(1900, 1, 1);
            canonical.Bol = "";
            canonical.Customer = message.secondaryRef;
            canonical.ShipmentType = "";
            canonical.CarrierCode = "";
            canonical.Freight = 0;
            canonical.TotalQtyShipped = 0;
            canonical.TotalCartons = 0;
            canonical.TotalWeight = 0;

            //if (message.Data != null)
            //{
            //    if (message.Data.FulfillType != null)
            //    {
            //        canonical.FulfillType = message.Data.FulfillType;
            //    }
            //    else
            //    {
            //        throw new Exception("Cancelled Shipping Order Mapping Error: The Fulfill Type in null.  This is a required field " +
            //            "for processing the message.  The Herschel Order Number is: " + message.documentRef);
            //    }

            //    if (locationCode == null || locationCode == "")
            //    {
            //        throw new Exception("Cancelled Shipping Order Mapping Error: The Warehouse Code is not populated.  This is a required field " +
            //            "for processing the message.  The Herschel Order Number is: " + message.documentRef);
            //    }
            //    else
            //    {
            //        canonical.Site = locationCode;
            //    }
            //}
            //else
            //{
            //    throw new Exception("Cancelled Shipping Order Mapping Error: The Data section is null, Fulfill Type and Warehouse Code are required fields " +
            //        "for processing the message.  The Herschel Order Number is: " + message.documentRef);
            //}
            //Loop through Cancelled Lines Items and populate canonical.lineItems
            canonical.LineItems = new List<LineItem>();

            LineItem canonicalItem = new LineItem();
            //if (message.ClientLineNumber == null || line.ClientLineNumber == "0" || line.ClientLineNumber == "")
            //{
            //    throw new Exception("Cancelled Shipping Order Mapping Error: The Client Item Line Number is not populated.  This is a required field " +
            //        "for processing the message.  The Herschel Order Number is: " + message.documentRef);
            //}
            //else
            //{
            //    canonicalItem.LineNbr = line.ClientLineNumber;
            //}
            //if (message.ItemNumber == null || message.ItemNumber == "")
            //{
            //    throw new Exception("Cancelled Shipping Order Mapping Error: The Item Number is not populated.  This is a required field " +
            //        "for processing the message.  The Herschel Order Number is: " + message.documentRef);
            //}
            //else
            //{
            //    canonicalItem.ItemNbr = message.ItemNumber;
            //}
            //canonicalItem.QtyOrdered = line.QuantityOrdered;
            canonicalItem.QtyShipped = 0;
            canonical.LineItems.Add(canonicalItem);

            return SerializeCanonicalShippingConfirmation.ToJson(canonical);
        }
    }
}