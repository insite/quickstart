using System;
using System.Threading.Tasks;

namespace BearerAuthDemo
{
    internal class Application
    {
        private readonly ApiClient _client;

        private readonly string _criteria;

        public Application(ApiClient client, string criteria)
        {
            _client = client;
            _criteria = criteria;
        }

        public async Task RunAsync(Action<string> output)
        {
            await new GroupDemo().RunAsync(_client, output);

            await new UserDemo().RunAsync(_client, output);

            await new GradebookDemo().RunAsync(_client, output, _criteria);

            await new PaginationDemo().RunAsync(_client, output);
        }
    }
}
