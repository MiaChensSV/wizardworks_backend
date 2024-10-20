namespace wizardworks_backend.Model
{
    public class BoardDataModel
    {
        public string[] AvailableColors { get; set; } = ["#FC5858", "#FCAD58", "#58FC61", "#948597", "#B058FC"];
        public List<BlockModel> BlocksState { get; set; } = [];
    }
}
