import { Button } from '../../../../components/Button'
import { DataTable } from '../../../../components/DataTable'
import { IconeLixeira } from '../../../../components/Icons'

export function AtivoTable({
  linhas,
  excluirAtivo,
  excluirTodosAtivos,
  abrirPainelOperacoes,
}) {
  const colunas = [
    {
      chave: 'ticker',
      titulo: 'Ticker',
      classeCelula: 'font-medium',
      render: (ativo) => ativo.ticker,
    },
    {
      chave: 'quantidade',
      titulo: 'Qtd',
      classeCelula: 'tabular-nums',
      render: (ativo) => ativo.quantidade,
    },
    {
      chave: 'precoMedio',
      titulo: 'Preço médio',
      classeCelula: 'tabular-nums',
      render: (ativo) => ativo.precoMedioFormatado,
    },
    {
      chave: 'cotacao',
      titulo: 'Cotação',
      classeCelula: 'tabular-nums',
      render: (ativo) => ativo.cotacaoFormatada,
    },
    {
      chave: 'rentabilidadePercent',
      titulo: 'Rent. %',
      classeCelula: (ativo) =>
        `tabular-nums ${
          ativo.rentabilidadePercentPositiva
            ? 'text-emerald-400'
            : 'text-red-400'
        }`,
      render: (ativo) => ativo.rentabilidadePercentFormatada,
    },
    {
      chave: 'rentabilidadeReais',
      titulo: 'Rent. R$',
      classeCelula: (ativo) =>
        `tabular-nums ${
          ativo.rentabilidadeReaisPositiva
            ? 'text-emerald-400'
            : 'text-red-400'
        }`,
      render: (ativo) => ativo.rentabilidadeReaisFormatada,
    },
    {
      chave: 'acoes',
      titulo: '',
      render: (ativo) => (
        <div className="flex flex-wrap items-center gap-3">
          <Button variante="secondary" onClick={() => abrirPainelOperacoes(ativo)}>
            Operações
          </Button>
          <Button
            variante="danger"
            onClick={() => excluirAtivo(ativo.id, ativo.ticker)}
            aria-label={`Excluir ${ativo.ticker}`}
            title="Excluir ativo"
            className="p-1"
          >
            <IconeLixeira />
          </Button>
        </div>
      ),
    },
  ]

  return (
    <DataTable
      titulo="Ativos"
      acao={
        linhas.length > 0 && (
          <Button
            variante="perigo"
            type="button"
            onClick={() => excluirTodosAtivos(linhas.length)}
          >
            Excluir todos
          </Button>
        )
      }
      colunas={colunas}
      itens={linhas}
      obterChaveLinha={(ativo) => ativo.id ?? ativo.ticker}
      estadoVazio="Nenhum ativo na carteira."
    />
  )
}
