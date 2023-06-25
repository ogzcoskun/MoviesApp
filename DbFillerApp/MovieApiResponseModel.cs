using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbFillerApp
{
    public class MovieApiResponseModel
    {
        public int? entries { get; set; }
        public List<Result> results { get; set; }
    }

    public class Caption
    {
        public string? plainText { get; set; }
        public string __typename { get; set; }
    }

    public class OriginalTitleText
    {
        public string text { get; set; }
        public string __typename { get; set; }
    }

    public class PrimaryImage
    {
        public string id { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public string? url { get; set; }
        public Caption caption { get; set; }
        public string __typename { get; set; }
    }

    public class ReleaseDate
    {
        public int? day { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }
        public string __typename { get; set; }
    }

    public class ReleaseYear
    {
        public int? year { get; set; }
        public object endYear { get; set; }
        public string __typename { get; set; }
    }

    public class Result
    {
        public string _id { get; set; }
        public string id { get; set; }
        public PrimaryImage primaryImage { get; set; }
        public TitleType titleType { get; set; }
        public TitleText titleText { get; set; }
        public OriginalTitleText originalTitleText { get; set; }
        public ReleaseYear releaseYear { get; set; }
        public ReleaseDate releaseDate { get; set; }
        public int? position { get; set; }
    }

    public class TitleText
    {
        public string text { get; set; }
        public string __typename { get; set; }
    }

    public class TitleType
    {
        public string text { get; set; }
        public string id { get; set; }
        public bool isSeries { get; set; }
        public bool isEpisode { get; set; }
        public string __typename { get; set; }
    }

}
