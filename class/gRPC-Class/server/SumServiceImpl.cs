using Grpc.Core;
using Sum;
using System.Threading.Tasks;
using static Sum.SumService;

namespace server
{
    public class SumServiceImpl: SumServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            int result = request.Sum.First + request.Sum.Second;
            return Task.FromResult(new SumResponse() { Result = result});
        }
    }
}
