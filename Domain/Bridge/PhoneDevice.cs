namespace TMPP_CRM.Domain.Bridge
{
    public class PhoneDevice : IMediaDevice
    {
        public string PlayAudio(string fileName)
        {
            return $"Playing audio '{fileName}' on Phone speaker.";
        }

        public string PlayVideo(string fileName)
        {
            return $"Playing video '{fileName}' on Phone screen.";
        }
    }
}
