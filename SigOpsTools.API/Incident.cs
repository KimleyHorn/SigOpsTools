using System.Drawing;

namespace SigOpsTools.API
{
    public class Incident
    {

        private int _id;
        private string _roadwayName;
        private DateTime _dateReported;
        private DateTime _dateUpdated;
        private string _description;
        private IncidentLocation _location;
        private string _directionOfTravel;
        private string _eventType;
        private string _eventSubtype;
        private string _detours;
        private string _lanesAffected;

        public Incident(int id, string roadwayName, DateTime dateReported, DateTime dateUpdated, string description, IncidentLocation location, string directionOfTravel, string eventType, string eventSubtype, string detours, string lanesAffected)
        {
            _id = id;
            _roadwayName = roadwayName;
            _dateReported = dateReported;
            _dateUpdated = dateUpdated;
            _description = description;
            _location = location;
            _directionOfTravel = directionOfTravel;
            _eventType = eventType;
            _eventSubtype = eventSubtype;
            _detours = detours;
            _lanesAffected = lanesAffected;
        }

        public Incident(int id, string roadwayName, DateTime dateReported, DateTime dateUpdated, string description, double lat, double lon, string directionOfTravel, string eventType, string eventSubtype, string detours, string lanesAffected)
        {
            _id = id;
            _roadwayName = roadwayName;
            _dateReported = dateReported;
            _dateUpdated = dateUpdated;
            _description = description;
            _location = new IncidentLocation();
            _directionOfTravel = directionOfTravel;
            _eventType = eventType;
            _eventSubtype = eventSubtype;
            _detours = detours;
            _lanesAffected = lanesAffected;
        }

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string RoadwayName
        {
            get => _roadwayName;
            set => _roadwayName = value;
        }

        public DateTime DateReported
        {
            get => _dateReported;
            set => _dateReported = value;
        }

        public DateTime DateUpdated
        {
            get => _dateUpdated;
            set => _dateUpdated = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public IncidentLocation Location
        {
            get => _location;
            set => _location = value;
        }

        public string DirectionOfTravel
        {
            get => _directionOfTravel;
            set => _directionOfTravel = value;
        }

        public string EventType
        {
            get => _eventType;
            set => _eventType = value;
        }

        public string EventSubtype
        {
            get => _eventSubtype;
            set => _eventSubtype = value;
        }

        public string Detours
        {
            get => _detours;
            set => _detours = value;
        }

        public string LanesAffected
        {
            get => _lanesAffected;
            set => _lanesAffected = value;
        }

        public override string ToString()
        {
            return
                $"Id: {_id}, RoadwayName: {_roadwayName}, DateReported: {_dateReported}, DateUpdated: {_dateUpdated}, Description: {_description}, Location: {_location}, DirectionOfTravel: {_directionOfTravel}, EventType: {_eventType}, EventSubtype: {_eventSubtype}, Detours: {_detours}, LanesAffected: {_lanesAffected}";
        }
    }

    public class IncidentLocation
    {
        private double _lat;
        private double _lon;

        //private string _roadway; TODO: Add roadway name and figure out a way to djikstra's algorithm to find all roads affected in the roadway direction
        //private string _direction; TODO: Add direction of travel
        public double Lat
        {
            get => _lat;
            set => _lat = value;
        }
        public double Lon
        {
            get => _lon;
            set => _lon = value;
        }
    }
}
