using System.ComponentModel;
using ModelContextProtocol.Server;
using UserManagement.Model;

namespace UserManagementConsole.Tools
{
    [McpServerToolType]
    public class SmartHomeTools
    {
        private readonly UserManagementService.ISmartHouseUtilities _smartHouseUtilities;
        public SmartHomeTools(UserManagementService.ISmartHouseUtilities smartHouseUtilities)
        {
            _smartHouseUtilities = smartHouseUtilities;
        }

        [McpServerTool, Description("Get the current status of the smart home system")]
        public string GetSmartHomeStatus()
        {
            return "All systems are operational.";
        }

        [McpServerTool, Description("Get Smart home faq data")]
        public IList<SmartHouse> GetSmartHomeFaqData()
        {
            return _smartHouseUtilities.GetSmartHouseList();
        }
    }
}
