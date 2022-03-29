using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MQTTSubscriber
{
    class Subscriber
    {
        public static object Ecoding { get; private set; }

        [Obsolete]
        static async Task Main(string[] args)
        {
            var mqttFactory = new MqttFactory();
            IMqttClient client = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                            .WithClientId(Guid.NewGuid().ToString())
                            .WithTcpServer("localhost", 1883)
                            .WithCleanSession()
                            .Build();

            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Conexão do broker foi um sucesso!");
                var topicFilter = new TopicFilterBuilder()
                                        .WithTopic("Maria")
                                        .Build();
               await client.SubscribeAsync(topicFilter);
            });
            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("O broker foi desconectado");
            });

            client.UseApplicationMessageReceivedHandler(async e => 
            {
                var texto = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"Received message - {texto}");
                
                if (!string.IsNullOrEmpty(texto))
                {
                    var httpCliente = new HttpClient();
                    var objeto = new { texto = texto };
                    var json = JsonConvert.SerializeObject(objeto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpCliente.PostAsync("https://localhost:44326/v1/mensagem", content);

                }
            });

            await client.ConnectAsync(options);

            Console.WriteLine("Por favor digite uma chave para ser exibida ");
            Console.ReadLine();

            await client.DisconnectAsync();
        }
    }
}
