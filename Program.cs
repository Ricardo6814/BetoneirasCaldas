using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

class Produto
{
    public string Nome { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public double ValorDiaria { get; set; }
}

class Aluguel
{
    public string Cliente { get; set; }
    public string Produto { get; set; }
    public int Dias { get; set; }
    public double ValorDiaria { get; set; }
    public bool Pago { get; set; }
    public DateTime Data { get; set; }

    public double ValorTotal => Dias * ValorDiaria;
}

class Program
{
    static List<Produto> produtos = new List<Produto>();
    static List<Aluguel> alugueis = new List<Aluguel>();

    static string caminhoProdutos = "produtos.txt";
    static string caminhoAlugueis = "aluguéis.txt";

    static void Main()
    {
        CarregarProdutos();
        CarregarAlugueis();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("SISTEMA DE ALUGUEL - EMPRESA DE BETONEIRA");
            Console.WriteLine("=========================================");
            Console.WriteLine("1 - Adicionar Produto");
            Console.WriteLine("2 - Registrar Aluguel");
            Console.WriteLine("3 - Ver Estoque de Equipamentos");
            Console.WriteLine("4 - Ver Aluguéis e Dívidas");
            Console.WriteLine("5 - Gerar Relatório (TXT)");
            Console.WriteLine("6 - Salvar e Sair");
            Console.WriteLine("=========================================");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": AdicionarProduto(); break;
                case "2": RegistrarAluguel(); break;
                case "3": ListarProdutos(); break;
                case "4": ListarAlugueis(); break;
                case "5": GerarRelatorio(); break;
                case "6":
                    SalvarProdutos();
                    SalvarAlugueis();
                    Console.WriteLine("Dados salvos. Até mais!");
                    return;
                default: Console.WriteLine("Opção inválida."); break;
            }

            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    static void AdicionarProduto()
    {
        Console.Write("Nome do equipamento: ");
        string nome = Console.ReadLine();

        if (BuscarProduto(nome) != null)
        {
            Console.WriteLine("Esse produto já está cadastrado!");
            return;
        }

        Console.Write("Quantidade disponível: ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("Quantidade inválida!");
            return;
        }

        Console.Write("Valor da diária (R$): ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double valor))
        {
            Console.WriteLine("Valor inválido!");
            return;
        }

        produtos.Add(new Produto { Nome = nome, QuantidadeDisponivel = quantidade, ValorDiaria = valor });
        Console.WriteLine("Produto cadastrado com sucesso!");
    }

    static void RegistrarAluguel()
    {
        Console.Write("Nome do cliente: ");
        string cliente = Console.ReadLine();

        Console.Write("Nome do equipamento: ");
        string nomeProduto = Console.ReadLine();

        Produto produto = BuscarProduto(nomeProduto);
        if (produto == null)
        {
            Console.WriteLine("Equipamento não encontrado.");
            return;
        }

        Console.Write("Dias de aluguel: ");
        if (!int.TryParse(Console.ReadLine(), out int dias) || dias <= 0)
        {
            Console.WriteLine("Quantidade inválida!");
            return;
        }

        Console.Write("Quantidade a alugar: ");
        if (!int.TryParse(Console.ReadLine(), out int qtd) || qtd <= 0 || qtd > produto.QuantidadeDisponivel)
        {
            Console.WriteLine("Quantidade inválida ou indisponível!");
            return;
        }

        Console.Write("Foi pago? (s/n): ");
        bool pago = Console.ReadLine().Trim().ToLower() == "s";

        produto.QuantidadeDisponivel -= qtd;

        for (int i = 0; i < qtd; i++)
        {
            alugueis.Add(new Aluguel
            {
                Cliente = cliente,
                Produto = nomeProduto,
                Dias = dias,
                ValorDiaria = produto.ValorDiaria,
                Pago = pago,
                Data = DateTime.Now
            });
        }

        Console.WriteLine("Aluguel registrado com sucesso!");
    }

    static void ListarProdutos()
    {
        Console.WriteLine("\nESTOQUE DE EQUIPAMENTOS");
        foreach (var p in produtos)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"Equipamento: {p.Nome}");
            Console.WriteLine($"Disponíveis: {p.QuantidadeDisponivel}");
            Console.WriteLine($"Valor Diária: R$ {p.ValorDiaria.ToString("F2", CultureInfo.InvariantCulture)}");
        }
    }

