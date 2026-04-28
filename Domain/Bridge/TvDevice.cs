namespace TMPP_CRM.Domain.Bridge
{
    public class TvDevice : IMediaDevice
    {
        public string PlayAudio(string fileName)
        {
            return $"Playing audio '{fileName}' on TV surround system.";
        }

        public string PlayVideo(string fileName)
        {
            return $"Playing video '{fileName}' on TV 4K screen.";
        }
    }
}
