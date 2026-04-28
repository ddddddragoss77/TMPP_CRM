namespace TMPP_CRM.Domain.Bridge
{
    public abstract class MediaFile
    {
        protected IMediaDevice _device;
        public string FileName { get; set; }

        protected MediaFile(IMediaDevice device, string fileName)
        {
            _device = device;
            FileName = fileName;
        }

        public abstract string Play();
    }
}
