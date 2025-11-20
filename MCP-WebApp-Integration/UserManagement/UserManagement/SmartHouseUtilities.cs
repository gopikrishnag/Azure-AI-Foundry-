

using UserManagement.Model;

namespace UserManagementService;

public interface ISmartHouseUtilities
{
    public IList<SmartHouse> GetSmartHouseList();
}

public class SmartHouseUtilities : ISmartHouseUtilities
{
    public IList<SmartHouse> GetSmartHouseList()
    {
        var lst = new List<SmartHouse>
        {
            new() {Prompt="Do I need a hub?", Completion="Some devices, like smart plugs and pulps, connect directly via WiFi"},
            new() {Prompt="Is my data secure", Completion="Yes, all your data well protects"},
        };
        return lst;
    }
}
