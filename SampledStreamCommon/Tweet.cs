namespace SampledStreamCommon
{
    /// <summary>
    /// Class for the contents of a single tweet
    /// </summary>
    public class Tweet
    {
        public string? Contents { get; }

        public Tweet(string? contents)
        {
            Contents = contents;
        }
    }
}
