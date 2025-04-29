namespace APIDevSteam.Models
{
    public class JogoMidia
    {
        public Guid JogoMidiaId { get; set; }
        public Guid GameId { get; set; }
        public Game? Game { get; set; }
        public string Tipo { get; set; }
        public string Url { get; set; }

    }
}
