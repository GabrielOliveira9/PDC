using CS_CriptoCoinRest.Model;
using System.Net.Http.Json;

namespace CS_CriptoCointRestClientConsole
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        //static void Main (string args[])
        static async Task Main(string[] args)
        {
            string baseUrl = "http://localhost:5074/api/CriptoCoins";

            //Listar moedas
            await GetCriptoCoins(baseUrl);

            //Inserir nova moeda
            var novaCripto = new CriptoCoin { Nome = "Ethereum", Descricao = "Contratos", Preco = 3500.75M };
            await PostCriptoCoin(baseUrl, novaCripto);

            //Listar moedas
            await GetCriptoCoins(baseUrl );

            string baseUrl2 = "http://localhost:5074/api/Wallets";

            // Listar wallets
            await GetWallets(baseUrl2);

            // Criar nova wallet
            var newWallet = new Wallet { Name = "MinhaCarteira" };
            await CreateWallet(baseUrl2, newWallet);

            // Adicionar item à wallet
            int walletId = 1; // ID da wallet Alvo
            var newItem = new WalletItem { Currency = "BTC", Amount = 1M };
            await AddItem(baseUrl2, walletId, newItem);

            // Listar novamente
            await GetWallets(baseUrl2);
        }

        static async Task GetCriptoCoins(string url)
        {
            var response = await client.GetAsync(url);

            var criptoCoins = await response.Content.ReadAsStringAsync();

            Console.WriteLine("\nResposta GET: "+  criptoCoins);
        }

        static async Task PostCriptoCoin(string url, CriptoCoin novaCripto)
        {
            var response = await client.PostAsJsonAsync(url, novaCripto);

            var resposta = await response.Content.ReadAsStringAsync();

            Console.WriteLine("\nResposta POST:" + resposta);
        }

        // ----------------------- CARTEIRAS ------------------

        static async Task GetWallets(string url)
        {
            var wallets = await client.GetFromJsonAsync<WalletDto[]>(url);
            Console.WriteLine("\nWallets:");
            foreach (var w in wallets!)
            {
                Console.WriteLine($"[{w.Id}] {w.Name} – {w.ItemCount} itens (total {w.TotalAmount})");
                foreach (var i in w.Items)
                    Console.WriteLine($"  • {i.Currency}: {i.Amount}");
            }
        }

        static async Task CreateWallet(string url, Wallet wallet)
        {
            var response = await client.PostAsJsonAsync(url, wallet);
            var created = await response.Content.ReadFromJsonAsync<WalletDto>();
            Console.WriteLine($"\nCreated Wallet: [{created!.Id}] {created.Name}");
        }

        static async Task AddItem(string baseUrl, int walletId, WalletItem item)
        {
            var url = $"{baseUrl}/{walletId}/items";
            var response = await client.PostAsJsonAsync(url, item);
            var added = await response.Content.ReadFromJsonAsync<WalletItem>();
            Console.WriteLine($"\nAdded Item: {added!.Currency} = {added.Amount} to Wallet {walletId}");
        }
    }
}
