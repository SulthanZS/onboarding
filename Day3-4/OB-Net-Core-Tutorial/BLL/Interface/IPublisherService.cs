using OB_Net_Core_Tutorial.BLL.DTO;
using OB_Net_Core_Tutorial.DAL.Models;


namespace OB_Net_Core_Tutorial.BLL.Interface
{
    public interface IPublisherService
    {
        Task SendCarToEventHub(Car car);
    }
}
