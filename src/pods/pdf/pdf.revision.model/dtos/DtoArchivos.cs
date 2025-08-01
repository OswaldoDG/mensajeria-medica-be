namespace pdf.revision.model.dtos;

public class DtoArchivos
{
    public int Id { get; set; }

    required public string Nombre { get; set; }

    public List<DtoParte> Partes { get; set; }
}
