using System;
using System.Collections.Generic;
using BC.Integration.Canonical.NRI;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BC.Integration.AppService.NriShippingOnRampService
{
    public static class Mapper
    {
        /// <summary>
        /// Converts a NRI shipping confiramtion into a canonical shipping confirmation message.
        /// </summary>
        /// <param name="message">NRI shipping confiramtion</param>
        /// <returns>canonical shipping confirmation</returns>
        //public static string ConvertShipmentConfirm(Nri.ShippingConfirmation.ShippingConfirmation message, string siteMapping)
        public static string ConvertShipmentConfirm(Nri.ShippingConfirmation.ShippingConfirmation message, string siteMapping)
        {
            //Add code to convert message to the canonical format
            CanonicalShippingConfirmation canonical = new CanonicalShippingConfirmation();


            canonical.PickNum = "1166";//message.PickNumber;
            canonical.OrderStatus = "Shipped";
            canonical.NriOrderNbr = message.OrderNumber.ToString();

            if (message.ClientReferenceNumber1 == null || message.ClientReferenceNumber1 == "")
                throw new Exception("Shipping Confirmation Mapping Error: Herschel's Order Number (called 'Client Reference Number 1' in the data) is not populated.  This is a required field " +
                   "for processing the message.  The NRI Order Number is: " + message.OrderNumber.ToString());
            else
            {
                if (message.ClientReferenceNumber1.IndexOf('.') > 0)
                    canonical.OrderNbr = message.ClientReferenceNumber1.Substring(0, message.ClientReferenceNumber1.IndexOf('.'));
                else
                    canonical.OrderNbr = message.ClientReferenceNumber1;
              
            }

            canonical.ShippingDate = message.ShipmentDate;
            canonical.OrderDate = message.OrderDate;

            if (message.Data == null)
            {
                throw new Exception("Shipping Confirmation Mapping Error: The Data section is null.  This is a required section " +
                    "for processing the message, it contains the warehouse code (site ID) and Fulfill Type fields.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
            }

            //Logic for determining warehouse code.  Due to NRI not seperating west coast warehouses into seperate wholesale and eComm stock, logic is needed to determine the actual
            //warehouse site ID.  This code can be switched out if necessary.
            if (siteMapping == "Logic")
            {
                string headerSiteId = GetHeaderSiteId(message);
                string dataSiteID = GetDataSiteId(message);
                if(headerSiteId.EndsWith("2")) //East coast eComm shipment, so header site ID will be correct
                {
                    canonical.Site = GetHeaderSiteId(message);
                }
                else if (dataSiteID.EndsWith("0")) //wholesale order, so header site ID will be correct
                {
                    canonical.Site = GetHeaderSiteId(message);
                }
                else //West coast eComm order, so use the first charactor of the header ID and add 1
                {
                    canonical.Site = headerSiteId.Substring(0, 1) + "1";
                }
            }
            else if (siteMapping == "HeaderClientWarehouseCode")
            {
                canonical.Site = GetHeaderSiteId(message);
            }
            else if (siteMapping == "DataWarehouseCode")
            {
                canonical.Site = GetDataSiteId(message);
            }
            else
            {
                throw new Exception("Shipping Confirmation Mapping Error: The configuration value for the Site Mapping is not supported.  Supported values are 'Logic', 'HeaderClientWarehouseCode', 'DataWarehouseCode'." +
                    "  Please update the NRIShippingOn-RampService configuration file.");
            }

            /*if (message.Data.FulfillType == null)
            {
                throw new Exception("Shipping Confirmation Mapping Error: Fulfill Type (contained in the Data section) is null.  This is a required field " +
                    "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
            }
            else
            {
                canonical.FulfillType = message.Data.FulfillType;
            }*/

            canonical.FulfillType = message.Data.FulfillType;


            if (message.ShipmentPin == null || message.ShipmentPin == "")
            {
                throw new Exception("Shipping Confirmation Mapping Error: Shipment Pin (aka Bill of Lading) is not populated.  This is a required field " +
                    "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
            }
            else
            {
                canonical.Bol = message.ShipmentPin;
            }

            canonical.Customer = message.ShipToCustomerCode;
            canonical.ShipmentType = message.CarrierServiceName;

            if (message.ClientCarrierCode == null || message.ClientCarrierCode == "")
            {
                throw new Exception("Shipping Confirmation Mapping Error: Client Carrier Code is not populated.  This is a required field " +
                    "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
            }
            else
            {
                canonical.CarrierCode = message.ClientCarrierCode;
            }

            canonical.Freight = message.CustomerFreightCharge;
            canonical.TotalQtyShipped = message.TotalQuantityShipped;

            canonical.TotalCartons = message.ShipmentCartons.Count;
            canonical.TotalWeight = message.TotalWeight();
            canonical.CancellationReason = message.CancellationReason;

            if (message.OrderConfirmationLines == null)
            {
                throw new Exception("Shipping Confirmation Mapping Error: The Shipping Confirmation has no line items.  This is a required section " +
                    "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
            }

            //Loop through OrderConfirmationLines and populate canonical.lineItems
            canonical.LineItems = new List<LineItem>();
            foreach (Nri.ShippingConfirmation.OrderConfirmationLine line in message.OrderConfirmationLines)
            {
                LineItem canonicalItem = new LineItem();
                canonicalItem.LineNbr = line.ClientLineNumber;

                if (line.ItemNumber == null || line.ItemNumber == "")
                {
                    throw new Exception("Shipping Confirmation Mapping Error: Item Number is not populated.  This is a required field " +
                        "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
                }
                else
                {
                    canonicalItem.ItemNbr = line.ItemNumber; 
                }

                canonicalItem.QtyOrdered = line.QuantityOrdered;
                canonicalItem.QtyShipped = line.QuantityShipped;
                canonicalItem.ItemPrice = Convert.ToDecimal(line.NetPrice);
                canonicalItem.CancellationReason = message.CancellationReason;
                canonical.LineItems.Add(canonicalItem);
            }

            //Loop through OrderCartons and populate canonical.Cartons
            canonical.Cartons = new List<Carton>();
            foreach (Nri.ShippingConfirmation.ShipmentCarton shpCart in message.ShipmentCartons)
            {
                Carton canonicalCarton = new Carton();
                canonicalCarton.CartonNbr = shpCart.Sscc;
                // canonicalCarton.CartonType = ""; ** TBD verify if this field is required.
                canonicalCarton.CartonWeight = shpCart.Weight;
                canonicalCarton.TrackingNumber = shpCart.PinNumber;

                canonicalCarton.CartonItems = new List<CartonItem>();
                foreach (Nri.ShippingConfirmation.ShipmentCartonItem shpCartItem in shpCart.ShipmentCartonItems)
                {
                    CartonItem canonicalCartonItem = new CartonItem();
                    canonicalCartonItem.CartonNbr = shpCart.Sscc;
                    canonicalCartonItem.ItemNumber =  shpCartItem.ItemNumber; 
                    canonicalCartonItem.ItemUom = shpCartItem.UomDescription;
                    canonicalCartonItem.QtyShipped = Convert.ToInt32(shpCartItem.Quantity);

                    canonicalCarton.CartonItems.Add(canonicalCartonItem);
                }

                canonical.Cartons.Add(canonicalCarton);
            }

            return SerializeCanonicalShippingConfirmation.ToJson(canonical);
            
        }

        private static string GetDataSiteId(Nri.ShippingConfirmation.ShippingConfirmation message)
        {
            string dataSiteID;
            if (message.Data.ClientWarehouseCode == null || message.Data.ClientWarehouseCode == "")
            {
                throw new Exception("Shipping Confirmation Mapping Error: Client Warehouse Code (aka Site), at the data level, is not populated.  This is a required field " +
                    "for processing the message.  It should contain the warehouse ID that was provided in the shipping order.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
            }
            else
            {
                dataSiteID = message.Data.ClientWarehouseCode;
            }

            return dataSiteID;
        }

        private static string GetHeaderSiteId(Nri.ShippingConfirmation.ShippingConfirmation message)
        {
            string headerSiteID = "";
            if (message.ClientWarehouseCode == null || message.ClientWarehouseCode == "")
            {
                throw new Exception("Shipping Confirmation Mapping Error: Client Warehouse Code (aka Site), at the header level, is not populated.  This is a required field " +
                    "for processing the message.  It should contain the actual NRI warehouse that shipped the product.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
            }
            else
            {
                headerSiteID = message.ClientWarehouseCode;
            }
            return headerSiteID;
        }

        /// <summary>
        /// Converts the NRI cancelled shipping orders into a zero shipped, shipping confirmation canonical message.
        /// </summary>
        /// <param name="message">Cancelled shipping order</param>
        /// <returns>zero shipped canonical shipping confirmation.</returns>
        public static string ConvertCancelled (Nri.CancelledShippingOrder.CancelledShippingOrder message)         
        {
            //Add code to convert message to the canonical format
            CanonicalShippingConfirmation canonical = new CanonicalShippingConfirmation();

            canonical.PickNum = "1166";//message.PickNumber;
            canonical.OrderStatus = message.Status;
            canonical.NriOrderNbr = message.DocumentNumber.ToString();
            if (message.ClientReferenceNumber1 == null || message.ClientReferenceNumber1 == "")
                throw new Exception("Cancelled Shipping Order Mapping Error: Herschel's Order Number (called 'Client Reference Number 1' in the data) is not populated.  This is a required field " +
                    "for processing the message.  The NRI Order Number is: " + message.DocumentNumber.ToString());
            else
            {
                if (message.ClientReferenceNumber1.IndexOf('.') > 0)
                    canonical.OrderNbr = message.ClientReferenceNumber1.Substring(0, message.ClientReferenceNumber1.IndexOf('.'));
                else
                    canonical.OrderNbr = message.ClientReferenceNumber1;
            }
            canonical.ShippingDate = Convert.ToDateTime(message.LastShipDate);
            canonical.OrderDate = Convert.ToDateTime( message.DocumentDate);
            canonical.Bol = "";
            canonical.Customer = message.ShipToCustomerCode;
            canonical.ShipmentType = "";
            canonical.CarrierCode = "";
            canonical.Freight = 0;
            canonical.TotalQtyShipped = 0;
            canonical.TotalCartons = 0;
            canonical.TotalWeight = 0;
            canonical.CancellationReason = message.CancellationReason;

            if (message.Data != null)
            {
                //if (message.Data.FulfillType != null)
                    canonical.FulfillType = message.Data.FulfillType;
                /*else
                    throw new Exception("Cancelled Shipping Order Mapping Error: The Fulfill Type in null.  This is a required field " +
                        "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);*/

                if (message.Data.WarehouseCode == null || message.Data.WarehouseCode == "")
                    throw new Exception("Cancelled Shipping Order Mapping Error: The Warehouse Code is not populated.  This is a required field " +
                        "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
                else
                    canonical.Site = message.Data.WarehouseCode;
            }
            else
                throw new Exception("Cancelled Shipping Order Mapping Error: The Data section is null, Warehouse Code is a required field. " +
                    " The Herschel Order Number is: " + message.ClientReferenceNumber1);

            
            //Loop through Cancelled Lines Items and populate canonical.lineItems
            canonical.LineItems = new List<LineItem>();
            foreach (Nri.CancelledShippingOrder.OrderLine line in message.OrderLines)
            {
                LineItem canonicalItem = new LineItem();
                canonicalItem.LineNbr = line.ClientLineNumber;

                if (line.ItemNumber == null || line.ItemNumber == "")
                {
                    throw new Exception("Cancelled Shipping Order Mapping Error: The Item Number is not populated.  This is a required field " +
                        "for processing the message.  The Herschel Order Number is: " + message.ClientReferenceNumber1);
                }
                else
                {
                    canonicalItem.ItemNbr = line.ItemNumber; 
                }

                canonicalItem.QtyOrdered = line.QuantityOrdered;
                canonicalItem.QtyShipped = 0;
                canonicalItem.CancellationReason = ""; //**TBD
                canonical.LineItems.Add(canonicalItem);
            }


           

            return SerializeCanonicalShippingConfirmation.ToJson(canonical);
        }
    }
}