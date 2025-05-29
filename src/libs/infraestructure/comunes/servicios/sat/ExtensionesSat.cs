namespace comunes.servicios.crm.sat;

public static class ExtensionesSat
{

    public static string RemoveSpahished(this string text)
    {
        text = text.Replace('á', 'a');
        text = text.Replace('é', 'e');
        text = text.Replace('í', 'i');
        text = text.Replace('ó', 'o');
        text = text.Replace('ú', 'u');

        text = text.Replace('Á', 'a');
        text = text.Replace('É', 'e');
        text = text.Replace('Í', 'i');
        text = text.Replace('Ó', 'o');
        text = text.Replace('Ú', 'u');

        text = text.Replace('ñ', 'n');
        text = text.Replace('Ñ', 'n');
        return text;
    }

    public static List<FormaPago> ObtieneFormasDePago()
    {
        List<FormaPago> l = [];
        l.Add(new FormaPago() { Clave = "01", Nombre = "Efectivo" });
        l.Add(new FormaPago() { Clave = "02", Nombre = "Cheque nominativo" });
        l.Add(new FormaPago() { Clave = "03", Nombre = "Transferencia electrónica de fondos" });
        l.Add(new FormaPago() { Clave = "04", Nombre = "Tarjeta de crédito" });
        l.Add(new FormaPago() { Clave = "05", Nombre = "Monedero electrónico" });
        l.Add(new FormaPago() { Clave = "06", Nombre = "Dinero electrónico" });
        l.Add(new FormaPago() { Clave = "08", Nombre = "Vales de despensa" });
        l.Add(new FormaPago() { Clave = "12", Nombre = "Dación en pago" });
        l.Add(new FormaPago() { Clave = "13", Nombre = "Pago por subrogación" });
        l.Add(new FormaPago() { Clave = "14", Nombre = "Pago por consignación" });
        l.Add(new FormaPago() { Clave = "15", Nombre = "Condonación" });
        l.Add(new FormaPago() { Clave = "17", Nombre = "Compensación" });
        l.Add(new FormaPago() { Clave = "23", Nombre = "Novación" });
        l.Add(new FormaPago() { Clave = "24", Nombre = "Confusión" });
        l.Add(new FormaPago() { Clave = "25", Nombre = "Remisión de deuda" });
        l.Add(new FormaPago() { Clave = "26", Nombre = "Prescripción o caducidad" });
        l.Add(new FormaPago() { Clave = "27", Nombre = "A satisfacción del acreedor" });
        l.Add(new FormaPago() { Clave = "28", Nombre = "Tarjeta de débito" });
        l.Add(new FormaPago() { Clave = "29", Nombre = "Tarjeta de servicios" });
        l.Add(new FormaPago() { Clave = "30", Nombre = "Aplicación de anticipos" });
        l.Add(new FormaPago() { Clave = "31", Nombre = "Intermediario pagos" });
        l.Add(new FormaPago() { Clave = "99", Nombre = "Por definir" });
        return l;
    }

    public static List<MetodoPago> ObtieneMetodosDePago()
    {
        List<MetodoPago> l = [];
        l.Add(new MetodoPago() { Clave = "PUE", Nombre = "Pago en una sola exhibición" });
        l.Add(new MetodoPago() { Clave = "PPD", Nombre = "Pago en parcialidades o diferido" });
        return l;
    }

