namespace TwitterSniper.Model
{
    public class TweetInfo
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string AuthorUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Url => $"https://twitter.com/{AuthorUsername}/status/{Id}";

        public override string ToString()
        {
            return $"@{AuthorUsername}: {Text}\nTime: {CreatedAt}\nURL: {Url}";
        }
    }
}
