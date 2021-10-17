using System;
using System.Threading.Tasks;
using API.Dtos;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using MPesa;
using Environment = MPesa.Environment;

namespace API.Helpers
{
    public class PaymentsHelper : IPayments
    {
        private readonly IConfiguration _configuration;

        public PaymentsHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<Response> DoC2B(PaymentRequest paymentRequest)
        {
            var client = new Client.Builder()
                .ApiKey(_configuration["ApiKey"])
                .PublicKey(_configuration["PublicKey"])
                .ServiceProviderCode("171717")
                .InitiatorIdentifier("SJGW67fK")
                .Environment(Environment.Development)
                .SecurityCredential("Mpesa2019")
                .Build();
            
            //C2B
            var paymentData = new Request.Builder()
                .Amount(paymentRequest.TotalPrice)
                .From($"258{paymentRequest.PhoneNumber}")
                .Reference(RandomStringGenerator.GetString())
                .Transaction("T12344A")
                .Build();

            try
            {
                var response = await client.Receive(paymentData);
                return response;

                //Once Open Open API always return Success even user inform wrong Password
                /*return new PaymentsResponseHelperDto
                {
                    ErrorMessage = response.IsSuccessfully ? "": response.Description,
                    IsSuccessfully = response.IsSuccessfully
                };*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }
    }
}