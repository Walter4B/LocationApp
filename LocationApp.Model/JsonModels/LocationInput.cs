using LocationApp.Model.Helper;

namespace LocationApp.Model.JsonModels
{
    public class LocationInput : DataTableOptions
    {
        public List<int> Categories { get; set; } = new();
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
    }
}
