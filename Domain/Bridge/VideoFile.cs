namespace TMPP_CRM.Domain.Bridge
{
    public class VideoFile : MediaFile
    {
        public VideoFile(IMediaDevice device, string fileName) : base(device, fileName)
        {
        }

        public override string Play()
        {
            return _device.PlayVideo(FileName);
        }
    }
}
