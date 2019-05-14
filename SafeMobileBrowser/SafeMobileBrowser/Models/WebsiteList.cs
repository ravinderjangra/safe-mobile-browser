using System.Collections.Generic;

namespace SafeMobileBrowser.Models
{
    //TODO: Get list from some common source
    public class WebsiteList
    {
        //[JsonProperty("alpha2_websites")]
        //public List<string> Alpha2Websites { get; set; }

        //[JsonProperty("mock_websites")]
        //public List<object> MockWebsites { get; set; }

        //public WebsiteList()
        //{
        //    Alpha2Websites = new List<string>();
        //}

        public static List<string> GetWebsiteList()
        {
            return new List<string>
            {
                "test.mobiletest",
                "cat.ashi",
                "home.chickenbacon",
                "home.dgeddes",
                "doggies.vmp",
                "dweb",
                "eye.eye",
                "heaven",
                "hello",
                "udhr",
                "wall.knot",
                "fear.knot",
                "maid.safedemo",
                "maidsafe.safenet",
                "the.odyssey",
                "safenetworkprimer",
                "safe-blues.jpl",
                "typer.game",
                "complements.valmarkpro",
                "safedapps.jpl1"
            };
        }
    }
}
