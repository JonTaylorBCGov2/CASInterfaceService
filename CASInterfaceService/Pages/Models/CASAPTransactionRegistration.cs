﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CASInterfaceService.Pages.Models
{
    public class CASAPTransactionRegistration
    {
        List<CASAPTransaction> casAPTransactionList;
        static CASAPTransactionRegistration casregd = null;

        // TEST  - molson.cas.gov.bc.ca - 142.34.166.75  - 7015
        // TRAIN - molson.cas.gov.bc.ca - 142.34.166.75  - 7013
        // PROD  - labatt.cas.gov.bc.ca - 142.34.166.201 - 7010

        //private const string URL = "https://<server>:<port>ords/cas/cfs/apinvoice/";
        private const string URL = "https://molson.cas.gov.bc.ca:7015/ords/cas/cfs/apinvoice/";
        //private const string TokenURL = "https://<server>:<port>/ords/casords/oauth/token";
        //private const string TokenURL = "https://molson.cas.gov.bc.ca:7015/ords/casords/oauth/token";
        private const string TokenURL = "https://wsgw.test.jag.gov.bc.ca/ords/casords/oauth/token";
        private const string clientID = "123";
        private const string secret = "456";

        private CASAPTransactionRegistration()
        {
            casAPTransactionList = new List<CASAPTransaction>();
        }

        public static CASAPTransactionRegistration getInstance()
        {
            if (casregd == null)
            {
                casregd = new CASAPTransactionRegistration();
                return casregd;
            }
            else
            {
                return casregd;
            }
        }

        public void Add(CASAPTransaction casapTransaction)
        {
            casAPTransactionList.Add(casapTransaction);
        }
        //public String Remove(String registrationNumber)
        //{
        //    for (int i = 0; i < casAPTransactionList.Count; i++)
        //    {
        //        CASAPTransaction casn = casAPTransactionList.ElementAt(i);
        //        if (casn.RegistrationNumber.Equals(registrationNumber))
        //        {
        //            casAPTransactionList.RemoveAt(i);//update the new record
        //            return "Delete successful";
        //        }
        //    }
        //    return "Delete un-successful";
        //}

        public List<CASAPTransaction> getAllCASAPTransaction()
        {
            return casAPTransactionList;
        }
        //public String UpdateStudent(Student std)
        //{
        //    for (int i = 0; i < studentList.Count; i++)
        //    {
        //        Student stdn = studentList.ElementAt(i);
        //        if (stdn.RegistrationNumber.Equals(std.RegistrationNumber))
        //        {
        //            studentList[i] = std;//update the new record
        //            return "Update successful";
        //        }
        //    }
        //    return "Update un-successful";
        //}

        public List<CASAPTransaction> getUpdateCASAPTransaction()
        {
            return casAPTransactionList;
        }

        public async void sendTransactionsToCAS(CASAPTransaction casapTransaction)
        {
            HttpClient client = new HttpClient();

            try
            {
                Console.WriteLine("Starting sendTransactionsToCAS.");

                Console.WriteLine("Start GetToken");
                var restClient = new RestClient(TokenURL);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&client_id=" + clientID + "&client_secret=" + secret, ParameterType.RequestBody);
                IRestResponse response = restClient.Execute(request);
                Console.WriteLine("Token: " + response.ToString());
                Console.WriteLine("End GetToken");



                Console.WriteLine("Start GetResponse");
                var restResponse = new RestClient(URL);
                var requestResponse = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&token=" + response, ParameterType.RequestBody);
                IRestResponse responseValue = restClient.Execute(request);
                Console.WriteLine("Response: " + responseValue.ToString());
                Console.WriteLine("Start GetResponse");

                //Console.WriteLine("Start TEST");
                //HttpWebRequest HttpWReq =
                //(HttpWebRequest)WebRequest.Create(TokenURL);

                //HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();
                //// Insert code that uses the response object.
                //Console.WriteLine(HttpWResp.StatusCode.ToString());
                //Console.WriteLine(HttpWResp.StatusDescription.ToString());
                //HttpWResp.Close();
                //Console.WriteLine("End TEST");


                // Send current data in memory to CAS
                //HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenURL);
                

                // Send content to CAS
                var content = new StringContent(casapTransaction.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage responseX = await client.PostAsync(client.BaseAddress.ToString(), content);
            }
            catch (Exception e)
            {
                var errorContent = new StringContent(casapTransaction.ToString(), Encoding.UTF8, "application/json");
                Console.WriteLine("Error in sendTransactionsToCAS. " + client.BaseAddress.ToString() + errorContent + client + e.ToString());
                throw e;
            }
        }
    }
}
