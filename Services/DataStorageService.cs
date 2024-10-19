using System.Text.Json;
using wizardworks_backend.Model;

namespace wizardworks_backend.Service
{
    public class DataStorageException : Exception
    {
        public DataStorageException(string message) : base(message) { }
    }

    public class DataStorageService
    {
        private static readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "DataStorage.json");
        private BoardDataModel? _boardData;

        public BoardDataModel GetBoardData()
        {
            try
            {
                ReadModelFromFile();
                if (_boardData == null)
                {
                    throw new DataStorageException("Fail to read data from file.");
                }
                return _boardData;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void AddBlockToBoardState(BlockModel block)
        {
            try
            {
                ReadModelFromFile();
                _boardData!.BlocksState.Add(block);
                WriteModelToFile();
            }
            catch (DataStorageException)
            {
                throw;
            }
        }

        public void DeleteBlocksState()
        {
            try
            {
                InitFileAndModel();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void InitFileAndModel()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
                _boardData = new BoardDataModel();
                string _boardDataStr = JsonSerializer.Serialize(_boardData);
                File.WriteAllText(_filePath, _boardDataStr);
            }
            catch (IOException)
            {
                throw new DataStorageException("Fail to initialize the file.");
            }
        }

        private void ReadModelFromFile()
        {
            try
            {
                // If file does not exist, create empty Board Data and create new file write the JSON data.
                if (!File.Exists(_filePath))
                {
                    InitFileAndModel();
                }
                else
                // If file exist, read JSON data.
                {
                    string _boardDataStr = File.ReadAllText(_filePath);
                    // If file has wrong format, recreate the file.

                    _boardData = JsonSerializer.Deserialize<BoardDataModel>(_boardDataStr);
                    if (_boardData == null)
                    {
                        InitFileAndModel();
                    }
                }
            }
            catch (JsonException)
            {
                InitFileAndModel();
            }
            catch (DataStorageException)
            {
                throw;
            }

        }

        private async void WriteModelToFile()
        {
            if (_boardData == null || !File.Exists(_filePath))
            {
                throw new DataStorageException("Fail to write data to file.");
            };
            string boardDataStr = JsonSerializer.Serialize(_boardData);
            await File.WriteAllTextAsync(_filePath, boardDataStr);
        }

         
    }
}
