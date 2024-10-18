using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using wizardworks_backend.Model;

namespace wizardworks_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        private static readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "blockList.json");

        [HttpGet]
        public async Task<IActionResult> GetBoxesResultAsync()
        {
            if (!System.IO.File.Exists(_filePath))
            {
                await System.IO.File.WriteAllTextAsync(_filePath, "[]");

                return Ok(new List<BlockModel>());
            }
            var jsonData = await System.IO.File.ReadAllTextAsync(_filePath);
            var blockList = JsonSerializer.Deserialize<List<BlockModel>>(jsonData);
            return Ok(blockList);
        }

        [HttpPost]

        public async Task<IActionResult> AddBlock([FromBody] BlockModel block)
        {
            if (block == null)
            {
                return BadRequest("Box can not be null");
            }

            List<BlockModel> blockList = new List<BlockModel>();
            if (System.IO.File.Exists(_filePath))
            {
                var jsonData = await System.IO.File.ReadAllTextAsync(_filePath);
                if (!string.IsNullOrWhiteSpace(jsonData))
                {
                    try
                    {
                        blockList = JsonSerializer.Deserialize<List<BlockModel>>(jsonData) ?? new List<BlockModel>();
                    }
                    catch (JsonException)
                    {
                        blockList = new List<BlockModel>();
                    }
                }
            }




            blockList.Add(block);

            var updatedJson = JsonSerializer.Serialize(blockList);
            await System.IO.File.WriteAllTextAsync(_filePath, updatedJson);

            return Ok(new { message = "Block added successfully" });
        }
    }
}
