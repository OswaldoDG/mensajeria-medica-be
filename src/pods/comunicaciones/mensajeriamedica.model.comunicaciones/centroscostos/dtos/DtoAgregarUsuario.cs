using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mensajeriamedica.model.comunicaciones.centroscostos.dtos
{
    public class DtoAgregarUsuario
    {
        public int CentroCostosId { get; set; }
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
    }
}
