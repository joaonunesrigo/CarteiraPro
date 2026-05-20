import { EmptyState } from '../../../../components/EmptyState'
import { Panel } from '../../../../components/Panel'
import { GraficoAlocacao } from './GraficoAlocacao'
import { GraficoAlocacaoPorTipo } from './GraficoAlocacaoPorTipo'
import { GraficoRentabilidade } from './GraficoRentabilidade'
import { GraficoValores } from './GraficoValores'

export function CarteiraGraficos({ dadosGraficos }) {
  if (!dadosGraficos.temDados) {
    return (
      <EmptyState>
        Adicione ativos na aba Carteira para visualizar os gráficos.
      </EmptyState>
    )
  }

  return (
    <section className="grid gap-4 lg:grid-cols-2">
      <Panel titulo="Alocação por ativo">
        <div className="p-4">
          <GraficoAlocacao dados={dadosGraficos.alocacao} />
          <ul className="mt-4 flex flex-wrap justify-center gap-x-4 gap-y-2">
            {dadosGraficos.alocacao.map((item) => (
              <li
                key={item.ticker}
                className="flex items-center gap-2 text-xs text-slate-300"
              >
                <span
                  className="h-2.5 w-2.5 rounded-full"
                  style={{ backgroundColor: item.cor }}
                />
                {item.ticker} ({item.percentual}%)
              </li>
            ))}
          </ul>
        </div>
      </Panel>

      <Panel titulo="Alocação por tipo de ativo">
        <div className="p-4">
          <GraficoAlocacaoPorTipo dados={dadosGraficos.alocacaoPorTipo} />
          <ul className="mt-4 flex flex-wrap justify-center gap-x-4 gap-y-2">
            {dadosGraficos.alocacaoPorTipo.map((item) => (
              <li
                key={item.tipo}
                className="flex items-center gap-2 text-xs text-slate-300"
              >
                <span
                  className="h-2.5 w-2.5 rounded-full"
                  style={{ backgroundColor: item.cor }}
                />
                {item.rotulo} ({item.percentual}%)
              </li>
            ))}
          </ul>
        </div>
      </Panel>

      <Panel titulo="Investido vs valor atual">
        <div className="p-4 pb-2">
          <GraficoValores dados={dadosGraficos.comparativo} />
          <div className="mt-2 flex justify-center gap-6 text-xs text-slate-400">
            <span className="flex items-center gap-2">
              <span className="h-2.5 w-2.5 rounded-sm bg-slate-500" />
              Investido
            </span>
            <span className="flex items-center gap-2">
              <span className="h-2.5 w-2.5 rounded-sm bg-violet-500" />
              Atual
            </span>
          </div>
          </div>
      </Panel>

      <Panel titulo="Rentabilidade por ativo (%)" className="lg:col-span-2">
        <div className="p-4">
          <GraficoRentabilidade dados={dadosGraficos.rentabilidade} />
        </div>
      </Panel>
    </section>
  )
}
