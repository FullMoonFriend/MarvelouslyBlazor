namespace BlazingServers.Data
{
    public class MarvelSeriesResponse
    {
        public class Rootobject
        {
            public int code { get; set; }
            public string status { get; set; }
            public string copyright { get; set; }
            public string attributionText { get; set; }
            public string attributionHTML { get; set; }
            public string etag { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public int offset { get; set; }
            public int limit { get; set; }
            public int total { get; set; }
            public int count { get; set; }
            public Result[] results { get; set; }
        }

        public class Result
        {
            public int id { get; set; }
            public string title { get; set; }
            public object description { get; set; }
            public string resourceURI { get; set; }
            public Url[] urls { get; set; }
            public int startYear { get; set; }
            public int endYear { get; set; }
            public string rating { get; set; }
            public string type { get; set; }
            public string modified { get; set; }
            public Thumbnail thumbnail { get; set; }
            public Creators creators { get; set; }
            public Characters characters { get; set; }
            public Stories stories { get; set; }
            public Comics comics { get; set; }
            public Events events { get; set; }
            public object next { get; set; }
            public object previous { get; set; }
        }

        public class Thumbnail
        {
            public string path { get; set; }
            public string extension { get; set; }
        }

        public class Creators
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public Item[] items { get; set; }
            public int returned { get; set; }
        }

        public class Item
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
            public string role { get; set; }
        }

        public class Characters
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public object[] items { get; set; }
            public int returned { get; set; }
        }

        public class Stories
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public Item1[] items { get; set; }
            public int returned { get; set; }
        }

        public class Item1
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        public class Comics
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public Item2[] items { get; set; }
            public int returned { get; set; }
        }

        public class Item2
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Events
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public object[] items { get; set; }
            public int returned { get; set; }
        }

        public class Url
        {
            public string type { get; set; }
            public string url { get; set; }
        }

    }
}
