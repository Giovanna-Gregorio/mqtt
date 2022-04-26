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
                Console.WriteLine("Conectado ao broker com sucesso ");
                var topicFilter = new TopicFilterBuilder()  
                            .WithTopic("Maria")
                            .Build();

                await client.SubscribeAsync(topicFilter); 
            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Desconectado do broker com sucesso");
            });

            client.UseApplicationMessageReceivedHandler(async e => 
            {
                
                Console.WriteLine($"mensagem recebida {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}\n");
                var enviar = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                if(enviar!=null)  
                {
                    var httpCliente = new HttpClient();
                    var objeto = new { mensagem = enviar };
                    var content = ToRequest(objeto);
                    var response = await httpCliente.PostAsync("https://localhost:44326/v1/MensagemBroker", content);

                }

            });

            
            await client.ConnectAsync(options);

            Console.ReadLine();

            //await client.DisconnectAsync();


        }

  
        private static StringContent ToRequest(object obj) // metodo para serealizar em json
        {
            var json = JsonConvert.SerializeObject(obj);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            return data;
        }
    }
}