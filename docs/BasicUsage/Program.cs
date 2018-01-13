using System;
using System.Threading.Tasks;

namespace BasicUsage
{
    class Program
    {
        static void Main(string[] args)
        {

            Task.Run(async () =>
                {
                    var jsonString = "{\"Number\":2, \"OtherNumber\":23445,\"Status\":1}";

                         var thisWillError = Newtonsoft.Json.JsonConvert.DeserializeObject<SimpleClass>(jsonString);

    var analyzer = new ApiAnalysis.SimpleJsonAnalyzer();
    var results = await analyzer.AnalyzeJsonAsync(jsonString, typeof(SimpleClass));

    foreach (var result in results)
    {
        Console.WriteLine(result);
    }

                    Console.WriteLine("Finished. Press any key to exit.");
                    Console.ReadKey(true);
                })
                .Wait();
        }
    }

    public class SimpleClass
    {
        public byte Number { get; set; }
        public byte OtherNumber { get; set; }
        public string Name { get; set; }
    }
}
