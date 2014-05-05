using System;

namespace ProfileTimelineView.Twitter
{
    public class TweetData : TimelineData
    {
        public string Name { get; set; }
        public string ScreenName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}