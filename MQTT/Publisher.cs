using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Threading.Tasks;

namespace MQTTPublisher
{
    class Publisher
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
            client.UseConnectedHandler(e =>    
            {
                Console.WriteLine("Conectado com sucesso\n ");
            });

            client.UseDisconnectedHandler(e =>  
            {
                //Console.WriteLine("Desconectado do broker com sucesso");
            });
            string x;
            //await client.ConnectAsync(options); 
            while (true)
            {
                Console.WriteLine("Por favor, pressione uma tecla para publicar a mensagem");
                Console.ReadLine();


                await PublishMessageAsync(client);
                Console.WriteLine("Para sair presione uma tecla - x, ou qualquer tecla para continuar\n");
                x = Console.ReadLine();
                if (x == "x")
                {
                    await client.DisconnectAsync();  
                    break;
                }

            }

        }

        static async Task PublishMessageAsync(IMqttClient client) 
        {
            Console.WriteLine("Por favor, digite a mensagem\n");
            var mensagem = Console.ReadLine();
            string messagePayLoad = $"{mensagem} ";
            var message = new MqttApplicationMessageBuilder()
                        .WithTopic("Maria") 
                        .WithPayload(messagePayLoad)
                        .WithAtLeastOnceQoS()
                        .Build();

            if (client.IsConnected)  
            {
                await client.PublishAsync(message);
                Console.WriteLine($"mensagem publicada - {messagePayLoad}\n");
            }
        }
    }
}