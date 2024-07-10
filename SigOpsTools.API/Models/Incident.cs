using System.Drawing;

namespace SigOpsTools.API.Models
{
    /// <summary>
    /// The incident class that will drive all of the 
    /// </summary>
    public class Incident
    {
        public Incident()
        {
            Id = string.Empty;
            RoadwayName = string.Empty;
            DateReported = DateTime.MinValue;
            DateUpdated = DateTime.MinValue;
            Description = string.Empty;
            Location = new IncidentLocation
            {
                Lat = 0.0,
                Lon = 0.0
            };
            DirectionOfTravel = string.Empty;
            EventType = string.Empty;
            EventSubtype = string.Empty;
            Detours = string.Empty;
            LanesAffected = string.Empty;
            Region = string.Empty;

        }

        public Incident(string id, string roadwayName, DateTime dateReported, DateTime dateUpdated, string description, IncidentLocation location, string directionOfTravel, string eventType, string eventSubtype, string detours, string lanesAffected, string region)
        {
            Id = id;
            RoadwayName = roadwayName;
            DateReported = dateReported;
            DateUpdated = dateUpdated;
            Description = description;
            Location = new IncidentLocation
            {
                Lat = location.Lat,
                Lon = location.Lon
            };
            DirectionOfTravel = directionOfTravel;
            EventType = eventType;
            EventSubtype = eventSubtype;
            Detours = detours;
            LanesAffected = lanesAffected;
            Region = region;
        }

        public Incident(string id = "", string roadwayName = "", DateTime dateReported = default, DateTime dateUpdated = default, string description = "", double lat = 0.0, double lon = 0.0, string directionOfTravel = "", string eventType = "", string eventSubtype = "", string detours = "", string lanesAffected = "", string region = "")
        {
            Id = id;
            RoadwayName = roadwayName;
            DateReported = dateReported;
            DateUpdated = dateUpdated;
            Description = description;
            Location = new IncidentLocation
            {
                Lat = lat,
                Lon = lon
            };
            DirectionOfTravel = directionOfTravel;
            EventType = eventType;
            EventSubtype = eventSubtype;
            Detours = detours;
            LanesAffected = lanesAffected;
            Region = region;
        }

        public string Id { get; set; }

        public string RoadwayName { get; set; }

        public DateTime DateReported { get; set; }

        public DateTime DateUpdated { get; set; }

        public string Description { get; set; }

        public IncidentLocation Location { get; set; }

        public string DirectionOfTravel { get; set; }

        public string EventType { get; set; }

        public string EventSubtype { get; set; }

        public string Detours { get; set; }

        public string LanesAffected { get; set; }

        public string Region { get; set; }

        public override string ToString()
        {
            return
                $"Id: {Id}, RoadwayName: {RoadwayName}, DateReported: {DateReported}, DateUpdated: {DateUpdated}, Description: {Description}, Location: {Location}, DirectionOfTravel: {DirectionOfTravel}, EventType: {EventType}, EventSubtype: {EventSubtype}, Detours: {Detours}, LanesAffected: {LanesAffected}, Region: {Region}";
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class IncidentLocation
    {
        //private string _roadway; TODO: Add roadway name and figure out a way to djikstra's algorithm to find all roads affected in the roadway direction
        public double Lat { get; set; }

        public double Lon { get; set; }
    }



}
