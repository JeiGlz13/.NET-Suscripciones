namespace WebApiAutores.Entidades
{
    public class RestriccionesDominio
    {
        public int Id { get; set; }
        public int LlaveId { get; set; }
        public string Dominio { get; set; }
        public LlaveAPI Llave { get; set; }
    }
}
