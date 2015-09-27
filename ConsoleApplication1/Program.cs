using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello, World!");

            string baseAddress = "http://localhost:9966";
            HttpClient client = new HttpClient();
            HttpRequestMessage batchRequest = new HttpRequestMessage(HttpMethod.Post, baseAddress + "/petclinic/batchy")
            {
                Content = new MultipartContent("mixed")
                {
                    // POST http://localhost:9966/petclinic/mockServlet/2
                    new HttpMessageContent(new HttpRequestMessage(HttpMethod.Post, baseAddress + "/petclinic/mockServlet/2")
                    {
                        Content = new ObjectContent<string>("my value", new JsonMediaTypeFormatter())
                    }),
                    // GET http://localhost:9966/petclinic/mockServlet/1
                    new HttpMessageContent(new HttpRequestMessage(HttpMethod.Get, baseAddress + "/petclinic/mockServlet/1"))
                }
            };

            HttpResponseMessage batchResponse = client.SendAsync(batchRequest).Result;

            string res = batchResponse.Content.ReadAsStringAsync().Result;
            string contentType = batchResponse.Content.Headers.GetValues("Content-Type").First();
            Console.WriteLine("Content-Type: " + contentType);
            Console.WriteLine();
            Console.Write(res);

            var streamContent = new StringContent(res);
            streamContent.Headers.Remove("Content-Type");
            streamContent.Headers.Add("Content-Type", contentType);
            MultipartStreamProvider streamProvider = streamContent.ReadAsMultipartAsync().Result;
            foreach (var content in streamProvider.Contents)
            {
                HttpResponseMessage response = content.ReadAsHttpResponseMessageAsync().Result;

                // Do something with the response messages
            }

            Console.ReadLine();
        }

    }
}
