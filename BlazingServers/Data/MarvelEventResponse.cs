namespace BlazingServers.Data
{
    public class MarvelEventResponse
    {
        public class Characters
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public List<Item> items { get; set; }
            public int returned { get; set; }
        }

        public class Comics
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public List<Item> items { get; set; }
            public int returned { get; set; }
        }

        public class Creators
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public List<Item> items { get; set; }
            public int returned { get; set; }
        }

        public class Data
        {
            public int offset { get; set; }
            public int limit { get; set; }
            public int total { get; set; }
            public int count { get; set; }
            public List<Result> results { get; set; }
        }

        public class Item
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
            public string role { get; set; }
            public string type { get; set; }
        }

        public class Next
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Previous
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Result
        {
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string resourceURI { get; set; }
            public List<Url> urls { get; set; }
            public DateTime modified { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public Thumbnail thumbnail { get; set; }
            public Creators creators { get; set; }
            public Characters characters { get; set; }
            public Stories stories { get; set; }
            public Comics comics { get; set; }
            public Series series { get; set; }
            public Next next { get; set; }
            public Previous previous { get; set; }
        }

        public class Root
        {
            public int code { get; set; }
            public string status { get; set; }
            public string copyright { get; set; }
            public string attributionText { get; set; }
            public string attributionHTML { get; set; }
            public string etag { get; set; }
            public Data data { get; set; }
        }

        public class Series
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public List<Item> items { get; set; }
            public int returned { get; set; }
        }

        public class Stories
        {
            public int available { get; set; }
            public string collectionURI { get; set; }
            public List<Item> items { get; set; }
            public int returned { get; set; }
        }

        public class Thumbnail
        {
            public string path { get; set; }
            public string extension { get; set; }
        }

        public class Url
        {
            public string type { get; set; }
            public string url { get; set; }
        }
    }
}
