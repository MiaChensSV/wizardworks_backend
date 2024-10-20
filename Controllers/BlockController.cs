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

        [HttpGet("GetBoardData")]
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

        [HttpPost("AddBlock")]
        public ActionResult<List<BlockModel>> AddBlock([FromBody] BlockModel block)
        {
            try
            {
                if (block == null)
                {
                    return BadRequest("Block can not be null");
                }
                // Get existing board status
                BoardDataModel boardData = _dataStorageService.GetBoardData();
                List<BlockModel> blocksState = boardData.BlocksState;

                string[] availableColors = boardData.AvailableColors;

                // Verify if color is valid.
                if (!availableColors.Contains(block.Color))
                {
                    return UnprocessableEntity("Block has invalid color.");
                }

                if (blocksState.Count > 0)
                {
                    BlockModel lastBlock = boardData.BlocksState.LastOrDefault<BlockModel>()!;
                    if (block.Color == lastBlock.Color)
                    {
                        return UnprocessableEntity("The new block has the same color with the current last block");
                    }
                }

                // Verify if position is valid
                // If no existing blocks, New block's osition must be 1 
                if (blocksState.Count == 0 && block.Position != 1)
                {
                    return UnprocessableEntity("Block has invalid position.");
                }
                // If has block, New position must be increament.
                else if (blocksState.Count > 0)
                {
                    int currentMaxPosition = blocksState.Max(block => block.Position);
                    if (block.Position != currentMaxPosition + 1)
                    {
                        return UnprocessableEntity("Block has invalid position.");
                    }
                }

                // Add the block to file storage
                _dataStorageService.AddBlockToBoardState(block);
                BoardDataModel latestBoardData = _dataStorageService.GetBoardData();
                List<BlockModel> latestBlocksState = latestBoardData.BlocksState;
                return Ok(latestBlocksState);
            }
            catch (DataStorageException exc)
            {
                return StatusCode(500, exc.Message);
            }      
            
        }

        [HttpDelete("ClearBlocksState")]
        public ActionResult<List<BlockModel>> ClearBlocksState()
        {
            try
            {
                _dataStorageService.DeleteBlocksState();
                BoardDataModel latestBlocksState = _dataStorageService.GetBoardData();
                List<BlockModel> latestBlockState = latestBlocksState.BlocksState;
                return StatusCode(200, latestBlockState);
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
