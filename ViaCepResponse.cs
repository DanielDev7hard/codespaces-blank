  using System.Text.Json.Serialization;

namespace FrontEnd
{
    public class ViaCepResponse
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ViaCepFinder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://viacep.com.br/ws/");

            Console.WriteLine("========================================");
            Console.WriteLine("    BUSCADOR DE ENDEREÇO VIA CEP (C#)   ");
            Console.WriteLine("========================================");

            Console.Write("\nDigite o CEP (somente números): ");
            string cep = Console.ReadLine() ?? "";
            cep = cep.Trim().Replace("-", "").Replace(" ", "");

            if (string.IsNullOrEmpty(cep) || cep.Length != 8)
            {
                Console.WriteLine("❌ Erro: O CEP deve conter exatamente 8 dígitos.");
                return;
            }

            try
            {
                Console.WriteLine("Buscando dados na API...");
                var response = await client.GetAsync($"{cep}/json/");

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var endereco = JsonSerializer.Deserialize<ViaCepResponse>(jsonString);

                    if (endereco != null && !endereco.Erro)
                    {
                        Console.WriteLine("\n--- Endereço Encontrado ---");
                        Console.WriteLine($"Logradouro: {endereco.Logradouro}");
                        Console.WriteLine($"Bairro:     {endereco.Bairro}");
                        Console.WriteLine($"Cidade/UF:  {endereco.Localidade} - {endereco.Uf}");
                        Console.WriteLine($"CEP:        {endereco.Cep}");
                    }
                    else
                    {
                        Console.WriteLine("\n⚠️ CEP não encontrado.");
                    }
                }
                else
                {
                    Console.WriteLine($"\n❌ Erro na requisição HTTP.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
            }
        }
    }
}