    public static List<RegimenFiscal> ObtieneRegimenesFiscales()
    {
        List<RegimenFiscal> l = [];
        l.Add(new RegimenFiscal() { Clave = "601", Nombre = "General de Ley Personas Morales", PFisica = false, PMoral = true });
        l.Add(new RegimenFiscal() { Clave = "603", Nombre = "Personas Morales con Fines no Lucrativos", PFisica = false, PMoral = true });
        l.Add(new RegimenFiscal() { Clave = "605", Nombre = "Sueldos y Salarios e Ingresos Asimilados a Salarios", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "606", Nombre = "Arrendamiento", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "607", Nombre = "Régimen de Enajenación o Adquisición de Bienes", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "608", Nombre = "Demás ingresos", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "610", Nombre = "Residentes en el Extranjero sin Establecimiento Permanente en México", PFisica = true, PMoral = true});
        l.Add(new RegimenFiscal() { Clave = "611", Nombre = "Ingresos por Dividendos (socios y accionistas)", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "612", Nombre = "Personas Físicas con Actividades Empresariales y Profesionales", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "614", Nombre = "Ingresos por intereses", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "615", Nombre = "Régimen de los ingresos por obtención de premios", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "616", Nombre = "Sin obligaciones fiscales", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "620", Nombre = "Sociedades Cooperativas de Producción que optan por diferir sus ingresos", PFisica = false, PMoral = true });
        l.Add(new RegimenFiscal() { Clave = "621", Nombre = "Incorporación Fiscal", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "622", Nombre = "Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras", PFisica = false, PMoral = true });
        l.Add(new RegimenFiscal() { Clave = "623", Nombre = "Opcional para Grupos de Sociedades", PFisica = false, PMoral = true });
        l.Add(new RegimenFiscal() { Clave = "624", Nombre = "Coordinados", PFisica = false, PMoral = true });
        l.Add(new RegimenFiscal() { Clave = "625", Nombre = "Régimen de las Actividades Empresariales con ingresos a través de Plataformas Tecnológicas", PFisica = true, PMoral = false });
        l.Add(new RegimenFiscal() { Clave = "626", Nombre = "Régimen Simplificado de Confianza", PFisica = true, PMoral = true });
        return l;

    }

    public static List<UsoCfdi> ObtieneUsosCfdi()
    {
        List<UsoCfdi> l = [];
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "G01", Nombre = "Adquisición de mercancías.", AplicaPFisica = true, AplicaPMoral = true  });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,616,620,621,622,623,624,625,626", Clave = "G02", Nombre = "Devoluciones, descuentos o bonificaciones.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "G03", Nombre = "Gastos en general.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I01", Nombre = "Construcciones.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I02", Nombre = "Mobiliario y equipo de oficina por inversiones.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I03", Nombre = "Equipo de transporte.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I04", Nombre = "Equipo de computo y accesorios.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I05", Nombre = "Dados, troqueles, moldes, matrices y herramental.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I06", Nombre = "Comunicaciones telefónicas.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I07", Nombre = "Comunicaciones satelitales.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,606,612,620,621,622,623,624,625,626", Clave = "I08", Nombre = "Otra maquinaria y equipo.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D01", Nombre = "Honorarios médicos, dentales y gastos hospitalarios.", AplicaPFisica = true, AplicaPMoral = false});
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D02", Nombre = "Gastos médicos por incapacidad o discapacidad.", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D03", Nombre = "Gastos funerales.", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D04", Nombre = "Donativos.", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D05", Nombre = "Intereses reales efectivamente pagados por créditos hipotecarios (casa habitación).", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D06", Nombre = "Aportaciones voluntarias al SAR.", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D07", Nombre = "Primas por seguros de gastos médicos.", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D08", Nombre = "Gastos de transportación escolar obligatoria.", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D09", Nombre = "Depósitos en cuentas para el ahorro, primas que tengan como base planes de pensiones.", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "605,606,608,611,612,614,607,615,625", Clave = "D10", Nombre = "Pagos por servicios educativos (colegiaturas).", AplicaPFisica = true, AplicaPMoral = false });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,605,606,608,610,611,612,614,616,620,621,622,623,624,607,615,625,626", Clave = "S01", Nombre = "Sin efectos fiscales.", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "601,603,605,606,608,610,611,612,614,616,620,621,622,623,624,607,615,625,626", Clave = "CP01", Nombre = "Pagos", AplicaPFisica = true, AplicaPMoral = true });
        l.Add(new UsoCfdi() { RegimenReceptor = "605", Clave = "CN01", Nombre = "Nómina", AplicaPFisica = true, AplicaPMoral = false });
        return l;
    }
}
