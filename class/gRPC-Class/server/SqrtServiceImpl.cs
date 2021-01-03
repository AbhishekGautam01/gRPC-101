using Grpc.Core;
using Sqrt;
using System;
using System.Threading.Tasks;
using static Sqrt.SqrtService;

namespace server
{
    public class SqrtServiceImpl: SqrtServiceBase
    {
        public override async Task<SqrtResponse> sqrt(SqrtRequest request, ServerCallContext context)
        {
            int number = request.Number;
            if(number >= 0)
            {
                return new SqrtResponse() { SqaureRoot = Math.Sqrt(number) };
            } else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Number is smaller than 0"));
            }
        }
    }
}
