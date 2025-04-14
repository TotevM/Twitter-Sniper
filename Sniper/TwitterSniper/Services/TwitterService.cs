using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using TwitterSniper.Model;

namespace TwitterSniper.Services
{
    public class TwitterService
    {
        private readonly TwitterClient _twitterClient;
        private readonly ConfigService _configService;
        private readonly ILogger<TwitterService> _logger;
        private Dictionary<long, DateTime> _processedTweets = new Dictionary<long, DateTime>();
        private const int MAX_CACHE_SIZE = 1000;

        public TwitterService(ConfigService configService, ILogger<TwitterService> logger)
        {
            _configService = configService;
            _logger = logger;

            _twitterClient = new TwitterClient(
                _configService.GetConsumerKey(),
                _configService.GetConsumerSecret(),
                _configService.GetAccessToken(),
                _configService.GetAccessTokenSecret());
        }

        public async Task StartMonitoring(CancellationToken cancellationToken)
        {
            var searchParams = _configService.GetSearchParameters();

            _logger.LogInformation("Starting Twitter monitoring for terms: {terms}",
                string.Join(", ", searchParams.SearchTerms));

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await MonitorSearchTerms(searchParams);
                    await Task.Delay(searchParams.RefreshIntervalSeconds * 1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Monitoring canceled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while monitoring Twitter");
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        private async Task MonitorSearchTerms(SearchParameters parameters)
        {
            foreach (var term in parameters.SearchTerms)
            {
                var searchParams = new SearchTweetsParameters(term)
                {
                    SearchType = SearchResultType.Recent,
                    PageSize = 100
                };

                var tweets = await _twitterClient.Search.SearchTweetsAsync(searchParams);
                var newTweets = FilterNewTweets(tweets);

                if (newTweets.Any())
                {
                    _logger.LogInformation("Found {count} new tweets for term: {term}",
                        newTweets.Count, term);

                    ProcessNewTweets(newTweets);
                }
            }

            CleanupCache();
        }

        private List<ITweet> FilterNewTweets(IEnumerable<ITweet> tweets)
        {
            return tweets
                .Where(t => !_processedTweets.ContainsKey(t.Id))
                .ToList();
        }

        private void ProcessNewTweets(List<ITweet> tweets)
        {
            foreach (var tweet in tweets)
            {
                var tweetModel = new TweetInfo
                {
                    Id = tweet.Id,
                    Text = tweet.Text,
                    AuthorUsername = tweet.CreatedBy.ScreenName,
                };

                Console.WriteLine("\n--------------------");
                Console.WriteLine(tweetModel.ToString());
                Console.WriteLine("--------------------\n");

                _processedTweets[tweet.Id] = DateTime.UtcNow;
            }
        }

        private void CleanupCache()
        {
            if (_processedTweets.Count > MAX_CACHE_SIZE)
            {
                var oldestEntries = _processedTweets
                    .OrderBy(x => x.Value)
                    .Take(_processedTweets.Count - MAX_CACHE_SIZE / 2);

                foreach (var entry in oldestEntries)
                {
                    _processedTweets.Remove(entry.Key);
                }
            }
        }
    }
}