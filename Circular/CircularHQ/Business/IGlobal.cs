using CircularHQ.Models;

namespace CircularHQ.Business
{
    public interface IGlobal
    {
        public CurrentUser currentUser { get; set; }
        public string UploadFolderPath { get; set; }

        CurrentUser GetCurrentUser();
    }
}
