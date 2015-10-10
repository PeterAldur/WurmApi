using System;

namespace AldursLab.WurmApi.Extensions.DotNet
{
    static class EventHandlerExtensions
    {
        public static string MethodInformationToString<T>(this EventHandler<T> eventHandler) where T : EventArgs
        {
            return string.Format("{0}.{1}",
                eventHandler.Method.DeclaringType != null
                    ? eventHandler.Method.DeclaringType.FullName
                    : string.Empty,
                eventHandler.Method.Name);
        }

        /// <summary>
        /// Invokes event handler in a thread safe way. Defaults source to null and event args to Empty.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="source"></param>
        /// <param name="eventArgs"></param>
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> handler, object source,
            TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            if (handler != null)
            {
                handler(source, eventArgs);
            }
        }
    }
}
