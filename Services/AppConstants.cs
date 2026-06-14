namespace ProviderAssignmentStarter.Services
{
    /// <summary>
    /// Helper service for application constants and lookups.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// List of all 159 Georgia counties, sorted alphabetically.
        /// </summary>
        public static readonly List<string> GeorgiaCounties = new()
        {
            "Appling", "Atkinson", "Bacon", "Baker", "Baldwin", "Banks", "Barrow", "Bartow",
            "Ben Hill", "Berrien", "Bibb", "Bleckley", "Brantley", "Brooks", "Bryan", "Bulloch",
            "Burke", "Butts", "Calhoun", "Camden", "Campbell", "Candler", "Carroll", "Catoosa",
            "Charlton", "Chatham", "Chattahoochee", "Chattooga", "Cherokee", "Cherry", "Clarke",
            "Clay", "Clayton", "Clinch", "Cobb", "Coffee", "Colquitt", "Columbia", "Cook",
            "Coweta", "Crawford", "Crisp", "Dade", "Dawson", "Decatur", "DeKalb", "Dodge",
            "Dooly", "Dougherty", "Douglas", "Early", "Echols", "Effingham", "Elbert", "Emanuel",
            "Evans", "Fannin", "Fayette", "Floyd", "Forsyth", "Franklin", "Fulton", "Gilmer",
            "Glascock", "Glynn", "Gordon", "Grady", "Graham", "Grant", "Greene", "Grundy",
            "Gwinnett", "Habersham", "Hall", "Hancock", "Haralson", "Harris", "Hart", "Heard",
            "Henry", "Houston", "Irwin", "Jackson", "Jasper", "Jeff Davis", "Jefferson", "Jenkins",
            "Johnson", "Jones", "Lamar", "Lanier", "Laurens", "Lee", "Liberty", "Lincoln",
            "Long", "Lowndes", "Lumpkin", "Macon", "Madison", "Marion", "McDuffie", "McIntosh",
            "Meriwether", "Miller", "Milton", "Mitchell", "Monroe", "Montgomery", "Morgan", "Murray",
            "Muscogee", "Newton", "Oconee", "Oglethorpe", "Paulding", "Peach", "Pickens", "Pierce",
            "Pike", "Polk", "Pulaski", "Putnam", "Quitman", "Rabun", "Randolph", "Richmond",
            "Rockdale", "Schley", "Screven", "Seminole", "Spalding", "Spalding", "Stephens", "Stewart",
            "Sumter", "Talbot", "Taliaferro", "Tattnall", "Taylor", "Telfair", "Terrell", "Thomas",
            "Tift", "Toombs", "Towns", "Treutlen", "Troup", "Turner", "Twiggs", "Union",
            "Upson", "Walker", "Walton", "Ware", "Warren", "Washington", "Wayne", "Webster",
            "Wheeler", "White", "Whitfield", "Wilcox", "Wilkes", "Wilkinson", "Worth", "Wynn"
        };

        /// <summary>
        /// Get sorted Georgia counties.
        /// </summary>
        public static List<string> GetSortedGeorgiaCounties()
        {
            return GeorgiaCounties.OrderBy(c => c).Distinct().ToList();
        }
    }
}
