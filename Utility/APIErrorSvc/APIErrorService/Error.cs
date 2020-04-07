using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BC.Integration.APIError
{
    public class Error
    {
        private string documentId;
        private string errorType;
        private int errorTypeId;
        private string errorId;
        private string errorMsg;
        private DateTime errorDate;
        private string location;
        private bool processed;
        private string division;
        private string season;
        private string customer;

        public  Error()
        {
        }

        public string DocumentId { get => documentId; set => documentId = value; }
       public int ErrorTypeId { get => errorTypeId; set => errorTypeId = value; }
        public string ErrorId { get => errorId; set => errorId = value; }
        public string ErrorMsg { get => errorMsg; set => errorMsg = value; }
        public string ErrorType { get => errorType; set => errorType = value; }
        public string Location { get => location; set => location = value; }
        public bool Processed { get => processed; set => processed = value; }
        public string Division { get => division; set => division = value; }
        public string Season { get => season; set => season = value; }
        public string Customer { get => customer; set => customer = value; }
        public DateTime ErrorDate { get => errorDate; set => errorDate = value; }

        public string CreateErrorMsg()
        {
            String msg;
            msg = "An error occured while processing a transaction in location. " + this.Location + " for customer :" + this.Customer + " The details or the errors are below : ";
            msg += "\nError type: " + this.ErrorType;
            msg += "\nTransaction type: " + this.ErrorTypeId;
            msg += "\nDocument Id: " + this.DocumentId;
            msg += "\nDivision: " + this.Division;

            return msg.ToString();
        }

    }
}