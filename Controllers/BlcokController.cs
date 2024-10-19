using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using wizardworks_backend.Model;
using wizardworks_backend.Service;

namespace wizardworks_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        private static readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "blockList.json");
        private readonly DataStorageService _dataStorageService;

        public BlockController(DataStorageService dataStorageService)
        {
            _dataStorageService = dataStorageService;
        }

        [HttpGet("/getBoardData")]
        public ActionResult<BoardDataModel> GetBoardData()
        {
            try
            {
                BoardDataModel boardData = _dataStorageService.GetBoardData();
                return Ok(boardData);
            } catch (DataStorageException exc)
            {
                return StatusCode(500, exc.Message);
            } catch (Exception)
            {
                return StatusCode(500, "Unknown error.");
            }
            
        }

        [HttpPost("/addBlock")]
        public ActionResult AddBlock([FromBody] BlockModel block)
        {
            try
            {
                if (block == null)
                {
                    return BadRequest("Block can not be null");
                }

                // Get existing board status
                BoardDataModel boardData = this._dataStorageService.GetBoardData();
                List<BlockModel> boardState = boardData.BlocksState;
                string[] availableColors = boardData.AvaiableColors;

                // Verify if color is valid.
                if (!availableColors.Contains(block.Color))
                {
                    return UnprocessableEntity("Block has invalid color.");
                }

                // Verify if position is valid
                // If no existing blocks, New block's osition must be 1 
                if (boardState.Count == 0 && block.Position != 1)
                {
                    return UnprocessableEntity("Block has invalid position.");
                }
                // If has block, New position must be increament.
                else if (boardState.Count > 0)
                {
                    int currentMaxPosition = boardState.Max(block => block.Position);
                    if (block.Position != currentMaxPosition + 1)
                    {
                        return UnprocessableEntity("Block has invalid position.");
                    }
                }

                // Add the block to file storage
                this._dataStorageService.AddBlockToBoardState(block);
            }
            catch (DataStorageException exc)
            {
                return StatusCode(500, exc.Message);
            }
       

            return Ok(new { message = "Block added successfully" });
        }

        [HttpDelete("/clearBlocksState")]
        public ActionResult ClearBlocksState()
        {
            try
            {
                this._dataStorageService.DeleteBlocksState();
                return StatusCode(200, "All blocks has been cleared");
            }
            catch (DataStorageException exc)
            {
                return StatusCode(500, exc.Message);
            }
            catch (Exception) 
            {
                return StatusCode(500, "Unknown error.");
            }
        }
    }
}
