using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTO.RestriccionesDominio
{
    public class ActualizarRestriccionDominioDTO
    {
        [Required]
        public string Dominio { get; set; }
    }
}
