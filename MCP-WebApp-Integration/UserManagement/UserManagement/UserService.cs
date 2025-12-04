

using Azure.Data.Tables;

namespace UserManagementServices
{
    public class UserService(TableClient tableClient)
    {
        public async Task<string> CreateNewUserAccount(
            string fullName,
            string emailAddress,
            string phoneNumber,
            string password)
        {
            string userId = Guid.NewGuid().ToString();
            var newUserAccount = new UserManagement.Model.Entities.UserAccountEntity
            {
                Id = userId,
                PartitionKey = "UserAccount",
                RowKey = userId,
                FullName = fullName,
                EmailAddress = emailAddress,
                PhoneNumber = phoneNumber,
                password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password))
            };

            await tableClient.AddEntityAsync(newUserAccount);
            return userId;
        }
    }
}
