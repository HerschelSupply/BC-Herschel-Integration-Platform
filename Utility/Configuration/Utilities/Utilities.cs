﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BC.Integration.Utility
{
    public class Utilities
    {
        public static string GetServerIpAddress()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address.ToString();
            }
        }

        /// <summary>
        /// Helper method to get configuration values from the collection. This method will always return 
        /// the first occurance of the Key in the collection then exit the loop.
        /// </summary>
        /// <param name="configuration">Collection of configurations</param>
        /// <param name="key">Configuration key to find in the collection</param>
        /// <returns>Value of the requested key</returns>
        public static string GetConfigurationValue(List<KeyValuePair<string, string>> configuration, string key)
        {
            //This method will always return the first occurance of the Key in the collection then exit the loop.
            foreach (KeyValuePair<string, string> pr in configuration)
            {
                if (pr.Key == key)
                    return pr.Value;
            }
            return "";
        }

        #region String Conversion Mapping Functions

        /// <summary>
        /// This method is used to facilitate the mapping of integers from strings.  If the string is empty it will map to '0'.
        /// </summary>
        /// <param name="data">string value to convert</param>
        /// <param name="n">returned value generated by the string</param>
        /// <returns>This will be true if the string was a int otherwise it will return false.</returns>
        public static bool ProcessIntString(string data, out int n)
        {
            if(data == "")
            {
                n = 0;
                return true;
            }

            return int.TryParse(data, out n);
        }

        /// <summary>
        /// This method is used to facilitate the mapping of doubles from strings.  If the string is empty it will map to '0'.
        /// </summary>
        /// <param name="data">string value to convert</param>
        /// <param name="n">returned value generated by the string</param>
        /// <returns>This will be true if the string was a int otherwise it will return false.</returns>
        public static bool ProcessDoubleString(string data, out double d)
        {
            if(data == "")
            {
                d = 0;
                return true;
            }

            return double.TryParse(data, out d);
        }

        /// <summary>
        /// This method is used to facilitate the mapping of decimals from strings.  If the string is empty it will map to '0'.
        /// </summary>
        /// <param name="data">string value to convert</param>
        /// <param name="n">returned value generated by the string</param>
        /// <returns>This will be true if the string was a int otherwise it will return false.</returns>
        public static bool ProcessDecimalString(string data, out decimal d)
        {
            if(data == "")
            {
                d = 0;
                return true;
            }

            return decimal.TryParse(data, out d);
        }

        #endregion

    }

    /// <summary>
    /// Generic Retry
    /// </summary>
    /// <typeparam name="TResult">return type</typeparam>
    /// <param name="action">Method needs to be executed</param>
    /// <param name="retryInterval">Retry interval</param>
    /// <param name="retryCount">Retry Count</param>
    /// <param name="expectedResult">Expected Result</param>
    /// <param name="isExpectedResultEqual">true/false to check equal 
    /// or not equal return value</param>
    /// <param name="isSuppressException">
    /// Suppress exception is true / false</param>
    /// <returns></returns>
    public class Retry
    {
        public static TResult Execute<TResult>(
          Func<TResult> action,
          TimeSpan retryInterval,
              int retryCount,
          TResult expectedResult,
          bool isExpectedResultEqual = true,
              bool isSuppressException = true)
        {
            TResult result = default(TResult);

            bool succeeded = false;
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);
                    // Execute method
                    result = action();

                    if (isExpectedResultEqual)
                        succeeded = result.Equals(expectedResult);
                    else
                        succeeded = !result.Equals(expectedResult);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                if (succeeded)
                    return result;
            }

            if (!isSuppressException)
                throw new AggregateException(exceptions);
            else
                return result;
        }
    }


}
