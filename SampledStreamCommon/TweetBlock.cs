namespace SampledStreamCommon
{
#pragma warning disable IDE1006 // Disable warnings for unconventional field names
    /// <summary>
    /// Class for the data for an incoming tweet
    /// </summary>
    public class TweetData
    {
        public string? id { get; set; }
        public string? text { get; set; }
    }

    /// <summary>
    /// Class for an incoming tweet
    /// </summary>
    public class Tweet
    {
        public TweetData? data { get; set; }
    }
#pragma warning restore IDE1006 // Restore warnings for unconventional field names

    /// <summary>
    /// Class for the contents of a block of tweets from the Twitter stream
    /// </summary>
    public class TweetBlock
    {
        public string Contents { get; }

        public TweetBlock(string contents)
        {
            Contents = contents;
        }
    }
}
