using System;
using System.Collections.Generic;
using BC.Integration;
using BC.Integration.Utility;
using System.Data;
using System.Data.SqlClient;

namespace BC.Integration.AppService.EpOffRampServiceTigersBC
{
    public class InventoryAdjustment
    {
        List<KeyValuePair<string, string>> configuration;
        bool enableInventoryAdjustment = true;
        string connectionString = "";

        public InventoryAdjustment(List<KeyValuePair<string, string>> configuration)
        {
            this.configuration = configuration;
            connectionString = Utilities.GetConfigurationValue(configuration, "ConnectionString");
            string val = Utilities.GetConfigurationValue(configuration, "EnableInventoryAdjustment");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                {
                    enableInventoryAdjustment = Convert.ToBoolean(val);
                }

                if (val == "1" || val == "0")
                {
                    enableInventoryAdjustment = Convert.ToBoolean(Convert.ToInt16(val));
                }
            }
        }
        public void Post(string EpFormattedJsonMessage)
        {
            if (enableInventoryAdjustment)
            {
                EPShippingConfirmation msg = EPShippingConfirmation.FromJson(EpFormattedJsonMessage);
                //Insert data into the SSIS integration inventory tables

                //Create connection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "InsertShipmentRecord";
                    connection.Open();
                    //Loop through line items and add them to the database
                    for (int i = 0; i < msg.ShipmentNotificationLines.Count; i++)
                    {
                        if (msg.ShipmentNotificationLines[i].QuantityShipped > 0) // if it's not cancelled, insert the line into the database. 
                        {
                            SqlParameter param1 = new SqlParameter("orderNumber", SqlDbType.VarChar, 50);
                            param1.Value = msg.ShipmentNotificationLines[i].OrderNbr;
                            command.Parameters.Add(param1);
                            SqlParameter param2 = new SqlParameter("nriOrderNumber", SqlDbType.VarChar, 50);
                            param2.Value = msg.ShipmentNotificationLines[i].NriOrderNumber == null ? msg.ShipmentNotificationLines[i].OrderNbr : msg.ShipmentNotificationLines[i].NriOrderNumber;
                            command.Parameters.Add(param2);
                            SqlParameter param3 = new SqlParameter("itemnmbr", SqlDbType.VarChar, 50);
                            param3.Value = msg.ShipmentNotificationLines[i].ItemNumber;
                            command.Parameters.Add(param3);
                            SqlParameter param4 = new SqlParameter("qtyshipped", SqlDbType.Int);
                            param4.Value = msg.ShipmentNotificationLines[i].QuantityShipped;
                            command.Parameters.Add(param4);
                            SqlParameter param5 = new SqlParameter("totalCartons", SqlDbType.Int);
                            param5.Value = msg.ShipmentNotificationLines[i].TotalCartons;
                            command.Parameters.Add(param5);
                            SqlParameter param6 = new SqlParameter("shippedDate", SqlDbType.Date);
                            //Need to convert dates
                            param6.Value = FromUnixTime(msg.ShipmentNotificationLines[i].ShippingDate);
                            command.Parameters.Add(param6);
                            SqlParameter param7 = new SqlParameter("trackingNumber", SqlDbType.VarChar, 100);
                            param7.Value = msg.ShipmentNotificationLines[i].Bol;
                            command.Parameters.Add(param7);
                            SqlParameter param8 = new SqlParameter("shipmentType", SqlDbType.VarChar, 50);
                            param8.Value = msg.ShipmentNotificationLines[i].ShipmentType;
                            command.Parameters.Add(param8);
                            SqlParameter param9 = new SqlParameter("carrierCode", SqlDbType.VarChar, 50);
                            param9.Value = msg.ShipmentNotificationLines[i].CarrierCode;
                            command.Parameters.Add(param9);
                            SqlParameter param10 = new SqlParameter("freight", SqlDbType.Decimal);
                            param10.Value = msg.ShipmentNotificationLines[i].Freight;
                            command.Parameters.Add(param10);
                            SqlParameter param11 = new SqlParameter("totalWeight", SqlDbType.Decimal);
                            param11.Value = Convert.ToDecimal(msg.ShipmentNotificationLines[i].TotalWeight); //string...
                            command.Parameters.Add(param11);
                            SqlParameter param12 = new SqlParameter("totalQuantityShipped", SqlDbType.Int);
                            param12.Value = msg.ShipmentNotificationLines[i].TotalQuantityShipped;
                            command.Parameters.Add(param12);
                            SqlParameter param13 = new SqlParameter("lnitemseq", SqlDbType.Int);
                            param13.Value = Convert.ToInt32(msg.ShipmentNotificationLines[i].LineNumber); //String...
                            command.Parameters.Add(param13);
                            SqlParameter param14 = new SqlParameter("customer", SqlDbType.VarChar, 50);
                            param14.Value = msg.ShipmentNotificationLines[i].Customer;
                            command.Parameters.Add(param14);
                            SqlParameter param15 = new SqlParameter("orderDate", SqlDbType.Date);
                            //Need to convert dates
                            param15.Value = FromUnixTime(msg.ShipmentNotificationLines[i].OrderDate);
                            command.Parameters.Add(param15);
                            SqlParameter param16 = new SqlParameter("quantityOrdered", SqlDbType.Int);
                            param16.Value = msg.ShipmentNotificationLines[i].QuantityOrdered;
                            command.Parameters.Add(param16);
                            SqlParameter param17 = new SqlParameter("fulfillType", SqlDbType.VarChar, 50);
                            if (msg.ShipmentNotificationLines[i].FulfillType != null)
                            {
                                param17.Value = msg.ShipmentNotificationLines[i].FulfillType;
                            }
                            else
                            {
                                param17.Value = "";
                            }
                            command.Parameters.Add(param17);
                            SqlParameter param18 = new SqlParameter("site", SqlDbType.VarChar, 50);
                            param18.Value = msg.ShipmentNotificationLines[i].Site;
                            command.Parameters.Add(param18);
                            SqlParameter param19 = new SqlParameter("filename", SqlDbType.VarChar, 200);
                            param19.Value = "HIP TigersShippingConfirmation " + DateTime.Now.ToString("yyyyMMdd");
                            command.Parameters.Add(param19);

                            int count = command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }
                    //close connection
                    connection.Close();
                }
            }
        }

        private DateTime FromUnixTime(double unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }
    }
}