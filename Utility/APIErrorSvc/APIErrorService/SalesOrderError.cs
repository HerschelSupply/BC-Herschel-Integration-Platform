using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace BC.Integration.APIError
{


    [XmlRoot("Error", Namespace = "http://Corp.Integration.Maps.B2BOrderShipment")]
    public partial class SalesOrderError : Error
    {
       
        private string pickNumber;
        private string invoiceNumber;

    

        public SalesOrderError()
        {
            this.ErrorType = "Sales Order";
            this.ErrorTypeId = 850;
        }


        public string PickNumber { get => pickNumber; set => pickNumber = value; }
        public string InvoiceNumber { get => invoiceNumber; set => invoiceNumber = value; }



        /// <summary>



        /// <summary>
        /// Convert the order Object into a Data Table Object that can be passed 
        /// as a parameter to a Stored Procedure
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable table = new DataTable("LINES");
            table.Columns.Add("SOPNUMBE", typeof(string));
            table.Columns.Add("LINEITEM", typeof(int));
            table.Columns.Add("QTYFULFI", typeof(int));
            table.Columns.Add("QTYTOINV", typeof(int));

          /*  foreach (LineItem line in LineItems)
            {
                if (!(line.ItemNumber == "FREIGHT"))
                {
                    table.Rows.Add(Header.SopNumber, line.LineItemSequence, line.QtyFulfilled, line.QtyToInvoice);

                }
            }*/

            return table;

        }



    }

}

