import { Button } from '../../../../components/Button'
import { EmptyState } from '../../../../components/EmptyState'
import { Panel } from '../../../../components/Panel'
import { SignedValue } from '../../../../components/SignedValue'

export function AtivoTable({ linhas, excluirAtivo }) {
  if (!linhas.length) {
    return <EmptyState>Nenhum ativo na carteira.</EmptyState>
  }

  return (
    <Panel titulo="Ativos">
      <div className="overflow-x-auto">
        <table className="w-full min-w-[640px] text-left text-sm">
          <thead className="bg-slate-900 text-slate-400">
            <tr>
              <th className="px-4 py-3 font-medium">Ticker</th>
              <th className="px-4 py-3 font-medium">Qtd</th>
              <th className="px-4 py-3 font-medium">Preço médio</th>
              <th className="px-4 py-3 font-medium">Cotação</th>
              <th className="px-4 py-3 font-medium">Rent. %</th>
              <th className="px-4 py-3 font-medium">Rent. R$</th>
              <th className="px-4 py-3 font-medium" />
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-800 bg-slate-900/30">
            {linhas.map((ativo) => (
              <tr key={ativo.ticker} className="text-slate-200">
                <td className="px-4 py-3 font-medium">{ativo.ticker}</td>
                <td className="px-4 py-3 tabular-nums">{ativo.quantidade}</td>
                <td className="px-4 py-3 tabular-nums">
                  {ativo.precoMedioFormatado}
                </td>
                <td className="px-4 py-3 tabular-nums">
                  {ativo.cotacaoFormatada}
                </td>
                <SignedValue positivo={ativo.rentabilidadePercentPositiva}>
                  {ativo.rentabilidadePercentFormatada}
                </SignedValue>
                <SignedValue positivo={ativo.rentabilidadeReaisPositiva}>
                  {ativo.rentabilidadeReaisFormatada}
                </SignedValue>
                <td className="px-4 py-3">
                  <Button
                    variante="danger"
                    onClick={() => excluirAtivo(ativo.id)}
                  >
                    Excluir
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </Panel>
  )
}
