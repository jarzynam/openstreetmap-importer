using OpenStreetMap.Common;

namespace OpenStreetMap.Importer.Importer.Helpers
{
    public struct PlaceGroup
    {
        public PlaceGroup(string openStreetMapTagKey, string openStreetMapTagValue, PlaceType placeType)
        {
            OpenStreetMapTagKey = openStreetMapTagKey;
            OpenStreetMapTagValue = openStreetMapTagValue;
            PlaceType = placeType;
        }
        public string OpenStreetMapTagKey { get; set; }
        public string OpenStreetMapTagValue { get; set; }
        public PlaceType PlaceType { get; set; }
    }

    public struct PlaceGroups
    {
        public static readonly PlaceGroup[] Items = {

            //Trade
            new("shop", "mall",PlaceType.ShoppingMall),
            new("shop", "convenience", PlaceType.Shop),
            new("shop", "supermarket", PlaceType.Supermarket),
            new("shop", "wine", PlaceType.WineShop),
            new("shop", "grocery", PlaceType.GroceryStore),
            new("amenity", "atm", PlaceType.Atm),

            //Communication
            new("aeroway", "aerodrome", PlaceType.Airport),
            new("amenity", "bicycle_rental", PlaceType.BikeRental),
            new("highway", "bus_stop", PlaceType.BusStation),
            new("amenity", "car_rental", PlaceType.CarRent),
            new("amenity", "parking", PlaceType.Parking),
            new("landuse", "port", PlaceType.Port),
            new("building", "train_station", PlaceType.TrainStation),
            new("station", "subway", PlaceType.SubwayStation),
            new("amenity", "taxi", PlaceType.TaxiStand),
            new("railway", "tram_stop", PlaceType.TramStation),

            //Entertainment
            new("tourism", "museum", PlaceType.Museum),
            new("tourism", "zoo", PlaceType.Zoo),
            new("amenity", "casino", PlaceType.Casino),
            new("amenity", "theatre", PlaceType.Theater),
            new("leisure", "stadium", PlaceType.Stadium),
            new("leisure", "bowling_alley", PlaceType.BowlingAlley),
            new("amenity", "cinema", PlaceType.MovieTheater),
            new("amenity", "arts_centre", PlaceType.ArtGallery),
            new("amenity", "music_venue", PlaceType.MusicVenue),
            new("leisure", "playground", PlaceType.Playground),

            //Gastronomy
            new("amenity", "bar", PlaceType.Bar),
            new("amenity", "pub", PlaceType.Bar),
            new("amenity", "cafe", PlaceType.Cafe),
            new("amenity", "restaurant", PlaceType.Restaurant),
            new("amenity", "fast_food", PlaceType.Restaurant),
            new("amenity", "food_court", PlaceType.Restaurant),
            new("amenity", "nightclub", PlaceType.NightClub),
            

            //Health
            new("amenity", "doctors", PlaceType.Doctor),
            new("healthcare", "doctor", PlaceType.Doctor),
            new("amenity", "dentist", PlaceType.Doctor),
            new("healthcare", "dentist", PlaceType.Doctor),
            new("amenity", "hospital", PlaceType.Hospital),
            new("healthcare", "hospital", PlaceType.Hospital),
            new("amenity", "pharmacy", PlaceType.Pharmacy),
            new("healthcare", "pharmacy", PlaceType.Pharmacy),
            new("amenity", "spa", PlaceType.Spa),
            new("amenity", "clinic", PlaceType.MedicalCenter),
            new("healthcare  ", "clinic", PlaceType.MedicalCenter),
            new("amenity", "gym", PlaceType.Gym),
            new("leisure", "fitness_station", PlaceType.Gym),
         

            //Recreation
            new("highway", "cycleway", PlaceType.BikeTrail),
            new("landuse", "recreation_ground", PlaceType.GeneralRecreation),
            new("leisure", "fitness_centre", PlaceType.Gym),
            new("leisure", "swimming_pool", PlaceType.Pool),
            new("leisure", "sport_pitch", PlaceType.SportField),
            new("leisure", "pitch", PlaceType.SportField),
            new("leisure", "sports_centre", PlaceType.SportField),
            new("leisure", "park", PlaceType.Park),
            new("leisure", "dog_park", PlaceType.Park),

            //Education
            new("amenity", "school", PlaceType.GeneralSchool),
            new("amenity", "college", PlaceType.College),
            new("amenity", "kindergarten", PlaceType.PreSchool),
            new("amenity", "university", PlaceType.University),

            //Administration
            new("office", "government", PlaceType.GovernmentBuilding),
            new("amenity", "post_office", PlaceType.PostOffice),
            new("amenity", "police", PlaceType.Police),
            new("amenity", "courthouse", PlaceType.Courthouse),
            new("amenity", "townhall", PlaceType.CityHall),
            new("amenity", "fire_station", PlaceType.FireStation),
            new("amenity", "embassy", PlaceType.Embassy) };
    }
}
