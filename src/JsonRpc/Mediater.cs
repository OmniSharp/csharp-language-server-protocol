using System.Collections.Generic;
using System.Threading.Tasks;
using JsonRPC.Server;

namespace JsonRPC
{
    public class Mediater
    {
        public async Task RespondTo(IEnumerable<ErrorNotificationRequest> items)
        {
            var response = new List<Task<object>>();
            foreach (var item in items)
            {
                if (item.IsRequest)
                {
                    // Map to request
                    // item.Request.Method
                }
                else if (item.IsNotification)
                {
                    // Map to method
                    // item.Notification.Method
                }
                else
                {
                    response.Add(Task.FromResult<object>(item.Error));
                }
            }

            var result = await Task.WhenAll(response.ToArray());

            if (result.Length == 1)
            {

            }
            else
            {
                
            }
        }
    }
}