using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTO.RestriccionesIP
{
    public class ActualizarRestriccionIPDTO
    {
        [Required]
        public string IP { get; set; }
    }
}
