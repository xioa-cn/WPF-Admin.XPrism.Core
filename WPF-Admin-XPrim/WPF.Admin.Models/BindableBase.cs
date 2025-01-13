using XPrism.Core.BindableBase;
using XPrism.Core.Events;

namespace WPF.Admin.Models;

public class BindableBase : ViewModelBase {
    private readonly IEventAggregator? _eventAggregator;

    public BindableBase(IEventAggregator eventAggregator) {
        _eventAggregator = eventAggregator;
    }
    
    
    public BindableBase() {
        
    }
    
}