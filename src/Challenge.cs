// DESAFIO: Sistema de Templates de Documentos
// PROBLEMA: Um sistema de gerenciamento documental precisa criar novos documentos
// baseados em templates pré-configurados complexos (contratos, propostas, relatórios)
// O código atual recria objetos do zero, perdendo muito tempo em inicializações

using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace DesignPatternChallenge;

// Contexto: Sistema que gerencia documentos corporativos com muitas configurações
// Templates são complexos e custosos para criar, mas precisamos gerar muitos documentos similares

public class DocumentTemplate: IPrototype
{
    public string Title { get; set; }
    public string Category { get; set; }
    public List<Section> Sections { get; set; }
    public DocumentStyle Style { get; set; }
    public List<string> RequiredFields { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public ApprovalWorkflow Workflow { get; set; }
    public List<string> Tags { get; set; }

    public DocumentTemplate()
    {
        Sections = new List<Section>();
        RequiredFields = new List<string>();
        Metadata = new Dictionary<string, string>();
        Tags = new List<string>();
    }

    public IPrototype Clone()
    {
        var clone = this.MemberwiseClone() as DocumentTemplate;

        // Deep copy das Sections (copiar cada Section individualmente)
        clone.Sections = new List<Section>();
        foreach (var section in this.Sections)
        {
            clone.Sections.Add(new Section
            {
                Name = section.Name,
                Content = section.Content,
                IsEditable = section.IsEditable,
                Placeholders = new List<string>(section.Placeholders)
            });
        }

        // Deep copy do Style
        clone.Style = new DocumentStyle()
        {
            FontFamily = this.Style.FontFamily,
            FontSize = this.Style.FontSize,
            HeaderColor = this.Style.HeaderColor,
            LogoUrl = this.Style.LogoUrl,
            PageMargins = new Margins
            {
                Top = this.Style.PageMargins.Top,
                Bottom = this.Style.PageMargins.Bottom,
                Left = this.Style.PageMargins.Left,
                Right = this.Style.PageMargins.Right
            }
        };

        // Deep copy do Workflow
        clone.Workflow = new ApprovalWorkflow()
        {
            RequiredApprovals = this.Workflow.RequiredApprovals,
            TimeoutDays = this.Workflow.TimeoutDays,
            Approvers = new List<string>(this.Workflow.Approvers)
        };

        // Deep copy das listas e dicionário
        clone.RequiredFields = new List<string>(this.RequiredFields);
        clone.Tags = new List<string>(this.Tags);
        clone.Metadata = new Dictionary<string, string>(this.Metadata);

        return clone;
    }
}

public class Section
{
    public string Name { get; set; }
    public string Content { get; set; }
    public bool IsEditable { get; set; }
    public List<string> Placeholders { get; set; }

    public Section()
    {
        Placeholders = new List<string>();
    }
}

public class DocumentStyle
{
    public string FontFamily { get; set; }
    public int FontSize { get; set; }
    public string HeaderColor { get; set; }
    public string LogoUrl { get; set; }
    public Margins PageMargins { get; set; }
}

public class Margins
{
    public int Top { get; set; }
    public int Bottom { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
}

public class ApprovalWorkflow
{
    public List<string> Approvers { get; set; }
    public int RequiredApprovals { get; set; }
    public int TimeoutDays { get; set; }

    public ApprovalWorkflow()
    {
        Approvers = new List<string>();
    }
}


public interface IPrototype
{
    public IPrototype Clone();
}

public class DocumentService
{
    // Problema: Criação manual de templates complexos repetidamente
    public DocumentTemplate CreateServiceContract()
    {
        Console.WriteLine("Criando template de Contrato de Serviço do zero...");
        
        // Simulando processo custoso de inicialização
        System.Threading.Thread.Sleep(100);
        
        var template = new DocumentTemplate
        {
            Title = "Contrato de Prestação de Serviços",
            Category = "Contratos",
            Style = new DocumentStyle
            {
                FontFamily = "Arial",
                FontSize = 12,
                HeaderColor = "#003366",
                LogoUrl = "https://company.com/logo.png",
                PageMargins = new Margins { Top = 2, Bottom = 2, Left = 3, Right = 3 }
            },
            Workflow = new ApprovalWorkflow
            {
                RequiredApprovals = 2,
                TimeoutDays = 5
            }
        };

        template.Workflow.Approvers.Add("gerente@empresa.com");
        template.Workflow.Approvers.Add("juridico@empresa.com");

        template.Sections.Add(new Section
        {
            Name = "Cláusula 1 - Objeto",
            Content = "O presente contrato tem por objeto...",
            IsEditable = true
        });
        template.Sections.Add(new Section
        {
            Name = "Cláusula 2 - Prazo",
            Content = "O prazo de vigência será de...",
            IsEditable = true
        });
        template.Sections.Add(new Section
        {
            Name = "Cláusula 3 - Valor",
            Content = "O valor total do contrato é de...",
            IsEditable = true
        });

        template.RequiredFields.Add("NomeCliente");
        template.RequiredFields.Add("CPF");
        template.RequiredFields.Add("Endereco");

        template.Tags.Add("contrato");
        template.Tags.Add("servicos");

        template.Metadata["Versao"] = "1.0";
        template.Metadata["Departamento"] = "Comercial";
        template.Metadata["UltimaRevisao"] = DateTime.Now.ToString();

        return template;
    }

    // Solução: Usando Clone para criar variação do template base
    public DocumentTemplate CreateConsultingContract(DocumentTemplate baseContract)
    {
        Console.WriteLine("Criando template de Contrato de Consultoria usando Clone...");

        var template2 = (DocumentTemplate)baseContract.Clone();
        template2.Title = "Contrato de Consultoria";
        if (template2.Sections.Count > 2)
        {
            template2.Sections.RemoveAt(2); // Removendo a cláusula de valor, que não se aplica a consultoria
        }
        if (template2.Tags.Count > 1)
        {
            template2.Tags[1] = "consultoria";
        }

        return template2;

    }

    public void DisplayTemplate(DocumentTemplate template)
    {
        Console.WriteLine($"\n=== {template.Title} ===");
        Console.WriteLine($"Categoria: {template.Category}");
        Console.WriteLine($"Seções: {template.Sections.Count}");
        Console.WriteLine($"Campos obrigatórios: {string.Join(", ", template.RequiredFields)}");
        Console.WriteLine($"Aprovadores: {string.Join(", ", template.Workflow.Approvers)}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Sistema de Templates de Documentos ===\n");

        var service = new DocumentService();

        // Problema: Precisamos criar 10 contratos de serviço
        // Cada um é criado do zero, mesmo sendo idênticos no início
        Console.WriteLine("Criando 5 contratos de serviço...");
        var startTime = DateTime.Now;
        var contract = service.CreateServiceContract();

        for (int i = 1; i < 5; i++)
        {
            var newContract = (DocumentTemplate)contract.Clone();
            // Depois modificamos apenas dados específicos do cliente
            newContract.Title = $"Contrato #{i} - Cliente {i}";
        }
        
        var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
        Console.WriteLine($"Tempo total: {elapsed}ms\n");

        // Solução: Usando Clone para criar templates similares sem recriar do zero

        var consultingContract = service.CreateConsultingContract(contract);
        service.DisplayTemplate(consultingContract);

        // Perguntas para reflexão:
        // - Como evitar recriar objetos complexos do zero?
        // - Como clonar um objeto mantendo toda sua estrutura e configurações?
        // - Como criar cópias profundas (deep copy) de objetos com referências?
        // - Como permitir personalização após clonagem?
    }
}
