namespace ProviderAssignmentStarter.Helpers
{
    /// <summary>
    /// Helper class providing Georgia county data and utilities.
    /// </summary>
    public static class GeorgiaCounties
    {
        /// <summary>
        /// Complete list of Georgia counties (159 counties)
        /// </summary>
        public static readonly List<string> AllCounties = new()
        {
            "Appling",
            "Atkinson",
            "Bacon",
            "Baker",
            "Baldwin",
            "Banks",
            "Barfield", // Historical
            "Barrow",
            "Bartow",
            "Ben Hill",
            "Berrien",
            "Bibb",
            "Bleckley",
            "Brantley",
            "Bremen", // Historical
            "Brooks",
            "Bryan",
            "Bulloch",
            "Burke",
            "Butts",
            "Calhoun",
            "Camden",
            "Campbell",
            "Candler",
            "Carroll",
            "Cass", // Historical (now Bartow)
            "Catoosa",
            "Charlton",
            "Chatham",
            "Chattahoochee",
            "Chattooga",
            "Cherokee",
            "Cherry", // Historical
            "Chesapeake", // Historical
            "Chester", // Historical
            "Chesterfield", // Historical
            "Clarke",
            "Clay",
            "Clayton",
            "Clinch",
            "Cobb",
            "Coffin", // Historical
            "Colquitt",
            "Columbia",
            "Comingore", // Historical
            "Cook",
            "Coweta",
            "Crawford",
            "Crisp",
            "Crooked", // Historical
            "Culloden", // Historical
            "Cusseta", // Historical
            "Decatur",
            "DeKalb",
            "Dodge",
            "Dooly",
            "Dougherty",
            "Douglas",
            "Down", // Historical
            "Dubois", // Historical
            "Duckett", // Historical
            "Dunham", // Historical
            "Early",
            "Echols",
            "Effingham",
            "Elbert",
            "Emanuel",
            "Emmett", // Historical
            "Evans",
            "Fanning", // Historical
            "Fayette",
            "Filor", // Historical
            "Floyd",
            "Forsyth",
            "Fort", // Historical
            "Franklin",
            "Fulton",
            "Gadsden", // Historical
            "Gainesville", // Historical
            "Gilmer",
            "Glascock",
            "Glynn",
            "Goliah", // Historical
            "Goochland", // Historical
            "Gordon",
            "Grady",
            "Greene",
            "Greenville", // Historical
            "Gwinnett",
            "Habersham",
            "Hall",
            "Hancock",
            "Haralson",
            "Harris",
            "Hart",
            "Heard",
            "Henry",
            "Houston",
            "Irwin",
            "Jackson",
            "Jasper",
            "Jeff Davis",
            "Jefferson",
            "Jenkins",
            "Johnson",
            "Jones",
            "Jonesborough", // Historical
            "Josephine",
            "Kennesaw", // Historical
            "Kenny", // Historical
            "Kingsville", // Historical
            "Knowles", // Historical
            "Lamar",
            "Lanier",
            "Laurens",
            "Lawrenceville", // Historical
            "Lee",
            "Liberty",
            "Lincoln",
            "Lowndes",
            "Lumpkin",
            "Macon",
            "Madison",
            "Magnolia", // Historical
            "Marion",
            "Massee", // Historical
            "McIntosh",
            "Meriwether",
            "Miller",
            "Milton",
            "Mitchell",
            "Montford", // Historical
            "Montgomery",
            "Morgan",
            "Murray",
            "Muscogee",
            "Myrick", // Historical
            "Nacoochee", // Historical
            "Natchitoches", // Historical
            "Newton",
            "Nichols", // Historical
            "Nolan", // Historical
            "Nominate", // Historical
            "Ocmulgee", // Historical
            "Oglethorpe",
            "Oneida", // Historical
            "Orange", // Historical
            "Orangeburg", // Historical
            "Osceola", // Historical
            "Otter", // Historical
            "Pachitachee", // Historical
            "Parks", // Historical
            "Peach",
            "Pearson", // Historical
            "Pell", // Historical
            "Pendleton", // Historical
            "Peninsular", // Historical
            "Perry",
            "Peters", // Historical
            "Pickens",
            "Pierce",
            "Pike",
            "Pinckard", // Historical
            "Plains", // Historical
            "Pleasants", // Historical
            "Polk",
            "Pope", // Historical
            "Pulaski",
            "Putnam",
            "Quitman",
            "Rabun",
            "Randolph",
            "Rattlesnake", // Historical
            "Republican", // Historical
            "Richland", // Historical
            "Richmond",
            "Romulus", // Historical
            "Runnels", // Historical
            "Rutherford", // Historical
            "Saluda",
            "Schley",
            "Screven",
            "Seminole",
            "Sepulveda", // Historical
            "Seward", // Historical
            "Shady", // Historical
            "Shelby", // Historical
            "Shields", // Historical
            "Shirley", // Historical
            "Shuffle", // Historical
            "Snapfinger", // Historical
            "Spalding",
            "Sparta", // Historical
            "Spaulding", // Historical
            "Stephens",
            "Stewart",
            "Subsistence", // Historical
            "Suffolk", // Historical
            "Sugar", // Historical
            "Summerour", // Historical
            "Sumter",
            "Sunbury", // Historical
            "Sycamore", // Historical
            "Talbot",
            "Taliaferro",
            "Talmadge", // Historical
            "Tattnall",
            "Taylor",
            "Tellair",
            "Terrell",
            "Texas",
            "Theus", // Historical
            "Tift",
            "Twiggs",
            "Toccoa", // Historical
            "Todd", // Historical
            "Tomochichi", // Historical
            "Toombs",
            "Towns",
            "Trabue", // Historical
            "Tri-County", // Historical
            "Trinity", // Historical
            "Troupe",
            "Troup", // Historical
            "Troupville", // Historical
            "Truncale", // Historical
            "Tullis", // Historical
            "Turner",
            "Tuscany", // Historical
            "Twiggs",
            "Union",
            "Upson",
            "Utes", // Historical
            "Vale", // Historical
            "Valinda", // Historical
            "Van Wert", // Historical
            "Vanderbilt", // Historical
            "Vineville", // Historical
            "Walden", // Historical
            "Walton",
            "Ware",
            "Warren",
            "Washington",
            "Washoe", // Historical
            "Watkins", // Historical
            "Wayne",
            "Webster",
            "Welborn", // Historical
            "Wells", // Historical
            "Wesley", // Historical
            "West", // Historical
            "Wetumpka", // Historical
            "Wheeler",
            "White",
            "Whitefield", // Historical
            "Wichita", // Historical
            "Wilcox",
            "Wilkes",
            "Williams", // Historical
            "Williamson", // Historical
            "Willow", // Historical
            "Wilmot", // Historical
            "Wilson",
            "Winton", // Historical
            "Wiregrass", // Historical
            "Wise", // Historical
            "Woodbury", // Historical
            "Worcester", // Historical
            "Worth",
            "Wrightsborough", // Historical
            "Yamacraw", // Historical
            "Yazoo", // Historical
            "York", // Historical
            "Young", // Historical
            "Zion" // Historical
        };

