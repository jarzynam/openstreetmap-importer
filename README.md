# Open Street Map MongoDB POI Importer
## Description
Main purpose of the program is to read [Open Street Map .pbf files](https://download.geofabrik.de/index.html), recognize points of intrests (schools, shops, parkings, etc.) and place them as places (with a single coordinate, not polygon) in Mongo Database.
 
Assumptions:
- *Program is not fully functional, the main purpose is creating a code sample*
- Program is importing [Nodes](https://wiki.openstreetmap.org/wiki/Node) and [Ways](https://wiki.openstreetmap.org/wiki/Way), other OSM types are ignored
- If Place in OpenStreetMap is represented by area/polygon, it will be transformed to the single geograpchic point (calculated central point of polygon)

  ![Calculation Image](/images/open-street-map-importer.png?raw=true)
- Program is importing only types specified in `PlaceGroups.cs` file, you can extend this list by modyfing the file
- .pbf files are large, single import (like Poland) can take a couple of minutes, although program is optimized to not eat all your memory (despite docker-compose memory consumption)

## How to run
### Prerequisities
- Docker for Desktop
- Visual Studio 2022

### Input
- place .pbf files in OpenStreetMapFiles directory

  ![Input file image](/images/open-street-map-input-files.jpg?raw=true)

### Output 
All POIs (places) are stored in MongoDB, `Database: Places, Collection: osmV{version}` . You can check them in MongoDB-Express (defult url: `http://localhost:8081`)
![DB Output Image](/images/mongo-db-output-results.jpg)

### Additional Configuration

#### Different Open Street Map types
Only types specified in `PlaceGroups` structure are imported into Mongo. Please modify this file, whenever you want to add (or remove) diferent types of places:
```csharp
 public struct PlaceGroups
 {
        public static readonly PlaceGroup[] Items = {

            //Trade
            new("shop", "mall",PlaceType.ShoppingMall),
            new("shop", "convenience", PlaceType.Shop),
            new("shop", "supermarket", PlaceType.Supermarket),
            new("shop", "wine", PlaceType.WineShop),
           
            /* .... */
}
```

## Development
Local solution is setup with `docker-compose` to fetch all necessary docker images from Docker registry including:
- Mongo database
- Mongo database explorer (mongo-express)

### Local debugging
1. Open Visual Studio
1. Set `docker-compose` as default project
1. Press F5 to start local debugging session
1. You would debug API and Worker in the same time

#### How to test
Solution contains unit `OpenStreetMap.Tests.Unit` with *example* tests (for future extension)

Run them with Standard Visual Studio test runner

# MongoDB Explorer
With [Mongo Express](https://github.com/mongo-express/mongo-express) You can explore the whole local mongo dabase by reaching `http://localhost:8081` in your browser.