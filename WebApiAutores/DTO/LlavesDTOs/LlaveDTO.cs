using WebApiAutores.DTO.RestriccionesDominio;
using WebApiAutores.DTO.RestriccionesIP;

namespace WebApiAutores.DTO.LlavesDTOs
{
    public class LlaveDTO
    {
        public int Id { get; set; }
        public string Llave { get; set; }
        public bool Activa { get; set; }
        public string TipoLlave { get; set; }
        public List<RestriccionDominioDTO> RestriccionesDominios { get; set; }
        public List<RestriccionesIPDTO> RestriccionesIPs { get; set; }
    }
}
