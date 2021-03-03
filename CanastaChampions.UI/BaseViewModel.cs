using System.ComponentModel;

namespace CanastaChampions.UI
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void ModelEventHandler(object obj, BaseNotification e);
        public event ModelEventHandler ModelEvent;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void InvokeEvent(object obj, BaseNotification action)
        {
            ModelEvent?.Invoke(obj, action);
        }
    }
}
