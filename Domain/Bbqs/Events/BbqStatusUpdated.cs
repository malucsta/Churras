using Domain.Common;

namespace Domain.Bbqs.Events
{
    public class BbqStatusUpdated : IEvent
    {
        public BbqStatusUpdated(bool gonnaHappen, bool trincaWillPay)
        {
            GonnaHappen = gonnaHappen;
            TrincaWillPay = trincaWillPay;
        }

        public bool GonnaHappen { get; }
        public bool TrincaWillPay { get; }
    }
}
