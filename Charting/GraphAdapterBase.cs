using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using xLib.Common;
using xLib.Templates;
using xLib.UI;

namespace xLib.Charting
{
    public class GraphAdapterBase : UITemplateAdapter, INotifyPropertyChanged, IDisposable
    {
        public ObservableCollection<UIProperty> Propertys { get; set; }

        public UIProperty<int> WindowSize = new UIProperty<int>(new TemplateTextBox("Value")) { Name = nameof(WindowSize), Value = 500 };

        public virtual DispatcherObject Dispatcher { get; set; } = xSupport.Context;

        public AutoResetEvent data_synchronizer = new AutoResetEvent(true);

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }

        public GraphAdapterBase()
        {
            Propertys = new ObservableCollection<UIProperty>
            {
                WindowSize
            };

            TemplateInit();
        }

        public virtual void UpdateTemplate()
        {

        }

        protected virtual void TemplateInit()
        {

        }

        public virtual void Clear()
        {

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Dispose()
        {

        }

        public virtual void Save(string path)
        {

        }

        public virtual async Task SaveAsync(string path)
        {
            await Task.Run(() => Save(path));
        }
    }
}