namespace CS_CriptoCoinRest.Model
{
    public class CriptoCoin
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }

        public CriptoCoin()
        {
            Nome = "";
            Descricao = "";
            Preco = 0;
        }
        public CriptoCoin(string nome, string descricao, decimal preco)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
        }
    }

}
