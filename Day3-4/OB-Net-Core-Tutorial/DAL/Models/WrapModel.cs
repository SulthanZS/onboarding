namespace OB_Net_Core_Tutorial.DAL.Models
{
    public class WrapModel
    {
        public IEnumerable<Car> Cars { get; set; }
        public Car Car { get; set; }
        public IEnumerable<Type> Types { get; set; }
        public Type Type { get; set; }
    }
}