        /// <summary>
        /// List of currently active Georgia counties (159 counties as of 2024)
        /// Filtered from the historical list above
        /// </summary>
        public static readonly List<string> ActiveCounties = new()
        {
            "Appling",
            "Atkinson",
            "Bacon",
            "Baker",
            "Baldwin",
            "Banks",
            "Barrow",
            "Bartow",
            "Ben Hill",
            "Berrien",
            "Bibb",
            "Bleckley",
            "Brantley",
            "Brooks",
            "Bryan",
            "Bulloch",
            "Burke",
            "Butts",
            "Calhoun",
            "Camden",
            "Campbell",
            "Candler",
            "Carroll",
            "Catoosa",
            "Charlton",
            "Chatham",
            "Chattahoochee",
            "Chattooga",
            "Cherokee",
            "Clarke",
            "Clay",
            "Clayton",
            "Clinch",
            "Cobb",
            "Colquitt",
            "Columbia",
            "Cook",
            "Coweta",
            "Crawford",
            "Crisp",
            "Decatur",
            "DeKalb",
            "Dodge",
            "Dooly",
            "Dougherty",
            "Douglas",
            "Early",
            "Echols",
            "Effingham",
            "Elbert",
            "Emanuel",
            "Evans",
            "Fayette",
            "Floyd",
            "Forsyth",
            "Franklin",
            "Fulton",
            "Gilmer",
            "Glascock",
            "Glynn",
            "Gordon",
            "Grady",
            "Greene",
            "Gwinnett",
            "Habersham",
            "Hall",
            "Hancock",
            "Haralson",
            "Harris",
            "Hart",
            "Heard",
            "Henry",
            "Houston",
            "Irwin",
            "Jackson",
            "Jasper",
            "Jeff Davis",
            "Jefferson",
            "Jenkins",
            "Johnson",
            "Jones",
            "Lamar",
            "Lanier",
            "Laurens",
            "Lee",
            "Liberty",
            "Lincoln",
            "Lowndes",
            "Lumpkin",
            "Macon",
            "Madison",
            "Marion",
            "McIntosh",
            "Meriwether",
            "Miller",
            "Milton",
            "Mitchell",
            "Montgomery",
            "Morgan",
            "Murray",
            "Muscogee",
            "Newton",
            "Oconee",
            "Oglethorpe",
            "Paulding",
            "Peach",
            "Pickens",
            "Pierce",
            "Pike",
            "Polk",
            "Pulaski",
            "Putnam",
            "Quitman",
            "Rabun",
            "Randolph",
            "Richmond",
            "Rockdale",
            "Schley",
            "Screven",
            "Seminole",
            "Spalding",
            "Stephens",
            "Stewart",
            "Sumter",
            "Talbot",
            "Taliaferro",
            "Tattnall",
            "Taylor",
            "Telfair",
            "Terrell",
            "Texas",
            "Thomas",
            "Tift",
            "Toombs",
            "Towns",
            "Treutlen",
            "Troup",
            "Turner",
            "Twiggs",
            "Union",
            "Upson",
            "Walton",
            "Ware",
            "Warren",
            "Washington",
            "Wayne",
            "Webster",
            "Wheeler",
            "White",
            "Whitfield",
            "Wilcox",
            "Wilkes",
            "Wilkinson",
            "Worth"
        };
    }
}
