using Microsoft.Extensions.Configuration;
using TwitterSniper.Model;

namespace TwitterSniper.Services
{
    public class ConfigService
    {
        private readonly IConfiguration _configuration;

        public ConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SearchParameters GetSearchParameters()
        {
            return _configuration.GetSection("TwitterAPI").Get<SearchParameters>();
        }

        public string GetConsumerKey() => _configuration["TwitterAPI:ConsumerKey"];
        public string GetConsumerSecret() => _configuration["TwitterAPI:ConsumerSecret"];
        public string GetAccessToken() => _configuration["TwitterAPI:AccessToken"];
        public string GetAccessTokenSecret() => _configuration["TwitterAPI:AccessTokenSecret"];
    }
}