using System.Threading.Tasks;
using API.Dtos;
using MPesa;

namespace API.Interfaces
{
    public interface IPayments
    {
        public Task <Response>  DoC2B(PaymentRequest paymentRequest); 
    }
}