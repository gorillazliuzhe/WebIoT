using System.Threading;
using System.Threading.Tasks;

namespace WebIoT.Playground
{
    public interface ILedClient
    {
        void LedOn();
        void LedOff();
        void LedHardPWMOff();
        void LedHardPWMOn();
        void LedSoftPWMOff(CancellationToken cancellationToken);
        Task LedSoftPWMOn(CancellationToken cancellationToken);
    }
}
