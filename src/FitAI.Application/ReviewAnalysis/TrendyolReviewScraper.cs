using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;
using Volo.Abp.DependencyInjection;
using System;
namespace FitAI.ReviewAnalysis;

public class TrendyolReviewScraper :
    ITrendyolReviewScraper,
    ITransientDependency
{
    public async Task<List<ReviewDto>> GetReviewsAsync(string url)
    {
    
        // Browser download (cache’e alınabilir ama şimdilik OK)
        //await new BrowserFetcher().DownloadAsync();

        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[]
            {
                "--no-sandbox",
                "--disable-setuid-sandbox",
                "--disable-blink-features=AutomationControlled"
            }
        });

        var page = await browser.NewPageAsync();

        // 🔥 Anti-bot basic bypass
        await page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120 Safari/537.36"
        );

        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = 1280,
            Height = 800
        });

        // ❌ redirect hack SİLİNDİ (HATALIYDI)

        await page.GoToAsync(url, new NavigationOptions
{
    WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
    Timeout = 60000
});

// Kullanıcının verdiği kısa link gerçek linke döndü mü?
url = page.Url.Split('?')[0].TrimEnd('/');

if (!url.EndsWith("/yorumlar"))
{
    url += "/yorumlar";

    await page.GoToAsync(url, new NavigationOptions
    {
        WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
        Timeout = 60000
    });
}
try
{
    await page.WaitForSelectorAsync(".review", new WaitForSelectorOptions
    {
        Timeout = 10000
    });
}
catch
{
    Console.WriteLine("Review container bulunamadı.");
}

        // 🔥 scroll (Trendyol lazy load için kritik)
        await page.EvaluateFunctionAsync(@"async () => {
            for (let i = 0; i < 5; i++) {
                window.scrollBy(0, document.body.scrollHeight);
                await new Promise(r => setTimeout(r, 1500));
            }
        }");

        await Task.Delay(5000);

        // 🔍 DEBUG (Swagger / log kontrolü için)
        var html = await page.GetContentAsync();
Console.WriteLine("Final URL = " + page.Url);
System.Console.WriteLine(html);

// 💡 gerçekçi selectorlar
var reviewElements =
    await page.QuerySelectorAllAsync(".review .review-comment span.review-comment span");

var dateElements =
    await page.QuerySelectorAllAsync(".review .detail-item.date");

Console.WriteLine($"Review count = {reviewElements.Length}");
Console.WriteLine($"Date count = {dateElements.Length}");

        var reviews = new List<ReviewDto>();

        if (reviewElements == null || reviewElements.Length == 0)
            return reviews;

        var count = reviewElements.Length;

        for (int i = 0; i < count; i++)
        {
            var text = await reviewElements[i]
                .EvaluateFunctionAsync<string>("e => e.innerText");

            string time = "";

            if (i < dateElements.Length)
            {
                time = await dateElements[i]
    .EvaluateFunctionAsync<string>(
        @"e => Array.from(e.querySelectorAll('span'))
                     .map(x => x.innerText)
                     .join(' ')");
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                reviews.Add(new ReviewDto
                {
                    Text = text.Trim(),
                    Sentiment = "neutral",
                    Issue = null,
                    Time = time?.Trim() ?? ""
                });
            }
        }

        return reviews;
    }
}