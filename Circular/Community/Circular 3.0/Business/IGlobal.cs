using CircularWeb.Models;

namespace CircularWeb.Business
{
    public interface IGlobal
    {
        public CurrentUser currentUser { get; set; }
        public string UploadFolderPath { get; set; }

        CurrentUser GetCurrentUser();
    }
}
