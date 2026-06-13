using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApplication1.Controllers
{
    public class QueryController : Controller
    {
// Hardcoded connection string for this local experiment
    private readonly string _connectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=SuperSecretPass123!;TrustServerCertificate=True;";

    [HttpGet]
    public IActionResult Index()
    {
        return View(new DataTable());
    }

    [HttpPost]
    public IActionResult Index(string query)
    {
        ViewBag.Query = query; // Keep the query in the textarea after submission
        var dataTable = new DataTable();

        if (string.IsNullOrWhiteSpace(query))
        {
            ViewBag.Message = "Please enter a valid SQL query.";
            return View(dataTable);
        }

        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            using var adapter = new SqlDataAdapter(command);
            
            // Fills the DataTable with dynamic columns and rows based on the query
            adapter.Fill(dataTable); 
            
            ViewBag.Message = $"Success! {dataTable.Rows.Count} row(s) returned.";
        }
        catch (Exception ex)
        {
            // Catching all exceptions to display SQL syntax errors to the UI
            ViewBag.Error = ex.Message; 
        }

        return View(dataTable);
    }

    }
}
