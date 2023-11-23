using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wa77cher.Scraper
{
    internal class SteamHoursScraper
    {
        private readonly HttpClient httpClient;

        public SteamHoursScraper()
        {
            httpClient = new HttpClient();
        }

        public async Task<double?> ScrapeProfile(string id)
        {
            try
            {
                var url = $"https://steamcommunity.com/id/{id}";

                // Fetch HTML content from the website
                var html = await httpClient.GetStringAsync(url);

                // Parse HTML using HtmlAgilityPack
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var hours = ExtractSteamWeeklyHours(document);

                return hours;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private double ExtractSteamWeeklyHours(HtmlDocument document)
        {
            var element = document.DocumentNode.SelectSingleNode("//div[@class='recentgame_quicklinks recentgame_recentplaytime']//h2");
            var hours = Convert.ToDouble(element?.InnerText.Trim().Split(" ")[0]);
            return hours;
        }
    }
}
