namespace TMPP_CRM.Domain.Bridge
{
    public class AudioFile : MediaFile
    {
        public AudioFile(IMediaDevice device, string fileName) : base(device, fileName)
        {
        }

        public override string Play()
        {
            return _device.PlayAudio(FileName);
        }
    }
}
