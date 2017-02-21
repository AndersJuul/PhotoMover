namespace AJF.PhotoMover.Service
{
    public class PhotoMoverConfig : IPhotoMoverConfig
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}