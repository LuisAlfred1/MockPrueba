using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockPrueba.Data;
using MockPrueba.Models;
using System.Diagnostics;

namespace MockPrueba.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Sumar las ventas
            var totalVentas = _context.Ventas.Sum(v => v.TotalVenta); //Suma del total de ventas

            // Sumar las unidades vendidas
            var totalUnidades = _context.Ventas.Sum(d => d.Cantidad); // Suma de unidades
                                                                      // Obtener ventas por mes
                                                                      // Ventas agrupadas por año y mes
            var ventasPorMes = _context.Ventas
                .GroupBy(v => new { v.FechaVenta.Year, v.FechaVenta.Month })
                .Select(g => new VentasPorMes
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalVentasMes = g.Sum(v => v.TotalVenta)
                })
                .ToList();

            // Crear un modelo con los valores que vamos a pasar a la vista
            var dashboardViewModel = new DashboardViewModel
            {
                TotalVentas = totalVentas,
                TotalUnidades = totalUnidades,
                VentasPorMes = ventasPorMes,
            };

            return View(dashboardViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> CargarArchivo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Por favor, seleccione un archivo de Excel");
            }

            if (!file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Por favor, seleccione un archivo de Excel");
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataTable = reader.AsDataSet().Tables[0];
                    var listaVentas = new List<Venta>();

                    for (int fila = 1; fila < dataTable.Rows.Count; fila++)
                    {
                        // Crear una nueva instancia de Cliente
                        var cliente = new Cliente
                        {
                            Nombre = dataTable.Rows[fila][1]?.ToString(),
                            Apellido = dataTable.Rows[fila][2]?.ToString(),
                            CorreoElectronico = dataTable.Rows[fila][3]?.ToString()
                        };

                        // Buscar o crear una nueva instancia de Producto
                        var producto = new Producto
                        {
                            CodigoBarras = dataTable.Rows[fila][4]?.ToString(),
                            NombreProducto = dataTable.Rows[fila][5]?.ToString(),
                            Descripcion = dataTable.Rows[fila][6]?.ToString(),
                            Categoria = dataTable.Rows[fila][7]?.ToString(),
                            Precio = decimal.TryParse(dataTable.Rows[fila][9].ToString(), out decimal precio) ? precio : 0
                        };

                        // Crear la instancia de Venta y relacionarla con Cliente y Producto
                        var venta = new Venta
                        {
                            FechaVenta = DateTime.TryParse(dataTable.Rows[fila][0].ToString(), out DateTime fechaVenta) ? fechaVenta : DateTime.MinValue,
                            Cliente = cliente,
                            Producto = producto,
                            Cantidad = int.TryParse(dataTable.Rows[fila][8].ToString(), out int cantidad) ? cantidad : 0,
                            TotalVenta = int.TryParse(dataTable.Rows[fila][10].ToString(), out int totalVenta) ? totalVenta : 0
                        };

                        listaVentas.Add(venta);
                    }
                    // Añadir todos los datos de ventas (que incluyen clientes y productos) al contexto
                    _context.Ventas.AddRange(listaVentas);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }
    }
}
