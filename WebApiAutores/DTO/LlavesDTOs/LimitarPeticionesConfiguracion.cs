namespace WebApiAutores.DTO.LlavesDTOs
{
    public class LimitarPeticionesConfiguracion
    {
        public int PeticionesPorDiaGratuito { get; set; }
        public string[] ListaBlancaRutas { get; set; }
    }
}
