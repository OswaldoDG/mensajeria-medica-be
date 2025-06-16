namespace mensajeriamedica.model.comunicaciones.hl7;

public class CampoConfig
{
    public int PipeIndex { get; set; }
    public int[] Componentes { get; set; } = Array.Empty<int>();
    public string Propiedad { get; set; } = string.Empty;
}