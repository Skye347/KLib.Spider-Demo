using System;
using KLib.Spider;
using KLib.HTTP;
using System.Threading.Tasks;
using KLib.Rss;
using HtmlAgilityPack;

namespace RssGenerator
{
    class MySpider : KLib.Spider.SpiderBase
    {
        public MySpider()
        {
            startList = new System.Collections.Generic.List<string>
            {
                "http://subhd.com/do/26675245"
            };
        }
        public override SpiderRequest Start(SpiderResponse response)
        {
            RssDocument rssDoc = new RssDocument();
            if (response.request.Url== "http://subhd.com/do/26675245")
            {
                RssChannel channel1 = new RssChannel();
                channel1.Description = "The Expanse subtitles";
                channel1.Title = "The Expanse subtitles from Subhd.com";
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.httpResponse.body);
                var nodes=doc.DocumentNode.SelectNodes("//div[@class='dt_edition']/a");
                foreach(var node in nodes)
                {
                    var title = node.InnerHtml;
                    var href = "www.subhd.com/" + node.Attributes[0];
                    channel1.Items.AddFirst(new RssItem()
                    {
                        Title = title,
                        Link=href
                    });
                }
                rssDoc.Channels.AddFirst(channel1);
            }
            rssDoc.filePath = "test.xml";
            RssBuilder.Build(rssDoc);
            Spider.Stop();
            return null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //KLib.Rss.RssDocument doc=new RssDocument();
            while (true)
            {
                Task.Run(() =>
                {
                    Spider.loadSpider(new MySpider());
                    Spider.Run();
                }).Wait();
                System.Threading.Thread.Sleep(3600000);
            }
        }
    }
}