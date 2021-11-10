namespace EllieBell.Services
{
	public class GloeggliService
	{
		public List<string> Messages { get; set; } = new();

		public void RingGloeggli()
		{
			Messages.Add($"{DateTime.Now:o}: Ellie has an epileptic event!");
		}
	}
}