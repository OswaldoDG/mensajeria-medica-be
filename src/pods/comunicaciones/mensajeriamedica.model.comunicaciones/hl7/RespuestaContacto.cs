using System.Diagnostics.Contracts;

namespace mensajeriamedica.model.comunicaciones.hl7
{
    public class RespuestaContacto
    {
        public ContactoEstudio Contacto { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
