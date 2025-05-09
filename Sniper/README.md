# Twitter Sniper

Real-time Twitter monitoring tool that tracks and displays tweets matching your search criteria.

## Setup

1. Add your Twitter API credentials to `appsettings.json`:
```json
{
  "TwitterAPI": {
    "ConsumerKey": "your_consumer_key",
    "ConsumerSecret": "your_consumer_secret",
    "AccessToken": "your_access_token",
    "AccessTokenSecret": "your_access_token_secret",
    "SearchTerms": ["term1", "term2"],
    "RefreshIntervalSeconds": 30
  }
}
```

2. Run the application:
```bash
dotnet run
```

## Features

- Real-time tweet monitoring
- Custom search terms
- Configurable refresh rate
- Clean console output