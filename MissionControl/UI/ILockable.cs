using System;
namespace MissionControl.UI
{
    public interface ILockable
    {
        void Lock();
        void Unlock();
    }
}
