import { Button } from '../../../../components/Button'
import { DataTable } from '../../../../components/DataTable'
import { IconeLixeira } from '../../../../components/Icons'
import { Input } from '../../../../components/Input'
import { Modal } from '../../../../components/Modal'
import { Select } from '../../../../components/Select'
import { OPCOES_TIPO_OPERACAO, TIPOS_OPERACAO } from '../../constants/tiposOperacao'

const moeda = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

const numero = new Intl.NumberFormat('pt-BR', {
  minimumFractionDigits: 2,
  maximumFractionDigits: 8,
})

function formatarData(data) {
  return new Date(data).toLocaleDateString('pt-BR', { timeZone: 'UTC' })
}

export function PainelOperacoesAtivo({
  ativoSelecionado,
  operacoes,
  formulario,
  carregando,
  salvando,
  fecharPainel,
  atualizarCampo,
  adicionarOperacao,
  removerOperacao,
}) {
  const ativo = ativoSelecionado

  const colunas = [
    {
      chave: 'data',
      titulo: 'Data',
      classeCelula: 'text-slate-300',
      render: (operacao) => formatarData(operacao.data),
    },
    {
      chave: 'tipo',
      titulo: 'Tipo',
      classeCelula: (operacao) => (operacao.tipo === 0 ? 'text-emerald-300' : 'text-red-300'),
      render: (operacao) => TIPOS_OPERACAO[operacao.tipo] ?? 'Operação',
    },
    {
      chave: 'quantidade',
      titulo: 'Qtd',
      classeCelula: 'tabular-nums text-slate-300',
      render: (operacao) => numero.format(operacao.quantidade),
    },
    {
      chave: 'precoUnitario',
      titulo: 'Preço',
      classeCelula: 'tabular-nums text-slate-300',
      render: (operacao) => moeda.format(operacao.precoUnitario),
    },
    {
      chave: 'taxas',
      titulo: 'Taxas',
      classeCelula: 'tabular-nums text-slate-300',
      render: (operacao) => moeda.format(operacao.taxas),
    },
    {
      chave: 'valorLiquido',
      titulo: 'Total',
      classeCelula: 'tabular-nums font-medium text-white',
      render: (operacao) => moeda.format(operacao.valorLiquido),
    },
    {
      chave: 'acoes',
      titulo: '',
      render: (operacao) => (
        <Button
          variante="danger"
          onClick={() => removerOperacao(operacao)}
          aria-label="Remover operação"
          title="Remover operação"
          className="p-1"
        >
          <IconeLixeira />
        </Button>
      ),
    },
  ]

  return (
    <Modal aberto={Boolean(ativo)} onFechar={fecharPainel} tamanho="xl" titulo={ativo ? `Operações - ${ativo.ticker}` : 'Operações'}>
      {ativo && (
        <div className="space-y-6">
          <form noValidate onSubmit={adicionarOperacao} className="grid gap-x-4 gap-y-6 sm:grid-cols-2 lg:grid-cols-6">
            <Select rotulo="Tipo" id="tipoOperacao" value={formulario.tipo} onChange={atualizarCampo('tipo')}>
              {OPCOES_TIPO_OPERACAO.map((tipo) => (
                <option key={tipo.valor} value={tipo.valor}>
                  {tipo.rotulo}
                </option>
              ))}
            </Select>

            <Input rotulo="Data" id="dataOperacao" type="date" value={formulario.data} onChange={atualizarCampo('data')} />

            <Input
              rotulo="Quantidade"
              id="quantidadeOperacao"
              type="number"
              step="0.0001"
              min="0"
              value={formulario.quantidade}
              onChange={atualizarCampo('quantidade')}
            />

            <Input
              rotulo="Preço unitário"
              id="precoUnitarioOperacao"
              type="number"
              step="0.01"
              min="0"
              value={formulario.precoUnitario}
              onChange={atualizarCampo('precoUnitario')}
            />

            <Input
              rotulo="Taxas"
              id="taxasOperacao"
              type="number"
              step="0.01"
              min="0"
              value={formulario.taxas}
              onChange={atualizarCampo('taxas')}
            />

            <div className="flex flex-col justify-end">
              <Button type="submit" disabled={salvando} className="w-full">
                {salvando ? 'Salvando...' : 'Registrar'}
              </Button>
            </div>
          </form>

          <DataTable
            titulo="Histórico"
            colunas={colunas}
            itens={operacoes}
            obterChaveLinha={(operacao) => operacao.id}
            carregando={carregando}
            estadoCarregando="Carregando operações..."
            estadoVazio="Nenhuma operação registrada para este ativo."
            minWidthClasse="min-w-[760px]"
          />
        </div>
      )}
    </Modal>
  )
}
