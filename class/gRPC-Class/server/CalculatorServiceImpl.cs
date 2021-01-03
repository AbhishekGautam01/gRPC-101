using Calculator;
using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Calculator.calculator;

namespace server
{
    public class CalculatorServiceImpl: calculatorBase
    {
        public override async Task PrimeDecomposition(PrimeDecompositionRequest request, IServerStreamWriter<PrimeDecompositionResponse> responseStream, ServerCallContext context)
        {
            int k = 2;
            int n = request.Input;
            while (n > 1)
            {
                if (n % k == 0)
                {
                    await responseStream.WriteAsync(new PrimeDecompositionResponse() { Result = k });
                    n = n / k;
                } else
                {
                    k = k + 1;
                }
            }
        }

        public override async Task<ComputeAverageResponse> ComputeAverage(IAsyncStreamReader<ComputeAverageRequest> requestStream, ServerCallContext context)
        {
            int currentSum = 0;
            int totalElement = 0;
            while(await requestStream.MoveNext())
            {
                totalElement++;
                currentSum += requestStream.Current.Input;
            }
            return new ComputeAverageResponse() { Response = (totalElement != 0) ? currentSum / totalElement : currentSum };
        }

        public override async Task FindMaximum(IAsyncStreamReader<FindMaximumRequest> requestStream, IServerStreamWriter<FindMaximumResponse> responseStream, ServerCallContext context)
        {
            var elements = new List<int>();
            while(await requestStream.MoveNext())
            {
                elements.Add(requestStream.Current.Request);
                await responseStream.WriteAsync(new FindMaximumResponse() { Response = elements.Max() });
            }
        }
    }
}
