namespace IncidentEmailService.Models
{
    /// <summary>
    /// The incident class that will drive all of the 
    /// </summary>
    public class Incident
    {
        public Incident()
        {
            ID = string.Empty;
            roadway_name = string.Empty;
            reported = DateTime.MinValue;
            last_updated = DateTime.MinValue;
            planned_end = DateTime.MinValue;
            description = string.Empty;
            latitude = 0.0;
            longitude = 0.0;
            direction_of_travel = string.Empty;
            event_type = string.Empty;
            subtype = string.Empty;
            detours = string.Empty;
            lanes_affected = string.Empty;
            region = string.Empty;

        }


        public Incident(string id = "", string roadwayName = "", DateTime dateReported = default, DateTime plannedEndDate = default, DateTime lastUpdated = default, string description = "", double lat = 0.0, double lon = 0.0, string directionOfTravel = "", string eventType = "", string subtype = "", string detours = "", string lanesAffected = "", string region = "")
        {
            ID = id;
            roadway_name = roadwayName;
            reported = dateReported;
            planned_end = dateReported;
            last_updated = lastUpdated;
            this.description = description;
            latitude = lat;
            longitude = lon;
            direction_of_travel = directionOfTravel;
            event_type = eventType;
            this.subtype = subtype;
            this.detours = detours;
            lanes_affected = lanesAffected;
            this.region = region;
        }

        public string ID { get; set; }

        public string roadway_name { get; set; }

        public DateTime reported { get; set; }

        public DateTime last_updated { get; set; }

        public DateTime planned_end { get; set; }

        public string description { get; set; }

        public double latitude { get; set; }

        public double longitude { get; set; }

        public string direction_of_travel { get; set; }

        public string event_type { get; set; }

        public string subtype { get; set; }

        public string detours { get; set; }

        public string lanes_affected { get; set; }

        public string region { get; set; }

        public override string ToString()
        {
            return
                $"Id: {ID}, RoadwayName: {roadway_name}, DateReported: {reported}, DateUpdated: {last_updated}, Description: {description}, Location: {latitude} {longitude}, DirectionOfTravel: {direction_of_travel}, EventType: {event_type}, EventSubtype: {subtype}, Detours: {detours}, LanesAffected: {lanes_affected}, Region: {region}";
        }
    }
}
