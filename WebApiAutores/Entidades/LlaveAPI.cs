using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entidades
{
    public class LlaveAPI
    {
        public int Id { get; set; }
        public string Llave { get; set; }
        public TipoLlave TipoLlave { get; set; }
        public bool Activa { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public List<RestriccionIP> RestriccionesIPs { get; set; }
        public List<RestriccionesDominio> RestriccionesDominios { get; set; }
    }

}
public enum TipoLlave
{
    Gratuita = 1,
    Profesional = 2
}
