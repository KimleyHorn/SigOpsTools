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
            ID = string.Empty;
            RoadwayName = string.Empty;
            DateReported = DateTime.MinValue;
            LastUpdated = DateTime.MinValue;
            Description = string.Empty;
            Latitude = 0.0;
            Longitude = 0.0;
            DirectionOfTravel = string.Empty;
            EventType = string.Empty;
            Subtype = string.Empty;
            Detours = string.Empty;
            LanesAffected = string.Empty;
            Region = string.Empty;

        }


        public Incident(string id = "", string roadwayName = "", DateTime dateReported = default, DateTime lastUpdated = default, string description = "", double lat = 0.0, double lon = 0.0, string directionOfTravel = "", string eventType = "", string subtype = "", string detours = "", string lanesAffected = "", string region = "")
        {
            ID = id;
            RoadwayName = roadwayName;
            DateReported = dateReported;
            LastUpdated = lastUpdated;
            Description = description;
            Latitude = lat;
            Longitude = lon;
            DirectionOfTravel = directionOfTravel;
            EventType = eventType;
            Subtype = subtype;
            Detours = detours;
            LanesAffected = lanesAffected;
            Region = region;
        }

        public string ID { get; set; }

        public string RoadwayName { get; set; }

        public DateTime DateReported { get; set; }

        public DateTime LastUpdated { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string DirectionOfTravel { get; set; }

        public string EventType { get; set; }

        public string Subtype { get; set; }

        public string Detours { get; set; }

        public string LanesAffected { get; set; }

        public string Region { get; set; }

        public override string ToString()
        {
            return
                $"Id: {ID}, RoadwayName: {RoadwayName}, DateReported: {DateReported}, DateUpdated: {LastUpdated}, Description: {Description}, Location: {Latitude} {Longitude}, DirectionOfTravel: {DirectionOfTravel}, EventType: {EventType}, EventSubtype: {Subtype}, Detours: {Detours}, LanesAffected: {LanesAffected}, Region: {Region}";
        }
    }
}
