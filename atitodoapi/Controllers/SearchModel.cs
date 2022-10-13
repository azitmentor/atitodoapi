namespace Atitodoapi.Controllers
{
    public class SearchModel
    {
        public string? text { get; set; }
        public List<string> tags { get; set; }
        public bool showdone { get; set; }
        public bool showdeleted { get; set; }
        public bool showarchived { get; set; }
        public bool starredonly { get; set; }
        public bool deletedonly { get; set; }
        public bool archivedonly { get; set; }
        public bool todayonly { get; set; }
        public bool wastoday { get; set; }
        public int hidebelow { get; set; }
    }
}