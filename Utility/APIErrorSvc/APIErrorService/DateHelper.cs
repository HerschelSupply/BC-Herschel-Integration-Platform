using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BC.Integration.AppService.APIError
{
    public static class DateHelper
    {
        public static string ConvertToLocalDate(DateTime inputDate, string zone)
        {
            TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById(zone);
            string date = TimeZoneInfo.ConvertTime(inputDate, tzInfo).ToString("yyyy-MM-dd");    
            return date;
        }
    }
   
}