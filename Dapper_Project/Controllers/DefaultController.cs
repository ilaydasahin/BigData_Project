using Dapper_Project.DAL.DTOs;
using Dapper_Project.DAL.Entities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Dapper_Project.Controllers
{
    public class DefaultController : Controller
    {
        private readonly string _connectionString = "Server =DESKTOP-1SCT98J; initial catalog = CARPLATES; integrated security = true";
        public async Task<IActionResult> Index()
        {
            await using var connection = new SqlConnection(_connectionString);
            //var values = (await connection.QueryAsync<PLATES>("Select * from PLATES")).AsList();
            //return View(values);



            //markaların sayılarına göre
            var brandMax = (await connection.QueryAsync<BrandResult>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count DESC")).FirstOrDefault();
            var brandMin = (await connection.QueryAsync<BrandResult>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count ASC")).FirstOrDefault();


            // renk
            //var colorMax = (await connection.QueryAsync<ColorResult>("SELECT TOP 1 COLOR, COUNT(*) AS count FROM PLATES GROUP BY COLOR ORDER BY count DESC")).FirstOrDefault();
            //var colorMin = (await connection.QueryAsync<ColorResult>("SELECT TOP 1 COLOR, COUNT(*) AS count FROM PLATES GROUP BY COLOR ORDER BY count ASC")).FirstOrDefault();

            //plaka
            var plateMax = (await connection.QueryAsync<PlateResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count DESC")).FirstOrDefault();
            var plateMin = (await connection.QueryAsync<PlateResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count ASC")).FirstOrDefault();


            var fuelType = (await connection.QueryAsync<FuelResult>("SELECT TOP 1 FUEL, COUNT(*) AS count FROM PLATES GROUP BY FUEL ORDER BY count DESC")).FirstOrDefault();

            var shiftType = (await connection.QueryAsync<ShiftResult>("SELECT TOP 1 SHIFTTYPE, COUNT(*) AS count FROM PLATES GROUP BY SHIFTTYPE ORDER BY count DESC")).FirstOrDefault();

            //var caseType = (await connection.QueryAsync<CaseTypeConclusion>("SELECT TOP 1 CASETYPE, COUNT(*) AS count FROM PLATES GROUP BY CASETYPE ORDER BY count DESC")).FirstOrDefault();

            //marka
            ViewData["brandMax"] = brandMax.BRAND;
            ViewData["countMax"] = brandMax.COUNT;

            ViewData["brandMin"] = brandMin.BRAND;
            ViewData["countMin"] = brandMin.COUNT;

            ////color
            //ViewData["colorMax"] = colorMax.COLOR;
            //ViewData["countMin"] = colorMax.COUNT;

            //ViewData["colorMin"] = colorMin.COLOR;
            //ViewData["countMin"] = colorMin.COUNT;

            //plaka
            ViewData["plateMax"] = plateMax.PLATE;
            ViewData["countMax"] = plateMax.COUNT;

            ViewData["plateMin"] = plateMin.PLATE;
            ViewData["countMin"] = plateMin.COUNT;

            //yakıt türü
            ViewData["fuelType"] = fuelType.FUEL;
            ViewData["fuelTypeCount"] = fuelType.COUNT;

            //vites
            ViewData["shiftType"] = shiftType.SHIFTTYPE;
            ViewData["shiftTypeCount"] = shiftType.COUNT;


            //ViewData["caseType"] = caseType.CASETYPE;
            //ViewData["caseTypeCount"] = caseType.COUNT;




            return View();

        }



        public async Task<IActionResult> Search(string keyword)
        {

            string query = @"
            SELECT TOP 1000 BRAND,COLOR, SUBSTRING(PLATE, 1, 2) AS PlatePrefix, SHIFTTYPE, FUEL
            FROM PLATES
            WHERE
               BRAND LIKE '%' + @Keyword + '%'
            
               OR PLATE LIKE '%' + @Keyword + '%'
               OR SHIFTTYPE LIKE '%' + @Keyword + '%'
               OR FUEL LIKE '%' + @Keyword + '%'
        ";

            await using var connection = new SqlConnection(_connectionString);
            connection.Open();

            //sorguyu çalıştırmak ve sonuç alamak için 
            // "SearchConclusion" sınıfı, sorgu sonuçlarıyla eşleşen verileri temsil eden bir model sınıfı olmalıdır.
            var searchResults = await connection.QueryAsync<SearchResult>(query, new { Keyword = keyword });

            // josn şeklinde  JsonResult" tipinde sonuç döndürü
            return Json(searchResults);

        }




    }

}