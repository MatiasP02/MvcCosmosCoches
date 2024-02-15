using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using MvcCosmosCoches.Models;

namespace MvcCosmosCoches.Services
{
    public class ServiceCosmosDb
    {
        //todo funciona con un client de cosmos
        //hemos creado una cuenta en un endpoint llamada cochescls
        DocumentClient client;
        String bbdd;
        String collection;
        public ServiceCosmosDb(IConfiguration configuration)
        {
            String endpoint = configuration["CosmosDb:endPoint"];
            String primarykey = configuration["CosmosDb:primaryKey"];
            this.bbdd = "CochesBD";
            this.collection = "CochesCollection";
            this.client = new DocumentClient(new Uri(endpoint), primarykey);
        }
        public async Task CrearBbddCocheAsync()
        {
            Database bbdd = new Database() { Id = this.bbdd };
            await this.client.CreateDatabaseAsync(bbdd);
        }
        public async Task CrearColeccionCochesAsync()
        {
            DocumentCollection coleccion = new DocumentCollection() { Id = this.collection };
            //Factory es para recuperar de cosmos la base de datos
            await this.client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(this.bbdd), coleccion);
        }
        public async Task InsertarCoche(Coche coche)
        {
            //recuperamos la URI para la coleccion donde ira la película
            Uri uri = UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            await this.client.CreateDocumentAsync(uri, coche);
        }
        public List<Coche> GetCoches()
        {
            // debemos indicar el numero de coches a recuperar
            FeedOptions options = new FeedOptions() { MaxItemCount = -1 };
            String sql = "SELECT * FROM C"; // a todo lo llama 'c'
            Uri uri = UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            IQueryable<Coche> consulta = this.client.CreateDocumentQuery<Coche>(uri, sql, options);
            return consulta.ToList();
        }

        public async Task<Coche> FindCocheAsyn(String id)
        {
            Uri uri = UriFactory.CreateDocumentUri(this.bbdd, this.collection, id);
            //lo que recupera es de la clase document

            Document document = await this.client.ReadDocumentAsync(uri);
            //este documento es un stream
            //guardamos en el objeto stream en memoria lo que recuperamos, para luego leerlo en memoria
            MemoryStream memory = new MemoryStream();
            using (var stream = new StreamReader(memory))
            {
                document.SaveTo(memory);
                memory.Position = 0;
                //deserializamos con JsonConvert
                Coche coche = JsonConvert.DeserializeObject<Coche>(await stream.ReadToEndAsync());
                return coche;
            }
        }
        public async Task ModificarCoche(Coche coche)
        {
            Uri uri = UriFactory.CreateDocumentUri(this.bbdd, this.collection, coche.Id);
            await this.client.ReplaceDocumentAsync(uri, coche);
        }

        public async Task EliminarCoche(String id)
        {
            Uri uri = UriFactory.CreateDocumentUri(this.bbdd, this.collection, id);
            await this.client.DeleteDocumentAsync(uri);
        }

        public List<Coche> BuscarCoches(String marca)
        {
            //Dejo preparado un método de búsqueda por si queremos ampliar el ejercicio
            FeedOptions options = new FeedOptions() { MaxItemCount = -1 };
            Uri uri = UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            String sql = "select * from c where c.Marca='" + marca + "'";
            IQueryable<Coche> query = this.client.CreateDocumentQuery<Coche>(uri, sql, options);
            IQueryable<Coche> querylambda = this.client.CreateDocumentQuery<Coche>(uri, options)
                    .Where(z => z.Marca == marca);

            return query.ToList();
        }

        public List<Coche> CrearCoches()
        {
            List<Coche> coches = new List<Coche>() {
            new Coche
            {
                Id="1",Marca="Toyota", Modelo="Yaris",
                Motor= new Motor{Caballos=110,Cilindrada=150,Tipo="Diesel", Turbo=true}, VelocidadMaxima = 160
            },
             new Coche
            {
               Id="2",Marca="Seat", Modelo="Leon", VelocidadMaxima = 160
            }
            };
            return coches;


        }

    }
}
