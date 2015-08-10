using System;

namespace AldursLab.WurmApi.Modules.Events
{
    class SimpleMarshaller : IEventMarshaller
    {
        public void Marshal(Action action)
        {
            action();
        }
    }
}
