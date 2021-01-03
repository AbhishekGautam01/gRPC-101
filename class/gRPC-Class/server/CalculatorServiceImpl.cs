using Calculator;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
