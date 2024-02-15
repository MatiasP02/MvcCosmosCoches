using Newtonsoft.Json;

namespace MvcCosmosCoches.Models
{
    public class Motor
    {
        public String Tipo { get; set; }
        public int Caballos { get; set; }
        public int Cilindrada { get; set; }
        public Boolean Turbo { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
