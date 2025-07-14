using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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

class DividaEmpresa
{
    public string Pessoa { get; set; }
    public string Descricao { get; set; }
    public double ValorTotal { get; set; }
    public double ValorPago { get; set; }
    public DateTime Data { get; set; }
    public bool Pago { get; set; }
}

class Program
{
    static List<Produto> produtos = new List<Produto>();
    static List<Aluguel> alugueis = new List<Aluguel>();
    static List<DividaEmpresa> dividas = new List<DividaEmpresa>();

    static string caminhoProdutos = "produtos.txt";
    static string caminhoAlugueis = "aluguéis.txt";
    static string caminhoDividas = "dividas.txt";

    static void Main()
    {
        CarregarProdutos();
        CarregarAlugueis();
        CarregarDividas();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE ALUGUEL - EMPRESA DE BETONEIRA ===");
            Console.WriteLine("1 - Adicionar Produto");
            Console.WriteLine("2 - Registrar Aluguel");
            Console.WriteLine("3 - Ver Estoque");
            Console.WriteLine("4 - Ver Aluguéis");
            Console.WriteLine("5 - Adicionar Dívida da Empresa");
            Console.WriteLine("6 - Ver Dívidas da Empresa");
            Console.WriteLine("7 - Registrar Pagamento de Dívida");
            Console.WriteLine("8 - Ver Dívidas (Mobile)");
            Console.WriteLine("9 - Ver Resumo Financeiro");
            Console.WriteLine("10 - Salvar e Sair");
            Console.Write("Escolha: ");
            string op = Console.ReadLine();

            switch (op)
            {
                case "1": AdicionarProduto(); break;
                case "2": RegistrarAluguel(); break;
                case "3": ListarProdutos(); break;
                case "4": ListarAlugueis(); break;
                case "5": AdicionarDivida(); break;
                case "6": ListarDividas(); break;
                case "7": RegistrarPagamento(); break;
                case "8": ListarDividasMobile(); break;
                case "9": ResumoFinanceiro(); break;
                case "10":
                    SalvarProdutos();
                    SalvarAlugueis();
                    SalvarDividas();
                    Console.WriteLine("Tudo salvo. Até logo!");
                    return;
                default: Console.WriteLine("Opção inválida."); break;
            }

            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    static void AdicionarProduto()
    {
        Console.Write("Nome: ");
        string nome = Console.ReadLine();

        Console.Write("Quantidade disponível: ");
        int qtd = int.Parse(Console.ReadLine());

        Console.Write("Valor da diária: ");
        double diaria = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

        produtos.Add(new Produto { Nome = nome, QuantidadeDisponivel = qtd, ValorDiaria = diaria });
        Console.WriteLine("Produto adicionado!");
    }

    static void RegistrarAluguel()
    {
        Console.Write("Cliente: ");
        string cliente = Console.ReadLine();

        Console.Write("Produto: ");
        string nome = Console.ReadLine();

        var p = produtos.Find(x => x.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
        if (p == null)
        {
            Console.WriteLine("Produto não encontrado.");
            return;
        }

        Console.Write("Dias de aluguel: ");
        int dias = int.Parse(Console.ReadLine());

        Console.Write("Quantidade a alugar: ");
        int qtd = int.Parse(Console.ReadLine());

        Console.Write("Pago? (s/n): ");
        bool pago = Console.ReadLine().ToLower() == "s";

        if (qtd > p.QuantidadeDisponivel)
        {
            Console.WriteLine("Quantidade indisponível.");
            return;
        }

        p.QuantidadeDisponivel -= qtd;

        for (int i = 0; i < qtd; i++)
        {
            alugueis.Add(new Aluguel
            {
                Cliente = cliente,
                Produto = nome,
                Dias = dias,
                ValorDiaria = p.ValorDiaria,
                Pago = pago,
                Data = DateTime.Now
            });
        }

        Console.WriteLine("Aluguel registrado.");
    }

    static void ListarProdutos()
    {
        foreach (var p in produtos)
        {
            Console.WriteLine($"Nome: {p.Nome} | Quantidade: {p.QuantidadeDisponivel} | Diária: R$ {p.ValorDiaria:F2}");
        }
    }

    static void ListarAlugueis()
    {
        foreach (var a in alugueis)
        {
            Console.WriteLine($"Cliente: {a.Cliente} | Produto: {a.Produto} | Dias: {a.Dias} | Data: {a.Data:dd/MM/yyyy} | Total: R$ {a.Dias * a.ValorDiaria:F2} | {(a.Pago ? "Pago" : "Aberto")}");
        }
    }

    static void AdicionarDivida()
    {
        Console.Write("Pessoa: ");
        string pessoa = Console.ReadLine();

        Console.Write("Descrição: ");
        string descricao = Console.ReadLine();

        Console.Write("Valor pago: ");
        double pago = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

        Console.Write("Data (dd/mm/yyyy): ");
        DateTime data = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

        Console.Write("Está pago? (s/n): ");
        bool quitado = Console.ReadLine().ToLower() == "s";

        dividas.Add(new DividaEmpresa
        {
            Pessoa = pessoa,
            Descricao = descricao,
            ValorPago = pago,
            Data = data,
            Pago = quitado
        });

        Console.WriteLine("Dívida registrada.");
    }

    // === ITEM 6 atualizado com resumo e TXT ===
    static void ListarDividas()
    {
        double total = 0;
        double totalPago = 0;
        double totalAberto = 0;

        var linhas = new List<string>();

        foreach (var d in dividas.OrderBy(d => d.Data))
        {
            linhas.Add("----------------------------------");
            linhas.Add($"Pessoa: {d.Pessoa}");
            linhas.Add($"Descrição: {d.Descricao}");
            linhas.Add($"Data: {d.Data:dd/MM/yyyy}");
            linhas.Add($"Valor Total: R$ {d.ValorTotal:F2}");
            linhas.Add($"Valor Pago:  R$ {d.ValorPago:F2}");
            linhas.Add($"Status: {(d.Pago ? "Pago" : "Em aberto")}");

            total += d.ValorTotal;
            totalPago += d.ValorPago;
        }

        totalAberto = total - totalPago;

        linhas.Add("");
        linhas.Add("===== RESUMO DAS DÍVIDAS =====");
        linhas.Add($"Total de Dívidas:   R$ {total:F2}");
        linhas.Add($"Total Pago:         R$ {totalPago:F2}");
        linhas.Add($"Total em Aberto:    R$ {totalAberto:F2}");

        // Mostra no console
        foreach (var linha in linhas)
            Console.WriteLine(linha);

        // Salva no arquivo txt
        File.WriteAllLines("dividas_empresa.txt", linhas);

        Console.WriteLine("\nRelatório salvo em dividas_empresa.txt");
    }

    static void RegistrarPagamento()
    {
        Console.Write("Pessoa: ");
        string pessoa = Console.ReadLine();

        var divida = dividas.FirstOrDefault(d => d.Pessoa.Equals(pessoa, StringComparison.OrdinalIgnoreCase) && !d.Pago);
        if (divida == null)
        {
            Console.WriteLine("Dívida não encontrada ou já quitada.");
            return;
        }

        Console.Write("Valor do pagamento: ");
        double valor = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);

        divida.ValorPago += valor;
        if (divida.ValorPago >= divida.ValorTotal)
            divida.Pago = true;

        Console.WriteLine("Pagamento registrado.");
    }

    static void ListarDividasMobile()
    {
        var linhas = new List<string>();
        double totalPagas = 0;
        double totalAberto = 0;

        foreach (var d in dividas.OrderBy(x => x.Data))
        {
            linhas.Add($"Pessoa: {d.Pessoa}");
            linhas.Add($"Data: {d.Data:dd/MM/yyyy}");
            linhas.Add($"Descrição: {d.Descricao}");
            linhas.Add($"Valor Pago: R$ {d.ValorPago:F2}");
            linhas.Add($"Status: {(d.Pago ? "Pago" : "Em aberto")}");
            linhas.Add("----------------------------------");

            if (d.Pago) totalPagas += d.ValorTotal;
            else totalAberto += d.ValorTotal;
        }

        double totalGeral = totalPagas + totalAberto;

        linhas.Add("== RESUMO ==");
        linhas.Add($"Total Pagas:     R$ {totalPagas:F2}");
        linhas.Add($"Total Em Aberto: R$ {totalAberto:F2}");
        linhas.Add($"TOTAL GERAL:     R$ {totalGeral:F2}");

        File.WriteAllLines("dividas_mobile.txt", linhas);
        foreach (var l in linhas) Console.WriteLine(l);
    }

    static void ResumoFinanceiro()
    {
        double totalAlugueis = alugueis.Sum(a => a.ValorTotal);
        double recebidos = alugueis.Where(a => a.Pago).Sum(a => a.ValorTotal);
        double emAberto = totalAlugueis - recebidos;

        double totalDividas = dividas.Sum(d => d.ValorTotal);
        double dividasPagas = dividas.Sum(d => d.ValorPago);

        var resumo = new List<string>
        {
            $"--- RESUMO FINANCEIRO ---",
            $"Data: {DateTime.Now:dd/MM/yyyy HH:mm}",
            $"Total Aluguéis:     R$ {totalAlugueis:F2}",
            $"Recebidos:          R$ {recebidos:F2}",
            $"Em Aberto:          R$ {emAberto:F2}",
            $"Total Dívidas:      R$ {totalDividas:F2}",
            $"Dívidas Pagas:      R$ {dividasPagas:F2}",
            $"Dívidas em Aberto:  R$ {totalDividas - dividasPagas:F2}",
            $"Saldo Final:        R$ {recebidos - dividasPagas:F2}"
        };

        File.WriteAllLines("resumo_financeiro.txt", resumo);
        foreach (var l in resumo) Console.WriteLine(l);
    }

    static void CarregarProdutos()
    {
        if (!File.Exists(caminhoProdutos)) return;
        foreach (var linha in File.ReadAllLines(caminhoProdutos))
        {
            var x = linha.Split(';');
            produtos.Add(new Produto { Nome = x[0], QuantidadeDisponivel = int.Parse(x[1]), ValorDiaria = double.Parse(x[2], CultureInfo.InvariantCulture) });
        }
    }

    static void SalvarProdutos()
    {
        using var sw = new StreamWriter(caminhoProdutos);
        foreach (var p in produtos)
            sw.WriteLine($"{p.Nome};{p.QuantidadeDisponivel};{p.ValorDiaria.ToString(CultureInfo.InvariantCulture)}");
    }

    static void CarregarAlugueis()
    {
        if (!File.Exists(caminhoAlugueis)) return;
        foreach (var linha in File.ReadAllLines(caminhoAlugueis))
        {
            var x = linha.Split(';');
            alugueis.Add(new Aluguel
            {
                Cliente = x[0],
                Produto = x[1],
                Dias = int.Parse(x[2]),
                ValorDiaria = double.Parse(x[3], CultureInfo.InvariantCulture),
                Pago = bool.Parse(x[4]),
                Data = DateTime.ParseExact(x[5], "yyyy-MM-dd", CultureInfo.InvariantCulture)
            });
        }
    }

    static void SalvarAlugueis()
    {
        using var sw = new StreamWriter(caminhoAlugueis);
        foreach (var a in alugueis)
            sw.WriteLine($"{a.Cliente};{a.Produto};{a.Dias};{a.ValorDiaria.ToString(CultureInfo.InvariantCulture)};{a.Pago};{a.Data:yyyy-MM-dd}");
    }

    static void CarregarDividas()
    {
        if (!File.Exists(caminhoDividas)) return;
        foreach (var linha in File.ReadAllLines(caminhoDividas))
        {
            var x = linha.Split(';');
            dividas.Add(new DividaEmpresa
            {
                Pessoa = x[0],
                Descricao = x[1],
                ValorPago = double.Parse(x[3], CultureInfo.InvariantCulture),
                Data = DateTime.ParseExact(x[4], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Pago = bool.Parse(x[5])
            });
        }
    }

    static void SalvarDividas()
    {
        using var sw = new StreamWriter(caminhoDividas);
        foreach (var d in dividas)
            sw.WriteLine($"{d.Pessoa};{d.Descricao};{d.ValorTotal.ToString(CultureInfo.InvariantCulture)};{d.ValorPago.ToString(CultureInfo.InvariantCulture)};{d.Data:yyyy-MM-dd};{d.Pago}");
    }
}