    static void ListarAlugueis()
    {
        Console.WriteLine("\nREGISTROS DE ALUGUÉIS E DÍVIDAS");
        foreach (var a in alugueis)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"Cliente: {a.Cliente}");
            Console.WriteLine($"Equipamento: {a.Produto}");
            Console.WriteLine($"Dias: {a.Dias}");
            Console.WriteLine($"Data: {a.Data:dd/MM/yyyy}");
            Console.WriteLine($"Valor Total: R$ {a.ValorTotal.ToString("F2", CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Status: {(a.Pago ? "Pago" : "EM ABERTO")}");
        }
    }

    static void GerarRelatorio()
    {
        string caminho = "relatorio_alugueis.txt";

        using (StreamWriter writer = new StreamWriter(caminho))
        {
            writer.WriteLine("RELATÓRIO DE ALUGUÉIS");
            writer.WriteLine($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}");
            writer.WriteLine();

            double totalRecebido = 0;
            double totalAberto = 0;

            foreach (var a in alugueis)
            {
                writer.WriteLine($"Cliente: {a.Cliente}");
                writer.WriteLine($"Equipamento: {a.Produto}");
                writer.WriteLine($"Dias: {a.Dias}");
                writer.WriteLine($"Data: {a.Data:dd/MM/yyyy}");
                writer.WriteLine($"Valor Total: R$ {a.ValorTotal.ToString("F2", CultureInfo.InvariantCulture)}");
                writer.WriteLine($"Status: {(a.Pago ? "Pago" : "EM ABERTO")}");
                writer.WriteLine("--------------------------------");

                if (a.Pago) totalRecebido += a.ValorTotal;
                else totalAberto += a.ValorTotal;
            }

            writer.WriteLine($"\nTotal Recebido: R$ {totalRecebido:F2}");
            writer.WriteLine($"Total em Aberto: R$ {totalAberto:F2}");
        }

        Console.WriteLine($"Relatório gerado: {Path.GetFullPath(caminho)}");
    }

    static Produto BuscarProduto(string nome)
    {
        return produtos.Find(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    static void CarregarProdutos()
    {
        if (!File.Exists(caminhoProdutos)) return;

        foreach (var linha in File.ReadAllLines(caminhoProdutos))
        {
            string[] partes = linha.Split(';');
            if (partes.Length == 3)
            {
                produtos.Add(new Produto
                {
                    Nome = partes[0],
                    QuantidadeDisponivel = int.Parse(partes[1]),
                    ValorDiaria = double.Parse(partes[2], CultureInfo.InvariantCulture)
                });
            }
        }
    }

    static void SalvarProdutos()
    {
        using (var writer = new StreamWriter(caminhoProdutos))
        {
            foreach (var p in produtos)
            {
                writer.WriteLine($"{p.Nome};{p.QuantidadeDisponivel};{p.ValorDiaria.ToString(CultureInfo.InvariantCulture)}");
            }
        }
    }

    static void CarregarAlugueis()
    {
        if (!File.Exists(caminhoAlugueis)) return;

        foreach (var linha in File.ReadAllLines(caminhoAlugueis))
        {
            string[] partes = linha.Split(';');
            if (partes.Length == 6)
            {
                alugueis.Add(new Aluguel
                {
                    Cliente = partes[0],
                    Produto = partes[1],
                    Dias = int.Parse(partes[2]),
                    ValorDiaria = double.Parse(partes[3], CultureInfo.InvariantCulture),
                    Pago = bool.Parse(partes[4]),
                    Data = DateTime.ParseExact(partes[5], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                });
            }
        }
    }

    static void SalvarAlugueis()
    {
        using (var writer = new StreamWriter(caminhoAlugueis))
        {
            foreach (var a in alugueis)
            {
                writer.WriteLine($"{a.Cliente};{a.Produto};{a.Dias};{a.ValorDiaria.ToString(CultureInfo.InvariantCulture)};{a.Pago};{a.Data:yyyy-MM-dd HH:mm:ss}");
            }
        }
    }
}
