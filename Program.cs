// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;


var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true);

IConfiguration config = builder.Build();
string apiKey = config["YouTubeApiKey"];

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("API key is not set. Please set the YouTubeApiKey in appsettings.json.");
    return;
}

Console.Write("Please enter the YouTube channel ID: ");
string channelId = Console.ReadLine() ?? string.Empty;

if (string.IsNullOrEmpty(channelId))
{
    Console.WriteLine("Channel ID is not set. Please provide a valid YouTube channel ID.");
    return;
}

string cacheFilePath = "cache.json";

// Load cache from file
var cache = new Dictionary<string, string>();
if (File.Exists(cacheFilePath))
{
    var cacheJson = await File.ReadAllTextAsync(cacheFilePath);
    cache = JsonSerializer.Deserialize<Dictionary<string, string>>(cacheJson);
}

var youtubeService = new YouTubeService(new BaseClientService.Initializer()
{
    ApiKey = apiKey,
});

// Step 1: Get uploads playlist id
string uploadsPlaylistId;
if (cache.ContainsKey(channelId))
{
    uploadsPlaylistId = cache[channelId];
}
else
{
    var channelListRequest = youtubeService.Channels.List("contentDetails");
    channelListRequest.Id = channelId;
    var channelListResponse = await channelListRequest.ExecuteAsync();
    uploadsPlaylistId = channelListResponse.Items[0].ContentDetails.RelatedPlaylists.Uploads;
    cache[channelId] = uploadsPlaylistId;
}

// Save cache to file
var cacheJsonToSave = JsonSerializer.Serialize(cache);
await File.WriteAllTextAsync(cacheFilePath, cacheJsonToSave);

// Step 2: Get video IDs from current year
var videoIds = new List<string>();
string nextPageToken = null;
DateTime currentYearStart = new DateTime(DateTime.Now.Year, 1, 1);

do
{
    var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
    playlistItemsListRequest.PlaylistId = uploadsPlaylistId;
    playlistItemsListRequest.MaxResults = 50;
    playlistItemsListRequest.PageToken = nextPageToken;
    var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

    foreach (var playlistItem in playlistItemsListResponse.Items)
    {
        var videoPublishedAt = playlistItem.Snippet.PublishedAtDateTimeOffset.Value.DateTime;
        if (videoPublishedAt >= currentYearStart)
        {
            videoIds.Add(playlistItem.Snippet.ResourceId.VideoId);
        }
    }

    nextPageToken = playlistItemsListResponse.NextPageToken;
} while (nextPageToken != null);

// Step 3: Get video statistics and sort by views
var videoStats = new List<(string Title, long ViewCount, string VideoId)>();
        
for (int i = 0; i < videoIds.Count; i += 50)
{
    var videosRequest = youtubeService.Videos.List("snippet,statistics");
    videosRequest.Id = string.Join(',', videoIds.Skip(i).Take(50));
    var videosResponse = await videosRequest.ExecuteAsync();

    foreach (var item in videosResponse.Items)
    {
        videoStats.Add((item.Snippet.Title, (long)(item.Statistics.ViewCount ?? 0), item.Id));    }
}

// Sort videos by view count in descending order
var topVideos = videoStats.OrderByDescending(v => v.ViewCount).Take(10);

// Print top 10 videos
foreach (var video in topVideos)
{
    Console.WriteLine($"{video.Title} ({video.ViewCount} views) - https://www.youtube.com/watch?v={video.VideoId}");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();