using System;
using System.Windows;

namespace MinimalMVVM
{
    public interface IRequireViewIdentification
    {
        Guid ViewID { get; }
    }

    public static class WindowManagerModel
    {
        public static void CloseWindow(Guid id)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext is IRequireViewIdentification w_id && w_id.ViewID.Equals(id))
                {
                    window.Close();
                }
            }
        }
    }
}
