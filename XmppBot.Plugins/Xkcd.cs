﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Fizzler.Systems.HtmlAgilityPack;

using HtmlAgilityPack;

using XmppBot.Common;

namespace XmppBot.Plugins
{
    [Export(typeof(IXmppBotPlugin))]
    public class Xkcd : XmppBotPluginBase, IXmppBotPlugin
    {
        private static readonly Random _random = new Random();

        public override string EvaluateEx(ParsedLine line)
        {
            if (line.Raw.IndexOf("http://xkcd", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return "(haha)";
            }

            if (line.IsCommand && line.Command == "xkcd")
            {
                var html = GetHtml("http://dynamic.xkcd.com/random/comic/").Result;

                return Format(Scrape(html));
            }

            return null;
        }

        private string Format(Tuple<HtmlNode, string> scrape)
        {
            return scrape.Item1.Attributes["src"].Value;
        }

        private Tuple<HtmlNode, string> Scrape(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var image = document.DocumentNode.QuerySelectorAll("#comic img").FirstOrDefault();
            var title = document.DocumentNode.QuerySelectorAll("#ctitle").FirstOrDefault();

            return Tuple.Create(image, title != null ? title.InnerText : "");

        }

        private async Task<string> GetHtml(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
            }
        }

        public override string Name
        {
            get { return "Xkcd"; }
        }
    }
}