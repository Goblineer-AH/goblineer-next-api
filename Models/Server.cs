namespace GoblineerNextApi.Models
{
    public class Server
    {
        public int ConnectedRealmId { get; set; }
        public string Region { get; set; } = "";
        public string RealmName { get; set; } = "";
    }
}