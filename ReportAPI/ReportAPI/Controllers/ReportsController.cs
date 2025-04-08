using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReportAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private static readonly string[] Parttners = new[]
        {
            "Sber", "TBank", "VTB", "Alfa", "Tatneft", "Transneft", "Rusal", "Alrosa", "Sibur"
        };

        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ILogger<ReportsController> logger)
        {
            _logger = logger;
        }

        //[Authorize("IsProthetic")]
        [HttpGet(Name = "GetReports")]
        public IEnumerable<OrderInfo> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new OrderInfo
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Donation = Random.Shared.Next(1000, 1000000),
                Partner = Parttners[Random.Shared.Next(Parttners.Length)]
            })
            .ToArray();
        }
    }
}
