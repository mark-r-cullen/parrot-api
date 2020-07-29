using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarkC
{
    public static class ParrotText
    {
        [FunctionName("ParrotText")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string text = data?.text;
            string icon = data?.icon;
            string blankIcon = data?.blankIcon ?? ":blank:";

            if (string.IsNullOrWhiteSpace(text))
            {
                return new BadRequestResult();
            }

            if (string.IsNullOrWhiteSpace(icon))
            {
                return new BadRequestResult();
            }

            var letters = new List<string[]>();

            foreach (var character in text.ToUpper())
            {
                var charStr = character.ToString();

                if (charStr == " ")
                {
                    charStr = "Space";
                }

                var letter = File.ReadAllLines(context.FunctionAppDirectory + @"\Resources\" + charStr + ".txt");
                letters.Add(letter);
            }

            var str = new StringBuilder();

            for (var j = 0; j < letters[0].Length; j++)
            {
                var line = string.Empty;

                for (var i = 0; i < letters.Count; i++)
                {
                    line += letters[i][j].Replace(" ", blankIcon)
                                         .Replace("x", icon);
                }

                str.AppendLine(line);
            }

            return new OkObjectResult(str.ToString());
        }
    }
}
