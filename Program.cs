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
    public bool Devolvido { get; set; }

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

    static string pastaDados = "data";
    static string caminhoProdutos = Path.Combine(pastaDados, "produtos.txt");
    static string caminhoAlugueis = Path.Combine(pastaDados, "aluguéis.txt");
    static string caminhoDividas = Path.Combine(pastaDados, "dividas.txt");

    static CultureInfo culturaBR = new CultureInfo("pt-BR");

    static void Main()
    {
        if (!Directory.Exists(pastaDados))
            Directory.CreateDirectory(pastaDados);

        CarregarDados();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== SISTEMA DE ALUGUEL - EMPRESA DE BETONEIRA ===");
            Console.WriteLine("1 - Adicionar Produto");
            Console.WriteLine("2 - Registrar Aluguel");
            Console.WriteLine("3 - Ver Estoque");
            Console.WriteLine("4 - Ver Aluguéis");
            Console.WriteLine("5 - Registrar Devolução de Equipamento");
            Console.WriteLine("6 - Adicionar Dívida da Empresa");
            Console.WriteLine("7 - Ver Dívidas da Empresa");
            Console.WriteLine("8 - Registrar Pagamento de Dívida");
            Console.WriteLine("9 - Ver Dívidas (Mobile)");
            Console.WriteLine("10 - Ver Resumo Financeiro");
            Console.WriteLine("11 - Salvar e Sair");
            Console.Write("Escolha: ");
            string op = Console.ReadLine();

            switch (op)
            {
                case "1": AdicionarProduto(); break;
                case "2": RegistrarAluguel(); break;
                case "3": ListarProdutos(); break;
                case "4": ListarAlugueis(); break;
                case "5": RegistrarDevolucao(); break;
                case "6": AdicionarDivida(); break;
                case "7": ListarDividas(); break;
                case "8": RegistrarPagamento(); break;
                case "9": ListarDividasMobile(); break;
                case "10": ResumoFinanceiro(); break;
                case "11":
                    SalvarTudo();
                    Console.WriteLine("\nTudo salvo. Até logo!");
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nOpção inválida.");
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    static void CarregarDados()
    {
        CarregarProdutos();
        CarregarAlugueis();
        CarregarDividas();
    }

    static void SalvarTudo()
    {
        SalvarProdutos();
        SalvarAlugueis();
        SalvarDividas();
    }

    static void AdicionarProduto()
    {
        Console.Write("Nome: ");
        string nome = Console.ReadLine();

        Console.Write("Quantidade disponível: ");
        if (!int.TryParse(Console.ReadLine(), out int qtd) || qtd < 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Quantidade inválida.");
            Console.ResetColor();
            return;
        }

        Console.Write("Valor da diária: ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double diaria) || diaria < 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Valor inválido.");
            Console.ResetColor();
            return;
        }

        produtos.Add(new Produto { Nome = nome, QuantidadeDisponivel = qtd, ValorDiaria = diaria });
        SalvarTudo();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Produto adicionado e salvo!");
        Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Produto não encontrado.");
            Console.ResetColor();
            return;
        }

        Console.Write("Dias de aluguel: ");
        if (!int.TryParse(Console.ReadLine(), out int dias) || dias <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Número de dias inválido.");
            Console.ResetColor();
            return;
        }

        Console.Write("Quantidade a alugar: ");
        if (!int.TryParse(Console.ReadLine(), out int qtd) || qtd <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Quantidade inválida.");
            Console.ResetColor();
            return;
        }

        if (qtd > p.QuantidadeDisponivel)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Quantidade indisponível.");
            Console.ResetColor();
            return;
        }

        Console.Write("Pago? (s/n): ");
        string pagoStr = Console.ReadLine().ToLower();
        bool pago = pagoStr == "s" || pagoStr == "sim";

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
                Data = DateTime.Now,
                Devolvido = false
            });
        }

        SalvarTudo();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Aluguel registrado e salvo!");
        Console.ResetColor();
    }

    static void RegistrarDevolucao()
    {
        Console.WriteLine("=== REGISTRAR DEVOLUÇÃO DE EQUIPAMENTO ===");

        var alugueisAtivos = alugueis.Where(a => !a.Devolvido).ToList();

        if (alugueisAtivos.Count == 0)
        {
            Console.WriteLine("Não há equipamentos alugados no momento.");
            return;
        }

        Console.WriteLine("\nEquipamentos alugados:");
        for (int i = 0; i < alugueisAtivos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - Cliente: {alugueisAtivos[i].Cliente} | Produto: {alugueisAtivos[i].Produto} | Data: {alugueisAtivos[i].Data:dd/MM/yyyy}");
        }

        Console.Write("\nSelecione o equipamento a devolver (número): ");
        if (!int.TryParse(Console.ReadLine(), out int selecao) || selecao < 1 || selecao > alugueisAtivos.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Seleção inválida.");
            Console.ResetColor();
            return;
        }

        var aluguel = alugueisAtivos[selecao - 1];
        aluguel.Devolvido = true;

        var produto = produtos.FirstOrDefault(p => p.Nome.Equals(aluguel.Produto, StringComparison.OrdinalIgnoreCase));
        if (produto != null)
        {
            produto.QuantidadeDisponivel++;
        }

        SalvarTudo();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nEquipamento {aluguel.Produto} devolvido por {aluguel.Cliente} em {DateTime.Now:dd/MM/yyyy}");
        Console.WriteLine($"Quantidade disponível atualizada: {produto?.QuantidadeDisponivel ?? 0}");
        Console.ResetColor();
    }

    static void ListarProdutos()
    {
        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (var p in produtos)
        {
            double valorTotalEstoque = p.QuantidadeDisponivel * p.ValorDiaria;
            Console.WriteLine($"Nome: {p.Nome} | Quantidade: {p.QuantidadeDisponivel} | Diária: {p.ValorDiaria.ToString("C", culturaBR)} | Valor total estoque (diária): {valorTotalEstoque.ToString("C", culturaBR)}");
        }
    }

    static void ListarAlugueis()
    {
        if (alugueis.Count == 0)
        {
            Console.WriteLine("Nenhum aluguel registrado.");
            return;
        }

        foreach (var a in alugueis)
        {
            Console.WriteLine($"Cliente: {a.Cliente} | Produto: {a.Produto} | Dias: {a.Dias} | Data: {a.Data:dd/MM/yyyy} | Total: {a.ValorTotal.ToString("C", culturaBR)} | {(a.Pago ? "Pago" : "Aberto")} | {(a.Devolvido ? "Devolvido" : "Em uso")}");
        }
    }

    static void AdicionarDivida()
    {
        Console.Write("Pessoa: ");
        string pessoa = Console.ReadLine();

        Console.Write("Descrição: ");
        string descricao = Console.ReadLine();

        Console.Write("Valor total da dívida: ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double valorTotal) || valorTotal < 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Valor total inválido.");
            Console.ResetColor();
            return;
        }

        Console.Write("Valor pago: ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double pago) || pago < 0 || pago > valorTotal)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Valor pago inválido.");
            Console.ResetColor();
            return;
        }

        Console.Write("Data (dd/MM/yyyy): ");
        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Data inválida.");
            Console.ResetColor();
            return;
        }

        Console.Write("Está pago? (s/n): ");
        string quitadoStr = Console.ReadLine().ToLower();
        bool quitado = quitadoStr == "s" || quitadoStr == "sim";

        dividas.Add(new DividaEmpresa
        {
            Pessoa = pessoa,
            Descricao = descricao,
            ValorTotal = valorTotal,
            ValorPago = pago,
            Data = data,
            Pago = quitado
        });

        SalvarTudo();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Dívida registrada e salva!");
        Console.ResetColor();
    }

    static void ListarDividas()
    {
        if (dividas.Count == 0)
        {
            Console.WriteLine("Nenhuma dívida registrada.");
            return;
        }

        double total = 0;
        double totalPago = 0;

        var linhas = new List<string>();

        foreach (var d in dividas.OrderBy(d => d.Data))
        {
            linhas.Add("----------------------------------");
            linhas.Add($"Pessoa: {d.Pessoa}");
            linhas.Add($"Descrição: {d.Descricao}");
            linhas.Add($"Data: {d.Data:dd/MM/yyyy}");
            linhas.Add($"Valor Total: {d.ValorTotal.ToString("C", culturaBR)}");
            linhas.Add($"Valor Pago:  {d.ValorPago.ToString("C", culturaBR)}");
            linhas.Add($"Status: {(d.Pago ? "Pago" : "Em aberto")}");

            total += d.ValorTotal;
            totalPago += d.ValorPago;
        }

        double totalAberto = total - totalPago;

        linhas.Add("");
        linhas.Add("===== RESUMO DAS DÍVIDAS =====");
        linhas.Add($"Total de Dívidas:   {total.ToString("C", culturaBR)}");
        linhas.Add($"Total Pago:         {totalPago.ToString("C", culturaBR)}");
        linhas.Add($"Total em Aberto:    {totalAberto.ToString("C", culturaBR)}");

        foreach (var linha in linhas)
            Console.WriteLine(linha);

        File.WriteAllLines(Path.Combine(pastaDados, "dividas_empresa.txt"), linhas);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nRelatório salvo em data/dividas_empresa.txt");
        Console.ResetColor();
    }

    static void RegistrarPagamento()
    {
        Console.Write("Pessoa: ");
        string pessoa = Console.ReadLine();

        var divida = dividas.FirstOrDefault(d => d.Pessoa.Equals(pessoa, StringComparison.OrdinalIgnoreCase) && !d.Pago);
        if (divida == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Dívida não encontrada ou já quitada.");
            Console.ResetColor();
            return;
        }

        Console.Write("Valor do pagamento: ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double valor) || valor <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Valor inválido.");
            Console.ResetColor();
            return;
        }

        divida.ValorPago += valor;
        if (divida.ValorPago >= divida.ValorTotal)
            divida.Pago = true;

        SalvarTudo();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Pagamento registrado e salvo!");
        Console.ResetColor();
    }

    static void ListarDividasMobile()
    {
        if (dividas.Count == 0)
        {
            Console.WriteLine("Nenhuma dívida registrada.");
            return;
        }

        var linhas = new List<string>();
        double totalPagas = 0;
        double totalAberto = 0;

        foreach (var d in dividas.OrderBy(x => x.Data))
        {
            linhas.Add($"Pessoa: {d.Pessoa}");
            linhas.Add($"Data: {d.Data:dd/MM/yyyy}");
            linhas.Add($"Descrição: {d.Descricao}");
            linhas.Add($"Valor Pago: {d.ValorPago.ToString("C", culturaBR)}");
            linhas.Add($"Status: {(d.Pago ? "Pago" : "Em aberto")}");
            linhas.Add("----------------------------------");

            if (d.Pago) totalPagas += d.ValorTotal;
            else totalAberto += d.ValorTotal;
        }

        double totalGeral = totalPagas + totalAberto;

        linhas.Add("== RESUMO ==");
        linhas.Add($"Total Pagas:     {totalPagas.ToString("C", culturaBR)}");
        linhas.Add($"Total Em Aberto: {totalAberto.ToString("C", culturaBR)}");
        linhas.Add($"TOTAL GERAL:     {totalGeral.ToString("C", culturaBR)}");

        File.WriteAllLines(Path.Combine(pastaDados, "dividas_mobile.txt"), linhas);
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
            $"Total Aluguéis:     {totalAlugueis.ToString("C", culturaBR)}",
            $"Recebidos:          {recebidos.ToString("C", culturaBR)}",
            $"Em Aberto:          {emAberto.ToString("C", culturaBR)}",
            $"Total Dívidas:      {totalDividas.ToString("C", culturaBR)}",
            $"Dívidas Pagas:      {dividasPagas.ToString("C", culturaBR)}",
            $"Dívidas em Aberto:  {(totalDividas - dividasPagas).ToString("C", culturaBR)}",
            $"Saldo Final:        {(recebidos - dividasPagas).ToString("C", culturaBR)}"
        };

        File.WriteAllLines(Path.Combine(pastaDados, "resumo_financeiro.txt"), resumo);
        foreach (var l in resumo) Console.WriteLine(l);
    }

    static void CarregarProdutos()
    {
        if (!File.Exists(caminhoProdutos)) return;
        foreach (var linha in File.ReadAllLines(caminhoProdutos))
        {
            var x = linha.Split(';');
            if (x.Length < 3) continue;
            if (!int.TryParse(x[1], out int qtd)) continue;
            if (!double.TryParse(x[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double diaria)) continue;

            produtos.Add(new Produto { Nome = x[0], QuantidadeDisponivel = qtd, ValorDiaria = diaria });
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
            if (x.Length < 7) continue;
            if (!int.TryParse(x[2], out int dias)) continue;
            if (!double.TryParse(x[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double valorDiaria)) continue;
            if (!bool.TryParse(x[4], out bool pago)) continue;
            if (!DateTime.TryParseExact(x[5], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data)) continue;
            if (!bool.TryParse(x[6], out bool devolvido)) continue;

            alugueis.Add(new Aluguel
            {
                Cliente = x[0],
                Produto = x[1],
                Dias = dias,
                ValorDiaria = valorDiaria,
                Pago = pago,
                Data = data,
                Devolvido = devolvido
            });
        }
    }

    static void SalvarAlugueis()
    {
        using var sw = new StreamWriter(caminhoAlugueis);
        foreach (var a in alugueis)
            sw.WriteLine($"{a.Cliente};{a.Produto};{a.Dias};{a.ValorDiaria.ToString(CultureInfo.InvariantCulture)};{a.Pago};{a.Data:yyyy-MM-dd};{a.Devolvido}");
    }

    static void CarregarDividas()
    {
        if (!File.Exists(caminhoDividas)) return;
        foreach (var linha in File.ReadAllLines(caminhoDividas))
        {
            var x = linha.Split(';');
            if (x.Length < 6) continue;
            if (!double.TryParse(x[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double valorTotal)) continue;
            if (!double.TryParse(x[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double valorPago)) continue;
            if (!DateTime.TryParseExact(x[4], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data)) continue;
            if (!bool.TryParse(x[5], out bool pago)) continue;

            dividas.Add(new DividaEmpresa
            {
                Pessoa = x[0],
                Descricao = x[1],
                ValorTotal = valorTotal,
                ValorPago = valorPago,
                Data = data,
                Pago = pago
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