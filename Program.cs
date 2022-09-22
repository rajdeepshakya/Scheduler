using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EarningScheduler
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Program program = new Program();
            //var data = Task.Run=>(program.GetBankTransaction());

            var data = Task.Run(() => program.GetBankTransaction()).Result;
        }

        public async Task<string> GetBankTransaction()
        {
            string resBody = "";
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                  
                   

                    var dd = new EncryptedParamsRequestModel
                    {
                        EncryptedParams= "RajdeepPayMasta"
                    };
                    var JsonReq1 = JsonConvert.SerializeObject(dd);
                    var enc = await Encrypt(JsonReq1);
                    var req = new RequestModel
                    {
                        Value = enc
                    };
                    var JsonReq = JsonConvert.SerializeObject(req);
                    var content = new StringContent(JsonReq, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("https://api.paymasta.co/api/CommonController/InsertDailyEarningByScheduler", content);
                    response.EnsureSuccessStatusCode();
                    resBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(resBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
                return resBody;
            }
        }

        public async Task<string> Encrypt(string Encryptval)
        {
            try
            {
                string textToEncrypt = Encryptval;
                string ToReturn = "";
                string publickey = "rajdeeps";
                string secretkey = "engineer";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }

    public class RequestModel
    {
        public string Value { get; set; }
    }

    public class EncryptedParamsRequestModel
    {
        public string EncryptedParams { get; set; }
    }
}
