using System;
using System.Threading.Tasks;
using System.Net.Http;
using Polly;

namespace PollyTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var httpClient = new HttpClient();
            var response = await Policy
                .HandleResult<HttpResponseMessage>(message=>!message.IsSuccessStatusCode)
                //.WaitAndRetryAsync(3,i=>TimeSpan.FromSeconds(5),(result,timeSpan,retryCount,context)=>
                //{
                //    Console.WriteLine("Request failed with {0} Waiting {1} before next retry. Retry attempt {2}", result.Result.StatusCode,timeSpan,retryCount);
                //})
                //exponential backoff
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5)
                },(result,timeSpan,retryCount,context)=>
                {
                    Console.WriteLine("Request failed with {0} Waiting {1} before next retry. Retry attempt {2}", result.Result.StatusCode, timeSpan, retryCount);
                })

                .ExecuteAsync(()=>httpClient.GetAsync("http://www.dinamalar.com"));
            if (response.IsSuccessStatusCode)
                Console.WriteLine("Response was successful");
            else
                Console.WriteLine("Response failed. Status code {0}",response.StatusCode);
            Console.ReadKey();
        }
    }
}
