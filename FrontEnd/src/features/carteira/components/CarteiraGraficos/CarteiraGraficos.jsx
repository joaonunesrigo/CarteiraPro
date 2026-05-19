import { EmptyState } from '../../../../components/EmptyState'
import { Panel } from '../../../../components/Panel'
import { GraficoAlocacao } from './GraficoAlocacao'
import { GraficoRentabilidade } from './GraficoRentabilidade'
import { GraficoValores } from './GraficoValores'

export function CarteiraGraficos({ dadosGraficos }) {
  if (!dadosGraficos.temDados) {
    return (
      <section className="space-y-4">
        <h2 className="text-lg font-medium text-white">Gráficos</h2>
        <EmptyState>Adicione ativos para visualizar os gráficos.</EmptyState>
      </section>
    )
  }

  return (
    <section className="space-y-4">
      <h2 className="text-lg font-medium text-white">Gráficos</h2>

      <div className="grid gap-4 lg:grid-cols-2">
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
      </div>
    </section>
  )
}
