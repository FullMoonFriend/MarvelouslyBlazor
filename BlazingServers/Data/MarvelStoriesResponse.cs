namespace BlazingServers.Data
{
    public class MarvelStoriesResponse
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
            public string description { get; set; }
            public string resourceURI { get; set; }
            public string type { get; set; }
            public DateTime modified { get; set; }
            public object thumbnail { get; set; }
            public Creators creators { get; set; }
            public Characters characters { get; set; }
            public Series series { get; set; }
            public Comics comics { get; set; }
            public Events events { get; set; }
            public Originalissue originalIssue { get; set; }
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
            public Item1[] items { get; set; }
            public int returned { get; set; }
        }

        public class Item1
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Series
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

        public class Comics
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public Item3[] items { get; set; }
            public int returned { get; set; }
        }

        public class Item3
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Events
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public Item4[] items { get; set; }
            public int returned { get; set; }
        }

        public class Item4
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Originalissue
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }


    }
}
