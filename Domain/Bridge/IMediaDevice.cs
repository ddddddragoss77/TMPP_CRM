namespace TMPP_CRM.Domain.Bridge
{
    public interface IMediaDevice
    {
        string PlayAudio(string fileName);
        string PlayVideo(string fileName);
    }
}
