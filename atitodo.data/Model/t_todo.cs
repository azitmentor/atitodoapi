namespace Atitodo.Data.Model
{
	public class t_todo
	{
		public int id { get; set; }
		public string todotext { get; set; }
		public int? impact { get; set; }
		public int? priority { get; set; }
		public int? length { get; set; }
		public DateTime created { get; set; }
		public DateTime modified { get; set; }
		public DateTime? archived { get; set; }
		public DateTime? deleted { get; set; }
		public DateTime? fortoday { get; set; }
		public DateTime? deadline { get; set; }
		public DateTime? done { get; set; }
		public string tags { get; set; }
		public int userid { get; set; }
		public int? parentid { get; set; }
		public string comment { get; set; }
		public string commentblock { get; set; }
		public bool starred { get; set; }
	}
}
