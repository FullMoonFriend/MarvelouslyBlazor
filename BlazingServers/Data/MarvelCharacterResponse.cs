
    namespace BlazingServers.Data
    {
        public class MarvelCharacterResponse
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
                public string name { get; set; }
                public string description { get; set; }
                public Thumbnail thumbnail { get; set; }
                public string resourceURI { get; set; }
            }

            public class Thumbnail
            {
                public string path { get; set; }
                public string extension { get; set; }
            }
        }
    }
