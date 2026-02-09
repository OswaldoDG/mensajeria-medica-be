using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;

namespace mensajeriamedica.services.comunicaciones.helper
{
    public static class ExcelHelper
    {
        public static void CrearExcelDesdeLista<T>(List<T> lista, string rutaArchivo)
        {
            if (lista == null || lista.Count == 0) return;

            using (var documento = SpreadsheetDocument.Create(rutaArchivo, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = documento.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylesPart.Stylesheet = GenerarEstilos();
                stylesPart.Stylesheet.Save();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                worksheetPart.Worksheet = new Worksheet();

                var sheetData = new SheetData();

                var sheets = documento.WorkbookPart.Workbook.AppendChild(new Sheets());
                var sheet = new Sheet()
                {
                    Id = documento.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Resultados"
                };
                sheets.Append(sheet);

                var propiedades = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var columnas = CalcularAnchos(lista, propiedades);

                if (columnas != null)
                {
                    worksheetPart.Worksheet.Append(columnas);
                }

                worksheetPart.Worksheet.Append(sheetData);

                var filaEncabezado = new Row();
                foreach (var prop in propiedades)
                {
                    filaEncabezado.Append(CrearCelda(prop.Name, 2));
                }
                sheetData.Append(filaEncabezado);

                foreach (var item in lista)
                {
                    var nuevaFila = new Row();
                    foreach (var prop in propiedades)
                    {
                        var valor = prop.GetValue(item);
                        string valorTexto = string.Empty;

                        if (valor != null)
                        {
                            if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                            {
                                valorTexto = ((DateTime)valor).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                            {
                                valorTexto = (bool)valor ? "True" : "False";
                            }
                            else
                            {
                                valorTexto = valor.ToString();
                            }
                        }

                        nuevaFila.Append(CrearCelda(valorTexto, 1));
                    }
                    sheetData.Append(nuevaFila);
                }

                workbookPart.Workbook.Save();
            }
        }

        private static Cell CrearCelda(string texto, uint styleIndex)
        {
            var celda = new Cell
            {
                DataType = CellValues.InlineString,
                StyleIndex = styleIndex
            };

            var inlineString = new InlineString();
            var text = new Text { Text = texto };
            inlineString.Append(text);
            celda.Append(inlineString);

            return celda;
        }

        private static Stylesheet GenerarEstilos()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(
                        new FontSize() { Val = 12 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(
                        new Bold(),
                        new FontSize() { Val = 14 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" })
                ),
                new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 })
                ),
                new Borders(
                    new Border(
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true },
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true }
                )
            );
        }

        private static Columns CalcularAnchos<T>(List<T> datos, PropertyInfo[] propiedades)
        {
            var columns = new Columns();
            double maxWidth;
            int columnIndex = 1;

            foreach (var prop in propiedades)
            {
                maxWidth = prop.Name.Length;

                foreach (var item in datos)
                {
                    var valor = prop.GetValue(item);
                    if (valor != null)
                    {
                        int length = valor.ToString().Length;
                        if (length > maxWidth)
                        {
                            maxWidth = length;
                        }
                    }
                }

                maxWidth = maxWidth + 2;

                columns.Append(new Column()
                {
                    Min = (uint)columnIndex,
                    Max = (uint)columnIndex,
                    Width = maxWidth,
                    CustomWidth = true
                });

                columnIndex++;
            }

            return columns;
        }
    }
}
